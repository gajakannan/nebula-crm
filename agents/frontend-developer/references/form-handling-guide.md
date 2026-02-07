# Form Handling Guide (React Hook Form + Zod)

**Version:** 2.0
**Last Updated:** 2026-01-30
**Applies To:** Frontend Developer

---

## Overview

This guide covers form handling using **React Hook Form** for form state management and **Zod** for schema validation. This combination provides excellent TypeScript support, performance, and developer experience.

**Philosophy:** Forms should validate as users type, provide clear error messages, and match backend validation rules exactly.

---

## When to Use This Approach vs JSON Schema

**Use React Hook Form + Zod when:**
- Form structure is **static** and defined in code
- You control the form schema (not driven by backend config)
- You want excellent **TypeScript inference**
- Form is simple to moderate complexity
- Examples: Login forms, settings forms, simple CRUD forms

**Use JSON Schema + AJV + RJSF when:**
- Forms are **dynamically generated** from backend/database
- Form structure varies by region, category, or other business rules
- Need **backend/frontend validation parity** (same schema validates both)
- Schema is stored in database or config files
- Examples: Dynamic order forms (region-specific), dynamic product configurators

**Recommendation:** Use JSON Schema + AJV for dynamic order forms (see `json-schema-forms-guide.md`), but React Hook Form + Zod is perfect for simpler forms like user settings, filters, search forms, etc.

---

---

## Installation

```bash
npm install react-hook-form zod @hookform/resolvers
```

---

## Basic Form

### Simple Form with Zod Validation

```tsx
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

// 1. Define Zod schema
const customerSchema = z.object({
  name: z.string().min(1, 'Name is required').max(100),
  email: z.string().email('Invalid email address'),
  phone: z.string().regex(/^\d{10}$/, 'Phone must be 10 digits'),
  region: z.string().min(1, 'Region is required'),
});

// 2. Infer TypeScript type from schema
type CustomerFormData = z.infer<typeof customerSchema>;

// 3. Create form component
function CreateCustomerForm() {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<CustomerFormData>({
    resolver: zodResolver(customerSchema),
  });

  const onSubmit = async (data: CustomerFormData) => {
    try {
      await createCustomer(data);
      toast.success('Customer created successfully');
    } catch (error) {
      toast.error('Failed to create customer');
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <Label htmlFor="name">Customer Name</Label>
        <Input
          id="name"
          {...register('name')}
          error={errors.name?.message}
        />
      </div>

      <div>
        <Label htmlFor="email">Email</Label>
        <Input
          id="email"
          type="email"
          {...register('email')}
          error={errors.email?.message}
        />
      </div>

      <div>
        <Label htmlFor="phone">Phone</Label>
        <Input
          id="phone"
          {...register('phone')}
          error={errors.phone?.message}
        />
      </div>

      <div>
        <Label htmlFor="region">Region</Label>
        <Input
          id="region"
          {...register('region')}
          error={errors.region?.message}
        />
      </div>

      <Button type="submit" disabled={isSubmitting}>
        {isSubmitting ? 'Creating...' : 'Create Customer'}
      </Button>
    </form>
  );
}
```

---

## Zod Schema Patterns

### Common Field Validations

```tsx
import { z } from 'zod';

// Required string
z.string().min(1, 'This field is required')

// Optional string
z.string().optional()

// String with length constraints
z.string().min(3, 'Minimum 3 characters').max(100, 'Maximum 100 characters')

// Email
z.string().email('Invalid email address')

// URL
z.string().url('Invalid URL')

// Phone (10 digits)
z.string().regex(/^\d{10}$/, 'Phone must be 10 digits')

// Order number (alphanumeric)
z.string().regex(/^[A-Z0-9]+$/, 'Order number must be alphanumeric')

// Numbers
z.number().min(0, 'Must be positive')
z.number().int('Must be an integer')

// Dates
z.date()
z.string().datetime() // ISO 8601 string
z.coerce.date() // Convert string to Date

// Booleans
z.boolean()

// Enums
z.enum(['pending', 'approved', 'rejected'])

// Arrays
z.array(z.string()).min(1, 'At least one item required')

// Objects
z.object({
  name: z.string(),
  age: z.number(),
})

// Nullable/Optional
z.string().nullable()
z.string().optional()
z.string().nullish() // nullable + optional
```

### Domain Schemas

```tsx
// Customer creation
export const createCustomerSchema = z.object({
  name: z.string().min(1, 'Customer name is required').max(200),
  email: z.string().email('Invalid email'),
  phone: z.string().regex(/^\d{10}$/, 'Phone must be 10 digits'),
  region: z.string().min(1, 'Region is required'),
  website: z.string().url('Invalid URL').optional(),
  notes: z.string().max(1000).optional(),
});

// Order creation
export const createOrderSchema = z.object({
  customerId: z.string().uuid('Invalid customer'),
  orderDate: z.coerce.date().min(new Date(), 'Date must be in future'),
  dueDate: z.coerce.date(),
  amount: z.number().min(0, 'Amount must be positive'),
  category: z.enum(['electronics', 'clothing', 'furniture']),
  notes: z.string().max(1000).optional(),
  attachments: z.array(z.string().url()).optional(),
}).refine(
  (data) => data.dueDate > data.orderDate,
  {
    message: 'Due date must be after order date',
    path: ['dueDate'],
  }
);

// Address creation
export const createAddressSchema = z.object({
  street: z.string().min(1, 'Street is required'),
  city: z.string().min(1, 'City is required'),
  region: z.string().min(1, 'Region is required'),
  postalCode: z.string().min(1, 'Postal code is required'),
  isPrimary: z.boolean().default(false),
});
```

### Schema Composition

```tsx
// Base schemas
const addressSchema = z.object({
  street: z.string().min(1, 'Street is required'),
  city: z.string().min(1, 'City is required'),
  region: z.string().min(1, 'Region is required'),
  postalCode: z.string().min(1, 'Postal code is required'),
});

const contactInfoSchema = z.object({
  email: z.string().email(),
  phone: z.string().regex(/^\d{10}$/),
});

// Compose into larger schema
const customerWithAddressSchema = createCustomerSchema.merge(
  z.object({
    mailingAddress: addressSchema,
    billingAddress: addressSchema.optional(),
  })
).merge(contactInfoSchema);
```

### Conditional Validation

```tsx
const orderSchema = z.object({
  category: z.enum(['standard', 'express']),
  region: z.string().min(1),
  expressPartner: z.string().optional(),
}).superRefine((data, ctx) => {
  // If express, partner is required
  if (data.category === 'express' && !data.expressPartner) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: 'Express partner is required for express orders',
      path: ['expressPartner'],
    });
  }
});
```

---

## shadcn/ui Form Components

### Using Form Component

```tsx
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';

function CustomerForm() {
  const form = useForm<CustomerFormData>({
    resolver: zodResolver(customerSchema),
    defaultValues: {
      name: '',
      email: '',
      region: '',
    },
  });

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Customer Name</FormLabel>
              <FormControl>
                <Input placeholder="Acme Corporation" {...field} />
              </FormControl>
              <FormDescription>
                Legal name of the organization
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Email</FormLabel>
              <FormControl>
                <Input placeholder="contact@example.com" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <Button type="submit">Submit</Button>
      </form>
    </Form>
  );
}
```

### Select/Dropdown

```tsx
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

<FormField
  control={form.control}
  name="region"
  render={({ field }) => (
    <FormItem>
      <FormLabel>Region</FormLabel>
      <Select onValueChange={field.onChange} defaultValue={field.value}>
        <FormControl>
          <SelectTrigger>
            <SelectValue placeholder="Select a region" />
          </SelectTrigger>
        </FormControl>
        <SelectContent>
          <SelectItem value="US-West">US West</SelectItem>
          <SelectItem value="US-East">US East</SelectItem>
          <SelectItem value="EU-West">EU West</SelectItem>
        </SelectContent>
      </Select>
      <FormMessage />
    </FormItem>
  )}
/>
```

### Checkbox

```tsx
import { Checkbox } from '@/components/ui/checkbox';

<FormField
  control={form.control}
  name="isPrimary"
  render={({ field }) => (
    <FormItem className="flex items-center space-x-2">
      <FormControl>
        <Checkbox
          checked={field.value}
          onCheckedChange={field.onChange}
        />
      </FormControl>
      <FormLabel className="!mt-0">Primary Address</FormLabel>
      <FormMessage />
    </FormItem>
  )}
/>
```

### Radio Group

```tsx
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';

<FormField
  control={form.control}
  name="category"
  render={({ field }) => (
    <FormItem>
      <FormLabel>Category</FormLabel>
      <FormControl>
        <RadioGroup
          onValueChange={field.onChange}
          defaultValue={field.value}
          className="flex flex-col space-y-1"
        >
          <FormItem className="flex items-center space-x-2">
            <FormControl>
              <RadioGroupItem value="standard" />
            </FormControl>
            <FormLabel className="!mt-0">Standard</FormLabel>
          </FormItem>
          <FormItem className="flex items-center space-x-2">
            <FormControl>
              <RadioGroupItem value="express" />
            </FormControl>
            <FormLabel className="!mt-0">Express</FormLabel>
          </FormItem>
        </RadioGroup>
      </FormControl>
      <FormMessage />
    </FormItem>
  )}
/>
```

### Date Picker

```tsx
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Calendar } from '@/components/ui/calendar';
import { CalendarIcon } from 'lucide-react';
import { format } from 'date-fns';

<FormField
  control={form.control}
  name="orderDate"
  render={({ field }) => (
    <FormItem className="flex flex-col">
      <FormLabel>Order Date</FormLabel>
      <Popover>
        <PopoverTrigger asChild>
          <FormControl>
            <Button
              variant="outline"
              className={cn(
                'pl-3 text-left font-normal',
                !field.value && 'text-muted-foreground'
              )}
            >
              {field.value ? (
                format(field.value, 'PPP')
              ) : (
                <span>Pick a date</span>
              )}
              <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
            </Button>
          </FormControl>
        </PopoverTrigger>
        <PopoverContent className="w-auto p-0" align="start">
          <Calendar
            mode="single"
            selected={field.value}
            onSelect={field.onChange}
            disabled={(date) => date < new Date()}
            initialFocus
          />
        </PopoverContent>
      </Popover>
      <FormMessage />
    </FormItem>
  )}
/>
```

### Combobox (Searchable Select)

```tsx
import { Combobox } from '@/components/ui/combobox';

<FormField
  control={form.control}
  name="customerId"
  render={({ field }) => (
    <FormItem>
      <FormLabel>Customer</FormLabel>
      <Combobox
        options={customers.map(c => ({ label: c.name, value: c.id }))}
        value={field.value}
        onValueChange={field.onChange}
        placeholder="Search customers..."
      />
      <FormMessage />
    </FormItem>
  )}
/>
```

---

## Advanced Patterns

### Multi-Step Forms

```tsx
function CreateOrderWizard() {
  const [step, setStep] = useState(1);

  const form = useForm<OrderFormData>({
    resolver: zodResolver(orderSchema),
    mode: 'onChange', // Validate on change
  });

  const nextStep = async () => {
    // Validate current step fields
    const stepFields = getStepFields(step);
    const isValid = await form.trigger(stepFields);

    if (isValid) {
      setStep(step + 1);
    }
  };

  const onSubmit = async (data: OrderFormData) => {
    await createOrder(data);
    toast.success('Order created');
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        {/* Progress indicator */}
        <StepIndicator currentStep={step} totalSteps={4} />

        {/* Step content */}
        {step === 1 && <BasicInfoStep form={form} />}
        {step === 2 && <DetailsStep form={form} />}
        {step === 3 && <DocumentsStep form={form} />}
        {step === 4 && <ReviewStep form={form} />}

        {/* Navigation */}
        <div className="flex justify-between">
          {step > 1 && (
            <Button
              type="button"
              variant="outline"
              onClick={() => setStep(step - 1)}
            >
              Back
            </Button>
          )}

          {step < 4 ? (
            <Button type="button" onClick={nextStep}>
              Next
            </Button>
          ) : (
            <Button type="submit">Submit</Button>
          )}
        </div>
      </form>
    </Form>
  );
}
```

### Dynamic Field Arrays

```tsx
import { useFieldArray } from 'react-hook-form';

const addressesSchema = z.object({
  addresses: z.array(
    z.object({
      street: z.string().min(1),
      city: z.string().min(1),
      region: z.string().optional(),
    })
  ).min(1, 'At least one address is required'),
});

function AddressesForm() {
  const form = useForm<z.infer<typeof addressesSchema>>({
    resolver: zodResolver(addressesSchema),
    defaultValues: {
      addresses: [{ street: '', city: '', region: '' }],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: 'addresses',
  });

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <div className="space-y-4">
          {fields.map((field, index) => (
            <Card key={field.id}>
              <CardHeader>
                <CardTitle>Address {index + 1}</CardTitle>
                {index > 0 && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    onClick={() => remove(index)}
                  >
                    Remove
                  </Button>
                )}
              </CardHeader>
              <CardContent className="space-y-4">
                <FormField
                  control={form.control}
                  name={`addresses.${index}.street`}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Street</FormLabel>
                      <FormControl>
                        <Input {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name={`addresses.${index}.city`}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>City</FormLabel>
                      <FormControl>
                        <Input {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </CardContent>
            </Card>
          ))}
        </div>

        <Button
          type="button"
          variant="outline"
          onClick={() => append({ street: '', city: '', region: '' })}
        >
          Add Address
        </Button>

        <Button type="submit">Save</Button>
      </form>
    </Form>
  );
}
```

### Dependent Fields

```tsx
function OrderForm() {
  const form = useForm<OrderFormData>({
    resolver: zodResolver(orderSchema),
  });

  // Watch category to show/hide fields
  const category = form.watch('category');

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <FormField
          control={form.control}
          name="category"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Category</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value="standard">Standard</SelectItem>
                  <SelectItem value="express">Express</SelectItem>
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Conditionally show express partner field */}
        {category === 'express' && (
          <FormField
            control={form.control}
            name="expressPartner"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Express Partner</FormLabel>
                <FormControl>
                  <Input {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        )}
      </form>
    </Form>
  );
}
```

### Server-Side Validation

```tsx
function CustomerForm() {
  const form = useForm<CustomerFormData>({
    resolver: zodResolver(customerSchema),
  });

  const onSubmit = async (data: CustomerFormData) => {
    try {
      await createCustomer(data);
      toast.success('Customer created');
    } catch (error) {
      // Handle API validation errors
      if (error.response?.status === 400) {
        const serverErrors = error.response.data.errors;

        // Map server errors to form fields
        Object.entries(serverErrors).forEach(([field, message]) => {
          form.setError(field as keyof CustomerFormData, {
            type: 'server',
            message: message as string,
          });
        });
      } else {
        toast.error('An error occurred');
      }
    }
  };

  return <Form {...form}>{/* ... */}</Form>;
}
```

### Debounced Async Validation

```tsx
const usernameSchema = z.string()
  .min(3)
  .refine(
    async (username) => {
      // Check if username is available
      const response = await checkUsernameAvailability(username);
      return response.available;
    },
    { message: 'Username is already taken' }
  );
```

---

## Form State Management

### Get Field Values

```tsx
const name = form.watch('name'); // Watch single field
const { name, email } = form.watch(); // Watch all fields
```

### Set Field Values

```tsx
form.setValue('name', 'New Value');
form.setValue('name', 'New Value', { shouldValidate: true });
```

### Reset Form

```tsx
form.reset(); // Reset to defaults
form.reset({ name: 'New Default' }); // Reset with new values
```

### Trigger Validation

```tsx
await form.trigger(); // Validate all fields
await form.trigger('name'); // Validate single field
await form.trigger(['name', 'email']); // Validate multiple fields
```

### Get Form State

```tsx
const { isDirty, isValid, isSubmitting, errors } = form.formState;
```

---

## Error Handling

### Display Errors

```tsx
// Field-level errors (handled by FormMessage)
<FormField
  control={form.control}
  name="email"
  render={({ field }) => (
    <FormItem>
      <FormLabel>Email</FormLabel>
      <FormControl>
        <Input {...field} />
      </FormControl>
      <FormMessage /> {/* Shows field error */}
    </FormItem>
  )}
/>

// Form-level errors
{form.formState.errors.root && (
  <Alert variant="destructive">
    <AlertDescription>
      {form.formState.errors.root.message}
    </AlertDescription>
  </Alert>
)}
```

### Custom Error Messages

```tsx
form.setError('email', {
  type: 'custom',
  message: 'Email already exists',
});

form.setError('root', {
  type: 'server',
  message: 'Failed to create customer. Please try again.',
});
```

---

## Best Practices

### 1. Match Backend Validation

**GOOD:** Frontend and backend validation rules are identical
```tsx
// Frontend
const customerSchema = z.object({
  email: z.string().email(),
});

// Backend (.NET)
[EmailAddress]
public string Email { get; set; }
```

### 2. Use TypeScript Inference

**GOOD:**
```tsx
const schema = z.object({ name: z.string() });
type FormData = z.infer<typeof schema>; // Inferred from schema
```

**BAD:**
```tsx
interface FormData { name: string; } // Manually defined
const schema = z.object({ name: z.string() }); // Duplicate definition
```

### 3. Provide Helpful Error Messages

**GOOD:**
```tsx
z.string()
  .min(1, 'Customer name is required')
  .max(200, 'Customer name must be less than 200 characters')
```

**BAD:**
```tsx
z.string().min(1).max(200) // Generic error messages
```

### 4. Use Default Values

**GOOD:**
```tsx
useForm({
  defaultValues: {
    isPrimary: false,
    region: 'US-West',
    category: 'standard',
  },
});
```

### 5. Validate on Blur for Better UX

```tsx
useForm({
  mode: 'onBlur', // Validate when field loses focus
  reValidateMode: 'onChange', // Re-validate on change after first validation
});
```

---

## Testing Forms

### Test Validation

```tsx
import { zodResolver } from '@hookform/resolvers/zod';
import { customerSchema } from './schemas';

describe('Customer Schema', () => {
  it('should reject invalid email', () => {
    const result = customerSchema.safeParse({
      name: 'Test Customer',
      email: 'invalid-email',
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toBe('Invalid email address');
    }
  });

  it('should accept valid data', () => {
    const result = customerSchema.safeParse({
      name: 'Test Customer',
      email: 'test@example.com',
      phone: '5551234567',
      region: 'US-West',
    });

    expect(result.success).toBe(true);
  });
});
```

---

## References

- [React Hook Form Docs](https://react-hook-form.com)
- [Zod Documentation](https://zod.dev)
- [shadcn/ui Form Components](https://ui.shadcn.com/docs/components/form)

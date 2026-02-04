# Form Handling Guide (React Hook Form + Zod)

**Version:** 2.0
**Last Updated:** 2026-01-30
**Applies To:** Frontend Developer

---

## Overview

This guide covers form handling in Nebula using **React Hook Form** for form state management and **Zod** for schema validation. This combination provides excellent TypeScript support, performance, and developer experience.

**Philosophy:** Forms should validate as users type, provide clear error messages, and match backend validation rules exactly.

---

## ⚠️ When to Use This Approach vs JSON Schema

**Use React Hook Form + Zod when:**
- ✅ Form structure is **static** and defined in code
- ✅ You control the form schema (not driven by backend config)
- ✅ You want excellent **TypeScript inference**
- ✅ Form is simple to moderate complexity
- ✅ Examples: Login forms, settings forms, simple CRUD forms

**Use JSON Schema + AJV + RJSF when:**
- ✅ Forms are **dynamically generated** from backend/database
- ✅ Form structure varies by state, coverage type, or other business rules
- ✅ Need **backend/frontend validation parity** (same schema validates both)
- ✅ Schema is stored in database or config files
- ✅ Examples: Insurance submissions (state-specific), dynamic product configurators

**For Nebula:** We recommend JSON Schema + AJV for submission/renewal forms (see `json-schema-forms-guide.md`), but React Hook Form + Zod is perfect for simpler forms like user settings, filters, search forms, etc.

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
const brokerSchema = z.object({
  name: z.string().min(1, 'Name is required').max(100),
  licenseNumber: z.string().min(1, 'License number is required'),
  email: z.string().email('Invalid email address'),
  phone: z.string().regex(/^\d{10}$/, 'Phone must be 10 digits'),
});

// 2. Infer TypeScript type from schema
type BrokerFormData = z.infer<typeof brokerSchema>;

// 3. Create form component
function CreateBrokerForm() {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<BrokerFormData>({
    resolver: zodResolver(brokerSchema),
  });

  const onSubmit = async (data: BrokerFormData) => {
    try {
      await createBroker(data);
      toast.success('Broker created successfully');
    } catch (error) {
      toast.error('Failed to create broker');
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <Label htmlFor="name">Broker Name</Label>
        <Input
          id="name"
          {...register('name')}
          error={errors.name?.message}
        />
      </div>

      <div>
        <Label htmlFor="licenseNumber">License Number</Label>
        <Input
          id="licenseNumber"
          {...register('licenseNumber')}
          error={errors.licenseNumber?.message}
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

      <Button type="submit" disabled={isSubmitting}>
        {isSubmitting ? 'Creating...' : 'Create Broker'}
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

// License number (alphanumeric)
z.string().regex(/^[A-Z0-9]+$/, 'License must be alphanumeric')

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
z.enum(['pending', 'approved', 'declined'])

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

### Insurance Domain Schemas

```tsx
// Broker creation
export const createBrokerSchema = z.object({
  name: z.string().min(1, 'Broker name is required').max(200),
  licenseNumber: z.string().min(1, 'License number is required'),
  licenseState: z.string().length(2, 'State must be 2 letters'),
  email: z.string().email('Invalid email'),
  phone: z.string().regex(/^\d{10}$/, 'Phone must be 10 digits'),
  website: z.string().url('Invalid URL').optional(),
  notes: z.string().max(1000).optional(),
});

// Submission creation
export const createSubmissionSchema = z.object({
  brokerId: z.string().uuid('Invalid broker'),
  accountId: z.string().uuid('Invalid account'),
  effectiveDate: z.coerce.date().min(new Date(), 'Date must be in future'),
  expirationDate: z.coerce.date(),
  premium: z.number().min(0, 'Premium must be positive'),
  coverageType: z.enum(['general-liability', 'property', 'workers-comp']),
  limits: z.object({
    perOccurrence: z.number().min(0),
    aggregate: z.number().min(0),
  }),
  attachments: z.array(z.string().url()).optional(),
}).refine(
  (data) => data.expirationDate > data.effectiveDate,
  {
    message: 'Expiration date must be after effective date',
    path: ['expirationDate'],
  }
);

// Contact creation
export const createContactSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().email('Invalid email'),
  phone: z.string().regex(/^\d{10}$/, 'Phone must be 10 digits').optional(),
  title: z.string().optional(),
  isPrimary: z.boolean().default(false),
});
```

### Schema Composition

```tsx
// Base schemas
const addressSchema = z.object({
  street: z.string().min(1, 'Street is required'),
  city: z.string().min(1, 'City is required'),
  state: z.string().length(2, 'State must be 2 letters'),
  zip: z.string().regex(/^\d{5}(-\d{4})?$/, 'Invalid ZIP code'),
});

const contactSchema = z.object({
  email: z.string().email(),
  phone: z.string().regex(/^\d{10}$/),
});

// Compose into larger schema
const brokerWithAddressSchema = createBrokerSchema.merge(
  z.object({
    mailingAddress: addressSchema,
    billingAddress: addressSchema.optional(),
  })
).merge(contactSchema);
```

### Conditional Validation

```tsx
const submissionSchema = z.object({
  coverageType: z.enum(['admitted', 'surplus']),
  state: z.string().length(2),
  surplusLinesBroker: z.string().optional(),
}).superRefine((data, ctx) => {
  // If surplus lines, broker is required
  if (data.coverageType === 'surplus' && !data.surplusLinesBroker) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: 'Surplus lines broker is required for surplus coverage',
      path: ['surplusLinesBroker'],
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

function BrokerForm() {
  const form = useForm<BrokerFormData>({
    resolver: zodResolver(brokerSchema),
    defaultValues: {
      name: '',
      licenseNumber: '',
      email: '',
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
              <FormLabel>Broker Name</FormLabel>
              <FormControl>
                <Input placeholder="ABC Insurance Brokers" {...field} />
              </FormControl>
              <FormDescription>
                Legal name of the brokerage
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="licenseNumber"
          render={({ field }) => (
            <FormItem>
              <FormLabel>License Number</FormLabel>
              <FormControl>
                <Input placeholder="0A12345" {...field} />
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
  name="state"
  render={({ field }) => (
    <FormItem>
      <FormLabel>State</FormLabel>
      <Select onValueChange={field.onChange} defaultValue={field.value}>
        <FormControl>
          <SelectTrigger>
            <SelectValue placeholder="Select a state" />
          </SelectTrigger>
        </FormControl>
        <SelectContent>
          <SelectItem value="CA">California</SelectItem>
          <SelectItem value="NY">New York</SelectItem>
          <SelectItem value="TX">Texas</SelectItem>
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
      <FormLabel className="!mt-0">Primary Contact</FormLabel>
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
  name="coverageType"
  render={({ field }) => (
    <FormItem>
      <FormLabel>Coverage Type</FormLabel>
      <FormControl>
        <RadioGroup
          onValueChange={field.onChange}
          defaultValue={field.value}
          className="flex flex-col space-y-1"
        >
          <FormItem className="flex items-center space-x-2">
            <FormControl>
              <RadioGroupItem value="admitted" />
            </FormControl>
            <FormLabel className="!mt-0">Admitted</FormLabel>
          </FormItem>
          <FormItem className="flex items-center space-x-2">
            <FormControl>
              <RadioGroupItem value="surplus" />
            </FormControl>
            <FormLabel className="!mt-0">Surplus Lines</FormLabel>
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
  name="effectiveDate"
  render={({ field }) => (
    <FormItem className="flex flex-col">
      <FormLabel>Effective Date</FormLabel>
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
  name="brokerId"
  render={({ field }) => (
    <FormItem>
      <FormLabel>Broker</FormLabel>
      <Combobox
        options={brokers.map(b => ({ label: b.name, value: b.id }))}
        value={field.value}
        onValueChange={field.onChange}
        placeholder="Search brokers..."
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
function CreateSubmissionWizard() {
  const [step, setStep] = useState(1);

  const form = useForm<SubmissionFormData>({
    resolver: zodResolver(submissionSchema),
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

  const onSubmit = async (data: SubmissionFormData) => {
    await createSubmission(data);
    toast.success('Submission created');
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        {/* Progress indicator */}
        <StepIndicator currentStep={step} totalSteps={4} />

        {/* Step content */}
        {step === 1 && <BasicInfoStep form={form} />}
        {step === 2 && <CoverageStep form={form} />}
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

const contactsSchema = z.object({
  contacts: z.array(
    z.object({
      name: z.string().min(1),
      email: z.string().email(),
      phone: z.string().optional(),
    })
  ).min(1, 'At least one contact is required'),
});

function ContactsForm() {
  const form = useForm<z.infer<typeof contactsSchema>>({
    resolver: zodResolver(contactsSchema),
    defaultValues: {
      contacts: [{ name: '', email: '', phone: '' }],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: 'contacts',
  });

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <div className="space-y-4">
          {fields.map((field, index) => (
            <Card key={field.id}>
              <CardHeader>
                <CardTitle>Contact {index + 1}</CardTitle>
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
                  name={`contacts.${index}.name`}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Name</FormLabel>
                      <FormControl>
                        <Input {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name={`contacts.${index}.email`}
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Email</FormLabel>
                      <FormControl>
                        <Input type="email" {...field} />
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
          onClick={() => append({ name: '', email: '', phone: '' })}
        >
          Add Contact
        </Button>

        <Button type="submit">Save</Button>
      </form>
    </Form>
  );
}
```

### Dependent Fields

```tsx
function SubmissionForm() {
  const form = useForm<SubmissionFormData>({
    resolver: zodResolver(submissionSchema),
  });

  // Watch coverage type to show/hide fields
  const coverageType = form.watch('coverageType');

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <FormField
          control={form.control}
          name="coverageType"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Coverage Type</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value="admitted">Admitted</SelectItem>
                  <SelectItem value="surplus">Surplus Lines</SelectItem>
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Conditionally show surplus lines broker field */}
        {coverageType === 'surplus' && (
          <FormField
            control={form.control}
            name="surplusLinesBroker"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Surplus Lines Broker</FormLabel>
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
function BrokerForm() {
  const form = useForm<BrokerFormData>({
    resolver: zodResolver(brokerSchema),
  });

  const onSubmit = async (data: BrokerFormData) => {
    try {
      await createBroker(data);
      toast.success('Broker created');
    } catch (error) {
      // Handle API validation errors
      if (error.response?.status === 400) {
        const serverErrors = error.response.data.errors;

        // Map server errors to form fields
        Object.entries(serverErrors).forEach(([field, message]) => {
          form.setError(field as keyof BrokerFormData, {
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
form.setError('licenseNumber', {
  type: 'custom',
  message: 'License number already exists',
});

form.setError('root', {
  type: 'server',
  message: 'Failed to create broker. Please try again.',
});
```

---

## Best Practices

### 1. Match Backend Validation

✅ **GOOD:** Frontend and backend validation rules are identical
```tsx
// Frontend
const brokerSchema = z.object({
  licenseNumber: z.string().regex(/^[A-Z0-9]{6,10}$/),
});

// Backend (.NET)
[RegularExpression(@"^[A-Z0-9]{6,10}$")]
public string LicenseNumber { get; set; }
```

### 2. Use TypeScript Inference

✅ **GOOD:**
```tsx
const schema = z.object({ name: z.string() });
type FormData = z.infer<typeof schema>; // Inferred from schema
```

❌ **BAD:**
```tsx
interface FormData { name: string; } // Manually defined
const schema = z.object({ name: z.string() }); // Duplicate definition
```

### 3. Provide Helpful Error Messages

✅ **GOOD:**
```tsx
z.string()
  .min(1, 'Broker name is required')
  .max(200, 'Broker name must be less than 200 characters')
```

❌ **BAD:**
```tsx
z.string().min(1).max(200) // Generic error messages
```

### 4. Use Default Values

✅ **GOOD:**
```tsx
useForm({
  defaultValues: {
    isPrimary: false,
    state: 'CA',
    coverageType: 'admitted',
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
import { brokerSchema } from './schemas';

describe('Broker Schema', () => {
  it('should reject invalid email', () => {
    const result = brokerSchema.safeParse({
      name: 'Test Broker',
      email: 'invalid-email',
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toBe('Invalid email address');
    }
  });

  it('should accept valid data', () => {
    const result = brokerSchema.safeParse({
      name: 'Test Broker',
      email: 'test@example.com',
      licenseNumber: 'ABC123',
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

# JSON Schema Forms Guide (AJV + RJSF)

**Version:** 1.0
**Last Updated:** 2026-01-30
**Applies To:** Frontend Developer

---

## Overview

This guide covers dynamic form handling in Nebula using **JSON Schema** for schema definition, **AJV** for validation (frontend and backend), and **React JSON Schema Form (RJSF)** for rendering.

**Why This Approach:**
- **Backend/Frontend Parity**: Same JSON Schema validates on both .NET backend and React frontend
- **Dynamic Forms**: Forms driven by configuration/database, not hardcoded
- **Industry Standard**: JSON Schema is ISO standard (draft-07, draft-2020-12)
- **Complex Insurance Logic**: State-specific rules, coverage type variations, compliance requirements
- **Reusable Schemas**: Share schemas across services and teams

---

## Installation

```bash
npm install @rjsf/core @rjsf/utils @rjsf/validator-ajv8
npm install ajv ajv-formats ajv-errors
npm install @rjsf/mui  # Material-UI theme (optional, can use shadcn/ui custom theme)
```

---

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                  Backend (.NET)                      │
│  ┌────────────────────────────────────────────────┐ │
│  │ JSON Schema Definition (shared)                │ │
│  │ - broker-schema.json                           │ │
│  │ - submission-schema.json                       │ │
│  │ - stored in database or config                 │ │
│  └────────────────────────────────────────────────┘ │
│  ┌────────────────────────────────────────────────┐ │
│  │ JSON Schema Validator (.NET)                   │ │
│  │ - NJsonSchema or Json.NET Schema               │ │
│  │ - Validates API requests                       │ │
│  └────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
                         │
                         │ API: GET /api/schemas/broker
                         ↓
┌─────────────────────────────────────────────────────┐
│                Frontend (React)                      │
│  ┌────────────────────────────────────────────────┐ │
│  │ Fetch JSON Schema from backend                 │ │
│  └────────────────────────────────────────────────┘ │
│  ┌────────────────────────────────────────────────┐ │
│  │ AJV Validator (validates user input)           │ │
│  └────────────────────────────────────────────────┘ │
│  ┌────────────────────────────────────────────────┐ │
│  │ RJSF (renders form UI from schema)             │ │
│  └────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

---

## JSON Schema Basics

### Simple Broker Schema

```json
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$id": "https://nebula.com/schemas/broker.json",
  "title": "Broker",
  "description": "Insurance broker information",
  "type": "object",
  "required": ["name", "licenseNumber", "licenseState", "email"],
  "properties": {
    "name": {
      "type": "string",
      "title": "Broker Name",
      "minLength": 1,
      "maxLength": 200,
      "description": "Legal name of the brokerage"
    },
    "licenseNumber": {
      "type": "string",
      "title": "License Number",
      "pattern": "^[A-Z0-9]{6,10}$",
      "description": "State-issued license number"
    },
    "licenseState": {
      "type": "string",
      "title": "License State",
      "enum": ["CA", "NY", "TX", "FL", "IL"],
      "description": "State where broker is licensed"
    },
    "email": {
      "type": "string",
      "format": "email",
      "title": "Email Address"
    },
    "phone": {
      "type": "string",
      "title": "Phone Number",
      "pattern": "^\\d{10}$",
      "description": "10-digit phone number"
    },
    "website": {
      "type": "string",
      "format": "uri",
      "title": "Website"
    }
  }
}
```

### Complex Submission Schema with Conditional Logic

```json
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$id": "https://nebula.com/schemas/submission.json",
  "title": "Insurance Submission",
  "type": "object",
  "required": ["brokerId", "accountId", "coverageType", "effectiveDate"],
  "properties": {
    "brokerId": {
      "type": "string",
      "format": "uuid",
      "title": "Broker"
    },
    "accountId": {
      "type": "string",
      "format": "uuid",
      "title": "Account"
    },
    "coverageType": {
      "type": "string",
      "title": "Coverage Type",
      "enum": ["admitted", "surplus"],
      "default": "admitted"
    },
    "effectiveDate": {
      "type": "string",
      "format": "date",
      "title": "Effective Date"
    },
    "expirationDate": {
      "type": "string",
      "format": "date",
      "title": "Expiration Date"
    },
    "premium": {
      "type": "number",
      "title": "Premium",
      "minimum": 0,
      "exclusiveMinimum": true
    },
    "limits": {
      "type": "object",
      "title": "Coverage Limits",
      "required": ["perOccurrence", "aggregate"],
      "properties": {
        "perOccurrence": {
          "type": "number",
          "title": "Per Occurrence",
          "minimum": 0
        },
        "aggregate": {
          "type": "number",
          "title": "Aggregate",
          "minimum": 0
        }
      }
    },
    "surplusLinesBroker": {
      "type": "string",
      "title": "Surplus Lines Broker",
      "minLength": 1
    }
  },
  "allOf": [
    {
      "if": {
        "properties": {
          "coverageType": { "const": "surplus" }
        }
      },
      "then": {
        "required": ["surplusLinesBroker"]
      }
    }
  ]
}
```

---

## Backend Integration

### Storing Schemas in .NET

```csharp
// Domain/Schemas/BrokerSchema.cs
public static class FormSchemas
{
    public static readonly string BrokerSchema = @"{
      ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
      ""type"": ""object"",
      ""required"": [""name"", ""licenseNumber"", ""email""],
      ""properties"": {
        ""name"": {
          ""type"": ""string"",
          ""minLength"": 1,
          ""maxLength"": 200
        },
        ""licenseNumber"": {
          ""type"": ""string"",
          ""pattern"": ""^[A-Z0-9]{6,10}$""
        },
        ""email"": {
          ""type"": ""string"",
          ""format"": ""email""
        }
      }
    }";
}

// Minimal API endpoint to serve schema
app.MapGet("/api/schemas/{schemaName}", (string schemaName) =>
{
    return schemaName switch
    {
        "broker" => Results.Json(JsonSerializer.Deserialize<JsonElement>(FormSchemas.BrokerSchema)),
        "submission" => Results.Json(JsonSerializer.Deserialize<JsonElement>(FormSchemas.SubmissionSchema)),
        _ => Results.NotFound()
    };
}).WithName("GetFormSchema").WithTags("Schemas");
```

### Validating with JSON Schema in .NET

```csharp
// Using NJsonSchema
using NJsonSchema;
using NJsonSchema.Validation;

public class SchemaValidator
{
    public async Task<(bool IsValid, ICollection<ValidationError> Errors)>
        ValidateAsync(string schemaJson, string dataJson)
    {
        var schema = await JsonSchema.FromJsonAsync(schemaJson);
        var data = JsonDocument.Parse(dataJson);
        var errors = schema.Validate(data.RootElement);

        return (errors.Count == 0, errors);
    }
}

// In Minimal API endpoint
app.MapPost("/api/brokers", async (
    CreateBrokerRequest request,
    SchemaValidator validator) =>
{
    var requestJson = JsonSerializer.Serialize(request);
    var (isValid, errors) = await validator.ValidateAsync(
        FormSchemas.BrokerSchema,
        requestJson
    );

    if (!isValid)
    {
        return Results.ValidationProblem(
            errors.ToDictionary(
                e => e.Path,
                e => new[] { e.Kind.ToString() }
            )
        );
    }

    // Process valid request...
    return Results.Ok();
});
```

---

## Frontend Implementation with RJSF

### Basic Form with RJSF

```tsx
// components/forms/BrokerForm.tsx
import Form from '@rjsf/core';
import validator from '@rjsf/validator-ajv8';
import { RJSFSchema, UiSchema } from '@rjsf/utils';

interface BrokerFormProps {
  onSubmit: (data: any) => void;
  initialData?: any;
}

export function BrokerForm({ onSubmit, initialData }: BrokerFormProps) {
  // Schema would typically be fetched from backend
  const schema: RJSFSchema = {
    type: 'object',
    required: ['name', 'licenseNumber', 'email'],
    properties: {
      name: {
        type: 'string',
        title: 'Broker Name',
        minLength: 1,
        maxLength: 200,
      },
      licenseNumber: {
        type: 'string',
        title: 'License Number',
        pattern: '^[A-Z0-9]{6,10}$',
      },
      licenseState: {
        type: 'string',
        title: 'License State',
        enum: ['CA', 'NY', 'TX', 'FL'],
      },
      email: {
        type: 'string',
        format: 'email',
        title: 'Email',
      },
      phone: {
        type: 'string',
        title: 'Phone',
        pattern: '^\\d{10}$',
      },
    },
  };

  // UI Schema for custom rendering
  const uiSchema: UiSchema = {
    licenseNumber: {
      'ui:help': 'Enter alphanumeric license number',
      'ui:placeholder': 'CA0123456',
    },
    phone: {
      'ui:help': '10 digits, no formatting',
      'ui:placeholder': '5551234567',
    },
    email: {
      'ui:widget': 'email',
    },
  };

  return (
    <Form
      schema={schema}
      uiSchema={uiSchema}
      validator={validator}
      formData={initialData}
      onSubmit={({ formData }) => onSubmit(formData)}
    />
  );
}
```

### Fetching Schema from Backend

```tsx
// hooks/useFormSchema.ts
import { useQuery } from '@tanstack/react-query';
import { RJSFSchema } from '@rjsf/utils';

export function useFormSchema(schemaName: string) {
  return useQuery({
    queryKey: ['schemas', schemaName],
    queryFn: async () => {
      const response = await fetch(`/api/schemas/${schemaName}`);
      if (!response.ok) throw new Error('Failed to fetch schema');
      return response.json() as Promise<RJSFSchema>;
    },
    staleTime: 1000 * 60 * 30, // Schemas don't change often
  });
}

// Usage
function DynamicBrokerForm() {
  const { data: schema, isLoading } = useFormSchema('broker');

  if (isLoading) return <Skeleton />;

  return (
    <Form
      schema={schema}
      validator={validator}
      onSubmit={handleSubmit}
    />
  );
}
```

### Custom Widgets for shadcn/ui

```tsx
// components/forms/widgets/CustomTextWidget.tsx
import { WidgetProps } from '@rjsf/utils';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

export function CustomTextWidget(props: WidgetProps) {
  const {
    id,
    value,
    required,
    disabled,
    readonly,
    onChange,
    onBlur,
    onFocus,
    label,
    schema,
    rawErrors = [],
  } = props;

  return (
    <div className="space-y-2">
      <Label htmlFor={id}>
        {label || schema.title}
        {required && <span className="text-destructive ml-1">*</span>}
      </Label>
      <Input
        id={id}
        value={value || ''}
        disabled={disabled || readonly}
        onChange={(e) => onChange(e.target.value)}
        onBlur={() => onBlur && onBlur(id, value)}
        onFocus={() => onFocus && onFocus(id, value)}
        placeholder={schema.examples?.[0] as string}
        className={rawErrors.length > 0 ? 'border-destructive' : ''}
      />
      {schema.description && (
        <p className="text-sm text-muted-foreground">{schema.description}</p>
      )}
      {rawErrors.length > 0 && (
        <p className="text-sm text-destructive">{rawErrors[0]}</p>
      )}
    </div>
  );
}

// Custom select widget
export function CustomSelectWidget(props: WidgetProps) {
  const { id, value, onChange, options, schema, required, rawErrors } = props;

  return (
    <div className="space-y-2">
      <Label htmlFor={id}>
        {schema.title}
        {required && <span className="text-destructive ml-1">*</span>}
      </Label>
      <Select
        value={value}
        onValueChange={(val) => onChange(val)}
      >
        <SelectTrigger className={rawErrors.length > 0 ? 'border-destructive' : ''}>
          <SelectValue placeholder={`Select ${schema.title}`} />
        </SelectTrigger>
        <SelectContent>
          {options.enumOptions?.map((option) => (
            <SelectItem key={option.value} value={option.value}>
              {option.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      {rawErrors.length > 0 && (
        <p className="text-sm text-destructive">{rawErrors[0]}</p>
      )}
    </div>
  );
}
```

### Custom Theme for shadcn/ui

```tsx
// components/forms/ShadcnTheme.tsx
import { ThemeProps } from '@rjsf/core';
import { CustomTextWidget } from './widgets/CustomTextWidget';
import { CustomSelectWidget } from './widgets/CustomSelectWidget';

const ShadcnTheme: ThemeProps = {
  widgets: {
    TextWidget: CustomTextWidget,
    EmailWidget: CustomTextWidget,
    URLWidget: CustomTextWidget,
    SelectWidget: CustomSelectWidget,
  },
  templates: {
    // Override default templates
    ObjectFieldTemplate: CustomObjectFieldTemplate,
    ArrayFieldTemplate: CustomArrayFieldTemplate,
    FieldTemplate: CustomFieldTemplate,
  },
};

export default ShadcnTheme;

// Usage
import { withTheme } from '@rjsf/core';

const ThemedForm = withTheme(ShadcnTheme);

function BrokerForm() {
  return (
    <ThemedForm
      schema={schema}
      validator={validator}
      onSubmit={handleSubmit}
    />
  );
}
```

---

## AJV Configuration with Custom Formats

```typescript
// lib/ajv-config.ts
import Ajv from 'ajv';
import addFormats from 'ajv-formats';
import ajvErrors from 'ajv-errors';

export function createAjvInstance() {
  const ajv = new Ajv({
    allErrors: true,
    verbose: true,
    strict: false,
    $data: true, // Enable $data references
  });

  // Add standard formats (email, uri, date, etc.)
  addFormats(ajv);

  // Add custom error messages
  ajvErrors(ajv);

  // Add custom formats
  ajv.addFormat('phone', {
    type: 'string',
    validate: (value: string) => /^\d{10}$/.test(value),
  });

  ajv.addFormat('license-number', {
    type: 'string',
    validate: (value: string) => /^[A-Z0-9]{6,10}$/.test(value),
  });

  // Add custom keywords
  ajv.addKeyword({
    keyword: 'isNotEmpty',
    type: 'string',
    validate: function validate(schema: any, data: string) {
      return typeof data === 'string' && data.trim().length > 0;
    },
    errors: false,
  });

  return ajv;
}

// Create custom validator for RJSF
import { customizeValidator } from '@rjsf/validator-ajv8';

export const customValidator = customizeValidator({}, createAjvInstance());
```

---

## Conditional Logic / Dynamic Fields

### Show Field Based on Another Field

```json
{
  "type": "object",
  "properties": {
    "coverageType": {
      "type": "string",
      "enum": ["admitted", "surplus"],
      "default": "admitted"
    },
    "surplusLinesBroker": {
      "type": "string"
    }
  },
  "dependencies": {
    "coverageType": {
      "oneOf": [
        {
          "properties": {
            "coverageType": { "const": "surplus" }
          },
          "required": ["surplusLinesBroker"]
        }
      ]
    }
  }
}
```

### Using `allOf` for Complex Conditions

```json
{
  "type": "object",
  "properties": {
    "state": {
      "type": "string",
      "enum": ["CA", "NY", "TX"]
    },
    "surplusLinesLicense": {
      "type": "string"
    }
  },
  "allOf": [
    {
      "if": {
        "properties": {
          "state": { "const": "CA" }
        }
      },
      "then": {
        "properties": {
          "surplusLinesLicense": {
            "pattern": "^CA[0-9]{7}$"
          }
        }
      }
    },
    {
      "if": {
        "properties": {
          "state": { "const": "NY" }
        }
      },
      "then": {
        "properties": {
          "surplusLinesLicense": {
            "pattern": "^NY[0-9]{7}$"
          }
        }
      }
    }
  ]
}
```

---

## Custom Validation Rules

### Cross-Field Validation

```json
{
  "type": "object",
  "properties": {
    "effectiveDate": {
      "type": "string",
      "format": "date"
    },
    "expirationDate": {
      "type": "string",
      "format": "date"
    }
  },
  "if": {
    "properties": {
      "effectiveDate": { "type": "string" },
      "expirationDate": { "type": "string" }
    },
    "required": ["effectiveDate", "expirationDate"]
  },
  "then": {
    "properties": {
      "expirationDate": {
        "formatMinimum": { "$data": "1/effectiveDate" }
      }
    }
  }
}
```

### Custom Error Messages

```json
{
  "type": "object",
  "properties": {
    "premium": {
      "type": "number",
      "minimum": 0,
      "errorMessage": {
        "type": "Premium must be a number",
        "minimum": "Premium must be greater than zero"
      }
    }
  }
}
```

---

## State-Specific Schema Examples

### California-Specific Submission Schema

```typescript
// lib/schemas/state-schemas.ts
export const californiaSubmissionSchema = {
  $schema: 'https://json-schema.org/draft/2020-12/schema',
  type: 'object',
  required: ['earthquakeCoverage', 'surplusLinesTax'],
  properties: {
    earthquakeCoverage: {
      type: 'boolean',
      title: 'Earthquake Coverage Required?',
      description: 'California requires earthquake coverage disclosure',
    },
    surplusLinesTax: {
      type: 'number',
      title: 'Surplus Lines Tax (%)',
      minimum: 0,
      maximum: 100,
      default: 3.0,
      description: 'California surplus lines stamping fee',
    },
  },
};

// Merge with base schema
export function getSubmissionSchema(state: string) {
  const baseSchema = { /* base submission schema */ };
  const stateSchema = stateSchemas[state] || {};

  return {
    ...baseSchema,
    properties: {
      ...baseSchema.properties,
      ...stateSchema.properties,
    },
    required: [
      ...baseSchema.required,
      ...(stateSchema.required || []),
    ],
  };
}
```

---

## Testing JSON Schemas

### Unit Testing Schemas

```typescript
import Ajv from 'ajv';
import { brokerSchema } from './schemas/broker';

describe('Broker Schema', () => {
  const ajv = new Ajv();
  const validate = ajv.compile(brokerSchema);

  it('should accept valid broker data', () => {
    const validData = {
      name: 'ABC Insurance Brokers',
      licenseNumber: 'CA0123456',
      licenseState: 'CA',
      email: 'contact@abc.com',
      phone: '5551234567',
    };

    const isValid = validate(validData);
    expect(isValid).toBe(true);
  });

  it('should reject invalid email', () => {
    const invalidData = {
      name: 'ABC Insurance',
      licenseNumber: 'CA0123456',
      licenseState: 'CA',
      email: 'invalid-email',
    };

    const isValid = validate(invalidData);
    expect(isValid).toBe(false);
    expect(validate.errors?.[0].keyword).toBe('format');
  });

  it('should require license number', () => {
    const invalidData = {
      name: 'ABC Insurance',
      licenseState: 'CA',
      email: 'contact@abc.com',
    };

    const isValid = validate(invalidData);
    expect(isValid).toBe(false);
    expect(validate.errors?.[0].keyword).toBe('required');
  });
});
```

---

## Best Practices

### 1. Version Your Schemas

```json
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$id": "https://nebula.com/schemas/broker/v1.0.0",
  "version": "1.0.0",
  "type": "object"
}
```

### 2. Use References for Reusability

```json
{
  "$defs": {
    "address": {
      "type": "object",
      "required": ["street", "city", "state", "zip"],
      "properties": {
        "street": { "type": "string" },
        "city": { "type": "string" },
        "state": { "type": "string", "pattern": "^[A-Z]{2}$" },
        "zip": { "type": "string", "pattern": "^\\d{5}(-\\d{4})?$" }
      }
    }
  },
  "type": "object",
  "properties": {
    "mailingAddress": { "$ref": "#/$defs/address" },
    "billingAddress": { "$ref": "#/$defs/address" }
  }
}
```

### 3. Provide Good Error Messages

```json
{
  "properties": {
    "premium": {
      "type": "number",
      "minimum": 0,
      "errorMessage": "Premium must be a positive number"
    }
  }
}
```

### 4. Use UI Schema for Customization

```typescript
const uiSchema = {
  premium: {
    'ui:widget': 'updown',
    'ui:help': 'Enter premium in USD',
    'ui:options': {
      prefix: '$',
    },
  },
  effectiveDate: {
    'ui:widget': 'date',
    'ui:options': {
      yearsRange: [2024, 2030],
    },
  },
};
```

---

## Comparison: JSON Schema + AJV vs Zod

| Feature | JSON Schema + AJV | Zod |
|---------|-------------------|-----|
| **TypeScript Inference** | ❌ No (manual types) | ✅ Excellent |
| **Backend Sharing** | ✅ Works in .NET/Node/Python | ❌ TypeScript only |
| **Dynamic Forms** | ✅ Designed for this | ⚠️ Possible but awkward |
| **Learning Curve** | ⚠️ Steeper (JSON Schema spec) | ✅ Simpler API |
| **Industry Standard** | ✅ ISO standard | ❌ Library-specific |
| **Tooling** | ✅ Many validators/generators | ⚠️ Limited to JS/TS |
| **Complex Validation** | ✅ `allOf`, `anyOf`, `if/then` | ✅ `.refine()`, `.superRefine()` |
| **Error Messages** | ⚠️ Requires ajv-errors | ✅ Built-in |
| **Performance** | ✅ Very fast (compiled) | ✅ Fast |

**Recommendation for Nebula:**
Use **JSON Schema + AJV + RJSF** because:
- Insurance forms are dynamic and state-specific
- Need backend/frontend validation parity (.NET + React)
- Schemas can be stored in database and updated without code deploy
- Industry standard for compliance and auditing

---

## Migration Path

If you have existing Zod schemas:

```typescript
// Convert Zod to JSON Schema
import { zodToJsonSchema } from 'zod-to-json-schema';

const zodSchema = z.object({
  name: z.string().min(1).max(200),
  email: z.string().email(),
});

const jsonSchema = zodToJsonSchema(zodSchema, 'BrokerSchema');
```

---

## References

- [JSON Schema Specification](https://json-schema.org)
- [AJV Documentation](https://ajv.js.org)
- [React JSON Schema Form](https://rjsf-team.github.io/react-jsonschema-form/)
- [NJsonSchema (.NET)](https://github.com/RicoSuter/NJsonSchema)
- [Understanding JSON Schema](https://json-schema.org/understanding-json-schema/)

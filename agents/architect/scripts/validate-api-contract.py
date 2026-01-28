#!/usr/bin/env python3
"""
API Contract Validation Script

Validates OpenAPI specifications for completeness and consistency.

Usage:
    python validate-api-contract.py <path-to-openapi-yaml>
    python validate-api-contract.py planning-mds/api/broker-api.yaml
"""

import sys
import yaml
from pathlib import Path
from typing import Dict, List, Tuple

class ApiContractValidator:
    def __init__(self, file_path: str):
        self.file_path = Path(file_path)
        self.spec = None
        self.errors = []
        self.warnings = []

    def load_spec(self) -> bool:
        """Load OpenAPI YAML spec."""
        try:
            with open(self.file_path, 'r', encoding='utf-8') as f:
                self.spec = yaml.safe_load(f)
            return True
        except Exception as e:
            self.errors.append(f"Failed to load API spec: {e}")
            return False

    def validate(self) -> Tuple[bool, List[str], List[str]]:
        """Validate API contract completeness and quality."""
        if not self.load_spec():
            return False, self.errors, self.warnings

        self.check_required_fields()
        self.check_paths()
        self.check_responses()
        self.check_error_contract()
        self.check_security()
        self.check_schemas()

        is_valid = len(self.errors) == 0
        return is_valid, self.errors, self.warnings

    def check_required_fields(self):
        """Check for required OpenAPI fields."""
        required = ['openapi', 'info', 'paths']
        for field in required:
            if field not in self.spec:
                self.errors.append(f"Missing required field: {field}")

        if 'info' in self.spec:
            info_required = ['title', 'version']
            for field in info_required:
                if field not in self.spec['info']:
                    self.errors.append(f"Missing required info field: {field}")

    def check_paths(self):
        """Check API paths follow REST conventions."""
        if 'paths' not in self.spec:
            return

        for path, methods in self.spec['paths'].items():
            # Check for verbs in path (should use HTTP methods instead)
            verb_indicators = ['get', 'post', 'put', 'delete', 'create', 'update', 'list']
            for verb in verb_indicators:
                if verb in path.lower():
                    self.warnings.append(f"Path '{path}' contains verb '{verb}' - use HTTP methods instead")

            # Check for non-plural resources (unless it's a singleton)
            if not path.startswith('/api/'):
                self.warnings.append(f"Path '{path}' doesn't start with /api/")

            # Check each method
            for method in methods:
                if method not in ['get', 'post', 'put', 'patch', 'delete', 'options', 'head']:
                    continue

                operation = methods[method]

                # Check for operationId
                if 'operationId' not in operation:
                    self.warnings.append(f"{method.upper()} {path}: Missing operationId")

                # Check for summary
                if 'summary' not in operation:
                    self.warnings.append(f"{method.upper()} {path}: Missing summary")

                # Check for responses
                if 'responses' not in operation:
                    self.errors.append(f"{method.upper()} {path}: Missing responses")

    def check_responses(self):
        """Check response definitions."""
        if 'paths' not in self.spec:
            return

        for path, methods in self.spec['paths'].items():
            for method, operation in methods.items():
                if method not in ['get', 'post', 'put', 'patch', 'delete']:
                    continue

                if 'responses' not in operation:
                    continue

                responses = operation['responses']

                # Check for success response
                success_codes = ['200', '201', '204']
                has_success = any(code in responses for code in success_codes)
                if not has_success:
                    self.warnings.append(f"{method.upper()} {path}: No success response (200, 201, or 204)")

                # POST should return 201
                if method == 'post' and '201' not in responses:
                    self.warnings.append(f"POST {path}: Should return 201 Created")

                # DELETE should return 204
                if method == 'delete' and '204' not in responses:
                    self.warnings.append(f"DELETE {path}: Should return 204 No Content")

                # Check for error responses
                if '400' not in responses:
                    self.warnings.append(f"{method.upper()} {path}: Missing 400 Bad Request response")

                if '401' not in responses:
                    self.warnings.append(f"{method.upper()} {path}: Missing 401 Unauthorized response")

                if '403' not in responses:
                    self.warnings.append(f"{method.upper()} {path}: Missing 403 Forbidden response")

    def check_error_contract(self):
        """Check for consistent error response schema."""
        if 'components' not in self.spec:
            self.warnings.append("Missing components section - define reusable schemas")
            return

        if 'schemas' not in self.spec['components']:
            self.warnings.append("Missing schemas in components")
            return

        schemas = self.spec['components']['schemas']

        # Check for ErrorResponse schema
        if 'ErrorResponse' not in schemas:
            self.errors.append("Missing ErrorResponse schema - all APIs should have consistent error format")
        else:
            error_schema = schemas['ErrorResponse']
            required_fields = ['code', 'message']
            if 'properties' in error_schema:
                for field in required_fields:
                    if field not in error_schema['properties']:
                        self.errors.append(f"ErrorResponse missing required field: {field}")

    def check_security(self):
        """Check security definitions."""
        if 'security' not in self.spec and 'securitySchemes' not in self.spec.get('components', {}):
            self.warnings.append("No security defined - all endpoints should require authentication")

    def check_schemas(self):
        """Check schema definitions."""
        if 'components' not in self.spec or 'schemas' not in self.spec['components']:
            return

        schemas = self.spec['components']['schemas']

        for schema_name, schema in schemas.items():
            # Check for description
            if 'description' not in schema and 'properties' not in schema:
                self.warnings.append(f"Schema '{schema_name}' missing description")

            # Check for required fields definition
            if 'properties' in schema and 'required' not in schema:
                self.warnings.append(f"Schema '{schema_name}' has properties but no 'required' array")

def main():
    if len(sys.argv) < 2:
        print("Usage: python validate-api-contract.py <openapi-yaml-file>")
        print("Example: python validate-api-contract.py planning-mds/api/broker-api.yaml")
        sys.exit(1)

    file_path = sys.argv[1]

    print(f"Validating API contract: {file_path}")
    print("-" * 60)

    validator = ApiContractValidator(file_path)
    is_valid, errors, warnings = validator.validate()

    # Print errors
    if errors:
        print("\n❌ ERRORS (Must Fix):")
        for i, error in enumerate(errors, 1):
            print(f"  {i}. {error}")

    # Print warnings
    if warnings:
        print("\n⚠️  WARNINGS (Should Fix):")
        for i, warning in enumerate(warnings, 1):
            print(f"  {i}. {warning}")

    # Print summary
    print("\n" + "=" * 60)
    if is_valid and not warnings:
        print("✅ API contract validation PASSED - No issues found!")
        sys.exit(0)
    elif is_valid:
        print(f"⚠️  API contract validation PASSED with {len(warnings)} warning(s)")
        sys.exit(0)
    else:
        print(f"❌ API contract validation FAILED with {len(errors)} error(s) and {len(warnings)} warning(s)")
        sys.exit(1)

if __name__ == "__main__":
    main()

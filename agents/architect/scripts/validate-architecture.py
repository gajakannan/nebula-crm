#!/usr/bin/env python3
"""
Architecture Validation Script

Validates architecture specifications in INCEPTION.md for completeness.

Usage:
    python validate-architecture.py <path-to-inception-md>
    python validate-architecture.py planning-mds/INCEPTION.md
"""

import sys
import re
from pathlib import Path
from typing import List, Dict, Tuple

class ArchitectureValidator:
    def __init__(self, file_path: str):
        self.file_path = Path(file_path)
        self.content = ""
        self.errors = []
        self.warnings = []

    def load_inception(self) -> bool:
        """Load INCEPTION.md content."""
        try:
            self.content = self.file_path.read_text(encoding='utf-8')
            return True
        except Exception as e:
            self.errors.append(f"Failed to read file: {e}")
            return False

    def validate(self) -> Tuple[bool, List[str], List[str]]:
        """Validate architecture completeness."""
        if not self.load_inception():
            return False, self.errors, self.warnings

        # Check Phase B sections (4.x)
        self.check_service_boundaries()
        self.check_data_model()
        self.check_workflow_rules()
        self.check_authorization_model()
        self.check_api_contracts()
        self.check_nfrs()

        # Quality checks
        self.check_for_todos()
        self.check_consistency()

        is_valid = len(self.errors) == 0
        return is_valid, self.errors, self.warnings

    def check_service_boundaries(self):
        """Check for service boundaries section (4.1)."""
        if "## 4.1" not in self.content and "### 4.1" not in self.content:
            self.errors.append("Missing Section 4.1: Service Boundaries")
        elif "TODO" in self.get_section_content("4.1"):
            self.errors.append("Section 4.1 (Service Boundaries) contains TODOs")

    def check_data_model(self):
        """Check for data model section (4.2)."""
        if "## 4.2" not in self.content and "### 4.2" not in self.content:
            self.errors.append("Missing Section 4.2: Data Model")
            return

        section = self.get_section_content("4.2")

        if "TODO" in section:
            self.errors.append("Section 4.2 (Data Model) contains TODOs")

        # Check for key entity definitions
        key_entities = ["Broker", "Submission", "Renewal"]
        for entity in key_entities:
            if entity not in section:
                self.warnings.append(f"Data Model doesn't mention '{entity}' entity")

        # Check for audit fields mention
        if "CreatedAt" not in section and "created_at" not in section.lower():
            self.warnings.append("Data Model doesn't mention audit fields (CreatedAt, UpdatedAt)")

    def check_workflow_rules(self):
        """Check for workflow rules section (4.3)."""
        if "## 4.3" not in self.content and "### 4.3" not in self.content:
            self.errors.append("Missing Section 4.3: Workflow Rules")
            return

        section = self.get_section_content("4.3")

        if "TODO" in section:
            self.errors.append("Section 4.3 (Workflow Rules) contains TODOs")

        # Check for workflow definitions
        workflows = ["Submission", "Renewal"]
        for workflow in workflows:
            if workflow not in section:
                self.warnings.append(f"Workflow Rules don't mention '{workflow}' workflow")

        # Check for transition mention
        if "transition" not in section.lower():
            self.warnings.append("Workflow Rules don't mention state transitions")

    def check_authorization_model(self):
        """Check for authorization model section (4.4)."""
        if "## 4.4" not in self.content and "### 4.4" not in self.content:
            self.errors.append("Missing Section 4.4: Authorization Model")
            return

        section = self.get_section_content("4.4")

        if "TODO" in section:
            self.errors.append("Section 4.4 (Authorization Model) contains TODOs")

        # Check for ABAC components
        if "subject" not in section.lower() and "user" not in section.lower():
            self.warnings.append("Authorization Model doesn't define subject attributes")

        if "resource" not in section.lower():
            self.warnings.append("Authorization Model doesn't define resource attributes")

        if "action" not in section.lower() and "permission" not in section.lower():
            self.warnings.append("Authorization Model doesn't define actions/permissions")

    def check_api_contracts(self):
        """Check for API contracts section (4.5)."""
        if "## 4.5" not in self.content and "### 4.5" not in self.content:
            self.errors.append("Missing Section 4.5: API Contracts")
            return

        section = self.get_section_content("4.5")

        if "TODO" in section:
            self.errors.append("Section 4.5 (API Contracts) contains TODOs")

        # Check for CRUD operations
        if "POST" not in section and "CREATE" not in section.upper():
            self.warnings.append("API Contracts don't mention POST/CREATE operations")

        if "GET" not in section and "READ" not in section.upper():
            self.warnings.append("API Contracts don't mention GET/READ operations")

    def check_nfrs(self):
        """Check for NFRs section (4.6)."""
        if "## 4.6" not in self.content and "### 4.6" not in self.content:
            self.errors.append("Missing Section 4.6: Non-Functional Requirements")
            return

        section = self.get_section_content("4.6")

        if "TODO" in section:
            self.errors.append("Section 4.6 (NFRs) contains TODOs")

        # Check for key NFR categories
        nfr_categories = ["performance", "security", "availability", "scalability"]
        for category in nfr_categories:
            if category not in section.lower():
                self.warnings.append(f"NFRs don't mention '{category}'")

        # Check for specific metrics
        if "ms" not in section and "second" not in section.lower():
            self.warnings.append("NFRs don't specify performance metrics (e.g., < 500ms)")

    def check_for_todos(self):
        """Check for remaining TODOs in Phase B."""
        phase_b_section = self.get_section_content("4")
        todo_count = phase_b_section.count("TODO")

        if todo_count > 0:
            self.errors.append(f"Phase B contains {todo_count} TODO(s) - all must be resolved")

    def check_consistency(self):
        """Check for consistency across sections."""
        # Check if entities in data model match API contracts
        data_model = self.get_section_content("4.2")
        api_contracts = self.get_section_content("4.5")

        # Simple heuristic: check if major entities appear in both
        entities = ["Broker", "Submission", "Renewal"]
        for entity in entities:
            if entity in data_model and entity not in api_contracts:
                self.warnings.append(f"Entity '{entity}' in Data Model but not in API Contracts")

    def get_section_content(self, section_num: str) -> str:
        """Extract content for a specific section."""
        pattern = rf"##\s*{section_num}[.:\s].*?(?=##\s*\d|\Z)"
        match = re.search(pattern, self.content, re.DOTALL | re.IGNORECASE)
        return match.group(0) if match else ""

def main():
    if len(sys.argv) < 2:
        print("Usage: python validate-architecture.py <inception-md-file>")
        print("Example: python validate-architecture.py planning-mds/INCEPTION.md")
        sys.exit(1)

    file_path = sys.argv[1]

    print(f"Validating architecture specification: {file_path}")
    print("-" * 60)

    validator = ArchitectureValidator(file_path)
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
        print("✅ Architecture validation PASSED - No issues found!")
        sys.exit(0)
    elif is_valid:
        print(f"⚠️  Architecture validation PASSED with {len(warnings)} warning(s)")
        sys.exit(0)
    else:
        print(f"❌ Architecture validation FAILED with {len(errors)} error(s) and {len(warnings)} warning(s)")
        sys.exit(1)

if __name__ == "__main__":
    main()

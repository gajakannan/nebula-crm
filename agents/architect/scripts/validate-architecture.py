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
        self.entities = []
        self.workflows = []

    def load_inception(self) -> bool:
        """Load INCEPTION.md content."""
        try:
            self.content = self.file_path.read_text(encoding='utf-8')
            return True
        except Exception as e:
            self.errors.append(f"Failed to read file: {e}")
            return False

    def extract_entities_from_inception(self) -> List[str]:
        """
        Extract core entities from section 1.3 of INCEPTION.md.

        Expected format:
        ### 1.3 Core entities (baseline)
        - Entity1 (optional description)
        - Entity2
        ...
        """
        section = self.get_section_content("1.3")
        entities = []

        for line in section.split('\n'):
            line = line.strip()
            # Match lines like "- Entity" or "* Entity" or "- Entity (description)"
            match = re.match(r'^[-*]\s+([A-Z][A-Za-z0-9]+)', line)
            if match:
                entity = match.group(1)
                entities.append(entity)

        return entities

    def extract_workflows_from_inception(self) -> List[str]:
        """
        Extract workflow names from section 1.4 of INCEPTION.md.

        Expected format:
        ### 1.4 Critical workflows (baseline)
        WorkflowName: State1 → State2 → ...
        AnotherWorkflow: State1 → State2 → ...
        """
        section = self.get_section_content("1.4")
        workflows = []

        for line in section.split('\n'):
            line = line.strip()
            # Match lines like "WorkflowName: State1 → State2 → ..."
            match = re.match(r'^([A-Z][A-Za-z]+):\s+', line)
            if match:
                workflow = match.group(1)
                workflows.append(workflow)

        return workflows

    def validate(self) -> Tuple[bool, List[str], List[str]]:
        """Validate architecture completeness."""
        if not self.load_inception():
            return False, self.errors, self.warnings

        # Extract entities and workflows from INCEPTION.md (section 1.3 and 1.4)
        self.entities = self.extract_entities_from_inception()
        self.workflows = self.extract_workflows_from_inception()

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

        # Check for key entity definitions (extracted from section 1.3)
        # Only check first few entities to avoid too many warnings
        key_entities = self.entities[:5] if len(self.entities) > 5 else self.entities
        for entity in key_entities:
            if entity not in section:
                self.warnings.append(f"Data Model doesn't mention '{entity}' entity (from section 1.3)")

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

        # Check for workflow definitions (extracted from section 1.4)
        for workflow in self.workflows:
            if workflow not in section:
                self.warnings.append(f"Workflow Rules don't mention '{workflow}' workflow (from section 1.4)")

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
        # Check first few entities to avoid too many warnings
        entities_to_check = self.entities[:5] if len(self.entities) > 5 else self.entities
        for entity in entities_to_check:
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

    # Print extracted context
    if validator.entities:
        print(f"\n[Entities] Extracted from section 1.3: {', '.join(validator.entities[:5])}")
        if len(validator.entities) > 5:
            print(f"           (and {len(validator.entities) - 5} more...)")
    if validator.workflows:
        print(f"[Workflows] Extracted from section 1.4: {', '.join(validator.workflows)}")

    # Print errors
    if errors:
        print("\n[ERROR] Must Fix:")
        for i, error in enumerate(errors, 1):
            print(f"  {i}. {error}")

    # Print warnings
    if warnings:
        print("\n[WARNING] Should Fix:")
        for i, warning in enumerate(warnings, 1):
            print(f"  {i}. {warning}")

    # Print summary
    print("\n" + "=" * 60)
    if is_valid and not warnings:
        print("[PASS] Architecture validation PASSED - No issues found!")
        sys.exit(0)
    elif is_valid:
        print(f"[PASS] Architecture validation PASSED with {len(warnings)} warning(s)")
        sys.exit(0)
    else:
        print(f"[FAIL] Architecture validation FAILED with {len(errors)} error(s) and {len(warnings)} warning(s)")
        sys.exit(1)

if __name__ == "__main__":
    main()

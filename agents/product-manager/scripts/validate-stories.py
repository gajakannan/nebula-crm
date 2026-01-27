#!/usr/bin/env python3
"""
Story Validation Script

Validates user stories for completeness and quality.
Checks that stories follow the template and have all required sections.

Usage:
    python validate-stories.py <path-to-story-file>
    python validate-stories.py planning-mds/stories/*.md
"""

import sys
import re
from pathlib import Path
from typing import List, Dict, Tuple

class StoryValidator:
    def __init__(self, file_path: str):
        self.file_path = Path(file_path)
        self.content = ""
        self.errors = []
        self.warnings = []

    def load_story(self) -> bool:
        """Load story file content."""
        try:
            self.content = self.file_path.read_text(encoding='utf-8')
            return True
        except Exception as e:
            self.errors.append(f"Failed to read file: {e}")
            return False

    def validate(self) -> Tuple[bool, List[str], List[str]]:
        """
        Validate story completeness and quality.
        Returns (is_valid, errors, warnings)
        """
        if not self.load_story():
            return False, self.errors, self.warnings

        # Required sections
        self.check_user_story_format()
        self.check_acceptance_criteria()
        self.check_data_requirements()
        self.check_dependencies()
        self.check_out_of_scope()
        self.check_definition_of_done()

        # Quality checks
        self.check_invest_criteria()
        self.check_acceptance_criteria_quality()

        is_valid = len(self.errors) == 0
        return is_valid, self.errors, self.warnings

    def check_user_story_format(self):
        """Check for 'As a...I want...So that...' format."""
        story_pattern = r"(?i)\*\*As\s+a\*\*.*\*\*I\s+want\*\*.*\*\*So\s+that\*\*"

        if not re.search(story_pattern, self.content):
            self.errors.append("Missing or malformed user story (As a...I want...So that...)")
        else:
            # Check if persona is specific (not just "user")
            if re.search(r"\*\*As\s+a\*\*\s+(user|someone|person)", self.content, re.IGNORECASE):
                self.warnings.append("User story uses generic persona 'user' - be more specific")

    def check_acceptance_criteria(self):
        """Check for acceptance criteria section."""
        if "## Acceptance Criteria" not in self.content and "### Acceptance Criteria" not in self.content:
            self.errors.append("Missing 'Acceptance Criteria' section")
            return

        # Check for at least one Given/When/Then or checklist item
        has_given_when_then = bool(re.search(r"(Given|When|Then)", self.content))
        has_checklist = bool(re.search(r"- \[ \]", self.content))

        if not has_given_when_then and not has_checklist:
            self.errors.append("Acceptance criteria section exists but has no criteria (use Given/When/Then or checklist)")

    def check_data_requirements(self):
        """Check for data requirements section."""
        if "## Data Requirements" not in self.content and "### Data Requirements" not in self.content:
            self.warnings.append("Missing 'Data Requirements' section - consider adding if story involves data")

    def check_dependencies(self):
        """Check for dependencies section."""
        if "## Dependencies" not in self.content and "### Dependencies" not in self.content:
            self.warnings.append("Missing 'Dependencies' section - confirm story is truly independent")

    def check_out_of_scope(self):
        """Check for out of scope section."""
        if "## Out of Scope" not in self.content and "### Out of Scope" not in self.content:
            self.warnings.append("Missing 'Out of Scope' section - explicitly document what's NOT included")

    def check_definition_of_done(self):
        """Check for definition of done."""
        if "## Definition of Done" not in self.content and "### Definition of Done" not in self.content:
            self.warnings.append("Missing 'Definition of Done' section")

    def check_invest_criteria(self):
        """Check INVEST criteria quality."""

        # Independent: Check for phrases indicating dependencies
        dependency_phrases = ["depends on", "requires", "needs", "after", "once", "when.*is complete"]
        for phrase in dependency_phrases:
            if re.search(phrase, self.content, re.IGNORECASE):
                self.warnings.append(f"Story may have dependencies - check 'Independent' (INVEST)")
                break

        # Valuable: Check for technical-only language
        technical_terms = ["database", "api", "endpoint", "schema", "migration", "refactor"]
        story_section = re.search(r"\*\*As\s+a\*\*.*?\*\*So\s+that\*\*.*?(?=\n\n|\Z)", self.content, re.DOTALL | re.IGNORECASE)
        if story_section:
            story_text = story_section.group(0).lower()
            if any(term in story_text for term in technical_terms):
                self.warnings.append("Story may be technical-focused rather than user-value focused (INVEST - Valuable)")

        # Small: Check story length (rough heuristic)
        if len(self.content) > 10000:
            self.warnings.append("Story is very large (>10k chars) - consider breaking into smaller slices (INVEST - Small)")

        # Testable: Check for vague terms in acceptance criteria
        vague_terms = ["properly", "correctly", "appropriate", "fast", "user-friendly", "intuitive"]
        ac_section = re.search(r"##? Acceptance Criteria.*?(?=##|\Z)", self.content, re.DOTALL | re.IGNORECASE)
        if ac_section:
            ac_text = ac_section.group(0).lower()
            found_vague = [term for term in vague_terms if term in ac_text]
            if found_vague:
                self.warnings.append(f"Acceptance criteria contain vague terms: {', '.join(found_vague)} - be more specific (INVEST - Testable)")

    def check_acceptance_criteria_quality(self):
        """Check acceptance criteria quality."""

        # Check for edge cases
        if "edge case" not in self.content.lower() and "error scenario" not in self.content.lower():
            self.warnings.append("No edge cases or error scenarios documented - consider adding")

        # Check for permission/authorization
        if "permission" not in self.content.lower() and "authorized" not in self.content.lower():
            self.warnings.append("No permission/authorization checks documented - consider adding if applicable")

        # Check for audit trail (if mutation involved)
        mutation_keywords = ["create", "update", "delete", "change", "transition", "modify"]
        if any(keyword in self.content.lower() for keyword in mutation_keywords):
            if "timeline" not in self.content.lower() and "audit" not in self.content.lower():
                self.warnings.append("Story involves data mutation but has no audit/timeline requirements")

def main():
    if len(sys.argv) < 2:
        print("Usage: python validate-stories.py <story-file-path>")
        print("Example: python validate-stories.py planning-mds/stories/S1-create-broker.md")
        sys.exit(1)

    file_path = sys.argv[1]

    print(f"Validating story: {file_path}")
    print("-" * 60)

    validator = StoryValidator(file_path)
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
        print("✅ Story validation PASSED - No issues found!")
        sys.exit(0)
    elif is_valid:
        print(f"⚠️  Story validation PASSED with {len(warnings)} warning(s)")
        sys.exit(0)
    else:
        print(f"❌ Story validation FAILED with {len(errors)} error(s) and {len(warnings)} warning(s)")
        sys.exit(1)

if __name__ == "__main__":
    main()

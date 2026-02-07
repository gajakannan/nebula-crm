#!/usr/bin/env python3
"""
Story Validation Script

Validates user stories for completeness and quality.
Checks that stories follow the template and have all required sections.

Usage:
    python validate-stories.py <file-or-dir> [<file-or-dir> ...]
    python validate-stories.py planning-mds/stories/
    python validate-stories.py planning-mds/stories/*.md
"""

import sys
import io
import re
from pathlib import Path

# Windows cp1252 stdout can't encode emojis used in report output.
# Reconfigure to utf-8 unconditionally — safe on all platforms.
if hasattr(sys.stdout, 'buffer'):
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
if hasattr(sys.stderr, 'buffer'):
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')
from typing import List, Dict, Tuple, Iterable

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
        self.check_single_story_per_file()
        self.check_story_header_fields()
        self.check_user_story_format()
        self.check_context_background()
        self.check_acceptance_criteria()
        self.check_data_requirements()
        self.check_role_based_visibility()
        self.check_non_functional_expectations()
        self.check_dependencies()
        self.check_out_of_scope()
        self.check_questions_assumptions()
        self.check_definition_of_done()

        # Quality checks
        self.check_invest_criteria()
        self.check_acceptance_criteria_quality()

        is_valid = len(self.errors) == 0
        return is_valid, self.errors, self.warnings

    def check_user_story_format(self):
        """Check for 'As a...I want...So that...' format."""
        story_pattern = r"\*\*As\s+a\*\*.*\*\*I\s+want\*\*.*\*\*So\s+that\*\*"

        if not re.search(story_pattern, self.content, re.IGNORECASE | re.DOTALL):
            self.errors.append("Missing or malformed user story (As a...I want...So that...)")
        else:
            # Check if persona is specific (not just "user")
            if re.search(r"\*\*As\s+a\*\*\s+(user|someone|person)", self.content, re.IGNORECASE):
                self.warnings.append("User story uses generic persona 'user' - be more specific")

    def check_single_story_per_file(self):
        """Ensure story files contain exactly one story."""
        story_id_markers = re.findall(r"\*\*Story ID:\*\*", self.content)
        if len(story_id_markers) > 1:
            self.errors.append("Multiple stories detected in one file. Keep one story per file.")

        # Legacy combined examples often use headings like "## Story S1: ..."
        legacy_story_headings = re.findall(r"^##\s+Story\s+[A-Za-z0-9_-]+", self.content, re.IGNORECASE | re.MULTILINE)
        if len(legacy_story_headings) > 1:
            self.errors.append("Multiple story sections detected. Split into separate files (one story per file).")

    def check_acceptance_criteria(self):
        """Check for acceptance criteria section."""
        section = self.get_section_content("Acceptance Criteria")
        if not section:
            self.errors.append("Missing 'Acceptance Criteria' section")
            return

        # Check for at least one Given/When/Then or checklist item
        has_given_when_then = bool(re.search(r"(Given|When|Then)", section))
        has_checklist = bool(re.search(r"- \[ \]", section))

        if not has_given_when_then and not has_checklist:
            self.errors.append("Acceptance criteria section exists but has no criteria (use Given/When/Then or checklist)")

    def check_data_requirements(self):
        """Check for data requirements section."""
        if not self.get_section_content("Data Requirements"):
            self.errors.append("Missing 'Data Requirements' section")

    def check_role_based_visibility(self):
        """Check for role-based visibility section."""
        if not self.get_section_content("Role-Based Visibility"):
            self.errors.append("Missing 'Role-Based Visibility' section")

    def check_non_functional_expectations(self):
        """Check for non-functional expectations section."""
        if not self.get_section_content("Non-Functional Expectations"):
            self.warnings.append("Missing 'Non-Functional Expectations' section (add if applicable)")

    def check_dependencies(self):
        """Check for dependencies section."""
        if not self.get_section_content("Dependencies"):
            self.errors.append("Missing 'Dependencies' section")

    def check_out_of_scope(self):
        """Check for out of scope section."""
        if not self.get_section_content("Out of Scope"):
            self.errors.append("Missing 'Out of Scope' section")

    def check_questions_assumptions(self):
        """Check for questions & assumptions section."""
        if not self.get_section_content("Questions & Assumptions"):
            self.warnings.append("Missing 'Questions & Assumptions' section")

    def check_definition_of_done(self):
        """Check for definition of done."""
        if not self.get_section_content("Definition of Done"):
            self.errors.append("Missing 'Definition of Done' section")

    def check_story_header_fields(self):
        """Check for story header fields in the template."""
        required_fields = [
            "Story ID",
            "Epic/Feature",
            "Title",
            "Priority",
            "Phase",
        ]
        for field in required_fields:
            if not re.search(rf"\*\*{re.escape(field)}:\*\*", self.content):
                self.errors.append(f"Missing story header field: {field}")

    def check_context_background(self):
        """Check for context & background section."""
        if not self.get_section_content("Context & Background"):
            self.warnings.append("Missing 'Context & Background' section")

    def check_invest_criteria(self):
        """Check INVEST criteria quality."""

        user_story_section = self.get_section_content("User Story")
        invest_scope = user_story_section if user_story_section else self.content

        # Independent: Check for phrases indicating dependencies
        dependency_phrases = ["depends on", "requires", "needs", "after", "once", "when.*is complete"]
        for phrase in dependency_phrases:
            if re.search(phrase, invest_scope, re.IGNORECASE):
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
        ac_section = self.get_section_content("Acceptance Criteria")
        if ac_section:
            ac_text = ac_section.lower()
            found_vague = [term for term in vague_terms if term in ac_text]
            if found_vague:
                self.warnings.append(f"Acceptance criteria contain vague terms: {', '.join(found_vague)} - be more specific (INVEST - Testable)")

    def check_acceptance_criteria_quality(self):
        """Check acceptance criteria quality."""
        ac_section = self.get_section_content("Acceptance Criteria")
        if not ac_section:
            return
        ac_text = ac_section.lower()

        # Check for edge cases
        if "edge case" not in ac_text and "error scenario" not in ac_text:
            self.warnings.append("No edge cases or error scenarios documented - consider adding")

        # Check for permission/authorization
        if "permission" not in ac_text and "authorized" not in ac_text:
            self.warnings.append("No permission/authorization checks documented - consider adding if applicable")

        # Check for audit trail (if mutation involved)
        mutation_keywords = ["create", "update", "delete", "change", "transition", "modify"]
        mutation_scope = "\n".join(
            [
                self.get_section_content("User Story"),
                ac_section,
            ]
        ).lower()
        if any(keyword in mutation_scope for keyword in mutation_keywords):
            if "timeline" not in ac_text and "audit" not in ac_text:
                self.warnings.append("Story involves data mutation but has no audit/timeline requirements")

    def get_section_content(self, section_name: str) -> str:
        """Return the content of a markdown section by name (## or ###)."""
        pattern = re.compile(rf"^##+\s+{re.escape(section_name)}\s*$", re.IGNORECASE | re.MULTILINE)
        match = pattern.search(self.content)
        if not match:
            return ""
        start = match.end()
        next_heading = re.search(r"^##+\s+", self.content[start:], re.MULTILINE)
        end = start + next_heading.start() if next_heading else len(self.content)
        return self.content[start:end].strip()

def collect_story_files(paths: Iterable[str]) -> Tuple[List[Path], List[str]]:
    story_files: List[Path] = []
    errors: List[str] = []

    for raw in paths:
        path = Path(raw)
        if not path.exists():
            errors.append(f"Path not found: {path}")
            continue
        if path.is_dir():
            for item in sorted(path.rglob("*.md")):
                if item.name.upper() == "STORY-INDEX.MD":
                    continue
                story_files.append(item)
        else:
            story_files.append(path)

    # Deduplicate while preserving order
    seen = set()
    unique_files = []
    for item in story_files:
        if item not in seen:
            seen.add(item)
            unique_files.append(item)

    return unique_files, errors


def main():
    if len(sys.argv) < 2:
        print("Usage: python validate-stories.py <file-or-dir> [<file-or-dir> ...]")
        print("Example: python validate-stories.py planning-mds/stories/")
        sys.exit(1)

    story_files, path_errors = collect_story_files(sys.argv[1:])

    if path_errors:
        for error in path_errors:
            print(f"❌ {error}")
        sys.exit(1)

    if not story_files:
        print("❌ No story files found to validate.")
        sys.exit(1)

    total_errors = 0
    total_warnings = 0

    for file_path in story_files:
        print(f"Validating story: {file_path}")
        print("-" * 60)

        validator = StoryValidator(str(file_path))
        is_valid, errors, warnings = validator.validate()

        if errors:
            print("\n❌ ERRORS (Must Fix):")
            for i, error in enumerate(errors, 1):
                print(f"  {i}. {error}")

        if warnings:
            print("\n⚠️  WARNINGS (Should Fix):")
            for i, warning in enumerate(warnings, 1):
                print(f"  {i}. {warning}")

        print("\n" + "=" * 60)
        if is_valid and not warnings:
            print("✅ Story validation PASSED - No issues found!")
        elif is_valid:
            print(f"⚠️  Story validation PASSED with {len(warnings)} warning(s)")
        else:
            print(f"❌ Story validation FAILED with {len(errors)} error(s) and {len(warnings)} warning(s)")

        total_errors += len(errors)
        total_warnings += len(warnings)

    if total_errors > 0:
        sys.exit(1)
    sys.exit(0)

if __name__ == "__main__":
    main()

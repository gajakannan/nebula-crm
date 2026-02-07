# Product Manager Scripts

Generic scripts for validating stories and generating indexes.

Story convention:
- One story per markdown file.

## validate-stories.py

Validate one or more story files:

```bash
python agents/product-manager/scripts/validate-stories.py planning-mds/stories/S1-example.md
python agents/product-manager/scripts/validate-stories.py planning-mds/stories/S1-example.md planning-mds/stories/S2-example.md
```

Validate a directory of stories:

```bash
python agents/product-manager/scripts/validate-stories.py planning-mds/stories/
```

## generate-story-index.py

Generate a story index for a directory:

```bash
python agents/product-manager/scripts/generate-story-index.py planning-mds/stories/
```

Outputs `planning-mds/stories/STORY-INDEX.md`.

# Product Manager Scripts

Generic scripts for validating stories and generating indexes.

## validate-stories.py

Validate a single story file:

```bash
python agents/product-manager/scripts/validate-stories.py planning-mds/stories/S1-example.md
```

Validate multiple stories:

```bash
for story in planning-mds/stories/*.md; do
  python agents/product-manager/scripts/validate-stories.py "$story"
done
```

## generate-story-index.py

Generate a story index for a directory:

```bash
python agents/product-manager/scripts/generate-story-index.py planning-mds/stories/
```

Outputs `planning-mds/stories/STORY-INDEX.md`.

# Shared JSON Schemas

This directory stores canonical JSON Schemas shared across frontend and backend.

Current baseline:
- `problem-details.schema.json` - RFC 7807 error contract with Nebula extension fields
- `broker.schema.json` - broker response contract
- `broker-create-request.schema.json` - broker creation payload
- `broker-update-request.schema.json` - broker update payload
- `broker-search-query.schema.json` - broker search query contract
- `paginated-broker-list.schema.json` - broker list response
- `contact.schema.json` - contact response contract
- `contact-create-request.schema.json` - contact creation payload
- `contact-update-request.schema.json` - contact update payload
- `dashboard-kpis.schema.json` - KPI widget response
- `dashboard-pipeline.schema.json` - pipeline summary response
- `pipeline-status-count.schema.json` - pipeline status count item
- `pipeline-mini-card.schema.json` - pipeline mini-card item
- `nudge-card.schema.json` - nudge card response item
- `task.schema.json` - task response contract
- `task-summary.schema.json` - dashboard task summary item
- `task-create-request.schema.json` - task creation payload
- `task-update-request.schema.json` - task update payload
- `timeline-event.schema.json` - activity timeline event response

Use these files as the source of truth for validation, OpenAPI alignment, and generated types.

## API schemas without JSON Schema files (intentional)

The following `components/schemas` entries in `nebula-api.yaml` do **not** have standalone JSON Schema
files because they are outside F0001/F0002 MVP implementation scope. They are read-only reference entities
or workflow entities that will gain schemas when their future features (F0005–F0008) are specified:

- `Account`, `MGA`, `Program` — reference lookups only; no create/update in MVP
- `Submission`, `Renewal` — detail + transitions only; schemas needed when F0006/F0007 land
- `WorkflowTransitionRequest`, `WorkflowTransitionRecord` — workflow payloads; schemas needed with F0006/F0007
- `PaginatedContactList` — follows the same structure as `PaginatedBrokerList`; add schema file when contact pagination testing begins

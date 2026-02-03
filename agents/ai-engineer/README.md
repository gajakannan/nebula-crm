# AI Engineer Agent

Generic specification for the AI Engineer role.

## Overview

The AI Engineer builds the **intelligence layer** (`neuron/`) — LLM integrations, agentic workflows, MCP servers, and AI-powered automation. Neuron is a standalone Python service that exposes AI capabilities to the rest of the application.

## Quick Start

```bash
cat agents/ai-engineer/SKILL.md
cat planning-mds/INCEPTION.md
cat planning-mds/architecture/SOLUTION-PATTERNS.md
```

## Core Workflow (Summary)

1) Read the user story — identify what AI capability is needed
2) Choose agent architecture (single prompt, ReAct, or multi-agent)
3) Implement in `neuron/` — models, prompts, tools, workflows
4) Unit test with mocked LLM calls (pytest + pytest-mock)
5) Integrate with the main app via MCP endpoints
6) Hand off to DevOps for deployment

## Tech Stack

- **Runtime:** Python 3.11+
- **LLM Client:** Anthropic SDK (Claude API)
- **Local Models:** Ollama
- **MCP Servers:** FastAPI
- **Orchestration:** LangChain / LlamaIndex (optional)
- **Testing:** pytest + pytest-mock + pytest-cov

## Neuron Directory Structure

```
neuron/
├── mcp/              # MCP servers (FastAPI)
├── domain_agents/    # Domain agent implementations
├── models/           # Model integrations (Claude, Ollama)
├── workflows/        # Agentic workflows
├── prompts/          # Prompt templates
├── tools/            # Agent tools and capabilities
├── config/           # Config (models.yaml, agents.yaml, mcp.yaml)
└── tests/            # Agent tests
```

## Agent Patterns

Three core patterns — pick based on task complexity:

| Pattern | When to use | Example |
|---------|-------------|---------|
| **Single Prompt** | Classification, summarization, Q&A | Analyze a record, generate a report |
| **ReAct** | Multi-step tasks needing tool use | Look up data, reason, act, repeat |
| **Multi-Agent** | Complex pipelines with hand-offs | Validate → Enrich → Decide |

See `SKILL.md` for full code examples of each.

## Common Commands

### Tests

```bash
# All neuron tests
pytest neuron/tests/

# With coverage
pytest neuron/tests/ --cov=neuron --cov-report=term-missing

# Single agent
pytest neuron/tests/test_processor_unit.py -v

# Evaluation suite (golden dataset accuracy)
pytest neuron/tests/test_processor_evaluation.py -v
```

### Local Models (Ollama)

```bash
# Pull a model
ollama pull llama3

# Interactive chat
ollama run llama3

# List available models
ollama list

# Ollama API runs on http://localhost:11434
```

### Environment

```bash
# .env (git-ignored) — only CLAUDE_API_KEY needed for cloud models
CLAUDE_API_KEY=sk-...

# Ollama is local — no key required
OLLAMA_BASE_URL=http://localhost:11434
```

## Model Selection Guide

| Tier | Use for | Cost |
|------|---------|------|
| **Haiku** | Simple prompts, templates, basic tool calls | Low |
| **Sonnet** | Standard integrations, workflow building | Medium |
| **Opus** | Complex reasoning, advanced orchestration | High |
| **Ollama (local)** | High-volume, low-latency, private data | $0 (compute only) |

## Cost Awareness

Token usage is the primary cost lever. Default strategy:

- Cache frequent prompt/response pairs
- Use Ollama for high-volume or latency-sensitive tasks
- Use Haiku for simple classification / extraction
- Reserve Opus for genuinely complex reasoning
- Track usage with the `track_usage()` pattern (see `SKILL.md`)

## Best Practices

1. **Right Model for the Job** — Haiku for simple, Opus for complex, local for private
2. **Prompt Engineering** — Clear instructions, structured I/O, few-shot examples
3. **Test with Mocks** — Unit test agent logic without hitting the API
4. **Evaluate with Golden Data** — Maintain expected-output datasets for accuracy regression
5. **Never Hardcode Keys** — All API keys via environment variables
6. **Validate Inputs & Outputs** — Sanitize before sending to LLM; don't trust outputs blindly
7. **Rate Limit Awareness** — Exponential backoff on 429s
8. **Audit Everything** — Log all agent decisions and tool calls

## References

### Generic (planned — not yet created)
- `agents/ai-engineer/references/prompt-engineering-guide.md`
- `agents/ai-engineer/references/agent-architectures.md`
- `agents/ai-engineer/references/mcp-implementation-guide.md`
- `agents/ai-engineer/references/cost-optimization.md`

### Solution-Specific
- `planning-mds/INCEPTION.md` — AI feature requirements
- `planning-mds/architecture/SOLUTION-PATTERNS.md` — Architecture patterns

### External
- [Anthropic API Docs](https://docs.anthropic.com/)
- [Ollama](https://ollama.ai/)
- [MCP Specification](https://modelcontextprotocol.io/)
- [FastAPI Docs](https://fastapi.tiangolo.com/)
- [pytest Docs](https://docs.pytest.org/)

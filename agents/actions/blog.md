# Action: Blog

## User Intent

Write development logs, technical articles, and blog posts about project progress, decisions, lessons learned, and interesting technical challenges.

## Agent Flow

```
Blogger
```

**Flow Type:** Single agent

## Prerequisites

- [ ] Something worth writing about (feature completed, decision made, problem solved, milestone reached)
- [ ] Context available (code, architecture docs, ADRs, etc.)
- [ ] Optional: Target audience identified

## Inputs

### From Planning
- `planning-mds/BLUEPRINT.md` (project context)
- `planning-mds/architecture/decisions/` (ADRs)
- User stories and features

### From Codebase
- Recent changes (git commits, PRs)
- Implementation details
- Test results
- Performance metrics

### From User
- Topic or focus area
- Target audience (developers, general tech, management)
- Blog post type (devlog, tutorial, case study, retrospective)
- Publishing destination (team blog, public blog, dev.to, etc.)

## Outputs

### Blog Posts
Types of posts:
- **DevLog:** Development progress updates, what was built, challenges faced
- **Tutorial:** How-to guides, step-by-step implementations
- **Case Study:** Deep dive into a specific problem and solution
- **Retrospective:** Lessons learned, what went well/poorly
- **Technical Deep Dive:** Detailed exploration of technical decisions
- **Architecture Decision:** Explaining key architectural choices
- **Performance:** Optimization efforts and results

### Deliverables
- Markdown blog post
- Code snippets (formatted and tested)
- Diagrams (if applicable)
- Screenshots (if applicable)
- SEO-friendly title and description
- Tags and categories

## Agent Responsibilities

### Blogger
1. **Topic Selection:**
   - Identify interesting, valuable topics
   - Consider audience and value proposition
   - Align with project goals

2. **Research:**
   - Review relevant code and docs
   - Understand technical details
   - Gather supporting data/metrics
   - Collect examples

3. **Writing:**
   - Create engaging title
   - Write clear introduction (hook + value proposition)
   - Develop main content with structure
   - Include code examples (tested and working)
   - Add visuals (diagrams, screenshots)
   - Write conclusion with takeaways
   - Add call-to-action (if applicable)

4. **Quality:**
   - Ensure technical accuracy
   - Check code examples work
   - Validate links
   - Proofread for clarity
   - Optimize for readability

5. **Publishing Prep:**
   - Format for target platform
   - Add metadata (tags, categories)
   - Create SEO-friendly description
   - Prepare social media snippets

## Validation Criteria

- [ ] Title is clear and engaging
- [ ] Introduction hooks reader and explains value
- [ ] Content is well-structured with headings
- [ ] Code examples are tested and work
- [ ] Technical details are accurate
- [ ] Visuals enhance understanding
- [ ] Conclusion summarizes key takeaways
- [ ] Post is proofread and polished
- [ ] Metadata and tags added

## Example Usage

### Scenario 1: Feature DevLog
```
User: "Write a devlog about building the customer management feature"

Blog Action:
  ↓
Blogger:
  - Reviews customer feature implementation
  - Examines user stories and architecture
  - Gathers code examples

  Post Structure:
    - Title: "Building Customer Management: Clean Architecture in Practice"
    - Introduction: Challenge of managing customer relationships
    - Feature Overview: What we built
    - Architecture Decisions: Why we chose certain patterns
    - Implementation Highlights: Code examples
    - Challenges: Authorization complexity
    - Results: Feature shipped, lessons learned
    - Conclusion: Key takeaways

  Output:
    - blog/2026/02-customer-management-feature.md
    - 1,200 words
    - 5 code examples
    - 2 diagrams
```

### Scenario 2: Technical Deep Dive
```
User: "Write about our ABAC authorization implementation"

Blog Action:
  ↓
Blogger:
  - Reviews authorization architecture
  - Studies Casbin implementation
  - Gathers metrics and examples

  Post Structure:
    - Title: "ABAC with Casbin: Fine-Grained Authorization in .NET"
    - Introduction: Authorization challenges in multi-tenant CRM
    - Why ABAC: RBAC limitations
    - Casbin Overview: What and why
    - Implementation: Code walkthrough
    - Policies: Policy examples
    - Performance: Benchmarks
    - Lessons: What we learned
    - Conclusion: When to use ABAC

  Output:
    - blog/2026/02-abac-authorization-casbin.md
    - 2,000 words
    - 8 code examples
    - 3 diagrams
    - Performance benchmarks table
```

### Scenario 3: Project Retrospective
```
User: "Write a retrospective on our Phase A experience"

Blog Action:
  ↓
Blogger:
  - Reviews Phase A outputs
  - Reflects on process
  - Identifies lessons learned

  Post Structure:
    - Title: "Phase A Retrospective: Requirements Done Right"
    - Introduction: Our approach to product planning
    - What Went Well: Vertical slicing, clear acceptance criteria
    - Challenges: Avoiding scope creep, managing ambiguity
    - Lessons Learned: 5 key insights
    - Process Changes: What we'll do differently
    - Conclusion: Advice for others

  Output:
    - blog/2026/02-phase-a-retrospective.md
    - 1,500 words
    - Lessons learned checklist
```

## Blog Post Types and Templates

### DevLog
- **Purpose:** Share development progress
- **Length:** 800-1,200 words
- **Structure:** What we built, why, how, challenges, results
- **Audience:** Team, stakeholders, developers

### Tutorial
- **Purpose:** Teach how to do something
- **Length:** 1,500-2,500 words
- **Structure:** Prerequisites, step-by-step, explanation, examples, conclusion
- **Audience:** Developers learning the topic

### Case Study
- **Purpose:** Deep dive into problem-solving
- **Length:** 1,500-2,000 words
- **Structure:** Problem, investigation, solution, results, lessons
- **Audience:** Technical professionals

### Retrospective
- **Purpose:** Reflect on experience
- **Length:** 1,000-1,500 words
- **Structure:** Context, what went well, challenges, lessons, changes
- **Audience:** Team, other practitioners

## Post-Blogging Next Steps

After blog action completes:
1. Review post for accuracy and clarity
2. Get peer review (optional)
3. Publish to target platform
4. Share on social media / team channels
5. Engage with comments and feedback

## Related Actions

- **After:** Any action - Blog about progress or learnings
- **With:** [document action](./document.md) - Docs for reference, blogs for narrative
- **Continuous:** Blog throughout the project lifecycle

## Publishing Destinations

- **Internal:** Team wiki, Confluence, Notion
- **Public:** Company blog, dev.to, Medium, Hashnode
- **Social:** LinkedIn, Twitter/X (thread format)
- **Version Control:** `blog/` or `docs/blog/` in repo

## Notes

- Blog regularly (weekly or bi-weekly) for maximum value
- Don't wait for perfection - publish and iterate
- Use blogs to document decisions and reasoning
- Blogs are great for onboarding new team members
- Consider blog posts as living documents (update over time)
- Technical blogs can become documentation later
- Celebrate wins and share learnings
- Be honest about challenges and failures (they're valuable!)

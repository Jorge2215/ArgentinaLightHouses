# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|----------|
| Architecture & design | Gandalf | System design, tech decisions, data model |
| Code review | Gandalf | Review PRs, quality gates, feedback |
| Scope & priorities | Gandalf | What to build next, trade-offs, decisions |
| Backend / C# / services | Gimli | Models, repositories, services, page models, DI, Program.cs |
| Frontend / UI / markup | Legolas | Razor Pages views (.cshtml), CSS, wwwroot, layout, accessibility |
| Testing | Aragorn | Write tests, find edge cases, integration tests, coverage |
| Session logging | Scribe | Automatic — never needs routing |
| Work queue / backlog | Ralph | Issue monitoring, PR tracking, board status |

## Issue Routing

| Label | Action | Who |
|-------|--------|-----|
| `squad` | Triage: analyze issue, assign `squad:{member}` label | Gandalf |
| `squad:gandalf` | Pick up issue and complete the work | Gandalf |
| `squad:gimli` | Pick up issue and complete the work | Gimli |
| `squad:legolas` | Pick up issue and complete the work | Legolas |
| `squad:aragorn` | Pick up issue and complete the work | Aragorn |

### How Issue Assignment Works

1. When a GitHub issue gets the `squad` label, the **Lead** triages it — analyzing content, assigning the right `squad:{member}` label, and commenting with triage notes.
2. When a `squad:{member}` label is applied, that member picks up the issue in their next session.
3. Members can reassign by removing their label and adding another member's label.
4. The `squad` label is the "inbox" — untriaged issues waiting for Lead review.

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for "what port does the server run on?"
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a feature is being built, spawn the tester to write test cases from requirements simultaneously.
7. **Issue-labeled work** — when a `squad:{member}` label is applied to an issue, route to that member. The Lead handles all `squad` (base label) triage.

---
description: Add or expand xUnit tests for PollockAgent.
---

Read CLAUDE.md. Then look through `tests/PollockAgent.Tests/` and identify code in `src/PollockAgent/` that lacks coverage — prioritize:

1. `DripValidator` — line-count limits, planning-comment rejection, edge cases (empty drip, single line, exactly at the limit).
2. `CodeCanvas` — snapshot correctness, apply for each `DripMode`, total-line counting across nested directories.
3. `AutomaticArchitect` — loop termination on line budget, on `feels_done`, on cancellation. Use a fake `ILlmClient` for these.

Write focused, behavior-named tests. Run `dotnet test` and ensure all pass before stopping.

$ARGUMENTS

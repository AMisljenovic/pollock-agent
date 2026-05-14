# PollockAgent — blueprint for AI assistants

This is an **empty canvas**. You — Claude Code, OpenAI Codex, or GitHub Copilot — are going to build it. This file is the seed: what to build, how to shape it, and the rules you'll work under.

## What you are building

PollockAgent is a .NET 9 console application — an *automatic architect*. At runtime, it writes code the way Jackson Pollock painted: not deliberate, not random. Some place in between. It grows a small platform without a fixed plan, reacting to what it has already written rather than planning what's next.

The reference is Nathan's monologue from *Ex Machina* (2014):

> Jackson Pollock. The drip painter. He let his mind go blank, and his hand go where it wanted. Not deliberate, not random. Some place in between. They called it automatic art.

The bet is that removing the planning step from code generation produces output that is weirder and more alive, not just worse.

## Conceptual model

Three things define the runtime:

**Intention seed** — a short phrase the human gives at startup. (`"a platform that grows"`, `"a tiny city"`, `"a museum of broken machines"`.) The agent never receives a more detailed brief than this.

**Canvas** — a directory on disk that holds the program being authored. State, visible between steps.

**Drip** — one atomic change to one file. Small (100–120 lines). The unit of progress.

The loop, in plain words: read the seed, read the canvas, ask the LLM for the next drip, validate it against constraints, apply it, repeat. Stop when a budget trips or the agent self-reports *feels done*.

## Required runtime constraints (enforce in code, not just prompt)

- **Max lines per drip.** Default 12. Reject larger.
- **Step timeout.** Default 30 seconds. Cancel and move on.
- **Total line budget.** Default 2000. Stop the loop when reached.
- **No planning comments.** Reject drips containing `// TODO`, `// FIXME`, `// first I'll...`, `step 1:`, etc. The agent acts; it does not narrate.
- **Feels-done flag.** The LLM may signal completion in its response; honor it.

The validator must enforce these mechanically. Don't trust the LLM to obey them just because the system prompt asks.

## Required layout

When you scaffold, use this shape:

```
PollockAgent.sln
src/PollockAgent/
  PollockAgent.csproj
  Program.cs
  Agent/
    AutomaticArchitect.cs
    IntentionSeed.cs
    Constraints.cs
    DripValidator.cs
  Canvas/
    CodeCanvas.cs
    Drip.cs
  Llm/
    ILlmClient.cs
    AnthropicLlmClient.cs
tests/PollockAgent.Tests/
  PollockAgent.Tests.csproj
  DripValidatorTests.cs
  ... (more as you go)
```

`ILlmClient` is the LLM abstraction. The default implementation calls the Anthropic Messages API at `https://api.anthropic.com/v1/messages` with model `claude-opus-4-7` (or current). Keep the client thin — raw `HttpClient` is fine; avoid heavyweight SDKs.

## Style

- C# 12 / .NET 9.
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.
- File-scoped namespaces, nullable enabled, implicit usings on.
- Records for immutable data, classes for behavior.
- `async` for all I/O. Thread `CancellationToken` through every layer.
- Test names describe behavior, not method names.

## Rules for you (the development assistant)

You are *not* under the Pollock constraints. You can plan, refactor, leave TODOs, and write 200-line files when a change calls for it. Build the project the normal, thoughtful way — including writing tests, handling edge cases, and reasoning before complex changes.

The Pollock philosophy applies to **the runtime**, not to you. You build the cage; you are not in it.

## Available commands

Command and prompt files live in `.claude/commands/`, `.codex/prompts/`, and `.github/prompts/`. They are mirrored across the three platforms.

- `/scaffold` — create the initial .NET solution and project files described above. Run this first.
- `/drip` — opt-in Pollock mode. You write code the way the runtime would. For experimentation and dogfooding only.
- `/test` — add or fill out tests for recent code.
- `/review` — read the canvas and report on what's missing relative to this blueprint.

## How to test (once scaffolded)

```bash
dotnet build
dotnet test
```

## How to run (once implemented)

```bash
export ANTHROPIC_API_KEY=sk-ant-...
dotnet run --project src/PollockAgent -- "a platform that grows"
```

The canvas the runtime produces appears in `./canvas/` (gitignored).

## Starting point

If this directory contains only this file and its siblings, run `/scaffold` first. After that, develop normally — file by file, with tests — until the project is buildable end-to-end. Don't skip tests.

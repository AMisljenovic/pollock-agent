# PollockAgent

> Jackson Pollock. The drip painter. He let his mind go blank, and his hand go where it wanted. Not deliberate, not random. Some place in between. They called it automatic art.
> — Nathan, *Ex Machina* (2014)

This repository is **an empty canvas plus a blueprint**. The code itself doesn't exist yet — Claude Code, OpenAI Codex, or GitHub Copilot will write it, guided by the instructions in `CLAUDE.md`, `AGENTS.md`, `.github/copilot-instructions.md`, and the per-tool command files.

## What gets built

PollockAgent is a .NET 9 console application — an *automatic architect* that, at runtime, writes code Pollock-style: small drips, reactive, no planning step. It takes a short intention seed and grows a platform until a budget trips or it self-reports done.

The runtime constraints (line caps, planning-comment rejection, step timeouts) are enforced *in code*, not just by prompting.

## How to use this repo

Open it in your coding assistant of choice:

| Assistant | Reads | Commands |
| --- | --- | --- |
| Claude Code | `CLAUDE.md`, `.claude/` | `/scaffold`, `/drip`, `/test`, `/review` |
| OpenAI Codex | `AGENTS.md`, `.codex/` | `/scaffold`, `/drip`, `/test`, `/review` |
| GitHub Copilot | `.github/copilot-instructions.md`, `.github/prompts/` | `/scaffold`, `/drip`, `/test`, `/review` |

All three tools see the same blueprint and the same four commands. Same content, different file conventions.

### First session

Run `/scaffold`. The assistant will create the .NET solution, projects, and folder structure described in the blueprint. After that, develop normally — file by file, with tests.

### Subsequent sessions

- `/review` — inventory the canvas against the blueprint.
- `/test` — fill in test coverage.
- `/drip` — opt-in Pollock mode (for dogfooding the philosophy, not routine work).

## Why the canvas is empty

The whole project is about the value of starting from a seed and growing reactively. The repo embodies that: a small set of instructions, and a question — what does the assistant build when this is all it has?

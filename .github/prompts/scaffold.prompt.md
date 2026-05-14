---
mode: agent
description: Create the initial .NET 9 solution and project structure for PollockAgent.
---

Read CLAUDE.md first. Then create the project structure described under "Required layout."

Steps:

1. From the repo root: `dotnet new sln -n PollockAgent`
2. `dotnet new console -n PollockAgent -o src/PollockAgent --framework net9.0`
3. `dotnet new xunit -n PollockAgent.Tests -o tests/PollockAgent.Tests --framework net9.0`
4. Add both projects to the solution.
5. Add a `ProjectReference` from the tests project to `src/PollockAgent/PollockAgent.csproj`.
6. In both `.csproj` files, set:
   - `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`
   - `<Nullable>enable</Nullable>`
   - `<ImplicitUsings>enable</ImplicitUsings>`
7. In the main project, set `<AssemblyName>pollock</AssemblyName>`.
8. Create empty subdirectories under `src/PollockAgent/`: `Agent/`, `Canvas/`, `Llm/`.
9. Verify with `dotnet build` — should succeed with zero warnings.

Do **not** start filling in the runtime logic in this step. Stop after scaffolding builds clean. The next session will start implementing the model classes.

$ARGUMENTS

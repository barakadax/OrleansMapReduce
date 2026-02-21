<!-- Workspace agent instructions for OrleansMapReduce -->
# Copilot / Agent Instructions

Purpose
-------
Provide concise, actionable instructions for AI assistants working on this repository so they can: (1) run and test the project, (2) follow project conventions, and (3) make safe, minimal changes.

Quick actions
-------------
- **Build solution:** `dotnet build`
- **Run Silo:** `dotnet run --project Silo/Silo.csproj`
- **Run Client:** `dotnet run --project Client/Client.csproj`
- **Run tests:** `dotnet test`
- **Format code:** `dotnet format`

Repository layout (high level)
-----------------------------
- **Silo/** — Orleans silo host.
- **Client/** — Console client that runs MapReduce jobs.
- **GrainInterfaces/** — Orleans grain interfaces.
- **Grains/** — Grain implementations (`TextGrain`, `WordGrain`, `NumberGrain`).
- **UnitTests/**, **FunctionalTests/** — test projects.
- **conductor/** — product docs, tracks, and workflow guidance.

Conventions & important patterns
-------------------------------
- Language: C# targeting .NET 10. Follow `conductor/code_styleguides/csharp.md` for naming and formatting.
- Indentation: 2 spaces. Braces: K&R style.
- Testing: NUnit + NSubstitute. Tests live under `UnitTests/` and `FunctionalTests/`.
- Dependency injection and Orleans patterns are used widely; avoid changes that alter activation or lifecycle without tests.

Agent behavior guidelines
------------------------
- Always run `dotnet build` and `dotnet test` after making changes that touch code or tests.
- When changing a grain or interface, update or add unit tests. Follow `conductor/workflow.md` for phase/tracking rules.
- Prefer small, focused PRs that include tests and follow the style guide.
- Do not modify generated files, build artifacts under `bin/` or `obj/`, or external configuration (unless explicitly requested).

Files to consult first
---------------------
- [README.md](README.md) — quick start and primary commands.
- [conductor/product.md](conductor/product.md) and [conductor/tech-stack.md](conductor/tech-stack.md) — goals and stack.
- [conductor/code_styleguides/csharp.md](conductor/code_styleguides/csharp.md) — code style.
- [conductor/workflow.md](conductor/workflow.md) — workflow and testing requirements.

Suggested example prompts
-------------------------
- "Run the unit tests and report failing tests and stack traces."
- "Refactor `TextGrain` to improve readability without changing behavior; add unit tests to cover the change." 
- "Add a small integration test that runs the Silo in-process and verifies a simple MapReduce result."

Next agent-customizations to consider
-----------------------------------
- Create an `applyTo` rule limiting heavy workspace-wide transformations to the `Grains/` area.
- Add a tiny helper agent (`/create-hook`) to run build+tests automatically before proposing code patches.

If you want, I can (A) commit this file, (B) add an `AGENTS.md` alternative, or (C) generate example prompts and a CI job snippet to run tests.

# Tooling

> **Owns:** how to build, test, run and verify; the documentation automation (doctool, hooks, skills, rules);
> the art pipeline; git setup notes. **Budget:** 300 lines.
> **Read when:** building, testing or automating, or wondering what a hook just did.

## Headless .NET harness (Tools/Headless, D-0007)

| Project | Compiles | Notes |
|---|---|---|
| Sim/Statecraft.Sim.Headless.csproj | `Assets/Scripts/Sim/**/*.cs` as Statecraft.Sim (netstandard2.1) | Unity's API surface; emits XML docs |
| Sim.Tests/Statecraft.Sim.Tests.Headless.csproj | `Assets/Tests/Sim/**/*.cs` (net10.0, NUnit 3.14) | the very files Unity's Test Runner runs |
| Runner/Statecraft.SimRunner.csproj | the SimRunner console tool (net10.0) | experiments and generated docs |

Shared settings live in `Tools/Headless/Directory.Build.props`: C# 9, implicit usings off, nullable off, warnings
as errors. It works with the Unity editor open, because it never touches Library/ or the project lock. NuGet
restores NUnit, NUnit3TestAdapter and Microsoft.NET.Test.Sdk from nuget.org on the first build.

From the repo root:

- `dotnet test Tools/Headless/Statecraft.Headless.slnx --nologo -v q`: build all three, run every test.
- `dotnet run --project Tools/Headless/Runner -- scripts`: list the lever scripts.
- `dotnet run --project Tools/Headless/Runner -- run --script iron-fist --years 40 --every 5`, optionally with
  `--csv out.csv` for every month of every nation.
- `dotnet run --project Tools/Headless/Runner -- docs`: regenerate PARAMETERS.md and SOCIETY_GRAPH.md.

## Compile-checking Unity-side code with the editor open

Unity writes one project file per assembly at the repo root (`Statecraft.Editor.csproj` and friends; git-ignored).
`dotnet build Statecraft.Editor.csproj -v q --nologo` compiles Sim, Game, UI and Editor against the real Unity
assemblies in about 2 seconds (measured 2026-10-14), with no lock on the project. The trap, inherited from
InsignificAnts: these files list sources explicitly, so a brand-new .cs file is not compiled until Unity regenerates
them (focus the editor, or run any batch mode); the failure then shows up as a misleading CS0246 at the call site.
Build output goes to the git-ignored `obj/` and `Temp/` folders.

## Unity batch mode (Tools/unity.ps1)

- `powershell -ExecutionPolicy Bypass -File Tools/unity.ps1 -Mode compile|test|setup [-BuildTarget Android]`
- The editor must be closed. The script exits with code 3 if the editor holds the project, and briefly waits out a
  batch run that is still shutting down.
- Logs and NUnit XML go to `Logs/batch/` (git-ignored). The script prints compiler errors and failed tests.
- Measured on Norm's PC (2026-10-14): first import with setup about 60 s; later setup about 17 s; tests about 20 s.
- Windows PowerShell's execution policy on this PC is the default (Restricted), hence `-ExecutionPolicy Bypass`,
  which applies to that one run only.

## Tools/verify.ps1

`powershell -ExecutionPolicy Bypass -File Tools/verify.ps1 [-Unity] [-Docs]` runs the headless tests and the doc
lint; `-Unity` adds Unity's edit-mode tests (editor closed); `-Docs` regenerates every generated doc first.

## Documentation automation (D-0014)

### doctool (Tools/docs/doctool.py: Python 3.10, standard library only)

| Command | Does |
|---|---|
| `brief [--refresh]` | the SessionStart brief; `--refresh` regenerates CODEMAP and the decision index first |
| `track` | PostToolUse hook: records code files Claude edits (docs drift) |
| `stop-check` | Stop hook: a notice to Norm when code has moved ahead of the docs (never blocks) |
| `pending` | lists code files changed since the last docs sync |
| `mark-synced` | clears that list; the last step of `/wrap-up` |
| `lint [--strict]` | budgets, Owns lines, links, map coverage, generated-doc freshness, ASCII UI, em dashes |
| `codemap` | regenerates Docs/generated/CODEMAP.md |
| `decisions` | regenerates Docs/decisions/INDEX.md |
| `generate [--sim]` | codemap and decisions, plus the runner's docs with `--sim` |
| `new-decision "Title"` | creates the next D-#### record with status Proposed |
| `rotate-worklog` | moves worklog entries beyond the newest ten into Docs/archive/ |

### Hooks (.claude/settings.json)

| Event | Runs | Effect |
|---|---|---|
| SessionStart (startup, resume, clear, compact) | `doctool brief --refresh` | regenerates CODEMAP and the decision index; the brief enters Claude's context |
| PostToolUse (Edit, Write, MultiEdit, NotebookEdit) | `doctool track` | records code edits; warns Claude about hand edits to Docs/generated; reminds at 8, 20 and 40 pending files |
| Stop | `doctool stop-check` | a one-line notice to Norm when the pending count changes |

Hooks run through Git Bash on this PC and start working once the workspace-trust prompt has been accepted. Only
edits made with Claude's Edit and Write tools are tracked; files changed by shell commands are not.

### Skills (.claude/skills)

| Skill | Use |
|---|---|
| `/catch-up` | orientation after a break: status, recent work, open questions |
| `/wrap-up` | the end-of-session ritual: regenerate, update docs, lint, git commands for Norm |
| `/decide <title>` | record a decision |
| `/design-review [topic]` | settle open questions with Norm and record the answers |
| `/doc-health` | a periodic deep audit, reading with cheap subagents |

### Path-scoped rules (.claude/rules)

`sim.md` (sim, tests, harness), `unity.md` (Game, UI, Editor, Assets/UI) and `docs.md` (Docs, CLAUDE.md) load
automatically when Claude reads or edits matching files.

## Art pipeline

See [../../ArtSource/README.md](../../ArtSource/README.md): `.blend` sources outside git, FBX and PNG exports
under `Assets/Art/`, wired in code.

## Git (Norm runs every git write, D-0003)

- `.gitignore` covers Unity's generated folders, root-level IDE files, harness build output, `ArtSource/`,
  `.blend` files and Claude's local state. `.gitattributes` normalizes line endings and marks binaries.
- Optional: Unity Smart Merge for scenes and prefabs. `.gitattributes` already routes YAML files to a
  `unityyamlmerge` driver; enable it by following the Unity manual page "Smart merge" with the UnityYAMLMerge
  tool in the 6000.3.13f1 install. Without it, git merges those files as plain text.

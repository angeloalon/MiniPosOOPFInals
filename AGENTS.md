# AGENTS.md

Mini-Grocery Store POS & Daily Inventory System — VB.NET Windows Forms, .NET Framework 4.7.2, SQLite. OOP finals project (ITP 102, University of Caloocan City).

## Source of truth

- `structureflow.md` (repo root) is the authoritative spec: layered architecture, OOP-pillar mapping per file, DB schema, roles/permissions, seed data, validation rules. Follow it when implementing.
- The repo currently contains only the empty VS template (`Form1`, project folder `oopfinals/oopfinals/`); the ~55 files the spec describes do not exist yet and must be built here. Verify the actual file tree before referencing spec paths like `Models/User.vb`.

## Repo layout & build

- The solution is **not** at the repo root: `oopfinals/oopfinals.sln`, project `oopfinals/oopfinals/oopfinals.vbproj`.
- NuGet packages (`System.Data.SQLite`) are committed at `oopfinals/packages/` (packages.config era); `.vbproj` HintPaths are relative (`..\packages\...`). Do not reshuffle these directories.
- Build (verified): run from `oopfinals/`
  ```
  & "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" oopfinals.sln -v:m
  ```
  or use any VS 2022 Developer PowerShell (`msbuild oopfinals.sln`).
- **`dotnet build` fails** on this project (verified: GenerateResource task-host error) — this is a classic .NET Framework project; always use full MSBuild.
- `nuget.exe` is not on PATH. Packages are already committed, so no restore is usually needed; if `oopfinals/packages/` is ever missing, restore in Visual Studio or with `nuget restore oopfinals.sln` from the solution dir.
- No tests exist. Verification = a clean MSBuild compile plus manually launching `oopfinals/oopfinals/bin/Debug/oopfinals.exe` and exercising the WinForms UI (can't be exercised headlessly).

## vbproj gotchas (old-style project)

- Every new `.vb` file **must** be added to the `<Compile Include=...>` list in `oopfinals.vbproj` — unlisted files are silently not compiled.
- The startup form is set in `My Project/Application.myapp` (`<MainForm>`). It is still `Form1`; the spec requires `LoginForm` once it exists.
- New designer-backed forms need both `X.vb` and `X.Designer.vb` entries (`.Designer.vb` with `<SubType>Form</SubType>` and `<DependentUpon>X.vb</DependentUpon>`), mirroring how `Form1` is registered.

## Code conventions (spec-mandated)

- Every new `.vb` file starts with `Option Explicit On` / `Option Strict On` on separate lines. The vbproj default is `OptionStrict Off` — do not rely on it.
- Namespaces: `oopfinals.Models`, `oopfinals.Data`, `oopfinals.Services`, `oopfinals.Controls`, `oopfinals.Forms`.
- Dependency flow is strictly one-way: `Forms → Services/Data → Models`; `Models` never references any other layer (spec §3).
- Model setters validate and throw (e.g., `Stock quantity cannot be negative.`); utility classes (`AuthService`, `ValidationRules`, `Theme`) are `NotInheritable` with a `Private Sub New()` and expose only `Shared` members.
- All SQL is parameterized (`@param` + `Parameters.AddWithValue`) — never string-concatenated.

## Runtime / database

- `grocery.db` is auto-created at `bin\Debug\grocery.db` on first run (spec §8); delete the file to rebuild and reseed demo data.
- Demo logins (seeded, spec §15): `owner`/`owner123`, `cashier`/`cashier123`, `clerk`/`clerk123`, `customer`/`customer123`.

## Git hygiene

- There is no `.gitignore`; `.vs/`, `obj/`, and `packages/` are committed. Building dirties tracked `obj/` caches and creates untracked `bin/` output — after a build, `git restore` modified caches and `git clean -f oopfinals/oopfinals/bin oopfinals/oopfinals/obj` to leave the tree clean. Don't commit new build artifacts.

## OpenCode config

- `.opencode/agents/` holds repo-local subagent definitions (`code-reviewer.md`, etc.) — edit those files, not a global config, when tuning agents.



## Project rules (read before writing any code)

- Source of truth: `structureflow.md`. If the code and that doc disagree, ask before changing either.
- Stack: VB.NET, Windows Forms, .NET Framework 4.7.2, SQLite via System.Data.SQLite.Core. `Option Strict On` and `Option Explicit On` in every file. Do not use .NET 5+ only APIs or packages.
- Structure: Forms/, Controls/, Services/, Data/, Models/. Forms hold UI only. Business logic goes in Services, SQL goes in Data repositories.
- OOP is graded: use inheritance, polymorphism, and interfaces as described in structureflow.md. Role routing must use polymorphism, not an if/else chain.

### Data and money
- Use `Decimal` for all money. Round with `Math.Round(x, 2, MidpointRounding.AwayFromZero)`.
- Use parameterized SQL only. Never build SQL with string concatenation.
- A sale is one transaction: guarded stock update (`... WHERE StockQty >= @qty`, check rows affected), insert the sale and items, write the stock movement. Roll back on any failure and rethrow the original error.
- Guard against an empty cart and a total of 0 or less before any SQL runs.
- Passwords: salted PBKDF2, never plain text. Never log or display passwords.

### UI rules
- Every form opens maximized.
- Anchor or Dock every control. Money labels use AutoSize. Dropdowns are `DropDownList`. No tiny fixed-size panels for money text.
- Test at 100% and 125% Windows display scaling.
- Use the shared theme and styled controls in Controls/. Don't style individual forms ad hoc.

### Workflow
- Build in small milestones. After each one: the solution must compile with 0 errors and the feature must run before moving on.
- Don't add NuGet packages without asking.
- Don't change files outside the current milestone.
- Handle errors with specific, user-friendly messages. Don't swallow exceptions.
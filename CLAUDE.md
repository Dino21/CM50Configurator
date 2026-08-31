# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

CM50 Configurator is a Windows Forms desktop app (Sesami) for browsing and editing the CM50 device
configuration tables in a MySQL database (`cm50`, `cm50usercustomization`, and the "collaudo" tables
`ddmcm50`/`utmcm50`/`ltmcm50`/`feedercm50`). Target framework is .NET Framework 4.8, classic
(non-SDK-style) csproj.

## Build & run

There is no test project in the solution.

```powershell
# Locate MSBuild (dotnet CLI does not build this classic csproj)
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe

# Debug build (bin\Debug\CM50-Configurator.exe)
& $msbuild CM50-Configurator.slnx /t:Build /p:Configuration=Debug

# Release build, used for the installer (bin\Release\CM50-Configurator.exe)
& $msbuild CM50-Configurator.slnx /t:Rebuild /p:Configuration=Release
```

This is a classic-style csproj: every new `.cs` file must be added explicitly to
`CM50-Configurator/CM50-Configurator.csproj` under `<Compile Include="...">` or it will not compile.

### Installer

Inno Setup script: `installer/CM50-Configurator.iss`. Rebuild after a Release build:

```powershell
& "C:\Users\Dino.Martorana\AppData\Local\Programs\Inno Setup 6\ISCC.exe" installer\CM50-Configurator.iss
```

Output goes to `installer/Output/` (git-ignored). The script embeds `App.config` — including the live
MySQL connection string in plaintext — into the installer, so treat the compiled `.exe` as sensitive,
same as `App.config` itself.

## Architecture

### Two module families

- **FK-heavy tables** (`cm50`, `cm50usercustomization`): bespoke `Cm50Repository`/`UserCustomizationRepository`
  + `DatabaseWriter`, with hand-designed forms (`Cm50Form`/`Form1` + their `*DetailForm`/`*ReviewChangesForm`/`New*Form`
  companions, all VS-Designer-generated).
- **"Simple" flat tables** (`ddmcm50`/`utmcm50`/`ltmcm50`/`feedercm50`): a single generic set —
  `SimpleTableListForm<T>` / `SimpleEntityDetailForm<T>` / `SimpleTableReviewForm<T>` / `SimpleTableWriter`,
  parameterized per entity via a `SimpleTableConfig<T>` built by each `*Repository.CreateWriterConfig()`.
  These forms build their UI in code (`BuildUi()`) rather than a Designer.cs, because the VS designer
  doesn't support generic forms.

`cm50` references DD/Asset/CDF by **code** (string); `cm50usercustomization` references the same
reference tables by **id**. Don't conflate the two when touching queries or writers.

### Reference data resolution

`ReferenceDataCache` (static, repopulated after every "Carica da DB") + `ReferenceSync` (looks up a
typed code against the cache; fills related fields on a match, leaves them null to signal "new") back
custom `TypeConverter`s (`DdReferenceConverter`, `AssetReferenceConverter`, `CdfReferenceConverter`,
`UnitConfigurationConverter`, `CassetteReferenceConverter`, `ReaderReferenceConverter`). These let the
PropertyGrid resolve a typed-in code to an existing reference or flag it as new, expandable to edit the
individual fields either way.

`DatabaseWriter`/`SimpleTableWriter`'s `Ensure*` methods use the same lookup at write time: an
unresolvable code opens `NewReferenceForm` to complete the required fields before inserting. Each row
commits in its own transaction, so one bad row doesn't block the rest of a batch.

### Universal edit workflow

Every module follows the same cycle: Carica da DB / Importa JSON populates an in-memory `List<T>`
(`_currentData`) and clones a `_baseline` snapshot → `Cm50Diff` / `UserCustomizationDiff` /
`SimpleEntityDiff` diff current-vs-baseline → a review form shows only new/changed rows (color-coded) →
explicit "Scrivi su DB" commits, "Annulla" discards. Nothing touches the database before that final
confirmation — bulk operations (JSON import, "Sostituisci Codice...") only ever mutate `_currentData`.

`BulkCodeReplaceForm` is fully generic: it offers a field for bulk replace only when
`TypeDescriptor.GetProperties(entityType)`'s `PropertyDescriptor.Converter.CanConvertFrom(typeof(string))`
is true — the exact same test the PropertyGrid itself relies on. A new entity gets bulk-replace support
automatically for any field whose `TypeConverter` already supports parsing from a string; no per-table
wiring needed.

### Theming and a WinForms footgun to avoid

`SesamiTheme.Apply(form)` is called by every form right after construction; it applies the dark
Sesami theme and embedded brand fonts, and sets `AutoScaleMode = None` before changing `Font` (must stay
in that order — changing `Font` with `AutoScaleMode.Font` still active rescales child control bounds
using stale metrics).

Separately, and only in the hand-built (`BuildUi()`-based) forms: never add a child control with
`Anchor` including `Right`/`Bottom` to a `Panel` *before* that panel itself has been added to the form's
`Controls` (which is what gives it its real, Dock-resized size). Doing it in the wrong order captures the
anchor's "distance from edge" against the panel's default/unsized dimensions, and the control silently
renders off-screen — this exact bug made the "Chiudi" button in `SimpleEntityDetailForm` and the
"Scrivi su DB"/"Annulla" buttons in `SimpleTableReviewForm` unreachable until fixed. Always
`Controls.Add(panel)` on the form first, then add anchored children to `panel.Controls`.

### Filtering, sorting, JSON

`GridFilter` (Support/GridFilter.cs) is a generic substring filter over all public properties via
reflection, shared by all three list forms; every grid is always sorted by code after filtering.

`JsonExportService`/`JsonImportService` share a `JsonSerializerSettings` with a custom
`ContractResolver` that forces object (not `ToString()`) serialization for every type in the `Models`
namespace — required because those types carry `ExpandableObjectConverter`-family `TypeConverter`s that
Newtonsoft would otherwise treat as string-convertible.

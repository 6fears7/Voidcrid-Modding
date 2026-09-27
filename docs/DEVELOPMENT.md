# Development

Voidcrid is a BepInEx plugin for Risk of Rain 2. It reworks Acrid's skills, adds skins and
death events, and ships as a Thunderstore package.

## Repository layout

```
Voidcrid.sln            Solution root
nuget.config            BepInEx + nuget.org package sources

src/Voidcrid/           The plugin assembly (netstandard2.1 -> Voidcrid.dll)
  VoidcridPlugin.cs       BepInPlugin entry point, config binding, hook registration
  Core/                   Cross-cutting helpers (Log)
  Init/                   Startup wiring: SkillSetup, Config, Language, Helpers
  Skills/                 EntityState implementations for each skill
  Achievements/           UnlockableDef achievement conditions
  Modules/                Custom damage types and death effects

libs/                   Vendored assemblies for soft-dependency mods only.
                        Everything else (game, BepInEx, R2API, MMHOOK) comes from NuGet.
assets/acrid3           AssetBundle built by the Unity project; copied next to Voidcrid.dll.
                        Committed prebuilt — recovered from the shipped 1.6.0 package, because the
                        copy previously in the repo was a stale one-asset stub.
packaging/              Thunderstore package metadata (manifest.json, icon.png)
unity/                  ThunderKit / Unity project that authors the AssetBundle
docs/                   This file, plus README images in docs/media/
attic/                  Fully commented-out experiments kept for reference. Not compiled.
```

## Building the plugin

```
dotnet restore
dotnet build Voidcrid.sln -c Release # generaqtes the zip bundle too in dist/
```

## Version bumps

```
dotnet build Voidcrid.sln -c Release -p:Bump=patch|minor|major
```

`packaging/manifest.json`'s `version_number` is the single source of truth for the mod version.
The csproj's `<Version>` is read from it at evaluation time, so the flag rewrites only that one
file, then builds and packages the new version in the same invocation. The `[BepInPlugin]`
version and `obj/**/PluginVersion.g.cs` are both generated from `<Version>`, so neither needs
hand-editing, and nothing in the csproj itself ever changes.

The flag rewrites that one tracked file on disk; review the diff and commit it after a bump.
A plain `dotnet build` (no `-p:Bump`) never touches the version. The solution has a single
project, so the bump runs exactly once per build.
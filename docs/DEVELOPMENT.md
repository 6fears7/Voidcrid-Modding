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
dotnet build -c Release
```

## Version bumps

1. `<Version>` in `src/Voidcrid/Voidcrid.csproj`
2. `version_number` in `packaging/manifest.json`
3. the third argument to `[BepInPlugin]` in `src/Voidcrid/VoidcridPlugin.cs`

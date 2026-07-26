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
assets/acrid3           AssetBundle built by the Unity project; copied next to Voidcrid.dll
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

Output lands in `src/Voidcrid/bin/Release/netstandard2.1/`, with `acrid3` copied alongside
`Voidcrid.dll` — that pairing matters, because `SkillSetup.LoadAssetBundle` locates the bundle by
taking its own assembly path and replacing `Voidcrid.dll` with `acrid3`.

To test in game, copy both files into `Risk of Rain 2/BepInEx/plugins/Voidcrid/`.

## Dependency rules

There are two distinct classes of reference, and mixing them is what made the old `libs/` folder
27 MB of duplicates:

- **NuGet** — the game assemblies (`RiskOfRain2.GameLibs`), `UnityEngine.Modules`, `BepInEx.Core`,
  `MMHOOK.RoR2` and every `R2API.*` module. Never vendor these; a stale local copy silently wins
  over the package and produces confusing type-identity errors.
- **`libs/`** — mods that are not published to NuGet and are referenced as *soft* dependencies:
  AncientScepter, the three Spikestrip content packs, and SkillsPlusPlus (`Skills.dll`). These are
  referenced with `Private=false` so they are never copied into the build output.

Adding a hard dependency means updating three places in step: the `PackageReference` in
`src/Voidcrid/Voidcrid.csproj`, the `dependencies` array in `packaging/manifest.json`, and the
`[BepInDependency]` attribute on `VoidcridDef`.

### Version targets

`RiskOfRain2.GameLibs` and `UnityEngine.Modules` are not "keep on latest" packages — they must
track the game as shipped. To check what that is:

```
strings -n 5 "<steam>/Risk of Rain 2/Risk of Rain 2_Data/globalgamemanagers" | head
```

The first lines give the Unity version (`2021.3.33f1`) and the game version (`1.4.1`). Pick the
`GameLibs` release matching the latter and the `UnityEngine.Modules` release matching the former.
Bumping Unity ahead of the engine the game runs will compile and then fail at runtime.

R2API modules are published to nuget.org and Thunderstore in lockstep, so the `PackageReference`
version and the `RiskofThunder-R2API_*` manifest entry should always be the same number.

## The Unity project

`unity/` is a ThunderKit project that produces the `acrid3` AssetBundle (skill icons, skin
textures, meshes). It is only needed when changing art; plugin-only work does not require opening
Unity. The built bundle is committed to `assets/` so the plugin can be built without it.

`unity/Assets/ThunderKitSettings/ThunderKitSettings.asset` contains a `GamePath` pointing at a
local Steam install. It is machine-specific — expect to repoint it, and avoid committing your own
path unless it is genuinely portable.

## Version bumps

The mod version appears in three places that must agree:

1. `<Version>` in `src/Voidcrid/Voidcrid.csproj`
2. `version_number` in `packaging/manifest.json`
3. the third argument to `[BepInPlugin]` in `src/Voidcrid/VoidcridPlugin.cs`

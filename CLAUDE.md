# Voidcrid

A BepInEx plugin for Risk of Rain 2 that reworks Acrid. Distributed on Thunderstore.

Full layout, build steps and dependency rules: [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md).

## Orientation

- `src/Voidcrid/` — the only compiled code. Entry point is `VoidcridDef` in `VoidcridPlugin.cs`;
  skill registration fans out from `Init/SkillSetup.cs`.
- `attic/` — commented-out dead code, not compiled. Don't fix it, don't count it as prior art.
- `unity/` — ThunderKit project that builds the `assets/acrid3` AssetBundle. Only touch it for art.
- `libs/` — vendored DLLs for **soft-dependency mods only**. Game/BepInEx/R2API assemblies come
  from NuGet; adding one to `libs/` reintroduces the duplicate-reference problem that was just
  cleaned up.

## Build

```
dotnet build Voidcrid.sln -c Release
```

`acrid3` is copied next to `Voidcrid.dll` in the output; the runtime loader finds the bundle by
string-replacing `Voidcrid.dll` in its own assembly path, so they must stay siblings.

## Known state (2026-07)

The repo structure is current, but the code is not. Pinned versions predate several game updates
and are the starting point for the fix pass:

- `RiskOfRain2.GameLibs` 1.3.2-r.1 and `MMHOOK.RoR2` 2024.9.5 in `src/Voidcrid/Voidcrid.csproj`
- `unity/` targets Unity 2019.4.26f1 while the plugin builds against `UnityEngine.Modules` 2021.3.33
- `packaging/manifest.json` omits R2API_Unlockable and R2API_Loadout, both of which the plugin
  references

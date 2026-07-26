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
string-replacing `Voidcrid.dll` in its own assembly path, so they must stay siblings. Every build
also mirrors both into `src/Voidcrid/bin/live/`, which an r2modman profile symlinks to for
in-game testing — see docs/DEVELOPMENT.md.

## Target versions (verified 2026-07-26)

Pinned against the shipping game: **RoR2 1.4.1 on Unity 2021.3.33f1**, confirmed by reading
`globalgamemanagers` from a local Steam install. `RiskOfRain2.GameLibs 1.4.1-r.0` matches it
exactly, and `UnityEngine.Modules` must stay on 2021.3.33 — do not bump it to 2022.x just because
newer packages exist; it has to track the engine the game actually ships.

`packaging/manifest.json` and the csproj `PackageReference`s are in sync and both current.

## Known state

The project builds clean against current packages — 0 errors, 0 warnings. Keep it that way.

Void-item counting goes through `Core/VoidItems.cs`, not `Inventory.GetItemCount` (obsolete, since
stacks can now be temporary). Live effects call `CountEffective`; anything gating a permanent unlock
calls `CountPermanent`. Modded damage types are written to `ProjectileDamage.damageType` via
`TagProjectile` in `Modules/DamageTypes/Death.cs` — the old `ModdedDamageTypeHolderComponent` is
obsolete, and unlike it the field-based API needs the prefab to actually have a `ProjectileDamage`.

`DamageTypes.voidcridDeath2` and `voidcridPoison` are reserved and attached but never read; only
`voidcridDeath` is checked (`Init/Helpers.cs`). They are inert markers, not dead code to remove.

Separately, `unity/` still targets Unity 2019.4.26f1 — three minor versions behind the engine the
game runs. That only blocks AssetBundle work, not plugin builds, since `assets/acrid3` is
committed prebuilt.

`assets/acrid3` was a 9 KB stub containing a single unused texture (`laserVoid.png`) while the code
loads six sprites from it; the real 930 KB bundle survived only inside the installed Thunderstore
package and has been restored. The root-level copy was lost in `1ce03e1 "Removed old stuff"`. The
bundle is a build output with no source of truth in the repo until `unity/` can build it again, so
treat it as irreplaceable.

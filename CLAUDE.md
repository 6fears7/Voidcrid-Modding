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

The project compiles cleanly against current packages (0 errors). What remains is three
deprecations, all of which are behavior changes rather than renames:

- **`Inventory.GetItemCount(ItemDef)`** → `GetItemCountEffective` / `GetItemCountPermanent`.
  28 call sites in `Skills/Death/VoidDeath.cs` and `Achievements/VoidcridAchievement.cs`. The
  split exists because items can now be temporary; picking the wrong one silently changes
  gameplay, so each call site needs a deliberate choice rather than a blanket replace.
- **`DamageAPI.ModdedDamageTypeHolderComponent`** → set `ProjectileDamage.damageType` directly.
  `Modules/DamageTypes/Death.cs`.
- `Entropy.instance` is an unused field (`Skills/Entropy.cs`), pre-existing.

Separately, `unity/` still targets Unity 2019.4.26f1 — three minor versions behind the engine the
game runs. That only blocks AssetBundle work, not plugin builds, since `assets/acrid3` is
committed prebuilt.

`assets/acrid3` was a 9 KB stub containing a single unused texture (`laserVoid.png`) while the code
loads six sprites from it; the real 930 KB bundle survived only inside the installed Thunderstore
package and has been restored. The root-level copy was lost in `1ce03e1 "Removed old stuff"`. The
bundle is a build output with no source of truth in the repo until `unity/` can build it again, so
treat it as irreplaceable.

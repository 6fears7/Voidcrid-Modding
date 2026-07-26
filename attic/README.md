# attic

Dead code kept for reference. Every file here is entirely commented out — this directory was
`src/Voidcrid/Unused/` and contributed nothing to the build, so it was moved out of the compile
tree rather than deleted.

- `BaseCharge.cs` — charge-up base state, superseded by the per-skill states in `src/Voidcrid/Skills/`
- `VoidSlash.cs` — melee skill that never shipped
- `VoidcridPassive.cs` — earlier passive implementation; the shipped one lives in `SkillSetup`
- `VoidcridSkinDef.cs` — skin registration via embedded AssetBundle, before the loose-file `acrid3` approach
- `VoidcridCharacterDef.cs` — empty

Nothing here has been compiled against a current game version. Treat it as notes, not as code.

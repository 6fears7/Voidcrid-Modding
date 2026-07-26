using System;
using System.Linq;
using System.Reflection;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace Voidcrid
{
    /// <summary>
    /// Acrid's two custom skins: Voidcrid and Blackrid.
    /// </summary>
    /// <remarks>
    /// Ported from the prebuilt Voidcrid_Skins.dll, which had no source in this repo and stopped
    /// working on RoR2 1.4.1. It resolved its renderers positionally — <c>renderers[2]</c> and
    /// <c>renderers[3]</c> of <c>GetComponentsInChildren&lt;Renderer&gt;(true)</c> on mdlCroco —
    /// and the model now returns only three: "Goo", "CrocoMesh", "CrocoSpineMesh". The array had
    /// shifted down by one, so every skin threw IndexOutOfRangeException while registering and
    /// none of them appeared in the lobby. This looks them up by name instead.
    ///
    /// Registration goes through R2API rather than appending to ModelSkinController.skins by
    /// hand: R2API owns the SkinDef.Awake suppression the old code hand-rolled, and builds the
    /// SkinDefParams that 1.4.1 actually reads from.
    /// </remarks>
    internal static class SkinSetup
    {
        private const string BundleFileName = "voidcrid_skins";
        private const string BodyName = "CrocoBody";

        private const string BodyRendererName = "CrocoMesh";
        private const string SpineRendererName = "CrocoSpineMesh";

        // Every skin replaces the body mesh with the same sculpt.
        private const string BodyMeshPath = "Assets/Resources/FINALLFXD.mesh";

        // The bundle's materials were authored against an editor stub, so the real game shader has
        // to be reattached at runtime or the skins render with the wrong lighting model.
        private const string StandardShaderKey = "RoR2/Base/Shaders/HGStandard.shader";

        private static AssetBundle assetBundle;

        /// <summary>One skin's art and unlock, as recovered from the old assembly.</summary>
        private readonly struct SkinSpec
        {
            internal SkinSpec(
                string name,
                string nameToken,
                string iconPath,
                string bodyMaterialPath,
                string spineMaterialPath,
                string unlockableName)
            {
                Name = name;
                NameToken = nameToken;
                IconPath = iconPath;
                BodyMaterialPath = bodyMaterialPath;
                SpineMaterialPath = spineMaterialPath;
                UnlockableName = unlockableName;
            }

            internal string Name { get; }
            internal string NameToken { get; }
            internal string IconPath { get; }
            internal string BodyMaterialPath { get; }
            internal string SpineMaterialPath { get; }

            /// <summary>Null leaves the skin available from the start.</summary>
            internal string UnlockableName { get; }
        }

        private static readonly SkinSpec[] Specs =
        {
            new SkinSpec(
                "Voidcrid",
                "SIXFEARS7_SKIN_VOIDCRID_NAME",
                @"Assets\SkinMods\Voidcrid_Skins\Icons\VoidcridIcon.png",
                "Assets/Resources/mouthfix.mat",
                "Assets/Resources/pink.mat",
                // Ships free.
                null),
            new SkinSpec(
                "Blackrid",
                "SIXFEARS7_SKIN_BLACKRID_NAME",
                @"Assets\SkinMods\Voidcrid_Skins\Icons\BlackridIcon.png",
                "Assets/Resources/Blackrid.mat",
                "Assets/Resources/demoneyes.mat",
                // Earned by the Grandfather Paradox achievement. The unlockable is *named*
                // "Skins.Croco.Voidcrid" because ParadoxAchievement registered it before the
                // skins were assigned to achievements; the name is a save-file key, so renaming
                // it to match would drop the unlock for everyone who already has it.
                "Skins.Croco.Blackrid"),
        };

        internal static void Init()
        {
            if (!LoadBundle())
            {
                return;
            }

            BodyCatalog.availability.CallWhenAvailable(AddSkins);
        }

        private static bool LoadBundle()
        {
            string path = Assembly.GetExecutingAssembly().Location.Replace("Voidcrid.dll", BundleFileName);

            assetBundle = AssetBundle.LoadFromFile(path);
            if (!assetBundle)
            {
                Log.LogError($"Could not load the skin AssetBundle at \"{path}\"; skins will be unavailable.");
                return false;
            }

            return true;
        }

        private static void AddSkins()
        {
            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(BodyName);
            if (!bodyPrefab)
            {
                Log.LogWarning($"No body prefab named \"{BodyName}\"; skipping skins.");
                return;
            }

            ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
            Transform modelTransform = modelLocator ? modelLocator.modelTransform : null;
            if (!modelTransform)
            {
                Log.LogWarning($"\"{BodyName}\" has no ModelLocator.modelTransform; skipping skins.");
                return;
            }

            GameObject modelObject = modelTransform.gameObject;
            Renderer bodyRenderer = FindRenderer(modelObject, BodyRendererName);
            Renderer spineRenderer = FindRenderer(modelObject, SpineRendererName);

            if (!bodyRenderer || !spineRenderer)
            {
                Log.LogWarning($"\"{modelObject.name}\" is missing \"{BodyRendererName}\" or "
                    + $"\"{SpineRendererName}\"; skipping skins rather than painting the wrong renderer.");
                return;
            }

            Shader standardShader = Addressables.LoadAssetAsync<Shader>(StandardShaderKey).WaitForCompletion();
            Mesh bodyMesh = assetBundle.LoadAsset<Mesh>(BodyMeshPath);

            foreach (SkinSpec spec in Specs)
            {
                AddSkin(spec, bodyPrefab, modelObject, bodyRenderer, spineRenderer, bodyMesh, standardShader);
            }
        }

        private static void AddSkin(
            SkinSpec spec,
            GameObject bodyPrefab,
            GameObject modelObject,
            Renderer bodyRenderer,
            Renderer spineRenderer,
            Mesh bodyMesh,
            Shader standardShader)
        {
            Material bodyMaterial = LoadMaterial(spec.BodyMaterialPath, standardShader);
            Material spineMaterial = LoadMaterial(spec.SpineMaterialPath, standardShader);

            if (!bodyMaterial || !spineMaterial)
            {
                Log.LogWarning($"Skipping skin \"{spec.Name}\": its materials are missing from the bundle.");
                return;
            }

            var info = new SkinDefParamsInfo
            {
                Name = spec.Name,
                NameToken = spec.NameToken,
                Icon = assetBundle.LoadAsset<Sprite>(spec.IconPath),
                UnlockableDef = FindUnlockableDef(spec.UnlockableName),
                RootObject = modelObject,
                BaseSkins = Array.Empty<SkinDef>(),
                RendererInfos = new[]
                {
                    NewRendererInfo(bodyRenderer, bodyMaterial),
                    NewRendererInfo(spineRenderer, spineMaterial),
                },
                MeshReplacements = bodyMesh
                    ? new[] { new SkinDefParams.MeshReplacement { renderer = bodyRenderer, mesh = bodyMesh } }
                    : Array.Empty<SkinDefParams.MeshReplacement>(),
            };

            if (!Skins.AddSkinToCharacter(bodyPrefab, info))
            {
                Log.LogWarning($"R2API rejected skin \"{spec.Name}\".");
            }
        }

        private static CharacterModel.RendererInfo NewRendererInfo(Renderer renderer, Material material) =>
            new CharacterModel.RendererInfo
            {
                renderer = renderer,
                defaultMaterial = material,
                defaultShadowCastingMode = ShadowCastingMode.Off,
                ignoreOverlays = false,
            };

        private static Material LoadMaterial(string path, Shader standardShader)
        {
            Material material = assetBundle.LoadAsset<Material>(path);

            if (material && standardShader)
            {
                material.shader = standardShader;
            }

            return material;
        }

        private static Renderer FindRenderer(GameObject modelObject, string rendererName)
        {
            return modelObject
                .GetComponentsInChildren<Renderer>(true)
                .FirstOrDefault(renderer => renderer && renderer.name == rendererName);
        }

        private static UnlockableDef FindUnlockableDef(string unlockableName)
        {
            if (string.IsNullOrEmpty(unlockableName))
            {
                return null;
            }

            UnlockableDef unlockableDef = ContentManager.unlockableDefs
                .FirstOrDefault(def => def && def.cachedName == unlockableName);

            if (!unlockableDef)
            {
                Log.LogWarning($"No unlockable named \"{unlockableName}\"; that skin will be available from the start.");
            }

            return unlockableDef;
        }
    }
}

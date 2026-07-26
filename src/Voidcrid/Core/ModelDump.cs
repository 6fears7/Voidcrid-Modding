using RoR2;
using UnityEngine;

namespace Voidcrid
{
    /// <summary>
    /// TEMPORARY diagnostic. Dumps CrocoBody's renderer layout to the log once at startup.
    /// </summary>
    /// <remarks>
    /// Two questions need answering from a live 1.4.1 game, and neither can be answered from the
    /// stripped reference assemblies:
    ///
    /// 1. Which entries of <c>GetComponentsInChildren&lt;Renderer&gt;(true)</c> are the body mesh
    ///    and the second painted renderer. Voidcrid_Skins asks for indices 2 and 3 of an array
    ///    that now has 3 entries, which is why all three skins fail to register.
    /// 2. Which entry of <see cref="CharacterModel.baseRendererInfos"/> is the emissive spine, i.e.
    ///    whether the old hardcoded index 1 that <see cref="EmissiveGlow"/> falls back to is still
    ///    the right one, and whether the "Spine" name match finds it first.
    ///
    /// This reads the prefab rather than a live in-run model, so it only needs a launch to the
    /// main menu. The instantiated model copies both arrays from the prefab, so the answers hold.
    ///
    /// Delete this file and its call in VoidcridPlugin.Awake once the layout is recorded.
    /// </remarks>
    internal static class ModelDump
    {
        private const string BodyName = "CrocoBody";

        internal static void Schedule()
        {
            BodyCatalog.availability.CallWhenAvailable(Dump);
        }

        private static void Dump()
        {
            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(BodyName);
            if (!bodyPrefab)
            {
                Log.LogWarning($"[ModelDump] No body prefab named \"{BodyName}\".");
                return;
            }

            ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
            Transform modelTransform = modelLocator ? modelLocator.modelTransform : null;
            if (!modelTransform)
            {
                Log.LogWarning($"[ModelDump] \"{BodyName}\" has no ModelLocator.modelTransform.");
                return;
            }

            DumpHierarchyRenderers(modelTransform.gameObject);
            DumpBaseRendererInfos(modelTransform.GetComponent<CharacterModel>());
        }

        // The array Voidcrid_Skins indexes positionally. Order is the model's hierarchy order,
        // which the game is free to change between updates.
        private static void DumpHierarchyRenderers(GameObject modelObject)
        {
            Renderer[] renderers = modelObject.GetComponentsInChildren<Renderer>(true);

            Log.LogInfo($"[ModelDump] {modelObject.name}: GetComponentsInChildren<Renderer>(true) returned {renderers.Length}");

            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (!renderer)
                {
                    Log.LogInfo($"[ModelDump]   renderers[{i}] = <destroyed>");
                    continue;
                }

                Log.LogInfo($"[ModelDump]   renderers[{i}] = \"{renderer.name}\" ({renderer.GetType().Name})"
                    + $", active={renderer.gameObject.activeSelf}"
                    + $", material=\"{(renderer.sharedMaterial ? renderer.sharedMaterial.name : "<none>")}\"");
            }
        }

        // The array the glow skills index. Distinct from the hierarchy order above.
        private static void DumpBaseRendererInfos(CharacterModel characterModel)
        {
            if (!characterModel)
            {
                Log.LogWarning("[ModelDump] Model has no CharacterModel component.");
                return;
            }

            CharacterModel.RendererInfo[] rendererInfos = characterModel.baseRendererInfos;
            if (rendererInfos == null)
            {
                Log.LogWarning("[ModelDump] CharacterModel.baseRendererInfos is null.");
                return;
            }

            Log.LogInfo($"[ModelDump] CharacterModel.baseRendererInfos has {rendererInfos.Length} entries");

            for (int i = 0; i < rendererInfos.Length; i++)
            {
                CharacterModel.RendererInfo rendererInfo = rendererInfos[i];

                Log.LogInfo($"[ModelDump]   baseRendererInfos[{i}]"
                    + $" renderer=\"{(rendererInfo.renderer ? rendererInfo.renderer.name : "<none>")}\""
                    + $" material=\"{(rendererInfo.defaultMaterial ? rendererInfo.defaultMaterial.name : "<none>")}\""
                    + $" ignoreOverlays={rendererInfo.ignoreOverlays}");
            }
        }
    }
}

using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace Voidcrid
{
    /// <summary>
    /// Acrid's glowing spine material, as used by the skills that pulse it while firing.
    /// </summary>
    /// <remarks>
    /// Two things make the raw <c>baseRendererInfos[1].defaultMaterial</c> lookup unsafe.
    ///
    /// The index is positional: it is whatever slot the glow renderer happened to occupy in the
    /// prefab when the skill was written, and the game is free to reorder or shorten that array
    /// in an update. Unguarded in <c>OnEnter</c> that is an <see cref="System.IndexOutOfRangeException"/>
    /// on the first line of the state, which kills the skill silently for the rest of the run.
    /// So resolve by renderer name first and fall back to the old index only if the name misses.
    ///
    /// The material is a shared prefab asset, not a per-character instance, so writing to it hits
    /// every Acrid in the game and outlives the skill. The old code "restored" it by setting
    /// emission to black, which is not what the material started as — each activation permanently
    /// dimmed it. <see cref="Restore"/> puts back the values captured before the first write.
    /// </remarks>
    internal class EmissiveGlow
    {
        // Matches "CrocoSpineMesh" and the alt-skin "CrocoVultureSpineMesh"; skins that rename
        // the renderer fall through to the index below.
        private const string RendererNameFragment = "Spine";
        private const int FallbackRendererIndex = 1;

        private static readonly int EmColorProperty = Shader.PropertyToID("_EmColor");

        // Keyed by material, not by state instance: the same shared asset is driven by three
        // different skills, so the pristine values have to survive overlapping activations.
        private static readonly Dictionary<Material, PristineState> pristineStates =
            new Dictionary<Material, PristineState>();

        private readonly Material material;

        private EmissiveGlow(Material material)
        {
            this.material = material;
        }

        /// <summary>
        /// Resolves the glow material off a character model, or null if this model has none.
        /// Callers must handle null — a missing glow is cosmetic, and no reason to drop the skill.
        /// </summary>
        internal static EmissiveGlow Acquire(Transform modelTransform)
        {
            CharacterModel characterModel = modelTransform ? modelTransform.GetComponent<CharacterModel>() : null;
            Material glow = characterModel ? FindGlowMaterial(characterModel) : null;

            if (!glow)
            {
                Log.LogWarning("No emissive glow material on this model; skill visuals will be skipped.");
                return null;
            }

            RememberPristineState(glow);
            return new EmissiveGlow(glow);
        }

        internal void SetEmission(Color color)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor(EmColorProperty, color);
        }

        internal void Restore()
        {
            if (!pristineStates.TryGetValue(material, out PristineState pristine))
            {
                return;
            }

            material.SetColor(EmColorProperty, pristine.emissionColor);

            if (pristine.emissionEnabled)
            {
                material.EnableKeyword("_EMISSION");
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }
        }

        private static Material FindGlowMaterial(CharacterModel characterModel)
        {
            CharacterModel.RendererInfo[] rendererInfos = characterModel.baseRendererInfos;

            if (rendererInfos == null)
            {
                return null;
            }

            foreach (CharacterModel.RendererInfo rendererInfo in rendererInfos)
            {
                if (rendererInfo.renderer
                    && rendererInfo.renderer.name.Contains(RendererNameFragment)
                    && rendererInfo.defaultMaterial)
                {
                    return rendererInfo.defaultMaterial;
                }
            }

            return FallbackRendererIndex < rendererInfos.Length
                ? rendererInfos[FallbackRendererIndex].defaultMaterial
                : null;
        }

        private static void RememberPristineState(Material material)
        {
            if (pristineStates.ContainsKey(material))
            {
                return;
            }

            pristineStates[material] = new PristineState
            {
                emissionColor = material.HasProperty(EmColorProperty)
                    ? material.GetColor(EmColorProperty)
                    : Color.black,
                emissionEnabled = material.IsKeywordEnabled("_EMISSION"),
            };
        }

        private struct PristineState
        {
            internal Color emissionColor;
            internal bool emissionEnabled;
        }
    }
}

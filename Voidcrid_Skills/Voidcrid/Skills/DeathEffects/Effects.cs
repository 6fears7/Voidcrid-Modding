#pragma warning disable Publicizer001
using R2API;
using RoR2;
using UnityEngine;

using UnityEngine.Networking;

namespace Voidcrid.Effects {
	public static class EffectProvider {

		public static GameObject VoidcridSilentKill { get; private set; }


		internal static void Init() {
			On.RoR2.HealthComponent.AssetReferences.Resolve += InterceptHealthCmpAssetReferences;

		}


		private static void InterceptHealthCmpAssetReferences(On.RoR2.HealthComponent.AssetReferences.orig_Resolve originalMethod) {
			originalMethod();
			VoidcridSilentKill = PrefabAPI.InstantiateClone(HealthComponent.AssetReferences.critGlassesVoidExecuteEffectPrefab, "VoidcridSilentKill");
			VoidcridSilentKill.AddComponent<NetworkIdentity>();
			EffectComponent fx = VoidcridSilentKill.GetComponentInChildren<EffectComponent>();
			fx.soundName = null;
			ContentAddition.AddEffect(VoidcridSilentKill);
			On.RoR2.HealthComponent.AssetReferences.Resolve -= InterceptHealthCmpAssetReferences;
		}

	}
}

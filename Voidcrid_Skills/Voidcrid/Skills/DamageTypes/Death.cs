using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using R2API;
using RoR2.Projectile;
using RoR2;
using UnityEngine.Networking;
using static R2API.DamageAPI;

namespace Voidcrid.Modules {
	public static class VoidcridDeathProjectile {

		public static GameObject VoidcridDeath { get; private set; }



		internal static void Init() {

			VoidcridDeath = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierDeathBombProjectile.prefab").WaitForCompletion(), "VoidcridDeathBomb");
			ModdedDamageTypeHolderComponent holder = VoidcridDeath.AddComponent<ModdedDamageTypeHolderComponent>();
			holder.Add(DamageTypes.voidcridDeath);
			ContentAddition.AddProjectile(VoidcridDeath);

		}
	}
}

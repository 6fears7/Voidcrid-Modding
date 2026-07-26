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

namespace Voidcrid.Modules
{
    public static class VoidcridDeathProjectile
    {

        public static GameObject VoidcridDeath { get; private set; }
        public static GameObject VoidcridDeath2 { get; private set; }

        public static GameObject VoidcridPoison { get; private set; }


        internal static void Init()
        {

            VoidcridDeath = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegacrabAntimatterExplosion.prefab").WaitForCompletion(), "VoidcridDeathBomb");
            VoidcridDeath2 = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierDeathBombProjectile.prefab").WaitForCompletion(), "VoidcridDeathBomb2");

            VoidcridPoison = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoLeapAcid.prefab").WaitForCompletion(), "VoidcridPoisonPool");

            TagProjectile(VoidcridDeath, DamageTypes.voidcridDeath);
            TagProjectile(VoidcridDeath2, DamageTypes.voidcridDeath2);
            TagProjectile(VoidcridPoison, DamageTypes.voidcridPoison);
            ContentAddition.AddProjectile(VoidcridDeath);
            ContentAddition.AddProjectile(VoidcridDeath2);
            ContentAddition.AddProjectile(VoidcridPoison);


        }

        /// <summary>
        /// Marks a projectile prefab with a ModdedDamageType.
        /// </summary>
        /// <remarks>
        /// Replaces the obsolete ModdedDamageTypeHolderComponent, which R2API now implements as a
        /// shim over this same field. Unlike the old component, this needs a ProjectileDamage to
        /// write into, so a prefab without one is reported rather than throwing.
        /// </remarks>
        private static void TagProjectile(GameObject projectilePrefab, ModdedDamageType damageType)
        {
            if (!projectilePrefab.TryGetComponent(out ProjectileDamage projectileDamage))
            {
                Log.LogError($"{projectilePrefab.name} has no ProjectileDamage component; " +
                             "its modded damage type will not be applied.");
                return;
            }

            projectileDamage.damageType.AddModdedDamageType(damageType);
        }
    }
}

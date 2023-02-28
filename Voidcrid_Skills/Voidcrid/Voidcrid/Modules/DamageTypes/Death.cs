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

            ModdedDamageTypeHolderComponent holder = VoidcridDeath.AddComponent<ModdedDamageTypeHolderComponent>();
            holder.Add(DamageTypes.voidcridDeath);
            ModdedDamageTypeHolderComponent holder2 = VoidcridDeath.AddComponent<ModdedDamageTypeHolderComponent>();
            holder2.Add(DamageTypes.voidcridDeath2);
            ModdedDamageTypeHolderComponent poisoner = VoidcridPoison.AddComponent<ModdedDamageTypeHolderComponent>();
            poisoner.Add(DamageTypes.voidcridPoison);
            ContentAddition.AddProjectile(VoidcridDeath);
            ContentAddition.AddProjectile(VoidcridDeath2);
            ContentAddition.AddProjectile(VoidcridPoison);


        }
    }
}

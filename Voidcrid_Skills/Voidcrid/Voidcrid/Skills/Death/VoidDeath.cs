using RoR2.Projectile;
using UnityEngine.Networking;
using UnityEngine;
using EntityStates;
using RoR2;
namespace Voidcrid.Modules
{

    public class DeathState : GenericCharacterDeath
    {

        public override bool shouldAutoDestroy => false;

        private string MuzzleName = "MouthMuzzle";
        private Transform muzzleTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            Vector3 vector = Vector3.up * 3f;
            if (base.characterMotor)
            {
                vector += base.characterMotor.velocity;
                base.characterMotor.enabled = false;
            }
            if (base.cachedModelTransform)
            {
                RagdollController component = base.cachedModelTransform.GetComponent<RagdollController>();
                if (component)
                {
                    component.BeginRagdoll(vector);
                }
            }
            muzzleTransform = FindModelChild(MuzzleName);
            if (isAuthority)
            {
                FireProjectileInfo fireProjectileInfo = default;
                //Reward corruption with cool death anims
                if (getVoidItemCount(characterBody.inventory) >= 3)
                {
                    fireProjectileInfo.projectilePrefab = VoidcridDeathProjectile.VoidcridDeath;
                }
                else if (getVoidItemCount(characterBody.inventory) >= 7)
                {
                    fireProjectileInfo.projectilePrefab = VoidcridDeathProjectile.VoidcridDeath2;
                }
                fireProjectileInfo.position = characterBody.corePosition;
                fireProjectileInfo.rotation = Quaternion.LookRotation(characterDirection.forward, Vector3.up);
                fireProjectileInfo.owner = gameObject;
                fireProjectileInfo.damage = damageStat;
                fireProjectileInfo.crit = characterBody.RollCrit();
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);


                FireProjectileInfo firePoison = default;
                firePoison.projectilePrefab = VoidcridDeathProjectile.VoidcridPoison;
                firePoison.position = characterBody.corePosition;
                firePoison.rotation = Quaternion.LookRotation(characterDirection.forward, Vector3.up);
                firePoison.owner = gameObject;
                firePoison.damage = damageStat;
                firePoison.crit = characterBody.RollCrit();
                ProjectileManager.instance.FireProjectile(firePoison);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= 6f)
            {
                DestroyModel();
                if (NetworkServer.active)
                {
                    DestroyBodyAsapServer();
                }
            }
        }

        public override void OnExit()
        {
            DestroyModel();
            base.OnExit();
        }


        //Reuse code cuz lazy
        private int getVoidItemCount(Inventory inventory)
        {
            int output = 0;
            output += inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ElementalRingVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ExplodeOnDeathVoid);
            output += inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ChainLightningVoid);
            output += inventory.GetItemCount(DLC1Content.Items.TreasureCacheVoid);
            output += inventory.GetItemCount(DLC1Content.Items.MushroomVoid);
            output += inventory.GetItemCount(DLC1Content.Items.BearVoid);
            output += inventory.GetItemCount(DLC1Content.Items.SlowOnHitVoid);
            output += inventory.GetItemCount(DLC1Content.Items.MissileVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoid);
            output += inventory.GetItemCount(DLC1Content.Items.BleedOnHitVoid);
            output += inventory.GetItemCount(DLC1Content.Items.CloverVoid);
            output += inventory.GetItemCount(DLC1Content.Items.VoidMegaCrabItem);
            return output;
        }

    }
}

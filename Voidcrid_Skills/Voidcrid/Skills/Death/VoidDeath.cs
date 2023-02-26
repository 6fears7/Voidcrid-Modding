using RoR2.Projectile;
using UnityEngine.Networking;
using UnityEngine;
using EntityStates;
using RoR2;
namespace Voidcrid.Modules {
    
	public class DeathState : GenericCharacterDeath {

		public override bool shouldAutoDestroy => false;

        private string MuzzleName = "MouthMuzzle";
		private Transform muzzleTransform;

		public override void OnEnter() {
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
			if (isAuthority) {
				FireProjectileInfo fireProjectileInfo = default;
				fireProjectileInfo.projectilePrefab = VoidcridDeathProjectile.VoidcridDeath;
				fireProjectileInfo.position = characterBody.footPosition;
				fireProjectileInfo.rotation = Quaternion.LookRotation(characterDirection.forward, Vector3.up);
				fireProjectileInfo.owner = gameObject;
				fireProjectileInfo.damage = damageStat;
				fireProjectileInfo.crit = characterBody.RollCrit();
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (fixedAge >= 5f) {
				DestroyModel();
				if (NetworkServer.active) {
					DestroyBodyAsapServer();
				}
			}
		}

		public override void OnExit() {
			DestroyModel();
			base.OnExit();
		}

	}
}

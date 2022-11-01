// EntityStates.Bandit2.StealthMode
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.Bandit2;
using UnityEngine.AddressableAssets;


namespace Voidcrid {
public class VoidEscape : StealthMode
{
    
    private new GameObject smokeBombEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathBombExplosion.prefab").WaitForCompletion();
    private new string smokeBombMuzzleString = "MuzzleCenter";

    private GameObject explosionPrefab =  Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathBombExplosion.prefab").WaitForCompletion();
	
	public float voidJailChance = 5f;
	public override void OnEnter()
	{
		base.OnEnter();
        
		animator = GetModelAnimator();
		_ = (bool)animator;
		if ((bool)base.characterBody)
		{
			if (NetworkServer.active)
			{
				base.characterBody.AddBuff(RoR2Content.Buffs.Cloak);
				base.characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
			}
			base.characterBody.onSkillActivatedAuthority += OnSkillActivatedAuthority;
		}

		FireSmokebomb();


		Util.PlaySound(enterStealthSound, base.gameObject);
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
        

        
		if (base.fixedAge > duration)
		{
			outer.SetNextStateToMain();
		}
	}

	public override void OnExit()
	{
		if (!outer.destroying)
		
		{
			FireSmokebomb();

		}


		Util.PlaySound(exitStealthSound, base.gameObject);
		if ((bool)base.characterBody)
		{
			base.characterBody.onSkillActivatedAuthority -= OnSkillActivatedAuthority;
			if (NetworkServer.active)
			{
				base.characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
				base.characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
			}
		}
		if ((bool)animator)
		{
			animator.SetLayerWeight(animator.GetLayerIndex("Body, StealthWeapon"), 0f);
		}
		base.OnExit();
	}

	private new void OnSkillActivatedAuthority(GenericSkill skill)
	{
		if (skill.skillDef.isCombatSkill)
		{
			outer.SetNextStateToMain();
		}
	}

	private new void FireSmokebomb()
	{
		if (base.isAuthority)
		{
			BlastAttack obj = new BlastAttack
			{
				radius = blastAttackRadius,
				procCoefficient = blastAttackProcCoefficient,
				position = base.transform.position,
				attacker = base.gameObject,
				crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
				baseDamage = 1,
				falloffModel = BlastAttack.FalloffModel.None,
				damageType =  (Util.CheckRoll(voidJailChance, base.characterBody.master) ? DamageType.VoidDeath : DamageType.SlowOnHit),
				baseForce = blastAttackForce

			};
			obj.teamIndex = TeamComponent.GetObjectTeam(obj.attacker);
			obj.attackerFiltering = AttackerFiltering.NeverHitSelf;
			

			obj.Fire();
		}
		if ((bool)smokeBombEffectPrefab)
		{
		EffectManager.SimpleMuzzleFlash(smokeBombEffectPrefab, base.gameObject, smokeBombMuzzleString, transmit: false);
		Vector3 footPosition = base.characterBody.footPosition;

		EffectManager.SpawnEffect(explosionPrefab, new EffectData
		{
			origin = footPosition,
			scale = blastAttackRadius
		}, transmit: false);



		}
		if ((bool)base.characterMotor)
		{
			base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, shortHopVelocity, base.characterMotor.velocity.z);
		}
		

	}


	public override InterruptPriority GetMinimumInterruptPriority()
	{
		return InterruptPriority.Skill;
	}

}
}
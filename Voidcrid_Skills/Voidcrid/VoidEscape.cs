// EntityStates.Bandit2.StealthMode
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.Bandit2;
using UnityEngine.AddressableAssets;
using EntityStates.VoidRaidCrab.Weapon;



namespace Voidcrid {
public class VoidEscape : StealthMode
{

	 private TemporaryVisualEffect voidFog;
	//  Addressables.LoadAssetAsync<GameObject>("Prefabs/TemporaryVisualEffects/voidFogMildEffect").WaitForCompletion();
	public static DamageReport onCharacterDeathGlobal;

	public CharacterBody body;

    [SerializeField]
	
    private new GameObject smokeBombEffectPrefab =  Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathPreExplosion.prefab").WaitForCompletion();
    private new string smokeBombMuzzleString = "MuzzleCenter";
	[SerializeField]
    private GameObject explosionPrefab =  Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathBombExplosion.prefab").WaitForCompletion();
	
	private  FireGravityBump FGBSound = new FireGravityBump();

	public GameObject voidFogInstance = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VoidFogMildEffect.prefab").WaitForCompletion();
	public float voidJailChance = 3f;

	private DamageType seasonalAttack;
	private bool hasBuff = false;
	public override void OnEnter()
	{
		base.OnEnter();
		
       	// voidFogInstance = Object.Instantiate(voidFogInstance, FindModelChild("MouthMuzzle"));
		if (VoidcridDef.Seasonal.Value == true) {

			seasonalAttack = DamageType.Freeze2s;

		} else if (VoidcridDef.Seasonal.Value == false) {

			seasonalAttack = DamageType.Generic;

		}
		animator = GetModelAnimator();
		_ = (bool)animator;
	
	if (NetworkServer.active) {
				characterBody.AddBuff(RoR2Content.Buffs.Cloak);
				characterBody.AddBuff(RoR2Content.Buffs.VoidFogStrong);
				characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
						
	}
			hasBuff = true;	
			base.characterBody.onSkillActivatedAuthority += OnSkillActivatedAuthority;
	

		FireSmokebomb();

		Util.PlaySound(FGBSound.enterSoundString, base.gameObject);
		
		base.characterBody.UpdateSingleTemporaryVisualEffect(ref voidFog, "Prefabs/TemporaryVisualEffects/voidFogMildEffect", characterBody.radius,true);


		// Hack to pull the visual state without actually updating the effect
	
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


		Util.PlaySound(FGBSound.enterSoundString, base.gameObject);
		if (NetworkServer.active && hasBuff)
		{
				characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
				characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
				characterBody.RemoveBuff(RoR2Content.Buffs.VoidFogStrong);
				
			 
		}
			hasBuff = false;
			base.characterBody.onSkillActivatedAuthority -= OnSkillActivatedAuthority;

		if ((bool)animator)
		{
			animator.SetLayerWeight(animator.GetLayerIndex("Body, StealthWeapon"), 0f);
		}
        base.characterBody.UpdateSingleTemporaryVisualEffect(ref voidFog, "Prefabs/TemporaryVisualEffects/voidFogMildEffect", characterBody.radius,false);
		base.OnExit();
	}

	private new void OnSkillActivatedAuthority(GenericSkill skill)
	{

		if (skill.isCombatSkill)
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
				baseDamage = VoidcridDef.EtherealDriftOverrideDamage.Value,
				falloffModel = BlastAttack.FalloffModel.None,
				damageType =  (Util.CheckRoll(VoidcridDef.EtherealDriftOverrideJailChance.Value, base.characterBody.master) ? DamageType.VoidDeath : seasonalAttack),
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
		}, transmit: true);



		}
		if ((bool)base.characterMotor)
		{
			base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, shortHopVelocity *1.7f, base.characterMotor.velocity.z);
		}
		

	}


	public override InterruptPriority GetMinimumInterruptPriority()
	{
		return InterruptPriority.Skill;
	}


	
}

}
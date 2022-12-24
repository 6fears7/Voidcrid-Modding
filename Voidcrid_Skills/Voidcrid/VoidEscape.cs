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

	[Command]
void CmdGiveBuffToClient(BuffDef buffDef, GameObject characterBody)
{
    // Add the buff to the character body
    characterBody.GetComponent<CharacterBody>().AddBuff(buffDef);
}
	public override void OnEnter()
	{
		base.OnEnter();
		
		CrocoDamageTypeController blarg = new CrocoDamageTypeController();
		
		
       	// voidFogInstance = Object.Instantiate(voidFogInstance, FindModelChild("MouthMuzzle"));
		if (VoidcridDef.Seasonal.Value == true) {

			seasonalAttack = DamageType.Freeze2s;

		} else if (VoidcridDef.Seasonal.Value == false) {

			seasonalAttack = DamageType.Generic;

		}
		animator = GetModelAnimator();
		_ = (bool)animator;

			if (base.characterBody)
			{
				if (NetworkServer.active)
				{   Debug.Log("Active Server check");
					base.characterBody.AddBuff(RoR2Content.Buffs.Cloak);
					base.characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
					base.characterBody.AddBuff(RoR2Content.Buffs.VoidFogStrong);

    				// Send the command to the client to give the buff
					// CmdGiveBuffToClient(RoR2Content.Buffs.Cloak, base.characterBody.gameObject);
					// CmdGiveBuffToClient(RoR2Content.Buffs.CloakSpeed, base.characterBody.gameObject);
					// CmdGiveBuffToClient(RoR2Content.Buffs.VoidFogStrong, base.characterBody.gameObject);
				}
				Debug.Log("Active Server check has failed again");


	

			hasBuff = true;	
			base.characterBody.onSkillActivatedAuthority += OnSkillActivatedAuthority;
			}
	

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

			if (base.characterBody)
			{
		if (NetworkServer.active && hasBuff)
		{
				characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
				characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
				characterBody.RemoveBuff(RoR2Content.Buffs.VoidFogStrong);
				
			 
		}
			hasBuff = false;
			base.characterBody.onSkillActivatedAuthority -= OnSkillActivatedAuthority;
			}

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
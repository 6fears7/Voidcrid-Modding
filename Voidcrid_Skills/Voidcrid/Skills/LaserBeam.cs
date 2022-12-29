using EntityStates;
using RoR2;
using UnityEngine;
using EntityStates.Mage.Weapon;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using EntityStates.VoidSurvivor.Weapon;
using RoR2.Items;

namespace Voidcrid {


public class NullBeam : BaseSkillState
{
	

	private string muzzle = "MouthMuzzle";

	private float baseDuration = 1f;

	// private GameObject ice = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallPillarProjectile.prefab").WaitForCompletion();

	private float maxDistance = 100f;

	private float minDistance = 1f;

	// private float voidJailChance = 0.3f;

	// private string attackSound;
	// private string endAttackSoundString = Voidcridbreath.endAttackSoundString;

	private float recoilAmplitude = 1f;

	private float bulletRadius = 1f;


	private GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamImpactCorrupt.prefab").WaitForCompletion();

	private GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureTracer.prefab").WaitForCompletion();
	private float spreadBloomValue = 0.3f;

	// private float maximumDuration = 2.5f;

	private float procCoefficientPerSecond = 0.1f;

	private float forcePerSecond = 2f;

	public ItemIndex index;



	private float minimumDuration;


	private float maxSpread = 2f;

    private DamageType deeprotDamage;
     private DamageType passiveAttack;
    private float rotAttack = 8f;

	private CrocoDamageTypeController crocoDamageTypeController;

	private float switchAttacks = 50f;

	private DamageType voidAttack;


	public override void OnEnter()
	{

		base.OnEnter();
		crocoDamageTypeController =  GetComponent<CrocoDamageTypeController>();

		minimumDuration = baseDuration / attackSpeedStat;
		
		 if (NetworkServer.active)
		 {
			base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
			base.characterBody.AddBuff(RoR2Content.Buffs.SmallArmorBoost);
		 }
		PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", VoidcridDef.NullBeamOverrideDuration.Value);		// Util.PlaySound(FireGravityBump.enterSoundString, base.gameObject);
		
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		FireBullet();
		Ray aimRay = GetAimRay();
		base.characterBody.SetAimTimer(3f);

			Vector3 point = GetAimRay().GetPoint(maxDistance + attackSpeedStat);
			if (Util.CharacterRaycast(base.gameObject, GetAimRay(), out var hitInfo, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
			{
				point = hitInfo.point;
			}
			
			if(base.isAuthority && ( fixedAge >= VoidcridDef.NullBeamOverrideDuration.Value || ( fixedAge >= baseDuration && !IsKeyDownAuthority() ) ) )		{
			outer.SetNextStateToMain();
		}
	}

	public override void OnExit()
	{

		if (NetworkServer.active)
		{
			base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
			base.characterBody.RemoveBuff(RoR2Content.Buffs.SmallArmorBoost);
		}
		base.OnExit();
	}

	private void FireBullet()
	{
		Ray aimRay = GetAimRay();
		AddRecoil(-1f * recoilAmplitude, -2f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
		bool hasDeeprot = VoidcridDef.HasDeeprot(base.skillLocator);

		if (hasDeeprot) {
		passiveAttack = (Util.CheckRoll(rotAttack, base.characterBody.master) ? crocoDamageTypeController.GetDamageType() : DamageType.Generic);

		} else {

			passiveAttack = DamageType.Generic;
		}

		voidAttack =  (Util.CheckRoll(VoidcridDef.NullBeamOverrideJailChance.Value, base.characterBody.master) ? DamageType.VoidDeath : DamageType.Generic);

		deeprotDamage = (Util.CheckRoll(switchAttacks, base.characterBody.master) ? passiveAttack : DamageType.Generic);


		if (base.isAuthority)
		{
			BulletAttack bulletAttack = new BulletAttack();
			bulletAttack.owner = base.gameObject;
			bulletAttack.weapon = base.gameObject;
			bulletAttack.origin = aimRay.origin;
			bulletAttack.aimVector = aimRay.direction;
			bulletAttack.muzzleName = muzzle;
			bulletAttack.maxDistance = Mathf.Lerp(minDistance, maxDistance, Random.value);
			bulletAttack.minSpread = 1f;
			bulletAttack.maxSpread = maxSpread;
			bulletAttack.radius = bulletRadius;
			bulletAttack.smartCollision = false;
			bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
			bulletAttack.hitMask = LayerIndex.entityPrecise.mask;
			bulletAttack.damage = VoidcridDef.NullBeamOverrideDamage.Value * damageStat;
			bulletAttack.procCoefficient = procCoefficientPerSecond;
			bulletAttack.force = forcePerSecond;
			bulletAttack.damageType = (Util.CheckRoll(switchAttacks, base.characterBody.master) ? deeprotDamage : voidAttack);
			bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
			bulletAttack.hitEffectPrefab = hitEffectPrefab;
			bulletAttack.tracerEffectPrefab = tracerEffectPrefab;
			bulletAttack.Fire();
		}
		base.characterBody.AddSpreadBloom(spreadBloomValue);
	}

	public override InterruptPriority GetMinimumInterruptPriority()
	{
		return InterruptPriority.Skill;
	}
	}
}
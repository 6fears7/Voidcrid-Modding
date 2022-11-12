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

	// private GameObject flashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/VoidRaidCrabMuzzleflashEyeMissiles.prefab").WaitForCompletion();

	private float maxDistance = 100f;

	private float minDistance = 1f;

	private float voidJailChance = 0.1f;

	// private string attackSound;
	// private string endAttackSoundString = Voidcridbreath.endAttackSoundString;

	private float recoilAmplitude = 1f;

	private float bulletRadius = 1f;


	private GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamImpactCorrupt.prefab").WaitForCompletion();

	private GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureTracer.prefab").WaitForCompletion();
	private float spreadBloomValue = 0.3f;

	private float maximumDuration = 2.5f;

	private float procCoefficientPerSecond = 0.1f;

	private float forcePerSecond = 2f;

	public ItemIndex index;



	private float minimumDuration;

	private float damageCoefficientPerSecond = 0.5f;

	private float maxSpread = 2f;

	// private GameObject blinkVfxInstance;

	public override void OnEnter()
	{

	
		base.OnEnter();
		minimumDuration = baseDuration / attackSpeedStat;
		
		if (NetworkServer.active)
		{
			base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
			base.characterBody.AddBuff(RoR2Content.Buffs.SmallArmorBoost);
		}
		PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", maximumDuration);		// Util.PlaySound(FireGravityBump.enterSoundString, base.gameObject);
		// blinkVfxInstance = Object.Instantiate(beamVfxPrefab);
		// blinkVfxInstance.transform.SetParent(base.characterBody.aimOriginTransform, worldPositionStays: false);
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		FireBullet();
		Ray aimRay = GetAimRay();
		base.characterBody.SetAimTimer(3f);
		// if ((bool)blinkVfxInstance)
		// {
			// EffectData effectData = new EffectData
			// {
			// 	origin = aimRay.origin,
			// 	start = aimRay.origin,
			// 	scale = bulletRadius
			// };
			Vector3 point = GetAimRay().GetPoint(maxDistance + attackSpeedStat);
			if (Util.CharacterRaycast(base.gameObject, GetAimRay(), out var hitInfo, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
			{
				point = hitInfo.point;
			}
			// blinkVfxInstance.transform.forward = point - blinkVfxInstance.transform.position;
			// EffectManager.SpawnEffect(beamVfxPrefab, effectData, transmit: true);
if(base.isAuthority && ( fixedAge >= maximumDuration || ( fixedAge >= baseDuration && !IsKeyDownAuthority() ) ) )		{
			outer.SetNextStateToMain();
		}
	}

	public override void OnExit()
	{
		// Util.PlaySound(ChargeNovabomb. fire, base.gameObject);
		// if ((bool)blinkVfxInstance)
		// {
		// 	VfxKillBehavior.KillVfxObject(blinkVfxInstance);
		// }
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
			bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
			bulletAttack.hitMask = LayerIndex.entityPrecise.mask;
			bulletAttack.damage = damageCoefficientPerSecond * damageStat;
			bulletAttack.procCoefficient = procCoefficientPerSecond;
			bulletAttack.force = forcePerSecond;
			bulletAttack.damageType = (Util.CheckRoll(voidJailChance, base.characterBody.master) ? DamageType.VoidDeath : DamageType.Generic);
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
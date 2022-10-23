using EntityStates;
using RoR2;
using UnityEngine;
using EntityStates.MajorConstruct.Weapon;
using EntityStates.VoidSurvivor.Weapon;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace CustomSkillsTutorial.Voidcrid
{
    public class NullBeam : BaseSkillState
    {

	private string muzzle = "MouthMuzzle";
	private float baseDuration = 1f;

private GameObject beamVfxPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamCorrupt.prefab").WaitForCompletion();
	private GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamMuzzleflash.prefab").WaitForCompletion();
   
	private float maxDistance = 100f;
	private float minDistance = 1f;

	private float recoilAmplitude = 1f;

	private float bulletRadius = 3f;

	private GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXFMJ.prefab").WaitForCompletion();
	
	private float spreadBloomValue = 0.3f;
	private float minimumDuration;

	private float procCoefficientPerSecond = 0.5f;
	private float forcePerSecond = 2f;

	private float damageCoefficientPerSecond = 0.5f;

	private float maxSpread = 2f;

	private GameObject blinkVfxInstance;
	

		public override void OnEnter()
	{

		base.OnEnter();

		minimumDuration = baseDuration / attackSpeedStat;

		if (NetworkServer.active)
		{
			base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
			base.characterBody.AddBuff(RoR2Content.Buffs.SmallArmorBoost);
		}

		PlayCrossfade("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", baseDuration, 0.05f);
		PlayCrossfade("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", baseDuration, 0.05f);

		blinkVfxInstance = Object.Instantiate(beamVfxPrefab);
		blinkVfxInstance.transform.SetParent(base.characterBody.aimOriginTransform, worldPositionStays: false);

	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();

		FireBullet();
		
		base.characterBody.SetAimTimer(3f);
		if ((bool)blinkVfxInstance)
		{
	
			Vector3 point = GetAimRay().GetPoint(maxDistance + attackSpeedStat);
			if (Util.CharacterRaycast(base.gameObject, GetAimRay(), out var hitInfo, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
			{
				point = hitInfo.point;
			}
			blinkVfxInstance.transform.forward = point - blinkVfxInstance.transform.position;
		}
		if (((base.fixedAge >= minimumDuration && !IsKeyDownAuthority())) && base.isAuthority)
		{
			outer.SetNextStateToMain();
		}
	}

	public override void OnExit()
	{
		if ((bool)blinkVfxInstance)
		{
			VfxKillBehavior.KillVfxObject(blinkVfxInstance);
		}
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
		if ((bool)muzzleflashEffectPrefab)
		{
			EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, muzzle, transmit: false);
		}
		if (base.isAuthority)
		{
			BulletAttack bulletAttack = new BulletAttack();
			bulletAttack.owner = base.gameObject;
			bulletAttack.weapon = base.gameObject;
			bulletAttack.origin = aimRay.origin;
			bulletAttack.aimVector = aimRay.direction;
			bulletAttack.muzzleName = muzzle;
			bulletAttack.maxDistance = Mathf.Lerp(minDistance, maxDistance, Random.value);
			bulletAttack.minSpread = 0f;
			bulletAttack.maxSpread = maxSpread;
			bulletAttack.radius = bulletRadius;
			bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
			bulletAttack.smartCollision = false;
			bulletAttack.stopperMask = default(LayerMask);
			bulletAttack.hitMask = LayerIndex.entityPrecise.mask;
			bulletAttack.damage = damageCoefficientPerSecond * damageStat;
			bulletAttack.procCoefficient = procCoefficientPerSecond;
			bulletAttack.force = forcePerSecond;
			bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
			bulletAttack.hitEffectPrefab = hitEffectPrefab;
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
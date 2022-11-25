using EntityStates;
using RoR2;
using UnityEngine;
using EntityStates.LemurianBruiserMonster;
using R2API.Networking;
using RoR2.Items;
using UnityEngine.Networking;



//Since we are using effects from Commando's Barrage skill, we will also be using the associated namespace
//You can also use Addressables or LegacyResourcesAPI to load whichever effects you like

namespace Voidcrid
{
    public class Voidcridbreath : Flamebreath
    {

	private static new float maxDistance = 20f;

	private new float baseFlamethrowerDuration = 2f;

	[SerializeField]
	private  new GameObject flamethrowerEffectPrefab = Flamebreath.flamethrowerEffectPrefab;
	[SerializeField]
	private new string startAttackSoundString = Flamebreath.startAttackSoundString;

	[SerializeField]
	private new string endAttackSoundString = Flamebreath.endAttackSoundString;

	private new const float flamethrowerEffectBaseDistance = 15f;

      
        public override void OnEnter()
        {

		
            base.OnEnter();

	stopwatch = 0f;
	entryDuration = baseEntryDuration;
	exitDuration = baseExitDuration;
	flamethrowerDuration = VoidcridDef.FlamebreathOverrideDuration.Value + attackSpeedStat;
	Transform modelTransform = GetModelTransform();
	if ((bool)base.characterBody)
	{
		base.characterBody.SetAimTimer(entryDuration + flamethrowerDuration + 1f);
	}
	if ((bool)modelTransform)
	{
		childLocator = modelTransform.GetComponent<ChildLocator>();
		modelTransform.GetComponent<AimAnimator>().enabled = true;
	}
	float num = flamethrowerDuration * tickFrequency;
	tickDamageCoefficient = totalDamageCoefficient / num;

	PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", flamethrowerDuration);
	Ray aimRay = GetAimRay();

	// if (flamethrowerEffectPrefab)
    //         {
    //             EffectManager.SimpleMuzzleFlash(tracerEffectPrefab, base.gameObject, "MouthMuzzle", true);
    //          }
	// if (NetworkServer.active  &&
	if  (muzzleTransform)
	{
		BulletAttack bulletAttack = new BulletAttack();
		bulletAttack.owner = base.gameObject;
		bulletAttack.weapon = base.gameObject;
		bulletAttack.origin = aimRay.origin;
		bulletAttack.aimVector = aimRay.direction;
		bulletAttack.minSpread = 0f;
		bulletAttack.maxSpread = maxSpread;
		bulletAttack.damage = tickDamageCoefficient * damageStat;
		bulletAttack.force = force;
		bulletAttack.muzzleName = "MouthMuzzle";
		bulletAttack.hitEffectPrefab = impactEffectPrefab;
		bulletAttack.isCrit = isCrit;
		bulletAttack.radius = radius;
		bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
		bulletAttack.stopperMask = LayerIndex.world.mask;
		bulletAttack.procCoefficient = procCoefficientPerTick;
		bulletAttack.maxDistance = maxDistance;
		bulletAttack.tracerEffectPrefab = flamethrowerEffectPrefab;
		bulletAttack.smartCollision = true;
		bulletAttack.damageType = (Util.CheckRoll(ignitePercentChance, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic);
		bulletAttack.Fire();
		isCrit = Util.CheckRoll(critStat, base.characterBody.master);

	}
    }

        public override void OnExit()
        {
	    	Util.PlaySound(endAttackSoundString, base.gameObject);
	PlayCrossfade("Gesture, Override", "BufferEmpty", 0.05f);
	if ((bool)flamethrowerEffectInstance)
	{
		EntityState.Destroy(flamethrowerEffectInstance.gameObject);
	}
        }
	

        //FixedUpdate() runs almost every frame of the skill
    public override void FixedUpdate()
	{
	base.FixedUpdate();
	stopwatch += Time.fixedDeltaTime;
	

	if (stopwatch >= entryDuration && stopwatch < entryDuration + flamethrowerDuration && !hasBegunFlamethrower)
	{
		hasBegunFlamethrower = true;
		Util.PlaySound(startAttackSoundString, base.gameObject);
		if ((bool)childLocator)
		{
			muzzleTransform = childLocator.FindChild("MouthMuzzle");
			
			flamethrowerEffectInstance = Object.Instantiate(flamethrowerEffectPrefab, muzzleTransform).transform;
			flamethrowerEffectInstance.transform.localPosition = Vector3.zero;
			flamethrowerEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = flamethrowerDuration;
		// 	EffectManager.SpawnEffect(flamethrowerEffectPrefab, new EffectData
		// {
		// 	origin = flamethrowerEffectInstance.transform.localPosition,
		// 	scale = radius
		// }, transmit: true);
		}

		
	}
	if (stopwatch >= entryDuration + flamethrowerDuration && hasBegunFlamethrower)
	{
		hasBegunFlamethrower = false;
		PlayCrossfade("Gesture, Override", "ExitFlamebreath", "ExitFlamebreath.playbackRate", exitDuration, 0.1f);
	}
	if (hasBegunFlamethrower)
	{
		flamethrowerStopwatch += Time.deltaTime;
		if (flamethrowerStopwatch > 1f / tickFrequency)
		{
			flamethrowerStopwatch -= 1f / tickFrequency;
			FireFlame("MouthMuzzle");
		}

	
	}
	else if ((bool)flamethrowerEffectInstance)
	{
		EntityState.Destroy(flamethrowerEffectInstance.gameObject);
	}
	if (stopwatch >= flamethrowerDuration + entryDuration + exitDuration && base.isAuthority)
	{
		outer.SetNextStateToMain();
	}
	}
		public override InterruptPriority GetMinimumInterruptPriority()
	{
		return InterruptPriority.Skill;
	}
	}
}

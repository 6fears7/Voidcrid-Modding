// // EntityStates.Croco.Slash
// using EntityStates;
// using RoR2;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.Networking;
// using EntityStates.BeetleGuardMonster;
// using System;


// namespace Voidcrid
// {
// public class VoidSlash : GroundSlam
// {

// 	public new static float baseDuration = 3.5f;

// 	public new static float damageCoefficient = 4f;


// 	// private Animator modelAnimator;

// 	// private Transform modelTransform;


// 	// private float duration;


// 	// private new  void EnableIndicator(Transform indicator)
// 	// {
// 	// 	if ((bool)indicator)
// 	// 	{
// 	// 		indicator.gameObject.SetActive(value: true);
// 	// 		ObjectScaleCurve component = indicator.gameObject.GetComponent<ObjectScaleCurve>();
// 	// 		if ((bool)component)
// 	// 		{
// 	// 			component.time = 0f;
// 	// 		}
// 	// 	}
// 	// }

// 	// private new void DisableIndicator(Transform indicator)
// 	// {
// 	// 	if ((bool)indicator)
// 	// 	{
// 	// 		indicator.gameObject.SetActive(value: false);
// 	// 	}
// 	// }

// 	public override void OnEnter()
// 	{
// 		base.OnEnter();
// 		modelAnimator = GetModelAnimator();
// 		modelTransform = GetModelTransform();
// 		Util.PlaySound(initialAttackSoundString, base.gameObject);
// 		base.characterDirection.forward = GetAimRay().direction;
// 		attack = new OverlapAttack();
// 		attack.attacker = base.gameObject;
// 		attack.inflictor = base.gameObject;
// 		attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
// 		attack.damage = damageCoefficient * damageStat;
// 		attack.hitEffectPrefab = hitEffectPrefab;
// 		attack.forceVector = Vector3.up * forceMagnitude;
// 		if ((bool)modelTransform)
// 		{
// 			attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "GroundSlam");
// 		}
// 		duration = baseDuration / attackSpeedStat;
// 		PlayAnimation("Body", "SleepLoop");		
		
// 		if (!modelTransform)
// 		{
// 			return;
// 		}
// 		modelChildLocator = modelTransform.GetComponent<ChildLocator>();
// 		if ((bool)modelChildLocator)
// 		{
// 			GameObject original = chargeEffectPrefab;
// 			Transform transform = modelChildLocator.FindChild("MuzzleHandL");
// 			Transform transform2 = modelChildLocator.FindChild("MuzzleHandR");
// 			if ((bool)transform)
// 			{
// 				leftHandChargeEffect = UnityEngine.Object.Instantiate(original, transform);
// 			}
// 			if ((bool)transform2)
// 			{
// 				rightHandChargeEffect = UnityEngine.Object.Instantiate(original, transform2);
// 			}
// 			// groundSlamIndicatorInstance = modelChildLocator.FindChild("GroundSlamIndicator");
// 			// EnableIndicator(groundSlamIndicatorInstance);
// 		}
// 	}

// 	public override void OnExit()
// 	{
// 		Debug.Log("Exiting");

// 		PlayCrossfade("Gesture, Override", "BufferEmpty", .2f);
// 		PlayCrossfade("Gesture, AdditiveHigh", "BufferEmpty", .2f);
// 		EntityState.Destroy(leftHandChargeEffect);
// 		EntityState.Destroy(rightHandChargeEffect);
// 		// DisableIndicator(groundSlamIndicatorInstance);
// 		// _ = (bool)base.characterDirection;
// 		base.OnExit();
// 	}

// 	public override void FixedUpdate()
// 	{
// 		base.FixedUpdate();
// 		// if ((bool)modelAnimator && modelAnimator.GetFloat("GroundSlam.hitBoxActive") > 0.5f)
// 		// {
// 			if (NetworkServer.active)
// 			{
// 				attack.Fire();
// 				Debug.Log("Firing");
// 			}
// 			if (base.isAuthority && (bool)modelTransform)
// 			{
// 				// DisableIndicator(groundSlamIndicatorInstance);
// 				EffectManager.SimpleMuzzleFlash(slamEffectPrefab, base.gameObject, "SlamZone", transmit: true);
// 			}
// 			EntityState.Destroy(leftHandChargeEffect);
// 			EntityState.Destroy(rightHandChargeEffect);
// 		if (base.fixedAge >= duration && base.isAuthority)
// 		{
// 			outer.SetNextStateToMain();
// 		}
// 	}

// 	public override InterruptPriority GetMinimumInterruptPriority()
// 	{
// 		return InterruptPriority.PrioritySkill;
// 	}
// }
// }



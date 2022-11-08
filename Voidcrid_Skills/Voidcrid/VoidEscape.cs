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

		public CharacterBody body;

    [SerializeField]
	
    private new GameObject smokeBombEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathBombExplosion.prefab").WaitForCompletion();
    private new string smokeBombMuzzleString = "MuzzleCenter";
	[SerializeField]
    private GameObject explosionPrefab =  Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathBombExplosion.prefab").WaitForCompletion();
	
	private TemporaryVisualEffect voidFogEffect;

	public GameObject voidFogInstance = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VoidFogMildEffect.prefab").WaitForCompletion();
	public float voidJailChance = 5f;
	public override void OnEnter()
	{
		base.OnEnter();
       	// voidFogInstance = Object.Instantiate(voidFogInstance, FindModelChild("MouthMuzzle"));

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

		try {
        UpdateSingleTemporaryVisualEffect(ref voidFogEffect, voidFogInstance, body.radius * 0.5f, 2, "MouthMuzzle");
		}

		catch {

			Console.print("Error loading Fog effect");
		}
        
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
		return InterruptPriority.PrioritySkill;
	}


	private void UpdateSingleTemporaryVisualEffect(ref TemporaryVisualEffect tempEffect, GameObject obj, float effectRadius, int count, string childLocatorOverride = "")
	{
		bool flag = tempEffect != null;
		if (flag == count > 0)
		{
			return;
		}
		if (count > 0)
		{
			if (flag)
			{
				return;
			}
			GameObject gameObject = Object.Instantiate(obj, body.corePosition, Quaternion.identity);
			tempEffect = gameObject.GetComponent<TemporaryVisualEffect>();
			tempEffect.parentTransform = body.coreTransform;
			tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
			tempEffect.healthComponent = body.healthComponent;
			tempEffect.radius = effectRadius;
			LocalCameraEffect component = gameObject.GetComponent<LocalCameraEffect>();
			if ((bool)component)
			{
				component.targetCharacter = base.gameObject;
			}
			if (string.IsNullOrEmpty(childLocatorOverride))
			{
				return;
			}
			ModelLocator modelLocator = body.modelLocator;
			ChildLocator childLocator;
			if (modelLocator == null)
			{
				childLocator = null;
			}
			else
			{
				Transform modelTransform = modelLocator.modelTransform;
				childLocator = ((modelTransform != null) ? modelTransform.GetComponent<ChildLocator>() : null);
			}
			ChildLocator childLocator2 = childLocator;
			if ((bool)childLocator2)
			{
				Transform transform = childLocator2.FindChild(childLocatorOverride);
				if ((bool)transform)
				{
					tempEffect.parentTransform = transform;
				}
			}
		}
		else if ((bool)tempEffect)
		{
			tempEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
		}
	}
}

}
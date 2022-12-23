using EntityStates;
using EntityStates.Croco;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using System;
using UnityEngine.AddressableAssets;

using EntityStates.VoidSurvivor;
using UnityEngine.Networking;

namespace Voidcrid {
public class VoidBleed : BaseSkillState
{    

        public  static float baseDuration = 2f;
        public  static float damageCoefficient = 5f;

        // private float voidJailChance = .05f;
        public  static float procCoefficient = 1f;
        public static float attackRecoil = 1.5f;
        public static float hitHopVelocity = 5.5f;
        public static string mecanimRotateParameter = "baseRotate";
        public int currentAttack;
        private float earlyExitTime = 0.90f;

        private  float duration;

        private float switchAttacks = 50f;
        
        private float blastAttackRadius = VoidcridDef.EntropyOverrideRadius.Value;
        private float earlyExitDuration;
        private ChildLocator childLocator;
        private bool hasFired1;
        private bool hasFired2;
        private bool hasFinishedFiring;
        [SerializeField]
        private float blastAttackForce = 1000f;

		// private GameObject groundImpact = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/LaserImpactEffect.prefab").WaitForCompletion();

        private  float hitPauseTimer;
        [SerializeField]

        private GameObject aoePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion();	

        private float blastAttackProcCoefficient = 1;
        private bool inHitPause;
        private DamageType voidAttack;
        private DamageType poisonAttack;

       

        private CrocoDamageTypeController crocoDamageTypeController;

	private GameObject leftFistEffectInstance;
 
	private GameObject rightFistEffectInstance;

    // private float entropyFiringSpeed = .3f;

    private DamageType entropyDamage;

    // private float baseEntropyDamage = 3f;

    [SerializeField]
    public GameObject leftfistEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDeathBombExplosion.prefab").WaitForCompletion();
    public GameObject rightfistEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoFistEffect.prefab").WaitForCompletion();
    
    private float stopwatch;
        private  Animator animator;
        private Transform modelBaseTransform;

        private BlastAttack obj;

        private  void FireSmash()
	{
        

            
			obj = new BlastAttack

            
			{
               
				radius = blastAttackRadius,
				procCoefficient = blastAttackProcCoefficient,
				position = base.transform.position,
				attacker = base.gameObject,
				crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
				baseDamage = base.damageStat * VoidcridDef.EntropyOverrideDamage.Value
,				falloffModel = BlastAttack.FalloffModel.None,
				damageType =  entropyDamage,
                
				baseForce = blastAttackForce


               

			};
            obj.bonusForce = Vector3.back * blastAttackForce;
			obj.teamIndex = TeamComponent.GetObjectTeam(obj.attacker);
			obj.attackerFiltering = AttackerFiltering.NeverHitSelf;
			

			obj.Fire();
        // }

        if ((bool)leftFistEffectInstance && rightFistEffectInstance)
		{
		Vector3 footPosition = base.characterBody.footPosition;

		EffectManager.SpawnEffect(aoePrefab, new EffectData
		{
			origin = footPosition,
			scale = blastAttackRadius
		}, transmit: true);

        }
    }
        public override void OnEnter()
        {
            base.OnEnter();
            
            crocoDamageTypeController = GetComponent<CrocoDamageTypeController>();

            voidAttack = (Util.CheckRoll(Voidcrid.VoidcridDef.EntropyOverrideJailChance.Value, base.characterBody.master) ? DamageType.VoidDeath : DamageType.Generic);
                if (NetworkServer.active){
            base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
                }
            if (VoidcridDef.Seasonal.Value == true) {

            poisonAttack = DamageType.Freeze2s;
            aoePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteIce/AffixWhiteExplosion.prefab").WaitForCompletion();

            } else if(VoidcridDef.Seasonal.Value == false){
            poisonAttack = crocoDamageTypeController.GetDamageType();
            aoePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion();	

            }
            entropyDamage = (Util.CheckRoll(switchAttacks, base.characterBody.master) ? voidAttack : poisonAttack);

            leftFistEffectInstance = UnityEngine.Object.Instantiate(leftfistEffectPrefab, FindModelChild("MuzzleHandL"));
		    rightFistEffectInstance = UnityEngine.Object.Instantiate(rightfistEffectPrefab, FindModelChild("MuzzleHandR"));

            this.duration = baseDuration / this.attackSpeedStat;
            this.earlyExitDuration = this.duration * earlyExitTime;
            this.hasFired1 = false;
            this.hasFired2 = false;
            this.hasFinishedFiring = false;
            this.moveSpeedStat = 0f;
            this.childLocator = base.GetModelChildLocator();
            this.modelBaseTransform = base.GetModelBaseTransform();
            this.animator = base.GetModelAnimator();

            bool grounded = base.characterMotor.isGrounded;
            bool moving = this.animator.GetBool("isMoving");

            string setAnimState = "Slash3";

            this.animator.SetBool("attacking", true);
            float num = Mathf.Max(duration, 0.2f);

            PlayCrossfade("Gesture, Additive", setAnimState, "Slash.playbackRate", num, 0.05f);
		    PlayCrossfade("Gesture, Override", setAnimState, "Slash.playbackRate", num,  0.05f);
            

            float dmg = VoidBleed.damageCoefficient;

            if (NetworkServer.active)
		{

          	ProcChainMask procChainMask = default(ProcChainMask);
			procChainMask.AddProc(ProcType.VoidSurvivorCrush);

            if (entropyDamage == poisonAttack)
			{
				base.healthComponent.HealFraction(.25f, procChainMask);
			}
			else
			{

				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = (.25f * base.healthComponent.fullCombinedHealth);
				damageInfo.position = base.characterBody.corePosition;
				damageInfo.force = Vector3.zero;
				damageInfo.damageColorIndex = DamageColorIndex.Void;
				damageInfo.crit = false;
				damageInfo.attacker = null;
				damageInfo.inflictor = null;
				damageInfo.damageType = DamageType.NonLethal;
				damageInfo.procCoefficient = 0f;
				damageInfo.procChainMask = procChainMask;
				base.healthComponent.TakeDamage(damageInfo);
                
        }
        }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.moveSpeedStat = 0f;

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
                // base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator); 
                this.inHitPause = false;
                // if (this.storedVelocity != Vector3.zero) base.characterMotor.velocity = this.storedVelocity;
            }

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("Slash3.playbackRate", 3f);
            }

            if (this.stopwatch >= this.duration * VoidcridDef.EntropyOverrideFireSpeed.Value && this.hasFired1 == false && this.hasFired2 == false && this.hasFinishedFiring == false)
            // && this.stopwatch <= this.duration * .6f
            {
                this.FireSmash();
                this.hasFired1 = true;
                
            }

             if (this.stopwatch >= this.duration * (VoidcridDef.EntropyOverrideFireSpeed.Value * 2) && this.hasFired1 == true && this.hasFired2 == false && this.hasFinishedFiring == false)

            {
                this.FireSmash();
                this.hasFired2 = true;
                
            }

            if (this.stopwatch >= this.duration * (VoidcridDef.EntropyOverrideFireSpeed.Value * 3) && this.hasFired1 == true && this.hasFired2 == true && this.hasFinishedFiring == false)

            {
                this.FireSmash();
                this.hasFinishedFiring = true;
                
            }




            // if (base.fixedAge >= this.earlyExitDuration && base.inputBank.skill1.down)
            // {
            //     var nextAttack = new VoidBleed();
            //     nextAttack.currentAttack = this.currentAttack + 1;
            //     this.outer.SetNextState(nextAttack);
            //     return;
            // }

            if (base.isAuthority && this.hasFinishedFiring == true)
            {
                // base.fixedAge >= this.duration && 
                this.outer.SetNextStateToMain();
                base.StartAimMode(0.2f, false);
                return;
            }


        }


        public override void OnExit()
        {
            this.moveSpeedStat = 0f;
            this.animator.SetBool("attacking", false);
                if (NetworkServer.active){
            base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            base.characterBody.RemoveBuff(RoR2Content.Buffs.AffixWhite);
                }
		    EntityState.Destroy(leftFistEffectInstance);
		    EntityState.Destroy(rightFistEffectInstance);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }


    }
    
}


//   RaycastHit[] hits = Physics.SphereCastAll(decayMuzzle.position, decayEffect.radius, Vector3.up);
//         foreach (RaycastHit hit in hits)
//         {
//             HealthComponent healthComponent = hit.collider.GetComponent<HealthComponent>();
//             if (healthComponent)
//             {
//                 healthComponent.TakeDamage(new DamageInfo
//                 {
//                     damage = damagePerSecond * Time.fixedDeltaTime,
//                     position = hit.point,
//                     damageType = DamageType.Generic,
//                     attacker = base.gameObject,
//                     inflictor = base.gameObject,
//                     force = Vector3.zero
//                 });
//             }
//         }
//     }
// }

// public override void FixedUpdate()
// {
//     base.FixedUpdate();

//     // Update the decay effect
//     decayEffect.Update();

//     // Calculate the remaining number of unstable atoms using the nuclear decay formula
//     float remainingUnstableAtoms = base.characterBody.initialUnstableAtoms * Mathf.Exp(-decayRate * Time.fixedDeltaTime);
//     base.characterBody.SetUnstableAtoms(remainingUnstableAtoms);

//     // End the decay effect when the duration has elapsed
//     if (base.fixedAge > decayDuration)
//     {
//         outer.SetNextStateToMain();
//     }
// }

// public override void OnExit()
// {
//     // Destroy the decay effect
//     decayEffect.Destroy();

//     base.OnExit();
// }
// }
// }





using EntityStates;
using EntityStates.Croco;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

using EntityStates.VoidSurvivor;
using UnityEngine.Networking;

namespace Voidcrid {
public class VoidBleed : BaseSkillState
{    

        public  static float baseDuration = 2f;
        public  static float damageCoefficient = 5f;

        private float voidJailChance = .05f;

        private float poisonChance = 50f;
        public  static float procCoefficient = 1f;
        public static float attackRecoil = 1.5f;
        public static float hitHopVelocity = 5.5f;
        public static string mecanimRotateParameter = "baseRotate";
        public int currentAttack;
        private float earlyExitTime = 0.90f;

        private  float duration;

        private float switchAttacks = 50f;
        
        private float blastAttackRadius = 10f;
        private float earlyExitDuration;
        private ChildLocator childLocator;
        private bool hasFired;

        [SerializeField]
        private float blastAttackForce = 20f;

		// private GameObject biteEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBiteEffect.prefab").WaitForCompletion();	
        private  float hitPauseTimer;
        [SerializeField]

        private GameObject aoePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion();	

        private float blastAttackProcCoefficient = 1;
        private bool inHitPause;

        private DamageType voidAttack;
        private DamageType poisonAttack;

	private GameObject leftFistEffectInstance;
 
	private GameObject rightFistEffectInstance;

    [SerializeField]
    public GameObject leftfistEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDeathBombExplosion.prefab").WaitForCompletion();
    public GameObject rightfistEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoFistEffect.prefab").WaitForCompletion();
        private float stopwatch;
        private  Animator animator;
        private Transform modelBaseTransform;

        

        private  void FireSmash()
	{
            
			BlastAttack obj = new BlastAttack
			{
               

				radius = blastAttackRadius,
				procCoefficient = blastAttackProcCoefficient,
				position = base.transform.position,
				attacker = base.gameObject,
				crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
				baseDamage = base.damageStat * .1f,
				falloffModel = BlastAttack.FalloffModel.None,
				damageType =  (Util.CheckRoll(switchAttacks, base.characterBody.master) ? voidAttack : poisonAttack),

				baseForce = blastAttackForce


               

			};
			obj.teamIndex = TeamComponent.GetObjectTeam(obj.attacker);
			obj.attackerFiltering = AttackerFiltering.NeverHitSelf;
			

			obj.Fire();
        // }

        if ((bool)leftfistEffectPrefab && rightfistEffectPrefab)
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
            base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            voidAttack = (Util.CheckRoll(voidJailChance, base.characterBody.master) ? DamageType.VoidDeath : DamageType.Generic);
            poisonAttack = DamageType.PoisonOnHit;

            leftFistEffectInstance = Object.Instantiate(leftfistEffectPrefab, FindModelChild("MuzzleHandL"));
		    rightFistEffectInstance = Object.Instantiate(rightfistEffectPrefab, FindModelChild("MuzzleHandR"));

            this.duration = baseDuration / this.attackSpeedStat;
            this.earlyExitDuration = this.duration * earlyExitTime;
            this.hasFired = false;
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

          	ProcChainMask procChainMask = default(ProcChainMask);
			procChainMask.AddProc(ProcType.VoidSurvivorCrush);

            if (poisonAttack ==  DamageType.PoisonOnHit)
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

            if (this.stopwatch >= this.duration * 0.45f && this.stopwatch <= this.duration * .9f)
            {
                this.FireSmash();
            }


            if (base.fixedAge >= this.earlyExitDuration && base.inputBank.skill1.down)
            {
                var nextAttack = new VoidBleed();
                nextAttack.currentAttack = this.currentAttack + 1;
                this.outer.SetNextState(nextAttack);
                return;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                base.StartAimMode(0.2f, false);
                return;
            }


        }


        public override void OnExit()
        {
            if (!this.hasFired) this.FireSmash();
            this.moveSpeedStat = 0f;
            this.animator.SetBool("attacking", false);
            base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
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

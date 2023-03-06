using EntityStates;
using EntityStates.Croco;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;


namespace Voidcrid.Skills
{
    public class VoidScepter : BaseSkillState
    {

        public static float baseDuration = 2f;
        public static float damageCoefficient = 5f;
        public int maxBounces;
        public float bounceRange = 3f;
        public static float procCoefficient = 1f;
        public static float attackRecoil = 1.5f;
        public static float hitHopVelocity = 5.5f;
        public static string mecanimRotateParameter = "baseRotate";
        public int currentAttack;
        private float earlyExitTime = 0.90f;

        private float duration;

        private float switchAttacks = 50f;

        private float blastAttackRadius = Voidcrid.VoidcridDef.ScepterEntropyOverrideRadius.Value;
        private float earlyExitDuration;
        private ChildLocator childLocator;
        private bool hasFired1;
        private bool hasFired2;
        private bool hasFinishedFiring;
        private bool firedBombardment;
        [SerializeField]
        private float blastAttackForce = 1000f;

        private GameObject groundImpact = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/VoidRaidCrabGravityBumpExplosionGround.prefab").WaitForCompletion();

        private float hitPauseTimer;
        [SerializeField]

        private GameObject aoePrefab;

        private GameObject aoe2 = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/VoidRaidCrabTripleBeamExplosion.prefab").WaitForCompletion();

        private float blastAttackProcCoefficient = 1;
        private bool inHitPause;

        private DamageType voidAttack;
        private DamageType poisonAttack;



        private CrocoDamageTypeController crocoDamageTypeController;

        private GameObject leftFistEffectInstance;

        private GameObject rightFistEffectInstance;
        private Material entropyGlow;

        float emissionIntensity = 4f;
        float minIntensity = 0.3f;


        private DamageType entropyDamage;


        [SerializeField]
        public GameObject leftfistEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosionCorrupted.prefab").WaitForCompletion();
        public GameObject rightfistEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosionCorrupted.prefab").WaitForCompletion();
        private GameObject projectilePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ElementalRingVoidBlackHole");

        private float stopwatch;
        private Animator animator;
        private Transform modelBaseTransform;

        private BlastAttack obj;

        private bool doVoidGlow = false;
        private void FireSmash()
        {



            obj = new BlastAttack


            {

                radius = blastAttackRadius,
                procCoefficient = blastAttackProcCoefficient,
                position = base.transform.position,
                attacker = base.gameObject,
                crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                baseDamage = base.damageStat * Voidcrid.VoidcridDef.ScepterEntropyOverrideDamage.Value
,
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = entropyDamage,

                baseForce = blastAttackForce



            };
            obj.bonusForce = Vector3.back * blastAttackForce;
            obj.teamIndex = TeamComponent.GetObjectTeam(obj.attacker);
            obj.attackerFiltering = AttackerFiltering.NeverHitSelf;

            if (entropyDamage == DamageType.Nullify)
            {
                R2API.DamageAPI.AddModdedDamageType(obj, Voidcrid.Modules.DamageTypes.entropyJail);
            }

            obj.Fire();

            if ((bool)leftFistEffectInstance && rightFistEffectInstance)
            {
                Vector3 footPosition = base.characterBody.footPosition;


                EffectManager.SpawnEffect(aoePrefab, new EffectData
                {
                    origin = footPosition,
                    scale = blastAttackRadius
                }, transmit: true);

                EffectManager.SpawnEffect(aoe2, new EffectData
                {
                    origin = footPosition,
                    scale = blastAttackRadius
                }, transmit: true);

            }
        }
        public override void OnEnter()
        {
            base.OnEnter();

            int spineIndexCheck = GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos.Length;
            
            if (spineIndexCheck > 1) 
        {
            doVoidGlow = true;
            entropyGlow = GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos[1].defaultMaterial;

        }
            crocoDamageTypeController = GetComponent<CrocoDamageTypeController>();

            voidAttack = (Util.CheckRoll(Voidcrid.VoidcridDef.EntropyOverrideJailChance.Value, base.characterBody.master) ? DamageType.Nullify : DamageType.Generic);

            poisonAttack = crocoDamageTypeController.GetDamageType();
            aoePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosionCorrupted.prefab").WaitForCompletion();
            entropyDamage = (Util.CheckRoll(switchAttacks, base.characterBody.master) ? voidAttack : poisonAttack);

            leftFistEffectInstance = UnityEngine.Object.Instantiate(leftfistEffectPrefab, FindModelChild("MuzzleHandL"));
            rightFistEffectInstance = UnityEngine.Object.Instantiate(rightfistEffectPrefab, FindModelChild("MuzzleHandR"));

            this.duration = baseDuration / this.attackSpeedStat;
            this.earlyExitDuration = this.duration * earlyExitTime;
            this.hasFired1 = false;
            this.hasFired2 = false;
            this.hasFinishedFiring = false;
            this.firedBombardment = false;
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
            PlayCrossfade("Gesture, Override", setAnimState, "Slash.playbackRate", num, 0.05f);


            float dmg = Entropy.damageCoefficient;
            if (NetworkServer.active)
            {

                ProcChainMask procChainMask = default(ProcChainMask);
                procChainMask.AddProc(ProcType.VoidSurvivorCrush);

                if (entropyDamage == poisonAttack)
                {
                    base.healthComponent.HealFraction(Voidcrid.VoidcridDef.EntropySelfHeal.Value, procChainMask);
                }
                else
                {

                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.damage = (Voidcrid.VoidcridDef.EntropySelfDamage.Value * base.healthComponent.fullCombinedHealth);
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
            ManageEntroypGlow();
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



            if (this.stopwatch >= this.duration * Voidcrid.VoidcridDef.ScepterEntropyOverrideFireSpeed.Value && this.hasFired1 == false && this.hasFired2 == false && this.hasFinishedFiring == false)
            // && this.stopwatch <= this.duration * .6f
            {
                if (base.isAuthority)
                {
                    this.FireSmash();
                    this.hasFired1 = true;
                }
                ManageEntroypGlow();
            }

            if (this.stopwatch >= this.duration * (Voidcrid.VoidcridDef.EntropyOverrideFireSpeed.Value * 2) && this.hasFired1 == true && this.hasFired2 == false && this.hasFinishedFiring == false)

            {
                if (base.isAuthority)
                {
                    this.FireSmash();
                    this.hasFired2 = true;
                }
                ManageEntroypGlow();
            }

            if (this.stopwatch >= this.duration * (Voidcrid.VoidcridDef.EntropyOverrideFireSpeed.Value * 3) && this.hasFired1 == true && this.hasFired2 == true && this.hasFinishedFiring == false)

            {
                if (base.isAuthority)
                {
                    this.FireSmash();
                    this.hasFinishedFiring = true;
                }
                ManageEntroypGlow();

            }

            if (IsKeyDownAuthority() && hasFinishedFiring && base.isAuthority)
            {

                Bombardment();
                this.firedBombardment = true;
            }


            if (base.isAuthority && (this.hasFinishedFiring == true || this.firedBombardment))
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
            EntityState.Destroy(leftFistEffectInstance);
            EntityState.Destroy(rightFistEffectInstance);
            base.OnExit();
        }

        private void ManageEntroypGlow()
        {

           if (doVoidGlow == true) {

            float emissionIncrement = minIntensity / (Voidcrid.VoidcridDef.ScepterEntropyOverrideFireSpeed.Value);

            if (entropyGlow)
            {
                entropyGlow.EnableKeyword("_EMISSION");
                entropyGlow.SetColor("_EmColor", Voidcrid.VoidcridDef.ScepterGlow.Value * emissionIntensity);
                emissionIntensity -= emissionIncrement;
            }



            if (hasFinishedFiring)
            {
                entropyGlow.DisableKeyword("_EMISSION");
                entropyGlow.SetColor("_EmColor", Color.black);

            }
        }

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void Bombardment()
        {
            float damage = 1f;

            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
            {
                damage = damage,
                damageTypeOverride = crocoDamageTypeController.GetDamageType(),
                crit = RollCrit(),
                damageColorIndex = DamageColorIndex.Void,
                position = base.characterBody.footPosition,
                procChainMask = default(ProcChainMask),
                force = 6000f,
                owner = base.gameObject,
                projectilePrefab = SkillSetup.voidFrogProjectile,
                rotation = Quaternion.identity,
                target = base.gameObject
            });
        }



    }



}

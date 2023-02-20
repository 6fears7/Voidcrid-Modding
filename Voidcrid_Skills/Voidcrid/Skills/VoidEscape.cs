using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Networking;
using EntityStates.Bandit2;
using UnityEngine.AddressableAssets;
using EntityStates.VoidRaidCrab.Weapon;



namespace Voidcrid
{
    public class VoidEscape : StealthMode
    {

        private TemporaryVisualEffect voidFog;

        // private float duration = StealthMode.duration;
        //  Addressables.LoadAssetAsync<GameObject>("Prefabs/TemporaryVisualEffects/voidFogMildEffect").WaitForCompletion();
        public static DamageReport onCharacterDeathGlobal;

        // private float blastAttackRadius = StealthMode.blastAttackRadius;

        // private float blastAttackForce = StealthMode.blastAttackForce;

        // private float shortHopVelocity = StealthMode.shortHopVelocity;

        // private float blastAttackProcCoefficient = StealthMode.blastAttackProcCoefficient;

        public CharacterBody body;

        [SerializeField]

        private new GameObject smokeBombEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathPreExplosion.prefab").WaitForCompletion();
        private new string smokeBombMuzzleString = "MuzzleCenter";
        [SerializeField]
        private GameObject explosionPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegaCrabDeathBombExplosion.prefab").WaitForCompletion();

        private FireGravityBump FGBSound = new FireGravityBump();

        public GameObject voidFogInstance = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VoidFogMildEffect.prefab").WaitForCompletion();
        public float voidJailChance = 3f;

        private CrocoDamageTypeController crocoDamageTypeController;

        private DamageType baseAttack;

        public override void OnEnter()
        {
            base.OnEnter();
            bool hasDeeprot = VoidcridDef.HasDeeprot(base.skillLocator);

            crocoDamageTypeController = GetComponent<CrocoDamageTypeController>();


            // voidFogInstance = Object.Instantiate(voidFogInstance, FindModelChild("MouthMuzzle"));
            if (hasDeeprot == true)
            {

                baseAttack = crocoDamageTypeController.GetDamageType();

            }
            else
            {

                baseAttack = DamageType.Stun1s;

            }
            // animator = GetModelAnimator();
            // _ = (bool)animator;

            if (base.characterBody)
            {
                if (NetworkServer.active)
                {   
                base.characterBody.AddBuff(RoR2Content.Buffs.Cloak);
                base.characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
                base.characterBody.AddBuff(RoR2Content.Buffs.VoidFogStrong);
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);

                }


                base.characterBody.onSkillActivatedAuthority += OnSkillActivatedAuthority;
            }

            if (base.isAuthority) {
            FireSmokebomb();
            }

            Util.PlaySound(FGBSound.enterSoundString, base.gameObject);

            // Hack to pull the visual state without actually updating the effect
            base.characterBody.UpdateSingleTemporaryVisualEffect(ref voidFog, "Prefabs/TemporaryVisualEffects/voidFogMildEffect", characterBody.radius, true);

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
            if (!outer.destroying && base.isAuthority)

            {
                FireSmokebomb();


            }


            Util.PlaySound(FGBSound.enterSoundString, base.gameObject);

            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
                characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                characterBody.RemoveBuff(RoR2Content.Buffs.VoidFogStrong);

                 }

                base.characterBody.onSkillActivatedAuthority -= OnSkillActivatedAuthority;
            }

            // if ((bool)animator)
            // {
            //     animator.SetLayerWeight(animator.GetLayerIndex("Body, StealthWeapon"), 0f);
            // }
            base.characterBody.UpdateSingleTemporaryVisualEffect(ref voidFog, "Prefabs/TemporaryVisualEffects/voidFogMildEffect", characterBody.radius, false);
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
                    damageType = (Util.CheckRoll(VoidcridDef.EtherealDriftOverrideJailChance.Value, base.characterBody.master) ? DamageType.VoidDeath : baseAttack),
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
                base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, shortHopVelocity * 1.7f, base.characterMotor.velocity.z);
            }


        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }



    }

}
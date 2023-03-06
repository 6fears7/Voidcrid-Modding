using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static R2API.DamageAPI;

namespace Voidcrid.Skills
{


    public class NullBeam : BaseSkillState
    {


        private string muzzle = "MouthMuzzle";

        private float baseDuration = 1f;


        private float maxDistance = 100f;

        private float minDistance = 1f;


        private float recoilAmplitude = 1f;

        private float bulletRadius = 1f;


        private GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamImpactCorrupt.prefab").WaitForCompletion();

        private GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerCaptureTracer.prefab").WaitForCompletion();
        private float spreadBloomValue = 0.3f;


        private float forcePerSecond = 2f;

        public ItemIndex index;

        public static float nullBeamDamage = Voidcrid.VoidcridDef.NullBeamOverrideDamage.Value;

        private float minimumDuration;

        private BuffDef nullifier = RoR2Content.Buffs.Nullified;
        private float maxSpread = 2f;

        private DamageType deeprotDamage;
        private DamageType passiveAttack;
        private float rotAttack = 8f;

        private CrocoDamageTypeController crocoDamageTypeController;

        private float switchAttacks = 50f;

        public static DamageType voidcridLaserAttack;


        private Material voidGlow;

        private bool doVoidGlow = false;

        private float maxIntensity = 4.0f;

        private float duration = Voidcrid.VoidcridDef.NullBeamOverrideDuration.Value;

        public override void OnEnter()
        {

            base.OnEnter();

            int spineIndexCheck = GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos.Length;


            crocoDamageTypeController = GetComponent<CrocoDamageTypeController>();

            minimumDuration = baseDuration / attackSpeedStat;

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
                base.characterBody.AddBuff(RoR2Content.Buffs.SmallArmorBoost);
            }
            PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", duration);     // Util.PlaySound(FireGravityBump.enterSoundString, base.gameObject);
            
            if (spineIndexCheck > 1) 
        {
            doVoidGlow = true;
            voidGlow = GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos[1].defaultMaterial;

        }


        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            FireBullet();
            float charge = CalcCharge();
            Ray aimRay = GetAimRay();
            base.characterBody.SetAimTimer(3f);

            if (doVoidGlow == true) {
                voidGlow.EnableKeyword("_EMISSION");
                voidGlow.SetColor("_EmColor", Voidcrid.VoidcridDef.VoidGlow.Value * maxIntensity);
                maxIntensity -= maxIntensity * Time.fixedDeltaTime / (Voidcrid.VoidcridDef.NullBeamOverrideDuration.Value - 1f);
                }
            Vector3 point = GetAimRay().GetPoint(maxDistance + attackSpeedStat);
            if (Util.CharacterRaycast(base.gameObject, GetAimRay(), out var hitInfo, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
            {
                point = hitInfo.point;
            }

            if (base.isAuthority && (fixedAge >= Voidcrid.VoidcridDef.NullBeamOverrideDuration.Value || (fixedAge >= baseDuration && !IsKeyDownAuthority())))
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
                base.characterBody.RemoveBuff(RoR2Content.Buffs.SmallArmorBoost);
            }
 
            if (doVoidGlow == true) {
                voidGlow.DisableKeyword("_EMISSION");
                voidGlow.SetColor("_EmColor", Color.black);
            }


        }

        protected float CalcCharge()
        {
            return Mathf.Clamp01(fixedAge / baseDuration);
        }



        private void FireBullet()
        {
            Ray aimRay = GetAimRay();
            AddRecoil(-1f * recoilAmplitude, -2f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
            bool hasDeeprot = VoidcridDef.HasDeeprot(base.skillLocator);


            if (hasDeeprot)
            {
                passiveAttack = (Util.CheckRoll(rotAttack, base.characterBody.master) ? crocoDamageTypeController.GetDamageType() : DamageType.Generic);

            }
            else
            {

                passiveAttack = DamageType.Generic;
            }



            voidcridLaserAttack = (Util.CheckRoll(Voidcrid.VoidcridDef.NullBeamOverrideJailChance.Value, base.characterBody.master) ? DamageType.Nullify : DamageType.Generic);

            deeprotDamage = (Util.CheckRoll(switchAttacks, base.characterBody.master) ? passiveAttack : DamageType.Generic);

            if (base.isAuthority)
            {
                BulletAttack bulletAttack = new BulletAttack();
                bulletAttack.owner = base.gameObject;
                bulletAttack.weapon = base.gameObject;
                bulletAttack.origin = aimRay.origin;
                bulletAttack.aimVector = aimRay.direction;
                bulletAttack.muzzleName = muzzle;
                bulletAttack.maxDistance = Mathf.Lerp(minDistance, maxDistance, UnityEngine.Random.value);
                bulletAttack.minSpread = 1f;
                bulletAttack.maxSpread = maxSpread;
                bulletAttack.radius = bulletRadius;
                bulletAttack.smartCollision = false;
                bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                bulletAttack.hitMask = LayerIndex.entityPrecise.mask;
                bulletAttack.damage = nullBeamDamage * damageStat;
                bulletAttack.procCoefficient = Voidcrid.VoidcridDef.NullBeamOverrideProc.Value;
                bulletAttack.force = forcePerSecond;
                bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
                bulletAttack.hitEffectPrefab = hitEffectPrefab;
                bulletAttack.tracerEffectPrefab = tracerEffectPrefab;
                bulletAttack.damageType = (Util.CheckRoll(switchAttacks, base.characterBody.master) ? deeprotDamage : voidcridLaserAttack);
                if (voidcridLaserAttack == DamageType.Nullify)
                {
                    R2API.DamageAPI.AddModdedDamageType(bulletAttack, Voidcrid.Modules.DamageTypes.nullBeamJail);
                }
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

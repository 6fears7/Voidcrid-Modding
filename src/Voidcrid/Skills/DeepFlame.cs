using EntityStates;
using EntityStates.LemurianBruiserMonster;
using RoR2;
using UnityEngine;


namespace Voidcrid.Skills
{
    // Standalone port of the Lemurian's Flamebreath. Subclassing it did not work: every tunable on
    // Flamebreath is a *static* field, so the `new` instance fields here were never read by the
    // inherited code, and base.OnEnter/base.FixedUpdate kept running their own copy of the
    // flamethrower on the same shared stopwatch. Values that were never meant to be overridden are
    // still read off the vanilla statics so the flame looks and hits the same as before.
    public class Voidcridbreath : BaseSkillState
    {
        private const string muzzleName = "MouthMuzzle";

        private float maxDistance = 12f;

        private float baseFlamethrowerDuration = Voidcrid.VoidcridDef.FlamebreathOverrideDuration.Value;

        private float totalDamageCoefficient = Voidcrid.VoidcridDef.FlamebreathOverrideDamage.Value;

        private float tickDamageCoefficient;

        private float flamethrowerStopwatch;

        private float stopwatch;

        private float entryDuration;

        private float exitDuration;

        private float flamethrowerDuration;

        private bool hasBegunFlamethrower;

        private ChildLocator childLocator;

        private Transform flamethrowerEffectInstance;

        private Transform muzzleTransform;


        public override void OnEnter()
        {
            base.OnEnter();

            stopwatch = 0f;
            entryDuration = Flamebreath.baseEntryDuration;
            exitDuration = Flamebreath.baseExitDuration;
            flamethrowerDuration = baseFlamethrowerDuration + attackSpeedStat;

            Transform modelTransform = GetModelTransform();
            if ((bool)base.characterBody)
            {
                base.characterBody.SetAimTimer(entryDuration + flamethrowerDuration + 1f);
            }
            if ((bool)modelTransform)
            {
                childLocator = modelTransform.GetComponent<ChildLocator>();
            }

            float num = flamethrowerDuration * Flamebreath.tickFrequency;
            tickDamageCoefficient = totalDamageCoefficient / num;

            PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", flamethrowerDuration);
        }

        public override void OnExit()
        {
            Util.PlaySound(Flamebreath.endAttackSoundString, base.gameObject);
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.05f);
            if ((bool)flamethrowerEffectInstance)
            {
                EntityState.Destroy(flamethrowerEffectInstance.gameObject);
            }
            base.OnExit();
        }


        //FixedUpdate() runs almost every frame of the skill
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;


            if (stopwatch >= entryDuration && stopwatch < entryDuration + flamethrowerDuration && !hasBegunFlamethrower)
            {
                hasBegunFlamethrower = true;
                Util.PlaySound(Flamebreath.startAttackSoundString, base.gameObject);
                if ((bool)childLocator)
                {
                    muzzleTransform = childLocator.FindChild(muzzleName);

                    flamethrowerEffectInstance = Object.Instantiate(Flamebreath.flamethrowerEffectPrefab, muzzleTransform).transform;
                    flamethrowerEffectInstance.transform.localPosition = Vector3.zero;
                    ScaleParticleSystemDuration scaleParticleSystemDuration = flamethrowerEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if ((bool)scaleParticleSystemDuration)
                    {
                        scaleParticleSystemDuration.newDuration = flamethrowerDuration;
                    }
                }


            }
            if (stopwatch >= entryDuration + flamethrowerDuration && hasBegunFlamethrower)
            {
                hasBegunFlamethrower = false;
                PlayCrossfade("Gesture, Override", "ExitFlamebreath", "ExitFlamebreath.playbackRate", exitDuration, 0.1f);
            }
            if (hasBegunFlamethrower)
            {
                flamethrowerStopwatch += Time.fixedDeltaTime;
                if (flamethrowerStopwatch > 1f / Flamebreath.tickFrequency)
                {
                    flamethrowerStopwatch -= 1f / Flamebreath.tickFrequency;
                    FireFlame(muzzleName);
                }

                UpdateFlamethrowerEffect();
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

        private void FireFlame(string muzzleString)
        {
            if (!base.isAuthority)
            {
                return;
            }

            Ray aimRay = GetAimRay();

            BulletAttack bulletAttack = new BulletAttack();
            bulletAttack.owner = base.gameObject;
            bulletAttack.weapon = base.gameObject;
            bulletAttack.origin = aimRay.origin;
            bulletAttack.aimVector = aimRay.direction;
            bulletAttack.minSpread = 0f;
            bulletAttack.maxSpread = Flamebreath.maxSpread;
            bulletAttack.damage = tickDamageCoefficient * damageStat;
            bulletAttack.force = Flamebreath.force;
            bulletAttack.muzzleName = muzzleString;
            bulletAttack.hitEffectPrefab = Flamebreath.impactEffectPrefab;
            bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
            bulletAttack.radius = Flamebreath.radius;
            bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
            bulletAttack.stopperMask = LayerIndex.world.mask;
            bulletAttack.procCoefficient = Flamebreath.procCoefficientPerTick;
            bulletAttack.maxDistance = maxDistance;
            bulletAttack.tracerEffectPrefab = Flamebreath.tracerEffectPrefab;
            bulletAttack.smartCollision = true;
            bulletAttack.damageType = (Util.CheckRoll(Flamebreath.ignitePercentChance, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic);
            bulletAttack.Fire();
        }

        private void UpdateFlamethrowerEffect()
        {
            Ray aimRay = GetAimRay();
            Vector3 direction = aimRay.direction;
            if ((bool)muzzleTransform)
            {
                muzzleTransform.forward = direction;
            }

        }
    }
}

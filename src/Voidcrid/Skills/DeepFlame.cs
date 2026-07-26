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

        private float damagePerSecond = Voidcrid.VoidcridDef.FlamebreathOverrideDamage.Value;

        private float tickDamageCoefficient;

        private float tickProcCoefficient;

        private float tickRate;

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
            flamethrowerDuration = baseFlamethrowerDuration;

            Transform modelTransform = GetModelTransform();
            if ((bool)base.characterBody)
            {
                base.characterBody.SetAimTimer(entryDuration + flamethrowerDuration + 1f);
            }
            if ((bool)modelTransform)
            {
                childLocator = modelTransform.GetComponent<ChildLocator>();
            }

            // Attack speed buys ticks, not time. The breath is always the same length so it still
            // reads as a breath, and a faster Voidcrid packs more flame into that window. Damage is
            // a per-second budget scaled by attack speed and *then* divided by the rate we actually
            // land on -- the attackSpeedStat term is what makes this scale at all, since ticks per
            // cast is duration * tickRate and a rate-only budget would cancel straight back out.
            // Dividing by the real rate is what makes the cap below fatten ticks instead of
            // flatlining DPS. Proc rides the same structure but does not scale unless asked to.
            float baseTickRate = Voidcrid.VoidcridDef.FlamebreathOverrideTickFreq.Value;
            if (baseTickRate <= 0f)
            {
                baseTickRate = Flamebreath.tickFrequency;
            }
            // tickFrequency is a static that EntityStateCatalog fills in; a zero here would make
            // the drain loop in FixedUpdate spin forever, so refuse to trust it.
            if (baseTickRate <= 0f)
            {
                baseTickRate = 10f;
            }

            tickRate = Mathf.Clamp(baseTickRate * attackSpeedStat, 1f, 1f / Time.fixedDeltaTime);
            tickDamageCoefficient = damagePerSecond * attackSpeedStat / tickRate;

            float procPerSecond = baseTickRate * Flamebreath.procCoefficientPerTick;
            if (Voidcrid.VoidcridDef.FlamebreathOverrideProcScaling.Value)
            {
                procPerSecond *= attackSpeedStat;
            }
            tickProcCoefficient = procPerSecond / tickRate;

            // PlayAnimation stretches Acrid's own FireSpit clip (a single one-shot open/spit/close,
            // not a loop) to fill the given duration via playbackRate, then just holds on its closed
            // final pose once that duration elapses. Passing flamethrowerDuration alone left the
            // mouth animation finishing -- and holding shut -- a full entryDuration before the flame
            // itself actually stops (FixedUpdate doesn't start the stream until stopwatch reaches
            // entryDuration and keeps it going through entryDuration + flamethrowerDuration). Cover
            // that whole active window instead so the mouth stays open until the flame does.
            PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", entryDuration + flamethrowerDuration);
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

                    GameObject effectPrefab = SkillSetup.GetVoidFlameEffectPrefab();
                    if (!effectPrefab)
                    {
                        effectPrefab = Flamebreath.flamethrowerEffectPrefab;
                    }

                    flamethrowerEffectInstance = Object.Instantiate(effectPrefab, muzzleTransform).transform;
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
                // while, not if: past ~60 ticks/sec more than one tick comes due per physics frame,
                // and an if would silently cap the rate and let the stopwatch drift upward forever.
                float tickInterval = 1f / tickRate;
                while (flamethrowerStopwatch > tickInterval)
                {
                    flamethrowerStopwatch -= tickInterval;
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
            bulletAttack.procCoefficient = tickProcCoefficient;
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

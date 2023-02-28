using RoR2;
using Voidcrid.Modules;
using UnityEngine;
using R2API;
using UnityEngine.Networking;
using Voidcrid.Effects;

namespace Voidcrid.Hooks
{

    public class HookSetup
    {


        private static void JailJailJail(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {


            if (info.HasModdedDamageType(DamageTypes.nullBeamJail) || info.HasModdedDamageType(DamageTypes.ethJail) || info.HasModdedDamageType(DamageTypes.entropyJail))
            {
                if (NetworkServer.active)
                {
                    self.body.AddTimedBuff(RoR2Content.Buffs.Nullified, 3f);
                }
            }

            orig(self, info);

        }


        private static void VoidcridDeathBombFake(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo)
        {
            if (damageInfo.rejected || Voidcrid.VoidcridDef.VoidcridBombDeath.Value == false)
            {
                originalMethod(@this, damageInfo);
                return;
            }


            originalMethod(@this, damageInfo);
            if (damageInfo.HasModdedDamageType(DamageTypes.voidcridDeath))
            {
                if (!@this.alive && @this.wasAlive && @this.body)
                {
                    Vector3 pos = @this.body.corePosition;
                    float radius = @this.body.bestFitRadius;

                    if (damageInfo.attacker)
                    {
                        CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
                        if (attacker != null)
                        {
                            EffectManager.SpawnEffect(
                                EffectProvider.VoidcridSilentKill,
                                new EffectData
                                {
                                    origin = pos,
                                    scale = radius
                                },
                                true
                            );
                        }
                    }
                }
            }
        }

        public static void Hook()
        {

            On.RoR2.HealthComponent.TakeDamage += JailJailJail;

            On.RoR2.HealthComponent.TakeDamage += VoidcridDeathBombFake;

        }

    }



}
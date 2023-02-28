using RoR2;
using RoR2.Achievements;
using static R2API.DamageAPI;
using Voidcrid.Modules;

namespace Voidcrid.Achievements
{
    [RegisterAchievement("RightToJail", "Skins.Croco.Blackrid", null, typeof(VoidcridJailUnlock))]
    public class DeepVoidcridAcievement : VoidcridAchievements
    {
        private class VoidcridJailUnlock : BaseServerAchievement
        {
            protected virtual int jailRequirement => 1;

            private int _jailAchievement = 0;
            protected virtual BodyIndex jailBodyIndex => BodyCatalog.FindBodyIndex("VoidJailerBody");
            public override void OnInstall()
            {
                base.OnInstall();
                On.RoR2.HealthComponent.TakeDamage += onJailDamage;

            }

            public override void OnUninstall()
            {
                base.OnUninstall();
                On.RoR2.HealthComponent.TakeDamage -= onJailDamage;
            }

            private void onJailDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
            {

                bool jailerBody = self.body.bodyIndex == jailBodyIndex && info.HasModdedDamageType(DamageTypes.nullBeamJail) || info.HasModdedDamageType(DamageTypes.ethJail) || info.HasModdedDamageType(DamageTypes.entropyJail);


                if (jailerBody)
                {
                    this._jailAchievement++;

                    if (this._jailAchievement >= jailRequirement)
                    {
                        base.Grant();


                    }
                }
                orig(self, info);
            }




        }


    }

}
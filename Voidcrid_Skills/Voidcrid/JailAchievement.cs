using RoR2;
using RoR2.Achievements;

namespace Voidcrid.Achievements { 
 [RegisterAchievement("RightToJail", "Skins.Croco.Blackrid", null,  typeof(VoidcridJailUnlock))]
    public class DeepVoidcridAcievement : VoidcridAchievements
    {
        private class VoidcridJailUnlock : BaseServerAchievement
        {
            protected virtual int jailRequirement => 1;
            // protected virtual BodyIndex newtBodyIndex => BodyCatalog.FindBodyIndex("ShopkeeperBody");
            
             private int _jailAchievement = 0;
             protected virtual BodyIndex jailBodyIndex => BodyCatalog.FindBodyIndex("VoidJailerBody");
             public override void OnInstall() {
                 base.OnInstall();
                 GlobalEventManager.onCharacterDeathGlobal += onCharacterDeathGlobal;

             }

            public override void OnUninstall() {
                base.OnUninstall();
                GlobalEventManager.onCharacterDeathGlobal -= onCharacterDeathGlobal;
            }

            private void onCharacterDeathGlobal(DamageReport damageReport) {

                bool newt = damageReport.victimBody && damageReport.victim._killingDamageType == 65536 && damageReport.victimBodyIndex == jailBodyIndex;
                
                
                if (newt) {
                    this._jailAchievement++;

                    if (this._jailAchievement >= jailRequirement) {
                        base.Grant();
                        
                    }
                }
        }



  
            }


        }

    }
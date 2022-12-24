using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace Voidcrid.Achievements {
	public class VoidcridWin : BaseAchievement
{
       
    [RegisterAchievement("Bisquick5", "Skins.Croco.Voidcrid", null,  typeof(VoidcridServerUnlock))]
    public class VoidcridUnlockAcievement : BaseAchievement
    {

            public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("CrocoBody");
        }

        public override void OnBodyRequirementMet() {
            base.OnBodyRequirementMet();

            base.SetServerTracked(true);
        }

        public override void OnBodyRequirementBroken() {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }
        private class VoidcridServerUnlock : BaseServerAchievement
        {
            protected virtual int newtRequirement => 1;
            protected virtual BodyIndex newtBodyIndex => BodyCatalog.FindBodyIndex("ShopkeeperBody");
            
             private int _paradoxAchieved = 0;

             public override void OnInstall() {
                 base.OnInstall();
                 GlobalEventManager.onCharacterDeathGlobal += onCharacterDeathGlobal;

             }

            public override void OnUninstall() {
                base.OnUninstall();

                GlobalEventManager.onCharacterDeathGlobal -= onCharacterDeathGlobal;
            }

            private void onCharacterDeathGlobal(DamageReport damageReport) {
                
                CharacterMaster currentPlayerMaster = NetworkUser.localPlayers[0].master;

                bool newt = damageReport.victimBody && damageReport.victimBodyIndex == newtBodyIndex;
                
                if (newt) {
                    this._paradoxAchieved++;

                    if (this._paradoxAchieved >= newtRequirement) {
                        currentPlayerMaster.TrueKill();
                        base.Grant();
                        
                    }
                }
            }



  
            }


        }

    }
}
using RoR2;
using RoR2.Achievements;

namespace Voidcrid.Achievements {

    
	public class VoidcridWin : BaseAchievement
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
       
    [RegisterAchievement("VoidcridUnlock", "Skins.Croco.Voidcrid", null,  typeof(VoidcridServerUnlock))]
    
        private class VoidcridServerUnlock : BaseServerAchievement
        {

            
                protected virtual BodyIndex newtBoiIndex => BodyCatalog.FindBodyIndex("NewtMonster");
                private int _grandfather = 0;
                protected virtual int newtRequirement => 1;

              private void onCharacterDeathGlobal(DamageReport damageReport) {

                bool newtboi = damageReport.victimBody && damageReport.victimBodyIndex == newtBoiIndex;
    
                if (newtboi) {
                    this._grandfather++;
                    if (this._grandfather >= newtRequirement) {
                        base.Grant();
                    }
                }
            }
			public override void OnInstall()
			{
				base.OnInstall();
                GlobalEventManager.onCharacterDeathGlobal += onCharacterDeathGlobal;

			}
                    public override void OnUninstall() {
                base.OnUninstall();

                GlobalEventManager.onCharacterDeathGlobal -= onCharacterDeathGlobal;
            }
        }




        // public override bool ShouldGrant(RunReport runReport)
        // {
        //     if (onCharacterDeathGlobal = 1 && this.localUser.cachedBody.bodyIndex == this.requiredBodyIndex)
        //     {
        //         base.Grant();
        //         return true;
        //     }


        //     return false;
        // }
    }

}

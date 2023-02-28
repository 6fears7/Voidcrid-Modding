using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace Voidcrid.Achievements
{
    [RegisterAchievement("GrandfatherParadox", "Skins.Croco.Voidcrid", null, typeof(VoidcridServerUnlock))]
    public class ParadoxUnlockAchievement : VoidcridAchievements
    {

        private class VoidcridServerUnlock : BaseServerAchievement
        {
            protected virtual int newtRequirement => 1;
            protected virtual BodyIndex newtBodyIndex => BodyCatalog.FindBodyIndex("ShopkeeperBody");

            private int _paradoxAchieved = 0;

            public override void OnInstall()
            {
                base.OnInstall();
                GlobalEventManager.onCharacterDeathGlobal += onCharacterDeathGlobal;

            }

            public override void OnUninstall()
            {
                base.OnUninstall();

                GlobalEventManager.onCharacterDeathGlobal -= onCharacterDeathGlobal;
            }

            private void onCharacterDeathGlobal(DamageReport damageReport)
            {
                // //Grant to all Acrids
                // for (int i = 0; i < NetworkUser.localPlayers.Count; i++)
                // {
                //     CharacterMaster playerMaster = NetworkUser.localPlayers[i].master;

                CharacterMaster currentPlayerMaster = NetworkUser.localPlayers[0].master;

                bool newt = damageReport.victimBody && damageReport.victimBodyIndex == newtBodyIndex;

                if (newt)
                {
                    this._paradoxAchieved++;

                    if (this._paradoxAchieved >= newtRequirement)
                    {
                        currentPlayerMaster.TrueKill();
                        base.Grant();

                    }
                }
            }

        }


    }
}
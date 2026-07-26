using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace Voidcrid.Achievements
{

    [RegisterAchievement("VoidcridUnlock", "Skills.Croco.Nullbeam", null, 0, typeof(VoidcridMasterUnlock))]
    public class DeepVoidcrid : VoidcridAchievements
    {
        private class VoidcridMasterUnlock : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                On.RoR2.CharacterMaster.OnInventoryChanged += CheckVoidcridUnlock;

            }

            public override void OnUninstall()
            {
                base.OnUninstall();
                On.RoR2.CharacterMaster.OnInventoryChanged -= CheckVoidcridUnlock;
            }
            private void CheckVoidcridUnlock(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
            {
                orig(self);

                if (self && self.teamIndex == TeamIndex.Player && self.inventory)
                {
                    // Permanent only: a temporarily-held item should not grant the unlock.
                    int count = VoidItems.CountPermanent(self.inventory);

                    if (count >= 7)
                    {
                        Chat.AddMessage(Util.GenerateColoredString("WARN?NG: C??NTA?NMENT BRE?CH", Color.magenta));
                        base.Grant();
                    }
                }
            }

        }


    }

}
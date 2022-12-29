using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace Voidcrid.Achievements { 

 [RegisterAchievement("VoidcridUnlock", "Skills.Croco.Nullbeam", null,  typeof(VoidcridMasterUnlock))]
    public class DeepVoidcrid : VoidcridAchievements
    {   
        private class VoidcridMasterUnlock : BaseServerAchievement
        {
             public override void OnInstall() {
                 base.OnInstall();
                On.RoR2.CharacterMaster.OnInventoryChanged += CheckVoidcridUnlock;

             }

            public override void OnUninstall() {
                base.OnUninstall();
               On.RoR2.CharacterMaster.OnInventoryChanged -= CheckVoidcridUnlock;
            }
 private void CheckVoidcridUnlock(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
        {
            orig(self);

            if (self && self.teamIndex == TeamIndex.Player && self.inventory)
            {
                int count = getVoidItemCount(self.inventory);
               
                if (count >= 4)
                {
                    Chat.AddMessage(Util.GenerateColoredString("WARN?NG: C??NTA?NMENT BRE?CH", Color.magenta));
                    base.Grant();
                }
            }
        }

            private int getVoidItemCount(Inventory inventory)
        {
            int output = 0;
            output += inventory.GetItemCount(DLC1Content.Items.CritGlassesVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ElementalRingVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ExplodeOnDeathVoid);
            output += inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ChainLightningVoid);
            output += inventory.GetItemCount(DLC1Content.Items.TreasureCacheVoid);
            output += inventory.GetItemCount(DLC1Content.Items.MushroomVoid);
            output += inventory.GetItemCount(DLC1Content.Items.BearVoid);
            output += inventory.GetItemCount(DLC1Content.Items.SlowOnHitVoid);
            output += inventory.GetItemCount(DLC1Content.Items.MissileVoid);
            output += inventory.GetItemCount(DLC1Content.Items.ExtraLifeVoid);
            output += inventory.GetItemCount(DLC1Content.Items.BleedOnHitVoid);
            output += inventory.GetItemCount(DLC1Content.Items.CloverVoid);
            output += inventory.GetItemCount(DLC1Content.Items.VoidMegaCrabItem);
            return output;
                }

            }


        }

    }
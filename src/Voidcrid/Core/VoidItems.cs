using System.Collections.Generic;
using RoR2;

namespace Voidcrid
{
    /// <summary>
    /// The void items Voidcrid treats as "corruption", and how to count them.
    /// </summary>
    /// <remarks>
    /// Counting is split because the game distinguishes permanent from temporary stacks. Callers
    /// driving a live effect want <see cref="CountEffective"/>, so the player gets what they are
    /// actually holding; callers gating a permanent unlock want <see cref="CountPermanent"/>, so a
    /// briefly-held item cannot buy content outright.
    /// </remarks>
    internal static class VoidItems
    {
        private static ItemDef[] _all;

        // Resolved lazily: DLC1Content is not populated until the game has loaded its content,
        // which is always well before the first death or inventory change.
        private static ItemDef[] All => _all ??= new[]
        {
            DLC1Content.Items.CritGlassesVoid,
            DLC1Content.Items.ElementalRingVoid,
            DLC1Content.Items.ExplodeOnDeathVoid,
            DLC1Content.Items.EquipmentMagazineVoid,
            DLC1Content.Items.ChainLightningVoid,
            DLC1Content.Items.TreasureCacheVoid,
            DLC1Content.Items.MushroomVoid,
            DLC1Content.Items.BearVoid,
            DLC1Content.Items.SlowOnHitVoid,
            DLC1Content.Items.MissileVoid,
            DLC1Content.Items.ExtraLifeVoid,
            DLC1Content.Items.BleedOnHitVoid,
            DLC1Content.Items.CloverVoid,
            DLC1Content.Items.VoidMegaCrabItem,
        };

        /// <summary>Void items currently in effect, temporary stacks included.</summary>
        internal static int CountEffective(Inventory inventory) => Count(inventory, permanentOnly: false);

        /// <summary>Void items held permanently, ignoring temporary stacks.</summary>
        internal static int CountPermanent(Inventory inventory) => Count(inventory, permanentOnly: true);

        private static int Count(Inventory inventory, bool permanentOnly)
        {
            if (!inventory)
            {
                return 0;
            }

            int total = 0;
            foreach (ItemDef item in All)
            {
                total += permanentOnly
                    ? inventory.GetItemCountPermanent(item)
                    : inventory.GetItemCountEffective(item);
            }

            return total;
        }
    }
}

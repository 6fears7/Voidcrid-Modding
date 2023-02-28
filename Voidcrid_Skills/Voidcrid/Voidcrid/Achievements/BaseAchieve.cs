using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace Voidcrid.Achievements
{
    public class VoidcridAchievements : BaseAchievement
    {

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("CrocoBody");
        }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();

            base.SetServerTracked(true);
        }

        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }


    }
}
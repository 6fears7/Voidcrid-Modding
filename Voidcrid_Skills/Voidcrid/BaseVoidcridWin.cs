// using RoR2;
// using RoR2.Achievements;
// using Voidcrid;
// using R2API;

// namespace Voidcrid.Achievements {
// 	public class VoidcridWin : BaseAchievement
// {
       
//     [RegisterAchievement("VoidcridUnlock", "Skins.Croco.Voidcrid", null, null)]
//     public class VoidcridUnlockAcievement : BaseEndingAchievement
//     {

//         public override BodyIndex LookUpRequiredBodyIndex()
//         {
//             return BodyCatalog.FindBodyIndex("CrocoBody");
//         }
//         public override bool ShouldGrant(RunReport runReport)
//         {
//             if (runReport.gameEnding && runReport.gameEnding.isWin && runReport.gameEnding == RoR2Content.GameEndings.MainEnding && this.localUser.cachedBody.bodyIndex == this.requiredBodyIndex)
//             {
//                 return true;
//             }
//             return false;
//         }
//     }
// }
// }
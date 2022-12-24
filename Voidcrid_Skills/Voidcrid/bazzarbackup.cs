// using RoR2;
// using RoR2.Achievements;

// namespace Voidcrid.Achievements {
// 	public class VoidcridWin : BaseAchievement
// {
       
//     [RegisterAchievement("ParadoxUnlock", "Skins.Croco.Voidcrid", null,  typeof(VoidcridServerUnlock))]
//         private class VoidcridServerUnlock : BaseServerAchievement
//         {
//             protected virtual int newtRequirement => 1;
//             protected virtual BodyIndex newtBodyIndex => BodyCatalog.FindBodyIndex("NewtMonster");

//             private int _paradoxAchieved = 0;


//             public override void OnInstall() {
//                 base.OnInstall();

//                 GlobalEventManager.onCharacterDeathGlobal += onCharacterDeathGlobal;

//             }

//             public override void OnUninstall() {
//                 base.OnUninstall();

//                 GlobalEventManager.onCharacterDeathGlobal -= onCharacterDeathGlobal;
//             }

//             private void onCharacterDeathGlobal(DamageReport damageReport) {

//                 bool newt = damageReport.victimBody && damageReport.victimBodyIndex == newtBodyIndex;
             
//                 if (newt) {
//                     this._paradoxAchieved++;
//                     if (this._paradoxAchieved > newtRequirement) {
//                         base.Grant();
//                     }
//                 }
//             }
//         }

//     }
// }

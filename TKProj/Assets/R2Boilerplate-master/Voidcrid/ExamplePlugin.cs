using System;
using BepInEx;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CustomSkillsTutorial
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.6Fears7.Voidcrid",
        "Voidcrid",
        "1.0.0")]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ContentAddition))]
    public class CustomSkillTutorial : BaseUnityPlugin
    {
        public void Awake()
        {
            //First we must load our survivor's Body prefab. For this tutorial, we are making a skill for Commando
            //If you would like to load a different survivor, you can find the key for their Body prefab at the following link
            //https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html
            GameObject crocoBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();
            GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();

            //We use LanguageAPI to add strings to the game, in the form of tokens
            LanguageAPI.Add("Flamethrower", "Deep Flame");
            LanguageAPI.Add("Burn", $"Release a burst of flame, <style=cArtifact>burning</style> enemies. The flame's longevity <style=cIsDamage>increases</style> with your speed.");
            LanguageAPI.Add("Beam", "Null Beam");
            LanguageAPI.Add("VBeam", $"Draw from the <style=cArtifact>Void</style>, collapsing and <style=cArtifact> choking</style> enemies.");
            LanguageAPI.Add("Escape", "Void Drift");
            LanguageAPI.Add("VEscape", $"Disappear into the Void, <style=cArtifact>ensaring enemies</style>.");

            //Now we must create a SkillDef
            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidBeam = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidEscape = ScriptableObject.CreateInstance<SkillDef>();        


            mySkillDef.activationState = new SerializableEntityStateType(typeof(CustomSkillsTutorial.Voidcrid.Voidcridbreath));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 2f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.cancelSprintingOnActivation = true;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.stockToConsume = 1;
            //For the skill icon, you will have to load a Sprite from your own AssetBundle
            mySkillDef.icon = null;
            mySkillDef.skillDescriptionToken = "Burn";
            mySkillDef.skillName = "Firebreath";
            mySkillDef.skillNameToken = "Flamethrower";

            voidBeam.activationState = new SerializableEntityStateType(typeof(CustomSkillsTutorial.Voidcrid.NullBeam));
            voidBeam.activationStateMachineName = "Weapon";
            voidBeam.baseMaxStock = 1;
            voidBeam.baseRechargeInterval = 10f;
            voidBeam.beginSkillCooldownOnSkillEnd = true;
            voidBeam.canceledFromSprinting = false;
            voidBeam.cancelSprintingOnActivation = true;
            voidBeam.fullRestockOnAssign = true;
            voidBeam.interruptPriority = InterruptPriority.Any;
            voidBeam.isCombatSkill = true;
            voidBeam.mustKeyPress = false;
            voidBeam.rechargeStock = 1;
            voidBeam.requiredStock = 1;
            voidBeam.stockToConsume = 1;
            //For the skill icon, you will have to load a Sprite from your own AssetBundle
            voidBeam.icon = null;
            voidBeam.skillDescriptionToken = "VBeam";
            voidBeam.skillName = "Gravity Beam";
            voidBeam.skillNameToken = "Beam";


            voidEscape.activationState = new SerializableEntityStateType(typeof(CustomSkillsTutorial.Voidcrid.VoidEscape));
            voidEscape.activationStateMachineName = "Weapon";
            voidEscape.baseMaxStock = 1;
            voidEscape.baseRechargeInterval = 10f;
            voidEscape.beginSkillCooldownOnSkillEnd = true;
            voidEscape.canceledFromSprinting = false;
            voidEscape.cancelSprintingOnActivation = true;
            voidEscape.fullRestockOnAssign = true;
            voidEscape.interruptPriority = InterruptPriority.Any;
            voidEscape.isCombatSkill = true;
            voidEscape.mustKeyPress = false;
            voidEscape.rechargeStock = 1;
            voidEscape.requiredStock = 1;
            voidEscape.stockToConsume = 1;

            voidEscape.icon = null;
            voidEscape.skillDescriptionToken = "VEscape";
            voidEscape.skillName = "Void Escape";
            voidEscape.skillNameToken = "Escape";

            ContentAddition.AddSkillDef(mySkillDef);
            ContentAddition.AddSkillDef(voidBeam);
            ContentAddition.AddSkillDef(voidEscape);
            

            SkillLocator skillLocator = voidcridBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillSecondary = skillLocator.secondary.skillFamily;
            SkillFamily specialSkill = skillLocator.special.skillFamily;
            SkillFamily skillUtility = skillLocator.utility.skillFamily;
    

            Array.Resize(ref skillSecondary.variants, skillSecondary.variants.Length + 1);
            skillSecondary.variants[skillSecondary.variants.Length - 1] = new SkillFamily.Variant

            {
                skillDef = voidBeam,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(voidBeam.skillNameToken, false, null)
            };

            Array.Resize(ref specialSkill.variants, specialSkill.variants.Length + 1);
            specialSkill.variants[specialSkill.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };

            Array.Resize(ref skillUtility.variants, skillUtility.variants.Length + 1);
            skillUtility.variants[skillUtility.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = voidEscape,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(voidEscape.skillNameToken, false, null)
            };


       }
    }
}
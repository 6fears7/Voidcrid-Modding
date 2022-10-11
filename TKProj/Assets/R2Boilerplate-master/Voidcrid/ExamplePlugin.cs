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

            //We use LanguageAPI to add strings to the game, in the form of tokens
            LanguageAPI.Add("Flamethrower", "Flame of the Deep");
            LanguageAPI.Add("Burn", $"Release a burst of flame, <style=cArtifact>burning</style> enemies. The flame's longevity <style=cIsDamage>increases</style> with your speed.");
            LanguageAPI.Add("Leap", "Umbral Leap");
            LanguageAPI.Add("VLeap", $"Spread the Void, <style=cArtifact>trapping and choking</style> enemies.");

            //Now we must create a SkillDef
            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidLeap = ScriptableObject.CreateInstance<SkillDef>();
            //Check step 2 for the code of the CustomSkillsTutorial.MyEntityStates.SimpleBulletAttack class
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

            voidLeap.activationState = new SerializableEntityStateType(typeof(CustomSkillsTutorial.Voidcrid.VoidcridLeap));
            voidLeap.activationStateMachineName = "Weapon";
            voidLeap.baseMaxStock = 1;
            voidLeap.baseRechargeInterval = 2f;
            voidLeap.beginSkillCooldownOnSkillEnd = true;
            voidLeap.canceledFromSprinting = false;
            voidLeap.cancelSprintingOnActivation = true;
            voidLeap.fullRestockOnAssign = true;
            voidLeap.interruptPriority = InterruptPriority.Any;
            voidLeap.isCombatSkill = true;
            voidLeap.mustKeyPress = false;
            voidLeap.rechargeStock = 1;
            voidLeap.requiredStock = 1;
            voidLeap.stockToConsume = 1;
            //For the skill icon, you will have to load a Sprite from your own AssetBundle
            voidLeap.icon = null;
            voidLeap.skillDescriptionToken = "Leap";
            voidLeap.skillName = "Umbral Leap";
            voidLeap.skillNameToken = "VLeap";

            //This adds our skilldef. If you don't do this, the skill will not work.
            ContentAddition.AddSkillDef(mySkillDef);
            ContentAddition.AddSkillDef(voidLeap);

            //Now we add our skill to one of the survivor's skill families
            //You can change component.primary to component.secondary, component.utility and component.special
            SkillLocator skillLocator = crocoBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillSecondary = skillLocator.secondary.skillFamily;
            SkillFamily utilitySkill = skillLocator.utility.skillFamily;
            //If this is an alternate skill, use this code.
            //Here, we add our skill as a variant to the existing Skill Family.
            Array.Resize(ref skillSecondary.variants, skillSecondary.variants.Length + 1);
            skillSecondary.variants[skillSecondary.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };

            Array.Resize(ref utilitySkill.variants, utilitySkill.variants.Length + 1);
            utilitySkill.variants[utilitySkill.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = voidLeap,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(voidLeap.skillNameToken, false, null)
            };


       }
    }
}
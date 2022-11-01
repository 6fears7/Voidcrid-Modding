using System;
using BepInEx;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.UI.CharacterSelectController;
using System.Collections.Generic;
using System.Linq;
using RoR2.UI;
using MonoMod.Cil;
using Mono.Cecil.Cil;


namespace Voidcrid
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.6Fears7.Voidcrid",
        "Voidcrid",
        "1.0.0")]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ContentAddition))]
    public class VoidcridDef : BaseUnityPlugin
    {
        public void Awake()
        {

            // GenericSkill skill;

            //First we must load our survivor's Body prefab. For this tutorial, we are making a skill for Commando
            //If you would like to load a different survivor, you can find the key for their Body prefab at the following link
            //https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html
            GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();
            

            
            //We use LanguageAPI to add strings to the game, in the form of tokens
            LanguageAPI.Add("Flamethrower", "Deep Flame");
            LanguageAPI.Add("Burn", $"Release a burst of flame, <style=cArtifact>burning</style> enemies. The flame's longevity <style=cIsDamage>increases</style> with your speed.");
            LanguageAPI.Add("Beam", "Null Beam");
            LanguageAPI.Add("VBeam", $"Draw from the <style=cArtifact>Void</style>, collapsing and <style=cArtifact> choking</style> enemies.");
            LanguageAPI.Add("Escape", "Void Drift");
            LanguageAPI.Add("VEscape", $"Disappear into the Void, <style=cArtifact>ensaring enemies</style>.");

            LanguageAPI.Add("Slash", "Void slash");
            LanguageAPI.Add("VSlash_Desc", "Cut down enemies");

            //Now we must create a SkillDef
            SkillDef voidBreath = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidBeam = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidEscape = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidSlash = ScriptableObject.CreateInstance<SkillDef>();
            // SkillDef passive = ScriptableObject.CreateInstance<SkillDef>();
        //    SkillDef NoneDef = ScriptableObject.CreateInstance<SkillDef>();

            // passiveDef.skillNameToken = "Voidcrid_Passive";
            // (passiveDef as ScriptableObject).name = passiveDef.skillNameToken;
            // passiveDef.skillDescriptionToken = "Voidcrid_Passive_Desc";
            // // passiveDef.icon = locator.passiveSkill.icon;
            // // passiveDef.keywordTokens = passiveSkill.keywordToken.Length>0 ? new string[] {passiveSkill.keywordToken} : null;
            // passiveDef.baseRechargeInterval = 0f;

            voidBreath.activationState = new SerializableEntityStateType(typeof(Voidcrid.Voidcridbreath));
            voidBreath.activationStateMachineName = "Weapon";
            voidBreath.baseMaxStock = 1;
            voidBreath.baseRechargeInterval = 2f;
            voidBreath.beginSkillCooldownOnSkillEnd = true;
            voidBreath.canceledFromSprinting = false;
            voidBreath.cancelSprintingOnActivation = true;
            voidBreath.fullRestockOnAssign = true;
            voidBreath.interruptPriority = InterruptPriority.Any;
            voidBreath.isCombatSkill = true;
            voidBreath.mustKeyPress = false;
            voidBreath.rechargeStock = 1;
            voidBreath.requiredStock = 1;
            voidBreath.stockToConsume = 1;
            //For the skill icon, you will have to load a Sprite from your own AssetBundle
            voidBreath.icon = null;
            voidBreath.skillDescriptionToken = "Burn";
            voidBreath.skillName = "Firebreath";
            voidBreath.skillNameToken = "Flamethrower";

            voidBeam.activationState = new SerializableEntityStateType(typeof(Voidcrid.NullBeam));
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


            
            voidSlash.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidSlash));
            voidSlash.activationStateMachineName = "Weapon";
            voidSlash.baseMaxStock = 1;
            voidSlash.baseRechargeInterval = 1f;
            voidSlash.beginSkillCooldownOnSkillEnd = true;
            voidSlash.canceledFromSprinting = false;
            voidSlash.cancelSprintingOnActivation = true;
            voidSlash.fullRestockOnAssign = true;
            voidSlash.interruptPriority = InterruptPriority.Any;
            voidSlash.isCombatSkill = true;
            voidSlash.mustKeyPress = false;
            voidSlash.rechargeStock = 1;
            voidSlash.requiredStock = 1;
            voidSlash.stockToConsume = 1;
            //For the skill icon, you will have to load a Sprite from your own AssetBundle
            voidSlash.icon = null;
            voidSlash.skillDescriptionToken = "VSlash_Desc";
            voidSlash.skillName = "Slash";
            voidSlash.skillNameToken = "Slash";

            voidEscape.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidEscape));
            voidEscape.activationStateMachineName = "Weapon";
            voidEscape.baseMaxStock = 1;
            voidEscape.baseRechargeInterval = 8f;
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

            ContentAddition.AddSkillDef(voidBreath);
            ContentAddition.AddSkillDef(voidBeam);
            ContentAddition.AddSkillDef(voidEscape);
            
        //    LanguageAPI.Add("VOIDCRID_PASSIVE","Voidtouched");
        //    LanguageAPI.Add("VOIDCRID_PASSIVE_DESC","Chance to jail enemies");
        //    NoneDef = ScriptableObject.CreateInstance<SkillDef>();
        //    NoneDef.skillNameToken = "VOIDCRID_PASSIVE";
        //    (NoneDef as ScriptableObject).name = NoneDef.skillNameToken;
        //    NoneDef.skillDescriptionToken = "VOIDCRID_PASSIVE_DESC";
        //    NoneDef.baseRechargeInterval = 0f;
        //    ContentAddition.AddSkillDef(NoneDef);

        //     On.RoR2.UI.LoadoutPanelController.Row.FromSkillSlot += (orig,owner,bodyI,slotI,slot) => {
        //      LoadoutPanelController.Row row = (LoadoutPanelController.Row)orig(owner,bodyI,slotI,slot);
        //      if((slot.skillFamily as ScriptableObject).name.Contains("Passive")){
        //          Transform label = row.rowPanelTransform.Find("SlotLabel") ?? row.rowPanelTransform.Find("LabelContainer").Find("SlotLabel");
        //          if(label)
        //           label.GetComponent<LanguageTextMeshController>().token = "Passive";
        //      }
        //      return row;
        //     };

        //         skill = voidcridBodyPrefab.AddComponent<GenericSkill>();
        //         SkillLocator locator = voidcridBodyPrefab.GetComponent<SkillLocator>();
        //         skill._skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
        //         (skill.skillFamily as ScriptableObject).name = voidcridBodyPrefab.name + "Passive";
        //         skill.skillFamily.variants = new SkillFamily.Variant[1];
        //         skill.skillFamily.variants[0] = new SkillFamily.Variant{skillDef = NoneDef,viewableNode = new ViewablesCatalog.Node(NoneDef.skillNameToken,false,null)}; 

                //   locator.passiveSkill.enabled = false;
                //   SkillDef passiveDef = ScriptableObject.CreateInstance<SkillDef>();
                //   passiveDef.skillNameToken = locator.passiveSkill.skillNameToken;
                //   (passiveDef as ScriptableObject).name = passiveDef.skillNameToken;
                //   passiveDef.skillDescriptionToken = locator.passiveSkill.skillDescriptionToken;
                //   passiveDef.icon = locator.passiveSkill.icon;
                //   passiveDef.keywordTokens = locator.passiveSkill.keywordToken.Length>0 ? new string[] {locator.passiveSkill.keywordToken} : null;
                //   passiveDef.baseRechargeInterval = 0f;
                    // ContentAddition.AddSkillDef(passiveDef);
                
                //   skill.skillFamily.variants[0] = new SkillFamily.Variant{skillDef = passiveDef,viewableNode = new ViewablesCatalog.Node(passiveDef.skillNameToken,false,null)};
                
                //  ContentAddition.AddSkillFamily(skill.skillFamily);

            
            // ContentAddition.AddSurvivorDef(voidcridUI);
            // ContentAddition.Add(passiveDef);


            SkillLocator skillLocator = voidcridBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillPrimary = skillLocator.primary.skillFamily;
            SkillFamily skillSecondary = skillLocator.secondary.skillFamily;
            SkillFamily specialSkill = skillLocator.special.skillFamily;
            SkillFamily skillUtility = skillLocator.utility.skillFamily;
            

        
            // SkillFamily voidPassive = blarg._skillFamily;

            // Array.Resize(ref voidPassive.variants, voidPassive.variants.Length + 1);
            // voidPassive.variants[voidPassive.variants.Length - 1] = new SkillFamily.Variant

            // {
            //     skillDef = passiveDef,
            //     // unlockableDef =,
            //     viewableNode = new ViewablesCatalog.Node(passiveDef.skillNameToken, false, null)
            // };


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
                skillDef = voidBreath,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(voidBreath.skillNameToken, false, null)
            };

            Array.Resize(ref skillUtility.variants, skillUtility.variants.Length + 1);
            skillUtility.variants[skillUtility.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = voidEscape,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(voidEscape.skillNameToken, false, null)
            };

            Array.Resize(ref skillPrimary.variants, skillPrimary.variants.Length + 1);
            skillPrimary.variants[skillPrimary.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = voidSlash,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(voidSlash.skillNameToken, false, null)
            };

       }


    }
}
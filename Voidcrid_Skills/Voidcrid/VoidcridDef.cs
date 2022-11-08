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

          

            //First we must load our survivor's Body prefab. For this tutorial, we are making a skill for Commando
            //If you would like to load a different survivor, you can find the key for their Body prefab at the following link
            //https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html
            GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();
            
            
            //We use LanguageAPI to add strings to the game, in the form of tokens
            LanguageAPI.Add("Flamethrower", "Deep Flame");
            LanguageAPI.Add("Burn", $"Release a burst of flame, <style=cDeath>burning</style> enemies.");
            LanguageAPI.Add("Beam", "N?ll Beam");
            LanguageAPI.Add("VBeam", $"<style=cArtifact>Void.</style> Draw from the <style=cArtifact>Void</style>, firing a laser beam for 4 seconds.");
            LanguageAPI.Add("Escape", "Ethereal Dr?ft");
            LanguageAPI.Add("VEscape", $"<style=cArtifact>Void.</style> Slip into the Void, with a chance to take enemies with you.");

            LanguageAPI.Add("Slash", "Entr?py");
            LanguageAPI.Add("VSlash_Desc", "<style=cArtifact>Void.</style> <style=cDeath>Bleed.</style> <style=cIsDamage>Unstable.</style> Engulf the Void, <style=cDeath>harming</style> yourself for 25% health with a chance to <style=cArtifact>jail</style> or inflict <style=cDeath>bleed</style> on enemies.");

            LanguageAPI.Add("VOIDCRID_PASSIVE", "Welcome to the <style=cArtifact>Void.</style>");
            LanguageAPI.Add("VOIDCRID_PASSIVE_DESC", "All <style=cArtifact>Void</style> attacks have a chance to <style=cArtifact>jail</style> enemies.");

            LanguageAPI.Add("VOIDCRID", "Voidcrid");

            LanguageAPI.Add("VOIDCRID_DESC", "All the details");

            //Now we must create a SkillDef
            SkillDef voidBreath = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidBeam = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidEscape = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidBite = ScriptableObject.CreateInstance<SkillDef>();
            SurvivorDef voidcrid = ScriptableObject.CreateInstance<SurvivorDef>();
    
           
            
            // SkillDef passive = ScriptableObject.CreateInstance<SkillDef>();

            // NoneDef.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidcridDef));
            // passiveDef.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidcridDef));
            voidBreath.activationState = new SerializableEntityStateType(typeof(Voidcrid.Voidcridbreath));
            voidBreath.activationStateMachineName = "Weapon";
            voidBreath.baseMaxStock = 1;
            voidBreath.baseRechargeInterval = 1f;
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


            
            voidBite.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidBleed));
            voidBite.activationStateMachineName = "Weapon";
  		    voidBite.baseMaxStock = 1;
		    voidBite.baseRechargeInterval = 4f;
		    voidBite.beginSkillCooldownOnSkillEnd = true;
		    voidBite.canceledFromSprinting = false;
		    voidBite.fullRestockOnAssign = true;
		    voidBite.interruptPriority = InterruptPriority.Any;
		    voidBite.resetCooldownTimerOnUse = false;
		    voidBite.isCombatSkill = true;
		    voidBite.mustKeyPress = false;
		    voidBite.cancelSprintingOnActivation = true;
		    voidBite.rechargeStock = 1;
		    voidBite.requiredStock = 1;
		    voidBite.stockToConsume = 1;
            //For the skill icon, you will have to load a Sprite from your own AssetBundle
            voidBite.icon = null;
            voidBite.skillDescriptionToken = "VSlash_Desc";
            voidBite.skillName = "Slash";
            voidBite.skillNameToken = "Slash";

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

     
            SkillLocator skillLocator = voidcridBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillPrimary = skillLocator.primary.skillFamily;
            SkillFamily skillSecondary = skillLocator.secondary.skillFamily;
            SkillFamily specialSkill = skillLocator.special.skillFamily;
            SkillFamily skillUtility = skillLocator.utility.skillFamily;

            if (voidcridBodyPrefab) {

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "VOIDCRID_PASSIVE";
            skillLocator.passiveSkill.skillDescriptionToken = "VOIDCRID_PASSIVE_DESC";
            
            }

            

        
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
                skillDef = voidBite,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(voidBite.skillNameToken, false, null)
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
                skillDef = voidBreath,
                // unlockableDef =,
                viewableNode = new ViewablesCatalog.Node(voidBreath.skillNameToken, false, null)
            };

       }



    }
}
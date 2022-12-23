using System;
using BepInEx;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using BepInEx.Configuration;
using System.Runtime.CompilerServices;
using Voidcrid.Achievements;



namespace Voidcrid
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.sixfears7.Voidcrid",
        "Voidcrid",
        "1.0.0")]
        
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]

    public class VoidcridDef : BaseUnityPlugin
    {
        

        public static ConfigEntry<float> NullBeamOverrideJailChance { get; set; }
        public static ConfigEntry<float> EntropyOverrideJailChance { get; set; }
        public static ConfigEntry<float> EtherealDriftOverrideJailChance { get; set; }
        public static ConfigEntry<float> NullBeamOverrideDamage{ get; set; }
        public static ConfigEntry<float> NullBeamOverrideDuration { get; set; }
        public static ConfigEntry<float> FlamebreathOverrideDuration { get; set; }
        public static ConfigEntry<float> EntropyOverrideDamage { get; set; }
        public static ConfigEntry<float> EntropyOverrideFireSpeed { get; set; }
        public static ConfigEntry<float> EtherealDriftOverrideDamage {get; set;}
        public static ConfigEntry<float> ScepterEntropyOverrideDamage {get; set;}

        public static ConfigEntry<float> FlamebreathOverrideDamage {get; set;}
        public static ConfigEntry<float> ScepterEntropyOverrideFireSpeed {get; set;}
        public static ConfigEntry<float> ScepterEntropyOverrideVoidJailChance {get; set;}

        public static ConfigEntry<float> ScepterEntropyOverrideRadius {get; set;}

        public static ConfigEntry<float> FlamebreathOverrideRecharge {get; set;}

        public static ConfigEntry<float> FlamebreathOverrideTickFreq {get; set;}

        public static ConfigEntry<float> NullBeamOverrideRecharge {get; set;}
        public static ConfigEntry<float> EtherealDriftOverrideRecharge {get; set;}
        public static ConfigEntry<float> EntropyOverrideRecharge {get; set;}

        public static ConfigEntry<float> EntropyOverrideRadius {get; set;}
        public static ConfigEntry<float> ScepterEntropyOverrideRecharge {get; set;}

        public static ConfigEntry<bool> VoidcridPassiveShow {get; set;}

        public static ConfigEntry<bool> Seasonal {get; set;}

        public static SkillDef voidScepter;
        public static bool ancientScepterInstalled = false;


        internal static AssetBundle mainAssetBundle;
        GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();

     
        private const string assetbundleName = "acrid3";
        private const string csProjName = "Voidcrid";


        private static UnlockableDef survivorUnlock;
        public const string characterOutro = "..and so it left, a shell of its former self.";
        public const string characterOutroFailure = "..and so it stayed, forever chained to the Abyss.";
        public void Awake()

        
        {
            		
                    
                    FlamebreathOverrideRecharge = Config.Bind<float>(
					"Recharge Interval",
					"Flamebreath Recharge",
					0.5f,
					"Measured in seconds"
				);

                    NullBeamOverrideRecharge = Config.Bind<float>(
					"Recharge Interval",
					"Null Beam Recharge",
					10f,
					"Measured in seconds"
				);

                    EtherealDriftOverrideRecharge = Config.Bind<float>(
					"Recharge Interval",
					"Ethereal Drift Recharge",
					8f,
					"Measured in seconds"
				);

                    EntropyOverrideRecharge = Config.Bind<float>(
					"Recharge Interval",
					"Entropy Recharge",
					6f,
					"Measured in seconds"
				);

                
                    ScepterEntropyOverrideRecharge = Config.Bind<float>(
					"Recharge Interval",
					"Deeprotted Entropy Recharge",
					6f,
					"Measured in seconds"
				);
                
               		NullBeamOverrideJailChance = Config.Bind<float>(
					"JailChance",
					"NullBeamJailChance",
					.1f,
					"Null Beam jail chance, measured in percentage."
				);
                
                    EtherealDriftOverrideJailChance = Config.Bind<float>(
					"JailChance",
					"EtherealDriftJailChance",
					5f,
					"Ethereal Drift jail chance, measured in percentage."
				);

                    EntropyOverrideJailChance = Config.Bind<float>(
					"JailChance",
					"EntropyJailChance",
					3f,
					"Entropy jail chance, measured in percentage"
				);

                    NullBeamOverrideDamage = Config.Bind<float>(
					"NullBeam",
					"Damage",
					0.3f,
					"NullBeam damageCoefficientPerSecond damage"
				);

                    NullBeamOverrideDuration = Config.Bind<float>(
					"NullBeam",
					"Duration",
					2.5f,
					"NullBeam maximumDuration, measured in seconds"
				);

                    EntropyOverrideDamage = Config.Bind<float>(
					"Entropy",
					"Damage",
					3f,
					"Entropy damage multiplier"
				);

                     EntropyOverrideFireSpeed = Config.Bind<float>(
					"Entropy",
					"Firing Speed",
					0.3f,
					"Entropy successive attack speed, measured in seconds"
				);

                FlamebreathOverrideDuration = Config.Bind<float>(
					"Flamebreath",
					"Duration",
					2f,
					"Flamebreath's duration, measured in seconds. Total duration: (Flamebreath duration + Attack Speed stat)"
				);

                FlamebreathOverrideDamage = Config.Bind<float>(
					"Flamebreath",
					"Damage",
					10f,
					"Flamebreath's totalDamageCoefficient, is divided over Flamebreath duration * tickFrequency: (totalDamageCoef / [Flamebreath duration * tickFrequency])"
				);

                    EtherealDriftOverrideDamage = Config.Bind<float>(
					"Ethereal Drift",
					"Damage",
					1f,
					"Blast Attack base damage"
				);

                    EntropyOverrideRadius = Config.Bind<float>(
					"Entropy",
					"Radius",
					12f,
					"Blast Attack radius"
				);

                    ScepterEntropyOverrideDamage = Config.Bind<float>(
					"Deeprotted Entropy (Scepter)",
					"Damage",
					4f,
					"Scepter's Entropy Blast Attack base damage"
				);

                    ScepterEntropyOverrideFireSpeed = Config.Bind<float>(
					"Deeprotted Entropy (Scepter)",
					"Firing Speed",
					0.3f,
					"Scepter's Entropy successive attack speed, measured in seconds"
				);

                    ScepterEntropyOverrideRadius = Config.Bind<float>(
					"Deeprotted Entropy (Scepter)",
					"Radius",
					12f,
					"Scepter's Entropy radius"
				);

                
                    VoidcridPassiveShow = Config.Bind<bool>(
					"Voidcrid",
					"Display",
					true,
					"Shows the Voidcrid fake Passive description"
				);

                 Seasonal = Config.Bind<bool>(
					"Voidcrid",
					"Seasonal",
					true,
					"Activates seasonal attributes"
				);


                    
                
            LoadAssetBundle();
            CreateUnlockableDef();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"))
            {
                ancientScepterInstalled = true;
                Debug.Log("ANCIENT SCEPTER REGISTERED");
                ScepterSkillSetup();
                ScepterSetup();
            }

            //If you would like to load a different survivor, you can find the key for their Body prefab at the following link
            //https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html
            
            //             On.RoR2.SurvivorCatalog.Init += (orig) =>
            // {
            //     orig();

            //     // AddVoidcridSkin();
            // };


            // LanguageAPI.Add("VOIDCRID_SKIN", "Voidcrid");
            LanguageAPI.Add("VOIDCRID_FLAMEBREATH", "Flamebreath");
            LanguageAPI.Add("VOIDCRID_FLAMEBREATH_DESC", $"<style=cDeath>Igniting.</style> <style=cIsDamage>Agile.</style> Release a burst of <style=cIsDamage>flame</style>, <style=cDeath>burning</style> enemies for <style=cIsDamage>250%</style> damage.");
            LanguageAPI.Add("VOIDCRID_NULLBEAM", $"<style=cArtifact>「N?ll Beam』</style>");
            LanguageAPI.Add("VOIDCRID_NULLBEAM_DESC", $"<style=cArtifact>Void.</style> Draw deep from the <style=cArtifact>Void</style>, battering enemies with a swath of <style=cDeath>tentacles</style> for <style=cIsDamage>900%</style> damage.");
            LanguageAPI.Add("VOIDCRID_VOIDDRIFT", $"<style=cArtifact>「Ethereal Dr?ft』</style>");
            LanguageAPI.Add("VOIDCRID_VOIDRIFT_DESC", $"<style=cArtifact>Void.</style> <style=cIsUtility>Seasonal.</style> <style=cIsDamage>Stunning.</style> Slip into the <style=cArtifact>Void</style> dealing <style=cIsDamage>400% total</style> damage, with a chance to take enemies with you.");
            LanguageAPI.Add("VOIDCRID_ENTROPY", $"<style=cArtifact>「Entr<style=cIsHealing>?</style>py』</style>");
            LanguageAPI.Add("VOIDCRID_ENTROPY_DESC", $"<style=cArtifact>Void.</style> <style=cIsUtility>Seasonal.</style> <style=cIsDamage>Agile.</style> <style=cIsHealing>Poisonous.</style> <style=cIsDamage>Unstable.</style> Reorganize your cells, <style=cIsHealing>healing</style> or <style=cDeath>harming</style> yourself for <style=cIsDamage>25%</style> health to damage for <style=cIsDamage>{EntropyOverrideDamage.Value}00% x 3</style> damage or <style=cIsHealing>poison</style> enemies.");
            LanguageAPI.Add("VOIDCRID_NULLBEAM", $"<style=cArtifact>「N?ll Beam』</style>");
            LanguageAPI.Add("VOIDCRID_NULLBEAM_DESC", $"<style=cArtifact>Void.</style> Draw deep from the <style=cArtifact>Void</style>, battering enemies with a swath of <style=cDeath>tentacles</style> for <style=cIsDamage>900%</style> damage.");
            LanguageAPI.Add("VOIDCRID_VOIDDRIFT", $"<style=cArtifact>「Ethereal Dr?ft』</style>");
            LanguageAPI.Add("VOIDCRID_VOIDRIFT_DESC", $"<style=cArtifact>Void.</style> <style=cIsUtility>Seasonal.</style> <style=cIsDamage>Stunning.</style> Slip into the <style=cArtifact>Void</style> dealing <style=cIsDamage>400% total</style> damage, with a chance to take enemies with you.");
            LanguageAPI.Add("VOIDCRID_ENTROPY", $"<style=cArtifact>「Entr<style=cIsHealing>?</style>py』</style>");
            LanguageAPI.Add("VOIDCRID_ENTROPY_DESC", $"<style=cArtifact>Void.</style> <style=cIsUtility>Seasonal.</style> <style=cIsDamage>Agile.</style> <style=cIsHealing>Poisonous.</style> <style=cIsDamage>Unstable.</style> Reorganize your cells, <style=cIsHealing>healing</style> or <style=cDeath>harming</style> yourself for <style=cIsDamage>25%</style> health to damage for <style=cIsDamage>{EntropyOverrideDamage.Value}00% x 3</style> damage or <style=cIsHealing>poison</style> enemies.");


            LanguageAPI.Add("SEASONAL_VOIDCRID_PASSIVE", "<style=cIsUtility>Void</style>crid");
            LanguageAPI.Add("SEASONAL_VOIDCRID_PASSIVE_DESC", "<style=cIsUtility>Seasonal.</style> Some attacks have a chance to <style=cIsUtility>freeze</style> enemies.");
            LanguageAPI.Add("SEASONAL_VOIDCRID_PASSIVE", "<style=cIsUtility>Void</style>crid");
            LanguageAPI.Add("SEASONAL_VOIDCRID_PASSIVE_DESC", "<style=cIsUtility>Seasonal.</style> Some attacks have a chance to <style=cIsUtility>freeze</style> enemies.");
            LanguageAPI.Add("VOIDCRID_PASSIVE", "<style=cArtifact>Void</style>crid");
            LanguageAPI.Add("VOIDCRID_PASSIVE_DESC", "All <style=cArtifact>Void</style> attacks have a chance to <style=cArtifact>jail</style> enemies.");
            
            LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_NAME" , "Grandfather Paradox");
	        LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_DESCRIPTION", "You monster.");

            if (VoidcridPassiveShow.Value == true) {

            LanguageAPI.Add("VOIDCRID_OUTRO_FLAVOR", characterOutro);
            LanguageAPI.Add("VOIDCRID_OUTRO_FAILURE", characterOutroFailure);
            }
            
            
            SkillDef voidBreath = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidBeam = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidEscape = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidPoison = ScriptableObject.CreateInstance<SkillDef>();
    
          
          
           
            voidBreath.activationState = new SerializableEntityStateType(typeof(Voidcrid.Voidcridbreath));
            voidBreath.activationStateMachineName = "Weapon";
            voidBreath.baseMaxStock = 1;
            voidBreath.baseRechargeInterval = FlamebreathOverrideRecharge.Value;
            voidBreath.baseRechargeInterval = FlamebreathOverrideRecharge.Value;
            voidBreath.beginSkillCooldownOnSkillEnd = true;
            voidBreath.canceledFromSprinting = false;
            voidBreath.cancelSprintingOnActivation = true;
            voidBreath.fullRestockOnAssign = true;
            voidBreath.interruptPriority = InterruptPriority.PrioritySkill;
            voidBreath.isCombatSkill = true;
            voidBreath.mustKeyPress = true; //test this with Backpack
            voidBreath.mustKeyPress = true; //test this with Backpack
            voidBreath.rechargeStock = 1;
            voidBreath.requiredStock = 1;
            voidBreath.stockToConsume = 1;
            voidBreath.icon = mainAssetBundle.LoadAsset<Sprite>("deepflame2.png");    
            voidBreath.skillDescriptionToken = "VOIDCRID_FLAMEBREATH_DESC";
            voidBreath.skillName = "VOIDCRID_FLAMEBREATH";
            voidBreath.skillNameToken = "VOIDCRID_FLAMEBREATH";

            voidBeam.activationState = new SerializableEntityStateType(typeof(Voidcrid.NullBeam));
            voidBeam.activationStateMachineName = "Weapon";
            voidBeam.baseMaxStock = 1;
            voidBeam.baseRechargeInterval = NullBeamOverrideRecharge.Value;
            voidBeam.baseRechargeInterval = NullBeamOverrideRecharge.Value;
            voidBeam.beginSkillCooldownOnSkillEnd = true;
            voidBeam.canceledFromSprinting = false;
            voidBeam.cancelSprintingOnActivation = true;
            voidBeam.fullRestockOnAssign = true;
            voidBeam.interruptPriority = InterruptPriority.PrioritySkill;
            voidBeam.isCombatSkill = true;
            voidBeam.rechargeStock = 1;
            voidBeam.requiredStock = 1;
            voidBeam.stockToConsume = 1;
            voidBeam.icon = mainAssetBundle.LoadAsset<Sprite>("nullbeam2.png");        
            voidBeam.skillDescriptionToken = "VOIDCRID_NULLBEAM_DESC";
            voidBeam.skillName = "VOIDCRID_NULLBEAM";
            voidBeam.skillNameToken = "VOIDCRID_NULLBEAM";
            voidBeam.mustKeyPress = true;
            voidBeam.mustKeyPress = true;


            
            voidPoison.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidBleed));
            voidPoison.activationStateMachineName = "Weapon";
  		    voidPoison.baseMaxStock = 1;
		    voidPoison.baseRechargeInterval = EntropyOverrideRecharge.Value;
		    voidPoison.baseRechargeInterval = EntropyOverrideRecharge.Value;
		    voidPoison.beginSkillCooldownOnSkillEnd = true;
		    voidPoison.canceledFromSprinting = false;
		    voidPoison.fullRestockOnAssign = true;
		    voidPoison.interruptPriority = InterruptPriority.PrioritySkill;
		    voidPoison.resetCooldownTimerOnUse = false;
		    voidPoison.isCombatSkill = true;
		    voidPoison.mustKeyPress = false;
		    voidPoison.cancelSprintingOnActivation = true;
		    voidPoison.rechargeStock = 1;
		    voidPoison.requiredStock = 1;
		    voidPoison.stockToConsume = 1;
            voidPoison.icon = mainAssetBundle.LoadAsset<Sprite>("entropy2.png");
            voidPoison.mustKeyPress = true;   
            voidPoison.icon = mainAssetBundle.LoadAsset<Sprite>("entropy2.png");
            voidPoison.mustKeyPress = true;   

            voidPoison.skillDescriptionToken = "VOIDCRID_ENTROPY_DESC";
            voidPoison.skillName = "VOIDCRID_ENTROPY";
            voidPoison.skillNameToken = "VOIDCRID_ENTROPY";

            voidEscape.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidEscape));
            voidEscape.activationStateMachineName = "Weapon";
            voidEscape.baseMaxStock = 1;
            voidEscape.baseRechargeInterval = EtherealDriftOverrideRecharge.Value;
            voidEscape.baseRechargeInterval = EtherealDriftOverrideRecharge.Value;
            voidEscape.beginSkillCooldownOnSkillEnd = true;
            voidEscape.canceledFromSprinting = false;
            voidEscape.cancelSprintingOnActivation = true;
            voidEscape.fullRestockOnAssign = true;
            voidEscape.interruptPriority = InterruptPriority.PrioritySkill;
            voidEscape.isCombatSkill = true;
            voidEscape.rechargeStock = 1;
            voidEscape.requiredStock = 1;
            voidEscape.stockToConsume = 1;
            voidEscape.mustKeyPress = true;
            voidEscape.mustKeyPress = true;

            voidEscape.icon = mainAssetBundle.LoadAsset<Sprite>("voiddrift.png");    
            voidEscape.skillDescriptionToken = "VOIDCRID_VOIDRIFT_DESC";
            voidEscape.skillName = "VOIDCRID_VOIDDRIFT";
            voidEscape.skillNameToken = "VOIDCRID_VOIDDRIFT";

            ContentAddition.AddSkillDef(voidBreath);
            ContentAddition.AddSkillDef(voidBeam);
            ContentAddition.AddSkillDef(voidEscape);
            ContentAddition.AddSkillDef(voidPoison);
            ContentAddition.AddSkillDef(voidPoison);
            // ContentAddition.AddUnlockableDef(voidcridUnlock);

     
            SkillLocator skillLocator = voidcridBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillPrimary = skillLocator.primary.skillFamily;
            SkillFamily skillSecondary = skillLocator.secondary.skillFamily;
            SkillFamily specialSkill = skillLocator.special.skillFamily;
            SkillFamily skillUtility = skillLocator.utility.skillFamily;
            

            if (VoidcridPassiveShow.Value == true && Seasonal.Value == false) {

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "VOIDCRID_PASSIVE";
            skillLocator.passiveSkill.skillDescriptionToken = "VOIDCRID_PASSIVE_DESC";
            skillLocator.passiveSkill.icon = mainAssetBundle.LoadAsset<Sprite>("voidcrid.png");

            } else if (VoidcridPassiveShow.Value == true && Seasonal.Value == true) {
            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "SEASONAL_VOIDCRID_PASSIVE";
            skillLocator.passiveSkill.skillDescriptionToken = "SEASONAL_VOIDCRID_PASSIVE_DESC";
            skillLocator.passiveSkill.icon = mainAssetBundle.LoadAsset<Sprite>("voidcridSeasonal.png");
            

            } 

            else {

                skillLocator.passiveSkill.enabled = false;
            }


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
                skillDef = voidPoison,
                // unlockableDef = voidcridUnlock,
                viewableNode = new ViewablesCatalog.Node(voidPoison.skillNameToken, false, null)
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




        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private void ScepterSetup()
        {

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(voidScepter, "CrocoBody", SkillSlot.Special, 1);
  

        }


            private void ScepterSkillSetup()
        {


            voidScepter = ScriptableObject.CreateInstance<SkillDef>();

            LanguageAPI.Add("VOIDCRID_SCEPTER_ENTROPY", $"<style=cArtifact>「Deeprotted Entr<style=cIsHealing>?</style>py』</style>");
            LanguageAPI.Add("VOIDCRID__SCEPTER_ENTROPY_DESC", $"<style=cArtifact>Void.</style> <style=cIsDamage>Agile.</style> <style=cIsHealing>Poisonous.</style> <style=cIsDamage>Unstable.</style> Damage is increased to <style=cIsDamage>{ScepterEntropyOverrideDamage.Value}00% x 3</style> and if held, <style=cArtifact>ensares</style> enemies for <style=cIsDamage> 15% </style> of your health and applies your passive.</style>");


            voidScepter.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidScepter));
            voidScepter.activationStateMachineName = "Weapon";
  		    voidScepter.baseMaxStock = 1;
		    voidScepter.baseRechargeInterval = ScepterEntropyOverrideRecharge.Value;
		    voidScepter.beginSkillCooldownOnSkillEnd = true;
		    voidScepter.canceledFromSprinting = false;
		    voidScepter.fullRestockOnAssign = true;
		    voidScepter.interruptPriority = InterruptPriority.PrioritySkill;
		    voidScepter.resetCooldownTimerOnUse = false;
		    voidScepter.isCombatSkill = true;
		    voidScepter.mustKeyPress = false;
		    voidScepter.cancelSprintingOnActivation = true;
		    voidScepter.rechargeStock = 1;
		    voidScepter.requiredStock = 1;
		    voidScepter.stockToConsume = 1;
            voidScepter.icon = mainAssetBundle.LoadAsset<Sprite>("deeprotentropy.png");
            voidScepter.mustKeyPress = true;
            voidScepter.skillDescriptionToken = "VOIDCRID__SCEPTER_ENTROPY_DESC";
            voidScepter.skillName = "VOIDCRID_SCEPTER_ENTROPY";
            voidScepter.skillNameToken = "VOIDCRID_SCEPTER_ENTROPY";

            ContentAddition.AddSkillDef(voidScepter);


        }
        internal static void LoadAssetBundle()
        {

            
            try
            {
                    
                mainAssetBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("Voidcrid.dll", "acrid3"));
                    
                Debug.Log("Loaded assetbundle" );


            }
            catch (Exception e)
            {
                Debug.Log("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e );
                return;
            }

        }
    
            private static UnlockableDef CreateUnlockableDef()
        {
            if (!survivorUnlock)
            {
                survivorUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
                survivorUnlock.cachedName = "Skins.Croco.Voidcrid";
                survivorUnlock.nameToken = "ACHIEVEMENT_VOIDCRIDUNLOCK_NAME";
                survivorUnlock.achievementIcon = mainAssetBundle.LoadAsset<Sprite>("deeprotentropy.png");
                ContentAddition.AddUnlockableDef(survivorUnlock);
            }

            // if (Config.forceUnlock) return null;
            return survivorUnlock;
        }
    }

}

        



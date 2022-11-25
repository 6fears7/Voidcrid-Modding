using System;
using BepInEx;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
// using RoR2.Achievements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using BepInEx.Configuration;
// using Voidcrid.Achievements;



namespace Voidcrid
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.sixfears7.Voidcrid",
        "Voidcrid",
        "1.0.0")]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ContentAddition), nameof(LoadoutAPI), nameof(UnlockableAPI))]
    
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


        

        internal static AssetBundle mainAssetBundle;
        GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();

     
        private const string assetbundleName = "acrid3";
        private const string csProjName = "Voidcrid";

        // public static UnlockableDef VoidcridSkinDef;

        public void Awake()

        
        {
            
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
					"NullBeam jail chance, measured in percentage"
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
					"Flamebreath's duration, measured in seconds. Note this skill's full duration is baseDuration (changed here) + Attack Speed stat"
				);
     
                
                
            LoadAssetBundle();

      
            //If you would like to load a different survivor, you can find the key for their Body prefab at the following link
            //https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html
            
            //             On.RoR2.SurvivorCatalog.Init += (orig) =>
            // {
            //     orig();

            //     // AddVoidcridSkin();
            // };


            LanguageAPI.Add("VOIDCRID_SKIN", "Voidcrid");
            //We use LanguageAPI to add strings to the game, in the form of tokens
            LanguageAPI.Add("VOIDCRID_FLAMEBREATH", "Flamebreath");
            LanguageAPI.Add("VOIDCRID_FLAMEBREATH_DESC", $"<style=cDeath>Igniting.</style> <style=cIsDamage>Agile.</style> Release a burst of <style=cIsDamage>flame</style>, <style=cDeath>burning</style> enemies for <style=cIsDamage>250%</style> damage.");
            LanguageAPI.Add("VOIDCRID_NULLBEAM", $"<style=cArtifact>N?ll Beam</style>");
            LanguageAPI.Add("VOIDCRID_NULLBEAM_DESC", $"<style=cArtifact>Void.</style> Draw deep from the <style=cArtifact>Void</style>, battering enemies with a swath of <style=cDeath>tentacles</style> for <style=cIsDamage>900%</style> damage.");
            LanguageAPI.Add("VOIDCRID_VOIDDRIFT", $"<style=cArtifact>Ethereal Dr?ft</style>");
            LanguageAPI.Add("VOIDCRID_VOIDRIFT_DESC", $"<style=cArtifact>Void.</style> <style=cIsDamage>Stunning.</style> Slip into the <style=cArtifact>Void</style> dealing <style=cIsDamage>400% total</style> damage, with a chance to take enemies with you.");

            LanguageAPI.Add("VOIDCRID_ENTROPY", $"<style=cArtifact>Entr<style=cIsHealing>?</style>py</style>");
            LanguageAPI.Add("VOIDCRID_ENTROPY_DESC", "<style=cArtifact>Void.</style> <style=cIsDamage>Agile.</style> <style=cIsHealing>Poisonous.</style> <style=cIsDamage>Unstable.</style> Reorganize your cells, <style=cIsHealing>healing</style> or <style=cDeath>harming</style> yourself for <style=cIsDamage>25%</style> health to damage for <style=cIsDamage>400% x 3</style> damage or <style=cIsHealing>poison</style> enemies.");

            LanguageAPI.Add("VOIDCRID_PASSIVE", "<style=cArtifact>Void</style>crid");
            LanguageAPI.Add("VOIDCRID_PASSIVE_DESC", "All <style=cArtifact>Void</style> attacks have a chance to <style=cArtifact>jail</style> enemies.");

            // LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_NAME", "Voidcrid: Alone at Last");
            // LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_DESC", "As Acrid, face your captor when all hope is lost.");




        //    UnlockableDef voidcridSkinUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            // voidcridSkinUnlock.cachedName = "Survivors.Croco.Voidcrid";
            // voidcridSkinUnlock.nameToken = "ACHIEVEMENT_VOIDCRIDCLEAR_NAME";
            // voidcridSkinUnlock.achievementIcon = mainAssetBundle.LoadAsset<Sprite>("voidcrid.png");
            // ContentAddition.AddUnlockableDef(voidcridSkinUnlock);
            SkillDef voidBreath = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidBeam = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidEscape = ScriptableObject.CreateInstance<SkillDef>();
            SkillDef voidPoison = ScriptableObject.CreateInstance<SkillDef>();
            // SkinDef voidSkin = ScriptableObject.CreateInstance<SkinDef>();

           
            voidBreath.activationState = new SerializableEntityStateType(typeof(Voidcrid.Voidcridbreath));
            voidBreath.activationStateMachineName = "Weapon";
            voidBreath.baseMaxStock = 1;
            voidBreath.baseRechargeInterval = 0.5f;
            voidBreath.beginSkillCooldownOnSkillEnd = true;
            voidBreath.canceledFromSprinting = false;
            voidBreath.cancelSprintingOnActivation = true;
            voidBreath.fullRestockOnAssign = true;
            voidBreath.interruptPriority = InterruptPriority.PrioritySkill;
            voidBreath.isCombatSkill = true;
            voidBreath.mustKeyPress = false;
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
            voidBeam.baseRechargeInterval = 10f;
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


            
            voidPoison.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidBleed));
            voidPoison.activationStateMachineName = "Weapon";
  		    voidPoison.baseMaxStock = 1;
		    voidPoison.baseRechargeInterval = 6f;
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

            voidPoison.skillDescriptionToken = "VOIDCRID_ENTROPY_DESC";
            voidPoison.skillName = "VOIDCRID_ENTROPY";
            voidPoison.skillNameToken = "VOIDCRID_ENTROPY";

            voidEscape.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidEscape));
            voidEscape.activationStateMachineName = "Weapon";
            voidEscape.baseMaxStock = 1;
            voidEscape.baseRechargeInterval = 8f;
            voidEscape.beginSkillCooldownOnSkillEnd = true;
            voidEscape.canceledFromSprinting = false;
            voidEscape.cancelSprintingOnActivation = true;
            voidEscape.fullRestockOnAssign = true;
            voidEscape.interruptPriority = InterruptPriority.PrioritySkill;
            voidEscape.isCombatSkill = true;
            voidEscape.mustKeyPress = false;
            voidEscape.rechargeStock = 1;
            voidEscape.requiredStock = 1;
            voidEscape.stockToConsume = 1;
            voidEscape.mustKeyPress = true;

            voidEscape.icon = mainAssetBundle.LoadAsset<Sprite>("voiddrift.png");    
            voidEscape.skillDescriptionToken = "VOIDCRID_VOIDRIFT_DESC";
            voidEscape.skillName = "VOIDCRID_VOIDDRIFT";
            voidEscape.skillNameToken = "VOIDCRID_VOIDDRIFT";

            ContentAddition.AddSkillDef(voidBreath);
            ContentAddition.AddSkillDef(voidBeam);
            ContentAddition.AddSkillDef(voidEscape);
            // ContentAddition.AddUnlockableDef(voidcridUnlock);

     
            SkillLocator skillLocator = voidcridBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillPrimary = skillLocator.primary.skillFamily;
            SkillFamily skillSecondary = skillLocator.secondary.skillFamily;
            SkillFamily specialSkill = skillLocator.special.skillFamily;
            SkillFamily skillUtility = skillLocator.utility.skillFamily;

            if (voidcridBodyPrefab) {

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "VOIDCRID_PASSIVE";
            skillLocator.passiveSkill.skillDescriptionToken = "VOIDCRID_PASSIVE_DESC";
            skillLocator.passiveSkill.icon = mainAssetBundle.LoadAsset<Sprite>("voidcrid.png");

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
        internal static void LoadAssetBundle()
        {

            
            try
            {
    
                    // using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{csProjName}.{assetbundleName}"))
 
                        mainAssetBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("Voidcrid.dll", "acrid3"));
                    
                Debug.Log("Loaded assetbundle" );


            }
            catch (Exception e)
            {
                Debug.Log("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e );
                return;
            }

        }

        
        //   private void AddVoidcridSkin()
        // {



        //     var bodyName = "CrocoBody";


        //     var bodyPrefab = BodyCatalog.FindBodyPrefab(bodyName);
        //     //Getting necessary components
        //     var renderers = bodyPrefab.GetComponentsInChildren<Renderer>(true);
        //     var skinController = bodyPrefab.GetComponentInChildren<ModelSkinController>();
        //     var mdl = skinController.gameObject;

        //     var blarg = new LoadoutAPI.SkinDefInfo
        //     {
        //         //Icon for your skin in the game, it can be any image, or you can use `LoadoutAPI.CreateSkinIcon` to easily create an icon that looks similar to the icons in the game.
        //         Icon = LoadoutAPI.CreateSkinIcon(Color.black, Color.magenta, Color.red, Color.magenta),
        //         //Replace `LumberJackCommando` with your skin name that can be used to access it through the code
        //         Name = "Voidcrid",
        //         //Replace `LUMBERJACK_SKIN` with your token
        //         NameToken = "VOIDCRID_SKIN",
        //         RootObject = mdl,
                
        //         BaseSkins = new SkinDef[] { skinController.skins[0] },

        //         UnlockableDef = VoidcridSkinDef,

  
        //         GameObjectActivations = new SkinDef.GameObjectActivation[0],
        //         //This is used to define which material should be used on a specific renderer.
        //         //Only one material per renderer(mesh) is can be used
        //         RendererInfos = new CharacterModel.RendererInfo[]
        //         {
        //             //To add another material replacement simply copy past this block right after and add `,` after the first one.
                   
        //             new CharacterModel.RendererInfo
        //             {
        //                 //Loading material from AssetBundle replace "@SkinTest:Assets/Resources/matLumberJack.mat" with your value.
        //                 //It should be in this format `{provider name}:{path to asset in unity}`
        //                 //To get path to asset you can right click on your asset in Unity and select `Copy path` option
        //                 defaultMaterial = mainAssetBundle.LoadAsset<Material>("pinkvoid.mat"),

        //                 //Should mesh cast shadows
        //                 defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
        //                 //Should mesh be ignored by overlays. For example shield outline from `Personal Shield Generator`
        //                 ignoreOverlays = false,
                      
        //                 renderer = renderers[2]
        //             },

        //                   new CharacterModel.RendererInfo
        //             {
                       
        //                 defaultMaterial = mainAssetBundle.LoadAsset<Material>("pink.mat"),

        //                 //Should mesh cast shadows
        //                 defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
        //                 //Should mesh be ignored by overlays. For example shield outline from `Personal Shield Generator`
        //                 ignoreOverlays = false,
                      
        //                 renderer = renderers[3]
        //             }
                    
        //         },
                

                
        //         // This is used to define which mesh should be used on a specific renderer.
        //         MeshReplacements = new SkinDef.MeshReplacement[]
        //         {
        //             //To add another mesh replacement simply copy past this block right after and add `,` after the first one.
        //             new SkinDef.MeshReplacement
        //             {
        //                 //Loading mesh from AssetBundle look at material replacement commentary to learn about how value should be changed.
        //                 mesh = mainAssetBundle.LoadAsset<Mesh>("FINALLYFXD.mesh"),
        //                 //Index you need can be found here: https://github.com/risk-of-thunder/R2Wiki/wiki/Creating-skin-for-vanilla-characters-with-custom-model#renderers
        //                 renderer = renderers[2]
        //             }
        //         },
        //         //You probably don't need to touch this line
        //         ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0],
        //         //This is used to add skins for minions e.g. EngiTurrets
        //         MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0],
        //     };

        
        

    //         //Adding new skin to a character's skin controller
    //         Array.Resize(ref skinController.skins, skinController.skins.Length + 1);
    //         skinController.skins[skinController.skins.Length - 1] = LoadoutAPI.CreateNewSkinDef(blarg);

    //         //Adding new skin into BodyCatalog

    //         var skinsField = typeof(BodyCatalog).GetFieldValue<SkinDef[][]>("skins");
    //         skinsField[(int) BodyCatalog.FindBodyIndex(bodyPrefab)] = skinController.skins;

    // }


    }

    // public abstract class VoidcridUnlockable : ModdedUnlockable
    // {
    //     public abstract string AchievementTokenPrefix { get; }
    //     public abstract string AchievementSpriteName { get; }

    //     public override string AchievementIdentifier { get => "ACHIEVEMENT_VOIDCRIDCLEAR_NAME"; }
    //     public override string UnlockableIdentifier { get => "ACHIEVEMENT_VOIDCRIDCLEAR_NAME"; }
    //     public override string AchievementNameToken { get => "ACHIEVEMENT_VOIDCRIDCLEAR_NAME"; }
    //     public override string AchievementDescToken { get =>"ACHIEVEMENT_VOIDCRIDCLEAR_DESC" ;}
    //     public override string UnlockableNameToken { get => "ACHIEVEMENT_VOIDCRIDCLEAR_NAME" ;}

    //     public override Sprite Sprite => VoidcridDef.mainAssetBundle.LoadAsset<Sprite>("voidcrid.png");

    //     public override Func<string> GetHowToUnlock
    //     {
    //         get => () => Language.GetStringFormatted("ACHIEVEMENT_VOIDCRIDCLEAR_DESC", new object[]
    //                         {
    //                             Language.GetString(AchievementDescToken),
    //                             Language.GetString(AchievementDescToken)
    //                         });
    //     }

    //     public override Func<string> GetUnlocked
    //     {
    //         get => () => Language.GetStringFormatted("ACHIEVEMENT_VOIDCRIDCLEAR_NAME", new object[]
    //                         {
    //                             Language.GetString(AchievementNameToken),
    //                             Language.GetString(AchievementDescToken)
    //                         });
    //     }
    // }
}

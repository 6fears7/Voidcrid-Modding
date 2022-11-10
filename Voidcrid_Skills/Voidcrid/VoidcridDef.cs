using System;
using BepInEx;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;



namespace Voidcrid
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.6Fears7.Voidcrid",
        "Voidcrid",
        "1.0.0")]
    [R2APISubmoduleDependency(nameof(LanguageAPI), nameof(ContentAddition), nameof(LoadoutAPI))]
    public class VoidcridDef : BaseUnityPlugin
    {
        internal static AssetBundle mainAssetBundle;

     
        private const string assetbundleName = "acrid3";
        private const string csProjName = "Voidcrid";

    


        public void Awake()
        {

            LoadAssetBundle();

      
            //If you would like to load a different survivor, you can find the key for their Body prefab at the following link
            //https://xiaoxiao921.github.io/GithubActionCacheTest/assetPathsDump.html
            GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();
            
                        On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                AddVoidcridSkin();
            };

            //Adding language token, so every time game sees `LUMBERJACK_SKIN` it will be replaced with `Lumberjack`
            //You can also use `LanguageAPI.Add("TOKEN", "Value", "Language")` to add localization for specific language
            //For example `LanguageAPI.Add("LUMBERJACK_SKIN", "Дровосек", "RU")`
            LanguageAPI.Add("VOIDCRID_SKIN", "Voidcrid");
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
            voidBeam.icon = mainAssetBundle.LoadAsset<Sprite>("voidcrid.png");        
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

        
          private void AddVoidcridSkin()
        {

            var bodyName = "CrocoBody";


            var bodyPrefab = BodyCatalog.FindBodyPrefab(bodyName);
            //Getting necessary components
            var renderers = bodyPrefab.GetComponentsInChildren<Renderer>(true);
            var skinController = bodyPrefab.GetComponentInChildren<ModelSkinController>();
            var mdl = skinController.gameObject;

                var skin = new LoadoutAPI.SkinDefInfo
            {
                //Icon for your skin in the game, it can be any image, or you can use `LoadoutAPI.CreateSkinIcon` to easily create an icon that looks similar to the icons in the game.
                Icon = LoadoutAPI.CreateSkinIcon(Color.black, Color.magenta, Color.red, Color.magenta),
                //Replace `LumberJackCommando` with your skin name that can be used to access it through the code
                Name = "fingers3",
                //Replace `LUMBERJACK_SKIN` with your token
                NameToken = "VOIDCRID_SKIN",
                RootObject = mdl,
              
                BaseSkins = new SkinDef[] { skinController.skins[0] },
                //Name of achievement after which skin will be unlocked
                //Leave that field empty if you want skin to be always available 
                // UnlockableDef = "",
                //This is used to disable/enable some gameobjects in body prefab.
                GameObjectActivations = new SkinDef.GameObjectActivation[0],
                //This is used to define which material should be used on a specific renderer.
                //Only one material per renderer(mesh) is can be used
                RendererInfos = new CharacterModel.RendererInfo[]
                {
                    //To add another material replacement simply copy past this block right after and add `,` after the first one.
                   
                    new CharacterModel.RendererInfo
                    {
                        //Loading material from AssetBundle replace "@SkinTest:Assets/Resources/matLumberJack.mat" with your value.
                        //It should be in this format `{provider name}:{path to asset in unity}`
                        //To get path to asset you can right click on your asset in Unity and select `Copy path` option
                        defaultMaterial = Resources.Load<Material>("Assets/Resources/ModdedAcrid/Default-Material.mat"),

                        //Should mesh cast shadows
                        defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                        //Should mesh be ignored by overlays. For example shield outline from `Personal Shield Generator`
                        ignoreOverlays = false,
                      
                        renderer = renderers[2]
                    }
                },
                //This is used to define which mesh should be used on a specific renderer.
                // MeshReplacements = new SkinDef.MeshReplacement[]
                // {
                //     //To add another mesh replacement simply copy past this block right after and add `,` after the first one.
                //     new SkinDef.MeshReplacement
                //     {
                //         //Loading mesh from AssetBundle look at material replacement commentary to learn about how value should be changed.
                //         mesh = Resources.Load<Mesh>(@"Voidcrid:Assets/Resources/ModdedAcrid/CrocoMesh.mesh"),
                //         //Index you need can be found here: https://github.com/risk-of-thunder/R2Wiki/wiki/Creating-skin-for-vanilla-characters-with-custom-model#renderers
                //         renderer = renderers[2]
                //     }
                // },
                //You probably don't need to touch this line
                ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0],
                //This is used to add skins for minions e.g. EngiTurrets
                MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0],
            };

            //Adding new skin to a character's skin controller
            Array.Resize(ref skinController.skins, skinController.skins.Length + 1);
            skinController.skins[skinController.skins.Length - 1] = LoadoutAPI.CreateNewSkinDef(skin);

            //Adding new skin into BodyCatalog

            var skinsField = typeof(BodyCatalog).GetFieldValue<SkinDef[][]>("skins");
            skinsField[(int) BodyCatalog.FindBodyIndex(bodyPrefab)] = skinController.skins;

    }


    }
}
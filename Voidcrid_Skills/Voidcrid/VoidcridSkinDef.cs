using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

//Change `SkinTest` to your project name
namespace Voidcrid
{
    //Marks mod as client-side
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    //Requests `R2API` submodules
    [R2APISubmoduleDependency(nameof(PrefabAPI), nameof(LoadoutAPI), nameof(ContentAddition), nameof(ContentAddition), nameof(LanguageAPI))]
    //Adds dependency to `R2API`
    [BepInDependency("com.bepis.r2api")]
    //Definition of a mod
    [BepInPlugin(
        //The GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config). Java package notation is commonly used, which is "com.[your name here].[your plugin name here]"
        "com.6fears7.VoidcridSkin",
        //The name is the name of the plugin that's displayed on load
        "VoidcridSkin",
        //The version number just specifies what version the plugin is.
        "1.0.0")]

    //Rename `SkinTest` to match your .cs file name
    public class VoidcridSkinDef : BaseUnityPlugin
    {
        private void Awake()
        {
            //Loading AssetBundle with a model. String value is "{ProjectName}.{AssetBundleFileName}"
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Voidcrid.moddedacrid"))
            {
                var MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                //This string value will be used as a part of resource path. Replace `@SkinTest` with your name prefferably it should be `@{mod name}`
                var loadedAsset = MainAssetBundle.LoadAsset<GameObject>("@Voidcrid:Assets/moddedcrid");
            }

            //Adding our skill after all characters were loaded
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                AddVoidcridSkin();
            };

            //Adding language token, so every time game sees `LUMBERJACK_SKIN` it will be replaced with `Lumberjack`
            //You can also use `LanguageAPI.Add("TOKEN", "Value", "Language")` to add localization for specific language
            //For example `LanguageAPI.Add("LUMBERJACK_SKIN", "Дровосек", "RU")`
            LanguageAPI.Add("VOIDCRID_SKIN", "Voidcrid");
        }

        private void AddVoidcridSkin()
        {

            var bodyName = "CrocoBody";

            try {

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
                //Defining skins that will be applyed before our.
                //Default skin index - 0, Monsoon skin index - 1
                //Or you can use `BaseSkins = Array.Empty<SkinDef>(),` if you don't want to add base skin
                //Because we will replace only Commando mesh, but not his pistols we have to use base skin.
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
                        defaultMaterial = Resources.Load<Material>("@Voidcrid:Assets/Resources/ModdedAcrid/Default-Material.mat"),
                        //Should mesh cast shadows
                        defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                        //Should mesh be ignored by overlays. For example shield outline from `Personal Shield Generator`
                        ignoreOverlays = false,
                        //Which renderer(mesh) to replace.
                        //Index you need can be found here: https://github.com/risk-of-thunder/R2Wiki/wiki/Creating-skin-for-vanilla-characters-with-custom-model#renderers
                        renderer = renderers[2]
                    }
                },
                //This is used to define which mesh should be used on a specific renderer.
                MeshReplacements = new SkinDef.MeshReplacement[]
                {
                    //To add another mesh replacement simply copy past this block right after and add `,` after the first one.
                    new SkinDef.MeshReplacement
                    {
                        //Loading mesh from AssetBundle look at material replacement commentary to learn about how value should be changed.
                        mesh = Resources.Load<Mesh>("@Voidcrid:Assets/Resources/ModdedAcrid/CrocoMesh.mesh"),
                        //Index you need can be found here: https://github.com/risk-of-thunder/R2Wiki/wiki/Creating-skin-for-vanilla-characters-with-custom-model#renderers
                        renderer = renderers[2]
                    }
                },
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

        } catch (Exception e)
        {

            Debug.LogWarning($" \"{e}\" Failed to add to \"{bodyName}\"");
        }
    }
    }
}
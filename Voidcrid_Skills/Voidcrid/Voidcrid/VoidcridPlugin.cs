using System;
using BepInEx;
using R2API;
using UnityEngine.AddressableAssets;
using RoR2;
using UnityEngine;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
namespace Voidcrid
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.sixfears7.Voidcrid",
        "Voidcrid",
        "1.0.0")]

    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.groovesalad.GrooveSaladSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.heyimnoob.NoopSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.plasmacore.PlasmaCoreSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.cwmlolzlz.skills", BepInDependency.DependencyFlags.SoftDependency)]

    public class VoidcridDef : BaseUnityPlugin
    {

        public static bool ancientScepterInstalled = false;
        public static bool skillsPlusInstalled = false;


        internal static GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();
        SkillLocator skillLocator = voidcridBodyPrefab.GetComponent<SkillLocator>();
        public static ConfigEntry<float> NullBeamOverrideJailChance { get; set; }
        public static ConfigEntry<float> EntropyOverrideJailChance { get; set; }
        public static ConfigEntry<float> EtherealDriftOverrideJailChance { get; set; }
        public static ConfigEntry<float> NullBeamOverrideDamage { get; set; }
        public static ConfigEntry<float> NullBeamOverrideDuration { get; set; }

        public static ConfigEntry<float> NullBeamOverrideProc { get; set; }
        public static ConfigEntry<float> FlamebreathOverrideDuration { get; set; }
        public static ConfigEntry<float> EntropyOverrideDamage { get; set; }
        public static ConfigEntry<float> EntropyOverrideFireSpeed { get; set; }
        public static ConfigEntry<float> EtherealDriftOverrideDamage { get; set; }
        public static ConfigEntry<float> ScepterEntropyOverrideDamage { get; set; }

        public static ConfigEntry<float> FlamebreathOverrideDamage { get; set; }
        public static ConfigEntry<float> ScepterEntropyOverrideFireSpeed { get; set; }
        public static ConfigEntry<float> ScepterEntropyOverrideVoidJailChance { get; set; }

        public static ConfigEntry<float> ScepterEntropyOverrideRadius { get; set; }

        public static ConfigEntry<float> FlamebreathOverrideRecharge { get; set; }

        public static ConfigEntry<float> FlamebreathOverrideTickFreq { get; set; }

        public static ConfigEntry<float> NullBeamOverrideRecharge { get; set; }
        public static ConfigEntry<float> EtherealDriftOverrideRecharge { get; set; }
        public static ConfigEntry<float> EntropyOverrideRecharge { get; set; }

        public static ConfigEntry<float> EntropyOverrideRadius { get; set; }
        public static ConfigEntry<float> EntropySelfDamage { get; set; }
        public static ConfigEntry<float> EntropySelfHeal { get; set; }

        public static ConfigEntry<Color> VoidGlow { get; set; }

        public static ConfigEntry<Color> ScepterGlow { get; set; }


        public static ConfigEntry<float> ScepterEntropyOverrideRecharge { get; set; }

        public static ConfigEntry<bool> VoidcridPassiveShow { get; set; }

        public static ConfigEntry<float> VoidcridFogDamageOverride { get; set; }

        public static ConfigEntry<bool> VoidcridBombDeath { get; set; }

        // public string characterOutro = "..and so it left, a shell of its former self.";
        //     string characterOutroFailure = "..and so it stayed, forever chained to the Abyss.";
        // public SkillLocator skillLocator;
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
            7f,
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
            12f,
            "Measured in seconds"
        );


            ScepterEntropyOverrideRecharge = Config.Bind<float>(
            "Recharge Interval",
            "Deeprotted Entropy Recharge",
            8f,
            "Measured in seconds"
        );

            NullBeamOverrideJailChance = Config.Bind<float>(
         "JailChance",
         "NullBeamJailChance",
         .4f,
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

            NullBeamOverrideProc = Config.Bind<float>(
            "NullBeam",
            "Proc",
            0.4f,
            "NullBeam proc, measured as a percentage"
        );


            EntropyOverrideDamage = Config.Bind<float>(
            "Entropy",
            "Damage",
            4f,
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

            EntropySelfDamage = Config.Bind<float>(
            "Entropy",
            "Self-Damage",
            .15f,
            "Damage taken to self from Entropy"
        );

            EntropySelfHeal = Config.Bind<float>(
            "Entropy",
            "Heal",
            .25f,
            "Healing amount from Entropy"
        );

            ScepterEntropyOverrideDamage = Config.Bind<float>(
            "Deeprotted Entropy (Scepter)",
            "Damage",
            5f,
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

            VoidGlow = Config.Bind<Color>(
            "Voicrid Glow",
            "Color",
            Color.magenta,
            "Voidcrid's Color Glow"
        );

            ScepterGlow = Config.Bind<Color>(
            "Voicrid Glow",
            "Scepter Color",
            Color.red,
            "Voidcrid's Scepter Color Glow"
        );

            VoidcridFogDamageOverride = Config.Bind<float>(
                "Entropy",
                "Fog Damage",
                0.08f,
                "Fog health fraction damage per second"
            );



            VoidcridPassiveShow = Config.Bind<bool>(
            "Voidcrid",
            "Display",
            true,
            "Shows the Voidcrid fake Passive description"
            );

            VoidcridBombDeath = Config.Bind<bool>(
            "Voidcrid",
            "Death",
            true,
            "Emit a devastating bomb on death"
        );
            

            Voidcrid.Language.LanguageSetup.SetLanguage();
            Voidcrid.SkillSetup.LoadAssetBundle();
            Voidcrid.SkillSetup.CreateFogProjectile();
            Voidcrid.SkillSetup.SetupSkills(skillLocator);
            Voidcrid.Modules.VoidcridDeathProjectile.Init();
            Voidcrid.Effects.EffectProvider.Init();
            Voidcrid.SkillSetup.DeathBehavior();
             Voidcrid.Hooks.HookSetup.Hook();

        }
        public static bool HasDeeprot(SkillLocator sk)
        {
            bool hasDeeprot = false;
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.plasmacore.PlasmaCoreSpikestripContent") && sk)
            {
                hasDeeprot = HasDeeprotInternal(sk);
            }
            return hasDeeprot;
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool HasDeeprotInternal(SkillLocator sk)
        {
            bool deeprotEquipped = false;
            if (PlasmaCoreSpikestripContent.Content.Skills.DeepRot.scriptableObject != null)
            {
                foreach (GenericSkill gs in sk.allSkills)
                {
                    if (gs.skillDef == PlasmaCoreSpikestripContent.Content.Skills.DeepRot.scriptableObject.SkillDefinition)
                    {
                        deeprotEquipped = true;
                        break;
                    }
                }
            }
            return deeprotEquipped;
        }

        private void Start()
        {


            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"))
            {
                ancientScepterInstalled = true;
                Voidcrid.SkillSetup.ScepterSkillSetup();
                Voidcrid.SkillSetup.ScepterSetup();
            }

            // if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.cwmlolzlz.skills"))
            // {
            //     skillsPlusInstalled = true;

            //     SkillsPlusCompat.init();
            //     Debug.Log("Init for SkillsCompat");

            // }

        }


    }

}





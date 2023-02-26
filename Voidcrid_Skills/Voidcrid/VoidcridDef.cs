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
using System.Linq;
using static R2API.DamageAPI;
using Voidcrid.Modules;
using Voidcrid.Effects;
using UnityEngine.Networking;
using CharacterBody = RoR2.CharacterBody;
using System.Collections.Generic;
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


        public static SkillDef voidScepter;
        public static bool ancientScepterInstalled = false;
        public static bool skillsPlusInstalled = false;

        internal static AssetBundle mainAssetBundle;
        GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();
        private const string assetbundleName = "acrid3";
        private const string csProjName = "Voidcrid";

        internal static GameObject voidFogProjectile;
        private static UnlockableDef entropyUnlock;

        private static UnlockableDef ethUnlock;

        private static UnlockableDef VoidcridUnlock;
        public const string characterOutro = "..and so it left, a shell of its former self.";
        public const string characterOutroFailure = "..and so it stayed, forever chained to the Abyss.";
        public string familyName = "Death States";
        
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

            LoadAssetBundle();
            SkillLocator skillLocator = voidcridBodyPrefab.GetComponent<SkillLocator>();
            CreateFogProjectile();
            FlamebreathSetup(skillLocator);
            NullBeamSetup(skillLocator);
            VoidEscapeSetup(skillLocator);
            EntropySetup(skillLocator);
            //InitializeDeathStates();
            Voidcrid.Modules.VoidcridDeathProjectile.Init();
            Voidcrid.Effects.EffectProvider.Init();
            DeathBehavior();
            Hook();
          


            LanguageAPI.Add("VOIDCRID_PASSIVE", "<style=cArtifact>Void</style>crid");
            LanguageAPI.Add("VOIDCRID_PASSIVE_DESC", "All <style=cArtifact>Void</style> attacks have a chance to <style=cArtifact>jail</style> enemies (and apply <style=cWorldEvent>Deeprot</style>, if selected).");

            LanguageAPI.Add("VOIDCRID_DEFAULT_DEATH", "Death");
            LanguageAPI.Add("VOIDCRID_DEFAULT_DEATH_DESC", "On death, die a normal death");
            LanguageAPI.Add("VOIDCRID_VOID_DEATH", "Void Death");

            LanguageAPI.Add("VOIDCRID_VOID_DEATH_DESC", "On death, die a Void servant's death");


            LanguageAPI.Add("ACHIEVEMENT_GRANDFATHERPARADOX_NAME", "Acrid: Grandfather Paradox");
            LanguageAPI.Add("ACHIEVEMENT_GRANDFATHERPARADOX_DESCRIPTION", "An unexpected amphibian, an unfortunate end.");

            LanguageAPI.Add("ACHIEVEMENT_RIGHTTOJAIL_NAME", "Acrid: Right To Jail");
            LanguageAPI.Add("ACHIEVEMENT_RIGHTTOJAIL_DESCRIPTION", "As Acrid, jail a Jailer.");

            LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_NAME", "...Left alone");
            LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_DESCRIPTION", "As Acrid, corrupt yourself 7 times to break containment.");


            if (VoidcridPassiveShow.Value == true)
            {
                SurvivorDef voidcridOutro = voidcridBodyPrefab.GetComponent<SurvivorDef>();

                skillLocator.passiveSkill.enabled = true;
                skillLocator.passiveSkill.skillNameToken = "VOIDCRID_PASSIVE";
                skillLocator.passiveSkill.skillDescriptionToken = "VOIDCRID_PASSIVE_DESC";
                skillLocator.passiveSkill.icon = mainAssetBundle.LoadAsset<Sprite>("icon.png");

                LanguageAPI.Add("CROCO_OUTRO_FLAVOR", characterOutro);
                LanguageAPI.Add("CROCO_MAIN_ENDING_ESCAPE_FAILURE_FLAVOR", characterOutroFailure);

                voidcridOutro.outroFlavorToken = characterOutro;
                voidcridOutro.mainEndingEscapeFailureFlavorToken = characterOutroFailure;
                               
            }
            else
            {

                skillLocator.passiveSkill.enabled = false;
            }

        }




        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ScepterSetup()
        {

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(voidScepter, "CrocoBody", SkillSlot.Special, 1);


        }


        private void FlamebreathSetup(SkillLocator skillLocator)
        {
            LanguageAPI.Add("VOIDCRID_FLAMEBREATH", "Flamebreath");
            LanguageAPI.Add("VOIDCRID_FLAMEBREATH_DESC", $"<style=cDeath>Igniting.</style> <style=cIsDamage>Agile.</style> Release a burst of <style=cIsDamage>flame</style>, <style=cDeath>burning</style> enemies for <style=cIsDamage>250%</style> damage.");

            SkillDef voidBreath = ScriptableObject.CreateInstance<SkillDef>();

            voidBreath.activationState = new SerializableEntityStateType(typeof(Voidcrid.Voidcridbreath));
            voidBreath.activationStateMachineName = "Weapon";
            voidBreath.baseMaxStock = 1;
            voidBreath.baseRechargeInterval = FlamebreathOverrideRecharge.Value;
            voidBreath.beginSkillCooldownOnSkillEnd = true;
            voidBreath.canceledFromSprinting = false;
            voidBreath.cancelSprintingOnActivation = true;
            voidBreath.fullRestockOnAssign = true;
            voidBreath.interruptPriority = InterruptPriority.PrioritySkill;
            voidBreath.isCombatSkill = true;
            voidBreath.mustKeyPress = false; //test this with Backpack
            voidBreath.rechargeStock = 1;
            voidBreath.requiredStock = 1;
            voidBreath.stockToConsume = 1;
            voidBreath.icon = mainAssetBundle.LoadAsset<Sprite>("deepflame2.png");
            voidBreath.skillDescriptionToken = "VOIDCRID_FLAMEBREATH_DESC";
            voidBreath.skillName = "VOIDCRID_FLAMEBREATH";
            voidBreath.skillNameToken = "VOIDCRID_FLAMEBREATH";

            ContentAddition.AddSkillDef(voidBreath);
            ContentAddition.AddEntityState(typeof(Voidcrid.Voidcridbreath), out _);
            
            SkillFamily skillPrimary = skillLocator.primary.skillFamily;

            Array.Resize(ref skillPrimary.variants, skillPrimary.variants.Length + 1);
            skillPrimary.variants[skillPrimary.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = voidBreath,
                // unlockableDef = ,
                viewableNode = new ViewablesCatalog.Node(voidBreath.skillNameToken, false, null)
            };

        }

    //  private void InitializeDeathStates() {
    //         SkillFamily recolorFamily = Voidcrid.VoidcridSkillSetup.CreateGenericSkillWithSkillFamily(voidcridBodyPrefab, "Death_States", true).skillFamily;
    //         List<SkillDef> skilldefs = new List<SkillDef> {
    //             createDefaultDeathState("Default Death"),
    //             createVoidDeathState("Void Death"),
    //         };

    //         for (int i = 0; i < skilldefs.Count; i++) {

    //             Voidcrid.VoidcridSkillSetup.AddSkillToFamily(recolorFamily, skilldefs[i], i == 0? null : null);
    //         }

    //  }
    //           private SkillDef createDefaultDeathState(string name) {


    //         return Voidcrid.VoidcridSkillSetup.CreateSkillDef(new SkillDefInfo {
    //             skillName = name,
    //             skillNameToken ="VOIDCRID_DEFAULT_DEATH",
    //             skillDescriptionToken = "VOIDCRID_DEFAULT_DEATH_DESC",
    //             skillIcon =  mainAssetBundle.LoadAsset<Sprite>("voiddrift.png"),
    //         });
    //    }

    //     private SkillDef createVoidDeathState(string name) {

    //         return Voidcrid.VoidcridSkillSetup.CreateSkillDef(new SkillDefInfo {
    //             skillName = name,
    //             skillNameToken = "VOIDCRID_VOID_DEATH",
    //             skillDescriptionToken = "VOIDCRID_VOID_DEATH_DESC",
    //             skillIcon =  mainAssetBundle.LoadAsset<Sprite>("voiddrift.png"),
    //         });
    //    }
        
        private void NullBeamSetup(SkillLocator skillLocator)
        {

            LanguageAPI.Add("VOIDCRID_NULLBEAM", $"<style=cArtifact>「N?ll Beam』</style>");
            LanguageAPI.Add("VOIDCRID_NULLBEAM_DESC", $"<style=cArtifact>Void.</style> Draw deep from the <style=cArtifact>Void</style>, battering enemies with a swath of <style=cDeath>tentacles</style> for <style=cIsDamage>900%</style> damage.");
            SkillDef voidBeam = ScriptableObject.CreateInstance<SkillDef>();

            voidBeam.activationState = new SerializableEntityStateType(typeof(Voidcrid.NullBeam));
            voidBeam.activationStateMachineName = "Weapon";
            voidBeam.baseMaxStock = 1;
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
            voidBeam.skillName = "nullifier";
            voidBeam.skillNameToken = "VOIDCRID_NULLBEAM";
            voidBeam.mustKeyPress = true;

            ContentAddition.AddSkillDef(voidBeam);
            ContentAddition.AddEntityState(typeof(Voidcrid.NullBeam), out _);

            VoidcridUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            VoidcridUnlock.cachedName = "Skills.Croco.Nullbeam";
            VoidcridUnlock.nameToken = "ACHIEVEMENT_VOIDCRIDUNLOCK_NAME";
            VoidcridUnlock.achievementIcon = mainAssetBundle.LoadAsset<Sprite>("nullbeam2.png");
            ContentAddition.AddUnlockableDef(VoidcridUnlock);
            SkillFamily skillSecondary = skillLocator.secondary.skillFamily;

            Array.Resize(ref skillSecondary.variants, skillSecondary.variants.Length + 1);
            skillSecondary.variants[skillSecondary.variants.Length - 1] = new SkillFamily.Variant

            {
                skillDef = voidBeam,
                unlockableDef = VoidcridUnlock,
                viewableNode = new ViewablesCatalog.Node(voidBeam.skillNameToken, false, null)
            };
        }

        private void DeathBehavior() {
        
         Debug.Log("Setting up death animation...");
			CharacterDeathBehavior deathBehavior = voidcridBodyPrefab.GetComponent<CharacterDeathBehavior>();
			deathBehavior.deathState = UtilCreateSerializableAndNetRegister<Voidcrid.Modules.DeathState>();

        }
        private void VoidEscapeSetup(SkillLocator skillLocator)
        {

            EntityStateMachine esm = voidcridBodyPrefab.AddComponent<EntityStateMachine>();
            esm.customName = "Drift";
            esm.initialStateType = new SerializableEntityStateType(typeof(Idle));
            esm.mainStateType = new SerializableEntityStateType(typeof(Idle));

            NetworkStateMachine networkStateMachine = voidcridBodyPrefab.GetComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = networkStateMachine.stateMachines.Append(esm).ToArray();


            LanguageAPI.Add("VOIDCRID_VOIDDRIFT", $"<style=cArtifact>「Ethereal Dr?ft』</style>");
            LanguageAPI.Add("VOIDCRID_VOIDRIFT_DESC", $"<style=cArtifact>Void.</style> <style=cIsDamage>Stunning.</style> Slip into the <style=cArtifact>Void</style> dealing <style=cIsDamage>400% total</style> damage, with a chance to take enemies with you.");
            SkillDef voidEscape = ScriptableObject.CreateInstance<SkillDef>();
            voidEscape.activationState = new SerializableEntityStateType(typeof(Voidcrid.VoidEscape));
            voidEscape.activationStateMachineName = "Drift";
            voidEscape.baseMaxStock = 1;
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
            voidEscape.icon = mainAssetBundle.LoadAsset<Sprite>("voiddrift.png");
            voidEscape.skillDescriptionToken = "VOIDCRID_VOIDRIFT_DESC";
            voidEscape.skillName = "VOIDCRID_VOIDDRIFT";
            voidEscape.skillNameToken = "VOIDCRID_VOIDDRIFT";

            ContentAddition.AddSkillDef(voidEscape);
            ContentAddition.AddEntityState(typeof(Voidcrid.VoidEscape), out _);
            SkillFamily skillUtility = skillLocator.utility.skillFamily;

            ethUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            ethUnlock.cachedName = "Skins.Croco.Blackrid";
            ethUnlock.nameToken = "ACHIEVEMENT_RIGHTTOJAIL_NAME";
            ethUnlock.achievementIcon = mainAssetBundle.LoadAsset<Sprite>("voiddrift.png");
            ContentAddition.AddUnlockableDef(ethUnlock);

            Array.Resize(ref skillUtility.variants, skillUtility.variants.Length + 1);
            skillUtility.variants[skillUtility.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = voidEscape,
                unlockableDef = ethUnlock,
                viewableNode = new ViewablesCatalog.Node(voidEscape.skillNameToken, false, null)
            };

        }

        private void EntropySetup(SkillLocator skillLocator)
        {
            LanguageAPI.Add("VOIDCRID_ENTROPY", $"<style=cArtifact>「Entr<style=cIsHealing>?</style>py』</style>");
            LanguageAPI.Add("VOIDCRID_ENTROPY_DESC", $"<style=cArtifact>Void.</style> <style=cIsDamage>Agile.</style> <style=cIsHealing>Poisonous.</style> <style=cIsDamage>Unstable.</style> Reorganize your cells, <style=cIsHealing>healing</style> for <style=cIsDamage>{EntropySelfHeal.Value * 100}%</style> or <style=cDeath>harming</style> yourself for <style=cIsDamage>{EntropySelfDamage.Value * 100}%</style> health to damage for <style=cIsDamage>{EntropyOverrideDamage.Value}00% x 3</style> damage or <style=cIsHealing>poison</style> enemies. If held, creates a temporary <style=cArtifact>black hole</style>, <style=cDeath>choking</style> everyone inside and applies your <style=cIsHealing>Passive</style> on exiting.");
            SkillDef Entropy = ScriptableObject.CreateInstance<SkillDef>();
            Entropy.activationState = new SerializableEntityStateType(typeof(Voidcrid.Entropy));
            Entropy.activationStateMachineName = "Weapon";
            Entropy.baseMaxStock = 1;
            Entropy.baseRechargeInterval = EntropyOverrideRecharge.Value;
            Entropy.beginSkillCooldownOnSkillEnd = true;
            Entropy.canceledFromSprinting = false;
            Entropy.fullRestockOnAssign = true;
            Entropy.interruptPriority = InterruptPriority.PrioritySkill;
            Entropy.resetCooldownTimerOnUse = false;
            Entropy.isCombatSkill = true;
            Entropy.mustKeyPress = false;
            Entropy.cancelSprintingOnActivation = true;
            Entropy.rechargeStock = 1;
            Entropy.requiredStock = 1;
            Entropy.stockToConsume = 1;
            Entropy.mustKeyPress = true;
            Entropy.icon = mainAssetBundle.LoadAsset<Sprite>("entropy2.png");
            Entropy.skillDescriptionToken = "VOIDCRID_ENTROPY_DESC";
            Entropy.skillName = "VOIDCRID_ENTROPY";
            Entropy.skillNameToken = "VOIDCRID_ENTROPY";

            ContentAddition.AddSkillDef(Entropy);
            ContentAddition.AddEntityState(typeof(Voidcrid.Entropy), out _);

            entropyUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
            entropyUnlock.cachedName = "Skins.Croco.Voidcrid";
            entropyUnlock.nameToken = "ACHIEVEMENT_GRANDFATHERPARADOX_NAME";
            entropyUnlock.achievementIcon = mainAssetBundle.LoadAsset<Sprite>("entropy2.png");
            ContentAddition.AddUnlockableDef(entropyUnlock);

            SkillFamily specialSkill = skillLocator.special.skillFamily;

            Array.Resize(ref specialSkill.variants, specialSkill.variants.Length + 1);
            specialSkill.variants[specialSkill.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = Entropy,
                unlockableDef = entropyUnlock,
                viewableNode = new ViewablesCatalog.Node(Entropy.skillNameToken, false, null)
            };


        }


        private void ScepterSkillSetup()
        {


            voidScepter = ScriptableObject.CreateInstance<SkillDef>();

            LanguageAPI.Add("VOIDCRID_SCEPTER_ENTROPY", $"<style=cArtifact>「Umbral Entr<style=cIsHealing>?</style>py』</style>");
            LanguageAPI.Add("VOIDCRID__SCEPTER_ENTROPY_DESC", $"<style=cArtifact>Void.</style> <style=cIsDamage>Agile.</style> <style=cIsHealing>Poisonous.</style> <style=cIsDamage>Unstable.</style> Damage is increased to <style=cIsDamage>{ScepterEntropyOverrideDamage.Value}00% x 3</style> and if held, <style=cArtifact>ensares</style> enemies for <style=cIsDamage> 10% </style> of your health and applies your Passive.</style>");


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
            ContentAddition.AddEntityState(typeof(Voidcrid.VoidScepter), out _);

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

        // public static bool HasVoidDeath(SkillLocator sk)
        // {
           
        //         foreach (GenericSkill gs in sk.allSkills)
        //         {
        //             if (gs.skillDef.skillNameToken == "VOIDCRID_VOID_DEATH")
        //             {   
        //                 Debug.Log("Selected Void death");
        //                 hasVoidDeathEquipped = true;
        //                 break;
        //             }
        //             Debug.Log("Didn't select voiddeath");
        //         }
        //     return hasVoidDeathEquipped;
        // }

        

        internal static void CreateFogProjectile()
        {

            Debug.Log("Creating Fog projectile");
            GameObject projectile = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ElementalRingVoidBlackHole"), "fogPain");
            FogDamageController fog = projectile.AddComponent<FogDamageController>();
            BuffDef fogNotify = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdVoidFogMild.asset").WaitForCompletion();
                
                fog.healthFractionPerSecond = VoidcridDef.VoidcridFogDamageOverride.Value;
                fog.dangerBuffDuration = 0.6f;
                fog.tickPeriodSeconds = .5f;
                fog.healthFractionRampCoefficientPerSecond = .015f;
                Debug.Log("Trying fog notify");
                fog.dangerBuffDef = fogNotify;
                Debug.Log("succeeded fog notify");
                var zone = projectile.AddComponent<SphereZone>();
                zone.radius = VoidcridDef.EntropyOverrideRadius.Value + 1;
                fog.initialSafeZones = new BaseZoneBehavior[] { zone };
                zone.isInverted = true;
                projectile.AddComponent<DestroyOnTimer>().duration = 5f;
                Debug.Log("Trying static assign");
                voidFogProjectile = projectile; 
                Debug.Log("Done");

                ContentAddition.AddProjectile(voidFogProjectile);
                Debug.Log("Finished creating voidFog projectile");
        }


            private static void JailJailJail(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info) {
            
                
                if (info.HasModdedDamageType(DamageTypes.nullBeamJail) || info.HasModdedDamageType(DamageTypes.ethJail) || info.HasModdedDamageType(DamageTypes.entropyJail))
            {
                if (NetworkServer.active) {
                self.body.AddTimedBuff(RoR2Content.Buffs.Nullified, 3f);
                }
            }

            orig(self, info);

		}

        	private static void VoidcridDeathBombFake(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			if (damageInfo.rejected || VoidcridBombDeath.Value == false) {
				originalMethod(@this, damageInfo);
				return;
			}

			originalMethod(@this, damageInfo);
			if (damageInfo.HasModdedDamageType(DamageTypes.voidcridDeath)) {
				if (!@this.alive && @this.wasAlive && @this.body) {
					Vector3 pos = @this.body.corePosition;
					float radius = @this.body.bestFitRadius;

					if (damageInfo.attacker) {
						CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
						if (attacker != null) {
							EffectManager.SpawnEffect(
								EffectProvider.VoidcridSilentKill,
								new EffectData {
									origin = pos,
									scale = radius
								},
								true
							);
						}
					}
				}
			}
		}

               private void Hook()
        {
        
                      On.RoR2.HealthComponent.TakeDamage += JailJailJail;

                      On.RoR2.HealthComponent.TakeDamage += VoidcridDeathBombFake;

        }

        		private static SerializableEntityStateType UtilCreateSerializableAndNetRegister<T>() where T : EntityState {

			Debug.Log($"Registering EntityState {typeof(T).FullName} and returning a new instance of {nameof(SerializableEntityStateType)} of that type...");
			ContentAddition.AddEntityState<T>(out _);
			return new SerializableEntityStateType(typeof(T));
		}



        internal static void LoadAssetBundle()
        {


            try
            {

                mainAssetBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("Voidcrid.dll", "acrid3"));

                Debug.Log("Loaded assetbundle");


            }
            catch (Exception e)
            {
                Debug.Log("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e);
                return;
            }

        }



        private void Start()
        {

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.cwmlolzlz.skills"))
            {
                skillsPlusInstalled = true;

                SkillsPlusCompat.init();
                Debug.Log("Init for SkillsCompat");


            }

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"))
            {
                ancientScepterInstalled = true;
                ScepterSkillSetup();
                ScepterSetup();
            }

        }


    }

}





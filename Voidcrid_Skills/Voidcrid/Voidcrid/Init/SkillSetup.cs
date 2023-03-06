using System;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Voidcrid
{

    public class SkillSetup
    {
        private static UnlockableDef entropyUnlock;
        public static GameObject voidFrogProjectile;
        private static UnlockableDef ethUnlock;

        public static GameObject voidBreath;

        private static UnlockableDef VoidcridUnlock;

        internal static AssetBundle mainAssetBundle;
        internal static GameObject voidcridBodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();
        
        SkillLocator skillLocator = voidcridBodyPrefab.GetComponent<SkillLocator>();
        
        private const string assetbundleName = "acrid3";
        private const string csProjName = "Voidcrid";
        // public static string characterOutro = "..and so it left, a shell of its former self.";
        // public static string characterOutroFailure = "..and so it stayed, forever chained to the Abyss.";
        public static void SetupSkills(SkillLocator skillLocator)
        {

        

            FlamebreathSetup(skillLocator);
            NullBeamSetup(skillLocator);
            VoidEscapeSetup(skillLocator);
            EntropySetup(skillLocator);
            // ManipulateFirebreathColor();
            VoidcridPassive(skillLocator);


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

        private static void FlamebreathSetup(SkillLocator skillLocator)
        {

            SkillDef voidBreath = ScriptableObject.CreateInstance<SkillDef>();

            voidBreath.activationState = new SerializableEntityStateType(typeof(Voidcrid.Skills.Voidcridbreath));
            voidBreath.activationStateMachineName = "Weapon";
            voidBreath.baseMaxStock = 1;
            voidBreath.baseRechargeInterval = Voidcrid.VoidcridDef.FlamebreathOverrideRecharge.Value;
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
            ContentAddition.AddEntityState(typeof(Voidcrid.Skills.Voidcridbreath), out _);

            SkillFamily skillPrimary = skillLocator.primary.skillFamily;

            Array.Resize(ref skillPrimary.variants, skillPrimary.variants.Length + 1);
            skillPrimary.variants[skillPrimary.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = voidBreath,
                // unlockableDef = ,
                viewableNode = new ViewablesCatalog.Node(voidBreath.skillNameToken, false, null)
            };

        }

        private static void NullBeamSetup(SkillLocator skillLocator)
        {

            SkillDef voidBeam = ScriptableObject.CreateInstance<SkillDef>();

            voidBeam.activationState = new SerializableEntityStateType(typeof(Voidcrid.Skills.NullBeam));
            voidBeam.activationStateMachineName = "Weapon";
            voidBeam.baseMaxStock = 1;
            voidBeam.baseRechargeInterval = Voidcrid.VoidcridDef.NullBeamOverrideRecharge.Value;
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
            ContentAddition.AddEntityState(typeof(Voidcrid.Skills.NullBeam), out _);

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

        private static void VoidEscapeSetup(SkillLocator skillLocator)
        {

            EntityStateMachine esm = voidcridBodyPrefab.AddComponent<EntityStateMachine>();
            esm.customName = "Drift";
            esm.initialStateType = new SerializableEntityStateType(typeof(Idle));
            esm.mainStateType = new SerializableEntityStateType(typeof(Idle));

            //Make a new machine so users can "punch through" Drift with another ability
            NetworkStateMachine networkStateMachine = voidcridBodyPrefab.GetComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = networkStateMachine.stateMachines.Append(esm).ToArray();


            SkillDef voidEscape = ScriptableObject.CreateInstance<SkillDef>();
            voidEscape.activationState = new SerializableEntityStateType(typeof(Voidcrid.Skills.VoidEscape));
            voidEscape.activationStateMachineName = "Drift";
            voidEscape.baseMaxStock = 1;
            voidEscape.baseRechargeInterval = Voidcrid.VoidcridDef.EtherealDriftOverrideRecharge.Value;
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
            ContentAddition.AddEntityState(typeof(Voidcrid.Skills.VoidEscape), out _);
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

        private static void EntropySetup(SkillLocator skillLocator)
        {
            SkillDef Entropy = ScriptableObject.CreateInstance<SkillDef>();
            Entropy.activationState = new SerializableEntityStateType(typeof(Voidcrid.Skills.Entropy));
            Entropy.activationStateMachineName = "Weapon";
            Entropy.baseMaxStock = 1;
            Entropy.baseRechargeInterval = Voidcrid.VoidcridDef.EntropyOverrideRecharge.Value;
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
            ContentAddition.AddEntityState(typeof(Voidcrid.Skills.Entropy), out _);

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

        

        public static void VoidcridPassive(SkillLocator skillLocator)
        {

            Debug.Log("Starting passive setup");
            if (Voidcrid.VoidcridDef.VoidcridPassiveShow.Value == true)
            {
                SurvivorDef voidcridOutro = voidcridBodyPrefab.GetComponent<SurvivorDef>();
            Debug.Log("Get Voidcrid Def");

                skillLocator.passiveSkill.enabled = true;
                skillLocator.passiveSkill.skillNameToken = "VOIDCRID_PASSIVE";
                skillLocator.passiveSkill.skillDescriptionToken = "VOIDCRID_PASSIVE_DESC";
                skillLocator.passiveSkill.icon = mainAssetBundle.LoadAsset<Sprite>("icon.png");
            Debug.Log("Load language");

                LanguageAPI.Add("CROCO_OUTRO_FLAVOR", "..and so it left, a shell of its former self.");
                LanguageAPI.Add("CROCO_MAIN_ENDING_ESCAPE_FAILURE_FLAVOR", "..and so it stayed, forever chained to the Abyss.");

            Debug.Log("Finished overriding ending");
            // Debug.Log("Add vars");

            //     voidcridOutro.outroFlavorToken = characterOutro;
            //     voidcridOutro.mainEndingEscapeFailureFlavorToken = characterOutroFailure;

            }
            else
            {

                skillLocator.passiveSkill.enabled = false;
            }

        }

        public static void DeathBehavior()
        {

            Debug.Log("Setting up death animation...");
            CharacterDeathBehavior deathBehavior = voidcridBodyPrefab.GetComponent<CharacterDeathBehavior>();
            deathBehavior.deathState = UtilCreateSerializableAndNetRegister<Voidcrid.Modules.DeathState>();

        }

        private static SerializableEntityStateType UtilCreateSerializableAndNetRegister<T>() where T : EntityState
        {

            Debug.Log($"Registering EntityState {typeof(T).FullName} and returning a new instance of {nameof(SerializableEntityStateType)} of that type...");
            ContentAddition.AddEntityState<T>(out _);
            return new SerializableEntityStateType(typeof(T));
        }


        internal static void ManipulateFirebreathColor()
        {
            GameObject flamePrefab = EntityStates.LemurianBruiserMonster.Flamebreath.flamethrowerEffectPrefab;

            GameObject colorizer = R2API.PrefabAPI.InstantiateClone(flamePrefab, "voidcridFirePurple");
            Material blarg;
            blarg = colorizer.GetComponent<Material>();
            blarg.SetColor("_COLOR", Color.magenta);
            voidBreath = colorizer;
            ContentAddition.AddProjectile(voidBreath);

        }

        internal static void CreateFogProjectile()
        {

            Debug.Log("Creating Fog projectile");
            GameObject projectile = R2API.PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ElementalRingVoidBlackHole"), "fogPain");
            FogDamageController fog = projectile.AddComponent<FogDamageController>();
            BuffDef fogNotify = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdVoidFogMild.asset").WaitForCompletion();

            fog.healthFractionPerSecond = Voidcrid.VoidcridDef.VoidcridFogDamageOverride.Value;
            fog.dangerBuffDuration = 0.6f;
            fog.tickPeriodSeconds = .5f;
            fog.healthFractionRampCoefficientPerSecond = .015f;
            fog.dangerBuffDef = fogNotify;
            var zone = projectile.AddComponent<SphereZone>();
            zone.radius = Voidcrid.VoidcridDef.EntropyOverrideRadius.Value + 1;
            fog.initialSafeZones = new BaseZoneBehavior[] { zone };
            zone.isInverted = true;
            projectile.AddComponent<DestroyOnTimer>().duration = 5f;
            voidFrogProjectile = projectile;

            ContentAddition.AddProjectile(voidFrogProjectile);
            Debug.Log("Finished creating voidFog projectile");
        }

    }
}
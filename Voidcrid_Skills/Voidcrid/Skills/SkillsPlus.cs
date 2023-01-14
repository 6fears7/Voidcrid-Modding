using SkillsPlusPlus;
using SkillsPlusPlus.Modifiers;
using RoR2;
using RoR2.Skills;
using R2API;
using UnityEngine;


namespace Voidcrid
{

    public class SkillsPlusCompat {
        public static float nullBeamDmg;
         public static void init() {

            nullBeamDmg = NullBeam.nullBeamDamage;

            doLanguage();
            Debug.Log("Added lang");
            SkillModifierManager.LoadSkillModifiers();
            Debug.Log("Added skill modifiers");
        }
        private static void doLanguage() {
            LanguageAPI.Add("VOIDCRID_NULLBEAM_UPGRADE_DESCRIPTION", "<style=cIsUtility>+5%</style> damage, <style=cIsUtility>+10%</style> bullet thickness, and <style=cIsUtility>+0.1</style> bullet spread");
        }
    }
    [SkillLevelModifier("VOIDCRID_NULLBEAM", typeof(NullBeam))]
    public class NullbeamModifier : SimpleSkillModifier<NullBeam> {

        public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef) {
            base.OnSkillLeveledUp(level, characterBody, skillDef);

            NullBeam.nullBeamDamage = MultScaling(SkillsPlusCompat.nullBeamDmg, .05f, level);
    
        }
    }
}
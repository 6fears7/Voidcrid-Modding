using R2API;

namespace Voidcrid.Language
{

    public class LanguageSetup
    {

        public static void SetLanguage()
        {
            LanguageAPI.Add("VOIDCRID_PASSIVE", "<style=cArtifact>Void</style>crid");
            LanguageAPI.Add("VOIDCRID_PASSIVE_DESC", "All <style=cArtifact>Void</style> attacks have a chance to <style=cArtifact>jail</style> enemies (and apply <style=cWorldEvent>Deeprot</style>, if selected).");


            LanguageAPI.Add("ACHIEVEMENT_GRANDFATHERPARADOX_NAME", "Acrid: Grandfather Paradox");
            LanguageAPI.Add("ACHIEVEMENT_GRANDFATHERPARADOX_DESCRIPTION", "An unexpected amphibian, an unfortunate end.");

            LanguageAPI.Add("ACHIEVEMENT_RIGHTTOJAIL_NAME", "Acrid: Right To Jail");
            LanguageAPI.Add("ACHIEVEMENT_RIGHTTOJAIL_DESCRIPTION", "As Acrid, jail a Jailer.");

            LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_NAME", "...Left alone");
            LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_DESCRIPTION", "As Acrid, corrupt yourself 7 times to break containment.");

            LanguageAPI.Add("VOIDCRID_PASSIVE", "<style=cArtifact>Void</style>crid");
            LanguageAPI.Add("VOIDCRID_PASSIVE_DESC", "All <style=cArtifact>Void</style> attacks have a chance to <style=cArtifact>jail</style> enemies (and apply <style=cWorldEvent>Deeprot</style>, if selected).");


            LanguageAPI.Add("ACHIEVEMENT_GRANDFATHERPARADOX_NAME", "Acrid: Grandfather Paradox");
            LanguageAPI.Add("ACHIEVEMENT_GRANDFATHERPARADOX_DESCRIPTION", "An unexpected amphibian, an unfortunate end.");

            LanguageAPI.Add("ACHIEVEMENT_RIGHTTOJAIL_NAME", "Acrid: Right To Jail");
            LanguageAPI.Add("ACHIEVEMENT_RIGHTTOJAIL_DESCRIPTION", "As Acrid, jail a Jailer.");

            LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_NAME", "...Left alone");
            LanguageAPI.Add("ACHIEVEMENT_VOIDCRIDUNLOCK_DESCRIPTION", "As Acrid, corrupt yourself 7 times to break containment.");

            LanguageAPI.Add("VOIDCRID_FLAMEBREATH", "Flamebreath");
            LanguageAPI.Add("VOIDCRID_FLAMEBREATH_DESC", $"<style=cDeath>Igniting.</style> <style=cIsDamage>Agile.</style> Release a burst of <style=cIsDamage>flame</style>, <style=cDeath>burning</style> enemies for <style=cIsDamage>250%</style> damage.");

            LanguageAPI.Add("VOIDCRID_NULLBEAM", $"<style=cArtifact>「N?ll Beam』</style>");
            LanguageAPI.Add("VOIDCRID_NULLBEAM_DESC", $"<style=cArtifact>Void.</style> Draw deep from the <style=cArtifact>Void</style>, battering enemies with a swath of <style=cDeath>tentacles</style> for <style=cIsDamage>900%</style> damage.");

            LanguageAPI.Add("VOIDCRID_VOIDDRIFT", $"<style=cArtifact>「Ethereal Dr?ft』</style>");
            LanguageAPI.Add("VOIDCRID_VOIDRIFT_DESC", $"<style=cArtifact>Void.</style> <style=cIsDamage>Stunning.</style> Slip into the <style=cArtifact>Void</style> dealing <style=cIsDamage>400% total</style> damage, with a chance to take enemies with you.");

            LanguageAPI.Add("VOIDCRID_ENTROPY", $"<style=cArtifact>「Entr<style=cIsHealing>?</style>py』</style>");
            LanguageAPI.Add("VOIDCRID_ENTROPY_DESC", $"<style=cArtifact>Void.</style> <style=cIsDamage>Agile.</style> <style=cIsHealing>Poisonous.</style> <style=cIsDamage>Unstable.</style> Reorganize your cells, <style=cIsHealing>healing</style> for <style=cIsDamage>{Voidcrid.VoidcridDef.EntropySelfHeal.Value * 100}%</style> or <style=cDeath>harming</style> yourself for <style=cIsDamage>{Voidcrid.VoidcridDef.EntropySelfDamage.Value * 100}%</style> health to damage for <style=cIsDamage>{Voidcrid.VoidcridDef.EntropyOverrideDamage.Value}00% x 3</style> damage or <style=cIsHealing>poison</style> enemies. If held, creates a temporary <style=cArtifact>black hole</style>, <style=cDeath>choking</style> everyone inside and applies your <style=cIsHealing>Passive</style> on exiting.");
            
        }
    }


}
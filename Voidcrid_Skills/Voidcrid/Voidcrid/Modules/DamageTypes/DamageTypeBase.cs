using R2API;


namespace Voidcrid.Modules
{
    //Declare damage types; effects are created with hooks in VoidcridDef.cs
    public class DamageTypes
    {

        //on hit apply Jailer tether
        public static DamageAPI.ModdedDamageType nullBeamJail = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType ethJail = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType entropyJail = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType voidcridDeath = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType voidcridDeath2 = DamageAPI.ReserveDamageType();

        public static DamageAPI.ModdedDamageType voidcridPoison = DamageAPI.ReserveDamageType();


    }
}
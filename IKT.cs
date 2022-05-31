using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using IKTweaks;


namespace SeatMod
{
    internal class IKT
    {

        public static void SetIKTweaksDisableAnim()
        {
            if (MelonHandler.Mods.Any(m => m.Info.Name == "IKTweaks"))
            {
                MelonPreferences.SetEntryValue<IgnoreAnimationsMode>("IkTweaks", "IgnoreAnimationsMode", IgnoreAnimationsMode.All);
                MelonPreferences.Save();
            }
        }

        public static string IKTweaksAnimMode()
        {
            if (MelonHandler.Mods.Any(m => m.Info.Name == "IKTweaks"))
            { //None - "Play all animations"
              //Head - "Ignore head animations"
              //Hands - "Ignore hands animations"
              //HandAndHead - "Ignore head and hands"
              //All - "Ignore all (always slide around)
                return MelonPreferences.GetEntryValue<IgnoreAnimationsMode>("IkTweaks", "IgnoreAnimationsMode").ToString();
            }
            //else Main.Logger.Msg("IKTweaks is missing");
            return "N/A";
        }


    }
}

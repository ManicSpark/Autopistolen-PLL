using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace WeaponsPlus
{
    [HarmonyPatch(typeof(PLPulseGrenadeInstance))]
    [HarmonyPatch("Start")]
    class PulseGrenadeInstance
    {
        static void Postfix(PLPulseGrenadeInstance __instance, PLPawnItem_LauncherBase ___MyLauncher)
        {
            __instance.DamageRadius = 7f + ___MyLauncher.Level;
        }
    }
}

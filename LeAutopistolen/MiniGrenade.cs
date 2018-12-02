using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeaponsPlus
{
    [HarmonyPatch(typeof(PLMiniGrenadeInstance))]
    [HarmonyPatch("Start")]
    class MiniGrenadeInstance
    {
        static void Postfix(PLMiniGrenadeInstance __instance, PLPawnItem_LauncherBase ___MyLauncher)
        {
            __instance.DamageRadius = 7f + ___MyLauncher.Level;
        }
    }
}

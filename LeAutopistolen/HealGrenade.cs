using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeaponsPlus
{
    [HarmonyPatch(typeof(PLHealGrenadeInstance))]
    [HarmonyPatch("Start")]
    class HealGrenadeInstance
    {
        static void Postfix(PLHealGrenadeInstance __instance, PLPawnItem_LauncherBase ___MyLauncher)
        {
            __instance.HealRadius = 7f + ___MyLauncher.Level;
        }
    }
}

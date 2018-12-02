using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;

namespace WeaponsPlus
{
    [HarmonyPatch(typeof(PLPawnItem_BurstPistol))]
    [HarmonyPatch("InternalFireShot")]
    class Burst
    {
        static bool Prefix(PLPawnItem_BurstPistol __instance, PLPawn ___MySetupPawn)
        {
            MethodInfo BurstShot = AccessTools.Method(__instance.GetType(), "BurstShot", null, null);

            ___MySetupPawn.StartCoroutine((IEnumerator)BurstShot.Invoke(__instance, new object[] { 3 + __instance.Level, 0.225f / (3 + __instance.Level) }));
            return false;
        }
    }
}

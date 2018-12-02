using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WeaponsPlus
{
    [HarmonyPatch(typeof(PLPawnItem_HandShotgun))]
    [HarmonyPatch("InternalFireShot")]
    class Cannon
    {
        static bool Prefix(PLPawnItem_HandShotgun __instance)
        {
            PLPawn plpawn = (PLPawn)AccessTools.Field(__instance.GetType(), "MySetupPawn").GetValue(__instance);
            MethodInfo methodInfo = AccessTools.Method(__instance.GetType(), "InternalFireSingleShot", null, null);
            float currentAccuracyRating = plpawn.CurrentAccuracyRating;
            if (__instance.AmmoCurrent > 0)
            {
                __instance.AmmoCurrent--;
                for (int i = 0; i < __instance.Level + 3; i++)
                {
                    methodInfo.Invoke(__instance, null);
                    plpawn.CurrentAccuracyRating = currentAccuracyRating;
                }
                methodInfo.Invoke(__instance, null);
            }
            return false;
        }
    }
}

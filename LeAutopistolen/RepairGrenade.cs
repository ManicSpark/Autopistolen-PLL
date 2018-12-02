using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeaponsPlus
{
    [HarmonyPatch(typeof(PLRepairGrenadeInstance))]
    [HarmonyPatch("Update")]
    class RepairGrenadeHeal
    {
        static void Postfix(PLRepairGrenadeInstance __instance, PLPawnItem_LauncherBase ___MyLauncher)
        {
            foreach (PLPawn plpawn in PLGameStatic.Instance.AllPawns)
            {
                if (plpawn != null && plpawn.MyCurrentTLI == __instance.MyTLI && !plpawn.IsDead && !plpawn.PreviewPawn && plpawn.GetPlayer() != null && plpawn.GetPlayer().RaceID == 2 && plpawn.Health < plpawn.MaxHealth)
                {
                    float sqrMagnitude = (plpawn._transform.position - __instance.transform.position).sqrMagnitude;
                    if (sqrMagnitude < 25f)
                    {
                        plpawn.Health += ___MyLauncher.CalcGrenadePower() * Time.deltaTime * 0.1f;
                        plpawn.Health = Mathf.Clamp(plpawn.Health, 0f, plpawn.MaxHealth);
                        plpawn.LastRepairActiveTime = Time.time;
                    }
                }
            }
        }
    }
}

using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WeaponsPlus
{
    [HarmonyPatch(typeof(PLPawnItem_Gun))]
    [HarmonyPatch("Setup")]
    class BackupGunFix
    {
        static void Postfix(PLPawnItem_Gun __instance, PLPawn ___MySetupPawn)
        {
            if (___MySetupPawn != null)
            {
                switch (__instance.PawnItemType)
                {
                    case EPawnItemType.E_PHASEPISTOL:
                        ((PLPawnItem_PhasePistol)__instance).GetType().GetField("MinAutoFireDelay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_PhasePistol)__instance, 0.13f - (0.001f * __instance.Level));
                        __instance.HeatMax = 1f + (0.01f * __instance.Level) ;
                        break;
                    case EPawnItemType.E_PIERCING_BEAM_PISTOL:
                        ((PLPawnItem_PierceLaserPistol)__instance).GetType().GetField("MinAutoFireDelay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_PierceLaserPistol)__instance, 0.38f - (0.01f * __instance.Level));
                        __instance.HeatMax = 1f + (0.1f * __instance.Level);
                        break;
                    case EPawnItemType.E_SMUGGLERS_PISTOL:
                        ((PLPawnItem_SmugglersPistol)__instance).GetType().GetField("MinAutoFireDelay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_SmugglersPistol)__instance, 0.2f - (0.01f * __instance.Level));
                        __instance.HeatMax = 1f + (0.1f * __instance.Level);
                        break;
                    case EPawnItemType.E_RANGER:
                        ((PLPawnItem_Ranger)__instance).GetType().GetField("MinAutoFireDelay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_Ranger)__instance, 1f - (0.01f * __instance.Level));
                        ((PLPawnItem_Ranger)__instance).GetType().GetField("Accuracy", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_Ranger)__instance, 2f + (0.2f * __instance.Level));
                        __instance.HeatMax = 1f + (0.1f * __instance.Level);
                        break;
                    case EPawnItemType.E_HEAVY_PISTOL:
                        ((PLPawnItem_HeavyPistol)__instance).GetType().GetField("MinAutoFireDelay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_HeavyPistol)__instance, 0.3f - (0.01f * __instance.Level));
                        ((PLPawnItem_HeavyPistol)__instance).GetType().GetField("Accuracy", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_HeavyPistol)__instance, 0.7f + (0.05f * __instance.Level));
                        __instance.HeatMax = 1f + (0.1f * __instance.Level);
                        break;
                    case EPawnItemType.E_HAND_SHOTGUN:
                        ((PLPawnItem_HandShotgun)__instance).GetType().GetField("MinAutoFireDelay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_HandShotgun)__instance, 0.5f - (0.01f * __instance.Level));
                        __instance.HeatMax = 1f + (0.2f * __instance.Level);
                        break;
                    case EPawnItemType.E_BURST_PISTOL:
                        ((PLPawnItem_BurstPistol)__instance).GetType().GetField("MinAutoFireDelay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_BurstPistol)__instance, 0.4f - (0.01f * __instance.Level));
                        __instance.HeatMax = 1f + (0.2f * __instance.Level);
                        break;
                    case EPawnItemType.E_PULSE_GRENADE:
                    case EPawnItemType.E_HEAL_GRENADE:
                    case EPawnItemType.E_ANTIFIRE_GRENADE:
                    case EPawnItemType.E_REPAIR_GRENADE:
                        __instance.AmmoMax = 9 + (1 * __instance.Level);
                        __instance.AmmoCurrent = __instance.AmmoMax;
                        break;
                    case EPawnItemType.E_MINI_GRENADE:
                        __instance.AmmoMax = 40 + (8 * __instance.Level);
                        __instance.AmmoCurrent = __instance.AmmoMax;
                        ((PLPawnItem_MiniGrenade)__instance).GetType().GetField("FireDelay", BindingFlags.Instance | BindingFlags.NonPublic).SetValue((PLPawnItem_MiniGrenade)__instance, 0.2f - (0.01f * __instance.Level));
                        break;
                    case EPawnItemType.E_HELD_BEAM_PISTOL:
                        __instance.HeatMax = 1.1f + (0.1f * __instance.Level);
                        break;
                    case EPawnItemType.E_HELD_BEAM_PISTOL_W_HEALING:
                        __instance.HeatMax = 1.1f + (0.1f * __instance.Level);
                        break;
                    case EPawnItemType.E_STUN_GRENADE:
                        __instance.AmmoMax = 4 + (1 * __instance.Level);
                        __instance.AmmoCurrent = __instance.AmmoMax;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

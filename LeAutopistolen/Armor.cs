using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace Pistolen
{
    [HarmonyPatch(typeof(PLServerClassInfo))]
    [HarmonyPatch("Update")]
    class Armor
    {
        static void Prefix(PLServerClassInfo __instance, bool ___CreatedStartingItems, int ___m_ClassID)
        {
            if (PhotonNetwork.isMasterClient && !___CreatedStartingItems && ___m_ClassID != -1 && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null)
            {
                if (___m_ClassID == 3)
                {
                    __instance.ClassLockerInventory.UpdateItem(PLServer.Instance.PawnInvItemIDCounter++, 13, 0, 0, -1);
                }
            }
        }
    }
}

using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

namespace WeaponsPlus
{
    [HarmonyPatch(typeof(PLPawnItem_PhasePistol))]
    [HarmonyPatch("OnActive")]
    class Heat
    {
        static bool Prefix(PLPawnItem_PhasePistol __instance, PLPawn ___MySetupPawn, PLPawnItemInstance ___MyGunInstance, EAimFireState ___AimFireState,ref bool ___hasPlayedOverheat, ref float ___LastFireTime, ref float ___MinAutoFireDelay)
        {
            if (___MyGunInstance != null && !___MyGunInstance.gameObject.activeSelf)
            {
                ___MyGunInstance.gameObject.SetActive(true);
            }
            if (___MySetupPawn != null && ___MySetupPawn.GetPlayer() != null)
            {
                ___AimFireState = EAimFireState.NEUTRAL;
                foreach (PLPawn plpawn in PLGameStatic.Instance.AllPawns)
                {
                    float num;
                    if (!XRSettings.enabled || ___MySetupPawn != PLNetworkManager.Instance.MyLocalPawn || ___MyGunInstance == null)
                    {
                        num = PLGlobal.ClosestSqDistBetweenTwoSegments(___MySetupPawn.PawnCamera.position, ___MySetupPawn.PawnCamera.position + ___MySetupPawn.PawnCamera.forward * 500f, plpawn.transform.position, plpawn.transform.position + plpawn.transform.up * 1.2f);
                    }
                    else
                    {
                        num = PLGlobal.ClosestSqDistBetweenTwoSegments(___MyGunInstance.ProjSpawn.position, ___MyGunInstance.ProjSpawn.position + ___MyGunInstance.ProjSpawn.forward * 500f, plpawn.transform.position, plpawn.transform.position + plpawn.transform.up * 1.2f);
                    }
                    if (plpawn != ___MySetupPawn && plpawn != null && num < 0.3f && !plpawn.IsDead && plpawn.HadRecentLOSSuccessToTarget(___MySetupPawn) && plpawn.MyPlayer != null && plpawn.MyPlayer.TeamID != ___MySetupPawn.MyPlayer.TeamID)
                    {
                        ___AimFireState = EAimFireState.ENEMY;
                    }
                }
                if (___MySetupPawn.MyActiveScreen != null && ___AimFireState != EAimFireState.ENEMY)
                {
                    ___AimFireState = EAimFireState.FRIENDLY;
                }
                bool flag = (!PLInput.Instance.GetButton(PLInput.EInputActionName.unlock_mouse) || ___MySetupPawn.ShowJetpackVisuals) && !___MySetupPawn.IsDead && ___MySetupPawn == PLNetworkManager.Instance.MyLocalPawn && !___MySetupPawn.IsSprinting && ___AimFireState != EAimFireState.FRIENDLY && !PLNetworkManager.Instance.MainMenu.IsActive() && PLNetworkManager.Instance.MainMenu.GetActiveMenuCount() == 0 && PLCameraSystem.Instance != null && PLCameraSystem.Instance.GetModeString() == "LocalPawn" && PLTabMenu.Instance != null && !PLTabMenu.Instance.TabMenuActive && PLTabMenu.Instance.DialogueMenu != null && PLTabMenu.Instance.DialogueMenu.CurrentActorInstance == null && PLTabMenu.Instance.TargetContainer == null && !PLTabMenu.Instance.IsDisplayingOrderMenu();
                bool flag2 = PLInput.Instance.GetButtonDown(PLInput.EInputActionName.fire) && flag;
                bool flag3 = PLInput.Instance.GetButton(PLInput.EInputActionName.fire) && flag;
                flag = flag3;
                if (___MySetupPawn.MyPlayer.IsBot)
                {
                    flag = false;
                    PLBotController plbotController = ___MySetupPawn.MyController as PLBotController;
                    if (plbotController != null)
                    {
                        flag = (!___MySetupPawn.IsDead && plbotController.AI_ItemUtilityRequest == EItemUtilityType.E_DAMAGE && plbotController.AI_ShouldUseActiveItem && PhotonNetwork.isMasterClient && !___MySetupPawn.IsWeaponStunned());
                        if (__instance.UsesHeat && __instance.Heat > __instance.HeatMax)
                        {
                            flag = false;
                        }
                    }
                    if (__instance.UsesAmmo && __instance.AmmoCurrent < __instance.AmmoMax)
                    {
                        __instance.AmmoCurrent++;
                    }
                }
                if (__instance.IsOverheated)
                {
                    if (!___hasPlayedOverheat)
                    {
                        PLMusic.PostEvent("play_sx_player_item_phasepistol_overheat", ___MySetupPawn.gameObject);
                        ___hasPlayedOverheat = true;
                    }
                    if (__instance.Heat <= 0f)
                    {
                        __instance.IsOverheated = false;
                        ___hasPlayedOverheat = false;
                    }
                }
                else if (__instance.Heat > __instance.HeatMax + 0.15f)
                {
                    __instance.IsOverheated = true;
                }
                MethodInfo updateoverheatps = AccessTools.Method(__instance.GetType(), "UpdateOverheatPS", null, null);
                updateoverheatps.Invoke(__instance, null);
                if (flag && !__instance.IsOverheated && (!__instance.UsesAmmo || __instance.AmmoCurrent > 0) && (flag2 || Time.time - ___LastFireTime > ___MinAutoFireDelay))
                {
                    ___LastFireTime = Time.time;
                    MethodInfo internalfireshot = AccessTools.Method(__instance.GetType(), "InternalFireShot", null, null);
                    internalfireshot.Invoke(__instance, null);
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(PLPawnItem_HeldBeamPistol))]
    [HarmonyPatch("OnUpdate")]
    class BeamHeat
    {
        static bool Prefix(PLPawnItem_HeldBeamPistol __instance, PLPawn ___MySetupPawn, PLPawnItemInstance ___MyGunInstance, EAimFireState ___AimFireState, ref bool ___hasPlayedOverheat, Transform[] ___gunChildTransforms, PLPawnInventory ___MyInventory, PLAmbientSFXControl ___FiringSFX, float ___ChargeAmount, float ___LastBeamAttackUpdateTime)
        {
            float num = 1f;
            PLPlayer cachedFriendlyPlayerOfClass = PLServer.Instance.GetCachedFriendlyPlayerOfClass(3);
            if (___MySetupPawn != null && ___MySetupPawn.GetPlayer() != null && ___MySetupPawn.GetPlayer().TeamID == 0 && cachedFriendlyPlayerOfClass != null)
            {
                num = 1f + (float)cachedFriendlyPlayerOfClass.Talents[23] * 0.32f;
            }
            foreach (PawnStatusEffect pawnStatusEffect in ___MySetupPawn.MyStatusEffects)
            {
                if (pawnStatusEffect != null && pawnStatusEffect.Type == EPawnStatusEffectType.GUN_COOLING)
                {
                    num += 0.4f * pawnStatusEffect.Strength;
                }
            }
            __instance.Heat -= Time.deltaTime * (0.2f * num);
            __instance.Heat = Mathf.Clamp(__instance.Heat, 0f, float.MaxValue);
            if (___MySetupPawn == null)
            {
                return false;
            }
            if (___MyGunInstance != null)
            {
                if (___MyGunInstance.transform.parent != __instance.GetTargetParentTransform())
                {
                    ___MyGunInstance.transform.parent = __instance.GetTargetParentTransform();
                    ___MyGunInstance.transform.localPosition = Vector3.zero;
                    ___MyGunInstance.transform.localRotation = Quaternion.identity;
                }
                if (UnityEngine.Random.Range(0, 1000) == 0)
                {
                    ___MyGunInstance.transform.localPosition = Vector3.zero;
                    ___MyGunInstance.transform.localRotation = Quaternion.identity;
                }
            }
            if (___MyGunInstance != null)
            {
                if (___gunChildTransforms == null || ___gunChildTransforms.Length == 0 || ___gunChildTransforms[0] == null)
                {
                    ___gunChildTransforms = ___MyGunInstance.gameObject.GetComponentsInChildren<Transform>();
                }
                foreach (Transform transform in ___gunChildTransforms)
                {
                    if (transform != null)
                    {
                        transform.gameObject.layer = ___MySetupPawn.gameObject.layer;
                    }
                }
            }

            if (___MySetupPawn != null && ___MySetupPawn.GetPlayer() != null && ___MyGunInstance != null && (___MySetupPawn.GetPlayer().GetPlayerID() == PLNetworkManager.Instance.LocalPlayerID || (___MySetupPawn.GetPlayer().IsBot && PhotonNetwork.isMasterClient)))

            {
                ___AimFireState = EAimFireState.NEUTRAL;
                bool flag = (!PLInput.Instance.GetButton(PLInput.EInputActionName.unlock_mouse) || ___MySetupPawn.ShowJetpackVisuals) && ___MySetupPawn.GetPlayer().GetPlayerID() == PLNetworkManager.Instance.LocalPlayerID && ___MyInventory.ActiveItem == __instance && !___MySetupPawn.IsDead && ___MySetupPawn == PLNetworkManager.Instance.MyLocalPawn && !___MySetupPawn.IsSprinting && ___AimFireState != EAimFireState.FRIENDLY && !PLNetworkManager.Instance.MainMenu.IsActive() && PLNetworkManager.Instance.MainMenu.GetActiveMenuCount() == 0 && PLCameraSystem.Instance != null && PLCameraSystem.Instance.GetModeString() == "LocalPawn" && PLTabMenu.Instance != null && !PLTabMenu.Instance.TabMenuActive && PLTabMenu.Instance.DialogueMenu != null && PLTabMenu.Instance.DialogueMenu.CurrentActorInstance == null && PLTabMenu.Instance.TargetContainer == null && !PLTabMenu.Instance.IsDisplayingOrderMenu();
                bool flag2 = PLInput.Instance.GetButtonDown(PLInput.EInputActionName.fire) && flag;
                bool flag3 = PLInput.Instance.GetButton(PLInput.EInputActionName.fire) && flag;
                flag = flag3;
                if (___MySetupPawn.MyPlayer.IsBot)
                {
                    flag = false;
                    PLBotController plbotController = ___MySetupPawn.MyController as PLBotController;
                    if (plbotController != null)
                    {
                        flag = (!___MySetupPawn.IsDead && plbotController.AI_ItemUtilityRequest == EItemUtilityType.E_DAMAGE && plbotController.AI_ShouldUseActiveItem && PhotonNetwork.isMasterClient && !___MySetupPawn.IsWeaponStunned());
                        if (__instance.UsesHeat)
                        {
                            if (__instance.IsFiring && __instance.Heat > __instance.HeatMax)
                            {
                                flag = false;
                            }
                            else if (!__instance.IsFiring && __instance.Heat > __instance.HeatMax/2)
                            {
                                flag = false;
                            }
                        }
                    }
                    if (__instance.UsesAmmo && __instance.AmmoCurrent < __instance.AmmoMax)
                    {
                        __instance.AmmoCurrent++;
                    }
                }
                if (__instance.IsOverheated)
                {
                    if (!___hasPlayedOverheat)
                    {
                        PLMusic.PostEvent("play_sx_player_item_phasepistol_overheat", ___MySetupPawn.gameObject);
                        ___hasPlayedOverheat = true;
                    }
                    if (__instance.Heat <= 0f)
                    {
                        __instance.IsOverheated = false;
                        ___hasPlayedOverheat = false;
                    }
                }
                else if (__instance.Heat > __instance.HeatMax + 0.15f)
                {
                    __instance.IsOverheated = true;
                }
                flag &= !__instance.IsOverheated;
                if (flag != __instance.IsFiring)
                {
                    __instance.IsFiring = flag;
                    PLServer.Instance.photonView.RPC("HeldBeamPistol_ChangeStatus", PhotonTargets.Others, new object[]
                    {
                    ___MySetupPawn.GetPlayer().GetPlayerID(),
                    __instance.NetID,
                    __instance.IsFiring
                    });
                }
            }
            if (___MyGunInstance != null && ___FiringSFX == null)

            {
                ___FiringSFX = ___MyGunInstance.gameObject.AddComponent<PLAmbientSFXControl>();
                ___FiringSFX.Event = "player_item_fireextinguisher";
            }
            if (___FiringSFX != null)

            {
                ___FiringSFX.IsOnShip = (___MySetupPawn != null && ___MySetupPawn.CurrentShip != null);
                ___FiringSFX.SFXEnabled = __instance.IsFiring;
            }
            if (__instance.IsFiring && ___MySetupPawn != null)

            {
                ___ChargeAmount += Time.deltaTime * 0.25f;
                float d = 0f;
                Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
                __instance.Heat += Time.deltaTime * 0.35f;
                Ray ray;
                if (___MySetupPawn.GetPlayer() != null && ___MySetupPawn.GetPlayer().IsBot && ___MySetupPawn.MyController.AI_Item_Target != null)
                {
                    Vector3 a = ___MySetupPawn.MyController.AI_Item_Target.position - ___MySetupPawn.VerticalMouseLook.transform.position;
                    ray = new Ray(___MySetupPawn.VerticalMouseLook.transform.position, (a + onUnitSphere * d).normalized);
                }
                else
                {
                    ray = new Ray(___MySetupPawn.PawnCamera.position, (___MySetupPawn.PawnCamera.forward + onUnitSphere * d).normalized);
                }
                RaycastHit raycastHit;
                Vector3 vector;
                if (Physics.Raycast(ray, out raycastHit, 2000f, ___MySetupPawn.GetCurrentRaycastCollisionMask()))
                {
                    vector = raycastHit.point;
                }
                else
                {
                    vector = ray.origin + ray.direction * 400f;
                }
                if (___MyGunInstance != null)
                {
                    float num2 = Vector3.Distance(___MyGunInstance.ProjSpawn.position, vector);
                    ___MyGunInstance.BeamMesh.transform.localScale = new Vector3(0.015f + ___ChargeAmount * 0.02f, num2 * 0.5f, 0.015f + ___ChargeAmount * 0.02f);
                    ___MyGunInstance.BeamMesh.transform.localPosition = new Vector3(0f, 0f, num2 * 0.5f);
                    ___MyGunInstance.BeamMesh.gameObject.layer = ___MyGunInstance.gameObject.layer;
                    ___MyGunInstance.AltPS2.gameObject.layer = ___MyGunInstance.gameObject.layer;
                    ___MyGunInstance.AltPS3.gameObject.layer = ___MyGunInstance.gameObject.layer;
                    ___MyGunInstance.BeamMesh.transform.parent.LookAt(vector);
                    ___MyGunInstance.AltPS.transform.position = Vector3.Lerp(___MyGunInstance.ProjSpawn.position, vector, 0.99f);
                    if (Time.time - ___LastBeamAttackUpdateTime > 0.4f)
                    {
                        ___LastBeamAttackUpdateTime = Time.time;
                        MethodInfo methodInfo = AccessTools.Method(__instance.GetType(), "CalcDamageDone", null, null);
                        if (___MySetupPawn != null && ___MySetupPawn.photonView != null && ___MySetupPawn.photonView.isMine)
                        {
                            foreach (PLCombatTarget plcombatTarget in PLGameStatic.Instance.AllCombatTargets)
                            {
                                if (plcombatTarget != null && plcombatTarget != ___MySetupPawn && plcombatTarget.MyCurrentTLI == ___MySetupPawn.MyCurrentTLI && plcombatTarget.photonView != null && plcombatTarget.GetIsFriendly() != ___MySetupPawn.GetIsFriendly())
                                {
                                    foreach (PLPawnCollisionSphere plpawnCollisionSphere in plcombatTarget.MyCollisionSpheres)
                                    {
                                        if (plpawnCollisionSphere != null)
                                        {
                                            Vector3 a2 = PLBeamSFX.NearestPointOnLine(___MyGunInstance.ProjSpawn.position, ___MyGunInstance.ProjSpawn.forward, plpawnCollisionSphere.transform.position);
                                            float num3 = Vector3.SqrMagnitude(a2 - plpawnCollisionSphere.transform.position);
                                            float num4 = 0.25f + plpawnCollisionSphere.Radius * plpawnCollisionSphere.Radius * 1.25f;
                                            if (num3 < num4)
                                            {
                                                float num5 = (float)methodInfo.Invoke(__instance, null) * (0.1f + ___ChargeAmount * 2f);
                                                plcombatTarget.TakeDamage(num5, true, ___MySetupPawn.CombatTargetID);
                                                plcombatTarget.LastShouldForceShowInHUDTime = Time.time;
                                                PLGlobal.SendNetworkMessageIfNotLocal("TakeDamage", plcombatTarget.photonView, new object[]
                                                {
                                                num5,
                                                true,
                                                ___MySetupPawn.CombatTargetID
                                                });
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (PhotonNetwork.isMasterClient)
                        {
                            PLSystemInstance component = raycastHit.collider.GetComponent<PLSystemInstance>();
                            if (component != null && component.MySystem != null)
                            {
                                component.MySystem.TakeDamage((float)methodInfo.Invoke(__instance, null) * 0.05f);
                            }
                        }
                    }
                }
                ___MySetupPawn.Cloaked = false;
            }
            else

            {
                ___ChargeAmount -= Time.deltaTime;
            }
            ___ChargeAmount = Mathf.Clamp(___ChargeAmount, 0, __instance.HeatMax);
            if (___MyGunInstance != null)

            {
                if (___MyGunInstance.gameObject.activeSelf)
                {
                    ___MyGunInstance.AltPS2Emm.rateOverTime = 100f * ___ChargeAmount;
                    ___MyGunInstance.AltPS3Emm.enabled = (___ChargeAmount > 0.935f);
                }
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.BeamMesh.gameObject, __instance.IsFiring);
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.AltPS.gameObject, __instance.IsFiring);
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.AltPS2.gameObject, true);
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.AltPS3.gameObject, true);
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.MuzzleFlash.gameObject, __instance.IsFiring);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(PLPawnItem_HeldBeamPistol_WithHealing))]
    [HarmonyPatch("OnUpdate")]
    class HealBeamHeat
    {
        static bool Prefix(PLPawnItem_HeldBeamPistol_WithHealing __instance, PLPawn ___MySetupPawn, PLPawnItemInstance ___MyGunInstance, EAimFireState ___AimFireState, ref bool ___hasPlayedOverheat, Transform[] ___gunChildTransforms, PLPawnInventory ___MyInventory, PLAmbientSFXControl ___FiringSFX, float ___ChargeAmount, float ___LastBeamAttackUpdateTime)
        {
            float num = 1f;
            PLPlayer cachedFriendlyPlayerOfClass = PLServer.Instance.GetCachedFriendlyPlayerOfClass(3);
            if (___MySetupPawn != null && ___MySetupPawn.GetPlayer() != null && ___MySetupPawn.GetPlayer().TeamID == 0 && cachedFriendlyPlayerOfClass != null)
            {
                num = 1f + (float)cachedFriendlyPlayerOfClass.Talents[23] * 0.32f;
            }
            foreach (PawnStatusEffect pawnStatusEffect in ___MySetupPawn.MyStatusEffects)
            {
                if (pawnStatusEffect != null && pawnStatusEffect.Type == EPawnStatusEffectType.GUN_COOLING)
                {
                    num += 0.4f * pawnStatusEffect.Strength;
                }
            }
            __instance.Heat -= Time.deltaTime * (0.2f * num);
            __instance.Heat = Mathf.Clamp(__instance.Heat, 0f, float.MaxValue);
            if (___MySetupPawn == null)
            {
                return false;
            }
            if (___MyGunInstance != null)
            {
                if (___MyGunInstance.transform.parent != __instance.GetTargetParentTransform())
                {
                    ___MyGunInstance.transform.parent = __instance.GetTargetParentTransform();
                    ___MyGunInstance.transform.localPosition = Vector3.zero;
                    ___MyGunInstance.transform.localRotation = Quaternion.identity;
                }
                if (UnityEngine.Random.Range(0, 1000) == 0)
                {
                    ___MyGunInstance.transform.localPosition = Vector3.zero;
                    ___MyGunInstance.transform.localRotation = Quaternion.identity;
                }
            }
            if (___MyGunInstance != null)
            {
                if (___gunChildTransforms == null || ___gunChildTransforms.Length == 0 || ___gunChildTransforms[0] == null)
                {
                    ___gunChildTransforms = ___MyGunInstance.gameObject.GetComponentsInChildren<Transform>();
                }
                foreach (Transform transform in ___gunChildTransforms)
                {
                    if (transform != null)
                    {
                        transform.gameObject.layer = ___MySetupPawn.gameObject.layer;
                    }
                }
            }

            if (___MySetupPawn != null && ___MySetupPawn.GetPlayer() != null && ___MyGunInstance != null && (___MySetupPawn.GetPlayer().GetPlayerID() == PLNetworkManager.Instance.LocalPlayerID || (___MySetupPawn.GetPlayer().IsBot && PhotonNetwork.isMasterClient)))
            {
                ___AimFireState = EAimFireState.NEUTRAL;
                bool flag = (!PLInput.Instance.GetButton(PLInput.EInputActionName.unlock_mouse) || ___MySetupPawn.ShowJetpackVisuals) && ___MySetupPawn.GetPlayer().GetPlayerID() == PLNetworkManager.Instance.LocalPlayerID && ___MyInventory.ActiveItem == __instance && !___MySetupPawn.IsDead && ___MySetupPawn == PLNetworkManager.Instance.MyLocalPawn && !___MySetupPawn.IsSprinting && ___AimFireState != EAimFireState.FRIENDLY && !PLNetworkManager.Instance.MainMenu.IsActive() && PLNetworkManager.Instance.MainMenu.GetActiveMenuCount() == 0 && PLCameraSystem.Instance != null && PLCameraSystem.Instance.GetModeString() == "LocalPawn" && PLTabMenu.Instance != null && !PLTabMenu.Instance.TabMenuActive && PLTabMenu.Instance.DialogueMenu != null && PLTabMenu.Instance.DialogueMenu.CurrentActorInstance == null && PLTabMenu.Instance.TargetContainer == null && !PLTabMenu.Instance.IsDisplayingOrderMenu();
                bool flag2 = PLInput.Instance.GetButtonDown(PLInput.EInputActionName.fire) && flag;
                bool flag3 = PLInput.Instance.GetButton(PLInput.EInputActionName.fire) && flag;
                flag = flag3;
                if (___MySetupPawn.MyPlayer.IsBot)
                {
                    flag = false;
                    PLBotController plbotController = ___MySetupPawn.MyController as PLBotController;
                    if (plbotController != null)
                    {
                        flag = (!___MySetupPawn.IsDead && (plbotController.AI_ItemUtilityRequest == EItemUtilityType.E_DAMAGE || plbotController.AI_ItemUtilityRequest == EItemUtilityType.E_HEALING) && plbotController.AI_ShouldUseActiveItem && PhotonNetwork.isMasterClient && !___MySetupPawn.IsWeaponStunned());
                        if (__instance.UsesHeat)
                        {
                            if (__instance.IsFiring && __instance.Heat > __instance.HeatMax)
                            {
                                flag = false;
                            }
                            else if (!__instance.IsFiring && __instance.Heat > __instance.HeatMax/2)
                            {
                                flag = false;
                            }
                        }
                    }
                    if (__instance.UsesAmmo && __instance.AmmoCurrent < __instance.AmmoMax)
                    {
                        __instance.AmmoCurrent++;
                    }
                }
                if (__instance.IsOverheated)
                {
                    if (!___hasPlayedOverheat)
                    {
                        PLMusic.PostEvent("play_sx_player_item_phasepistol_overheat", ___MySetupPawn.gameObject);
                        ___hasPlayedOverheat = true;
                    }
                    if (__instance.Heat <= 0f)
                    {
                        __instance.IsOverheated = false;
                        ___hasPlayedOverheat = false;
                    }
                }
                else if (__instance.Heat > __instance.HeatMax + 0.15f)
                {
                    __instance.IsOverheated = true;
                }
                flag &= !__instance.IsOverheated;
                if (flag != __instance.IsFiring)
                {
                    __instance.IsFiring = flag;
                    PLServer.Instance.photonView.RPC("HeldBeamPistolHealing_ChangeStatus", PhotonTargets.Others, new object[]
                    {
                    ___MySetupPawn.GetPlayer().GetPlayerID(),
                    __instance.NetID,
                    __instance.IsFiring
                    });
                }
            }
            if (___MyGunInstance != null && ___FiringSFX == null)
            {
                ___FiringSFX = ___MyGunInstance.gameObject.AddComponent<PLAmbientSFXControl>();
                ___FiringSFX.Event = "sx_ship_generic_internal_computer_coolant_high";
            }
            if (___FiringSFX != null)
            {
                ___FiringSFX.IsOnShip = (___MySetupPawn != null && ___MySetupPawn.CurrentShip != null);
                ___FiringSFX.SFXEnabled = __instance.IsFiring;
            }
            if (__instance.IsFiring && ___MySetupPawn != null)
            {
                ___ChargeAmount += Time.deltaTime * 0.25f;
                float d = 0f;
                Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
                __instance.Heat += Time.deltaTime * 0.35f;
                Ray ray;
                if (___MySetupPawn.GetPlayer() != null && ___MySetupPawn.GetPlayer().IsBot && ___MySetupPawn.MyController.AI_Item_Target != null)
                {
                    Vector3 a = ___MySetupPawn.MyController.AI_Item_Target.position - ___MySetupPawn.VerticalMouseLook.transform.position;
                    ray = new Ray(___MySetupPawn.VerticalMouseLook.transform.position, (a + onUnitSphere * d).normalized);
                }
                else
                {
                    ray = new Ray(___MySetupPawn.PawnCamera.position, (___MySetupPawn.PawnCamera.forward + onUnitSphere * d).normalized);
                }
                RaycastHit raycastHit;
                Vector3 vector;
                if (Physics.Raycast(ray, out raycastHit, 2000f, ___MySetupPawn.GetCurrentRaycastCollisionMask()))
                {
                    vector = raycastHit.point;
                }
                else
                {
                    vector = ray.origin + ray.direction * 400f;
                }
                if (___MyGunInstance != null)
                {
                    float num2 = Vector3.Distance(___MyGunInstance.ProjSpawn.position, vector);
                    ___MyGunInstance.BeamMesh.transform.localScale = new Vector3(0.015f + ___ChargeAmount * 0.02f, num2 * 0.5f, 0.015f + ___ChargeAmount * 0.02f);
                    ___MyGunInstance.BeamMesh.transform.localPosition = new Vector3(0f, 0f, num2 * 0.5f);
                    ___MyGunInstance.BeamMesh.gameObject.layer = ___MyGunInstance.gameObject.layer;
                    ___MyGunInstance.AltPS2.gameObject.layer = ___MyGunInstance.gameObject.layer;
                    ___MyGunInstance.AltPS3.gameObject.layer = ___MyGunInstance.gameObject.layer;
                    ___MyGunInstance.BeamMesh.transform.parent.LookAt(vector);
                    ___MyGunInstance.AltPS.transform.position = Vector3.Lerp(___MyGunInstance.ProjSpawn.position, vector, 0.99f);
                    if (Time.time - ___LastBeamAttackUpdateTime > 0.4f)
                    {
                        ___LastBeamAttackUpdateTime = Time.time;
                        MethodInfo methodInfo = AccessTools.Method(__instance.GetType(), "CalcDamageDone", null, null);
                        MethodInfo methodInfo2 = AccessTools.Method(__instance.GetType(), "CalcHealingDone", null, null);
                        if (___MySetupPawn != null && ___MySetupPawn.photonView != null && ___MySetupPawn.photonView.isMine)
                        {
                            foreach (PLCombatTarget plcombatTarget in PLGameStatic.Instance.AllCombatTargets)
                            {
                                if (plcombatTarget != null && plcombatTarget != ___MySetupPawn && plcombatTarget.MyCurrentTLI == ___MySetupPawn.MyCurrentTLI && plcombatTarget.photonView != null)
                                {
                                    foreach (PLPawnCollisionSphere plpawnCollisionSphere in plcombatTarget.MyCollisionSpheres)
                                    {
                                        if (plpawnCollisionSphere != null)
                                        {
                                            Vector3 a2 = PLBeamSFX.NearestPointOnLine(___MyGunInstance.ProjSpawn.position, ___MyGunInstance.ProjSpawn.forward, plpawnCollisionSphere.transform.position);
                                            float num3 = Vector3.SqrMagnitude(a2 - plpawnCollisionSphere.transform.position);
                                            float num4 = 0.33f + plpawnCollisionSphere.Radius * plpawnCollisionSphere.Radius * 1.25f;
                                            float num5 = 0.1f + ___ChargeAmount * 2f;
                                            if (num3 < num4)
                                            {
                                                if (plcombatTarget.GetIsFriendly() != ___MySetupPawn.GetIsFriendly())
                                                {
                                                    float num6 = (float)methodInfo.Invoke(__instance, null) * num5;
                                                    plcombatTarget.TakeDamage(num6, true, ___MySetupPawn.CombatTargetID);
                                                    plcombatTarget.LastShouldForceShowInHUDTime = Time.time;
                                                    PLGlobal.SendNetworkMessageIfNotLocal("TakeDamage", plcombatTarget.photonView, new object[]
                                                    {
                                                    num6,
                                                    true,
                                                    ___MySetupPawn.CombatTargetID
                                                    });
                                                }
                                                else
                                                {
                                                    float num6 = (float)methodInfo2.Invoke(__instance, null) * num5;
                                                    plcombatTarget.Heal(num6);
                                                    plcombatTarget.LastShouldForceShowInHUDTime = Time.time;
                                                    PLGlobal.SendNetworkMessageIfNotLocal("Heal", plcombatTarget.photonView, new object[]
                                                    {
                                                    num6
                                                    });
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (PhotonNetwork.isMasterClient)
                        {
                            PLSystemInstance component = raycastHit.collider.GetComponent<PLSystemInstance>();
                            if (component != null && component.MySystem != null)
                            {
                                component.MySystem.TakeDamage((float)methodInfo.Invoke(__instance, null) * 0.05f);
                            }
                        }
                    }
                }
                ___MySetupPawn.Cloaked = false;
            }
            else
            {
                ___ChargeAmount -= Time.deltaTime * 1.4f;
            }
            ___ChargeAmount = Mathf.Clamp(___ChargeAmount, 0, __instance.HeatMax);
            if (___MyGunInstance != null)
            {
                if (___MyGunInstance.gameObject.activeSelf)
                {
                    ___MyGunInstance.AltPS2Emm.rateOverTime = 100f * ___ChargeAmount;
                    ___MyGunInstance.AltPS3Emm.enabled = (___ChargeAmount > 0.935f);
                }
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.BeamMesh.gameObject, __instance.IsFiring);
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.AltPS.gameObject, __instance.IsFiring);
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.AltPS2.gameObject, true);
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.AltPS3.gameObject, true);
                PLGlobal.SafeGameObjectSetActive(___MyGunInstance.MuzzleFlash.gameObject, __instance.IsFiring);
            }
            return false;
        }
    }
}

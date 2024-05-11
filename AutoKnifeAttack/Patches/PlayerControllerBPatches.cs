using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class AutomaticKnifePatch
{
    internal static float AttackCooldown = 0.07f;
    private static float AttackDelay = 0.0f;
    private static bool IsUsingAttack => IngamePlayerSettings.Instance.playerInput.actions.FindAction("ActivateItem", false).IsPressed();

    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    internal static void ItemActivatePostfix(PlayerControllerB __instance)
    {
        if (GameNetworkManager.Instance.localPlayerController is PlayerControllerB localPlayer && localPlayer.actualClientId == __instance.actualClientId) return;
        if (__instance.currentlyHeldObjectServer is GrabbableObject item && item is KnifeItem)
        {

            if (IsUsingAttack)
            {
                AttackDelay += Time.deltaTime;
                if (AttackDelay >= AttackCooldown)
                {
                    if (item.RequireCooldown())
                    {
                        return;
                    }
                    AttackDelay = 0f;
                    item.UseItemOnClient(true);
                }
            }
        }
    }
}

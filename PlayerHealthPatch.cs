using HarmonyLib;

namespace RepoPracticeMod;

[HarmonyPatch(typeof(PlayerHealth), nameof(PlayerHealth.Hurt))]
internal static class PlayerHealthPatch
{
    internal static bool GodModeEnabled { get; set; }

    [HarmonyPrefix]
    private static bool Prefix(PlayerHealth __instance)
    {
        if (!GodModeEnabled)
        {
            return true;
        }

        var localPlayer = SemiFunc.PlayerAvatarLocal();

        if (localPlayer == null)
        {
            return true;
        }

        if (__instance == localPlayer.playerHealth)
        {
            return false;
        }

        return true;
    }
}
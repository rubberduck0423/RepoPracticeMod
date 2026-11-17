using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MenuLib;
using UnityEngine;

namespace RepoPracticeMod;

[BepInPlugin("MaNyaNyo.RepoPracticeMod", "RepoPracticeMod", "1.0")]
[BepInDependency("nickklmao.menulib", BepInDependency.DependencyFlags.HardDependency)]
public class RepoPracticeMod : BaseUnityPlugin
{
    internal static RepoPracticeMod Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    private void Awake()
    {
        Instance = this;
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();

        MenuAPI.AddElementToMainMenu(parent =>
        {
            MenuAPI.CreateREPOButton(
                "RepoDev Test",
                () => Logger.LogInfo("테스트 버튼을 눌렀습니다."),
                parent,
                localPosition: Vector2.zero
            );
        });

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void Update()
    {
        // Code that runs every frame goes here
    }
}
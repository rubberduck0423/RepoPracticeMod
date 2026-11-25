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

        MenuAPI.AddElementToEscapeMenu(parent =>
        {
            var position = new Vector2(0f, 0f);
            var buttons = parent.GetComponentsInChildren<MenuButton>(true);
            bool foundButton = false;

            foreach (var button in buttons)
            {
                Vector3 buttonPosition = parent.InverseTransformPoint(button.transform.position);

                if (!foundButton || buttonPosition.y < position.y)
                {
                    position = new Vector2(buttonPosition.x, buttonPosition.y);
                    foundButton = true;
                }
            }

            position.y -= 40f;
            MenuAPI.CreateREPOButton("모드", OpenModPage, parent, localPosition: position);
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
        
    }

    private void Update()
    {
        
    }

    private void OpenModPage()
    {
        var page = MenuAPI.CreateREPOPopupPage("모드", shouldCachePage: false, pageDimmerVisibility: true, spacing: 20f); // 모드 창

        page.AddElementToScrollView(parent => //스크롤 바
        {
            var toggle = MenuAPI.CreateREPOToggle("무적", enabled => // 무적 토글
            {
                PlayerHealthPatch.GodModeEnabled = enabled;
                Logger.LogInfo(enabled ? "ON" : "OFF");
            }, parent, defaultValue: PlayerHealthPatch.GodModeEnabled);

            return toggle.rectTransform;
        });

        page.AddElementToScrollView(parent => // 아이템 소환 버튼
        {
            var button = MenuAPI.CreateREPOButton("아이템 소환", ItemMenu.Open, parent);
            return button.rectTransform;
        });

        page.AddElementToScrollView(parent => // 몬스터 소환 버튼
        {
            var button = MenuAPI.CreateREPOButton("몬스터 소환", EnemyMenu.Open, parent);
            return button.rectTransform;
        });

        page.AddElementToScrollView(parent => // 뒤로가기 버튼
        {
            var button = MenuAPI.CreateREPOButton("뒤로가기", () => page.ClosePage(closePagesAddedOnTop: false), parent);
            return button.rectTransform;
        });

        page.OpenPage(openOnTop: false);
    }
}
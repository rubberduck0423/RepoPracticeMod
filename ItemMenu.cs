using System.Linq;
using MenuLib;

namespace RepoPracticeMod;

internal static class ItemMenu
{
    internal static void Open()
    {
        var stats = StatsManager.instance;

        if (stats == null || stats.itemDictionary == null)
        {
            RepoPracticeMod.Logger.LogWarning("아이템 목록이 아직 준비되지 않았습니다.");
            return;
        }

        var items = stats.itemDictionary.Values.Where(item => item != null && item.prefab != null).Distinct().OrderBy(item => item.itemName).ToList();
        var page = MenuAPI.CreateREPOPopupPage("아이템 소환", shouldCachePage: false, pageDimmerVisibility: true, spacing: 10f);

        foreach (var item in items)
        {
            page.AddElementToScrollView(parent =>
            {
                string displayName = string.IsNullOrWhiteSpace(item.itemName) ? item.name : item.itemName;
                var button = MenuAPI.CreateREPOButton(displayName, () => ItemSpawner.Spawn(item), parent);
                return button.rectTransform;
            });
        }

        page.AddElementToScrollView(parent =>
        {
            var button = MenuAPI.CreateREPOButton("뒤로가기", () => page.ClosePage(closePagesAddedOnTop: false), parent);
            return button.rectTransform;
        });

        page.OpenPage(openOnTop: false);
    }
}
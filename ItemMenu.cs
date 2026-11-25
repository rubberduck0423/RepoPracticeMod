using System.Linq;
using MenuLib;

namespace RepoPracticeMod;

internal static class ItemMenu
{
    internal static void Open()
    {
        var stats = StatsManager.instance;

        

        var items = stats.itemDictionary.Values.Where(item => item != null && item.prefab != null).Distinct().OrderBy(item => item.itemName).ToList(); // 아이템 목록 가져오기
        var page = MenuAPI.CreateREPOPopupPage("아이템 소환", shouldCachePage: false, pageDimmerVisibility: true, spacing: 10f); // 아이템 소환 창

        foreach (var item in items)
        {
            page.AddElementToScrollView(parent => // 아이템 버튼
            {
                string displayName = string.IsNullOrWhiteSpace(item.itemName) ? item.name : item.itemName;
                var button = MenuAPI.CreateREPOButton(displayName, () => ItemSpawner.Spawn(item), parent); // 아이템 소환
                return button.rectTransform;
            });
        }

        page.AddElementToScrollView(parent =>
        {
            var button = MenuAPI.CreateREPOButton("뒤로가기", () => page.ClosePage(closePagesAddedOnTop: false), parent); // 뒤로가기 버튼
            return button.rectTransform;
        });

        page.OpenPage(openOnTop: false);
    }
}
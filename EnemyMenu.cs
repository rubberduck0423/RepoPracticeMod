using System.Linq;
using MenuLib;

namespace RepoPracticeMod;

internal static class EnemyMenu
{
    internal static void Open()
    {
        var director = EnemyDirector.instance;

        

        var enemies = director.enemiesDifficulty1.Concat(director.enemiesDifficulty2).Concat(director.enemiesDifficulty3).Where(enemy => enemy != null && enemy.spawnObjects != null && enemy.spawnObjects.Count > 0).Distinct().OrderBy(enemy => enemy.name).ToList(); // 몬스터 목록 가져오기
        var page = MenuAPI.CreateREPOPopupPage("몬스터 소환", shouldCachePage: false, pageDimmerVisibility: true, spacing: 10f); // 몬스터 소환 창

        foreach (var enemy in enemies)
        {
            page.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton(enemy.name, () => EnemySpawner.Spawn(enemy), parent); // 몬스터 소환
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
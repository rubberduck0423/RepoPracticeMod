using HarmonyLib;
using UnityEngine;

namespace RepoPracticeMod;

internal static class EnemySpawner
{
    internal static void Spawn(EnemySetup setup)
    {
        var point = SemiFunc.LevelPointsGetClosestToLocalPlayer(); // 플레이어 근처의 LevelPoint 가져오기
        Vector3 position = point.transform.position;

        foreach (var prefabRef in setup.spawnObjects)
        {
            var spawned = UnityEngine.Object.Instantiate(prefabRef.Prefab, position, Quaternion.identity); // EnemyParent 프리팹 소환

            var enemyParent = spawned.GetComponent<EnemyParent>();
            var enemy = spawned.GetComponentInChildren<Enemy>();

            AccessTools.Field(typeof(EnemyParent), "SetupDone").SetValue(enemyParent, true);
            AccessTools.Field(typeof(EnemyParent), "firstSpawnPointUsed").SetValue(enemyParent, true);

            enemy.EnemyTeleported(position); // Enemy의 위치를 LevelPoint로 이동
        }
    }
}
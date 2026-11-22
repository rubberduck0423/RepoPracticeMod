using System;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace RepoPracticeMod;

internal static class EnemySpawner
{
    internal static void Spawn(EnemySetup setup)
    {
        var player = SemiFunc.PlayerAvatarLocal();

        if (player == null || !SemiFunc.LevelGenDone() || !SemiFunc.RunIsLevel())
        {
            RepoPracticeMod.Logger.LogWarning("몬스터 소환은 맵 생성이 끝난 탐사 중에 사용하세요.");
            return;
        }

        if (!SemiFunc.IsMasterClientOrSingleplayer())
        {
            RepoPracticeMod.Logger.LogWarning("멀티플레이에서는 방장이 소환해야 합니다.");
            return;
        }

        if (setup == null || setup.spawnObjects == null || setup.spawnObjects.Count == 0)
        {
            RepoPracticeMod.Logger.LogWarning("몬스터의 생성 정보를 찾을 수 없습니다.");
            return;
        }

        Transform directionSource = player.localCamera != null ? player.localCamera.transform : player.transform;
        Vector3 forward = Vector3.ProjectOnPlane(directionSource.forward, Vector3.up).normalized;

        if (forward.sqrMagnitude < 0.01f)
        {
            forward = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up).normalized;
        }

        Vector3 target = player.transform.position + forward * 4f;

        if (!NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            RepoPracticeMod.Logger.LogWarning("앞쪽에서 몬스터가 이동할 수 있는 바닥을 찾지 못했습니다.");
            return;
        }

        foreach (var prefabRef in setup.spawnObjects)
        {
            if (prefabRef == null)
            {
                continue;
            }

            try
            {
                GameObject spawned;

                if (SemiFunc.IsMultiplayer())
                {
                    var spawnDataMethod = AccessTools.Method("EnemyFloaterHalloweenState:SpawnData");
                    var spawnData = spawnDataMethod?.Invoke(null, new object[] { prefabRef }) as object[];
                    spawned = PhotonNetwork.InstantiateRoomObject(prefabRef.ResourcePath, hit.position, Quaternion.identity, 0, spawnData);
                }
                else
                {
                    var prefab = prefabRef.Prefab;

                    if (prefab == null)
                    {
                        RepoPracticeMod.Logger.LogWarning($"프리팹을 불러오지 못했습니다: {setup.name}");
                        continue;
                    }

                    spawned = UnityEngine.Object.Instantiate(prefab, hit.position, Quaternion.identity);
                }

                if (spawned == null)
                {
                    RepoPracticeMod.Logger.LogWarning($"몬스터 생성에 실패했습니다: {setup.name}");
                    continue;
                }

                var enemyParent = spawned.GetComponent<EnemyParent>();
                var enemy = spawned.GetComponentInChildren<Enemy>();

                if (enemyParent != null)
                {
                    AccessTools.Field(typeof(EnemyParent), "SetupDone").SetValue(enemyParent, true);
                    AccessTools.Field(typeof(EnemyParent), "firstSpawnPointUsed").SetValue(enemyParent, true);
                }

                if (enemy != null)
                {
                    enemy.EnemyTeleported(hit.position);
                }

                RepoPracticeMod.Logger.LogInfo($"몬스터 소환 완료: {setup.name}");
            }
            catch (Exception exception)
            {
                RepoPracticeMod.Logger.LogError($"몬스터 소환 중 오류: {setup.name}\n{exception}");
            }
        }
    }
}
using System;
using Photon.Pun;
using UnityEngine;

namespace RepoPracticeMod;

internal static class ItemSpawner
{
    internal static void Spawn(Item item)
    {
        var player = SemiFunc.PlayerAvatarLocal();

        if (player == null || !SemiFunc.LevelGenDone())
        {
            RepoPracticeMod.Logger.LogWarning("플레이어 또는 맵이 준비되지 않았습니다.");
            return;
        }

        if (!SemiFunc.IsMasterClientOrSingleplayer())
        {
            RepoPracticeMod.Logger.LogWarning("멀티플레이에서는 방장이 소환해야 합니다.");
            return;
        }

        if (item == null || item.prefab == null)
        {
            RepoPracticeMod.Logger.LogWarning("아이템의 생성 정보를 찾을 수 없습니다.");
            return;
        }

        try
        {
            Transform directionSource = player.localCamera != null ? player.localCamera.transform : player.transform;
            Vector3 forward = Vector3.ProjectOnPlane(directionSource.forward, Vector3.up).normalized;

            if (forward.sqrMagnitude < 0.01f)
            {
                forward = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up).normalized;
            }

            Vector3 position = player.transform.position + forward * 2f + Vector3.up * 0.5f;
            Quaternion rotation = Quaternion.LookRotation(forward) * item.spawnRotationOffset;
            GameObject spawned;

            if (SemiFunc.IsMultiplayer())
            {
                spawned = PhotonNetwork.InstantiateRoomObject(item.prefab.ResourcePath, position, rotation);
            }
            else
            {
                var prefab = item.prefab.Prefab;

                if (prefab == null)
                {
                    RepoPracticeMod.Logger.LogWarning($"프리팹을 불러오지 못했습니다. {item.name}");
                    return;
                }

                spawned = UnityEngine.Object.Instantiate(prefab, position, rotation);
            }

            if (spawned == null)
            {
                RepoPracticeMod.Logger.LogWarning($"아이템 생성에 실패했습니다. {item.name}");
                return;
            }

            RepoPracticeMod.Logger.LogInfo($"아이템 소환 완료: {item.name}");
        }
        catch (Exception exception)
        {
            RepoPracticeMod.Logger.LogError($"아이템 소환 중 오류: {item.name}\n{exception}");
        }
    }
}
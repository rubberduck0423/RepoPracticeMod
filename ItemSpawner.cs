using UnityEngine;

namespace RepoPracticeMod;

internal static class ItemSpawner
{
    internal static void Spawn(Item item)
    {
        var player = SemiFunc.PlayerAvatarLocal(); // 로컬 플레이어 가져오기

        Vector3 forward = Vector3.ProjectOnPlane(player.localCamera.transform.forward, Vector3.up).normalized;
        Vector3 position = player.transform.position + forward * 2f + Vector3.up * 0.5f; // 플레이어 앞쪽
        Quaternion rotation = Quaternion.LookRotation(forward) * item.spawnRotationOffset; 

        UnityEngine.Object.Instantiate(item.prefab.Prefab, position, rotation); // 아이템 소환
    }
}
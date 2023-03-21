using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Game;

using IngameDebugConsole;

namespace FrigidBlackwaters.Cheats
{
    public class ConsoleCommands
    {
        [ConsoleMethod("Info", "Basic Information about the Game")]
        public static void Info()
        {
            Debug.Log("Frigid Blackwaters is a Story Driven Dungeon Crawler, and this is its command line.");
        }

        [ConsoleMethod("ReloadScene", "ReloadCurrentScene")]
        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        [ConsoleMethod("ReloadSceneFancy", "ReloadCurrentScene")]
        public static void ReloadSceneFancy()
        {
            SceneChanger.Instance.ChangeScene(SceneManager.GetActiveScene().name, null);
        }

        [ConsoleMethod("SetTimeScale", "Set the time scale of the game")]
        public static void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

#if UNITY_EDITOR
        [ConsoleMethod("SpawnPlayer", "Spawn the player at your mouse position.")]
        public static void SpawnPlayer()
        {
            SpawnMobs("Player");
        }

        [ConsoleMethod("SpawnMob", "Spawn a mob at your mouse position.")]
        public static void SpawnMobs(string spawnableName)
        {
            SpawnMobs(spawnableName, 1, 0);
        }

        [ConsoleMethod("SpawnMob", "Spawn a mob at your mouse position.")]
        public static void SpawnMobs(string spawnableName, int quantity, float radius)
        {
            string[] guids = AssetDatabase.FindAssets(spawnableName + " t:" + typeof(MobSpawnable).Name);
            if (guids.Length > 0)
            {
                MobSpawnable mobSpawnable = AssetDatabase.LoadAssetAtPath<MobSpawnable>(AssetDatabase.GUIDToAssetPath(guids[0]));
                for (int i = 0; i < quantity; i++)
                {
                    float angleRad = Mathf.PI * 2 / quantity * i;
                    Vector2 spawnPosition = (Vector2)MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()) + new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;
                    if (mobSpawnable.CanSpawnMobAt(spawnPosition))
                    {
                        mobSpawnable.SpawnMob(spawnPosition, Vector2.down);
                    }
                }
            }
        }

        [ConsoleMethod("MovePlayer", "Moves the player to your mouse position.")]
        public static void MovePlayer()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                Vector2 movePosition = MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
                if (player.CanMoveTo(movePosition))
                {
                    player.MoveTo(movePosition);
                }
            }
        }

        [ConsoleMethod("SpawnProjectile", "Spawn a projectile at your mouse position.")]
        public static void SpawnProjectile(string projectilePrefabName)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab " + projectilePrefabName);
            foreach (string guid in guids)
            {
                Projectile projectilePrefab = AssetDatabase.LoadAssetAtPath<Projectile>(AssetDatabase.GUIDToAssetPath(guid));
                if (projectilePrefab != null)
                {
                    Projectile spawnedProjectile = FrigidInstancing.CreateInstance<Projectile>(projectilePrefab);
                    spawnedProjectile.LaunchProjectile(
                        0,
                        DamageAlignment.Neutrals,
                        () => FrigidInstancing.DestroyInstance(spawnedProjectile),
                        MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()),
                        Vector2.right,
                        null,
                        null,
                        null
                        );
                    return;
                }
            }
        }

        [ConsoleMethod("SpawnExplosion", "Spawn a explosion at your mouse position.")]
        public static void SpawnExplosion(string explosionPrefabName)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab " + explosionPrefabName);
            foreach (string guid in guids)
            {
                Explosion explosionPrefab = AssetDatabase.LoadAssetAtPath<Explosion>(AssetDatabase.GUIDToAssetPath(guid));
                if (explosionPrefab != null)
                {
                    Explosion spawnedExplosion = FrigidInstancing.CreateInstance<Explosion>(explosionPrefab);
                    spawnedExplosion.SummonExplosion(
                        0,
                        DamageAlignment.Neutrals,
                        () => FrigidInstancing.DestroyInstance(spawnedExplosion),
                        MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()),
                        0,
                        null,
                        null,
                        null
                        );
                    return;
                }
            }
        }

        [ConsoleMethod("GiveItemToPlayer", "Give the player an item.")]
        public static void GiveItemToPlayer(string itemStorableName)
        {
            GiveItemToPlayer(itemStorableName, 1);
        }

        [ConsoleMethod("GiveItemToPlayer", "Give the player an item.")]
        public static void GiveItemToPlayer(string itemStorableName, int quantity)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(ItemStorable).Name + " " + itemStorableName);
            foreach (string guid in guids)
            {
                ItemStorable itemStorable = AssetDatabase.LoadAssetAtPath<ItemStorable>(AssetDatabase.GUIDToAssetPath(guid));
                if (itemStorable != null)
                {
                    if (PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out ItemStorage playerItemStorage))
                    {
                        foreach (ItemStorageGrid storageGrid in playerItemStorage.StorageGrids)
                        {
                            for (int x = 0; x < storageGrid.Dimensions.x; x++)
                            {
                                for (int y = 0; y < storageGrid.Dimensions.y; y++)
                                {
                                    if (storageGrid.TryGetStash(new Vector2Int(x, y), out ContainerItemStash itemStash))
                                    {
                                        if (itemStash.CanStackStorable(itemStorable) && !itemStash.IsFull)
                                        {
                                            List<Item> createdItems = itemStorable.CreateItems(quantity);
                                            playerItemStorage.AddStoredItems(createdItems);
                                            itemStash.AddItems(createdItems, itemStorable);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}

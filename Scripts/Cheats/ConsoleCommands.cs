using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;
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
            if (AssetDatabaseUpdater.TryFindAsset<MobSpawnable>(spawnableName, out MobSpawnable mobSpawnable))
            {
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

        [ConsoleMethod("GiveStamps", "Gives the player stamps.")]
        public static void GiveStamps(int stampsAdded)
        {
            Stamps.TotalStamps += stampsAdded;
            Stamps.CurrentStamps += stampsAdded;
        }

        [ConsoleMethod("MovePlayer", "Moves the player to your mouse position.")]
        public static void MovePlayer()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                Vector2 movePosition = MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
                if (player.CanMoveTo(movePosition, false))
                {
                    player.MoveTo(movePosition, false);
                }
            }
        }

        [ConsoleMethod("SpawnProjectile", "Spawn a projectile at your mouse position.")]
        public static void SpawnProjectile(string projectilePrefabName)
        {
            if (AssetDatabaseUpdater.TryFindPrefab<Projectile>(projectilePrefabName, out Projectile projectilePrefab))
            {
                Projectile spawnedProjectile = FrigidMonoBehaviour.CreateInstance<Projectile>(projectilePrefab);
                spawnedProjectile.LaunchProjectile(
                    0,
                    DamageAlignment.Neutrals,
                    () => FrigidMonoBehaviour.DestroyInstance(spawnedProjectile),
                    MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()),
                    Vector2.right,
                    null,
                    null,
                    null
                    );
            }
        }

        [ConsoleMethod("SpawnExplosion", "Spawn a explosion at your mouse position.")]
        public static void SpawnExplosion(string explosionPrefabName)
        {
            if (AssetDatabaseUpdater.TryFindPrefab<Explosion>(explosionPrefabName, out Explosion explosionPrefab))
            {
                Explosion spawnedExplosion = FrigidMonoBehaviour.CreateInstance<Explosion>(explosionPrefab);
                spawnedExplosion.SummonExplosion(
                    0,
                    DamageAlignment.Neutrals,
                    () => FrigidMonoBehaviour.DestroyInstance(spawnedExplosion),
                    MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()),
                    0,
                    null,
                    null,
                    null
                    );
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
            if (PlayerMob.TryGet(out PlayerMob player) && ItemStorage.TryGetStorageUsedByMob(player, out ItemStorage playerItemStorage) && AssetDatabaseUpdater.TryFindAsset<ItemStorable>(itemStorableName, out ItemStorable itemStorable))
            {
                List<Item> items = itemStorable.CreateItems(quantity);
                foreach (ItemStorageGrid storageGrid in playerItemStorage.StorageGrids)
                {
                    for (int x = 0; x < storageGrid.Dimensions.x; x++)
                    {
                        for (int y = 0; y < storageGrid.Dimensions.y; y++)
                        {
                            if (storageGrid.TryGetStash(new Vector2Int(x, y), out ContainerItemStash itemStash))
                            {
                                items.RemoveRange(0, itemStash.PushItems(itemStorable, items));
                                if (items.Count == 0)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                ItemStorable.DiscardItems(items);
            }
        }

        [ConsoleMethod("AddPlayerEquipment", "Add equipment to the player.")]
        public static void AddPlayerEquipment(string equipmentSpawnableName, string equipContextName)
        {
            if (PlayerMob.TryGet(out PlayerMob player) && AssetDatabaseUpdater.TryFindAsset<MobEquipmentSpawnable>(equipmentSpawnableName, out MobEquipmentSpawnable mobEquipmentSpawnable) && AssetDatabaseUpdater.TryFindAsset<MobEquipContext>(equipContextName, out MobEquipContext mobEquipContext)) 
            {
                if (player.TryGetEquipPointInContext(mobEquipContext, out MobEquipPoint mobEquipPoint))
                {
                    mobEquipPoint.AddEquipment(mobEquipmentSpawnable.Spawn());
                }
            }
        }
#endif
    }
}

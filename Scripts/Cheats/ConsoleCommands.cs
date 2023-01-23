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
            SpawnMob(FrigidPaths.ProjectFolder.UNIVERSAL, "Player");
        }

        [ConsoleMethod("SpawnMob", "Spawn a mob at your mouse position.")]
        public static void SpawnMob(string contextFolderName, string spawnableName)
        {
            SpawnMob(contextFolderName, spawnableName, 1, 0);
        }

        [ConsoleMethod("SpawnMob", "Spawn a mob at your mouse position.")]
        public static void SpawnMob(string contextFolderName, string spawnableName, int quantity, float radius)
        {
            if (contextFolderName[contextFolderName.Length - 1] != '/')
            {
                contextFolderName += "/";
            }
            string path = 
                FrigidPaths.ProjectFolder.ASSETS + 
                FrigidPaths.ProjectFolder.SCRIPTABLE_OBJECTS + 
                contextFolderName + 
                FrigidPaths.ProjectFolder.MOBS +
                FrigidPaths.ProjectFolder.SPAWNING +
                spawnableName + ".asset";
            MobSpawnable mobSpawnable = AssetDatabase.LoadAssetAtPath<MobSpawnable>(path);
            if (mobSpawnable == null)
            {
                Debug.LogWarning("Could not find MobSpawnable of name " + spawnableName + ".");
                return;
            }

            for (int i = 0; i < quantity; i++)
            {
                float angleRad = Mathf.PI * 2 / quantity * i;
                // mobSpawnable.SpawnMob((Vector2)MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()) + new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius, Vector2.down);
            }
        }

        [ConsoleMethod("SpawnProjectile", "Spawn a projectile at your mouse position.")]
        public static void SpawnProjectile(string projectilePrefabName)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab " + projectilePrefabName);
            if (guids.Length > 0)
            {
                Projectile projectilePrefab = AssetDatabase.LoadAssetAtPath<Projectile>(AssetDatabase.GUIDToAssetPath(guids[0]));
                if (projectilePrefab != null)
                {
                    Projectile spawnedProjectile = FrigidInstancing.CreateInstance<Projectile>(projectilePrefab);
                    spawnedProjectile.LaunchProjectile(
                        0,
                        DamageAlignment.Neutrals,
                        () => FrigidInstancing.DestroyInstance(spawnedProjectile),
                        (Vector2)MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()),
                        Vector2.right,
                        null,
                        null,
                        null
                        );
                }
            }
        }

        [ConsoleMethod("SpawnExplosion", "Spawn a explosion at your mouse position.")]
        public static void SpawnExplosion(string explosionPrefabName)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab " + explosionPrefabName);
            if (guids.Length > 0)
            {
                Explosion explosionPrefab = AssetDatabase.LoadAssetAtPath<Explosion>(AssetDatabase.GUIDToAssetPath(guids[0]));
                if (explosionPrefab != null)
                {
                    Explosion spawnedExplosion = FrigidInstancing.CreateInstance<Explosion>(explosionPrefab);
                    spawnedExplosion.SummonExplosion(
                        0,
                        DamageAlignment.Neutrals,
                        () => FrigidInstancing.DestroyInstance(spawnedExplosion),
                        (Vector2)MainCamera.Instance.Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()),
                        0,
                        null,
                        null,
                        null
                        );
                }
            }
        }

        [ConsoleMethod("GiveItemToPlayer", "Give the player an item.")]
        public static void GiveItemToPlayer(string contextFolderName, string itemTypeFolderName, string itemStorableName)
        {
            GiveItemToPlayer(contextFolderName, itemTypeFolderName, itemStorableName, 1);
        }

        [ConsoleMethod("GiveItemToPlayer", "Give the player an item.")]
        public static void GiveItemToPlayer(string contextFolderName, string itemTypeFolderName, string itemStorableName, int quantity)
        {
            if (contextFolderName[contextFolderName.Length - 1] != '/')
            {
                contextFolderName += "/";
            }
            if (itemTypeFolderName[itemTypeFolderName.Length - 1] != '/')
            {
                itemTypeFolderName += "/";
            }
            string path = 
                FrigidPaths.ProjectFolder.ASSETS +
                FrigidPaths.ProjectFolder.SCRIPTABLE_OBJECTS + 
                contextFolderName + 
                FrigidPaths.ProjectFolder.ITEMS + 
                FrigidPaths.ProjectFolder.STORAGE + 
                FrigidPaths.ProjectFolder.STORABLES + 
                itemTypeFolderName + 
                itemStorableName + ".asset";
            ItemStorable itemStorable = (ItemStorable)AssetDatabase.LoadAssetAtPath<ItemStorable>(path);
            if (itemStorable == null)
            {
                Debug.LogWarning("Could not find ItemStorable of name " + itemStorableName + ".");
                return;
            }

            /* TODO MOBS_V2
            if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob))
            {
                List<ItemStorage> playerStorages = ItemStorage.FindStoragesUsedByMob(recentPlayerMob);
                foreach (ItemStorage playerStorage in playerStorages)
                {
                    foreach (ItemStorageGrid storageGrid in playerStorage.StorageGrids)
                    {
                        for (int x = 0; x < storageGrid.Dimensions.x; x++)
                        {
                            for (int y = 0; y < storageGrid.Dimensions.y; y++)
                            {
                                if (storageGrid.TryGetStash(new Vector2Int(x, y), out ContainerItemStash itemStash))
                                {
                                    if (itemStash.CanStackItemStorable(itemStorable) && !itemStash.IsFull)
                                    {
                                        itemStash.AddItems(itemStorable.CreateItems(quantity), itemStorable);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                Debug.LogWarning("Could not fit the item into the storage.");
            }
            else
            {
                Debug.LogWarning("Cannot find the player.");
            }
            */
        }
#endif
    }
}

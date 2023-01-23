using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class GameCamera : FrigidMonoBehaviour
    {
        private const int CAMERA_TILED_AREA_X_LENGTH_COVERAGE = 22;
        private const int CAMERA_TILED_AREA_Y_LENGTH_COVERAGE = 14;
        private const float CAMERA_SLIDE_SPEED = 10f;

        private FrigidCoroutine currentFollowMobInTiledAreaRoutine;

        protected override void OnEnable()
        {
            base.OnEnable();
            /* Redo later
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnOrderChanged += SlideAndFollow;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnAddedToPresentMobs += SlideAndFollow;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnRemovedFromPresentMobs += SlideAndFollow;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnMobAdded += SnapAndFollow;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnMobRemoved += SnapAndFollow;
            */
            TiledArea.OnFocusedTiledAreaChanged += SnapAndFollow;
            RenewFollowRoutine(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            /*
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnOrderChanged -= SlideAndFollow;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnAddedToPresentMobs -= SlideAndFollow;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnRemovedFromPresentMobs -= SlideAndFollow;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnMobAdded -= SnapAndFollow;
            Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).OnMobRemoved -= SnapAndFollow;
            */
            TiledArea.OnFocusedTiledAreaChanged -= SnapAndFollow;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void SlideAndFollow()
        {
            RenewFollowRoutine(true);
        }

        private void SlideAndFollow(Mob player)
        {
            RenewFollowRoutine(true);
        }

        private void SnapAndFollow()
        {
            RenewFollowRoutine(false);
        }
        
        private void SnapAndFollow(Mob player)
        {
            RenewFollowRoutine(false);
        }

        private void RenewFollowRoutine(bool slideToTarget)
        {
            FrigidCoroutine.Kill(this.currentFollowMobInTiledAreaRoutine);
            if (TiledArea.TryGetFocusedTiledArea(out TiledArea focusedTiledArea))
            {
                /* TODO
                if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob) && 
                    recentPlayerMob.TiledAreaOccupier.TryGetCurrentTiledArea(out TiledArea playerTiledArea) &&
                    playerTiledArea == focusedTiledArea)
                {
                    this.currentFollowMobInTiledAreaRoutine = FrigidCoroutine.Run(FollowMobInTiledAreaLoop(recentPlayerMob, focusedTiledArea, slideToTarget), this.gameObject);
                }
                */
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> FollowMobInTiledAreaLoop(Mob mob, TiledArea tiledArea, bool slideToTarget)
        {
            Vector2 extent =
                new Vector2(
                    GameConstants.UNIT_WORLD_SIZE * Mathf.Max(tiledArea.MainAreaDimensions.x - CAMERA_TILED_AREA_X_LENGTH_COVERAGE, 0) / 2f,
                    GameConstants.UNIT_WORLD_SIZE * Mathf.Max(tiledArea.MainAreaDimensions.y - CAMERA_TILED_AREA_Y_LENGTH_COVERAGE, 0) / 2f
                    );

            if (slideToTarget) 
            {
                while (true)
                {
                    Vector3 target =
                        new Vector3(
                            Mathf.Clamp(mob.AbsolutePosition.x, tiledArea.AbsoluteCenterPosition.x - extent.x, tiledArea.AbsoluteCenterPosition.x + extent.x),
                            Mathf.Clamp(mob.AbsolutePosition.y, tiledArea.AbsoluteCenterPosition.y - extent.y, tiledArea.AbsoluteCenterPosition.y + extent.y)
                            );
                    this.transform.position += (target - this.transform.position) * CAMERA_SLIDE_SPEED * Time.unscaledDeltaTime;
                    if (Vector2.Distance(target, this.transform.position) < GameConstants.SMALLEST_WORLD_SIZE) break;
                    yield return null;
                }
            }

            while (true)
            {
                Vector2 target =
                    new Vector2(
                        Mathf.Clamp(mob.AbsolutePosition.x, tiledArea.AbsoluteCenterPosition.x - extent.x, tiledArea.AbsoluteCenterPosition.x + extent.x),
                        Mathf.Clamp(mob.AbsolutePosition.y, tiledArea.AbsoluteCenterPosition.y - extent.y, tiledArea.AbsoluteCenterPosition.y + extent.y)
                        );
                this.transform.position = target;
                yield return null;
            }
        }
    }
}

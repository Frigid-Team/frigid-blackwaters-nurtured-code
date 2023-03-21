using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class GameCamera : FrigidMonoBehaviour
    {
        private const int CAMERA_TILED_AREA_X_LENGTH_COVERAGE = 22;
        private const int CAMERA_TILED_AREA_Y_LENGTH_COVERAGE = 14;
        private const float CAMERA_SLIDE_SPEED = 10f;

        private FrigidCoroutine currentFollowRoutine;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMob.OnExists += Startup;
            PlayerMob.OnUnexists += Teardown;
            TiledArea.OnFocusedTiledAreaChanged += RenewFollowRoutine;
            Startup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerMob.OnExists -= Startup;
            PlayerMob.OnUnexists -= Teardown;
            TiledArea.OnFocusedTiledAreaChanged -= RenewFollowRoutine;
            Teardown();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void Startup()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                this.transform.position = CalculateCameraPosition();
                player.OnTiledAreaChanged += RenewFollowRoutine;
                BeginFollowRoutine();
            }
        }

        private void Teardown()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                player.OnTiledAreaChanged -= RenewFollowRoutine;
                FinishFollowRoutine();
            }
        }

        private void RenewFollowRoutine(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            RenewFollowRoutine();
        }

        private void RenewFollowRoutine()
        {
            FinishFollowRoutine();
            this.transform.position = CalculateCameraPosition();
            BeginFollowRoutine();
        }

        private void BeginFollowRoutine()
        {
            if (TiledArea.TryGetFocusedTiledArea(out TiledArea focusedTiledArea) && PlayerMob.TryGet(out PlayerMob player))
            {
                if (player.TiledArea == focusedTiledArea)
                {
                    this.currentFollowRoutine = FrigidCoroutine.Run(FollowPlayerInTiledArea(), this.gameObject);
                }
            }
        }

        private void FinishFollowRoutine()
        {
            FrigidCoroutine.Kill(this.currentFollowRoutine);
        }

        private IEnumerator<FrigidCoroutine.Delay> FollowPlayerInTiledArea()
        {
            while (true)
            {
                Vector3 target = CalculateCameraPosition();
                Vector3 delta = (target - this.transform.position).normalized * Mathf.Min(FrigidCoroutine.DeltaTime * CAMERA_SLIDE_SPEED, Vector2.Distance(this.transform.position, target));

                if (delta.magnitude < GameConstants.SMALLEST_WORLD_SIZE) this.transform.position = target;
                else this.transform.position += delta;

                yield return null;
            }
        }

        private Vector3 CalculateCameraPosition()
        {
            if (TiledArea.TryGetFocusedTiledArea(out TiledArea focusedTiledArea) && PlayerMob.TryGet(out PlayerMob player) && player.TiledArea == focusedTiledArea)
            {
                Vector2 extent =
                    new Vector2(
                        GameConstants.UNIT_WORLD_SIZE * Mathf.Max(focusedTiledArea.MainAreaDimensions.x - CAMERA_TILED_AREA_X_LENGTH_COVERAGE, 0) / 2f,
                        GameConstants.UNIT_WORLD_SIZE * Mathf.Max(focusedTiledArea.MainAreaDimensions.y - CAMERA_TILED_AREA_Y_LENGTH_COVERAGE, 0) / 2f
                        );
                Vector3 target =
                    new Vector3(
                        Mathf.Clamp(player.Position.x, focusedTiledArea.CenterPosition.x - extent.x, focusedTiledArea.CenterPosition.x + extent.x),
                        Mathf.Clamp(player.Position.y, focusedTiledArea.CenterPosition.y - extent.y, focusedTiledArea.CenterPosition.y + extent.y)
                        );
                return target;
            }
            return this.transform.position;
        }
    }
}

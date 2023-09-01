using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class GameCamera : FrigidMonoBehaviour
    {
        private const float CAMERA_SLIDE_SPEED = 10f;

        [SerializeField]
        private Camera camera;
        [SerializeField]
        private Vector2 wallViewportPadding;

        private FrigidCoroutine currentFollowRoutine;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMob.OnExists += this.Startup;
            PlayerMob.OnUnexists += this.Teardown;
            TiledArea.OnFocusedAreaChanged += this.RenewFollowRoutine;
            this.Startup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerMob.OnExists -= this.Startup;
            PlayerMob.OnUnexists -= this.Teardown;
            TiledArea.OnFocusedAreaChanged -= this.RenewFollowRoutine;
            this.Teardown();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void Startup()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                this.transform.position = this.CalculateCameraPosition();
                player.OnTiledAreaChanged += this.RenewFollowRoutine;
                this.BeginFollowRoutine();
            }
        }

        private void Teardown()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                player.OnTiledAreaChanged -= this.RenewFollowRoutine;
                this.FinishFollowRoutine();
            }
        }

        private void RenewFollowRoutine(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            this.RenewFollowRoutine();
        }

        private void RenewFollowRoutine()
        {
            this.FinishFollowRoutine();
            this.transform.position = this.CalculateCameraPosition();
            this.BeginFollowRoutine();
        }

        private void BeginFollowRoutine()
        {
            this.currentFollowRoutine = FrigidCoroutine.Run(this.FollowPlayerInTiledArea(), this.gameObject);
        }

        private void FinishFollowRoutine()
        {
            FrigidCoroutine.Kill(this.currentFollowRoutine);
        }

        private IEnumerator<FrigidCoroutine.Delay> FollowPlayerInTiledArea()
        {
            while (true)
            {
                Vector3 target = this.CalculateCameraPosition();
                Vector3 delta = (target - this.transform.position).normalized * Mathf.Min(FrigidCoroutine.DeltaTime * CAMERA_SLIDE_SPEED, Vector2.Distance(this.transform.position, target));

                if (delta.magnitude < FrigidConstants.SMALLEST_WORLD_SIZE) this.transform.position = target;
                else this.transform.position += delta;

                yield return null;
            }
        }

        private Vector3 CalculateCameraPosition()
        {
            if (TiledArea.TryGetFocusedArea(out TiledArea focusedTiledArea) && PlayerMob.TryGet(out PlayerMob player) && player.TiledArea == focusedTiledArea)
            {
                Vector2 coverageSize = new Vector2(this.camera.orthographicSize * 2 * this.camera.aspect, this.camera.orthographicSize * 2);
                Vector2 viewportSize = 
                    focusedTiledArea.HasVisibleWalls ? 
                    (FrigidConstants.UNIT_WORLD_SIZE * (Vector2)focusedTiledArea.WallAreaDimensions + this.wallViewportPadding) : 
                    (FrigidConstants.UNIT_WORLD_SIZE * (Vector2)focusedTiledArea.MainAreaDimensions);
                Vector2 extent = new Vector2(Mathf.Max((viewportSize.x - coverageSize.x) / 2f, 0f), Mathf.Max((viewportSize.y - coverageSize.y) / 2f, 0f));
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

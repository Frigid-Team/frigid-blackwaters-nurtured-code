using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class GameCamera : FrigidMonoBehaviour
    {
        [SerializeField]
        private Camera camera;
        [SerializeField]
        private Vector2 wallViewportPadding;
        
        [SerializeField]
        private FollowCursorSetting followCursorSetting;
        [SerializeField]
        [ShowIfInt("followCursorSetting", 2, true)]
        private float smartDelayDuration;
        [SerializeField]
        private float cursorOffsetDistance;
        [SerializeField]
        private float movementOffsetDistance;
        [SerializeField]
        private float slideSpeed;

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
            Vector2 currentLeanOffset = Vector2.zero;
            float lastLeanTime = Time.time;

            this.transform.position = this.CalculateCameraTargetPosition(ref currentLeanOffset, ref lastLeanTime, true);

            const float MinSlideDistance = 4f / FrigidConstants.PixelsPerUnit;
            while (true)
            {
                Vector3 target = this.CalculateCameraTargetPosition(ref currentLeanOffset, ref lastLeanTime, false);
                Vector3 difference = target - this.transform.position;
                float distance = difference.magnitude;
                float distanceDelta = Mathf.Min(distance, FrigidCoroutine.DeltaTime * Mathf.Max(MinSlideDistance, distance) * this.slideSpeed);
                Vector3 delta = difference.normalized * distanceDelta;
                this.transform.position += delta;
                yield return null;
            }
        }

        private Vector3 CalculateCameraTargetPosition(ref Vector2 currentLeanOffset, ref float lastLeanTime, bool cameraPanDisabled)
        {
            if (!TiledArea.TryGetFocusedArea(out TiledArea focusedTiledArea) || !PlayerMob.TryGet(out PlayerMob player) || player.TiledArea != focusedTiledArea)
            {
                return this.transform.position;
            }

            Vector2 coverageSize = new Vector2(this.camera.orthographicSize * 2 * this.camera.aspect, this.camera.orthographicSize * 2);
            Vector2 viewportSize = 
                focusedTiledArea.HasVisibleWalls ? 
                (FrigidConstants.UnitWorldSize * (Vector2)focusedTiledArea.WallAreaDimensions + this.wallViewportPadding) : 
                (FrigidConstants.UnitWorldSize * (Vector2)focusedTiledArea.MainAreaDimensions);
            Vector2 extent = new Vector2(Mathf.Max((viewportSize.x - coverageSize.x) / 2f, 0f), Mathf.Max((viewportSize.y - coverageSize.y) / 2f, 0f));

            Vector2 targetPosition = player.Position;
            if (!cameraPanDisabled)
            {
                if (this.followCursorSetting == FollowCursorSetting.Following || this.followCursorSetting == FollowCursorSetting.SmartFollow && CharacterInput.AttackHeld)
                {
                    float doubleLeanDistance = Vector2.Distance(CharacterInput.AimWorldPosition, targetPosition);
                    doubleLeanDistance = Mathf.Min(this.cursorOffsetDistance * 2f, doubleLeanDistance);
                    currentLeanOffset = (CharacterInput.AimWorldPosition - targetPosition).normalized * doubleLeanDistance / 2f;
                    lastLeanTime = Time.time;
                }
                if (this.followCursorSetting != FollowCursorSetting.SmartFollow || Time.time < lastLeanTime + this.smartDelayDuration)
                {
                    targetPosition = targetPosition + currentLeanOffset;
                }
                targetPosition = targetPosition + CharacterInput.CurrentMovementVector.normalized * this.movementOffsetDistance * 2f;
            }
            targetPosition.x = Mathf.Clamp(targetPosition.x, focusedTiledArea.CenterPosition.x - extent.x, focusedTiledArea.CenterPosition.x + extent.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, focusedTiledArea.CenterPosition.y - extent.y, focusedTiledArea.CenterPosition.y + extent.y);
            return targetPosition;
        }

        private enum FollowCursorSetting
        {
            Disabled,
            Following,
            SmartFollow
        }
    }
}

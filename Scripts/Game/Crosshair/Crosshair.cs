using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class Crosshair : FrigidMonoBehaviourWithUpdate
    {
        [Header("Fill")]
        [SerializeField]
        private SpriteRenderer fillSpriteRenderer;
        [SerializeField]
        private List<Sprite> fillSprites;

        [Header("Point")]
        [SerializeField]
        private SpriteRenderer pointSpriteRenderer;
        [SerializeField]
        private List<Sprite> pulseSprites;
        [SerializeField]
        private float pulseDuration;

        private float lastTimePulsed;

        protected override void OnEnable()
        {
            base.OnEnable();
            CursorDisplay.Hidden.Request();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CursorDisplay.Hidden.Release();
        }

        protected override void Start()
        {
            base.Start();
            this.lastTimePulsed = 0;
        }

        protected override void Update()
        {
            base.Update();

            this.transform.position = CharacterInput.AimWorldPosition;

            float cursorFill = 1f;
            if (PlayerMob.TryGet(out PlayerMob player) && player.TryGetEquippedPiece(out MobEquipmentPiece equippedPiece))
            {
                if (equippedPiece.IsFiring)
                {
                    this.lastTimePulsed = Time.time;
                }
                cursorFill = equippedPiece.Cooldown.Progress;
            }
            this.fillSpriteRenderer.sprite = this.fillSprites[Mathf.FloorToInt((this.fillSprites.Count - 1) * cursorFill)];
            this.pointSpriteRenderer.sprite = this.pulseSprites[Mathf.FloorToInt((this.pulseSprites.Count - 1) * Mathf.Clamp01((Time.time - this.lastTimePulsed) / this.pulseDuration))];
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

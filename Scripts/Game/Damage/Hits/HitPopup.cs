using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class HitPopup : FrigidMonoBehaviour
    {
        [SerializeField]
        private TextMesh damageNumberText;
        [SerializeField]
        private SpriteRenderer modificationIconRenderer;
        [SerializeField]
        private float bounceDistance;
        [SerializeField]
        private float bounceDuration;
        [SerializeField]
        private float fadeDuration;

        public void ShowHit(HitInfo hitInfo, Action onComplete)
        {
            this.transform.position = hitInfo.HitPosition;
            if (hitInfo.Damage == 0)
            {
                this.damageNumberText.text = "";
            }
            else
            {
                this.damageNumberText.text = hitInfo.Damage.ToString();
            }

            if (hitInfo.TryGetHitModifier(out HitModifier hitModifier))
            {
                this.modificationIconRenderer.enabled = true;
                this.modificationIconRenderer.sprite = hitModifier.Modification.PopupIcon;
            }
            else
            {
                this.modificationIconRenderer.enabled = false;
            }

            DoPopup(hitInfo, onComplete);
        }

        private void DoPopup(HitInfo hitInfo, Action onComplete)
        {
            Color damageNumberColor = this.damageNumberText.color;
            Color modificationIconColor = this.modificationIconRenderer.color;

            this.damageNumberText.color = new Color(damageNumberColor.r, damageNumberColor.g, damageNumberColor.b, 1);
            this.modificationIconRenderer.color = new Color(modificationIconColor.r, modificationIconColor.g, modificationIconColor.b, 1);
            FrigidCoroutine.Run(
                TweenCoroutine.Value(
                    this.bounceDuration,
                    hitInfo.HitPosition, 
                    hitInfo.HitPosition + Vector2.up * this.bounceDistance,
                    onValueUpdated: (Vector2 position) => this.transform.position = position,
                    onComplete: () => FrigidCoroutine.Run(
                        TweenCoroutine.Value(
                            this.fadeDuration, 
                            1, 
                            0, 
                            onValueUpdated:
                            (float alpha) =>
                            {
                                this.damageNumberText.color = new Color(damageNumberColor.r, damageNumberColor.g, damageNumberColor.b, alpha);
                                this.modificationIconRenderer.color = new Color(modificationIconColor.r, modificationIconColor.g, modificationIconColor.b, alpha);
                            },
                            onComplete: onComplete
                            ), 
                        this.gameObject
                        )
                    ), 
                this.gameObject
                );
        }
    }
}

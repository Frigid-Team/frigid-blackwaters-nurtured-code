using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobOverheadPopup : FrigidMonoBehaviour
    {
        [SerializeField]
        private MeshRenderer textMeshRenderer;
        [SerializeField]
        private TextMesh numberText;
        [SerializeField]
        private SpriteRenderer iconSpriteRenderer;
        [SerializeField]
        private float bounceDistance;
        [SerializeField]
        private float bounceDuration;
        [SerializeField]
        private float fadeDuration;
        [SerializeField]
        private Vector2 textSpacing;
        [SerializeField]
        private Vector2 positionVariance;
        [Space]
        [SerializeField]
        private Sprite healIconSprite;
        [SerializeField]
        private Sprite noHealIconSprite;
        [SerializeField]
        private ColorSerializedReference healColor;
        [SerializeField]
        private Sprite damageIconSprite;
        [SerializeField]
        private Sprite noDamageIconSprite;
        [SerializeField]
        private ColorSerializedReference damageColor;

        public void ShowHeal(int heal, Action onComplete)
        {
            this.DoPopup(heal == 0 ? this.noHealIconSprite : this.healIconSprite, heal.ToString(), this.healColor.MutableValue, onComplete);
        }

        public void ShowDamage(int damage, Action onComplete)
        {
            this.DoPopup(damage == 0 ? this.noDamageIconSprite : this.damageIconSprite, damage.ToString(), this.damageColor.MutableValue, onComplete);
        }

        private void DoPopup(Sprite iconSprite, string text, Color color, Action onComplete)
        {
            this.iconSpriteRenderer.sprite = iconSprite;
            this.iconSpriteRenderer.color = new Color(color.r, color.g, color.b, 1);
            this.numberText.color = new Color(color.r, color.g, color.b, 1);
            this.numberText.text = text;

            float iconWidth = this.iconSpriteRenderer.bounds.size.x;
            float textWidth = this.textMeshRenderer.bounds.size.x;
            float totalWidth = iconWidth + textWidth + this.textSpacing.x;

            this.iconSpriteRenderer.transform.localPosition = new Vector2((-totalWidth + iconWidth) / 2, 0);
            this.numberText.transform.localPosition = new Vector2(this.iconSpriteRenderer.transform.localPosition.x + this.textSpacing.x + textWidth / 2, this.textSpacing.y);

            Vector2 localDisplayPosition = new Vector2(UnityEngine.Random.Range(-1f, 1f) * this.positionVariance.x, UnityEngine.Random.Range(-1f, 1f) * this.positionVariance.y);
            FrigidCoroutine.Run(
                Tween.Value(
                    this.bounceDuration,
                    localDisplayPosition,
                    localDisplayPosition + Vector2.up * this.bounceDistance,
                    onValueUpdated: (Vector2 position) => this.transform.localPosition = position,
                    onComplete: () => FrigidCoroutine.Run(
                        Tween.Value(
                            this.fadeDuration, 
                            1, 
                            0, 
                            onValueUpdated:
                            (float alpha) =>
                            {
                                this.numberText.color = new Color(color.r, color.g, color.b, alpha);
                                this.iconSpriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
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

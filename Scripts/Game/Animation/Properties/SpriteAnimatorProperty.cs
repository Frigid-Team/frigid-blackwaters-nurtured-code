using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SpriteAnimatorProperty : RendererAnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        [HideInInspector]
        private List<ColorSerializedReference> colors;
        [SerializeField]
        [HideInInspector]
        private List<float> hideChances;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<SpriteSerializedReference> sprites;

        public Sprite Sprite
        {
            get
            {
                return this.spriteRenderer.sprite;
            }
        }

        public ColorSerializedReference GetColorByReference(int animationIndex)
        {
            return this.colors[animationIndex];
        }

        public void SetColorByReference(int animationIndex, ColorSerializedReference color)
        {
            if (this.colors[animationIndex] != color)
            {
                FrigidEdit.RecordChanges(this);
                this.colors[animationIndex] = color;
            }
        }

        public float GetHideChance(int animationIndex)
        {
            return this.hideChances[animationIndex];
        }

        public void SetHideChance(int animationIndex, float hideChance)
        {
            if (this.hideChances[animationIndex] != hideChance)
            {
                FrigidEdit.RecordChanges(this);
                this.hideChances[animationIndex] = hideChance;
            }
        }

        public SpriteSerializedReference GetSpriteByReference(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.sprites[animationIndex][frameIndex][orientationIndex];
        }

        public void SetSpriteByReference(int animationIndex, int frameIndex, int orientationIndex, SpriteSerializedReference sprite)
        {
            if (this.sprites[animationIndex][frameIndex][orientationIndex] != sprite)
            {
                FrigidEdit.RecordChanges(this);
                this.sprites[animationIndex][frameIndex][orientationIndex] = sprite;
            }
        }

#if UNITY_EDITOR
        public void ImportSpriteSheet(int animationIndex, int frameIndex, int orientationIndex, string spriteSheetPath)
        {
            if (string.IsNullOrWhiteSpace(spriteSheetPath)) return;

            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath).OfType<Sprite>().ToArray();
            for (int frameOffset = 0; frameOffset < Mathf.Min(sprites.Length, this.Body.GetFrameCount(animationIndex) - frameIndex); frameOffset++)
            {
                this.SetSpriteByReference(
                    animationIndex,
                    frameIndex + frameOffset,
                    orientationIndex,
                    new SpriteSerializedReference(SerializedReferenceType.Custom, sprites[frameOffset], null, new List<Sprite>(), null)
                    );
            }
        }
#endif

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.spriteRenderer = FrigidEdit.AddComponent<SpriteRenderer>(this.gameObject);
            this.spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
            this.hideChances = new List<float>();
            this.colors = new List<ColorSerializedReference>();
            this.sprites = new Nested3DList<SpriteSerializedReference>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.hideChances.Add(0);
                this.colors.Add(new ColorSerializedReference());
                this.sprites.Add(new Nested2DList<SpriteSerializedReference>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.sprites[animationIndex].Add(new Nested1DList<SpriteSerializedReference>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.sprites[animationIndex][frameIndex].Add(new SpriteSerializedReference());
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.hideChances.Insert(animationIndex, 0);
            this.colors.Insert(animationIndex, new ColorSerializedReference());
            this.sprites.Insert(animationIndex, new Nested2DList<SpriteSerializedReference>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.sprites[animationIndex].Add(new Nested1DList<SpriteSerializedReference>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.sprites[animationIndex][frameIndex].Add(new SpriteSerializedReference());
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.hideChances.RemoveAt(animationIndex);
            this.colors.RemoveAt(animationIndex);
            this.sprites.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sprites[animationIndex].Insert(frameIndex, new Nested1DList<SpriteSerializedReference>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.sprites[animationIndex][frameIndex].Add(new SpriteSerializedReference());
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sprites[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sprites[animationIndex][frameIndex].Insert(orientationIndex, new SpriteSerializedReference());
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sprites[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            SpriteAnimatorProperty otherSpriteProperty = otherProperty as SpriteAnimatorProperty;
            if (otherSpriteProperty)
            {
                otherSpriteProperty.SetColorByReference(toAnimationIndex, this.GetColorByReference(fromAnimationIndex));
                otherSpriteProperty.SetHideChance(toAnimationIndex, this.GetHideChance(fromAnimationIndex));
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            SpriteAnimatorProperty otherSpriteProperty = otherProperty as SpriteAnimatorProperty;
            if (otherSpriteProperty)
            {
                otherSpriteProperty.SetSpriteByReference(toAnimationIndex, toFrameIndex, toOrientationIndex, new SpriteSerializedReference(this.GetSpriteByReference(fromAnimationIndex, fromFrameIndex, fromOrientationIndex)));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.spriteRenderer.enabled = false;
        }

        public override void Enable(bool enabled)
        {
            base.Enable(enabled);
            this.spriteRenderer.enabled = enabled;
        }

        public override void AnimationEnter()
        {
            this.spriteRenderer.color = this.GetColorByReference(this.Body.CurrAnimationIndex).MutableValue;
            if (!this.Body.Previewing)
            {
                this.spriteRenderer.enabled = this.GetHideChance(this.Body.CurrAnimationIndex) < Random.Range(0f, 1f);
            }
            base.AnimationEnter();
        }

        public override void OrientationEnter()
        {
            this.spriteRenderer.sprite = this.GetSpriteByReference(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex).MutableValue;
            base.OrientationEnter();
        }

        protected override Renderer Renderer
        {
            get
            {
                return this.spriteRenderer;
            }
        }
    }
}

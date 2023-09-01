#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(SpriteAnimatorProperty))]
    public class SpriteAnimatorToolPropertyDrawer : RendererAnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Sprite";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#cc00ff", out Color color);
                return color;
            }
        }

        public override void DrawAnimationEditFields(int animationIndex)
        {
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            spriteProperty.SetColorByReference(animationIndex, CoreGUILayout.ColorSerializedReferenceField("Color", spriteProperty.GetColorByReference(animationIndex)));
            spriteProperty.SetHideChance(animationIndex, EditorGUILayout.FloatField("Hide Chance (0 - 1)", spriteProperty.GetHideChance(animationIndex)));
            base.DrawAnimationEditFields(animationIndex);
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            spriteProperty.SetSpriteByReference(
                animationIndex,
                frameIndex,
                orientationIndex,
                CoreGUILayout.ObjectSerializedReferenceField<SpriteSerializedReference, Sprite>("Sprite", spriteProperty.GetSpriteByReference(animationIndex, frameIndex, orientationIndex))
                );
            Object[] selected = UtilityGUILayout.DragAndDropBox("Drag Spritesheets Onto To Import", GUILayoutUtility.GetLastRect().width, EditorGUIUtility.singleLineHeight * 3, false, typeof(Texture2D));
            if (selected.Length > 0)
            {
                spriteProperty.ImportSpriteSheet(animationIndex, frameIndex, orientationIndex, AssetDatabase.GetAssetPath(selected[0]));
            }
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawPreview(Vector2 previewSize, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected)
        {
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            Sprite sprite = spriteProperty.GetSpriteByReference(animationIndex, frameIndex, orientationIndex).ImmutableValue;
            if (sprite != null)
            {
                Vector2 adjustedSize = sprite.rect.size / FrigidConstants.PIXELS_PER_UNIT * worldToWindowScalingFactor;
                Vector2 pivotOffset = adjustedSize * (Vector2.one - new Vector2(sprite.rect.size.x - sprite.pivot.x, sprite.pivot.y) / sprite.rect.size);
                Rect spriteDrawRect = new Rect(previewSize / 2 - pivotOffset, adjustedSize);
                using (new UtilityGUI.ColorScope(spriteProperty.GetColorByReference(animationIndex).ImmutableValue))
                {
                    UtilityGUI.DrawSprite(spriteDrawRect, sprite);
                }
            }
            base.DrawPreview(previewSize, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected);
        }

        public override void DrawOrientationCellPreview(Vector2 cellSize, int animationIndex, int frameIndex, int orientationIndex)
        {
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            Sprite sprite = spriteProperty.GetSpriteByReference(animationIndex, frameIndex, orientationIndex).ImmutableValue;

            if (sprite != null)
            {
                Rect spriteDrawRect = new Rect(Vector2.zero, cellSize);
                if (sprite.rect.width > sprite.rect.height)
                {
                    float heightDelta = (spriteDrawRect.height - (spriteDrawRect.width * sprite.rect.height / sprite.rect.width)) / 2;
                    spriteDrawRect.yMin += heightDelta;
                    spriteDrawRect.yMax -= heightDelta;
                }
                else
                {
                    float widthDelta = (spriteDrawRect.width - (spriteDrawRect.height * sprite.rect.width / sprite.rect.height)) / 2;
                    spriteDrawRect.xMin += widthDelta;
                    spriteDrawRect.xMax -= widthDelta;
                }
                using (new UtilityGUI.ColorScope(spriteProperty.GetColorByReference(animationIndex).ImmutableValue))
                {
                    UtilityGUI.DrawSprite(spriteDrawRect, sprite);
                }
            }

            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

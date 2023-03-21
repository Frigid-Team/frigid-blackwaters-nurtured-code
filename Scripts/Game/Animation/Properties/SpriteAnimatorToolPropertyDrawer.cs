#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(SpriteAnimatorProperty))]
    public class SpriteAnimatorToolPropertyDrawer : SortingOrderedAnimatorToolPropertyDrawer
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

        public override void DrawGeneralEditFields()
        {
            base.DrawGeneralEditFields();
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            spriteProperty.UseSharedMaterial = EditorGUILayout.Toggle("Use Shared Material", spriteProperty.UseSharedMaterial);
            if (!spriteProperty.UseSharedMaterial)
            {
                spriteProperty.OriginalMaterial = (Material)EditorGUILayout.ObjectField("Material", spriteProperty.OriginalMaterial, typeof(Material), false);
            }
        }

        public override void DrawAnimationEditFields(int animationIndex)
        {
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            if (spriteProperty.UseSharedMaterial)
            {
                spriteProperty.SetSharedMaterial(animationIndex, (Material)EditorGUILayout.ObjectField("Shared Material", spriteProperty.GetSharedMaterial(animationIndex), typeof(Material), false));
            }
            else
            {
                spriteProperty.SetEnableOutline(animationIndex, EditorGUILayout.Toggle("Enable Outline", spriteProperty.GetEnableOutline(animationIndex)));
            }
            spriteProperty.SetColorByReference(animationIndex, Core.GUILayoutHelper.ColorSerializedReferenceField("Color", spriteProperty.GetColorByReference(animationIndex)));
            spriteProperty.SetHideChance(animationIndex, EditorGUILayout.FloatField("Hide Chance (0 - 1)", spriteProperty.GetHideChance(animationIndex)));
            base.DrawAnimationEditFields(animationIndex);
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            EditorGUILayout.LabelField("Material Tweens In Frame", GUIStyling.WordWrapAndCenter(EditorStyles.label));
            Utility.GUILayoutHelper.DrawIndexedList(
                spriteProperty.GetNumberMaterialTweensInFrame(animationIndex, frameIndex),
                (int index) => spriteProperty.AddMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index),
                (int index) => spriteProperty.RemoveMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index),
                (int index) =>
                {
                    MaterialTweenCoroutineTemplateSerializedReference materialTween = Core.GUILayoutHelper.MaterialTweenTemplateSerializedReferenceField(string.Format("Tween [{0}]", index), spriteProperty.GetMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index));
                    spriteProperty.SetMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index, materialTween);
                }
                );
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            spriteProperty.SetSpriteByReference(
                animationIndex,
                frameIndex,
                orientationIndex,
                Core.GUILayoutHelper.ObjectSerializedReferenceField<SpriteSerializedReference, Sprite>("Sprite", spriteProperty.GetSpriteByReference(animationIndex, frameIndex, orientationIndex))
                );
            spriteProperty.SetLocalOffset(
                animationIndex,
                frameIndex,
                orientationIndex,
                EditorGUILayout.Vector2Field("Local Offset", spriteProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex))
                );
            UnityEngine.Object[] selected = Utility.GUILayoutHelper.DragAndDropBox("Drag Spritesheets Onto To Import", GUILayoutUtility.GetLastRect().width, EditorGUIUtility.singleLineHeight * 3, false, typeof(Texture2D));
            if (selected.Length > 0)
            {
                spriteProperty.ImportSpriteSheet(animationIndex, orientationIndex, AssetDatabase.GetAssetPath(selected[0]));
            }
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override Vector2 DrawPreview(
            Vector2 previewSize,
            Vector2 previewOffset, 
            float worldToWindowScalingFactor,
            int animationIndex, 
            int frameIndex, 
            int orientationIndex,
            bool propertySelected,
            out List<(Rect rect, Action onDrag)> dragRequests
            )
        {
            SpriteAnimatorProperty spriteProperty = (SpriteAnimatorProperty)this.Property;
            Sprite sprite = spriteProperty.GetSpriteByReference(animationIndex, frameIndex, orientationIndex).ImmutableValue;
            dragRequests = new List<(Rect rect, Action onDrag)>();
            Vector2 localPreviewOffset = spriteProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex) * new Vector2(1, -1) * worldToWindowScalingFactor;
            if (sprite != null)
            {
                Vector2 adjustedSize = sprite.rect.size / GameConstants.PIXELS_PER_UNIT * worldToWindowScalingFactor;
                Vector2 pivotOffset = adjustedSize * (Vector2.one - new Vector2(sprite.rect.size.x - sprite.pivot.x, sprite.pivot.y) / sprite.rect.size);
                Vector2 adjustedPosition = previewSize / 2 - pivotOffset + localPreviewOffset + previewOffset;
                Rect spriteDrawRect = new Rect(adjustedPosition, adjustedSize);
                using (new GUIHelper.ColorScope(spriteProperty.RendererColor))
                {
                    GUIHelper.DrawSprite(spriteDrawRect, sprite);
                }

                if (propertySelected)
                {
                    using (new GUIHelper.ColorScope(this.AccentColor))
                    {
                        GUIHelper.DrawLineBox(spriteDrawRect);
                        Vector2 pivotBoxSize = new Vector2(this.Config.HandleLength, this.Config.HandleLength);
                        GUIHelper.DrawLineBox(spriteDrawRect);
                    }
                    dragRequests.Add(
                        (spriteDrawRect,
                        () =>
                        {
                            Vector2 newLocalOffset = spriteProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor;
                            spriteProperty.SetLocalOffset(animationIndex, frameIndex, orientationIndex, newLocalOffset);
                        })
                        );
                }
            }
            return localPreviewOffset;
        }

        public override void DrawOrientationCellPreview(
            Vector2 cellSize,
            int animationIndex,
            int frameIndex,
            int orientationIndex
            )
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
                using (new GUIHelper.ColorScope(spriteProperty.RendererColor))
                {
                    GUIHelper.DrawSprite(spriteDrawRect, sprite);
                }
            }

            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

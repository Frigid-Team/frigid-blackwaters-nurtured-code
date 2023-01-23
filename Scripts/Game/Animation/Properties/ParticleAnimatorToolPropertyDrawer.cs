#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(ParticleAnimatorProperty))]
    public class ParticleAnimatorToolPropertyDrawer : SortingOrderedAnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Particle";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#ff33cc", out Color color);
                return color;
            }
        }

        public override float[] CalculateChildPreviewOrders(int animationIndex, int frameIndex, int orientationIndex)
        {
            return base.CalculateChildPreviewOrders(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawGeneralEditFields()
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            particleProperty.Loop = EditorGUILayout.Toggle("Is Looped Particle", particleProperty.Loop);
            base.DrawGeneralEditFields();
        }

        public override void DrawAnimationEditFields(int animationIndex)
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            if (particleProperty.Loop)
            {
                particleProperty.SetIsPlayedInAnimation(animationIndex, EditorGUILayout.Toggle("Is Played This Animation", particleProperty.GetIsPlayedInAnimation(animationIndex)));
            }
            base.DrawAnimationEditFields(animationIndex);
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            if (!particleProperty.Loop)
            {
                particleProperty.SetPlayThisFrame(animationIndex, frameIndex, EditorGUILayout.Toggle("Play This Frame", particleProperty.GetPlayThisFrame(animationIndex, frameIndex)));
            }
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawPreview(Vector2 previewSize, Vector2 previewOffset, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected, out List<(Rect rect, Action onDrag)> dragRequests)
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            dragRequests = new List<(Rect rect, Action onDrag)>();

            Vector2 particleDrawPosition = previewSize / 2 + particleProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex) * new Vector2(1, -1) * worldToWindowScalingFactor + previewOffset;
            (Vector2 position, float radius)[] draws = new (Vector2 position, float radius)[9];
            draws[0] = (particleDrawPosition, this.Config.HandleLength);
            for (int i = 0; i < 3; i++)
            {
                float angleRad = (particleProperty.GetLocalRotationDeg(animationIndex, frameIndex, orientationIndex) - 45f + 45f * i) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * new Vector2(1, -1);
                draws[i + 1] = (particleDrawPosition + direction * this.Config.HandleGrabLength / 2, this.Config.HandleLength / 1.25f);
            }
            for (int i = 0; i < 5; i++)
            {
                float angleRad = (particleProperty.GetLocalRotationDeg(animationIndex, frameIndex, orientationIndex) - 45f + 22.5f * i) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * new Vector2(1, -1);
                draws[i + 4] = (particleDrawPosition + direction * this.Config.HandleGrabLength, this.Config.HandleLength / 2.5f);
            }
            using (new GUIHelper.ColorScope(propertySelected ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
            {
                Vector2 grabSize = new Vector2(this.Config.HandleGrabLength, this.Config.HandleGrabLength);
                foreach ((Vector2 position, float radius) draw in draws)
                {
                    GUIHelper.DrawSolidArc(draw.position, 0, 2 * Mathf.PI, draw.radius / 2);
                    if (propertySelected)
                    {
                        dragRequests.Add(
                            (new Rect(draw.position - grabSize / 2, grabSize),
                            () =>
                            {
                                Vector2 newLocalOffset = particleProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor;
                                particleProperty.SetLocalOffset(animationIndex, frameIndex, orientationIndex, newLocalOffset);
                            })
                            );
                    }
                }
            }

            base.DrawPreview(previewSize, previewOffset, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected, out List<(Rect rect, Action onDrag)> baseDragRequests);
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            particleProperty.SetLocalOffset(animationIndex, frameIndex, orientationIndex, EditorGUILayout.Vector2Field("Local Offset", particleProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex)));
            particleProperty.SetLocalRotationDeg(animationIndex, frameIndex, orientationIndex, EditorGUILayout.Slider("Local Rotation", particleProperty.GetLocalRotationDeg(animationIndex, frameIndex, orientationIndex), 0, 360));
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            if (!particleProperty.Loop && particleProperty.GetPlayThisFrame(animationIndex, frameIndex))
            {
                using (new GUIHelper.ColorScope(this.AccentColor))
                {
                    GUI.DrawTexture(new Rect(cellSize / 2, cellSize), this.Config.CellPreviewDiamondTexture);
                }
            }
            base.DrawFrameCellPreview(cellSize, animationIndex, frameIndex);
        }

        public override void DrawOrientationCellPreview(Vector2 cellSize, int animationIndex, int frameIndex, int orientationIndex)
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            float drawSeparationDistance = Mathf.Max(cellSize.x, cellSize.y) / 6;
            (Vector2 position, float radius)[] draws = new (Vector2 position, float radius)[9];
            draws[0] = (cellSize / 2, drawSeparationDistance);
            for (int i = 0; i < 3; i++)
            {
                float angleRad = (particleProperty.GetLocalRotationDeg(animationIndex, frameIndex, orientationIndex) - 45f + 45f * i) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * new Vector2(1, -1);
                draws[i + 1] = (cellSize / 2 + direction * drawSeparationDistance, drawSeparationDistance / 2);
            }
            for (int i = 0; i < 5; i++)
            {
                float angleRad = (particleProperty.GetLocalRotationDeg(animationIndex, frameIndex, orientationIndex) - 45f + 22.5f * i) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * new Vector2(1, -1);
                draws[i + 4] = (cellSize / 2 + direction * drawSeparationDistance * 2, drawSeparationDistance / 4);
            }
            using (new GUIHelper.ColorScope(this.AccentColor))
            {
                foreach ((Vector2 position, float radius) draw in draws)
                {
                    GUIHelper.DrawSolidArc(draw.position, 0, 2 * Mathf.PI, draw.radius / 2);
                }
            }
            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

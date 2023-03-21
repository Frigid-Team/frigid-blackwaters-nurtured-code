#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.Collections.Generic;
using System;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(LightAnimatorProperty))]
    public class LightAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Light";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#ffff99", out Color color);
                return color;
            }
        }

        public override void DrawGeneralEditFields()
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;
            lightProperty.LightType = (Light2D.LightType)EditorGUILayout.EnumPopup("Light Type", lightProperty.LightType);
            if (lightProperty.LightType == Light2D.LightType.Freeform)
            {
                lightProperty.NumberFreeformPoints = EditorGUILayout.IntField("Number Freeform Points", lightProperty.NumberFreeformPoints);
            }
            base.DrawGeneralEditFields();
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;
            lightProperty.SetLightColorByReference(animationIndex, frameIndex, Core.GUILayoutHelper.ColorSerializedReferenceField("Light Color", lightProperty.GetLightColorByReference(animationIndex, frameIndex)));
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;
            lightProperty.SetLocalOffset(animationIndex, frameIndex, orientationIndex, EditorGUILayout.Vector2Field("Local Offset", lightProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex)));
            switch (lightProperty.LightType)
            {
                case Light2D.LightType.Point:
                    lightProperty.SetInnerRadius(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.FloatField("Inner Radius", lightProperty.GetInnerRadius(animationIndex, frameIndex, orientationIndex))
                        );
                    lightProperty.SetOuterRadius(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.FloatField("Outer Radius", lightProperty.GetOuterRadius(animationIndex, frameIndex, orientationIndex))
                        );
                    lightProperty.SetNormalAngleRad(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.Slider("Normal Angle", lightProperty.GetNormalAngleRad(animationIndex, frameIndex, orientationIndex) * Mathf.Rad2Deg, 0, 360) * Mathf.Deg2Rad
                        );
                    lightProperty.SetInnerAngleRad(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.Slider("Inner Angle", lightProperty.GetInnerAngleRad(animationIndex, frameIndex, orientationIndex) * Mathf.Rad2Deg, 0, lightProperty.GetOuterAngleRad(animationIndex, frameIndex, orientationIndex) * Mathf.Rad2Deg) * Mathf.Deg2Rad
                        );
                    lightProperty.SetOuterAngleRad(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.Slider("Outer Angle", lightProperty.GetOuterAngleRad(animationIndex, frameIndex, orientationIndex) * Mathf.Rad2Deg, lightProperty.GetInnerAngleRad(animationIndex, frameIndex, orientationIndex) * Mathf.Rad2Deg, 360) * Mathf.Deg2Rad
                        );
                    break;
                case Light2D.LightType.Freeform:
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        lightProperty.NumberFreeformPoints = EditorGUILayout.IntField("Number Freeform Points", lightProperty.NumberFreeformPoints);
                        EditorGUILayout.LabelField("Points", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
                    }
                    for (int pointIndex = 0; pointIndex < lightProperty.NumberFreeformPoints; pointIndex++)
                    {
                        lightProperty.SetFreeformPointAt(animationIndex, pointIndex, EditorGUILayout.Vector2Field("", lightProperty.GetFreeformPointAt(animationIndex, pointIndex)));
                    }
                    break;
            }
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override Vector2 DrawPreview(Vector2 previewSize, Vector2 previewOffset, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected, out List<(Rect rect, Action onDrag)> dragRequests)
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;

            dragRequests = new List<(Rect rect, Action onDrag)>();

            Vector2 handleSize = new Vector2(this.Config.HandleLength, this.Config.HandleLength);
            Vector2 grabSize = new Vector2(this.Config.HandleGrabLength, this.Config.HandleGrabLength);

            Vector2 localPreviewOffset = lightProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor * new Vector2(1, -1);
            switch (lightProperty.LightType)
            {
                case Light2D.LightType.Point:
                    Vector2 circleDrawCenter = previewSize / 2 + localPreviewOffset + previewOffset;
                    float outerCircleDrawRadius = lightProperty.GetOuterRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;
                    float innerCircleDrawRadius = lightProperty.GetInnerRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;

                    using (new GUIHelper.ColorScope(propertySelected ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                    {
                        GUIHelper.DrawLineArc(circleDrawCenter, 0, Mathf.PI * 2, outerCircleDrawRadius);
                        GUIHelper.DrawLineArc(circleDrawCenter, 0, Mathf.PI * 2, innerCircleDrawRadius);
                    }

                    if (propertySelected)
                    {
                        Vector2 closestOuterPoint = Geometry.FindNearestPointOnCircle(circleDrawCenter, outerCircleDrawRadius, Event.current.mousePosition);
                        Vector2 closestInnerPoint = Geometry.FindNearestPointOnCircle(circleDrawCenter, innerCircleDrawRadius, Event.current.mousePosition);
                        float outerDistanceToMouse = Vector2.Distance(closestOuterPoint, Event.current.mousePosition);
                        float innerDistanceToMouse = Vector2.Distance(closestInnerPoint, Event.current.mousePosition);
                        if (outerDistanceToMouse < innerDistanceToMouse)
                        {
                            Rect grabRect = new Rect(closestOuterPoint - grabSize / 2, grabSize);
                            Rect handleRect = new Rect(closestOuterPoint - handleSize / 2, handleSize);
                            if (outerDistanceToMouse < this.Config.HandleGrabLength / 2)
                            {
                                using (new GUIHelper.ColorScope(this.AccentColor))
                                {
                                    GUIHelper.DrawSolidBox(handleRect);
                                }
                            }
                            dragRequests.Add((grabRect, () => lightProperty.SetOuterRadius(animationIndex, frameIndex, orientationIndex, ((Event.current.mousePosition - circleDrawCenter) * new Vector2(1, -1)).magnitude / worldToWindowScalingFactor)));
                        }
                        else
                        {
                            Rect grabRect = new Rect(closestInnerPoint - grabSize / 2, grabSize);
                            Rect handleRect = new Rect(closestInnerPoint - handleSize / 2, handleSize);
                            if (innerDistanceToMouse < this.Config.HandleGrabLength / 2)
                            {
                                using (new GUIHelper.ColorScope(this.AccentColor))
                                {
                                    GUIHelper.DrawSolidBox(handleRect);
                                }
                            }

                            dragRequests.Add((grabRect, () => lightProperty.SetInnerRadius(animationIndex, frameIndex, orientationIndex, ((Event.current.mousePosition - circleDrawCenter) * new Vector2(1, -1)).magnitude / worldToWindowScalingFactor)));
                        }
                        dragRequests.Add((new Rect(Vector2.zero, previewSize), () => lightProperty.SetLocalOffset(animationIndex, frameIndex, orientationIndex, lightProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor)));
                    }

                    float arcDrawRadius = lightProperty.GetOuterRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor + this.Config.HandleGrabLength / 2;
                    float normalAngleRad = lightProperty.GetNormalAngleRad(animationIndex, frameIndex, orientationIndex);
                    float innerHalfAngleRad = lightProperty.GetInnerAngleRad(animationIndex, frameIndex, orientationIndex) / 2;
                    float outerHalfAngleRad = lightProperty.GetOuterAngleRad(animationIndex, frameIndex, orientationIndex) / 2;

                    Vector2 innerHandlePosMin = circleDrawCenter + new Vector2(Mathf.Cos(-normalAngleRad - innerHalfAngleRad), Mathf.Sin(-normalAngleRad - innerHalfAngleRad)) * arcDrawRadius;
                    Vector2 innerHandlePosMax = circleDrawCenter + new Vector2(Mathf.Cos(-normalAngleRad + innerHalfAngleRad), Mathf.Sin(-normalAngleRad + innerHalfAngleRad)) * arcDrawRadius;
                    Vector2 outerHandlePosMin = circleDrawCenter + new Vector2(Mathf.Cos(-normalAngleRad - outerHalfAngleRad), Mathf.Sin(-normalAngleRad - outerHalfAngleRad)) * arcDrawRadius;
                    Vector2 outerHandlePosMax = circleDrawCenter + new Vector2(Mathf.Cos(-normalAngleRad + outerHalfAngleRad), Mathf.Sin(-normalAngleRad + outerHalfAngleRad)) * arcDrawRadius;

                    using (new GUIHelper.ColorScope(propertySelected ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                    {
                        GUIHelper.DrawLine(circleDrawCenter, innerHandlePosMin);
                        GUIHelper.DrawLine(circleDrawCenter, innerHandlePosMax);
                        GUIHelper.DrawLine(circleDrawCenter, outerHandlePosMin);
                        GUIHelper.DrawLine(circleDrawCenter, outerHandlePosMax);

                        GUIHelper.DrawLineArc(circleDrawCenter, -normalAngleRad - outerHalfAngleRad, -normalAngleRad + outerHalfAngleRad, arcDrawRadius);
                    }
                    break;
                case Light2D.LightType.Freeform:
                    Vector2[] drawPoints = new Vector2[lightProperty.NumberFreeformPoints];
                    for (int pointIndex = 0; pointIndex < lightProperty.NumberFreeformPoints; pointIndex++)
                    {
                        drawPoints[pointIndex] = localPreviewOffset + lightProperty.GetFreeformPointAt(animationIndex, pointIndex) * worldToWindowScalingFactor * new Vector2(1, -1) + previewSize / 2 + previewOffset;
                    }
                    using (new GUIHelper.ColorScope(this.AccentColor))
                    {
                        GUIHelper.DrawLinePolygon(drawPoints);
                    }

                    if (propertySelected)
                    {
                        for (int pointIndex = 0; pointIndex < lightProperty.NumberFreeformPoints; pointIndex++)
                        {
                            Rect handleRect = new Rect(drawPoints[pointIndex] - handleSize / 2, handleSize);
                            Rect grabRect = new Rect(drawPoints[pointIndex] - grabSize / 2, grabSize);
                            using (new GUIHelper.ColorScope(grabRect.Contains(Event.current.mousePosition) ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                            {
                                GUIHelper.DrawSolidBox(handleRect);
                            }
                            int savedPointIndex = pointIndex;
                            dragRequests.Add((grabRect, () => lightProperty.SetFreeformPointAt(animationIndex, savedPointIndex, lightProperty.GetFreeformPointAt(animationIndex, savedPointIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor)));
                        }
                    }
                    break;
            }
            return localPreviewOffset;
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;

            using (new GUIHelper.ColorScope(lightProperty.GetLightColorByReference(animationIndex, frameIndex).ImmutableValue))
            {
                GUI.DrawTexture(new Rect(Vector2.zero, cellSize), this.Config.CellPreviewSquareTexture);
            }
            
            base.DrawFrameCellPreview(cellSize, animationIndex, frameIndex);
        }

        public override void DrawOrientationCellPreview(Vector2 cellSize, int animationIndex, int frameIndex, int orientationIndex)
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;

            Vector2 drawSize = new Vector2(cellSize.x - this.Config.CellPreviewPadding * 2, cellSize.y - this.Config.CellPreviewPadding * 2);

            switch (lightProperty.LightType)
            {
                case Light2D.LightType.Point:
                    float outerCircleDrawRadius = Mathf.Min(drawSize.x, drawSize.y) / 2;
                    float innerCircleDrawRadius = outerCircleDrawRadius * lightProperty.GetInnerRadius(animationIndex, frameIndex, orientationIndex) / lightProperty.GetOuterRadius(animationIndex, frameIndex, orientationIndex);

                    using (new GUIHelper.ColorScope(this.AccentColor))
                    {
                        GUIHelper.DrawLineArc(cellSize / 2, 0, Mathf.PI * 2, outerCircleDrawRadius);
                        GUIHelper.DrawLineArc(cellSize / 2, 0, Mathf.PI * 2, innerCircleDrawRadius);

                        float normalAngleRad = lightProperty.GetNormalAngleRad(animationIndex, frameIndex, orientationIndex);
                        float innerHalfAngleRad = lightProperty.GetInnerAngleRad(animationIndex, frameIndex, orientationIndex) / 2;
                        float outerHalfAngleRad = lightProperty.GetOuterAngleRad(animationIndex, frameIndex, orientationIndex) / 2;

                        Vector2 innerHandlePosMin = cellSize / 2 + new Vector2(Mathf.Cos(-normalAngleRad - innerHalfAngleRad), Mathf.Sin(-normalAngleRad - innerHalfAngleRad)) * outerCircleDrawRadius;
                        Vector2 innerHandlePosMax = cellSize / 2 + new Vector2(Mathf.Cos(-normalAngleRad + innerHalfAngleRad), Mathf.Sin(-normalAngleRad + innerHalfAngleRad)) * outerCircleDrawRadius;
                        Vector2 outerHandlePosMin = cellSize / 2 + new Vector2(Mathf.Cos(-normalAngleRad - outerHalfAngleRad), Mathf.Sin(-normalAngleRad - outerHalfAngleRad)) * outerCircleDrawRadius;
                        Vector2 outerHandlePosMax = cellSize / 2 + new Vector2(Mathf.Cos(-normalAngleRad + outerHalfAngleRad), Mathf.Sin(-normalAngleRad + outerHalfAngleRad)) * outerCircleDrawRadius;

                        GUIHelper.DrawLine(cellSize / 2, innerHandlePosMin);
                        GUIHelper.DrawLine(cellSize / 2, innerHandlePosMax);
                        GUIHelper.DrawLine(cellSize / 2, outerHandlePosMin);
                        GUIHelper.DrawLine(cellSize / 2, outerHandlePosMax);
                    }
                    break;
                case Light2D.LightType.Freeform:
                    if (lightProperty.NumberFreeformPoints > 0)
                    {
                        Vector2[] drawPoints = new Vector2[lightProperty.NumberFreeformPoints];
                        Bounds bounds = new Bounds(Vector2.zero, Vector2.zero);
                        for (int pointIndex = 0; pointIndex < drawPoints.Length; pointIndex++)
                        {
                            drawPoints[pointIndex] = lightProperty.GetFreeformPointAt(animationIndex, pointIndex) * new Vector2(1, -1);
                            bounds.Encapsulate(drawPoints[pointIndex]);
                        }
                        float scalingFactor;
                        if (bounds.size.x > bounds.size.y)
                        {
                            scalingFactor = drawSize.x / bounds.size.x;
                        }
                        else
                        {
                            scalingFactor = drawSize.y / bounds.size.y;
                        }
                        for (int pointIndex = 0; pointIndex < drawPoints.Length; pointIndex++)
                        {
                            drawPoints[pointIndex] = cellSize / 2 + (drawPoints[pointIndex] - (Vector2)bounds.center) * scalingFactor;
                        }

                        using (new GUIHelper.ColorScope(this.AccentColor))
                        {
                            GUIHelper.DrawLinePolygon(drawPoints);
                        }
                    }
                    break;
            }
            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

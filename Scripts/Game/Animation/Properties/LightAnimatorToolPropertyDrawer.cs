#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;

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
            lightProperty.SetLightColorByReference(animationIndex, frameIndex, CoreGUILayout.ColorSerializedReferenceField("Light Color", lightProperty.GetLightColorByReference(animationIndex, frameIndex)));
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;
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
                        EditorGUILayout.LabelField("Points", UtilityStyles.WordWrapAndCenter(EditorStyles.boldLabel));
                    }
                    for (int pointIndex = 0; pointIndex < lightProperty.NumberFreeformPoints; pointIndex++)
                    {
                        lightProperty.SetFreeformPointAt(animationIndex, pointIndex, EditorGUILayout.Vector2Field("", lightProperty.GetFreeformPointAt(animationIndex, pointIndex)));
                    }
                    break;
            }
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawPreview(Vector2 previewSize, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected)
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;

            Vector2 handleSize = new Vector2(this.Config.HandleLength, this.Config.HandleLength);
            Vector2 grabSize = new Vector2(this.Config.HandleGrabLength, this.Config.HandleGrabLength);

            Vector2 drawPosition = previewSize / 2;
            switch (lightProperty.LightType)
            {
                case Light2D.LightType.Point:
                    float outerCircleDrawRadius = lightProperty.GetOuterRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;
                    float innerCircleDrawRadius = lightProperty.GetInnerRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;

                    using (new UtilityGUI.ColorScope(propertySelected ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                    {
                        UtilityGUI.DrawLineArc(drawPosition, 0, Mathf.PI * 2, outerCircleDrawRadius);
                        UtilityGUI.DrawLineArc(drawPosition, 0, Mathf.PI * 2, innerCircleDrawRadius);
                    }

                    if (propertySelected)
                    {
                        Vector2 closestOuterPoint = Geometry.FindNearestPointOnCircle(drawPosition, outerCircleDrawRadius, Event.current.mousePosition);
                        Vector2 closestInnerPoint = Geometry.FindNearestPointOnCircle(drawPosition, innerCircleDrawRadius, Event.current.mousePosition);
                        float outerDistanceToMouse = Vector2.Distance(closestOuterPoint, Event.current.mousePosition);
                        float innerDistanceToMouse = Vector2.Distance(closestInnerPoint, Event.current.mousePosition);
                        if (outerDistanceToMouse < innerDistanceToMouse)
                        {
                            Rect grabRect = new Rect(closestOuterPoint - grabSize / 2, grabSize);
                            Rect handleRect = new Rect(closestOuterPoint - handleSize / 2, handleSize);
                            if (outerDistanceToMouse < this.Config.HandleGrabLength / 2)
                            {
                                using (new UtilityGUI.ColorScope(this.AccentColor))
                                {
                                    UtilityGUI.DrawSolidBox(handleRect);
                                }
                            }

                            if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && grabRect.Contains(Event.current.mousePosition - Event.current.delta))
                            {
                                lightProperty.SetOuterRadius(animationIndex, frameIndex, orientationIndex, ((Event.current.mousePosition - drawPosition) * new Vector2(1, -1)).magnitude / worldToWindowScalingFactor);
                                Event.current.Use();
                            }
                        }
                        else
                        {
                            Rect grabRect = new Rect(closestInnerPoint - grabSize / 2, grabSize);
                            Rect handleRect = new Rect(closestInnerPoint - handleSize / 2, handleSize);
                            if (innerDistanceToMouse < this.Config.HandleGrabLength / 2)
                            {
                                using (new UtilityGUI.ColorScope(this.AccentColor))
                                {
                                    UtilityGUI.DrawSolidBox(handleRect);
                                }
                            }

                            if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && grabRect.Contains(Event.current.mousePosition - Event.current.delta))
                            {
                                lightProperty.SetInnerRadius(animationIndex, frameIndex, orientationIndex, ((Event.current.mousePosition - drawPosition) * new Vector2(1, -1)).magnitude / worldToWindowScalingFactor);
                                Event.current.Use();
                            }
                        }
                    }

                    float arcDrawRadius = lightProperty.GetOuterRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor + this.Config.HandleGrabLength / 2;
                    float normalAngleRad = Mathf.PI / 2;
                    float innerHalfAngleRad = lightProperty.GetInnerAngleRad(animationIndex, frameIndex, orientationIndex) / 2;
                    float outerHalfAngleRad = lightProperty.GetOuterAngleRad(animationIndex, frameIndex, orientationIndex) / 2;

                    Vector2 innerHandlePosMin = drawPosition + new Vector2(Mathf.Cos(-normalAngleRad - innerHalfAngleRad), Mathf.Sin(-normalAngleRad - innerHalfAngleRad)) * arcDrawRadius;
                    Vector2 innerHandlePosMax = drawPosition + new Vector2(Mathf.Cos(-normalAngleRad + innerHalfAngleRad), Mathf.Sin(-normalAngleRad + innerHalfAngleRad)) * arcDrawRadius;
                    Vector2 outerHandlePosMin = drawPosition + new Vector2(Mathf.Cos(-normalAngleRad - outerHalfAngleRad), Mathf.Sin(-normalAngleRad - outerHalfAngleRad)) * arcDrawRadius;
                    Vector2 outerHandlePosMax = drawPosition + new Vector2(Mathf.Cos(-normalAngleRad + outerHalfAngleRad), Mathf.Sin(-normalAngleRad + outerHalfAngleRad)) * arcDrawRadius;

                    using (new UtilityGUI.ColorScope(propertySelected ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                    {
                        UtilityGUI.DrawLine(drawPosition, innerHandlePosMin);
                        UtilityGUI.DrawLine(drawPosition, innerHandlePosMax);
                        UtilityGUI.DrawLine(drawPosition, outerHandlePosMin);
                        UtilityGUI.DrawLine(drawPosition, outerHandlePosMax);

                        UtilityGUI.DrawLineArc(drawPosition, -normalAngleRad - outerHalfAngleRad, -normalAngleRad + outerHalfAngleRad, arcDrawRadius);
                    }
                    break;
                case Light2D.LightType.Freeform:
                    Vector2[] drawPoints = new Vector2[lightProperty.NumberFreeformPoints];
                    for (int pointIndex = 0; pointIndex < lightProperty.NumberFreeformPoints; pointIndex++)
                    {
                        drawPoints[pointIndex] = drawPosition + lightProperty.GetFreeformPointAt(animationIndex, pointIndex) * worldToWindowScalingFactor * new Vector2(1, -1);
                    }
                    using (new UtilityGUI.ColorScope(this.AccentColor))
                    {
                        UtilityGUI.DrawLinePolygon(drawPoints);
                    }

                    if (propertySelected)
                    {
                        for (int pointIndex = 0; pointIndex < lightProperty.NumberFreeformPoints; pointIndex++)
                        {
                            Rect handleRect = new Rect(drawPoints[pointIndex] - handleSize / 2, handleSize);
                            Rect grabRect = new Rect(drawPoints[pointIndex] - grabSize / 2, grabSize);
                            using (new UtilityGUI.ColorScope(grabRect.Contains(Event.current.mousePosition) ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                            {
                                UtilityGUI.DrawSolidBox(handleRect);
                            }
                            int savedPointIndex = pointIndex;

                            if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && grabRect.Contains(Event.current.mousePosition - Event.current.delta))
                            {
                                lightProperty.SetFreeformPointAt(animationIndex, savedPointIndex, lightProperty.GetFreeformPointAt(animationIndex, savedPointIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor);
                                Event.current.Use();
                            }
                        }
                    }
                    break;
            }
            base.DrawPreview(previewSize, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            LightAnimatorProperty lightProperty = (LightAnimatorProperty)this.Property;

            using (new UtilityGUI.ColorScope(lightProperty.GetLightColorByReference(animationIndex, frameIndex).ImmutableValue))
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

                    using (new UtilityGUI.ColorScope(this.AccentColor))
                    {
                        UtilityGUI.DrawLineArc(cellSize / 2, 0, Mathf.PI * 2, outerCircleDrawRadius);
                        UtilityGUI.DrawLineArc(cellSize / 2, 0, Mathf.PI * 2, innerCircleDrawRadius);

                        float normalAngleRad = Mathf.PI / 2;
                        float innerHalfAngleRad = lightProperty.GetInnerAngleRad(animationIndex, frameIndex, orientationIndex) / 2;
                        float outerHalfAngleRad = lightProperty.GetOuterAngleRad(animationIndex, frameIndex, orientationIndex) / 2;

                        Vector2 innerHandlePosMin = cellSize / 2 + new Vector2(Mathf.Cos(-normalAngleRad - innerHalfAngleRad), Mathf.Sin(-normalAngleRad - innerHalfAngleRad)) * outerCircleDrawRadius;
                        Vector2 innerHandlePosMax = cellSize / 2 + new Vector2(Mathf.Cos(-normalAngleRad + innerHalfAngleRad), Mathf.Sin(-normalAngleRad + innerHalfAngleRad)) * outerCircleDrawRadius;
                        Vector2 outerHandlePosMin = cellSize / 2 + new Vector2(Mathf.Cos(-normalAngleRad - outerHalfAngleRad), Mathf.Sin(-normalAngleRad - outerHalfAngleRad)) * outerCircleDrawRadius;
                        Vector2 outerHandlePosMax = cellSize / 2 + new Vector2(Mathf.Cos(-normalAngleRad + outerHalfAngleRad), Mathf.Sin(-normalAngleRad + outerHalfAngleRad)) * outerCircleDrawRadius;

                        UtilityGUI.DrawLine(cellSize / 2, innerHandlePosMin);
                        UtilityGUI.DrawLine(cellSize / 2, innerHandlePosMax);
                        UtilityGUI.DrawLine(cellSize / 2, outerHandlePosMin);
                        UtilityGUI.DrawLine(cellSize / 2, outerHandlePosMax);
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

                        using (new UtilityGUI.ColorScope(this.AccentColor))
                        {
                            UtilityGUI.DrawLinePolygon(drawPoints);
                        }
                    }
                    break;
            }
            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

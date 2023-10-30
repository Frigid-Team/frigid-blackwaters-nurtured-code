#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(LineAnimatorProperty))]
    public class LineAnimatorToolPropertyDrawer : RendererAnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Line";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#3b43e3", out Color color);
                return color;
            }
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            LineAnimatorProperty lineProperty = (LineAnimatorProperty)this.Property;
            lineProperty.SetLineWidth(animationIndex, frameIndex, orientationIndex, EditorGUILayout.FloatField("Line Width", lineProperty.GetLineWidth(animationIndex, frameIndex, orientationIndex)));
            UtilityGUILayout.IndexedList(
                "Points",
                lineProperty.GetNumberLinePoints(animationIndex, frameIndex, orientationIndex),
                (int pointIndex) => lineProperty.AddLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex),
                (int pointIndex) => lineProperty.RemoveLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex),
                (int pointIndex) => lineProperty.SetLinePointAt(
                    animationIndex,
                    frameIndex,
                    orientationIndex,
                    pointIndex,
                    EditorGUILayout.Vector2Field("", lineProperty.GetLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex))
                    )
                );
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawPreview(Vector2 previewSize, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected)
        {
            LineAnimatorProperty lineProperty = (LineAnimatorProperty)this.Property;
            
            Vector2[] lineDrawPoints = new Vector2[lineProperty.GetNumberLinePoints(animationIndex, frameIndex, orientationIndex)];
            if (lineDrawPoints.Length <= 1) return;

            for (int pointIndex = 0; pointIndex < lineDrawPoints.Length; pointIndex++)
            {
                lineDrawPoints[pointIndex] = previewSize / 2 + lineProperty.GetLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex) * new Vector2(1, -1) * worldToWindowScalingFactor;
            }

            float drawWidth = lineProperty.GetLineWidth(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;
            for (int pointIndex = 0; pointIndex < lineDrawPoints.Length - 1; pointIndex++)
            {
                Vector2 currLineDrawPoint = lineDrawPoints[pointIndex];
                Vector2 nextLineDrawPoint = lineDrawPoints[pointIndex + 1];
                Vector2[] polygonDrawPoints = Geometry.GetRectAlongLine(currLineDrawPoint, nextLineDrawPoint, drawWidth);

                using (new UtilityGUI.ColorScope(UtilityGUIUtility.Darken(this.AccentColor, propertySelected ? 0 : 1)))
                {
                    UtilityGUI.DrawLinePolygon(polygonDrawPoints);
                }
            }

            Vector2 handleSize = new Vector2(this.Config.HandleLength, this.Config.HandleLength);
            Vector2 grabSize = new Vector2(this.Config.HandleGrabLength, this.Config.HandleGrabLength);

            if (propertySelected)
            {
                bool drawAdd = true;
                for (int pointIndex = 0; pointIndex < lineDrawPoints.Length; pointIndex++)
                {
                    Rect handleRect = new Rect(lineDrawPoints[pointIndex] - handleSize / 2, handleSize);
                    Rect grabRect = new Rect(lineDrawPoints[pointIndex] - grabSize / 2, grabSize);

                    Color handleColor = UtilityGUIUtility.Darken(this.AccentColor);
                    if (grabRect.Contains(Event.current.mousePosition))
                    {
                        handleColor = this.AccentColor;
                        drawAdd = false;

                        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                        {
                            lineProperty.RemoveLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex);
                            break;
                        }
                    }

                    using (new UtilityGUI.ColorScope(handleColor))
                    {
                        UtilityGUI.DrawSolidBox(handleRect);
                    }

                    if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && grabRect.Contains(Event.current.mousePosition - Event.current.delta))
                    {
                        Vector2 newPolygonPoint = lineProperty.GetLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor;
                        lineProperty.SetLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex, newPolygonPoint);
                        Event.current.Use();
                    }
                }

                if (drawAdd)
                {
                    int addIndex = -1;
                    Vector2 addPoint = Vector2.zero;
                    float shortestDistance = float.MaxValue;
                    for (int pointIndex = 0; pointIndex < lineDrawPoints.Length - 1; pointIndex++)
                    {
                        Vector2 nearestPoint = Geometry.FindNearestPointOnLine(lineDrawPoints[pointIndex], lineDrawPoints[pointIndex + 1], Event.current.mousePosition);
                        float distance = Vector2.Distance(Event.current.mousePosition, nearestPoint);
                        if (Vector2.Distance(nearestPoint, Event.current.mousePosition) < this.Config.HandleGrabLength / 2 && distance < shortestDistance)
                        {
                            addIndex = pointIndex + 1;
                            addPoint = nearestPoint;
                            shortestDistance = distance;
                        }
                    }

                    if (addIndex != -1)
                    {
                        Rect handleRect = new Rect(addPoint - handleSize / 2, handleSize);
                        using (new UtilityGUI.ColorScope(this.AccentColor))
                        {
                            UtilityGUI.DrawSolidBox(handleRect);
                        }
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            lineProperty.AddLinePointAt(animationIndex, frameIndex, orientationIndex, addIndex);
                            lineProperty.SetLinePointAt(animationIndex, frameIndex, orientationIndex, addIndex, (addPoint - previewSize / 2) * new Vector2(1, -1) / worldToWindowScalingFactor);
                            Event.current.Use();
                        }
                    }
                }
            }

            base.DrawPreview(previewSize, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected);
        }

        public override void DrawOrientationCellPreview(Vector2 cellSize, int animationIndex, int frameIndex, int orientationIndex)
        {
            Vector2 drawSize = new Vector2(cellSize.x - this.Config.CellPreviewPadding * 2, cellSize.y - this.Config.CellPreviewPadding * 2);

            LineAnimatorProperty lineProperty = (LineAnimatorProperty)this.Property;

            Bounds bounds = new Bounds(Vector2.zero, Vector2.zero);
            Vector2[] linePoints = new Vector2[lineProperty.GetNumberLinePoints(animationIndex, frameIndex, orientationIndex)];
            for (int pointIndex = 0; pointIndex < linePoints.Length; pointIndex++)
            {
                linePoints[pointIndex] = lineProperty.GetLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex) * new Vector2(1, -1);
                bounds.Encapsulate(linePoints[pointIndex]);
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
            for (int pointIndex = 0; pointIndex < linePoints.Length; pointIndex++)
            {
                linePoints[pointIndex] = cellSize / 2 + (linePoints[pointIndex] - (Vector2)bounds.center) * scalingFactor;
            }

            using (new UtilityGUI.ColorScope(Color.white))
            {
                UtilityGUI.DrawLineSegments(linePoints);
            }
            
            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

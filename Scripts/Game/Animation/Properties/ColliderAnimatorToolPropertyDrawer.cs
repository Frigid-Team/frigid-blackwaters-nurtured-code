#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class ColliderAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override float[] CalculateChildPreviewOrders(int animationIndex, int frameIndex, int orientationIndex)
        {
            return new float[] { GUI_PREVIEW_ORDER };
        }

        public override void DrawGeneralEditFields()
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;
            colliderProperty.ShapeType = (ColliderAnimatorPropertyShapeType)EditorGUILayout.EnumPopup("Shape Type", colliderProperty.ShapeType);
            base.DrawGeneralEditFields();
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;
            switch (colliderProperty.ShapeType)
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
                    EditorGUILayout.LabelField("Points", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
                    Utility.GUILayoutHelper.DrawIndexedList(
                    colliderProperty.GetNumberPolygonPoints(animationIndex, frameIndex, orientationIndex),
                    (int index) => colliderProperty.AddPolygonPointAt(animationIndex, frameIndex, orientationIndex, index, Vector2.zero),
                    (int index) => colliderProperty.RemovePolygonPointAt(animationIndex, frameIndex, orientationIndex, index),
                    (int index) => colliderProperty.SetPolygonPointAt(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        index,
                        EditorGUILayout.Vector2Field("", colliderProperty.GetPolygonPointAt(animationIndex, frameIndex, orientationIndex, index))
                        )
                    );
                    break;
                case ColliderAnimatorPropertyShapeType.Circle:
                    colliderProperty.SetCenter(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.Vector2Field("Center", colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex))
                        );
                    colliderProperty.SetRadius(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.FloatField("Radius", colliderProperty.GetRadius(animationIndex, frameIndex, orientationIndex))
                        );
                    break;
                case ColliderAnimatorPropertyShapeType.Box:
                    colliderProperty.SetCenter(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.Vector2Field("Center", colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex))
                        );
                    colliderProperty.SetSize(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.Vector2Field("Size", colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex))
                        );
                    break;
                case ColliderAnimatorPropertyShapeType.Capsule:
                    colliderProperty.SetCenter(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.Vector2Field("Center", colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex))
                        );
                    colliderProperty.SetSize(
                        animationIndex,
                        frameIndex,
                        orientationIndex,
                        EditorGUILayout.Vector2Field("Size", colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex))
                        );
                    break;
            }
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawPreview(
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
            dragRequests = new List<(Rect rect, Action onDrag)>();

            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;

            Vector2 handleSize = new Vector2(this.Config.HandleLength, this.Config.HandleLength);
            Vector2 grabSize = new Vector2(this.Config.HandleGrabLength, this.Config.HandleGrabLength);

            switch (colliderProperty.ShapeType)
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
                    Vector2[] points = new Vector2[colliderProperty.GetNumberPolygonPoints(animationIndex, frameIndex, orientationIndex)];
                    for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                    {
                        points[pointIndex] = previewSize / 2 + colliderProperty.GetPolygonPointAt(animationIndex, frameIndex, orientationIndex, pointIndex) * new Vector2(1, -1) * worldToWindowScalingFactor + previewOffset;
                    }

                    using (new GUIHelper.ColorScope(propertySelected ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                    {
                        GUIHelper.DrawLinePolygon(points);
                    }

                    if (propertySelected)
                    {
                        bool drawAdd = true;
                        for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                        {
                            Rect handleRect = new Rect(points[pointIndex] - handleSize / 2, handleSize);
                            Rect grabRect = new Rect(points[pointIndex] - grabSize / 2, grabSize);

                            Color handleColor = GUIStyling.Darken(this.AccentColor);
                            if (grabRect.Contains(Event.current.mousePosition))
                            {
                                handleColor = this.AccentColor;
                                drawAdd = false;

                                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                                {
                                    colliderProperty.RemovePolygonPointAt(animationIndex, frameIndex, orientationIndex, pointIndex);
                                    break;
                                }
                            }

                            using (new GUIHelper.ColorScope(handleColor))
                            {
                                GUIHelper.DrawSolidBox(handleRect);
                            }

                            Vector2 newColliderPoint = colliderProperty.GetPolygonPointAt(animationIndex, frameIndex, orientationIndex, pointIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor;
                            int savedPointIndex = pointIndex;
                            dragRequests.Add((grabRect, () => colliderProperty.SetPolygonPointAt(animationIndex, frameIndex, orientationIndex, savedPointIndex, newColliderPoint)));
                        }

                        if (points.Length > 1 && drawAdd)
                        {
                            int addIndex = -1;
                            Vector2 addPoint = Vector2.zero;
                            float shortestDistance = float.MaxValue;
                            for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                            {
                                Vector2 nearestPoint = Geometry.FindNearestPointOnLine(points[pointIndex], points[(pointIndex + 1) % points.Length], Event.current.mousePosition);
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
                                using (new GUIHelper.ColorScope(this.AccentColor))
                                {
                                    GUIHelper.DrawSolidBox(handleRect);
                                }
                                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                                {
                                    colliderProperty.AddPolygonPointAt(
                                        animationIndex,
                                        frameIndex,
                                        orientationIndex,
                                        addIndex,
                                        (addPoint - previewSize / 2 - previewOffset) * new Vector2(1, -1) / worldToWindowScalingFactor
                                        );
                                }
                            }
                        }
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Circle:
                    Vector2 circleDrawCenter = previewSize / 2 + colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor * new Vector2(1, -1) + previewOffset;
                    float circleDrawRadius = colliderProperty.GetRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;

                    using (new GUIHelper.ColorScope(propertySelected ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                    {
                        GUIHelper.DrawLineArc(circleDrawCenter, 0, Mathf.PI * 2, circleDrawRadius);
                    }

                    if (propertySelected)
                    {
                        Vector2 closestPoint = Geometry.FindNearestPointOnCircle(circleDrawCenter, circleDrawRadius, Event.current.mousePosition);
                        float distanceToMouse = Vector2.Distance(closestPoint, Event.current.mousePosition);
                        if (distanceToMouse < this.Config.HandleGrabLength / 2)
                        {
                            Rect grabRect = new Rect(closestPoint - grabSize / 2, grabSize);
                            Rect handleRect = new Rect(closestPoint - handleSize / 2, handleSize);

                            using (new GUIHelper.ColorScope(this.AccentColor))
                            {
                                GUIHelper.DrawSolidBox(handleRect);
                            }

                            dragRequests.Add((grabRect, () => colliderProperty.SetRadius(animationIndex, frameIndex, orientationIndex, ((Event.current.mousePosition - circleDrawCenter) * new Vector2(1, -1)).magnitude / worldToWindowScalingFactor)));
                        }
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Box:
                    Vector2 boxDrawCenter = previewSize / 2 + colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor * new Vector2(1, -1) + previewOffset;
                    Vector2 boxDrawSize = colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;
                    Rect boxDrawRect = new Rect(boxDrawCenter - boxDrawSize / 2, boxDrawSize);
                    using (new GUIHelper.ColorScope(propertySelected ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                    {
                        GUIHelper.DrawLineBox(boxDrawRect);
                    }

                    if (propertySelected)
                    {
                        Vector2 topLeftCornerPos = boxDrawCenter - boxDrawSize / 2;
                        Vector2 bottomRightCornerPos = boxDrawCenter + boxDrawSize / 2;

                        Rect topLeftHandlePos = new Rect(topLeftCornerPos - handleSize / 2, handleSize);
                        Rect bottomRightHandlePos = new Rect(bottomRightCornerPos - handleSize / 2, handleSize);

                        Rect topLeftGrabPos = new Rect(topLeftCornerPos - grabSize / 2, grabSize);
                        Rect bottomRightGrabPos = new Rect(bottomRightCornerPos - grabSize / 2, grabSize);

                        using (new GUIHelper.ColorScope(topLeftGrabPos.Contains(Event.current.mousePosition) ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                        {
                            GUIHelper.DrawSolidBox(topLeftHandlePos);
                        }
                        using (new GUIHelper.ColorScope(bottomRightGrabPos.Contains(Event.current.mousePosition) ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                        {
                            GUIHelper.DrawSolidBox(bottomRightHandlePos);
                        }

                        dragRequests.Add(
                            (topLeftGrabPos, 
                            () => 
                            {
                                Vector2 worldDelta = Event.current.delta / worldToWindowScalingFactor * new Vector2(1, -1);
                                colliderProperty.SetSize(animationIndex, frameIndex, orientationIndex, colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) + worldDelta * new Vector2(-1, 1));
                                colliderProperty.SetCenter(animationIndex, frameIndex, orientationIndex, colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) + worldDelta / 2);
                            })
                            );
                        dragRequests.Add(
                            (bottomRightGrabPos, 
                            () => 
                            {
                                Vector2 worldDelta = Event.current.delta / worldToWindowScalingFactor * new Vector2(-1, 1);
                                colliderProperty.SetSize(animationIndex, frameIndex, orientationIndex, colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) + worldDelta * new Vector2(-1, 1));
                                colliderProperty.SetCenter(animationIndex, frameIndex, orientationIndex, colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) - worldDelta / 2);
                            })
                            );
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Capsule:
                    Vector2 capsuleDrawCenter = previewSize / 2 + colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor * new Vector2(1, -1) + previewOffset;
                    Vector2 capsuleDrawSize = colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;
                    Rect capsuleDrawRect = new Rect(capsuleDrawCenter - capsuleDrawSize / 2, capsuleDrawSize);
                    using (new GUIHelper.ColorScope(propertySelected ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                    {
                        GUIHelper.DrawLineCapsule(capsuleDrawRect);
                    }

                    if (propertySelected)
                    {
                        Vector2 topLeftCornerPos = capsuleDrawCenter - capsuleDrawSize / 2;
                        Vector2 bottomRightCornerPos = capsuleDrawCenter + capsuleDrawSize / 2;

                        Rect topLeftHandlePos = new Rect(topLeftCornerPos - handleSize / 2, handleSize);
                        Rect bottomRightHandlePos = new Rect(bottomRightCornerPos - handleSize / 2, handleSize);

                        Rect topLeftGrabPos = new Rect(topLeftCornerPos - grabSize / 2, grabSize);
                        Rect bottomRightGrabPos = new Rect(bottomRightCornerPos - grabSize / 2, grabSize);

                        using (new GUIHelper.ColorScope(topLeftGrabPos.Contains(Event.current.mousePosition) ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                        {
                            GUIHelper.DrawSolidBox(topLeftHandlePos);
                        }
                        using (new GUIHelper.ColorScope(bottomRightGrabPos.Contains(Event.current.mousePosition) ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
                        {
                            GUIHelper.DrawSolidBox(bottomRightHandlePos);
                        }

                        dragRequests.Add(
                            (topLeftGrabPos,
                            () =>
                            {
                                Vector2 worldDelta = Event.current.delta / worldToWindowScalingFactor * new Vector2(1, -1);
                                colliderProperty.SetSize(animationIndex, frameIndex, orientationIndex, colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) + worldDelta * new Vector2(-1, 1));
                                colliderProperty.SetCenter(animationIndex, frameIndex, orientationIndex, colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) + worldDelta / 2);
                            })
                            );
                        dragRequests.Add(
                            (bottomRightGrabPos,
                            () =>
                            {
                                Vector2 worldDelta = Event.current.delta / worldToWindowScalingFactor * new Vector2(-1, 1);
                                colliderProperty.SetSize(animationIndex, frameIndex, orientationIndex, colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) + worldDelta * new Vector2(-1, 1));
                                colliderProperty.SetCenter(animationIndex, frameIndex, orientationIndex, colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) - worldDelta / 2);
                            })
                            );
                    }
                    break;
            }
            base.DrawPreview(previewSize, previewOffset, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected, out List<(Rect rect, Action onDrag)> baseDragRequests);
        }

        public override void DrawOrientationCellPreview(
            Vector2 cellSize,
            int animationIndex,
            int frameIndex,
            int orientationIndex
            )
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;

            Vector2 drawSize = new Vector2(cellSize.x - this.Config.CellPreviewPadding * 2, cellSize.y - this.Config.CellPreviewPadding * 2);

            switch (colliderProperty.ShapeType) 
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
                    if (colliderProperty.GetNumberPolygonPoints(animationIndex, frameIndex, orientationIndex) > 0)
                    {
                        Vector2[] drawPoints = new Vector2[colliderProperty.GetNumberPolygonPoints(animationIndex, frameIndex, orientationIndex)];
                        Bounds bounds = new Bounds(Vector2.zero, Vector2.zero);
                        for (int pointIndex = 0; pointIndex < drawPoints.Length; pointIndex++)
                        {
                            drawPoints[pointIndex] = colliderProperty.GetPolygonPointAt(animationIndex, frameIndex, orientationIndex, pointIndex) * new Vector2(1, -1);
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
                case ColliderAnimatorPropertyShapeType.Circle:
                    if (colliderProperty.GetRadius(animationIndex, frameIndex, orientationIndex) > 0)
                    {
                        float circleDrawRadius = Mathf.Min(drawSize.x, drawSize.y) / 2;
                        using (new GUIHelper.ColorScope(this.AccentColor))
                        {
                            GUIHelper.DrawLineArc(cellSize / 2, 0, Mathf.PI * 2, circleDrawRadius);
                        }
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Box:
                    Vector2 boxSize = colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex);
                    float boxScalingFactor;
                    if (boxSize.x > boxSize.y)
                    {
                        boxScalingFactor = drawSize.x / boxSize.x;
                    }
                    else
                    {
                        boxScalingFactor = drawSize.y / boxSize.y;
                    }
                    using (new GUIHelper.ColorScope(this.AccentColor))
                    {
                        GUIHelper.DrawLineBox(new Rect(cellSize / 2 - boxSize * boxScalingFactor / 2, boxSize * boxScalingFactor));
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Capsule:
                    Vector2 capsuleSize = colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex);
                    float capsuleScalingFactor;
                    if (capsuleSize.x > capsuleSize.y)
                    {
                        capsuleScalingFactor = drawSize.x / capsuleSize.x;
                    }
                    else
                    {
                        capsuleScalingFactor = drawSize.y / capsuleSize.y;
                    }
                    using (new GUIHelper.ColorScope(this.AccentColor))
                    {
                        GUIHelper.DrawLineCapsule(new Rect(cellSize / 2 - capsuleSize * capsuleScalingFactor / 2, capsuleSize * capsuleScalingFactor));
                    }
                    break;
            }

            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

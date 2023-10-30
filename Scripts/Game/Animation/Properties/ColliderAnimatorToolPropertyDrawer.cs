#if UNITY_EDITOR
using System;
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
            return new float[] { GUIPreviewOrder };
        }

        public override void DrawGeneralEditFields()
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;
            colliderProperty.ShapeType = (ColliderAnimatorPropertyShapeType)EditorGUILayout.EnumPopup("Shape Type", colliderProperty.ShapeType);
            base.DrawGeneralEditFields();
        }

        public override void DrawAnimationEditFields(int animationIndex)
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;
            if (colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Polygon || colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Line)
            {
                if (GUILayout.Button("Rotate Points In Animation"))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(),
                        new RotatePolygonPopup(
                            (Vector2 origin, float angle) =>
                            {
                                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++) 
                                {
                                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                                    {
                                        for (int pathIndex = 0; pathIndex < colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex); pathIndex++) 
                                        {
                                            for (int pointIndex = 0; pointIndex < colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex); pointIndex++)
                                            {
                                                Vector2 currPoint = colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                                                colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex, currPoint.RotateAround(origin, angle));
                                            }
                                        }
                                    }
                                }
                            }
                            )
                        );
                }
                if (GUILayout.Button("Scale Points In Animation"))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(),
                        new ScalePolygonPopup(
                            (Vector2 origin, float scale) =>
                            {
                                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                                {
                                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                                    {
                                        for (int pathIndex = 0; pathIndex < colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex); pathIndex++)
                                        {
                                            for (int pointIndex = 0; pointIndex < colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex); pointIndex++)
                                            {
                                                Vector2 currPoint = colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                                                colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex, origin + (currPoint - origin) * scale);
                                            }
                                        }
                                    }
                                }
                            }
                            )
                        );
                }
            }
            base.DrawAnimationEditFields(animationIndex);
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;
            if (colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Polygon || colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Line)
            {
                if (GUILayout.Button("Rotate Points In Frame"))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(),
                        new RotatePolygonPopup(
                            (Vector2 origin, float angle) =>
                            {
                                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                                {
                                    for (int pathIndex = 0; pathIndex < colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex); pathIndex++)
                                    {
                                        for (int pointIndex = 0; pointIndex < colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex); pointIndex++)
                                        {
                                            Vector2 currPoint = colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                                            colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex, currPoint.RotateAround(origin, angle));
                                        }
                                    }
                                }
                            }
                            )
                        );
                }
                if (GUILayout.Button("Scale Points In Frame"))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(),
                        new ScalePolygonPopup(
                            (Vector2 origin, float scale) =>
                            {
                                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                                {
                                    for (int pathIndex = 0; pathIndex < colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex); pathIndex++)
                                    {
                                        for (int pointIndex = 0; pointIndex < colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex); pointIndex++)
                                        {
                                            Vector2 currPoint = colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                                            colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex, origin + (currPoint - origin) * scale);
                                        }
                                    }
                                }
                            }
                            )
                        );
                }
            }
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;

            if (colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Polygon || colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Line)
            {
                if (GUILayout.Button("Rotate Points In Orientation"))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(),
                        new RotatePolygonPopup(
                            (Vector2 origin, float angle) =>
                            {
                                for (int pathIndex = 0; pathIndex < colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex); pathIndex++)
                                {
                                    for (int pointIndex = 0; pointIndex < colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex); pointIndex++)
                                    {
                                        Vector2 currPoint = colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                                        colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex, currPoint.RotateAround(origin, angle));
                                    }
                                }
                            }
                            )
                        );
                }
                if (GUILayout.Button("Scale Points In Orientation"))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(),
                        new ScalePolygonPopup(
                            (Vector2 origin, float scale) =>
                            {
                                for (int pathIndex = 0; pathIndex < colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex); pathIndex++)
                                {
                                    for (int pointIndex = 0; pointIndex < colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex); pointIndex++)
                                    {
                                        Vector2 currPoint = colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                                        colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex, origin + (currPoint - origin) * scale);
                                    }
                                }
                            }
                            )
                        );
                }
            }

            switch (colliderProperty.ShapeType)
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
                case ColliderAnimatorPropertyShapeType.Line:

                    UtilityGUILayout.IndexedList(
                        "Paths",
                        colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex),
                        (int pathIndex) => colliderProperty.AddPathAt(animationIndex, frameIndex, orientationIndex, pathIndex),
                        (int pathIndex) => colliderProperty.RemovePathAt(animationIndex, frameIndex, orientationIndex, pathIndex),
                        (int pathIndex) =>
                        {
                            UtilityGUILayout.IndexedList(
                                "Points",
                                colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex),
                                (int pointIndex) => colliderProperty.AddPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex),
                                (int pointIndex) => colliderProperty.RemovePointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex),
                                (int pointIndex) => colliderProperty.SetPointAt(
                                    animationIndex,
                                    frameIndex,
                                    orientationIndex,
                                    pathIndex,
                                    pointIndex,
                                    EditorGUILayout.Vector2Field("", colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex))
                                    )
                                );
                        }
                        );
                    if (colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Line)
                    {
                        colliderProperty.SetRadius(
                            animationIndex,
                            frameIndex,
                            orientationIndex,
                            EditorGUILayout.FloatField("Width", colliderProperty.GetRadius(animationIndex, frameIndex, orientationIndex))
                            );
                    }
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

        public override void DrawPreview(Vector2 previewSize, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected)
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;

            Vector2 handleSize = new Vector2(this.Config.HandleLength, this.Config.HandleLength);
            Vector2 grabSize = new Vector2(this.Config.HandleGrabLength, this.Config.HandleGrabLength);

            float lineDrawWidth = colliderProperty.GetRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;

            switch (colliderProperty.ShapeType)
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
                case ColliderAnimatorPropertyShapeType.Line:
                    for (int pathIndex = 0; pathIndex < colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex); pathIndex++)
                    {
                        Vector2[] drawPoints = new Vector2[colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex)];
                        for (int pointIndex = 0; pointIndex < drawPoints.Length; pointIndex++)
                        {
                            drawPoints[pointIndex] = previewSize / 2 + colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex) * new Vector2(1, -1) * worldToWindowScalingFactor;
                        }

                        if (drawPoints.Length == 0) continue;

                        using (new UtilityGUI.ColorScope(propertySelected ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                        {
                            if (colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Polygon)
                            {
                                UtilityGUI.DrawLinePolygon(drawPoints);
                            }
                            else
                            {
                                for (int pointIndex = 0; pointIndex < drawPoints.Length - 1; pointIndex++)
                                {
                                    UtilityGUI.DrawLinePolygon(Geometry.GetRectAlongLine(drawPoints[pointIndex], drawPoints[pointIndex + 1], lineDrawWidth));
                                }
                            }
                        }

                        if (propertySelected)
                        {
                            bool drawAdd = true;
                            for (int pointIndex = 0; pointIndex < drawPoints.Length; pointIndex++)
                            {
                                Rect handleRect = new Rect(drawPoints[pointIndex] - handleSize / 2, handleSize);
                                Rect grabRect = new Rect(drawPoints[pointIndex] - grabSize / 2, grabSize);

                                Color handleColor = UtilityGUIUtility.Darken(this.AccentColor);
                                if (grabRect.Contains(Event.current.mousePosition))
                                {
                                    handleColor = this.AccentColor;
                                    drawAdd = false;

                                    if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                                    {
                                        colliderProperty.RemovePointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                                        break;
                                    }
                                }

                                using (new UtilityGUI.ColorScope(handleColor))
                                {
                                    UtilityGUI.DrawSolidBox(handleRect);
                                }

                                if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && grabRect.Contains(Event.current.mousePosition - Event.current.delta))
                                {
                                    Vector2 newPolygonPoint = colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor;
                                    colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex, newPolygonPoint);
                                    Event.current.Use();
                                }
                            }

                            if (drawPoints.Length > 1 && drawAdd)
                            {
                                int addIndex = -1;
                                Vector2 addPoint = Vector2.zero;
                                float shortestDistance = float.MaxValue;
                                int numberLineSegments = drawPoints.Length - (colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Polygon ? 0 : 1);
                                for (int pointIndex = 0; pointIndex < numberLineSegments; pointIndex++)
                                {
                                    Vector2 nearestPoint = Geometry.FindNearestPointOnLine(drawPoints[pointIndex], drawPoints[(pointIndex + 1) % drawPoints.Length], Event.current.mousePosition);
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
                                        colliderProperty.AddPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, addIndex);
                                        colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, addIndex, (addPoint - previewSize / 2) * new Vector2(1, -1) / worldToWindowScalingFactor);
                                        Event.current.Use();
                                    }
                                }
                            }
                        }
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Circle:
                    Vector2 circleDrawCenter = previewSize / 2 + colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor * new Vector2(1, -1);
                    float circleDrawRadius = colliderProperty.GetRadius(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;

                    using (new UtilityGUI.ColorScope(propertySelected ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                    {
                        UtilityGUI.DrawLineArc(circleDrawCenter, 0, Mathf.PI * 2, circleDrawRadius);
                    }

                    if (propertySelected)
                    {
                        Vector2 closestPoint = Geometry.FindNearestPointOnCircle(circleDrawCenter, circleDrawRadius, Event.current.mousePosition);
                        float distanceToMouse = Vector2.Distance(closestPoint, Event.current.mousePosition);
                        if (distanceToMouse < this.Config.HandleGrabLength / 2)
                        {
                            Rect grabRect = new Rect(closestPoint - grabSize / 2, grabSize);
                            Rect handleRect = new Rect(closestPoint - handleSize / 2, handleSize);

                            using (new UtilityGUI.ColorScope(this.AccentColor))
                            {
                                UtilityGUI.DrawSolidBox(handleRect);
                            }

                            if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && grabRect.Contains(Event.current.mousePosition - Event.current.delta))
                            {
                                colliderProperty.SetRadius(animationIndex, frameIndex, orientationIndex, ((Event.current.mousePosition - circleDrawCenter) * new Vector2(1, -1)).magnitude / worldToWindowScalingFactor);
                                Event.current.Use();
                            }
                        }
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Box:
                    Vector2 boxDrawCenter = previewSize / 2 + colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor * new Vector2(1, -1);
                    Vector2 boxDrawSize = colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;
                    Rect boxDrawRect = new Rect(boxDrawCenter - boxDrawSize / 2, boxDrawSize);
                    using (new UtilityGUI.ColorScope(propertySelected ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                    {
                        UtilityGUI.DrawLineBox(boxDrawRect);
                    }

                    if (propertySelected)
                    {
                        Vector2 topLeftCornerPos = boxDrawCenter - boxDrawSize / 2;
                        Vector2 bottomRightCornerPos = boxDrawCenter + boxDrawSize / 2;

                        Rect topLeftHandlePos = new Rect(topLeftCornerPos - handleSize / 2, handleSize);
                        Rect bottomRightHandlePos = new Rect(bottomRightCornerPos - handleSize / 2, handleSize);

                        Rect topLeftGrabPos = new Rect(topLeftCornerPos - grabSize / 2, grabSize);
                        Rect bottomRightGrabPos = new Rect(bottomRightCornerPos - grabSize / 2, grabSize);

                        using (new UtilityGUI.ColorScope(topLeftGrabPos.Contains(Event.current.mousePosition) ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                        {
                            UtilityGUI.DrawSolidBox(topLeftHandlePos);
                        }
                        using (new UtilityGUI.ColorScope(bottomRightGrabPos.Contains(Event.current.mousePosition) ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                        {
                            UtilityGUI.DrawSolidBox(bottomRightHandlePos);
                        }

                        Vector2 worldDelta = Event.current.delta / worldToWindowScalingFactor * new Vector2(1, -1);
                        if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && topLeftGrabPos.Contains(Event.current.mousePosition - Event.current.delta))
                        {
                            colliderProperty.SetSize(animationIndex, frameIndex, orientationIndex, colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) + worldDelta * new Vector2(-1, 1));
                            colliderProperty.SetCenter(animationIndex, frameIndex, orientationIndex, colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) + worldDelta / 2);
                            Event.current.Use();
                        }
                        if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && bottomRightGrabPos.Contains(Event.current.mousePosition - Event.current.delta))
                        {
                            colliderProperty.SetSize(animationIndex, frameIndex, orientationIndex, colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) + worldDelta * new Vector2(1, -1));
                            colliderProperty.SetCenter(animationIndex, frameIndex, orientationIndex, colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) + worldDelta / 2);
                            Event.current.Use();
                        }
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Capsule:
                    Vector2 capsuleDrawCenter = previewSize / 2 + colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor * new Vector2(1, -1);
                    Vector2 capsuleDrawSize = colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) * worldToWindowScalingFactor;
                    Rect capsuleDrawRect = new Rect(capsuleDrawCenter - capsuleDrawSize / 2, capsuleDrawSize);
                    using (new UtilityGUI.ColorScope(propertySelected ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                    {
                        UtilityGUI.DrawLineCapsule(capsuleDrawRect);
                    }

                    if (propertySelected)
                    {
                        Vector2 topLeftCornerPos = capsuleDrawCenter - capsuleDrawSize / 2;
                        Vector2 bottomRightCornerPos = capsuleDrawCenter + capsuleDrawSize / 2;

                        Rect topLeftHandlePos = new Rect(topLeftCornerPos - handleSize / 2, handleSize);
                        Rect bottomRightHandlePos = new Rect(bottomRightCornerPos - handleSize / 2, handleSize);

                        Rect topLeftGrabPos = new Rect(topLeftCornerPos - grabSize / 2, grabSize);
                        Rect bottomRightGrabPos = new Rect(bottomRightCornerPos - grabSize / 2, grabSize);

                        using (new UtilityGUI.ColorScope(topLeftGrabPos.Contains(Event.current.mousePosition) ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                        {
                            UtilityGUI.DrawSolidBox(topLeftHandlePos);
                        }
                        using (new UtilityGUI.ColorScope(bottomRightGrabPos.Contains(Event.current.mousePosition) ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
                        {
                            UtilityGUI.DrawSolidBox(bottomRightHandlePos);
                        }

                        Vector2 worldDelta = Event.current.delta / worldToWindowScalingFactor * new Vector2(1, -1);
                        if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && topLeftGrabPos.Contains(Event.current.mousePosition - Event.current.delta))
                        {
                            colliderProperty.SetSize(animationIndex, frameIndex, orientationIndex, colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) + worldDelta * new Vector2(-1, 1));
                            colliderProperty.SetCenter(animationIndex, frameIndex, orientationIndex, colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) + worldDelta / 2);
                            Event.current.Use();
                        }
                        if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && bottomRightGrabPos.Contains(Event.current.mousePosition - Event.current.delta))
                        {
                            colliderProperty.SetSize(animationIndex, frameIndex, orientationIndex, colliderProperty.GetSize(animationIndex, frameIndex, orientationIndex) + worldDelta * new Vector2(1, -1));
                            colliderProperty.SetCenter(animationIndex, frameIndex, orientationIndex, colliderProperty.GetCenter(animationIndex, frameIndex, orientationIndex) + worldDelta / 2);
                            Event.current.Use();
                        }
                    }
                    break;
            }
            base.DrawPreview(previewSize, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected);
        }

        public override void DrawOrientationCellPreview(Vector2 cellSize, int animationIndex, int frameIndex, int orientationIndex)
        {
            ColliderAnimatorProperty colliderProperty = (ColliderAnimatorProperty)this.Property;

            Vector2 drawSize = new Vector2(cellSize.x - this.Config.CellPreviewPadding * 2, cellSize.y - this.Config.CellPreviewPadding * 2);

            switch (colliderProperty.ShapeType) 
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
                case ColliderAnimatorPropertyShapeType.Line:

                    Bounds bounds = new Bounds(Vector2.zero, Vector2.zero);
                    Vector2[][] pathDrawPoints = new Vector2[colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex)][];
                    for (int pathIndex = 0; pathIndex < pathDrawPoints.Length; pathIndex++)
                    {
                        pathDrawPoints[pathIndex] = new Vector2[colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex)];
                        for (int pointIndex = 0; pointIndex < pathDrawPoints[pathIndex].Length; pointIndex++)
                        {
                            pathDrawPoints[pathIndex][pointIndex] = colliderProperty.GetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex) * new Vector2(1, -1);
                            bounds.Encapsulate(pathDrawPoints[pathIndex][pointIndex]);
                        }
                    }
                    foreach (Vector2[] drawPoints in pathDrawPoints)
                    {
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
                            if (colliderProperty.ShapeType == ColliderAnimatorPropertyShapeType.Polygon)
                            {
                                UtilityGUI.DrawLinePolygon(drawPoints);
                            }
                            else
                            {
                                UtilityGUI.DrawLineSegments(drawPoints);
                            }
                        }
                    }
                    break;
                case ColliderAnimatorPropertyShapeType.Circle:
                    if (colliderProperty.GetRadius(animationIndex, frameIndex, orientationIndex) > 0)
                    {
                        float circleDrawRadius = Mathf.Min(drawSize.x, drawSize.y) / 2;
                        using (new UtilityGUI.ColorScope(this.AccentColor))
                        {
                            UtilityGUI.DrawLineArc(cellSize / 2, 0, Mathf.PI * 2, circleDrawRadius);
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
                    using (new UtilityGUI.ColorScope(this.AccentColor))
                    {
                        UtilityGUI.DrawLineBox(new Rect(cellSize / 2 - boxSize * boxScalingFactor / 2, boxSize * boxScalingFactor));
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
                    using (new UtilityGUI.ColorScope(this.AccentColor))
                    {
                        UtilityGUI.DrawLineCapsule(new Rect(cellSize / 2 - capsuleSize * capsuleScalingFactor / 2, capsuleSize * capsuleScalingFactor));
                    }
                    break;
            }

            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }

        private class RotatePolygonPopup : FrigidPopup
        {
            private Action<Vector2, float> onFinished;
            private Vector2 origin;
            private float angle;

            public RotatePolygonPopup(Action<Vector2, float> onFinished)
            {
                this.onFinished = onFinished;
                this.origin = Vector2.zero;
                this.angle = 0;
            }

            protected override void Draw()
            {
                EditorGUILayout.LabelField("Enter Rotate Parameters Below");
                this.origin = EditorGUILayout.Vector2Field("Origin", this.origin);
                this.angle = EditorGUILayout.FloatField("Angle", this.angle);
                if (GUILayout.Button("Done"))
                {
                    this.onFinished?.Invoke(this.origin, this.angle);
                    this.editorWindow.Close();
                }
            }
        }

        private class ScalePolygonPopup : FrigidPopup
        {
            private Action<Vector2, float> onFinished;
            private Vector2 origin;
            private float scale;

            public ScalePolygonPopup(Action<Vector2, float> onFinished)
            {
                this.onFinished = onFinished;
                this.origin = Vector2.zero;
                this.scale = 1.0f;
            }

            protected override void Draw()
            {
                EditorGUILayout.LabelField("Enter Scale Parameters Below");
                this.origin = EditorGUILayout.Vector2Field("Origin", this.origin);
                this.scale = EditorGUILayout.FloatField("Scale", this.scale);
                if (GUILayout.Button("Done"))
                {
                    this.onFinished?.Invoke(this.origin, this.scale);
                    this.editorWindow.Close();
                }
            }
        }
    }
}
#endif

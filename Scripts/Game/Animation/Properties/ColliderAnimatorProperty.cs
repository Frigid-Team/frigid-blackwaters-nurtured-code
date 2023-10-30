using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class ColliderAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private ColliderAnimatorPropertyShapeType shapeType;
        [SerializeField]
        [ReadOnly]
        private Collider2D collider;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Nested2DList<Vector2>> pathPoints;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> centers;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> radii;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> sizes;

        public ColliderAnimatorPropertyShapeType ShapeType
        {
            get
            {
                return this.shapeType;
            }
            set
            {
                if (this.shapeType != value)
                {
                    FrigidEdit.RecordChanges(this);
                    this.shapeType = value;
                    this.SetToShapeType();
                }
            }
        }

        public Collider2D Collider
        {
            get
            {
                return this.collider;
            }
        }

        public bool ForceDisabled
        {
            get
            {
                return !this.collider.enabled;
            }
            set
            {
                this.collider.enabled = !value;
            }
        }

        public bool TryGetBounds(out Bounds bounds)
        {
            bounds = this.collider.bounds;
            return bounds.size.magnitude > 0;
        }

        public int GetNumberPaths(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.pathPoints[animationIndex][frameIndex][orientationIndex].Count;
        }

        public void AddPathAt(int animationIndex, int frameIndex, int orientationIndex, int pathIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints[animationIndex][frameIndex][orientationIndex].Insert(pathIndex, new Nested1DList<Vector2>());
        }

        public void RemovePathAt(int animationIndex, int frameIndex, int orientationIndex, int pathIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints[animationIndex][frameIndex][orientationIndex].RemoveAt(pathIndex);
        }

        public int GetNumberPoints(int animationIndex, int frameIndex, int orientationIndex, int pathIndex)
        {
            return this.pathPoints[animationIndex][frameIndex][orientationIndex][pathIndex].Count;
        }

        public void AddPointAt(int animationIndex, int frameIndex, int orientationIndex, int pathIndex, int pointIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints[animationIndex][frameIndex][orientationIndex][pathIndex].Insert(pointIndex, Vector2.zero);
        }

        public void RemovePointAt(int animationIndex, int frameIndex, int orientationIndex, int pathIndex, int pointIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints[animationIndex][frameIndex][orientationIndex][pathIndex].RemoveAt(pointIndex);
        }

        public Vector2 GetPointAt(int animationIndex, int frameIndex, int orientationIndex, int pathIndex, int pointIndex)
        {
            return this.pathPoints[animationIndex][frameIndex][orientationIndex][pathIndex][pointIndex];
        }

        public void SetPointAt(int animationIndex, int frameIndex, int orientationIndex, int pathIndex, int pointIndex, Vector2 point)
        {
            if (this.pathPoints[animationIndex][frameIndex][orientationIndex][pathIndex][pointIndex] != point)
            {
                FrigidEdit.RecordChanges(this);
                this.pathPoints[animationIndex][frameIndex][orientationIndex][pathIndex][pointIndex] = point;
            }
        }

        public Vector2 GetCenter(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.centers[animationIndex][frameIndex][orientationIndex];
        }

        public void SetCenter(int animationIndex, int frameIndex, int orientationIndex, Vector2 center)
        {
            if (this.centers[animationIndex][frameIndex][orientationIndex] != center)
            {
                FrigidEdit.RecordChanges(this);
                this.centers[animationIndex][frameIndex][orientationIndex] = center;
            }
        }

        public float GetRadius(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.radii[animationIndex][frameIndex][orientationIndex];
        }

        public void SetRadius(int animationIndex, int frameIndex, int orientationIndex, float radius)
        {
            radius = Mathf.Max(radius, 0);
            if (this.radii[animationIndex][frameIndex][orientationIndex] != radius)
            {
                FrigidEdit.RecordChanges(this);
                this.radii[animationIndex][frameIndex][orientationIndex] = radius;
            }
        }

        public Vector2 GetSize(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.sizes[animationIndex][frameIndex][orientationIndex];
        }

        public void SetSize(int animationIndex, int frameIndex, int orientationIndex, Vector2 size)
        {
            size = new Vector2(Mathf.Max(0, size.x), Mathf.Max(0, size.y));
            if (this.sizes[animationIndex][frameIndex][orientationIndex] != size)
            {
                FrigidEdit.RecordChanges(this);
                this.sizes[animationIndex][frameIndex][orientationIndex] = size;
            }
        } 

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.shapeType = ColliderAnimatorPropertyShapeType.Polygon;
            this.SetToShapeType();
            this.pathPoints = new Nested3DList<Nested2DList<Vector2>>();
            this.centers = new Nested3DList<Vector2>();
            this.radii = new Nested3DList<float>();
            this.sizes = new Nested3DList<Vector2>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.pathPoints.Add(new Nested2DList<Nested2DList<Vector2>>());
                this.centers.Add(new Nested2DList<Vector2>());
                this.radii.Add(new Nested2DList<float>());
                this.sizes.Add(new Nested2DList<Vector2>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.pathPoints[animationIndex].Add(new Nested1DList<Nested2DList<Vector2>>());
                    this.centers[animationIndex].Add(new Nested1DList<Vector2>());
                    this.radii[animationIndex].Add(new Nested1DList<float>());
                    this.sizes[animationIndex].Add(new Nested1DList<Vector2>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.pathPoints[animationIndex][frameIndex].Add(new Nested2DList<Vector2>());
                        this.centers[animationIndex][frameIndex].Add(Vector2.zero);
                        this.radii[animationIndex][frameIndex].Add(0);
                        this.sizes[animationIndex][frameIndex].Add(Vector2.zero);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints.Insert(animationIndex, new Nested2DList<Nested2DList<Vector2>>());
            this.centers.Insert(animationIndex, new Nested2DList<Vector2>());
            this.radii.Insert(animationIndex, new Nested2DList<float>());
            this.sizes.Insert(animationIndex, new Nested2DList<Vector2>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.pathPoints[animationIndex].Add(new Nested1DList<Nested2DList<Vector2>>());
                this.centers[animationIndex].Add(new Nested1DList<Vector2>());
                this.radii[animationIndex].Add(new Nested1DList<float>());
                this.sizes[animationIndex].Add(new Nested1DList<Vector2>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.pathPoints[animationIndex][frameIndex].Add(new Nested2DList<Vector2>());
                    this.centers[animationIndex][frameIndex].Add(Vector2.zero);
                    this.radii[animationIndex][frameIndex].Add(0);
                    this.sizes[animationIndex][frameIndex].Add(Vector2.zero);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints.RemoveAt(animationIndex);
            this.centers.RemoveAt(animationIndex);
            this.radii.RemoveAt(animationIndex);
            this.sizes.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints[animationIndex].Insert(frameIndex, new Nested1DList<Nested2DList<Vector2>>());
            this.centers[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            this.radii[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            this.sizes[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.pathPoints[animationIndex][frameIndex].Add(new Nested2DList<Vector2>());
                this.centers[animationIndex][frameIndex].Add(Vector2.zero);
                this.radii[animationIndex][frameIndex].Add(0);
                this.sizes[animationIndex][frameIndex].Add(Vector2.zero);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints[animationIndex].RemoveAt(frameIndex);
            this.centers[animationIndex].RemoveAt(frameIndex);
            this.radii[animationIndex].RemoveAt(frameIndex);
            this.sizes[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints[animationIndex][frameIndex].Insert(orientationIndex, new Nested2DList<Vector2>());
            this.centers[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            this.radii[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.sizes[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pathPoints[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.centers[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.radii[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.sizes[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            ColliderAnimatorProperty otherColliderProperty = otherProperty as ColliderAnimatorProperty;

            if (otherColliderProperty)
            {
                for (int pathIndex = otherColliderProperty.GetNumberPaths(toAnimationIndex, toFrameIndex, toOrientationIndex);
                    pathIndex < this.GetNumberPaths(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                    pathIndex++)
                {
                    otherColliderProperty.AddPathAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pathIndex);
                }
                for (int pathIndex = otherColliderProperty.GetNumberPaths(toAnimationIndex, toFrameIndex, toOrientationIndex) - 1;
                    pathIndex >= this.GetNumberPaths(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                    pathIndex--)
                {
                    otherColliderProperty.RemovePathAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pathIndex);
                }
                for (int pathIndex = 0; pathIndex < this.GetNumberPaths(fromAnimationIndex, fromFrameIndex, fromOrientationIndex); pathIndex++)
                {
                    for (int pointIndex = otherColliderProperty.GetNumberPoints(toAnimationIndex, toFrameIndex, toOrientationIndex, pathIndex);
                        pointIndex < this.GetNumberPoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex, pathIndex);
                        pointIndex++)
                    {
                        otherColliderProperty.AddPointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pathIndex, pointIndex);
                    }
                    for (int pointIndex = otherColliderProperty.GetNumberPoints(toAnimationIndex, toFrameIndex, toOrientationIndex, pathIndex) - 1;
                        pointIndex >= this.GetNumberPoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex, pathIndex);
                        pointIndex--)
                    {
                        otherColliderProperty.RemovePointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pathIndex, pointIndex);
                    }
                    for (int pointIndex = 0;
                        pointIndex < this.GetNumberPoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex, pathIndex);
                        pointIndex++)
                    {
                        otherColliderProperty.SetPointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pathIndex, pointIndex, this.GetPointAt(fromAnimationIndex, fromFrameIndex, fromOrientationIndex, pathIndex, pointIndex));
                    }
                }

                otherColliderProperty.SetCenter(toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetCenter(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
                otherColliderProperty.SetRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetRadius(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
                otherColliderProperty.SetSize(toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetSize(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            this.collider.enabled = false;
            base.Initialize();
        }

        public override void Enable(bool enabled)
        {
            this.collider.enabled = enabled;
            base.Enable(enabled);
        }

        public override void OrientationEnter()
        {
            switch (this.ShapeType)
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
                case ColliderAnimatorPropertyShapeType.Line:
                    PolygonCollider2D polygonCollider = (PolygonCollider2D)this.collider;
                    int pathCount = this.GetNumberPaths(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    float lineWidth = this.GetRadius(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    int colliderPathCount = 0;
                    List<Vector2[]> colliderPaths = new List<Vector2[]>();
                    for (int pathIndex = 0; pathIndex < pathCount; pathIndex++)
                    {
                        int numPoints = this.GetNumberPoints(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex, pathIndex);
                        Vector2[] points = new Vector2[Mathf.Max(1, numPoints)];
                        for (int pointIndex = 0; pointIndex < numPoints; pointIndex++)
                        {
                            points[pointIndex] = this.GetPointAt(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex, pathIndex, pointIndex);
                        }

                        if (this.ShapeType == ColliderAnimatorPropertyShapeType.Polygon)
                        {
                            if (numPoints < 3) continue;

                            colliderPathCount++;
                            colliderPaths.Add(points);
                        }
                        else
                        {
                            if (numPoints < 2) continue;

                            colliderPathCount += numPoints - 1;
                            for (int pointIndex = 0; pointIndex < numPoints - 1; pointIndex++)
                            {
                                colliderPaths.Add(Geometry.GetRectAlongLine(points[pointIndex], points[pointIndex + 1], lineWidth));
                            }
                        }
                    }
                    polygonCollider.pathCount = colliderPathCount;
                    for (int i = 0; i < polygonCollider.pathCount; i++)
                    {
                        polygonCollider.SetPath(i, colliderPaths[i]);
                    }
                    polygonCollider.enabled = polygonCollider.pathCount > 0;
                    break;
                case ColliderAnimatorPropertyShapeType.Circle:
                    CircleCollider2D circleCollider = (CircleCollider2D)this.collider;
                    circleCollider.offset = this.GetCenter(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    float circRadius = this.GetRadius(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    circleCollider.radius = circRadius;
                    circleCollider.enabled = circRadius > FrigidConstants.WorldSizeEpsilon;
                    break;
                case ColliderAnimatorPropertyShapeType.Box:
                    BoxCollider2D boxCollider = (BoxCollider2D)this.collider;
                    boxCollider.offset = this.GetCenter(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    Vector2 boxSize = this.GetSize(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    boxCollider.size = boxSize;
                    boxCollider.enabled = boxSize.x > FrigidConstants.WorldSizeEpsilon && boxSize.y > FrigidConstants.WorldSizeEpsilon;
                    break;
                case ColliderAnimatorPropertyShapeType.Capsule:
                    CapsuleCollider2D capsuleCollider = (CapsuleCollider2D)this.collider;
                    capsuleCollider.offset = this.GetCenter(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    Vector2 capsuleSize = this.GetSize(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    if (capsuleSize.x > capsuleSize.y) capsuleCollider.direction = CapsuleDirection2D.Horizontal;
                    else capsuleCollider.direction = CapsuleDirection2D.Vertical;
                    capsuleCollider.size = capsuleSize;
                    capsuleCollider.enabled = capsuleSize.x > FrigidConstants.WorldSizeEpsilon && capsuleSize.y > FrigidConstants.WorldSizeEpsilon;
                    break;
            }
            base.OrientationEnter();
        }

        private void SetToShapeType()
        {
            switch (this.ShapeType)
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
                case ColliderAnimatorPropertyShapeType.Line:
                    this.SetColliderType<PolygonCollider2D>();
                    break;
                case ColliderAnimatorPropertyShapeType.Circle:
                    this.SetColliderType<CircleCollider2D>();
                    break;
                case ColliderAnimatorPropertyShapeType.Box:
                    this.SetColliderType<BoxCollider2D>();
                    break;
                case ColliderAnimatorPropertyShapeType.Capsule:
                    this.SetColliderType<CapsuleCollider2D>();
                    break;
            }
        }

        protected virtual void ColliderTypeUpdated() { }

        private void SetColliderType<C>() where C : Collider2D
        {
            FrigidEdit.RecordChanges(this);
            if (this.collider != null)
            {
                FrigidEdit.RemoveComponent(this.collider);
            }
            this.collider = FrigidEdit.AddComponent<C>(this.gameObject);
            this.ColliderTypeUpdated();
        }

        [Serializable]
        public class PointsInPolygon
        {
            [SerializeField]
            private List<Vector2> points;

            public PointsInPolygon()
            {
                this.points = new List<Vector2>();
            }

            public List<Vector2> Points
            {
                get
                {
                    return this.points;
                }
            }
        }
    }
}

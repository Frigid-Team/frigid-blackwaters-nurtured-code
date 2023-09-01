using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

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
        private Nested3DList<PointsInPolygon> pointsInPolygons;
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

        public int GetNumberPolygonPoints(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.pointsInPolygons[animationIndex][frameIndex][orientationIndex].Points.Count;
        }

        public void AddPolygonPointAt(int animationIndex, int frameIndex, int orientationIndex, int index, Vector2 point)
        {
            FrigidEdit.RecordChanges(this);
            this.pointsInPolygons[animationIndex][frameIndex][orientationIndex].Points.Insert(index, point);
        }

        public void RemovePolygonPointAt(int animationIndex, int frameIndex, int orientationIndex, int index)
        {
            FrigidEdit.RecordChanges(this);
            this.pointsInPolygons[animationIndex][frameIndex][orientationIndex].Points.RemoveAt(index);
        }

        public Vector2 GetPolygonPointAt(int animationIndex, int frameIndex, int orientationIndex, int index)
        {
            return this.pointsInPolygons[animationIndex][frameIndex][orientationIndex].Points[index];
        }

        public void SetPolygonPointAt(int animationIndex, int frameIndex, int orientationIndex, int index, Vector2 point)
        {
            if (this.pointsInPolygons[animationIndex][frameIndex][orientationIndex].Points[index] != point)
            {
                FrigidEdit.RecordChanges(this);
                this.pointsInPolygons[animationIndex][frameIndex][orientationIndex].Points[index] = point;
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
            this.pointsInPolygons = new Nested3DList<PointsInPolygon>();
            this.centers = new Nested3DList<Vector2>();
            this.radii = new Nested3DList<float>();
            this.sizes = new Nested3DList<Vector2>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.pointsInPolygons.Add(new Nested2DList<PointsInPolygon>());
                this.centers.Add(new Nested2DList<Vector2>());
                this.radii.Add(new Nested2DList<float>());
                this.sizes.Add(new Nested2DList<Vector2>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.pointsInPolygons[animationIndex].Add(new Nested1DList<PointsInPolygon>());
                    this.centers[animationIndex].Add(new Nested1DList<Vector2>());
                    this.radii[animationIndex].Add(new Nested1DList<float>());
                    this.sizes[animationIndex].Add(new Nested1DList<Vector2>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.pointsInPolygons[animationIndex][frameIndex].Add(new PointsInPolygon());
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
            this.pointsInPolygons.Insert(animationIndex, new Nested2DList<PointsInPolygon>());
            this.centers.Insert(animationIndex, new Nested2DList<Vector2>());
            this.radii.Insert(animationIndex, new Nested2DList<float>());
            this.sizes.Insert(animationIndex, new Nested2DList<Vector2>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.pointsInPolygons[animationIndex].Add(new Nested1DList<PointsInPolygon>());
                this.centers[animationIndex].Add(new Nested1DList<Vector2>());
                this.radii[animationIndex].Add(new Nested1DList<float>());
                this.sizes[animationIndex].Add(new Nested1DList<Vector2>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.pointsInPolygons[animationIndex][frameIndex].Add(new PointsInPolygon());
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
            this.pointsInPolygons.RemoveAt(animationIndex);
            this.centers.RemoveAt(animationIndex);
            this.radii.RemoveAt(animationIndex);
            this.sizes.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pointsInPolygons[animationIndex].Insert(frameIndex, new Nested1DList<PointsInPolygon>());
            this.centers[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            this.radii[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            this.sizes[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.pointsInPolygons[animationIndex][frameIndex].Add(new PointsInPolygon());
                this.centers[animationIndex][frameIndex].Add(Vector2.zero);
                this.radii[animationIndex][frameIndex].Add(0);
                this.sizes[animationIndex][frameIndex].Add(Vector2.zero);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pointsInPolygons[animationIndex].RemoveAt(frameIndex);
            this.centers[animationIndex].RemoveAt(frameIndex);
            this.radii[animationIndex].RemoveAt(frameIndex);
            this.sizes[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pointsInPolygons[animationIndex][frameIndex].Insert(orientationIndex, new PointsInPolygon());
            this.centers[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            this.radii[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.sizes[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.pointsInPolygons[animationIndex][frameIndex].RemoveAt(orientationIndex);
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
                for (int pointIndex = 0;
                    pointIndex < Mathf.Min(otherColliderProperty.GetNumberPolygonPoints(toAnimationIndex, toFrameIndex, toOrientationIndex), this.GetNumberPolygonPoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
                    pointIndex++)
                {
                    otherColliderProperty.SetPolygonPointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pointIndex, this.GetPolygonPointAt(fromAnimationIndex, fromFrameIndex, fromOrientationIndex, pointIndex));
                }
                for (int pointIndex = otherColliderProperty.GetNumberPolygonPoints(toAnimationIndex, toFrameIndex, toOrientationIndex);
                    pointIndex < this.GetNumberPolygonPoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                    pointIndex++)
                {
                    otherColliderProperty.AddPolygonPointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pointIndex, this.GetPolygonPointAt(fromAnimationIndex, fromFrameIndex, fromOrientationIndex, pointIndex));
                }
                for (int pointIndex = otherColliderProperty.GetNumberPolygonPoints(toAnimationIndex, toFrameIndex, toOrientationIndex) - 1;
                    pointIndex >= this.GetNumberPolygonPoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                    pointIndex--)
                {
                    otherColliderProperty.RemovePolygonPointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pointIndex);
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
                    int numPoints = this.GetNumberPolygonPoints(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    Vector2[] polygonPoints = new Vector2[Mathf.Max(1, numPoints)];
                    for (int pointIndex = 0; pointIndex < numPoints; pointIndex++)
                    {
                        polygonPoints[pointIndex] = this.GetPolygonPointAt(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex, pointIndex);
                    }
                    PolygonCollider2D polygonCollider = (PolygonCollider2D)this.collider;
                    polygonCollider.SetPath(0, polygonPoints);
                    polygonCollider.enabled = numPoints > 0;
                    break;
                case ColliderAnimatorPropertyShapeType.Circle:
                    CircleCollider2D circleCollider = (CircleCollider2D)this.collider;
                    circleCollider.offset = this.GetCenter(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    float circRadius = this.GetRadius(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    circleCollider.radius = circRadius;
                    circleCollider.enabled = circRadius > FrigidConstants.SMALLEST_WORLD_SIZE;
                    break;
                case ColliderAnimatorPropertyShapeType.Box:
                    BoxCollider2D boxCollider = (BoxCollider2D)this.collider;
                    boxCollider.offset = this.GetCenter(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    Vector2 boxSize = this.GetSize(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    boxCollider.size = boxSize;
                    boxCollider.enabled = boxSize.x > FrigidConstants.SMALLEST_WORLD_SIZE && boxSize.y > FrigidConstants.SMALLEST_WORLD_SIZE;
                    break;
                case ColliderAnimatorPropertyShapeType.Capsule:
                    CapsuleCollider2D capsuleCollider = (CapsuleCollider2D)this.collider;
                    capsuleCollider.offset = this.GetCenter(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    Vector2 capsuleSize = this.GetSize(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                    if (capsuleSize.x > capsuleSize.y) capsuleCollider.direction = CapsuleDirection2D.Horizontal;
                    else capsuleCollider.direction = CapsuleDirection2D.Vertical;
                    capsuleCollider.size = capsuleSize;
                    capsuleCollider.enabled = capsuleSize.x > FrigidConstants.SMALLEST_WORLD_SIZE && capsuleSize.y > FrigidConstants.SMALLEST_WORLD_SIZE;
                    break;
            }
            base.OrientationEnter();
        }

        private void SetToShapeType()
        {
            switch (this.ShapeType)
            {
                case ColliderAnimatorPropertyShapeType.Polygon:
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

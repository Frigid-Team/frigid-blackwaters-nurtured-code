using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class LineAnimatorProperty : RendererAnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private LineRenderer lineRenderer;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Nested1DList<Vector2>> linePoints;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> lineWidths;

        public int GetNumberLinePoints(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.linePoints[animationIndex][frameIndex][orientationIndex].Count;
        }

        public void AddLinePointAt(int animationIndex, int frameIndex, int orientationIndex, int pointIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.linePoints[animationIndex][frameIndex][orientationIndex].Insert(pointIndex, Vector2.zero);
        }

        public void RemoveLinePointAt(int animationIndex, int frameIndex, int orientationIndex, int pointIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.linePoints[animationIndex][frameIndex][orientationIndex].RemoveAt(pointIndex);
        }

        public Vector2 GetLinePointAt(int animationIndex, int frameIndex, int orientationIndex, int pointIndex)
        {
            return this.linePoints[animationIndex][frameIndex][orientationIndex][pointIndex];
        }

        public void SetLinePointAt(int animationIndex, int frameIndex, int orientationIndex, int pointIndex, Vector2 point)
        {
            if (this.linePoints[animationIndex][frameIndex][orientationIndex][pointIndex] != point)
            {
                FrigidEdit.RecordChanges(this);
                this.linePoints[animationIndex][frameIndex][orientationIndex][pointIndex] = point;
            }
        }

        public float GetLineWidth(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.lineWidths[animationIndex][frameIndex][orientationIndex];
        }

        public void SetLineWidth(int animationIndex, int frameIndex, int orientationIndex, float lineWidth)
        {
            if (this.lineWidths[animationIndex][frameIndex][orientationIndex] != lineWidth)
            {
                FrigidEdit.RecordChanges(this);
                this.lineWidths[animationIndex][frameIndex][orientationIndex] = lineWidth;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.lineRenderer = FrigidEdit.AddComponent<LineRenderer>(this.gameObject);
            this.lineRenderer.useWorldSpace = false;
            this.lineRenderer.textureMode = LineTextureMode.Tile;
            this.lineRenderer.widthMultiplier = 1.0f;
            this.lineRenderer.widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
            this.lineRenderer.alignment = LineAlignment.TransformZ;
            this.linePoints = new Nested3DList<Nested1DList<Vector2>>();
            this.lineWidths = new Nested3DList<float>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.linePoints.Add(new Nested2DList<Nested1DList<Vector2>>());
                this.lineWidths.Add(new Nested2DList<float>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.linePoints[animationIndex].Add(new Nested1DList<Nested1DList<Vector2>>());
                    this.lineWidths[animationIndex].Add(new Nested1DList<float>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.linePoints[animationIndex][frameIndex].Add(new Nested1DList<Vector2>());
                        this.lineWidths[animationIndex][frameIndex].Add(1f);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.linePoints.Insert(animationIndex, new Nested2DList<Nested1DList<Vector2>>());
            this.lineWidths.Insert(animationIndex, new Nested2DList<float>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.linePoints[animationIndex].Add(new Nested1DList<Nested1DList<Vector2>>());
                this.lineWidths[animationIndex].Add(new Nested1DList<float>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.linePoints[animationIndex][frameIndex].Add(new Nested1DList<Vector2>());
                    this.lineWidths[animationIndex][frameIndex].Add(1f);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.linePoints.RemoveAt(animationIndex);
            this.lineWidths.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.linePoints[animationIndex].Insert(frameIndex, new Nested1DList<Nested1DList<Vector2>>());
            this.lineWidths[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.linePoints[animationIndex][frameIndex].Add(new Nested1DList<Vector2>());
                this.lineWidths[animationIndex][frameIndex].Add(1f);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.linePoints[animationIndex].RemoveAt(frameIndex);
            this.lineWidths[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.linePoints[animationIndex][frameIndex].Insert(orientationIndex, new Nested1DList<Vector2>());
            this.lineWidths[animationIndex][frameIndex].Insert(orientationIndex, 1f);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.linePoints[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.lineWidths[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            LineAnimatorProperty otherLineProperty = otherProperty as LineAnimatorProperty;
            if (otherLineProperty)
            {
                for (int pointIndex = otherLineProperty.GetNumberLinePoints(toAnimationIndex, toFrameIndex, toOrientationIndex);
                    pointIndex < this.GetNumberLinePoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                    pointIndex++)
                {
                    otherLineProperty.AddLinePointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pointIndex);
                }
                for (int pointIndex = otherLineProperty.GetNumberLinePoints(toAnimationIndex, toFrameIndex, toOrientationIndex) - 1;
                    pointIndex >= this.GetNumberLinePoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                    pointIndex--)
                {
                    otherLineProperty.RemoveLinePointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pointIndex);
                }
                for (int pointIndex = 0;
                    pointIndex < this.GetNumberLinePoints(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                    pointIndex++)
                {
                    otherLineProperty.SetLinePointAt(toAnimationIndex, toFrameIndex, toOrientationIndex, pointIndex, this.GetLinePointAt(fromAnimationIndex, fromFrameIndex, fromOrientationIndex, pointIndex));
                }
                otherLineProperty.SetLineWidth(toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetLineWidth(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.lineRenderer.enabled = false;
        }

        public override void Enable(bool enabled)
        {
            base.Enable(enabled);
            this.lineRenderer.enabled = enabled;
        }

        public override void OrientationEnter()
        {
            base.OrientationEnter();
            this.lineRenderer.positionCount = this.GetNumberLinePoints(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            for (int pointIndex = 0; pointIndex < this.lineRenderer.positionCount; pointIndex++)
            {
                this.lineRenderer.SetPosition(pointIndex, this.GetLinePointAt(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex, pointIndex));
            }
            float width = this.GetLineWidth(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            this.lineRenderer.widthCurve = AnimationCurve.Linear(0f, width, 1f, width);
        }

        protected override Renderer Renderer
        {
            get
            {
                return this.lineRenderer;
            }
        }
    }
}

using System.Linq;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class LinePointsFieldAnimatorProperty : OrientationFieldAnimatorProperty<LineAnimatorProperty>
    {
        private Vector2[] linePoints;

        public Vector2[] LinePoints
        {
            get
            {
                return this.linePoints;
            }
            set
            {
                if (!Enumerable.SequenceEqual(this.linePoints, value))
                {
                    this.linePoints = value;
                    this.SetValuesInProperties();
                }
            }
        }

        public override void Initialize()
        {
            this.linePoints = new Vector2[0];
            base.Initialize();
        }

        protected override void SetValue(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex)
        {
            LineAnimatorProperty lineProperty = this.GetParameteredProperty(propertyIndex);

            for (int pointIndex = lineProperty.GetNumberLinePoints(animationIndex, frameIndex, orientationIndex);
                pointIndex < this.LinePoints.Length;
                pointIndex++)
            {
                lineProperty.AddLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex);
            }
            for (int pointIndex = lineProperty.GetNumberLinePoints(animationIndex, frameIndex, orientationIndex) - 1;
                pointIndex >= this.LinePoints.Length;
                pointIndex--)
            {
                lineProperty.RemoveLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex);
            }
            for (int pointIndex = 0;
                pointIndex < this.LinePoints.Length;
                pointIndex++)
            {
                lineProperty.SetLinePointAt(animationIndex, frameIndex, orientationIndex, pointIndex, this.LinePoints[pointIndex]);
            }
        }
    }
}

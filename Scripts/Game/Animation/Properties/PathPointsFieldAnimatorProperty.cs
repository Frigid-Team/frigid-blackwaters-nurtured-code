using System;
using System.Linq;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class PathPointsFieldAnimatorProperty : OrientationFieldAnimatorProperty<ColliderAnimatorProperty>
    {
        private Vector2[][] pathPoints;

        public int PathCount
        {
            get
            {
                return this.pathPoints.Length;
            }
            set
            {
                if (this.pathPoints.Length != value)
                {
                    Array.Resize(ref this.pathPoints, value);
                    for (int pathIndex = 0; pathIndex < this.pathPoints.Length; pathIndex++)
                    {
                        if (this.pathPoints[pathIndex] == null)
                        {
                            this.pathPoints[pathIndex] = new Vector2[0];
                        }
                    }
                    this.SetValuesInProperties();
                }
            }
        }

        public void SetPath(int pathIndex, Vector2[] pathPoints)
        {
            if (!Enumerable.SequenceEqual(this.pathPoints[pathIndex], pathPoints))
            {
                this.pathPoints[pathIndex] = pathPoints;
                this.SetValuesInProperties();
            }
        }

        public Vector2[] GetPath(int pathIndex)
        {
            return this.pathPoints[pathIndex];
        }

        public override void Initialize()
        {
            this.pathPoints = new Vector2[0][];
            base.Initialize();
        }

        protected override void SetValue(int propertyIndex, int animationIndex, int frameIndex, int orientationIndex)
        {
            ColliderAnimatorProperty colliderProperty = this.GetParameteredProperty(propertyIndex);

            for (int pathIndex = colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex); pathIndex < this.PathCount; pathIndex++)
            {
                colliderProperty.AddPathAt(animationIndex, frameIndex, orientationIndex, pathIndex);
            }
            for (int pathIndex = colliderProperty.GetNumberPaths(animationIndex, frameIndex, orientationIndex) - 1; pathIndex >= this.PathCount; pathIndex--)
            {
                colliderProperty.RemovePathAt(animationIndex, frameIndex, orientationIndex, pathIndex);
            }
            for (int pathIndex = 0; pathIndex < this.PathCount; pathIndex++)
            {
                Vector2[] pathPoints = this.GetPath(pathIndex);
                for (int pointIndex = colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex); pointIndex < pathPoints.Length; pointIndex++)
                {
                    colliderProperty.AddPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                }
                for (int pointIndex = colliderProperty.GetNumberPoints(animationIndex, frameIndex, orientationIndex, pathIndex) - 1; pointIndex >= pathPoints.Length; pointIndex--)
                {
                    colliderProperty.RemovePointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex);
                }
                for (int pointIndex = 0; pointIndex < pathPoints.Length; pointIndex++)
                {
                    colliderProperty.SetPointAt(animationIndex, frameIndex, orientationIndex, pathIndex, pointIndex, pathPoints[pointIndex]);
                }
            }
        }
    }
}

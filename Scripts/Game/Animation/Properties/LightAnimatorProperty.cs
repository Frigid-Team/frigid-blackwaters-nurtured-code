using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class LightAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private Light2D light;
        [SerializeField]
        [HideInInspector]
        private List<FreeformShapePath> freeformShapePaths;
        [SerializeField]
        [HideInInspector]
        private Nested2DList<ColorSerializedReference> lightColors;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> localOffsets;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> innerRadii;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> outerRadii;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> normalAnglesRad;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> innerAnglesRad;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> outerAnglesRad;

        public override List<AnimatorProperty> ChildProperties
        {
            get
            {
                return new List<AnimatorProperty>();
            }
        }

        public Light2D.LightType LightType
        {
            get
            {
                return this.light.lightType;
            }
            set
            {
                if (this.light.lightType != value)
                {

                    FrigidEditMode.RecordPotentialChanges(this);
                    this.light.lightType = value;
                }
            }
        }

        public int NumberFreeformPoints
        {
            get
            {
                return this.light.shapePath.Length;
            }
            set
            {
                if (this.light.shapePath.Length != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this.light);
                    FrigidEditMode.RecordPotentialChanges(this);

                    // Nasty hack with reflection. Unfortunately, Unity's 2D lights do not have a setter for this property.
                    FieldInfo shapePathField = typeof(Light2D).GetField("m_ShapePath", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    Vector3[] currShapePath = this.light.shapePath;
                    Array.Resize(ref currShapePath, value);
                    shapePathField.SetValue(this.light, currShapePath);

                    foreach (FreeformShapePath freeformShapePath in this.freeformShapePaths)
                    {
                        Vector2[] shapePath = freeformShapePath.ShapePath;
                        Array.Resize(ref shapePath, value);
                        freeformShapePath.ShapePath = shapePath;
                    }
                }
            }
        }

        public Vector2 GetFreeformPointAt(int animationIndex, int index)
        {
            return this.freeformShapePaths[animationIndex].ShapePath[index];
        }

        public void SetFreeformPointAt(int animationIndex, int index, Vector2 point)
        {
            if (this.freeformShapePaths[animationIndex].ShapePath[index] != point)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.freeformShapePaths[animationIndex].ShapePath[index] = point;
            }
        }

        public ColorSerializedReference GetLightColorByReference(int animationIndex, int frameIndex)
        {
            return this.lightColors[animationIndex][frameIndex];
        }

        public void SetLightColorByReference(int animationIndex, int frameIndex, ColorSerializedReference lightColor)
        {
            if (this.lightColors[animationIndex][frameIndex] != lightColor)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.lightColors[animationIndex][frameIndex] = lightColor;
            }
        }

        public Vector2 GetLocalOffset(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.localOffsets[animationIndex][frameIndex][orientationIndex];
        }

        public void SetLocalOffset(int animationIndex, int frameIndex, int orientationIndex, Vector2 localOffset)
        {
            if (this.localOffsets[animationIndex][frameIndex][orientationIndex] != localOffset)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.localOffsets[animationIndex][frameIndex][orientationIndex] = localOffset;
            }
        }

        public float GetOuterRadius(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.outerRadii[animationIndex][frameIndex][orientationIndex];
        }

        public void SetOuterRadius(int animationIndex, int frameIndex, int orientationIndex, float outerRadius)
        {
            outerRadius = Mathf.Max(Mathf.Max(outerRadius, GetInnerRadius(animationIndex, frameIndex, orientationIndex)), 0);
            if (this.outerRadii[animationIndex][frameIndex][orientationIndex] != outerRadius)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.outerRadii[animationIndex][frameIndex][orientationIndex] = outerRadius;
            }
        }

        public float GetInnerRadius(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.innerRadii[animationIndex][frameIndex][orientationIndex];
        }

        public void SetInnerRadius(int animationIndex, int frameIndex, int orientationIndex, float innerRadius)
        {
            innerRadius = Mathf.Max(Mathf.Min(innerRadius, GetOuterRadius(animationIndex, frameIndex, orientationIndex)), 0);
            if (this.innerRadii[animationIndex][frameIndex][orientationIndex] != innerRadius)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.innerRadii[animationIndex][frameIndex][orientationIndex] = innerRadius;
            }
        }

        public float GetNormalAngleRad(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.normalAnglesRad[animationIndex][frameIndex][orientationIndex];
        }

        public void SetNormalAngleRad(int animationIndex, int frameIndex, int orientationIndex, float normalAngleRad)
        {
            if (this.normalAnglesRad[animationIndex][frameIndex][orientationIndex] != normalAngleRad)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.normalAnglesRad[animationIndex][frameIndex][orientationIndex] = normalAngleRad;
            }
        }

        public float GetInnerAngleRad(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.innerAnglesRad[animationIndex][frameIndex][orientationIndex];
        }

        public void SetInnerAngleRad(int animationIndex, int frameIndex, int orientationIndex, float innerAngleRad)
        {
            innerAngleRad = Mathf.Min(innerAngleRad, GetOuterAngleRad(animationIndex, frameIndex, orientationIndex));
            if (this.innerAnglesRad[animationIndex][frameIndex][orientationIndex] != innerAngleRad)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.innerAnglesRad[animationIndex][frameIndex][orientationIndex] = innerAngleRad;
            }
        }

        public float GetOuterAngleRad(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.outerAnglesRad[animationIndex][frameIndex][orientationIndex];
        }

        public void SetOuterAngleRad(int animationIndex, int frameIndex, int orientationIndex, float outerAngleRad)
        {
            outerAngleRad = Mathf.Max(outerAngleRad, GetInnerAngleRad(animationIndex, frameIndex, orientationIndex));
            if (this.outerAnglesRad[animationIndex][frameIndex][orientationIndex] != outerAngleRad)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.outerAnglesRad[animationIndex][frameIndex][orientationIndex] = outerAngleRad;
            }
        }

        public override void Created()
        {
            this.light = FrigidEditMode.AddComponent<Light2D>(this.gameObject);
            FrigidEditMode.RecordPotentialChanges(this);
            this.LightType = Light2D.LightType.Point;
            this.freeformShapePaths = new List<FreeformShapePath>();
            this.lightColors = new Nested2DList<ColorSerializedReference>();
            this.localOffsets = new Nested3DList<Vector2>();
            this.innerRadii = new Nested3DList<float>();
            this.outerRadii = new Nested3DList<float>();
            this.normalAnglesRad = new Nested3DList<float>();
            this.innerAnglesRad = new Nested3DList<float>();
            this.outerAnglesRad = new Nested3DList<float>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.freeformShapePaths.Add(new FreeformShapePath(this.NumberFreeformPoints));
                this.lightColors.Add(new Nested1DList<ColorSerializedReference>());
                this.localOffsets.Add(new Nested2DList<Vector2>());
                this.innerRadii.Add(new Nested2DList<float>());
                this.outerRadii.Add(new Nested2DList<float>());
                this.normalAnglesRad.Add(new Nested2DList<float>());
                this.innerAnglesRad.Add(new Nested2DList<float>());
                this.outerAnglesRad.Add(new Nested2DList<float>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.lightColors[animationIndex].Add(new ColorSerializedReference());
                    this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                    this.innerRadii[animationIndex].Add(new Nested1DList<float>());
                    this.outerRadii[animationIndex].Add(new Nested1DList<float>());
                    this.normalAnglesRad[animationIndex].Add(new Nested1DList<float>());
                    this.innerAnglesRad[animationIndex].Add(new Nested1DList<float>());
                    this.outerAnglesRad[animationIndex].Add(new Nested1DList<float>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                        this.innerRadii[animationIndex][frameIndex].Add(0);
                        this.outerRadii[animationIndex][frameIndex].Add(0);
                        this.normalAnglesRad[animationIndex][frameIndex].Add(0);
                        this.innerAnglesRad[animationIndex][frameIndex].Add(0);
                        this.outerAnglesRad[animationIndex][frameIndex].Add(0);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.freeformShapePaths.Insert(animationIndex, new FreeformShapePath(this.NumberFreeformPoints));
            this.lightColors.Insert(animationIndex, new Nested1DList<ColorSerializedReference>());
            this.localOffsets.Insert(animationIndex, new Nested2DList<Vector2>());
            this.innerRadii.Insert(animationIndex, new Nested2DList<float>());
            this.outerRadii.Insert(animationIndex, new Nested2DList<float>());
            this.normalAnglesRad.Insert(animationIndex, new Nested2DList<float>());
            this.innerAnglesRad.Insert(animationIndex, new Nested2DList<float>());
            this.outerAnglesRad.Insert(animationIndex, new Nested2DList<float>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.lightColors[animationIndex].Add(new ColorSerializedReference());
                this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                this.innerRadii[animationIndex].Add(new Nested1DList<float>());
                this.outerRadii[animationIndex].Add(new Nested1DList<float>());
                this.normalAnglesRad[animationIndex].Add(new Nested1DList<float>());
                this.innerAnglesRad[animationIndex].Add(new Nested1DList<float>());
                this.outerAnglesRad[animationIndex].Add(new Nested1DList<float>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                    this.innerRadii[animationIndex][frameIndex].Add(0);
                    this.outerRadii[animationIndex][frameIndex].Add(0);
                    this.normalAnglesRad[animationIndex][frameIndex].Add(0);
                    this.innerAnglesRad[animationIndex][frameIndex].Add(0);
                    this.outerAnglesRad[animationIndex][frameIndex].Add(0);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.freeformShapePaths.RemoveAt(animationIndex);
            this.lightColors.RemoveAt(animationIndex);
            this.localOffsets.RemoveAt(animationIndex);
            this.innerRadii.RemoveAt(animationIndex);
            this.outerRadii.RemoveAt(animationIndex);
            this.normalAnglesRad.RemoveAt(animationIndex);
            this.innerAnglesRad.RemoveAt(animationIndex);
            this.outerAnglesRad.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.lightColors[animationIndex].Insert(animationIndex, new ColorSerializedReference());
            this.localOffsets[animationIndex].Insert(animationIndex, new Nested1DList<Vector2>());
            this.innerRadii[animationIndex].Insert(animationIndex, new Nested1DList<float>());
            this.outerRadii[animationIndex].Insert(animationIndex, new Nested1DList<float>());
            this.normalAnglesRad[animationIndex].Insert(animationIndex, new Nested1DList<float>());
            this.innerAnglesRad[animationIndex].Insert(animationIndex, new Nested1DList<float>());
            this.outerAnglesRad[animationIndex].Insert(animationIndex, new Nested1DList<float>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                this.innerRadii[animationIndex][frameIndex].Add(0);
                this.outerRadii[animationIndex][frameIndex].Add(0);
                this.normalAnglesRad[animationIndex][frameIndex].Add(0);
                this.innerAnglesRad[animationIndex][frameIndex].Add(0);
                this.outerAnglesRad[animationIndex][frameIndex].Add(0);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.lightColors[animationIndex].RemoveAt(animationIndex);
            this.localOffsets[animationIndex].RemoveAt(animationIndex);
            this.innerRadii[animationIndex].RemoveAt(animationIndex);
            this.outerRadii[animationIndex].RemoveAt(animationIndex);
            this.normalAnglesRad[animationIndex].RemoveAt(animationIndex);
            this.innerAnglesRad[animationIndex].RemoveAt(animationIndex);
            this.outerAnglesRad[animationIndex].RemoveAt(animationIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            this.innerRadii[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.outerRadii[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.normalAnglesRad[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.innerAnglesRad[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.outerAnglesRad[animationIndex][frameIndex].Insert(orientationIndex, 0);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.innerRadii[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.outerRadii[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.normalAnglesRad[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.innerAnglesRad[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.outerAnglesRad[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherAnimation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex)
        {
            LightAnimatorProperty otherLightProperty = otherProperty as LightAnimatorProperty;
            if (otherLightProperty)
            {
                if (otherLightProperty.NumberFreeformPoints == this.NumberFreeformPoints)
                {
                    for (int pointIndex = 0; pointIndex < this.NumberFreeformPoints; pointIndex++)
                    {
                        otherLightProperty.SetFreeformPointAt(toAnimationIndex, pointIndex, GetFreeformPointAt(fromAnimationIndex, pointIndex));
                    }
                }
            }
            base.CopyPasteToAnotherAnimation(otherProperty, fromAnimationIndex, toAnimationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            LightAnimatorProperty otherLightProperty = otherProperty as LightAnimatorProperty;
            if (otherLightProperty)
            {
                otherLightProperty.SetLightColorByReference(toAnimationIndex, toFrameIndex, new ColorSerializedReference(GetLightColorByReference(fromAnimationIndex, fromFrameIndex)));
                otherLightProperty.SetLocalOffset(toAnimationIndex, toFrameIndex, toOrientationIndex, GetLocalOffset(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));

                float myOuterRadius = GetOuterRadius(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                float myInnerRadius = GetInnerRadius(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                otherLightProperty.SetInnerRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, 0);
                otherLightProperty.SetOuterRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, 0);
                otherLightProperty.SetOuterRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, myOuterRadius);
                otherLightProperty.SetInnerRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, myInnerRadius);

                otherLightProperty.SetNormalAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, GetNormalAngleRad(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));

                float myOuterAngleRad = GetOuterAngleRad(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                float myInnerAngleRad = GetInnerAngleRad(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                otherLightProperty.SetInnerAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, 0);
                otherLightProperty.SetOuterAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, 0);
                otherLightProperty.SetOuterAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, myOuterAngleRad);
                otherLightProperty.SetInnerAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, myInnerAngleRad);
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void AnimationEnter(int animationIndex, float elapsedDuration)
        {
            if (this.LightType == Light2D.LightType.Freeform)
            {
                for (int pointIndex = 0; pointIndex < this.NumberFreeformPoints; pointIndex++)
                {
                    this.light.shapePath[pointIndex] = GetFreeformPointAt(animationIndex, pointIndex);
                }
            }
            base.AnimationEnter(animationIndex, elapsedDuration);
        }

        public override void SetFrameEnter(int animationIndex, int frameIndex, float elapsedDuration, int loopsElapsed)
        {
            this.light.color = GetLightColorByReference(animationIndex, frameIndex).MutableValue;
            base.SetFrameEnter(animationIndex, frameIndex, elapsedDuration, loopsElapsed);
        }

        public override void OrientFrameEnter(int animationIndex, int frameIndex, int orientationIndex, float elapsedDuration)
        {
            this.light.transform.localPosition = GetLocalOffset(animationIndex, frameIndex, orientationIndex);
            switch (this.LightType) 
            {
                case Light2D.LightType.Point:
                    this.light.pointLightInnerRadius = 0;
                    this.light.pointLightOuterRadius = 0;
                    this.light.pointLightOuterRadius = GetOuterRadius(animationIndex, frameIndex, orientationIndex);
                    this.light.pointLightInnerRadius = GetInnerRadius(animationIndex, frameIndex, orientationIndex);

                    this.light.pointLightInnerAngle = 0;
                    this.light.pointLightOuterAngle = 0;
                    this.light.pointLightOuterAngle = GetOuterAngleRad(animationIndex, frameIndex, orientationIndex) * Mathf.Rad2Deg;
                    this.light.pointLightInnerAngle = GetInnerAngleRad(animationIndex, frameIndex, orientationIndex) * Mathf.Rad2Deg;

                    this.light.transform.localRotation = Quaternion.Euler(0, 0, GetNormalAngleRad(animationIndex, frameIndex, orientationIndex) * Mathf.Rad2Deg - 90);
                    break;
            }
            base.OrientFrameEnter(animationIndex, frameIndex, orientationIndex, elapsedDuration);
        }

        [Serializable]
        private class FreeformShapePath
        {
            [SerializeField]
            private Vector2[] shapePath;

            public FreeformShapePath(int numberPoints)
            {
                this.shapePath = new Vector2[numberPoints];
            }

            public Vector2[] ShapePath
            {
                get
                {
                    return this.shapePath;
                }
                set
                {
                    this.shapePath = value;
                }
            }
        }
    }
}

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
        private Nested3DList<float> innerRadii;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> outerRadii;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> innerAnglesRad;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<float> outerAnglesRad;


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

                    FrigidEdit.RecordChanges(this);
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
                    FrigidEdit.RecordChanges(this.light);
                    FrigidEdit.RecordChanges(this);

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
                FrigidEdit.RecordChanges(this);
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
                FrigidEdit.RecordChanges(this);
                this.lightColors[animationIndex][frameIndex] = lightColor;
            }
        }

        public float GetOuterRadius(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.outerRadii[animationIndex][frameIndex][orientationIndex];
        }

        public void SetOuterRadius(int animationIndex, int frameIndex, int orientationIndex, float outerRadius)
        {
            outerRadius = Mathf.Max(Mathf.Max(outerRadius, this.GetInnerRadius(animationIndex, frameIndex, orientationIndex)), 0);
            if (this.outerRadii[animationIndex][frameIndex][orientationIndex] != outerRadius)
            {
                FrigidEdit.RecordChanges(this);
                this.outerRadii[animationIndex][frameIndex][orientationIndex] = outerRadius;
            }
        }

        public float GetInnerRadius(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.innerRadii[animationIndex][frameIndex][orientationIndex];
        }

        public void SetInnerRadius(int animationIndex, int frameIndex, int orientationIndex, float innerRadius)
        {
            innerRadius = Mathf.Max(Mathf.Min(innerRadius, this.GetOuterRadius(animationIndex, frameIndex, orientationIndex)), 0);
            if (this.innerRadii[animationIndex][frameIndex][orientationIndex] != innerRadius)
            {
                FrigidEdit.RecordChanges(this);
                this.innerRadii[animationIndex][frameIndex][orientationIndex] = innerRadius;
            }
        }

        public float GetInnerAngleRad(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.innerAnglesRad[animationIndex][frameIndex][orientationIndex];
        }

        public void SetInnerAngleRad(int animationIndex, int frameIndex, int orientationIndex, float innerAngleRad)
        {
            innerAngleRad = Mathf.Min(innerAngleRad, this.GetOuterAngleRad(animationIndex, frameIndex, orientationIndex));
            if (this.innerAnglesRad[animationIndex][frameIndex][orientationIndex] != innerAngleRad)
            {
                FrigidEdit.RecordChanges(this);
                this.innerAnglesRad[animationIndex][frameIndex][orientationIndex] = innerAngleRad;
            }
        }

        public float GetOuterAngleRad(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.outerAnglesRad[animationIndex][frameIndex][orientationIndex];
        }

        public void SetOuterAngleRad(int animationIndex, int frameIndex, int orientationIndex, float outerAngleRad)
        {
            outerAngleRad = Mathf.Max(outerAngleRad, this.GetInnerAngleRad(animationIndex, frameIndex, orientationIndex));
            if (this.outerAnglesRad[animationIndex][frameIndex][orientationIndex] != outerAngleRad)
            {
                FrigidEdit.RecordChanges(this);
                this.outerAnglesRad[animationIndex][frameIndex][orientationIndex] = outerAngleRad;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.light = FrigidEdit.AddComponent<Light2D>(this.gameObject);
            // Nasty hack since sorting layers are not public.
            FieldInfo fieldInfo = this.light.GetType().GetField("m_ApplyToSortingLayers", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(this.light, new int[] { SortingLayer.NameToID(FrigidSortingLayer.World.ToString()) });
            this.LightType = Light2D.LightType.Point;
            this.freeformShapePaths = new List<FreeformShapePath>();
            this.lightColors = new Nested2DList<ColorSerializedReference>();
            this.innerRadii = new Nested3DList<float>();
            this.outerRadii = new Nested3DList<float>();
            this.innerAnglesRad = new Nested3DList<float>();
            this.outerAnglesRad = new Nested3DList<float>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.freeformShapePaths.Add(new FreeformShapePath(this.NumberFreeformPoints));
                this.lightColors.Add(new Nested1DList<ColorSerializedReference>());
                this.innerRadii.Add(new Nested2DList<float>());
                this.outerRadii.Add(new Nested2DList<float>());
                this.innerAnglesRad.Add(new Nested2DList<float>());
                this.outerAnglesRad.Add(new Nested2DList<float>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.lightColors[animationIndex].Add(new ColorSerializedReference());
                    this.innerRadii[animationIndex].Add(new Nested1DList<float>());
                    this.outerRadii[animationIndex].Add(new Nested1DList<float>());
                    this.innerAnglesRad[animationIndex].Add(new Nested1DList<float>());
                    this.outerAnglesRad[animationIndex].Add(new Nested1DList<float>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.innerRadii[animationIndex][frameIndex].Add(0);
                        this.outerRadii[animationIndex][frameIndex].Add(0);
                        this.innerAnglesRad[animationIndex][frameIndex].Add(0);
                        this.outerAnglesRad[animationIndex][frameIndex].Add(0);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.freeformShapePaths.Insert(animationIndex, new FreeformShapePath(this.NumberFreeformPoints));
            this.lightColors.Insert(animationIndex, new Nested1DList<ColorSerializedReference>());
            this.innerRadii.Insert(animationIndex, new Nested2DList<float>());
            this.outerRadii.Insert(animationIndex, new Nested2DList<float>());
            this.innerAnglesRad.Insert(animationIndex, new Nested2DList<float>());
            this.outerAnglesRad.Insert(animationIndex, new Nested2DList<float>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.lightColors[animationIndex].Add(new ColorSerializedReference());
                this.innerRadii[animationIndex].Add(new Nested1DList<float>());
                this.outerRadii[animationIndex].Add(new Nested1DList<float>());
                this.innerAnglesRad[animationIndex].Add(new Nested1DList<float>());
                this.outerAnglesRad[animationIndex].Add(new Nested1DList<float>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.innerRadii[animationIndex][frameIndex].Add(0);
                    this.outerRadii[animationIndex][frameIndex].Add(0);
                    this.innerAnglesRad[animationIndex][frameIndex].Add(0);
                    this.outerAnglesRad[animationIndex][frameIndex].Add(0);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.freeformShapePaths.RemoveAt(animationIndex);
            this.lightColors.RemoveAt(animationIndex);
            this.innerRadii.RemoveAt(animationIndex);
            this.outerRadii.RemoveAt(animationIndex);
            this.innerAnglesRad.RemoveAt(animationIndex);
            this.outerAnglesRad.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.lightColors[animationIndex].Insert(frameIndex, new ColorSerializedReference());
            this.innerRadii[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            this.outerRadii[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            this.innerAnglesRad[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            this.outerAnglesRad[animationIndex].Insert(frameIndex, new Nested1DList<float>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.innerRadii[animationIndex][frameIndex].Add(0);
                this.outerRadii[animationIndex][frameIndex].Add(0);
                this.innerAnglesRad[animationIndex][frameIndex].Add(0);
                this.outerAnglesRad[animationIndex][frameIndex].Add(0);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.lightColors[animationIndex].RemoveAt(frameIndex);
            this.innerRadii[animationIndex].RemoveAt(frameIndex);
            this.outerRadii[animationIndex].RemoveAt(frameIndex);
            this.innerAnglesRad[animationIndex].RemoveAt(frameIndex);
            this.outerAnglesRad[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.innerRadii[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.outerRadii[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.innerAnglesRad[animationIndex][frameIndex].Insert(orientationIndex, 0);
            this.outerAnglesRad[animationIndex][frameIndex].Insert(orientationIndex, 0);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.innerRadii[animationIndex][frameIndex].RemoveAt(orientationIndex);
            this.outerRadii[animationIndex][frameIndex].RemoveAt(orientationIndex);
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
                        otherLightProperty.SetFreeformPointAt(toAnimationIndex, pointIndex, this.GetFreeformPointAt(fromAnimationIndex, pointIndex));
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
                otherLightProperty.SetLightColorByReference(toAnimationIndex, toFrameIndex, new ColorSerializedReference(this.GetLightColorByReference(fromAnimationIndex, fromFrameIndex)));

                float myOuterRadius = this.GetOuterRadius(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                float myInnerRadius = this.GetInnerRadius(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                otherLightProperty.SetInnerRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, 0);
                otherLightProperty.SetOuterRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, 0);
                otherLightProperty.SetOuterRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, myOuterRadius);
                otherLightProperty.SetInnerRadius(toAnimationIndex, toFrameIndex, toOrientationIndex, myInnerRadius);

                float myOuterAngleRad = this.GetOuterAngleRad(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                float myInnerAngleRad = this.GetInnerAngleRad(fromAnimationIndex, fromFrameIndex, fromOrientationIndex);
                otherLightProperty.SetInnerAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, 0);
                otherLightProperty.SetOuterAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, 0);
                otherLightProperty.SetOuterAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, myOuterAngleRad);
                otherLightProperty.SetInnerAngleRad(toAnimationIndex, toFrameIndex, toOrientationIndex, myInnerAngleRad);
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void Initialize()
        {
            this.light.enabled = false;
            base.Initialize();
        }

        public override void Enable(bool enabled)
        {
            this.light.enabled = enabled;
            base.Enable(enabled);
        }

        public override void AnimationEnter()
        {
            if (this.LightType == Light2D.LightType.Freeform)
            {
                for (int pointIndex = 0; pointIndex < this.NumberFreeformPoints; pointIndex++)
                {
                    this.light.shapePath[pointIndex] = this.GetFreeformPointAt(this.Body.CurrAnimationIndex, pointIndex);
                }
            }
            base.AnimationEnter();
        }

        public override void AnimationExit()
        {
            if (this.LightType == Light2D.LightType.Freeform)
            {
                for (int pointIndex = 0; pointIndex < this.NumberFreeformPoints; pointIndex++)
                {
                    this.light.shapePath[pointIndex] = Vector2.zero;
                }
            }
            base.AnimationExit();
        }

        public override void FrameEnter()
        {
            this.light.color = this.GetLightColorByReference(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex).MutableValue;
            base.FrameEnter();
        }

        public override void FrameExit()
        {
            this.light.color = Color.clear;
            base.FrameExit();
        }

        public override void OrientationEnter()
        {
            if (this.LightType == Light2D.LightType.Point)
            {
                this.light.pointLightInnerRadius = 0;
                this.light.pointLightOuterRadius = 0;
                this.light.pointLightInnerAngle = 0;
                this.light.pointLightOuterAngle = 0;
                this.light.pointLightOuterRadius = this.GetOuterRadius(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                this.light.pointLightInnerRadius = this.GetInnerRadius(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
                this.light.pointLightOuterAngle = this.GetOuterAngleRad(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex) * Mathf.Rad2Deg;
                this.light.pointLightInnerAngle = this.GetInnerAngleRad(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex) * Mathf.Rad2Deg;
            }
            base.OrientationEnter();
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

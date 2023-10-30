using FrigidBlackwaters.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class MaterialTweenOptionSet : IEquatable<MaterialTweenOptionSet>
    {
        [SerializeField]
        private TweenOptionSet tweenRoutine;
        [SerializeField]
        private bool setToOriginValueAfterIteration;
        [SerializeField]
        private string propertyId;
        [SerializeField]
        private MaterialProperties.Type propertyType;
        [SerializeField]
        [ShowIfInt("propertyType", 0, true)]
        private Color originColor;
        [SerializeField]
        [ShowIfInt("propertyType", 0, true)]
        private Color targetColor;
        [SerializeField]
        [ShowIfInt("propertyType", 2, true)]
        private float originFloat;
        [SerializeField]
        [ShowIfInt("propertyType", 2, true)]
        private float targetFloat;

        public MaterialTweenOptionSet()
        {
            this.tweenRoutine = new TweenOptionSet();
            this.setToOriginValueAfterIteration = false;
            this.propertyId = string.Empty;
            this.propertyType = MaterialProperties.Type.Color;
            this.originColor = Color.white;
            this.targetColor = Color.white;
            this.originFloat = 0;
            this.targetFloat = 0;
        }

        public MaterialTweenOptionSet(MaterialTweenOptionSet other)
        {
            this.tweenRoutine = new TweenOptionSet(other.tweenRoutine);
            this.setToOriginValueAfterIteration = other.setToOriginValueAfterIteration;
            this.propertyId = other.propertyId;
            this.propertyType = other.propertyType;
            this.originColor = other.originColor;
            this.targetColor = other.targetColor;
            this.originFloat = other.originFloat;
            this.targetFloat = other.targetFloat;
        }

        public MaterialTweenOptionSet(
            TweenOptionSet tweenRoutine,
            bool setToOriginValueAfterIteration,
            string propertyId,
            MaterialProperties.Type propertyType,
            Color originColor,
            Color targetColor,
            float originFloat,
            float targetFloat
            )
        {
            this.tweenRoutine = tweenRoutine;
            this.setToOriginValueAfterIteration = setToOriginValueAfterIteration;
            this.propertyId = propertyId;
            this.propertyType = propertyType;
            this.originColor = originColor;
            this.targetColor = targetColor;
            this.originFloat = originFloat;
            this.targetFloat = targetFloat;
        }

        public TweenOptionSet TweenRoutine
        {
            get
            {
                return this.tweenRoutine;
            }
        }

        public bool SetToOriginValueAfterIteration
        {
            get
            {
                return this.setToOriginValueAfterIteration;
            }
        }

        public string PropertyId
        {
            get
            {
                return this.propertyId;
            }
        }

        public MaterialProperties.Type PropertyType
        {
            get
            {
                return this.propertyType;
            }
        }

        public Color OriginColor
        {
            get
            {
                return this.originColor;
            }
        }

        public Color TargetColor
        {
            get
            {
                return this.targetColor;
            }
        }

        public float OriginFloat
        {
            get
            {
                return this.originFloat;
            }
        }

        public float TargetFloat
        {
            get
            {
                return this.targetFloat;
            }
        }

        public IEnumerator<FrigidCoroutine.Delay> MakeRoutine(Renderer renderer, Action onComplete = null)
        {
            return 
                this.tweenRoutine.MakeRoutine(
                    onUpdate: (float progress01) =>
                    {
                        object value = default;
                        switch (this.propertyType)
                        {
                            case MaterialProperties.Type.Color:
                                value = (this.targetColor - this.originColor) * progress01 + this.originColor;
                                break;
                            case MaterialProperties.Type.Float:
                                value = (this.targetFloat - this.originFloat) * progress01 + this.originFloat;
                                break;
                        }
                        MaterialProperties.SetProperty(renderer, this.propertyType, this.propertyId, value);
                    },
                    onIterationComplete:
                    () =>
                    {
                        if (this.setToOriginValueAfterIteration)
                        {
                            object value = default;
                            switch (this.propertyType)
                            {
                                case MaterialProperties.Type.Color:
                                    value = this.originColor;
                                    break;
                                case MaterialProperties.Type.Float:
                                    value = originFloat;
                                    break;
                            }
                            MaterialProperties.SetProperty(renderer, this.propertyType, this.propertyId, value);
                        }
                    },
                    onComplete : onComplete
                    );
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as MaterialTweenOptionSet);
        }

        public bool Equals(MaterialTweenOptionSet other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;

            bool propertiesEqual = this.propertyType == other.propertyType;
            switch (this.propertyType)
            {
                case MaterialProperties.Type.Color: propertiesEqual &= this.originColor == other.originColor && this.targetColor == other.targetColor; break;
                case MaterialProperties.Type.Float: propertiesEqual &= this.originFloat == other.originFloat && this.targetFloat == other.targetFloat; break;
            }

            return this.tweenRoutine == other.tweenRoutine && this.setToOriginValueAfterIteration == other.setToOriginValueAfterIteration && propertiesEqual;
        }

        public override int GetHashCode()
        {
            switch (this.propertyType)
            {
                case MaterialProperties.Type.Color:
                    return (this.tweenRoutine, this.setToOriginValueAfterIteration, this.originColor, this.targetColor).GetHashCode();
                case MaterialProperties.Type.Boolean:
                    return (this.tweenRoutine, this.setToOriginValueAfterIteration).GetHashCode();
                case MaterialProperties.Type.Float:
                    return (this.tweenRoutine, this.setToOriginValueAfterIteration, this.originFloat, this.targetFloat).GetHashCode();
            }
            return 0;
        }

        public static bool operator ==(MaterialTweenOptionSet lhs, MaterialTweenOptionSet rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(MaterialTweenOptionSet lhs, MaterialTweenOptionSet rhs)
        {
            return !(lhs == rhs);
        }
    }
}

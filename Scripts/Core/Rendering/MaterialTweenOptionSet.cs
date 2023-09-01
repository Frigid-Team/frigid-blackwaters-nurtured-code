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
        private MaterialProperties.ColorProperty colorProperty;
        [SerializeField]
        private Color originColor;
        [SerializeField]
        private Color targetColor;

        public MaterialTweenOptionSet()
        {
            this.tweenRoutine = new TweenOptionSet();
            this.setToOriginValueAfterIteration = false;
            this.colorProperty = MaterialProperties.ColorProperty.Color;
            this.originColor = Color.white;
            this.targetColor = Color.white;
        }

        public MaterialTweenOptionSet(MaterialTweenOptionSet other)
        {
            this.tweenRoutine = new TweenOptionSet(other.tweenRoutine);
            this.setToOriginValueAfterIteration = other.setToOriginValueAfterIteration;
            this.colorProperty = other.colorProperty;
            this.originColor = other.originColor;
            this.targetColor = other.targetColor;
        }

        public MaterialTweenOptionSet(
            TweenOptionSet tweenRoutine,
            bool setToOriginValueAfterIteration,
            MaterialProperties.ColorProperty colorProperty,
            Color originColor,
            Color targetColor
            )
        {
            this.tweenRoutine = tweenRoutine;
            this.setToOriginValueAfterIteration = setToOriginValueAfterIteration;
            this.colorProperty = colorProperty;
            this.originColor = originColor;
            this.targetColor = targetColor;
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

        public MaterialProperties.ColorProperty ColorProperty
        {
            get
            {
                return this.colorProperty;
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

        public static MaterialTweenOptionSet ColorSwap(MaterialProperties.ColorProperty colorProperty, Color color)
        {
            return new MaterialTweenOptionSet(TweenOptionSet.Delay(0f), false, colorProperty, color, color);
        }

        public IEnumerator<FrigidCoroutine.Delay> MakeRoutine(Renderer renderer, Action onComplete = null)
        {
            return 
                this.tweenRoutine.MakeRoutine(
                    onUpdate: (float progress01) =>
                    {
                        MaterialProperties.SetColor(renderer, this.colorProperty, (this.targetColor - this.originColor) * progress01 + this.originColor);
                    },
                    onIterationComplete:
                    () =>
                    {
                        if (this.setToOriginValueAfterIteration)
                        {
                            MaterialProperties.SetColor(renderer, this.colorProperty, this.originColor);
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

            return
                this.tweenRoutine == other.tweenRoutine &&
                this.setToOriginValueAfterIteration == other.setToOriginValueAfterIteration &&
                this.colorProperty == other.colorProperty &&
                this.originColor == other.originColor && 
                this.targetColor == other.targetColor;
        }

        public override int GetHashCode()
        {
            return (this.tweenRoutine, this.setToOriginValueAfterIteration, this.colorProperty, this.originColor, this.targetColor).GetHashCode();
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

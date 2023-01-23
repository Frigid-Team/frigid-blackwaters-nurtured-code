using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class MaterialTweenCoroutineTemplate : IEquatable<MaterialTweenCoroutineTemplate>
    {
        [SerializeField]
        private TweenCoroutineTemplate tweenRoutine;
        [SerializeField]
        private bool setToOriginValueAfterIteration;
        [SerializeField]
        private MaterialParameters.ColorParameter colorParameter;
        [SerializeField]
        private Color originColor;
        [SerializeField]
        private Color targetColor;

        public MaterialTweenCoroutineTemplate()
        {
            this.tweenRoutine = new TweenCoroutineTemplate();
            this.setToOriginValueAfterIteration = false;
            this.colorParameter = MaterialParameters.ColorParameter.MainColor;
            this.originColor = Color.white;
            this.targetColor = Color.white;
        }

        public MaterialTweenCoroutineTemplate(MaterialTweenCoroutineTemplate other)
        {
            this.tweenRoutine = new TweenCoroutineTemplate(other.tweenRoutine);
            this.setToOriginValueAfterIteration = other.setToOriginValueAfterIteration;
            this.colorParameter = other.colorParameter;
            this.originColor = other.originColor;
            this.targetColor = other.targetColor;
        }

        public MaterialTweenCoroutineTemplate(
            TweenCoroutineTemplate tweenRoutine,
            bool setToOriginValueAfterIteration,
            MaterialParameters.ColorParameter tweenValue, 
            Color originColor,
            Color targetColor
            )
        {
            this.tweenRoutine = tweenRoutine;
            this.setToOriginValueAfterIteration = setToOriginValueAfterIteration;
            this.colorParameter = tweenValue;
            this.originColor = originColor;
            this.targetColor = targetColor;
        }

        public TweenCoroutineTemplate TweenRoutine
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

        public MaterialParameters.ColorParameter ColorParameter
        {
            get
            {
                return this.colorParameter;
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

        public IEnumerator<FrigidCoroutine.Delay> GetRoutine(Material material, Action onComplete = null)
        {
            return 
                this.tweenRoutine.GetRoutine(
                    onUpdate: (float progress01) => MaterialParameters.SetColor(material, this.colorParameter, (this.targetColor - this.originColor) * progress01 + this.originColor),
                    onIterationComplete:
                    () =>
                    {
                        if (this.setToOriginValueAfterIteration)
                        {
                            MaterialParameters.SetColor(material, this.colorParameter, this.originColor);
                        }
                    },
                    onComplete : onComplete
                    );
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MaterialTweenCoroutineTemplate);
        }

        public bool Equals(MaterialTweenCoroutineTemplate other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return
                this.tweenRoutine == other.tweenRoutine &&
                this.setToOriginValueAfterIteration == other.setToOriginValueAfterIteration &&
                this.colorParameter == other.colorParameter &&
                this.originColor == other.originColor && 
                this.targetColor == other.targetColor;
        }

        public override int GetHashCode()
        {
            return (this.tweenRoutine, this.setToOriginValueAfterIteration, this.colorParameter, this.originColor, this.targetColor).GetHashCode();
        }

        public static bool operator ==(MaterialTweenCoroutineTemplate lhs, MaterialTweenCoroutineTemplate rhs)
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

        public static bool operator !=(MaterialTweenCoroutineTemplate lhs, MaterialTweenCoroutineTemplate rhs)
        {
            return !(lhs == rhs);
        }
    }
}

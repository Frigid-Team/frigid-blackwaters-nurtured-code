using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class TweenCoroutineTemplate : IEquatable<TweenCoroutineTemplate>
    {
        [SerializeField]
        private FloatSerializedReference iterationDuration;
        [SerializeField]
        private EasingType easingType;
        [SerializeField]
        private bool loopInfinitely;
        [SerializeField]
        [ShowIfBool("loopInfinitely", false)]
        private IntSerializedReference additionalNumberIterations;
        [SerializeField]
        private FloatSerializedReference initialElapsedDuration;
        [SerializeField]
        private FloatSerializedReference durationBetweenIterations;
        [SerializeField]
        private bool pingPong;

        public TweenCoroutineTemplate()
        {
            this.iterationDuration = new FloatSerializedReference();
            this.easingType = EasingType.Linear;
            this.loopInfinitely = false;
            this.additionalNumberIterations = new IntSerializedReference();
            this.initialElapsedDuration = new FloatSerializedReference();
            this.durationBetweenIterations = new FloatSerializedReference();
            this.pingPong = false;
        }

        public TweenCoroutineTemplate(TweenCoroutineTemplate other)
        {
            this.iterationDuration = new FloatSerializedReference(other.iterationDuration);
            this.easingType = other.easingType;
            this.loopInfinitely = other.loopInfinitely;
            this.additionalNumberIterations = new IntSerializedReference(other.additionalNumberIterations);
            this.initialElapsedDuration = new FloatSerializedReference(other.initialElapsedDuration);
            this.durationBetweenIterations = new FloatSerializedReference(other.durationBetweenIterations);
            this.pingPong = other.pingPong;
        }

        public TweenCoroutineTemplate(
            FloatSerializedReference iterationDuration,
            EasingType easingType, 
            bool loopInfinitely,
            IntSerializedReference additionalNumberIterations, 
            FloatSerializedReference initialElapsedDuration,
            FloatSerializedReference durationBetweenIterations,
            bool pingPong
            )
        {
            this.iterationDuration = iterationDuration;
            this.easingType = easingType;
            this.loopInfinitely = loopInfinitely;
            this.additionalNumberIterations = additionalNumberIterations;
            this.initialElapsedDuration = initialElapsedDuration;
            this.durationBetweenIterations = durationBetweenIterations;
            this.pingPong = pingPong;
        }

        public FloatSerializedReference IterationDurationByReference
        {
            get
            {
                return this.iterationDuration;
            }
        }

        public EasingType EasingType
        {
            get
            {
                return this.easingType;
            }
        }

        public bool LoopInfinitely
        {
            get
            {
                return this.loopInfinitely;
            }
        }

        public IntSerializedReference AdditionalNumberIterationsByReference
        {
            get
            {
                return this.additionalNumberIterations;
            }
        }

        public FloatSerializedReference InitialElapsedDurationByReference
        {
            get
            {
                return this.initialElapsedDuration;
            }
        }

        public FloatSerializedReference DurationBetweenIterationsByReference
        {
            get
            {
                return this.durationBetweenIterations;
            }
        }

        public bool PingPong
        {
            get
            {
                return this.pingPong;
            }
        }

        public IEnumerator<FrigidCoroutine.Delay> GetRoutine(
            bool useRealTime = false,
            Action<float> onUpdate = null,
            Action onIterationBegin = null,
            Action onIterationComplete = null,
            Action onComplete = null
            )
        {
            return TweenCoroutine.Value(
                this.iterationDuration.MutableValue,
                this.easingType,
                this.loopInfinitely,
                1 + this.additionalNumberIterations.MutableValue,
                this.durationBetweenIterations.MutableValue,
                !this.loopInfinitely ? this.initialElapsedDuration.MutableValue : 0,
                this.pingPong,
                useRealTime,
                onUpdate,
                onIterationBegin,
                onIterationComplete,
                onComplete
                );
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TweenCoroutineTemplate);
        }

        public bool Equals(TweenCoroutineTemplate other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return
                this.iterationDuration == other.iterationDuration &&
                this.easingType == other.easingType &&
                this.loopInfinitely == other.loopInfinitely &&
                (!this.loopInfinitely || this.additionalNumberIterations == other.additionalNumberIterations) &&
                this.initialElapsedDuration == other.initialElapsedDuration &&
                this.durationBetweenIterations == other.durationBetweenIterations &&
                this.pingPong == other.pingPong;
        }

        public override int GetHashCode()
        {
            if (this.loopInfinitely)
            {
                return (this.iterationDuration, this.easingType, this.initialElapsedDuration, this.durationBetweenIterations, this.pingPong).GetHashCode();
            }
            else
            {
                return (this.iterationDuration, this.easingType, this.additionalNumberIterations, this.initialElapsedDuration, this.durationBetweenIterations, this.pingPong).GetHashCode();
            }
        }

        public static bool operator ==(TweenCoroutineTemplate lhs, TweenCoroutineTemplate rhs)
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

        public static bool operator !=(TweenCoroutineTemplate lhs, TweenCoroutineTemplate rhs)
        {
            return !(lhs == rhs);
        }
    }
}

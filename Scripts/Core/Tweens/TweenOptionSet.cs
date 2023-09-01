using FrigidBlackwaters.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    [Serializable]
    public class TweenOptionSet : IEquatable<TweenOptionSet>
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

        public TweenOptionSet()
        {
            this.iterationDuration = new FloatSerializedReference();
            this.easingType = EasingType.Linear;
            this.loopInfinitely = false;
            this.additionalNumberIterations = new IntSerializedReference();
            this.initialElapsedDuration = new FloatSerializedReference();
            this.durationBetweenIterations = new FloatSerializedReference();
            this.pingPong = false;
        }

        public TweenOptionSet(TweenOptionSet other)
        {
            this.iterationDuration = new FloatSerializedReference(other.iterationDuration);
            this.easingType = other.easingType;
            this.loopInfinitely = other.loopInfinitely;
            this.additionalNumberIterations = new IntSerializedReference(other.additionalNumberIterations);
            this.initialElapsedDuration = new FloatSerializedReference(other.initialElapsedDuration);
            this.durationBetweenIterations = new FloatSerializedReference(other.durationBetweenIterations);
            this.pingPong = other.pingPong;
        }

        public TweenOptionSet(
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

        public static TweenOptionSet Delay(float duration)
        {
            return new TweenOptionSet(
                new FloatSerializedReference(SerializedReferenceType.Custom, 0, null, 0, 0, new List<float>(), null), 
                EasingType.Linear, 
                false, 
                new IntSerializedReference(SerializedReferenceType.Custom, 0, null, 0, 0, new List<int>(), null),
                new FloatSerializedReference(SerializedReferenceType.Custom, -duration, null, 0, 0, new List<float>(), null),
                new FloatSerializedReference(SerializedReferenceType.Custom, 0, null, 0, 0, new List<float>(), null), 
                false
                );
        }

        public bool TryCalculateTotalDuration(out float totalDuration)
        {
            totalDuration = (this.iterationDuration.ImmutableValue + this.durationBetweenIterations.ImmutableValue) * (1 + this.additionalNumberIterations.ImmutableValue) - this.initialElapsedDuration.ImmutableValue;
            return !this.loopInfinitely;
        }

        public IEnumerator<FrigidCoroutine.Delay> MakeRoutine(
            bool useRealTime = false,
            Action<float> onUpdate = null,
            Action onIterationBegin = null,
            Action onIterationComplete = null,
            Action onComplete = null
            )
        {
            return Tween.Value(
                this.iterationDuration.ImmutableValue,
                this.easingType,
                this.loopInfinitely,
                1 + this.additionalNumberIterations.ImmutableValue,
                this.durationBetweenIterations.ImmutableValue,
                !this.loopInfinitely ? this.initialElapsedDuration.ImmutableValue : 0,
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
            return this.Equals(obj as TweenOptionSet);
        }

        public bool Equals(TweenOptionSet other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;

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

        public static bool operator ==(TweenOptionSet lhs, TweenOptionSet rhs)
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

        public static bool operator !=(TweenOptionSet lhs, TweenOptionSet rhs)
        {
            return !(lhs == rhs);
        }
    }
}

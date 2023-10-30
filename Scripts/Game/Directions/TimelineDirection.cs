using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TimelineDirection : Direction
    {
        [SerializeField]
        private List<TimeStamp> timeStamps;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            float cumulativeDuration = 0;
            foreach (TimeStamp timeStamp in this.timeStamps)
            {
                if (elapsedDuration <= cumulativeDuration + timeStamp.ActiveDuration)
                {
                    return timeStamp.Direction.Retrieve(currDirections, elapsedDuration, elapsedDurationDelta);
                }
                cumulativeDuration += timeStamp.ActiveDuration;
            }
            return currDirections;
        }

        [Serializable]
        public struct TimeStamp
        {
            [SerializeField]
            private Direction direction;
            [SerializeField]
            private FloatSerializedReference activeDuration;

            public Direction Direction
            {
                get
                {
                    return this.direction;
                }
            }

            public float ActiveDuration
            {
                get
                {
                    return this.activeDuration.ImmutableValue;
                }
            }
        }
    }
}

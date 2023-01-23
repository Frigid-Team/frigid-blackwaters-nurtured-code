using System.Collections.Generic;
using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TimelineDirection : Direction
    {
        [SerializeField]
        private bool useRecentNonZeroDirection;
        [SerializeField]
        private List<TimeStamp> timeStamps;

        public override Vector2[] Calculate(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            float cumulativeDuration = 0;
            foreach (TimeStamp timeStamp in this.timeStamps)
            {
                if (elapsedDuration < cumulativeDuration + timeStamp.ActiveDuration)
                {
                    return timeStamp.Direction.Calculate(currDirections, elapsedDuration, elapsedDurationDelta);
                }
                cumulativeDuration += timeStamp.ActiveDuration;
            }
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++) directions[i] = Vector2.zero;
            return directions;
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

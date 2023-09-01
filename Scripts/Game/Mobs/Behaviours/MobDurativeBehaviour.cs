using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDurativeBehaviour : MobBehaviour
    {
        [SerializeField]
        private FloatSerializedReference delayDuration;
        [SerializeField]
        private FloatSerializedReference behaveDuration;
        [SerializeField]
        private List<MobBehaviour> behaviours;

        private float elapsedDuration;
        private float warmupDuration;
        private float totalDuration;

        public override bool IsFinished
        {
            get
            {
                return this.elapsedDuration >= this.totalDuration;
            }
        }

        public override void Added()
        {
            base.Added();
            this.elapsedDuration = 0;
            this.warmupDuration = Mathf.Max(0, this.delayDuration.MutableValue);
            this.totalDuration = Mathf.Max(0, this.behaveDuration.MutableValue) + this.warmupDuration;
            if (this.warmupDuration == 0 && this.totalDuration > 0)
            {
                foreach (MobBehaviour behaviour in this.behaviours)
                {
                    this.Owner.AddBehaviour(behaviour, this.Owner.GetIsIgnoringTimeScale(this));
                }
            }
        }

        public override void Removed()
        {
            base.Removed();
            if (this.elapsedDuration >= this.warmupDuration && this.elapsedDuration < this.totalDuration)
            {
                foreach (MobBehaviour behaviour in this.behaviours)
                {
                    this.Owner.RemoveBehaviour(behaviour);
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            float nextElapsedDuration = this.elapsedDuration + this.EnterDurationDelta;
            if (this.elapsedDuration < this.warmupDuration && nextElapsedDuration >= this.warmupDuration)
            {
                foreach (MobBehaviour behaviour in this.behaviours)
                {
                    this.Owner.AddBehaviour(behaviour, this.Owner.GetIsIgnoringTimeScale(this));
                }
            }
            else if (this.elapsedDuration < this.totalDuration && nextElapsedDuration >= this.totalDuration)
            {
                foreach (MobBehaviour behaviour in this.behaviours)
                {
                    this.Owner.RemoveBehaviour(behaviour);
                }
            }
            this.elapsedDuration = nextElapsedDuration;
        }
    }
}

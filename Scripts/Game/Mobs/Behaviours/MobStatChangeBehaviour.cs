using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobStatChangeBehaviour : MobBehaviour
    {
        [SerializeField]
        private List<StatChange> statChanges;

        public override void Enter()
        {
            base.Enter();
            foreach(StatChange statChange in this.statChanges)
            {
                this.Owner.SetStatAmount(statChange.StatChanged, this.Owner.GetStatAmount(statChange.StatChanged) + statChange.Value.ImmutableValue);
            }
        }

        public override void Exit()
        {
            base.Exit();
            foreach (StatChange statChange in this.statChanges)
            {
                this.Owner.SetStatAmount(statChange.StatChanged, this.Owner.GetStatAmount(statChange.StatChanged) - statChange.Value.ImmutableValue);
            }
        }

        [Serializable]
        private struct StatChange
        {
            [SerializeField]
            private MobStat stat;
            [SerializeField]
            private IntSerializedReference value;

            public MobStat StatChanged
            {
                get
                {
                    return this.stat;
                }
            }
            public IntSerializedReference Value
            {
                get
                {
                    return this.value;
                }
            }
        }
    }
}
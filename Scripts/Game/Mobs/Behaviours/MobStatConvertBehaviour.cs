using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobStatConvertBehaviour : MobBehaviour
    {
        [SerializeField]
        private List<StatConversion> statConversions;

        private Dictionary<StatConversion, Action<int, int>> statCallbacks;

        protected override void Awake()
        {
            base.Awake();
            this.statCallbacks = new Dictionary<StatConversion, Action<int, int>>();
        }

        public override void Enter()
        {
            base.Enter();
            foreach (StatConversion statConversion in this.statConversions)
            {
                this.Owner.SetStatAmount(
                    MobStatLayer.Secondary,
                    statConversion.ModifiedStat,
                    this.Owner.GetStatAmount(MobStatLayer.Secondary, statConversion.ModifiedStat) + Mathf.FloorToInt(this.Owner.GetStatAmount(MobStatLayer.Primary, statConversion.StatConverted) * statConversion.PercentConverted.ImmutableValue)
                    );

                Action<int, int> statCallback = (int before, int after) => this.UpdateStat(before, after, statConversion);
                this.statCallbacks.Add(statConversion, statCallback);
                this.Owner.SubscribeToStatChange(MobStatLayer.Primary, statConversion.StatConverted, statCallback);
            }
        }

        private void UpdateStat(int before, int after, StatConversion statConversion)
        {
            this.Owner.SetStatAmount(MobStatLayer.Secondary, statConversion.ModifiedStat, this.Owner.GetStatAmount(MobStatLayer.Secondary, statConversion.ModifiedStat) + Mathf.RoundToInt((after - before) * statConversion.PercentConverted.ImmutableValue));
        }

        public override void Exit()
        {
            base.Exit();
            foreach (StatConversion statConversion in this.statConversions)
            {
                this.Owner.SetStatAmount(
                    MobStatLayer.Secondary,
                    statConversion.ModifiedStat,
                    this.Owner.GetStatAmount(MobStatLayer.Secondary, statConversion.ModifiedStat) - Mathf.FloorToInt(this.Owner.GetStatAmount(MobStatLayer.Primary, statConversion.StatConverted) * statConversion.PercentConverted.ImmutableValue)
                    );

                Action<int, int> statCallback = this.statCallbacks[statConversion];
                this.Owner.UnsubscribeToStatChange(MobStatLayer.Primary, statConversion.StatConverted, statCallback);
                this.statCallbacks.Remove(statConversion);
            }
        }

        [Serializable]
        private struct StatConversion
        {
            [SerializeField]
            private MobStat modifiedStat;
            [SerializeField]
            private MobStat statConverted;
            [SerializeField]
            private FloatSerializedReference percentConverted;

            public MobStat ModifiedStat
            {
                get
                {
                    return this.modifiedStat;
                }
            }
            public MobStat StatConverted
            {
                get
                {
                    return this.statConverted;
                }
            }

            public FloatSerializedReference PercentConverted
            {
                get
                {
                    return this.percentConverted;
                }
            }
        }
    }
}
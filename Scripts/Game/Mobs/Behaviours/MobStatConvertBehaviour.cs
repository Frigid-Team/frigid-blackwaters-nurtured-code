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
                int convertedStatAmount = this.Owner.GetStatAmount(MobStatLayer.Primary, statConversion.StatConverted);
                this.Owner.SetStatAmount(
                    MobStatLayer.Secondary,
                    statConversion.ModifiedStat,
                    this.Owner.GetStatAmount(MobStatLayer.Secondary, statConversion.ModifiedStat) + Mathf.FloorToInt(convertedStatAmount * statConversion.PercentConverted.ImmutableValue)
                    );

                Action<int, int> statCallback = (int amountBefore, int amountAfter) => this.UpdateStat(amountBefore, amountAfter, statConversion);
                this.statCallbacks.Add(statConversion, statCallback);
                this.Owner.SubscribeToStatChange(MobStatLayer.Primary, statConversion.StatConverted, statCallback);
            }
        }

        private void UpdateStat(int amountBefore, int amountAfter, StatConversion statConversion)
        {
            this.Owner.SetStatAmount(
                MobStatLayer.Secondary,
                statConversion.ModifiedStat,
                this.Owner.GetStatAmount(MobStatLayer.Secondary, statConversion.ModifiedStat) - Mathf.FloorToInt(amountBefore * statConversion.PercentConverted.ImmutableValue)
                );
            this.Owner.SetStatAmount(
                MobStatLayer.Secondary, 
                statConversion.ModifiedStat, 
                this.Owner.GetStatAmount(MobStatLayer.Secondary, statConversion.ModifiedStat) + Mathf.FloorToInt(amountAfter * statConversion.PercentConverted.ImmutableValue)
                );
        }

        public override void Exit()
        {
            base.Exit();
            foreach (StatConversion statConversion in this.statConversions)
            {
                int convertedStatAmount = this.Owner.GetStatAmount(MobStatLayer.Primary, statConversion.StatConverted);
                this.Owner.SetStatAmount(
                    MobStatLayer.Secondary,
                    statConversion.ModifiedStat,
                    this.Owner.GetStatAmount(MobStatLayer.Secondary, statConversion.ModifiedStat) - Mathf.FloorToInt(convertedStatAmount * statConversion.PercentConverted.ImmutableValue)
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
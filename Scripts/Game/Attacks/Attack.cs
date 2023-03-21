using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class Attack : FrigidMonoBehaviour
    {
        [SerializeField]
        private DamageAlignment damageAlignment;

        private int damageBonus;
        private Action<HitInfo> onHitDealt;
        private Action<BreakInfo> onBreakDealt;
        private Action<ThreatInfo> onThreatDealt;

        public DamageAlignment DamageAlignment
        {
            get
            {
                return this.damageAlignment;
            }
            set
            {
                this.damageAlignment = value;
            }
        }

        public int DamageBonus
        {
            get
            {
                return this.damageBonus;
            }
            set
            {
                this.damageBonus = value;
            }
        }

        public Action<HitInfo> OnHitDealt
        {
            get
            {
                return this.onHitDealt;
            }
            set
            {
                this.onHitDealt = value;
            }
        }

        public Action<BreakInfo> OnBreakDealt
        {
            get
            {
                return this.onBreakDealt;
            }
            set
            {
                this.onBreakDealt = value;
            }
        }

        public Action<ThreatInfo> OnThreatDealt
        {
            get
            {
                return this.onThreatDealt;
            }
            set
            {
                this.onThreatDealt = value;
            }
        }

        public abstract void Perform(float elapsedDuration);
    }
}

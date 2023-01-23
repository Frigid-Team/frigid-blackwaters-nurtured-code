using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageReceiverBox<RB, DB, I> : FrigidMonoBehaviour where RB : DamageReceiverBox<RB, DB, I> where DB : DamageDealerBox<DB, RB, I>
    {
        [SerializeField]
        private DamageAlignment damageAlignment;
        [SerializeField]
        private bool isIgnoringDamage;
        [SerializeField]
        private FloatSerializedReference bufferDuration;

        private Action<I> onReceived;
        private float lastReceiveTime;

        public bool IsIgnoringDamage
        {
            get
            {
                return this.isIgnoringDamage;
            }
            set
            {
                this.isIgnoringDamage = value;
            }
        }

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

        public Action<I> OnReceived
        {
            get
            {
                return this.onReceived;
            }
            set
            {
                this.onReceived = value;
            }
        }

        public bool TryDamage(DB damageDealerBox, Vector2 position, Vector2 direction, out I info)
        {
            if (this.isIgnoringDamage ||
                this.damageAlignment == DamageAlignment.None ||
                damageDealerBox.DamageAlignment == DamageAlignment.None || 
                this.damageAlignment == damageDealerBox.DamageAlignment || 
                Time.time - this.lastReceiveTime < this.bufferDuration.ImmutableValue)
            {
                info = default(I);
                return false;
            }
            this.lastReceiveTime = Time.time;
            info = ProcessDamage(damageDealerBox, position, direction);
            this.onReceived?.Invoke(info);
            return true;
        }

        protected abstract I ProcessDamage(DB damageDealerBox, Vector2 position, Vector2 direction);

        protected override void Awake()
        {
            base.Awake();
            this.lastReceiveTime = 0;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

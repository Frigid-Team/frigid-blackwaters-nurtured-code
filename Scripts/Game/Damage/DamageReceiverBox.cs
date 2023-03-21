using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageReceiverBox<RB, DB, I> : FrigidMonoBehaviourWithPhysics where RB : DamageReceiverBox<RB, DB, I> where DB : DamageDealerBox<DB, RB, I> where I : DamageInfo
    {
        [SerializeField]
        private DamageAlignment damageAlignment;
        [SerializeField]
        private DamageChannel damageChannel;
        [SerializeField]
        private bool isIgnoringDamage;
        [SerializeField]
        private FloatSerializedReference bufferDuration;
        [SerializeField]
        private FloatSerializedReference pauseDuration;

        private bool isIgnoringDamageLastFixedTimeStep;
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

        public DamageChannel DamageChannel
        {
            get
            {
                return this.damageChannel;
            }
            set
            {
                this.damageChannel = value;
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

        public bool TryDamage(DB damageDealerBox, Vector2 position, Vector2 direction, Collider2D collision, out I info)
        {
            if (this.isIgnoringDamageLastFixedTimeStep || 
                !DamageDetectionTable.CanInteract(damageDealerBox.DamageAlignment, this.DamageAlignment, damageDealerBox.DamageChannel, this.DamageChannel) ||
                Time.time - this.lastReceiveTime < this.bufferDuration.ImmutableValue)
            {
                info = default(I);
                return false;
            }
            this.lastReceiveTime = Time.time;
            info = ProcessDamage(damageDealerBox, position, direction, collision);
            this.onReceived?.Invoke(info);
            float pauseDuration = this.pauseDuration.MutableValue;
            if (pauseDuration > 0)
            {
                TimePauser.Paused.Request();
                FrigidCoroutine.Run(TweenCoroutine.DelayedCall(pauseDuration, TimePauser.Paused.Release, true));
            }
            return true;
        }
        protected abstract I ProcessDamage(DB damageDealerBox, Vector2 position, Vector2 direction, Collider2D collision);

        protected override void Awake()
        {
            base.Awake();
            this.isIgnoringDamageLastFixedTimeStep = this.isIgnoringDamage;
            this.lastReceiveTime = 0;
        }

        protected override void FixedUpdate() 
        {
            base.FixedUpdate();
            this.isIgnoringDamageLastFixedTimeStep = this.isIgnoringDamage;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

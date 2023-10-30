using System;
using System.Collections.Generic;
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
        private Dictionary<DB, float> recentReceives;

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
                this.recentReceives.ContainsKey(damageDealerBox))
            {
                info = default(I);
                return false;
            }
            this.recentReceives.Add(damageDealerBox, Time.fixedTime);
            info = this.ProcessDamage(damageDealerBox, position, direction, collision);
            this.onReceived?.Invoke(info);
            float pauseDuration = this.pauseDuration.MutableValue;
            if (pauseDuration > 0)
            {
                TimePauser.Paused.Request();
                FrigidCoroutine.Run(Tween.Delay(pauseDuration, TimePauser.Paused.Release, true));
            }
            return true;
        }
        protected abstract I ProcessDamage(DB damageDealerBox, Vector2 position, Vector2 direction, Collider2D collision);

        protected override void Awake()
        {
            base.Awake();
            this.isIgnoringDamageLastFixedTimeStep = this.isIgnoringDamage;
            this.recentReceives = new Dictionary<DB, float>();
        }

        protected override void FixedUpdate() 
        {
            base.FixedUpdate();
            this.isIgnoringDamageLastFixedTimeStep = this.isIgnoringDamage;

            DB[] expiredDamageDealerBoxes = new DB[this.recentReceives.Count];
            int numberExpired = 0;
            float bufferDuration = this.bufferDuration.ImmutableValue;
            foreach (KeyValuePair<DB, float> recentReceive in this.recentReceives)
            {
                DB damageDealerBox = recentReceive.Key;
                float timeReceived = recentReceive.Value;

                if (Time.fixedTime - timeReceived >= bufferDuration)
                {
                    expiredDamageDealerBoxes[numberExpired] = damageDealerBox;
                    numberExpired++;
                }
            }
            for (int i = 0; i < numberExpired; i++)
            {
                this.recentReceives.Remove(expiredDamageDealerBoxes[i]);
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageDealerBox<DB, RB, I> : FrigidMonoBehaviour where DB : DamageDealerBox<DB, RB, I> where RB : DamageReceiverBox<RB, DB, I>
    {
        [SerializeField]
        private DamageAlignment damageAlignment;
        [SerializeField]
        private bool isIgnoringDamage;

        private Action<I> onDealt;

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

        public Action<I> OnDealt
        {
            get
            {
                return this.onDealt;
            }
            set
            {
                this.onDealt = value;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!this.isIgnoringDamage && collision.TryGetComponent<RB>(out RB damageReceiverBox))
            {
                if (damageReceiverBox.TryDamage(
                    (DB)this, 
                    collision.ClosestPoint(this.transform.position),
                    (collision.bounds.center - this.transform.position).normalized, 
                    out I info 
                    ))
                {
                    this.onDealt?.Invoke(info);
                }
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

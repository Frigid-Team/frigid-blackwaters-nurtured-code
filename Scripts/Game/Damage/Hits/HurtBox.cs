using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class HurtBox : DamageReceiverBox<HurtBox, HitBox, HitInfo>
    {
        private static SceneVariable<Dictionary<HitPopup, RecyclePool<HitPopup>>> hitPopupPools;

        [SerializeField]
        private HitPopup hitPopupPrefab;

        private int damageMitigation;
        private List<HitModifier> hitModifiers;

        static HurtBox()
        {
            hitPopupPools = new SceneVariable<Dictionary<HitPopup, RecyclePool<HitPopup>>>(() => new Dictionary<HitPopup, RecyclePool<HitPopup>>());
        }

        public int DamageMitigation
        {
            get
            {
                return this.damageMitigation;
            }
            set
            {
                this.damageMitigation = value;
            }
        }

        public void AddHitModifier(HitModifier hitModifier)
        {
            this.hitModifiers.Add(hitModifier);
        }

        public void RemoveHitModifier(HitModifier hitModifier)
        {
            this.hitModifiers.Remove(hitModifier);
        }

        protected override HitInfo ProcessDamage(HitBox hitBox, Vector2 position, Vector2 direction, Collider2D collision)
        {
            HitInfo hitInfo = new HitInfo(
                hitBox.BaseDamage,
                hitBox.DamageBonus,
                this.damageMitigation,
                position,
                direction,
                this.hitModifiers,
                collision
                );
            if (!hitPopupPools.Current.ContainsKey(this.hitPopupPrefab))
            {
                hitPopupPools.Current.Add(
                    this.hitPopupPrefab,
                    new RecyclePool<HitPopup>(
                        () => FrigidInstancing.CreateInstance(this.hitPopupPrefab),
                        (HitPopup hitPopup) => FrigidInstancing.DestroyInstance(hitPopup)
                        )
                    );
            }
            HitPopup spawnedHitPopup = hitPopupPools.Current[this.hitPopupPrefab].Retrieve();
            spawnedHitPopup.ShowHit(hitInfo, () => hitPopupPools.Current[this.hitPopupPrefab].Pool(spawnedHitPopup));
            return hitInfo;
        }

        protected override void Awake()
        {
            base.Awake();
            this.damageMitigation = 0;
            this.hitModifiers = new List<HitModifier>();
        }
    }
}

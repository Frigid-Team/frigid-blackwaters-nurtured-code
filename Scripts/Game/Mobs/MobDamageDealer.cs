using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDamageDealer
    {
        private AnimatorBody animatorBody;
        private MobStats stats;

        private CountingSemaphore stopDealingDamage;

        private Action<HitInfo> onHitDealt;
        private Action<BreakInfo> onBreakDealt;
        private Action<ThreatInfo> onThreatDealt;

        private LinkedList<HitInfo> hitsDealt;
        private LinkedList<BreakInfo> breaksDealt;
        private LinkedList<ThreatInfo> threatsDealt;

        public MobDamageDealer(DamageAlignment alignment, AnimatorBody animatorBody, MobStats stats)
        {
            this.animatorBody = animatorBody;
            this.stats = stats;

            this.stats.GetStatValue(MobStat.Might).OnAmountChanged += UpdateDamageBonusFromMightChange;
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageAlignment = alignment;
                hitBoxProperty.OnDealt += HitDealt;
                hitBoxProperty.DamageBonus += DamageBonusFromMight(this.stats.GetStatValue(MobStat.Might).Amount);
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = alignment;
                breakBoxProperty.OnDealt += BreakDealt;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.DamageAlignment = alignment;
                threatBoxProperty.OnDealt += ThreatDealt;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageAlignment = alignment;
                attackProperty.OnHitDealt += HitDealt;
                attackProperty.OnBreakDealt += BreakDealt;
                attackProperty.OnThreatDealt += ThreatDealt;
                attackProperty.DamageBonus += DamageBonusFromMight(this.stats.GetStatValue(MobStat.Might).Amount);
            } 

            this.stopDealingDamage = new CountingSemaphore();
            this.stopDealingDamage.OnFirstRequest += DisableDamageDealers;
            this.stopDealingDamage.OnLastRelease += EnableDamageDealers;

            this.hitsDealt = new LinkedList<HitInfo>();
            this.breaksDealt = new LinkedList<BreakInfo>();
            this.threatsDealt = new LinkedList<ThreatInfo>();

            EnableDamageDealers();
        }

        public CountingSemaphore StopDealingDamage
        {
            get
            {
                return this.stopDealingDamage;
            }
        }

        public int CurrentDamageBonusFromMight
        {
            get
            {
                return DamageBonusFromMight(this.stats.GetStatValue(MobStat.Might).Amount);
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

        public LinkedList<HitInfo> HitsDealt
        {
            get
            {
                return this.hitsDealt;
            }
        }

        public LinkedList<BreakInfo> BreaksDealt
        {
            get
            {
                return this.breaksDealt;
            }
        }

        public LinkedList<ThreatInfo> ThreatsDealt
        {
            get
            {
                return this.threatsDealt;
            }
        }

        public void HitDealt(HitInfo hitInfo)
        {
            this.hitsDealt.AddFirst(hitInfo);
            this.onHitDealt?.Invoke(hitInfo);
        }

        public void BreakDealt(BreakInfo breakInfo)
        {
            this.breaksDealt.AddFirst(breakInfo);
            this.onBreakDealt?.Invoke(breakInfo);
        }

        public void ThreatDealt(ThreatInfo threatInfo)
        {
            this.threatsDealt.AddFirst(threatInfo);
            this.onThreatDealt?.Invoke(threatInfo);
        }

        private void EnableDamageDealers()
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>()) hitBoxProperty.IsIgnoringDamage = false;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>()) breakBoxProperty.IsIgnoringDamage = false;
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>()) threatBoxProperty.IsIgnoringDamage = false;
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>()) attackProperty.ForceStop = false;
        }

        private void DisableDamageDealers()
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>()) hitBoxProperty.IsIgnoringDamage = true;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetProperties<BreakBoxAnimatorProperty>()) breakBoxProperty.IsIgnoringDamage = true;
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetProperties<ThreatBoxAnimatorProperty>()) threatBoxProperty.IsIgnoringDamage = true;
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>()) attackProperty.ForceStop = true;
        }

        private void UpdateDamageBonusFromMightChange(int previousMight, int currentMight)
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageBonus += DamageBonusFromMight(currentMight) - DamageBonusFromMight(previousMight);
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageBonus += DamageBonusFromMight(currentMight) - DamageBonusFromMight(previousMight);
            }
        }

        private int DamageBonusFromMight(int might)
        {
            return might;
        }
    }
}

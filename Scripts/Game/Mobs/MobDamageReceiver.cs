using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDamageReceiver
    {
        private AnimatorBody animatorBody;
        private MobStats stats;

        private CountingSemaphore stopReceivingDamage;

        private Action<HitInfo> onHitReceived;
        private Action<BreakInfo> onBreakReceived;
        private Action<ThreatInfo> onThreatReceived;

        private LinkedList<HitInfo> hitsReceived;
        private LinkedList<BreakInfo> breaksReceived;
        private LinkedList<ThreatInfo> threatsReceived;

        public MobDamageReceiver(DamageAlignment alignment, AnimatorBody animatorBody, MobStats stats)
        {
            this.animatorBody = animatorBody;
            this.stats = stats;

            this.stats.GetStatValue(MobStat.Armour).OnAmountChanged += UpdateDamageMitigationFromArmourChange;
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in animatorBody.GetProperties<HurtBoxAnimatorProperty>())
            {
                hurtBoxProperty.DamageAlignment = alignment;
                hurtBoxProperty.OnReceived += HitReceived;
                hurtBoxProperty.DamageMitigation += DamageMitigationFromArmour(this.stats.GetStatValue(MobStat.Armour).Amount);
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in animatorBody.GetProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.DamageAlignment = alignment;
                resistBoxProperty.OnReceived += BreakReceived;
            }
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in animatorBody.GetProperties<LookoutBoxAnimatorProperty>())
            {
                lookoutBoxProperty.DamageAlignment = alignment;
                lookoutBoxProperty.OnReceived += ThreatReceived;
            }

            this.stopReceivingDamage = new CountingSemaphore();
            this.stopReceivingDamage.OnFirstRequest += DisableDamageReceivers;
            this.stopReceivingDamage.OnLastRelease += EnableDamageReceivers;

            this.hitsReceived = new LinkedList<HitInfo>();
            this.breaksReceived = new LinkedList<BreakInfo>();
            this.threatsReceived = new LinkedList<ThreatInfo>();

            EnableDamageReceivers();
        }

        public CountingSemaphore StopReceivingDamage
        {
            get
            {
                return this.stopReceivingDamage;
            }
        }

        public int CurrentDamageMitigationFromArmour
        {
            get
            {
                return DamageMitigationFromArmour(this.stats.GetStatValue(MobStat.Armour).Amount);
            }
        }

        public Action<HitInfo> OnHitReceived
        {
            get
            {
                return this.onHitReceived;
            }
            set
            {
                this.onHitReceived = value;
            }
        }

        public Action<BreakInfo> OnBreakReceived
        {
            get
            {
                return this.onBreakReceived;
            }
            set
            {
                this.onBreakReceived = value;
            }
        }

        public Action<ThreatInfo> OnThreatReceived
        {
            get
            {
                return this.onThreatReceived;
            }
            set
            {
                this.onThreatReceived = value;
            }
        }

        public LinkedList<HitInfo> HitsReceived
        {
            get
            {
                return this.hitsReceived;
            }
        }

        public LinkedList<BreakInfo> BreaksReceived
        {
            get
            {
                return this.breaksReceived;
            }
        }

        public LinkedList<ThreatInfo> ThreatsReceived
        {
            get
            {
                return this.threatsReceived;
            }
        }

        public void HitReceived(HitInfo hitInfo)
        {
            this.hitsReceived.AddFirst(hitInfo);
            this.onHitReceived?.Invoke(hitInfo);
        }

        public void BreakReceived(BreakInfo breakInfo)
        {
            this.breaksReceived.AddFirst(breakInfo);
            this.onBreakReceived?.Invoke(breakInfo);
        }

        public void ThreatReceived(ThreatInfo threatInfo)
        {
            this.threatsReceived.AddFirst(threatInfo);
            this.onThreatReceived?.Invoke(threatInfo);
        }

        private void EnableDamageReceivers()
        {
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetProperties<HurtBoxAnimatorProperty>()) hurtBoxProperty.IsIgnoringDamage = false;
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetProperties<ResistBoxAnimatorProperty>()) resistBoxProperty.IsIgnoringDamage = false;
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetProperties<LookoutBoxAnimatorProperty>()) lookoutBoxProperty.IsIgnoringDamage = false;
        }

        private void DisableDamageReceivers()
        {
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.animatorBody.GetProperties<HurtBoxAnimatorProperty>()) hurtBoxProperty.IsIgnoringDamage = true;
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.animatorBody.GetProperties<ResistBoxAnimatorProperty>()) resistBoxProperty.IsIgnoringDamage = true;
            foreach (LookoutBoxAnimatorProperty lookoutBoxProperty in this.animatorBody.GetProperties<LookoutBoxAnimatorProperty>()) lookoutBoxProperty.IsIgnoringDamage = true;
        }

        private void UpdateDamageMitigationFromArmourChange(int previousArmour, int currentArmour)
        {
            foreach (HurtBoxAnimatorProperty hurtBoxAnimatorProperty in this.animatorBody.GetProperties<HurtBoxAnimatorProperty>())
            {
                hurtBoxAnimatorProperty.DamageMitigation += DamageMitigationFromArmour(currentArmour) - DamageMitigationFromArmour(previousArmour);
            }
        }

        private int DamageMitigationFromArmour(int armour)
        {
            return armour;
        }
    }
}

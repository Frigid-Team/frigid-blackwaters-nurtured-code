using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobHealth
    {
        private int tempHealth;
        private int currentHealth;
        private int maxHealth;

        private Action<int, int> onCurrentHealthChanged;
        private Action<int, int> onMaxHealthChanged;

        public MobHealth(int maxHealth, MobStats stats)
        {
            this.tempHealth = 0;
            this.currentHealth = maxHealth;
            this.maxHealth = maxHealth;

            UpdateMaxHealth(MaxHealthBonusFromVitality(stats.GetStatValue(MobStat.Vitality).Amount));
            stats.GetStatValue(MobStat.Vitality).OnAmountChanged += UpdateMaxHealthBonusFromVitalityChange;
        }

        public int TempHealth
        {
            get
            {
                return this.tempHealth;
            }
        }

        public int CurrentHealth
        {
            get
            {
                return this.currentHealth;
            }
        }

        public int MaxHealth
        {
            get
            {
                return this.maxHealth;
            }
        }

        public Action<int, int> OnCurrentHealthChanged
        {
            get
            {
                return this.onCurrentHealthChanged;
            }
            set
            {
                this.onCurrentHealthChanged = value;
            }
        }

        public Action<int, int> OnMaxHealthChanged
        {
            get
            {
                return this.onMaxHealthChanged;
            }
            set
            {
                this.onMaxHealthChanged = value;
            }
        }

        public void GrantTempHealth(int tempHealth)
        {
            if (this.currentHealth <= 0 || tempHealth <= 0) return;
            UpdateTempHealth(tempHealth);
        }

        public void Damage(int damage)
        {
            if (this.currentHealth <= 0 || damage <= 0) return;
            int previousTempHealth = this.tempHealth;
            UpdateTempHealth(-damage);
            damage -= previousTempHealth - this.tempHealth;
            UpdateCurrentHealth(-damage);
        }

        public void Heal(int heal)
        {
            if (this.currentHealth <= 0 || heal <= 0) return;
            UpdateCurrentHealth(heal);
        }

        private void UpdateTempHealth(int tempHealthDelta = 0)
        {
            int previousTempHealth = this.tempHealth;
            this.tempHealth = Mathf.Max(this.tempHealth + tempHealthDelta, 0);
            int previousCurrentHealth = this.currentHealth;
            UpdateMaxHealth(this.tempHealth - previousTempHealth);
            UpdateCurrentHealth(this.tempHealth - previousTempHealth - (this.currentHealth - previousCurrentHealth));
        }

        private void UpdateCurrentHealth(int currentHealthDelta = 0)
        {
            int previousCurrentHealth = this.currentHealth;
            this.currentHealth = Mathf.Clamp(this.currentHealth + currentHealthDelta, 0, this.maxHealth);
            if (previousCurrentHealth != this.currentHealth) this.onCurrentHealthChanged?.Invoke(previousCurrentHealth, this.currentHealth);
        }

        private void UpdateMaxHealth(int maxHealthDelta = 0)
        {
            int previousMaxHealth = this.maxHealth;
            this.maxHealth = Mathf.Max(this.maxHealth + maxHealthDelta, 1);
            UpdateCurrentHealth();
            if (previousMaxHealth != this.maxHealth) this.onMaxHealthChanged?.Invoke(previousMaxHealth, this.maxHealth);
        }

        private void UpdateMaxHealthBonusFromVitalityChange(int previousVitality, int currentVitality)
        {
            UpdateMaxHealth(MaxHealthBonusFromVitality(currentVitality) - MaxHealthBonusFromVitality(previousVitality));
        }

        private int MaxHealthBonusFromVitality(int vitality)
        {
            return vitality;
        }
    }
}

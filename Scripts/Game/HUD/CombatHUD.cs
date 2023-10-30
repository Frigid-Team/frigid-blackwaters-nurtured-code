using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class CombatHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private BarHUD healthBarHUD;
        [SerializeField]
        private List<HealthBarAlignmentSetting> healthBarAlignmentSettings;
        [SerializeField]
        private BarHUD dashBarHUD;
        [SerializeField]
        private BarHUD equipmentBarHUD;
        [SerializeField]
        private Text equipmentQuantityText;

        private FrigidCoroutine equipmentHUDRoutine;
        private FrigidCoroutine dashHUDRoutine;

        protected override void Awake()
        {
            base.Awake();
            this.equipmentQuantityText.text = "";
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerMob.OnExists += this.Startup;
            PlayerMob.OnUnexists += this.Teardown;
            this.Startup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerMob.OnExists -= this.Startup;
            PlayerMob.OnUnexists -= this.Teardown;
            this.Teardown();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void Startup()
        {
            if (PlayerMob.TryGet(out PlayerMob player)) 
            {
                player.OnMaxHealthChanged += this.UpdateHealthMaximumOnHUD;
                player.OnRemainingHealthChanged += this.UpdateHealthRemainingOnHUD;
                player.OnDashResourceChange += this.UpdateDashResourceOnHUD;
                player.OnEquipChange += this.UpdateEquippedEquipmentOnHUD;

                this.healthBarHUD.MainBarColor = Color.clear;
                this.healthBarHUD.BufferBarColor = Color.clear;
                foreach (HealthBarAlignmentSetting healthBarAlignmentSetting in this.healthBarAlignmentSettings)
                {
                    if (healthBarAlignmentSetting.DamageAlignment == player.Alignment)
                    {
                        this.healthBarHUD.MainBarColor = healthBarAlignmentSetting.MainBarColor;
                        this.healthBarHUD.BufferBarColor = healthBarAlignmentSetting.BufferBarColor;
                        break;
                    }
                }

                this.UpdateHealthMaximumOnHUD(0, player.MaxHealth);
                this.UpdateHealthRemainingOnHUD(0, player.RemainingHealth);
                this.UpdateDashResourceOnHUD(null, player.GetDashResource());
                this.UpdateEquippedEquipmentOnHUD(false, null, player.TryGetEquippedEquipment(out MobEquipment equippedEquipment), equippedEquipment);
            }
        }

        private void Teardown()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                player.OnMaxHealthChanged -= this.UpdateHealthMaximumOnHUD;
                player.OnRemainingHealthChanged -= this.UpdateHealthRemainingOnHUD;
                player.OnDashResourceChange -= this.UpdateDashResourceOnHUD;
                player.OnEquipChange -= this.UpdateEquippedEquipmentOnHUD;
            }
            FrigidCoroutine.Kill(this.equipmentHUDRoutine);
            FrigidCoroutine.Kill(this.dashHUDRoutine);
        }

        private void UpdateHealthMaximumOnHUD(int previousMaxHealth, int currentMaxHealth)
        {
            this.healthBarHUD.SetMaximum(currentMaxHealth);
        }

        private void UpdateHealthRemainingOnHUD(int previousRemainingHealth, int currentRemainingHealth)
        {
            this.healthBarHUD.SetCurrent(currentRemainingHealth);
        }

        private void UpdateEquippedEquipmentOnHUD(bool hasPrevious, MobEquipment previousEquippedEquipment, bool hasCurrent, MobEquipment currentEquippedEquipment)
        {
            FrigidCoroutine.Kill(this.equipmentHUDRoutine);
            if (hasCurrent && currentEquippedEquipment.AbilityResources.Count > 0)
            {
                AbilityResource abilityResource = currentEquippedEquipment.AbilityResources[0];
                this.equipmentBarHUD.Transition(Mathf.FloorToInt(abilityResource.Progress * 100), 100);
                IEnumerator<FrigidCoroutine.Delay> ShowEquipmentOnHUD()
                {
                    while (true)
                    {
                        this.equipmentBarHUD.SetCurrent(Mathf.FloorToInt(abilityResource.Progress * 100));
                        this.equipmentQuantityText.text = abilityResource.Quantity >= 0 ? abilityResource.Quantity.ToString() : "-";
                        yield return null;
                    }
                }
                this.equipmentHUDRoutine = FrigidCoroutine.Run(ShowEquipmentOnHUD());
                return;
            }
            this.equipmentBarHUD.Transition(0, 100);
            this.equipmentQuantityText.text = string.Empty;
            this.equipmentHUDRoutine = null;
        }

        private void UpdateDashResourceOnHUD(AbilityResource previousDashResource, AbilityResource currentDashResource)
        {
            FrigidCoroutine.Kill(this.dashHUDRoutine);
            this.dashBarHUD.Transition(Mathf.FloorToInt(currentDashResource.Progress * 100), 100);
            IEnumerator<FrigidCoroutine.Delay> ShowDashProgressOnHUD()
            {
                while (true)
                {
                    this.dashBarHUD.SetCurrent(Mathf.FloorToInt(currentDashResource.Progress * 100));
                    yield return null;
                }
            }
            this.dashHUDRoutine = FrigidCoroutine.Run(ShowDashProgressOnHUD());
        }

        [Serializable]
        private struct HealthBarAlignmentSetting
        {
            [SerializeField]
            private DamageAlignment damageAlignment;
            [SerializeField]
            private ColorSerializedReference mainBarColor;
            [SerializeField]
            private ColorSerializedReference bufferBarColor;

            public DamageAlignment DamageAlignment
            {
                get
                {
                    return this.damageAlignment;
                }
            }

            public Color MainBarColor
            {
                get
                {
                    return this.mainBarColor.ImmutableValue;
                }
            }

            public Color BufferBarColor
            {
                get
                {
                    return this.bufferBarColor.ImmutableValue;
                }
            }
        }
    }
}

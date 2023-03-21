using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class CombatHUD : FrigidMonoBehaviour
    {
        [SerializeField]
        private BarHUD healthBarHUD;
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
            PlayerMob.OnExists += Startup;
            PlayerMob.OnUnexists += Teardown;
            Startup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerMob.OnExists -= Startup;
            PlayerMob.OnUnexists -= Teardown;
            Teardown();
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void Startup()
        {
            if (PlayerMob.TryGet(out PlayerMob player)) 
            {
                player.OnMaxHealthChanged += UpdateHealthMaximumOnHUD;
                player.OnRemainingHealthChanged += UpdateHealthRemainingOnHUD;
                player.OnDashTimerChange += UpdateDashTimerOnHUD;
                player.OnEquipChange += UpdateEquippedPieceOnHUD;

                UpdateHealthMaximumOnHUD(0, player.MaxHealth);
                UpdateHealthRemainingOnHUD(0, player.RemainingHealth);
                UpdateDashTimerOnHUD(null, player.GetDashTimer());
                UpdateEquippedPieceOnHUD(false, null, player.TryGetEquippedPiece(out MobEquipmentPiece equippedPiece), equippedPiece);
            }
        }

        private void Teardown()
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                player.OnMaxHealthChanged -= UpdateHealthMaximumOnHUD;
                player.OnRemainingHealthChanged -= UpdateHealthRemainingOnHUD;
                player.OnDashTimerChange -= UpdateDashTimerOnHUD;
                player.OnEquipChange -= UpdateEquippedPieceOnHUD;
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

        private void UpdateEquippedPieceOnHUD(bool hasPrevious, MobEquipmentPiece previousEquippedPiece, bool hasCurrent, MobEquipmentPiece currentEquippedPiece)
        {
            FrigidCoroutine.Kill(this.equipmentHUDRoutine);
            this.equipmentBarHUD.Transition(Mathf.FloorToInt(currentEquippedPiece.Cooldown.Progress * 100), 100);
            IEnumerator<FrigidCoroutine.Delay> ShowEquipmentOnHUD()
            {
                while (true)
                {
                    this.equipmentBarHUD.SetCurrent(Mathf.FloorToInt(currentEquippedPiece.Cooldown.Progress * 100));
                    this.equipmentQuantityText.text = currentEquippedPiece.Cooldown.Quantity.ToString();
                    yield return null;
                }
            }
            this.equipmentHUDRoutine = FrigidCoroutine.Run(ShowEquipmentOnHUD());
        }

        private void UpdateDashTimerOnHUD(Timer previousDashTimer, Timer currentDashTimer)
        {
            FrigidCoroutine.Kill(this.dashHUDRoutine);
            this.dashBarHUD.Transition(Mathf.FloorToInt(currentDashTimer.Progress * 100), 100);
            IEnumerator<FrigidCoroutine.Delay> ShowDashProgressOnHUD()
            {
                while (true)
                {
                    this.dashBarHUD.SetCurrent(Mathf.FloorToInt(currentDashTimer.Progress * 100));
                    yield return null;
                }
            }
            this.dashHUDRoutine = FrigidCoroutine.Run(ShowDashProgressOnHUD());
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobEquipment : FrigidMonoBehaviour
    {
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private MobEquipmentStateNode rootStateNode;
        [SerializeField]
        private MaterialTweenOptionSetSerializedReference materialTweenOnSwitch;

        private List<AbilityResource> abilityResources;

        private MobEquipPoint equipPoint;
        private bool equipped;

        public MobEquipPoint EquipPoint
        {
            get
            {
                return this.equipPoint;
            }
        }

        public Vector2 FacingDirection
        {
            get
            {
                return this.animatorBody.Direction;
            }
        }

        public bool IsFiring
        {
            get
            {
                return this.rootStateNode.CurrentState.IsFiring;
            }
        }

        public List<AbilityResource> AbilityResources
        {
            get
            {
                return this.abilityResources;
            }
        }

        public void OwnedBy(MobEquipPoint equipPoint)
        {
            if (this.equipPoint != equipPoint)
            {
                if (this.equipPoint != null)
                {
                    this.equipPoint.Equipper.OnRequestedTimeScaleChanged -= this.HandleEquipperTimeScaleChange;
                    this.equipPoint.OnEquipChange -= this.ShowEquipPointSwitch;
                }
                this.equipPoint = equipPoint;
                if (this.equipPoint != null)
                {
                    this.equipPoint.Equipper.OnRequestedTimeScaleChanged += this.HandleEquipperTimeScaleChange;
                    this.equipPoint.OnEquipChange += this.ShowEquipPointSwitch;
                    this.HandleEquipperTimeScaleChange();
                }
            }
        }

        public void Spawn()
        {
            this.rootStateNode.OwnedBy(this, this.animatorBody);
            this.abilityResources = new List<AbilityResource>();
            this.rootStateNode.CollectAbilityResources(ref this.abilityResources);
            this.rootStateNode.Spawn();

            this.equipped = false;
            FrigidCoroutine.Run(this.Refresh(), this.gameObject);
        }

        public void Equip()
        {
            this.equipped = true;

            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageAlignment = this.EquipPoint.Equipper.Alignment;
                hitBoxProperty.OnDealt += this.EquipPoint.Equipper.HitDealt;
                hitBoxProperty.DamageBonus += Mob.DamageBonusFromMight(this.EquipPoint.Equipper.GetTotalStatAmount(MobStat.Might));
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = this.EquipPoint.Equipper.Alignment;
                breakBoxProperty.OnDealt += this.EquipPoint.Equipper.BreakDealt;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.DamageAlignment = this.EquipPoint.Equipper.Alignment;
                threatBoxProperty.OnDealt += this.EquipPoint.Equipper.ThreatDealt;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageAlignment = this.EquipPoint.Equipper.Alignment;
                attackProperty.OnHitDealt += this.EquipPoint.Equipper.HitDealt;
                attackProperty.OnBreakDealt += this.EquipPoint.Equipper.BreakDealt;
                attackProperty.OnThreatDealt += this.EquipPoint.Equipper.ThreatDealt;
                attackProperty.DamageBonus += Mob.DamageBonusFromMight(this.EquipPoint.Equipper.GetTotalStatAmount(MobStat.Might));
            }
            this.EquipPoint.Equipper.SubscribeToTotalStatChange(MobStat.Might, this.UpdateDamageBonusFromMightChange);

            this.EquipPoint.Equipper.StopDealingDamage.OnFirstRequest += this.DisableDamageDealers;
            this.EquipPoint.Equipper.StopDealingDamage.OnLastRelease += this.EnableDamageDealers;
            if (this.EquipPoint.Equipper.StopDealingDamage)
            {
                this.DisableDamageDealers();
            }
            else
            {
                this.EnableDamageDealers();
            }

            this.rootStateNode.OnCurrentStateChanged += this.HandleNewState;
            this.rootStateNode.Equipped();
            this.rootStateNode.Enter();
        }

        public void Unequip()
        {
            this.equipped = false;

            this.rootStateNode.Exit();
            this.rootStateNode.Unequipped();
            this.rootStateNode.OnCurrentStateChanged -= this.HandleNewState;

            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageAlignment = DamageAlignment.None;
                hitBoxProperty.OnDealt -= this.EquipPoint.Equipper.HitDealt;
                hitBoxProperty.DamageBonus -= Mob.DamageBonusFromMight(this.EquipPoint.Equipper.GetTotalStatAmount(MobStat.Might));
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.DamageAlignment = DamageAlignment.None;
                breakBoxProperty.OnDealt -= this.EquipPoint.Equipper.BreakDealt;
            }
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>())
            {
                threatBoxProperty.DamageAlignment = DamageAlignment.None;
                threatBoxProperty.OnDealt -= this.EquipPoint.Equipper.ThreatDealt;
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageAlignment = DamageAlignment.None;
                attackProperty.OnHitDealt -= this.EquipPoint.Equipper.HitDealt;
                attackProperty.OnBreakDealt -= this.EquipPoint.Equipper.BreakDealt;
                attackProperty.OnThreatDealt -= this.EquipPoint.Equipper.ThreatDealt;
                attackProperty.DamageBonus -= Mob.DamageBonusFromMight(this.EquipPoint.Equipper.GetTotalStatAmount(MobStat.Might));
            }
            this.EquipPoint.Equipper.UnsubscribeToTotalStatChange(MobStat.Might, this.UpdateDamageBonusFromMightChange);

            this.EquipPoint.Equipper.StopDealingDamage.OnFirstRequest -= this.DisableDamageDealers;
            this.EquipPoint.Equipper.StopDealingDamage.OnLastRelease -= this.EnableDamageDealers;

            this.animatorBody.Stop();
        }

        private IEnumerator<FrigidCoroutine.Delay> Refresh()
        {
            while (true)
            {
                if (this.equipped)
                {
                    this.rootStateNode.Refresh();
                }
                yield return null;
            }
        }

        private void HandleNewState(MobEquipmentState previousEquipmentState, MobEquipmentState currentEquipmentState) { }

        private void HandleEquipperTimeScaleChange()
        {
            this.animatorBody.TimeScale = this.EquipPoint.Equipper.RequestedTimeScale;
        }

        private void ShowEquipPointSwitch(bool hasPrevious, MobEquipment previousEquippedEquipment, bool hasCurrent, MobEquipment currentEquippedEquipment)
        {
            if (hasCurrent && currentEquippedEquipment == this)
            {
                foreach (SpriteAnimatorProperty spriteProperty in this.animatorBody.GetReferencedProperties<SpriteAnimatorProperty>())
                {
                    spriteProperty.OneShotMaterialTween(this.materialTweenOnSwitch.MutableValue);
                }
            }
        }

        private void UpdateDamageBonusFromMightChange(int previousMight, int currentMight)
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>())
            {
                hitBoxProperty.DamageBonus += Mob.DamageBonusFromMight(currentMight) - Mob.DamageBonusFromMight(previousMight);
            }
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>())
            {
                attackProperty.DamageBonus += Mob.DamageBonusFromMight(currentMight) - Mob.DamageBonusFromMight(previousMight);
            }
        }

        private void EnableDamageDealers()
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>()) hitBoxProperty.IsIgnoringDamage = false;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>()) breakBoxProperty.IsIgnoringDamage = false;
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>()) threatBoxProperty.IsIgnoringDamage = false;
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>()) attackProperty.IsIgnoringDamage = false;
        }

        private void DisableDamageDealers()
        {
            foreach (HitBoxAnimatorProperty hitBoxProperty in this.animatorBody.GetReferencedProperties<HitBoxAnimatorProperty>()) hitBoxProperty.IsIgnoringDamage = true;
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.animatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>()) breakBoxProperty.IsIgnoringDamage = true;
            foreach (ThreatBoxAnimatorProperty threatBoxProperty in this.animatorBody.GetReferencedProperties<ThreatBoxAnimatorProperty>()) threatBoxProperty.IsIgnoringDamage = true;
            foreach (AttackAnimatorProperty attackProperty in this.animatorBody.GetReferencedProperties<AttackAnimatorProperty>()) attackProperty.IsIgnoringDamage = true;
        }
    }
}

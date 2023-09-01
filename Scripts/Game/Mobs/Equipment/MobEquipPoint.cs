using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [RequireComponent(typeof(SortingGroupAnimatorProperty))]
    public class MobEquipPoint : FrigidMonoBehaviour
    {
        [SerializeField]
        private MobEquipContext equipContext;
        [SerializeField]
        private SortingGroupAnimatorProperty sortingGroupProperty;
        [SerializeField]
        private List<Usage> usages;
        [Space]
        [SerializeField]
        private List<MobEquipmentSpawnable> existingEquipmentSpawnables;

        private List<MobEquipment> equipments;
        private int equipIndex;
        private Action<bool, MobEquipment, bool, MobEquipment> onEquipChange;

        private Usage currentUsage;

        private Mob equipper;
        private MobStateNode equipperRootStateNode;

        public MobEquipContext EquipContext
        {
            get
            {
                return this.equipContext;
            }
        }

        public Vector2 AimPosition
        {
            get
            {
                return this.currentUsage != null ? this.currentUsage.AimTargeter.Retrieve(Vector2.zero, 0, 0) : (this.Equipper.Position + this.Equipper.FacingDirection);
            }
        }

        public bool IsTriggered
        {
            get
            {
                return this.currentUsage != null && this.currentUsage.TriggerCondition.Evaluate(0, 0);
            }
        }

        public Mob Equipper
        {
            get
            {
                return this.equipper;
            }
        }

        public Action<bool, MobEquipment, bool, MobEquipment> OnEquipChange
        {
            get
            {
                return this.onEquipChange;
            }
            set
            {
                this.onEquipChange = value;
            }
        }

        public bool TryGetEquippedEquipment(out MobEquipment equippedEquipment)
        {
            if (this.equipIndex != -1)
            {
                equippedEquipment = this.equipments[this.equipIndex];
                return true;
            }
            equippedEquipment = null;
            return false;
        }

        public void Spawn(Mob equipper, MobStateNode equipperRootStateNode)
        {
            this.equipper = equipper;
            this.equipperRootStateNode = equipperRootStateNode;
            this.equipperRootStateNode.OnCurrentStateChanged += this.UpdateCurrentUsage;
            this.UpdateCurrentUsage();

            this.sortingGroupProperty.OnEnabledChanged +=
                (bool active) =>
                {
                    if (active)
                    {
                        if (this.equipIndex != -1) this.equipments[this.equipIndex].Equip();
                    }
                    else
                    {
                        if (this.equipIndex != -1) this.equipments[this.equipIndex].Unequip();
                    }
                };

            this.equipments = new List<MobEquipment>();
            this.equipIndex = -1;
            foreach (MobEquipmentSpawnable existingEquipmentSpawnable in this.existingEquipmentSpawnables)
            {
                this.AddEquipment(existingEquipmentSpawnable.Spawn());
            }

            FrigidCoroutine.Run(this.Refresh(), this.gameObject);
        }

        public bool AddEquipment(MobEquipment equipment)
        {
            if (!this.equipments.Contains(equipment))
            {
                this.equipments.Add(equipment);
                equipment.transform.SetParent(this.transform);
                equipment.transform.localPosition = Vector2.zero;
                equipment.Assign(this);
                this.UpdateEquippedEquipment(Mathf.Clamp(this.equipIndex, 0, this.equipments.Count - 1));
                return true;
            }
            return false;
        }

        public bool RemoveEquipment(MobEquipment equipment)
        {
            int equipmentIndex = this.equipments.IndexOf(equipment);
            if (equipmentIndex != -1)
            {
                if (this.equipments.Count == 1)
                {
                    this.UpdateEquippedEquipment(-1);
                }
                else if (equipmentIndex == this.equipIndex)
                {
                    this.UpdateEquippedEquipment((equipmentIndex + 1) % (this.equipments.Count - 1));
                }
                equipment.Unassign();
                equipment.transform.SetParent(null);
                equipment.transform.localPosition = Vector2.zero;
                this.equipments.RemoveAt(equipmentIndex);
                return true;
            }
            return false;
        }

        private void UpdateCurrentUsage(MobState previousState, MobState currentState)
        {
            this.UpdateCurrentUsage();
        }

        private void UpdateCurrentUsage()
        {
            this.currentUsage = null;
            foreach (Usage usage in this.usages)
            {
                if (usage.HasCurrentState(this.equipperRootStateNode.CurrentState))
                {
                    this.currentUsage = usage;
                    return;
                }
            }
        }

        private void UpdateEquippedEquipment(int equipIndex)
        {
            if (equipIndex != this.equipIndex)
            {
                int previousEquipIndex = this.equipIndex;
                MobEquipment previousEquippedEquipment = null;
                if (previousEquipIndex != -1 && this.sortingGroupProperty.Enabled)
                {
                    previousEquippedEquipment = this.equipments[previousEquipIndex];
                    previousEquippedEquipment.Unequip();
                }

                this.equipIndex = equipIndex;
                MobEquipment currentEquippedEquipment = null;
                if (this.equipIndex != -1 && this.sortingGroupProperty.Enabled)
                {
                    currentEquippedEquipment = this.equipments[this.equipIndex];
                    currentEquippedEquipment.Equip();
                }

                this.onEquipChange?.Invoke(previousEquipIndex != -1, previousEquippedEquipment, this.equipIndex != -1, currentEquippedEquipment);
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> Refresh()
        {
            while (true)
            {
                if (this.currentUsage != null && this.sortingGroupProperty.Enabled)
                {
                    for (int equipIndex = 0; equipIndex < Mathf.Min(this.equipments.Count, this.currentUsage.EquipConditionPerIndex.Length); equipIndex++)
                    {
                        if (equipIndex != this.equipIndex && this.currentUsage.EquipConditionPerIndex[equipIndex].Evaluate(0, 0))
                        {
                            this.UpdateEquippedEquipment(equipIndex);
                        }
                    }
                }
                yield return null;
            }
        }

        [Serializable]
        private class Usage
        {
            [SerializeField]
            private List<MobStateNode> stateNodes;
            [Space]
            [SerializeField]
            private Targeter aimTargeter;
            [SerializeField]
            private Conditional triggerCondition;
            [SerializeField]
            private Conditional[] equipConditionPerIndex;

            public bool HasCurrentState(MobState currentState)
            {
                return this.stateNodes.Any((MobStateNode stateNode) => stateNode.CurrentState == currentState);
            }

            public Targeter AimTargeter
            {
                get
                {
                    return this.aimTargeter;
                }
            }

            public Conditional TriggerCondition
            {
                get
                {
                    return this.triggerCondition;
                }
            }

            public Conditional[] EquipConditionPerIndex
            {
                get
                {
                    return this.equipConditionPerIndex;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipPoint : FrigidMonoBehaviour
    {
        [SerializeField]
        private SortingPointAnimatorProperty sortingPointProperty;
        [SerializeField]
        private Direction aimDirection;
        [SerializeField]
        private ConditionalClause triggerConditions;
        [SerializeField]
        private List<MobEquipmentPiece> defaultEquipmentPiecePrefabs;

        private List<MobEquipmentPiece> equipmentPieces;
        private int equipIndex;
        private Action<bool, MobEquipmentPiece, bool, MobEquipmentPiece> onEquipChange;

        private Mob equipper;

        private float equippedDuration;
        private float equippedDurationDelta;

        public Mob Equipper
        {
            get
            {
                return this.equipper;
            }
        }

        public Vector2 AimDirection
        {
            get
            {
                Vector2 aimDirection = this.aimDirection.Calculate(Vector2.zero, this.equippedDuration, this.equippedDurationDelta);
                if (aimDirection.magnitude <= 0)
                {
                    return this.Equipper.FacingDirection;
                }
                return aimDirection;
            }
        }

        public bool Triggered
        {
            get
            {
                return this.triggerConditions.Evaluate(this.equippedDuration, this.equippedDurationDelta);
            }
        }

        public Action<bool, MobEquipmentPiece, bool, MobEquipmentPiece> OnEquipChange
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

        public bool TryGetEquippedPiece(out MobEquipmentPiece equippedPiece)
        {
            if (this.equipIndex != -1)
            {
                equippedPiece = this.equipmentPieces[this.equipIndex];
                return true;
            }
            equippedPiece = null;
            return false;
        }

        public void Spawn(Mob equipper)
        {
            this.equipper = equipper;
            this.sortingPointProperty.OnShownChanged +=
                (bool shown) =>
                {
                    if (shown)
                    {
                        this.equippedDuration = 0;
                        this.equippedDurationDelta = 0;
                        if (this.equipIndex != -1) this.equipmentPieces[this.equipIndex].Equip();
                    }
                    else
                    {
                        if (this.equipIndex != -1) this.equipmentPieces[this.equipIndex].Unequip();
                    }
                };
            this.equipmentPieces = new List<MobEquipmentPiece>();
            this.equipIndex = -1;
            FrigidCoroutine.Run(Refresh(), this.gameObject);

            foreach (MobEquipmentPiece defaultEquipmentPiecePrefab in this.defaultEquipmentPiecePrefabs)
            {
                MobEquipmentPiece equipmentPiece = FrigidInstancing.CreateInstance<MobEquipmentPiece>(defaultEquipmentPiecePrefab);
                equipmentPiece.Spawn();
                AddEquipmentPiece(equipmentPiece);
            }
        }

        public void AddEquipmentPiece(MobEquipmentPiece equipmentPiece)
        {
            if (!this.equipmentPieces.Contains(equipmentPiece))
            {
                this.equipmentPieces.Add(equipmentPiece);
                equipmentPiece.transform.SetParent(this.transform);
                equipmentPiece.transform.localPosition = Vector2.zero;
                equipmentPiece.Assign(this);
                UpdateEquippedPiece(Mathf.Clamp(this.equipIndex, 0, this.equipmentPieces.Count - 1));
            }
        }

        public void RemoveEquipmentPiece(MobEquipmentPiece equipmentPiece)
        {
            int pieceIndex = this.equipmentPieces.IndexOf(equipmentPiece);
            if (pieceIndex != -1)
            {
                if (this.equipmentPieces.Count == 1)
                {
                    UpdateEquippedPiece(-1);
                }
                else if (pieceIndex == this.equipIndex)
                {
                    UpdateEquippedPiece((pieceIndex + 1) % this.equipmentPieces.Count);
                }
                equipmentPiece.Unassign();
                equipmentPiece.transform.SetParent(null);
                equipmentPiece.transform.localPosition = Vector2.zero;
                this.equipmentPieces.RemoveAt(pieceIndex);
            }
        }

        private void UpdateEquippedPiece(int equipIndex)
        {
            if (equipIndex != this.equipIndex)
            {
                int previousEquipIndex = this.equipIndex;
                MobEquipmentPiece previousEquippedPiece = null;
                if (previousEquipIndex != -1 && this.sortingPointProperty.Shown)
                {
                    previousEquippedPiece = this.equipmentPieces[previousEquipIndex];
                    previousEquippedPiece.Unequip();
                }

                this.equipIndex = equipIndex;
                MobEquipmentPiece currentEquippedPiece = null;
                if (this.equipIndex != -1 && this.sortingPointProperty.Shown)
                {
                    currentEquippedPiece = this.equipmentPieces[this.equipIndex];
                    currentEquippedPiece.Equip();
                }

                this.onEquipChange?.Invoke(previousEquipIndex == -1, previousEquippedPiece, this.equipIndex == -1, currentEquippedPiece);
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> Refresh()
        {
            while (true)
            {
                this.equippedDurationDelta = Time.deltaTime * this.Equipper.RequestedTimeScale;
                this.equippedDuration += this.equippedDurationDelta;
                yield return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TerrainObstruction : TerrainContent
    {
        [SerializeField]
        private Rigidbody2D rigidBody2D;

        private Action onBroken;

        public Action OnBroken
        {
            get
            {
                return this.onBroken;
            }
            set
            {
                this.onBroken = value;
            }
        }

        public override void Populate(Vector2 orientationDirection, NavigationGrid navigationGrid, List<Vector2Int> tileIndexPositions)
        {
            base.Populate(orientationDirection, navigationGrid, tileIndexPositions);

            foreach (Vector2Int tileIndexPosition in this.TileIndexPositions)
            {
                foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
                {
                    this.NavigationGrid[tileIndexPosition].AddObstruction(resistBoxProperty.DefensiveResistance);
                }
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.OnReceived += this.AttemptBreak;
                resistBoxProperty.IsIgnoringDamage = false;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.AnimatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.IsIgnoringDamage = false;
            }
            foreach (PushColliderAnimatorProperty pushColliderProperty in this.AnimatorBody.GetReferencedProperties<PushColliderAnimatorProperty>())
            {
                pushColliderProperty.Layer = (int)FrigidLayer.Obstructions;
            }
            this.rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
            this.rigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        protected virtual void Break()
        {
            foreach (Vector2Int tileIndexPosition in this.TileIndexPositions)
            {
                foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
                {
                    this.NavigationGrid[tileIndexPosition].RemoveObstruction(resistBoxProperty.DefensiveResistance);
                }
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.OnReceived -= this.AttemptBreak;
                resistBoxProperty.IsIgnoringDamage = true;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.AnimatorBody.GetReferencedProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.IsIgnoringDamage = true;
            }
            foreach (PushColliderAnimatorProperty pushColliderProperty in this.AnimatorBody.GetReferencedProperties<PushColliderAnimatorProperty>())
            {
                pushColliderProperty.Layer = (int)FrigidLayer.Default;
            }
            this.rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
            this.rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void AttemptBreak(BreakInfo breakInfo)
        {
            if (!breakInfo.Broken) return;
            this.Break();
            this.onBroken?.Invoke();
        }

#if UNITY_EDITOR
        private bool CanBreak()
        {
            if (this.AnimatorBody == null) return false;
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetReferencedProperties<ResistBoxAnimatorProperty>())
            {
                if (resistBoxProperty.DefensiveResistance < Resistance.Unbreakable)
                {
                    return true;
                }
            }
            return false;
        }
#endif
    }
}

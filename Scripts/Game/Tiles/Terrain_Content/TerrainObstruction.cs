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

        public override void Populated(Vector2 orientationDirection, NavigationGrid navigationGrid, List<Vector2Int> allTileIndices)
        {
            base.Populated(orientationDirection, navigationGrid, allTileIndices);

            foreach (Vector2Int tileIndices in this.AllTileIndices)
            {
                foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetProperties<ResistBoxAnimatorProperty>())
                {
                    this.NavigationGrid.AddObstruction(tileIndices, resistBoxProperty.DefensiveResistance);
                }
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.OnReceived += AttemptBreak;
                resistBoxProperty.IsIgnoringDamage = false;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.AnimatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.IsIgnoringDamage = false;
            }
            this.rigidBody2D.bodyType = RigidbodyType2D.Static;
        }

        protected virtual void Break()
        {
            foreach (Vector2Int tileIndices in this.AllTileIndices)
            {
                foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetProperties<ResistBoxAnimatorProperty>())
                {
                    this.NavigationGrid.RemoveObstruction(tileIndices, resistBoxProperty.DefensiveResistance);
                }
            }
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetProperties<ResistBoxAnimatorProperty>())
            {
                resistBoxProperty.OnReceived -= AttemptBreak;
                resistBoxProperty.IsIgnoringDamage = true;
            }
            foreach (BreakBoxAnimatorProperty breakBoxProperty in this.AnimatorBody.GetProperties<BreakBoxAnimatorProperty>())
            {
                breakBoxProperty.IsIgnoringDamage = true;
            }
            this.rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
        }

        private void AttemptBreak(BreakInfo breakInfo)
        {
            if (!breakInfo.Broken) return;
            Break();
            this.onBroken?.Invoke();
        }

#if UNITY_EDITOR
        private bool CanBreak()
        {
            if (this.AnimatorBody == null) return false;
            foreach (ResistBoxAnimatorProperty resistBoxProperty in this.AnimatorBody.GetProperties<ResistBoxAnimatorProperty>())
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

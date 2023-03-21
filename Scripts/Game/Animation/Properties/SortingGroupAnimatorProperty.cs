using UnityEngine;
using UnityEngine.Rendering;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class SortingGroupAnimatorProperty : SortingOrderedAnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private SortingGroup sortingGroup;

        public override int CurrentSortingOrder
        {
            get
            {
                return this.sortingGroup.sortingOrder;
            }
        }

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sortingGroup = FrigidEditMode.AddComponent<SortingGroup>(this.gameObject);
            this.sortingGroup.sortingLayerName = FrigidSortingLayer.World.ToString();
            base.Created();
        }

        public override void OrientationEnter()
        {
            try
            {
                this.sortingGroup.sortingOrder = GetSortingOrder(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            }
            catch
            {
                Debug.Log(this.Body);
            }
            base.OrientationEnter();
        }
    }
}

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

        public override int SortingOrder
        {
            get
            {
                return this.sortingGroup.sortingOrder;
            }
            protected set
            {
                this.sortingGroup.sortingOrder = value;
            }
        }


        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.sortingGroup = FrigidEdit.AddComponent<SortingGroup>(this.gameObject);
            this.sortingGroup.sortingLayerName = FrigidSortingLayer.World.ToString();
            base.Created();
        }
    }
}

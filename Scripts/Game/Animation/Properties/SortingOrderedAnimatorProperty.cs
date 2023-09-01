using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class SortingOrderedAnimatorProperty : AnimatorProperty
    {
        [SerializeField]
        [HideInInspector]
        private Nested3DList<int> sortingOrders;

        public abstract int SortingOrder
        {
            get;
            protected set;
        }

        public int GetSortingOrder(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.sortingOrders[animationIndex][frameIndex][orientationIndex];
        }

        public void SetSortingOrder(int animationIndex, int frameIndex, int orientationIndex, int sortingOrder)
        {
            if (this.sortingOrders[animationIndex][frameIndex][orientationIndex] != sortingOrder)
            {
                FrigidEdit.RecordChanges(this);
                this.sortingOrders[animationIndex][frameIndex][orientationIndex] = sortingOrder;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.sortingOrders = new Nested3DList<int>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.sortingOrders.Add(new Nested2DList<int>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.sortingOrders[animationIndex].Add(new Nested1DList<int>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.sortingOrders[animationIndex][frameIndex].Add(0);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sortingOrders.Insert(animationIndex, new Nested2DList<int>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.sortingOrders[animationIndex].Add(new Nested1DList<int>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.sortingOrders[animationIndex][frameIndex].Add(0);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sortingOrders.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sortingOrders[animationIndex].Insert(frameIndex, new Nested1DList<int>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.sortingOrders[animationIndex][frameIndex].Add(0);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sortingOrders[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sortingOrders[animationIndex][frameIndex].Insert(orientationIndex, 0);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.sortingOrders[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            SortingOrderedAnimatorProperty otherSortingOrderedProperty = otherProperty as SortingOrderedAnimatorProperty;
            if (otherSortingOrderedProperty)
            {
                otherSortingOrderedProperty.SetSortingOrder(toAnimationIndex, toFrameIndex, toOrientationIndex, this.GetSortingOrder(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void OrientationEnter()
        {
            this.SortingOrder = this.GetSortingOrder(this.Body.CurrAnimationIndex, this.Body.CurrFrameIndex, this.Body.CurrOrientationIndex);
            base.OrientationEnter();
        }
    }
}

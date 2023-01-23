using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class SortingPointAnimatorProperty : SortingOrderedAnimatorProperty
    {
        [SerializeField]
        [ReadOnly]
        private SortingGroup sortingGroup;
        [SerializeField]
        [HideInInspector]
        private Nested3DList<Vector2> localOffsets;

        public override List<AnimatorProperty> ChildProperties
        {
            get
            {
                return new List<AnimatorProperty>();
            }
        }

        public Vector2 GetLocalOffset(int animationIndex, int frameIndex, int orientationIndex)
        {
            return this.localOffsets[animationIndex][frameIndex][orientationIndex];
        }

        public void SetLocalOffset(int animationIndex, int frameIndex, int orientationIndex, Vector2 localOffset)
        {
            if (this.localOffsets[animationIndex][frameIndex][orientationIndex] != localOffset)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.localOffsets[animationIndex][frameIndex][orientationIndex] = localOffset;
            }
        }

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sortingGroup = FrigidEditMode.AddComponent<SortingGroup>(this.gameObject);
            this.sortingGroup.sortingLayerName = FrigidSortingLayer.World.ToString();
            this.localOffsets = new Nested3DList<Vector2>();
            for (int animationIndex = 0; animationIndex < this.Body.GetAnimationCount(); animationIndex++)
            {
                this.localOffsets.Add(new Nested2DList<Vector2>());
                for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
                {
                    this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                    for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                    {
                        this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                    }
                }
            }
            base.Created();
        }

        public override void AnimationAddedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets.Insert(animationIndex, new Nested2DList<Vector2>());
            for (int frameIndex = 0; frameIndex < this.Body.GetFrameCount(animationIndex); frameIndex++)
            {
                this.localOffsets[animationIndex].Add(new Nested1DList<Vector2>());
                for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
                {
                    this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
                }
            }
            base.AnimationAddedAt(animationIndex);
        }

        public override void AnimationRemovedAt(int animationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets.RemoveAt(animationIndex);
            base.AnimationRemovedAt(animationIndex);
        }

        public override void FrameAddedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex].Insert(frameIndex, new Nested1DList<Vector2>());
            for (int orientationIndex = 0; orientationIndex < this.Body.GetOrientationCount(animationIndex); orientationIndex++)
            {
                this.localOffsets[animationIndex][frameIndex].Add(Vector2.zero);
            }
            base.FrameAddedAt(animationIndex, frameIndex);
        }

        public override void FrameRemovedAt(int animationIndex, int frameIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex].RemoveAt(frameIndex);
            base.FrameRemovedAt(animationIndex, frameIndex);
        }

        public override void OrientationAddedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex][frameIndex].Insert(orientationIndex, Vector2.zero);
            base.OrientationAddedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void OrientationRemovedAt(int animationIndex, int frameIndex, int orientationIndex)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.localOffsets[animationIndex][frameIndex].RemoveAt(orientationIndex);
            base.OrientationRemovedAt(animationIndex, frameIndex, orientationIndex);
        }

        public override void CopyPasteToAnotherOrientation(AnimatorProperty otherProperty, int fromAnimationIndex, int toAnimationIndex, int fromFrameIndex, int toFrameIndex, int fromOrientationIndex, int toOrientationIndex)
        {
            SortingPointAnimatorProperty otherSortingPointProperty = otherProperty as SortingPointAnimatorProperty;
            if (otherSortingPointProperty)
            {
                otherSortingPointProperty.SetLocalOffset(toAnimationIndex, toFrameIndex, toOrientationIndex, GetLocalOffset(fromAnimationIndex, fromFrameIndex, fromOrientationIndex));
            }
            base.CopyPasteToAnotherOrientation(otherProperty, fromAnimationIndex, toAnimationIndex, fromFrameIndex, toFrameIndex, fromOrientationIndex, toOrientationIndex);
        }

        public override void OrientFrameEnter(int animationIndex, int frameIndex, int orientationIndex, float elapsedDuration)
        {
            this.sortingGroup.sortingOrder = GetSortingOrder(animationIndex, frameIndex, orientationIndex);
            this.transform.localPosition = GetLocalOffset(animationIndex, frameIndex, orientationIndex);
            base.OrientFrameEnter(animationIndex, frameIndex, orientationIndex, elapsedDuration);
        }
    }
}

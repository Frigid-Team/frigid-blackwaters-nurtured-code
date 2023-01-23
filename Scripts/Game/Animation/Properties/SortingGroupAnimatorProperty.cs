using System.Collections.Generic;
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
        [SerializeField]
        [ReadOnly]
        private List<SpriteAnimatorProperty> spriteProperties;
        [SerializeField]
        [ReadOnly]
        private List<ParticleAnimatorProperty> particleProperties;
        [SerializeField]
        [ReadOnly]
        private List<SortingGroupAnimatorProperty> sortingGroupProperties;
        [SerializeField]
        [ReadOnly]
        private List<SortingPointAnimatorProperty> sortingPointProperties;

        public override List<AnimatorProperty> ChildProperties
        {
            get
            {
                List<AnimatorProperty> childProperties = new List<AnimatorProperty>();
                childProperties.AddRange(this.spriteProperties);
                childProperties.AddRange(this.particleProperties);
                childProperties.AddRange(this.sortingGroupProperties);
                childProperties.AddRange(this.sortingPointProperties);
                return childProperties;
            }
        }

        public int GetNumberSpriteProperties()
        {
            return this.spriteProperties.Count;
        }

        public SpriteAnimatorProperty GetSpritePropertyAt(int index)
        {
            return this.spriteProperties[index];
        }

        public void AddSpritePropertyAt(int index)
        {
            SpriteAnimatorProperty newSpriteProperty = CreateSubProperty<SpriteAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.spriteProperties.Insert(index, newSpriteProperty);
            newSpriteProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newSpriteProperty));
        }

        public void RemoveSpritePropertyAt(int index)
        {
            DestroySubProperty(this.spriteProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.spriteProperties.RemoveAt(index);
        }

        public int GetNumberParticleProperties()
        {
            return this.particleProperties.Count;
        }

        public ParticleAnimatorProperty GetParticlePropertyAt(int index)
        {
            return this.particleProperties[index];
        }

        public void AddParticlePropertyAt(int index)
        {
            ParticleAnimatorProperty newParticleProperty = CreateSubProperty<ParticleAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.particleProperties.Insert(index, newParticleProperty);
            newParticleProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newParticleProperty));
        }

        public void RemoveParticlePropertyAt(int index)
        {
            DestroySubProperty(this.particleProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.particleProperties.RemoveAt(index);
        }

        public int GetNumberSortingGroupProperties()
        {
            return this.sortingGroupProperties.Count;
        }

        public SortingGroupAnimatorProperty GetSortingGroupPropertyAt(int index)
        {
            return this.sortingGroupProperties[index];
        }

        public void AddSortingGroupPropertyAt(int index)
        {
            SortingGroupAnimatorProperty newSortingGroupProperty = CreateSubProperty<SortingGroupAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.sortingGroupProperties.Insert(index, newSortingGroupProperty);
            newSortingGroupProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newSortingGroupProperty));
        }

        public void RemoveSortingGroupPropertyAt(int index)
        {
            DestroySubProperty(this.sortingGroupProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.sortingGroupProperties.RemoveAt(index);
        }

        public int GetNumberSortingPointProperties()
        {
            return this.sortingPointProperties.Count;
        }

        public SortingPointAnimatorProperty GetSortingPointPropertyAt(int index)
        {
            return this.sortingPointProperties[index];
        }

        public void AddSortingPointPropertyAt(int index)
        {
            SortingPointAnimatorProperty newSortingPointProperty = CreateSubProperty<SortingPointAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.sortingPointProperties.Insert(index, newSortingPointProperty);
            newSortingPointProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newSortingPointProperty));
        }

        public void RemoveSortingPointPropertyAt(int index)
        {
            DestroySubProperty(this.sortingPointProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.sortingPointProperties.RemoveAt(index);
        }

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.sortingGroup = FrigidEditMode.AddComponent<SortingGroup>(this.gameObject);
            this.sortingGroup.sortingLayerName = FrigidSortingLayer.World.ToString();
            this.spriteProperties = new List<SpriteAnimatorProperty>();
            this.particleProperties = new List<ParticleAnimatorProperty>();
            this.sortingGroupProperties = new List<SortingGroupAnimatorProperty>();
            this.sortingPointProperties = new List<SortingPointAnimatorProperty>();
            base.Created();
        }

        public override void OrientFrameEnter(int animationIndex, int frameIndex, int orientationIndex, float elapsedDuration)
        {
            this.sortingGroup.sortingOrder = GetSortingOrder(animationIndex, frameIndex, orientationIndex);
            base.OrientFrameEnter(animationIndex, frameIndex, orientationIndex, elapsedDuration);
        }
    }
}

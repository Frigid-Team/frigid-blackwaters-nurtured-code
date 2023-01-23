using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class RootAnimatorProperty : AnimatorProperty
    {
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
        [SerializeField]
        [ReadOnly]
        private List<HitBoxAnimatorProperty> hitBoxProperties;
        [SerializeField]
        [ReadOnly]
        private List<HurtBoxAnimatorProperty> hurtBoxProperties;
        [SerializeField]
        [ReadOnly]
        private List<BreakBoxAnimatorProperty> breakBoxProperties;
        [SerializeField]
        [ReadOnly]
        private List<ResistBoxAnimatorProperty> resistBoxProperties;
        [SerializeField]
        [ReadOnly]
        private List<ThreatBoxAnimatorProperty> threatBoxProperties;
        [SerializeField]
        [ReadOnly]
        private List<LookoutBoxAnimatorProperty> lookoutBoxProperties;
        [SerializeField]
        [ReadOnly]
        private List<PushColliderAnimatorProperty> pushColliderProperties;
        [SerializeField]
        [ReadOnly]
        private List<AttackAnimatorProperty> attackProperties;
        [SerializeField]
        [ReadOnly]
        private List<AudioAnimatorProperty> audioProperties;
        [SerializeField]
        [ReadOnly]
        private List<LightAnimatorProperty> lightProperties;
        
        public override List<AnimatorProperty> ChildProperties
        {
            get
            {
                List<AnimatorProperty> childProperties = new List<AnimatorProperty>();
                childProperties.AddRange(this.spriteProperties);
                childProperties.AddRange(this.particleProperties);
                childProperties.AddRange(this.sortingGroupProperties);
                childProperties.AddRange(this.sortingPointProperties);
                childProperties.AddRange(this.hitBoxProperties);
                childProperties.AddRange(this.hurtBoxProperties);
                childProperties.AddRange(this.breakBoxProperties);
                childProperties.AddRange(this.resistBoxProperties);
                childProperties.AddRange(this.threatBoxProperties);
                childProperties.AddRange(this.lookoutBoxProperties);
                childProperties.AddRange(this.pushColliderProperties);
                childProperties.AddRange(this.attackProperties);
                childProperties.AddRange(this.audioProperties);
                childProperties.AddRange(this.lightProperties);
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

        public int GetNumberHitBoxProperties()
        {
            return this.hitBoxProperties.Count;
        }

        public HitBoxAnimatorProperty GetHitBoxPropertyAt(int index)
        {
            return this.hitBoxProperties[index];
        }

        public void AddHitBoxPropertyAt(int index)
        {
            HitBoxAnimatorProperty newHitBoxProperty = CreateSubProperty<HitBoxAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.hitBoxProperties.Insert(index, newHitBoxProperty);
            newHitBoxProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newHitBoxProperty));
        }

        public void RemoveHitBoxPropertyAt(int index)
        {
            DestroySubProperty(this.hitBoxProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.hitBoxProperties.RemoveAt(index);
        }

        public int GetNumberHurtBoxProperties()
        {
            return this.hurtBoxProperties.Count;
        }

        public HurtBoxAnimatorProperty GetHurtBoxPropertyAt(int index)
        {
            return this.hurtBoxProperties[index];
        }

        public void AddHurtBoxPropertyAt(int index)
        {
            HurtBoxAnimatorProperty newHurtBoxProperty = CreateSubProperty<HurtBoxAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.hurtBoxProperties.Insert(index, newHurtBoxProperty);
            newHurtBoxProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newHurtBoxProperty));
        }

        public void RemoveHurtBoxPropertyAt(int index)
        {
            DestroySubProperty(this.hurtBoxProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.hurtBoxProperties.RemoveAt(index);
        }

        public int GetNumberBreakBoxProperties()
        {
            return this.breakBoxProperties.Count;
        }

        public BreakBoxAnimatorProperty GetBreakBoxPropertyAt(int index)
        {
            return this.breakBoxProperties[index];
        }

        public void AddBreakBoxPropertyAt(int index)
        {
            BreakBoxAnimatorProperty newBreakBoxProperty = CreateSubProperty<BreakBoxAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.breakBoxProperties.Insert(index, newBreakBoxProperty);
            newBreakBoxProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newBreakBoxProperty));
        }

        public void RemoveBreakBoxPropertyAt(int index)
        {
            DestroySubProperty(this.breakBoxProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.breakBoxProperties.RemoveAt(index);
        }

        public int GetNumberResistBoxProperties()
        {
            return this.resistBoxProperties.Count;
        }

        public ResistBoxAnimatorProperty GetResistBoxPropertyAt(int index)
        {
            return this.resistBoxProperties[index];
        }

        public void AddResistBoxPropertyAt(int index)
        {
            ResistBoxAnimatorProperty newResistBoxProperty = CreateSubProperty<ResistBoxAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.resistBoxProperties.Insert(index, newResistBoxProperty);
            newResistBoxProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newResistBoxProperty));
        }

        public void RemoveResistBoxPropertyAt(int index)
        {
            DestroySubProperty(this.resistBoxProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.resistBoxProperties.RemoveAt(index);
        }

        public int GetNumberThreatBoxProperties()
        {
            return this.threatBoxProperties.Count;
        }

        public ThreatBoxAnimatorProperty GetThreatBoxPropertyAt(int index)
        {
            return this.threatBoxProperties[index];
        }

        public void AddThreatBoxPropertyAt(int index)
        {
            ThreatBoxAnimatorProperty newThreatBoxProperty = CreateSubProperty<ThreatBoxAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.threatBoxProperties.Insert(index, newThreatBoxProperty);
            newThreatBoxProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newThreatBoxProperty));
        }

        public void RemoveThreatBoxPropertyAt(int index)
        {
            DestroySubProperty(this.threatBoxProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.threatBoxProperties.RemoveAt(index);
        }

        public int GetNumberLookoutBoxProperties()
        {
            return this.lookoutBoxProperties.Count;
        }

        public LookoutBoxAnimatorProperty GetLookoutBoxPropertyAt(int index)
        {
            return this.lookoutBoxProperties[index];
        }

        public void AddLookoutBoxPropertyAt(int index)
        {
            LookoutBoxAnimatorProperty newLookoutBoxProperty = CreateSubProperty<LookoutBoxAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.lookoutBoxProperties.Insert(index, newLookoutBoxProperty);
            newLookoutBoxProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newLookoutBoxProperty));
        }

        public void RemoveLookoutBoxPropertyAt(int index)
        {
            DestroySubProperty(this.lookoutBoxProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.lookoutBoxProperties.RemoveAt(index);
        }

        public int GetNumberPushColliderProperties()
        {
            return this.pushColliderProperties.Count;
        }

        public PushColliderAnimatorProperty GetPushColliderPropertyAt(int index)
        {
            return this.pushColliderProperties[index];
        }

        public void AddPushColliderPropertyAt(int index)
        {
            PushColliderAnimatorProperty newPushColliderProperty = CreateSubProperty<PushColliderAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.pushColliderProperties.Insert(index, newPushColliderProperty);
            newPushColliderProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newPushColliderProperty));
        }

        public void RemovePushColliderPropertyAt(int index)
        {
            DestroySubProperty(this.pushColliderProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.pushColliderProperties.RemoveAt(index);
        }

        public int GetNumberAttackProperties()
        {
            return this.attackProperties.Count;
        }

        public AttackAnimatorProperty GetAttackPropertyAt(int index)
        {
            return this.attackProperties[index];
        }

        public void AddAttackPropertyAt(int index)
        {
            AttackAnimatorProperty newAttackProperty = CreateSubProperty<AttackAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.attackProperties.Insert(index, newAttackProperty);
            newAttackProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newAttackProperty));
        }

        public void RemoveAttackPropertyAt(int index)
        {
            DestroySubProperty(this.attackProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.attackProperties.RemoveAt(index);
        }

        public int GetNumberAudioProperties()
        {
            return this.audioProperties.Count;
        }

        public AudioAnimatorProperty GetAudioPropertyAt(int index)
        {
            return this.audioProperties[index];
        }

        public void AddAudioPropertyAt(int index)
        {
            AudioAnimatorProperty newAudioProperty = CreateSubProperty<AudioAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.audioProperties.Insert(index, newAudioProperty);
            newAudioProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newAudioProperty));
        }

        public void RemoveAudioPropertyAt(int index)
        {
            DestroySubProperty(this.audioProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.audioProperties.RemoveAt(index);
        }

        public int GetNumberLightProperties()
        {
            return this.lightProperties.Count;
        }

        public LightAnimatorProperty GetLightPropertyAt(int index)
        {
            return this.lightProperties[index];
        }

        public void AddLightPropertyAt(int index)
        {
            LightAnimatorProperty newLightProperty = CreateSubProperty<LightAnimatorProperty>();
            FrigidEditMode.RecordPotentialChanges(this);
            this.lightProperties.Insert(index, newLightProperty);
            newLightProperty.transform.SetSiblingIndex(this.ChildProperties.IndexOf(newLightProperty));
        }

        public void RemoveLightPropertyAt(int index)
        {
            DestroySubProperty(this.lightProperties[index]);
            FrigidEditMode.RecordPotentialChanges(this);
            this.lightProperties.RemoveAt(index);
        }

        public override void Created()
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.spriteProperties = new List<SpriteAnimatorProperty>();
            this.particleProperties = new List<ParticleAnimatorProperty>();
            this.sortingGroupProperties = new List<SortingGroupAnimatorProperty>();
            this.sortingPointProperties = new List<SortingPointAnimatorProperty>();
            this.hitBoxProperties = new List<HitBoxAnimatorProperty>();
            this.hurtBoxProperties = new List<HurtBoxAnimatorProperty>();
            this.breakBoxProperties = new List<BreakBoxAnimatorProperty>();
            this.resistBoxProperties = new List<ResistBoxAnimatorProperty>();
            this.threatBoxProperties = new List<ThreatBoxAnimatorProperty>();
            this.lookoutBoxProperties = new List<LookoutBoxAnimatorProperty>();
            this.pushColliderProperties = new List<PushColliderAnimatorProperty>();
            this.attackProperties = new List<AttackAnimatorProperty>();
            this.audioProperties = new List<AudioAnimatorProperty>();
            this.lightProperties = new List<LightAnimatorProperty>();
            base.Created();
        }
    }
}

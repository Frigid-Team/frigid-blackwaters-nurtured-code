using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageReceiverBoxAnimatorProperty<RB, DB, I> : ColliderAnimatorProperty where RB : DamageReceiverBox<RB, DB, I> where DB : DamageDealerBox<DB, RB, I> where I : DamageInfo
    {
        [SerializeField]
        [ReadOnly]
        private RB damageReceiverBox;
        [SerializeField]
        [HideInInspector]
        private bool playAudioOnReceived;
        [SerializeField]
        [HideInInspector]
        private AudioClipSerializedReference audioClipOnReceived;
        [SerializeField]
        [HideInInspector]
        private List<MaterialTweenCoroutineTemplateSerializedReference> materialTweensOnReceived;

        public DamageAlignment DamageAlignment
        {
            get
            {
                return this.damageReceiverBox.DamageAlignment;
            }
            set
            {
                this.damageReceiverBox.DamageAlignment = value;
            }
        }

        public DamageChannel DamageChannel
        {
            get
            {
                return this.damageReceiverBox.DamageChannel;
            }
            set
            {
                this.damageReceiverBox.DamageChannel = value;
            }
        }

        public Action<I> OnReceived
        {
            get
            {
                return this.damageReceiverBox.OnReceived;
            }
            set
            {
                this.damageReceiverBox.OnReceived = value;
            }
        }

        public bool IsIgnoringDamage
        {
            get
            {
                return this.damageReceiverBox.IsIgnoringDamage;
            }
            set
            {
                this.damageReceiverBox.IsIgnoringDamage = value;
            }
        }

        public bool PlayAudioOnReceived
        {
            get
            {
                return this.playAudioOnReceived;
            }
            set
            {
                if (this.playAudioOnReceived != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.playAudioOnReceived = value;
                }
            }
        }

        public AudioClipSerializedReference AudioClipOnReceivedByReference
        {
            get
            {
                return this.audioClipOnReceived;
            }
            set
            {
                if (this.audioClipOnReceived != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.audioClipOnReceived = value;
                }
            }
        }

        public int GetNumberMaterialTweensOnReceived()
        {
            return this.materialTweensOnReceived.Count;
        }

        public void AddMaterialTweenOnReceivedAt(int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensOnReceived.Insert(index, new MaterialTweenCoroutineTemplateSerializedReference());
        }

        public void RemoveMaterialTweenOnReceivedAt(int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensOnReceived.RemoveAt(index);
        }

        public MaterialTweenCoroutineTemplateSerializedReference GetMaterialTweenOnReceivedByReferenceAt(int index)
        {
            return this.materialTweensOnReceived[index];
        }

        public void SetMaterialTweenOnReceivedByReferenceAt(int index, MaterialTweenCoroutineTemplateSerializedReference materialEffectOnReceived)
        {
            if (this.materialTweensOnReceived[index] != materialEffectOnReceived)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.materialTweensOnReceived[index] = materialEffectOnReceived;
            }
        } 

        public override void Created()
        {
            this.damageReceiverBox = FrigidEditMode.AddComponent<RB>(this.gameObject);
            this.playAudioOnReceived = false;
            this.audioClipOnReceived = new AudioClipSerializedReference();
            this.materialTweensOnReceived = new List<MaterialTweenCoroutineTemplateSerializedReference>();
            base.Created();
        }

        public override void Initialize()
        {
            base.Initialize();
            this.damageReceiverBox.OnReceived +=
                (I info) =>
                {
                    if (!info.IsNonTrivial) return;

                    foreach (SpriteAnimatorProperty spriteProperty in this.Body.GetCurrentProperties<SpriteAnimatorProperty>())
                    {
                        for (int effectIndex = 0; effectIndex < GetNumberMaterialTweensOnReceived(); effectIndex++)
                        {
                            spriteProperty.OneShotMaterialTween(GetMaterialTweenOnReceivedByReferenceAt(effectIndex).ImmutableValue);
                        }
                    }

                    if (!this.PlayAudioOnReceived) return;
                    foreach (AudioAnimatorProperty audioProperty in this.Body.GetCurrentProperties<AudioAnimatorProperty>())
                    {
                        if (audioProperty.Loop) continue;
                        audioProperty.PlayOneShot(this.audioClipOnReceived.MutableValue);
                    }
                };
        }

        protected RB DamageReceiverBox
        {
            get
            {
                return this.damageReceiverBox;
            }
        }

        protected override void ColliderTypeUpdated()
        {
            this.Collider.isTrigger = true;
            base.ColliderTypeUpdated();
        }
    }
}

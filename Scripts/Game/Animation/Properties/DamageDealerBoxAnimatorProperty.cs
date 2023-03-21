using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageDealerBoxAnimatorProperty<DB, RB, I> : ColliderAnimatorProperty where DB : DamageDealerBox<DB, RB, I> where RB : DamageReceiverBox<RB, DB, I> where I : DamageInfo
    {
        [SerializeField]
        [ReadOnly]
        private DB damageDealerBox;
        [SerializeField]
        [HideInInspector]
        private bool playAudioOnDealt;
        [SerializeField]
        [HideInInspector]
        private AudioClipSerializedReference audioClipOnDealt;
        [SerializeField]
        [HideInInspector]
        private List<MaterialTweenCoroutineTemplateSerializedReference> materialTweensOnDealt;

        public DamageAlignment DamageAlignment
        {
            get
            {
                return this.damageDealerBox.DamageAlignment;
            }
            set
            {
                this.damageDealerBox.DamageAlignment = value;
            }
        }

        public DamageChannel DamageChannel
        {
            get
            {
                return this.damageDealerBox.DamageChannel;
            }
            set
            {
                this.damageDealerBox.DamageChannel = value;
            }
        }

        public Action<I> OnDealt
        {
            get
            {
                return this.damageDealerBox.OnDealt;
            }
            set
            {
                this.damageDealerBox.OnDealt = value;
            }
        }

        public bool IsIgnoringDamage
        {
            get
            {
                return this.damageDealerBox.IsIgnoringDamage;
            }
            set
            {
                this.damageDealerBox.IsIgnoringDamage = value;
            }
        }

        public bool PlayAudioOnDealt
        {
            get
            {
                return this.playAudioOnDealt;
            }
            set
            {
                if (this.playAudioOnDealt != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.playAudioOnDealt = value;
                }
            }
        }

        public AudioClipSerializedReference AudioClipOnDealtByReference
        {
            get
            {
                return this.audioClipOnDealt;
            }
            set
            {
                if (this.audioClipOnDealt != value)
                {
                    FrigidEditMode.RecordPotentialChanges(this);
                    this.audioClipOnDealt = value;
                }
            }
        }

        public int GetNumberMaterialTweensOnDealt()
        {
            return this.materialTweensOnDealt.Count;
        }

        public void AddMaterialTweenOnDealtAt(int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensOnDealt.Insert(index, new MaterialTweenCoroutineTemplateSerializedReference());
        }

        public void RemoveMaterialTweenOnDealtAt(int index)
        {
            FrigidEditMode.RecordPotentialChanges(this);
            this.materialTweensOnDealt.RemoveAt(index);
        }

        public MaterialTweenCoroutineTemplateSerializedReference GetMaterialTweenOnDealtByReferenceAt(int index)
        {
            return this.materialTweensOnDealt[index];
        }

        public void SetMaterialTweenOnDealtByReferenceAt(int index, MaterialTweenCoroutineTemplateSerializedReference materialEffectOnDealt)
        {
            if (this.materialTweensOnDealt[index] != materialEffectOnDealt)
            {
                FrigidEditMode.RecordPotentialChanges(this);
                this.materialTweensOnDealt[index] = materialEffectOnDealt;
            }
        }

        public override void Created()
        {
            this.damageDealerBox = FrigidEditMode.AddComponent<DB>(this.gameObject);
            this.playAudioOnDealt = false;
            this.audioClipOnDealt = new AudioClipSerializedReference();
            this.materialTweensOnDealt = new List<MaterialTweenCoroutineTemplateSerializedReference>();
            base.Created();
        }

        public override void Initialize()
        {
            base.Initialize();
            this.damageDealerBox.OnDealt +=
                (I info) =>
                {
                    if (!info.IsNonTrivial) return;

                    foreach (SpriteAnimatorProperty spriteProperty in this.Body.GetCurrentProperties<SpriteAnimatorProperty>())
                    {
                        for (int effectIndex = 0; effectIndex < GetNumberMaterialTweensOnDealt(); effectIndex++)
                        {
                            spriteProperty.OneShotMaterialTween(GetMaterialTweenOnDealtByReferenceAt(effectIndex).ImmutableValue);
                        }
                    }

                    if (!this.PlayAudioOnDealt) return;
                    foreach (AudioAnimatorProperty audioProperty in this.Body.GetCurrentProperties<AudioAnimatorProperty>())
                    {
                        if (audioProperty.Loop) continue;
                        audioProperty.PlayOneShot(this.audioClipOnDealt.MutableValue);
                    }
                };
        }

        protected DB DamageDealerBox
        {
            get
            {
                return this.damageDealerBox;
            }
        }

        protected override void ColliderTypeUpdated()
        {
            this.Collider.isTrigger = true;
            base.ColliderTypeUpdated();
        }
    }
}

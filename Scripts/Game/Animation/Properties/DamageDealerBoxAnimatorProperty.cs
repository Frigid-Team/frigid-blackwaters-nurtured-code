using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageDealerBoxAnimatorProperty<DB, RB, I> : ColliderAnimatorProperty where DB : DamageDealerBox<DB, RB, I> where RB : DamageReceiverBox<RB, DB, I> 
    {
        [SerializeField]
        [ReadOnly]
        private DB damageDealerBox;
        [SerializeField]
        [HideInInspector]
        private bool playAudioOnDealt;
        [SerializeField]
        [ReadOnly]
        private AudioSource audioSource;
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
                    if (this.playAudioOnDealt)
                    {
                        this.audioSource = FrigidEditMode.AddComponent<AudioSource>(this.gameObject);
                        this.audioSource.playOnAwake = false;
                    }
                    else
                    {
                        FrigidEditMode.RemoveComponent(this.audioSource);
                    }
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
                this.audioClipOnDealt = value;
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
                    if (this.PlayAudioOnDealt)
                    {
                        this.audioSource.PlayOneShot(this.AudioClipOnDealtByReference.MutableValue);
                    }

                    foreach (SpriteAnimatorProperty spriteProperty in this.Body.GetProperties<SpriteAnimatorProperty>())
                    {
                        for (int effectIndex = 0; effectIndex < GetNumberMaterialTweensOnDealt(); effectIndex++)
                        {
                            spriteProperty.OneShotMaterialTween(GetMaterialTweenOnDealtByReferenceAt(effectIndex).ImmutableValue);
                        }
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

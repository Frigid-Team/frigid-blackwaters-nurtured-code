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
        private List<MaterialTweenOptionSetSerializedReference> materialTweensOnDealt;

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
                    FrigidEdit.RecordChanges(this);
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
                    FrigidEdit.RecordChanges(this);
                    this.audioClipOnDealt = value;
                }
            }
        }

        public int GetNumberMaterialTweensOnDealt()
        {
            return this.materialTweensOnDealt.Count;
        }

        public void AddMaterialTweenOnDealtAt(int tweenIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweensOnDealt.Insert(tweenIndex, new MaterialTweenOptionSetSerializedReference());
        }

        public void RemoveMaterialTweenOnDealtAt(int tweenIndex)
        {
            FrigidEdit.RecordChanges(this);
            this.materialTweensOnDealt.RemoveAt(tweenIndex);
        }

        public MaterialTweenOptionSetSerializedReference GetMaterialTweenOnDealtByReferenceAt(int tweenIndex)
        {
            return this.materialTweensOnDealt[tweenIndex];
        }

        public void SetMaterialTweenOnDealtByReferenceAt(int tweenIndex, MaterialTweenOptionSetSerializedReference materialEffectOnDealt)
        {
            if (this.materialTweensOnDealt[tweenIndex] != materialEffectOnDealt)
            {
                FrigidEdit.RecordChanges(this);
                this.materialTweensOnDealt[tweenIndex] = materialEffectOnDealt;
            }
        }

        public override void Created()
        {
            FrigidEdit.RecordChanges(this);
            this.damageDealerBox = FrigidEdit.AddComponent<DB>(this.gameObject);
            this.playAudioOnDealt = false;
            this.audioClipOnDealt = new AudioClipSerializedReference();
            this.materialTweensOnDealt = new List<MaterialTweenOptionSetSerializedReference>();
            base.Created();
        }

        public override void Initialize()
        {
            base.Initialize();
            this.damageDealerBox.OnDealt +=
                (I info) =>
                {
                    if (!info.IsNonTrivial) return;

                    foreach (SpriteAnimatorProperty spriteProperty in this.Body.GetReferencedProperties<SpriteAnimatorProperty>())
                    {
                        for (int effectIndex = 0; effectIndex < this.GetNumberMaterialTweensOnDealt(); effectIndex++)
                        {
                            spriteProperty.OneShotMaterialTween(this.GetMaterialTweenOnDealtByReferenceAt(effectIndex).ImmutableValue);
                        }
                    }

                    if (!this.PlayAudioOnDealt) return;
                    foreach (AudioAnimatorProperty audioProperty in this.Body.GetReferencedProperties<AudioAnimatorProperty>())
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

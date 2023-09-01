using UnityEngine;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class MobModifyHitsBehaviour : MobBehaviour
    {
        [SerializeField]
        private HitModifier hitModifier;
        [SerializeField]
        private bool playAudioOnModified;
        [SerializeField]
        [ShowIfBool("playAudioOnModified", true)]
        private AudioClipSerializedReference audioClipOnModified;

        public override void Enter()
        {
            base.Enter();
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.OwnerAnimatorBody.GetReferencedProperties<HurtBoxAnimatorProperty>())
            {
                hurtBoxProperty.AddHitModifier(this.hitModifier);
                hurtBoxProperty.OnReceived += this.ModifiedFeedback;
            }
        }

        public override void Exit()
        {
            base.Exit();
            foreach (HurtBoxAnimatorProperty hurtBoxProperty in this.OwnerAnimatorBody.GetReferencedProperties<HurtBoxAnimatorProperty>())
            {
                hurtBoxProperty.RemoveHitModifier(this.hitModifier);
                hurtBoxProperty.OnReceived -= this.ModifiedFeedback;
            }
        }

        private void ModifiedFeedback(HitInfo hitInfo)
        {
            if (!this.playAudioOnModified) return;
            if (hitInfo.AppliedHitModifiers.Contains(this.hitModifier))
            {
                foreach (AudioAnimatorProperty audioProperty in this.OwnerAnimatorBody.GetReferencedProperties<AudioAnimatorProperty>())
                {
                    if (!audioProperty.Loop) audioProperty.PlayOneShot(this.audioClipOnModified.MutableValue);
                }
            }
        }
    }
}
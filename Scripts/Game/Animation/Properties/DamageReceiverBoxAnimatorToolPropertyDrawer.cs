#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageReceiverBoxAnimatorToolPropertyDrawer<RB, DB, I> : ColliderAnimatorToolPropertyDrawer where RB : DamageReceiverBox<RB, DB, I> where DB : DamageDealerBox<DB, RB, I> where I : DamageInfo
    {
        public override void DrawGeneralEditFields()
        {
            DamageReceiverBoxAnimatorProperty<RB, DB, I> damageReceiverBoxProperty = (DamageReceiverBoxAnimatorProperty<RB, DB, I>)this.Property;
            damageReceiverBoxProperty.PlayAudioOnReceived = EditorGUILayout.Toggle("Play Audio On Received", damageReceiverBoxProperty.PlayAudioOnReceived);
            if (damageReceiverBoxProperty.PlayAudioOnReceived)
            {
                damageReceiverBoxProperty.AudioClipOnReceivedByReference = GUILayoutHelper.ObjectSerializedReferenceField<AudioClipSerializedReference, AudioClip>("Audio Clip On Received", damageReceiverBoxProperty.AudioClipOnReceivedByReference);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Material Tweens On Received");
            Utility.GUILayoutHelper.DrawIndexedList(
                damageReceiverBoxProperty.GetNumberMaterialTweensOnReceived(),
                damageReceiverBoxProperty.AddMaterialTweenOnReceivedAt,
                damageReceiverBoxProperty.RemoveMaterialTweenOnReceivedAt,
                (int index) => damageReceiverBoxProperty.SetMaterialTweenOnReceivedByReferenceAt(
                    index, 
                    GUILayoutHelper.MaterialTweenTemplateSerializedReferenceField("Material Tween On Received", damageReceiverBoxProperty.GetMaterialTweenOnReceivedByReferenceAt(index))
                    )
                );
            base.DrawGeneralEditFields();
        }
    }
}
#endif

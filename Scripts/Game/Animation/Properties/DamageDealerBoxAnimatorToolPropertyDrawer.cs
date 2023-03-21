#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageDealerBoxAnimatorToolPropertyDrawer<DB, RB, I> : ColliderAnimatorToolPropertyDrawer where DB : DamageDealerBox<DB, RB, I> where RB : DamageReceiverBox<RB, DB, I> where I : DamageInfo
    {
        public override void DrawGeneralEditFields()
        {
            DamageDealerBoxAnimatorProperty<DB, RB, I> damageDealerBoxProperty = (DamageDealerBoxAnimatorProperty<DB, RB, I>)this.Property;
            damageDealerBoxProperty.PlayAudioOnDealt = EditorGUILayout.Toggle("Play Audio On Dealt", damageDealerBoxProperty.PlayAudioOnDealt);
            if (damageDealerBoxProperty.PlayAudioOnDealt)
            {
                damageDealerBoxProperty.AudioClipOnDealtByReference = GUILayoutHelper.ObjectSerializedReferenceField<AudioClipSerializedReference, AudioClip>("Audio Clip On Dealt", damageDealerBoxProperty.AudioClipOnDealtByReference);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Material Tweens On Received");
            Utility.GUILayoutHelper.DrawIndexedList(
                damageDealerBoxProperty.GetNumberMaterialTweensOnDealt(),
                damageDealerBoxProperty.AddMaterialTweenOnDealtAt,
                damageDealerBoxProperty.RemoveMaterialTweenOnDealtAt,
                (int index) => damageDealerBoxProperty.SetMaterialTweenOnDealtByReferenceAt(
                    index,
                    GUILayoutHelper.MaterialTweenTemplateSerializedReferenceField("Material Tweens On Dealt", damageDealerBoxProperty.GetMaterialTweenOnDealtByReferenceAt(index))
                    )
                );
            base.DrawGeneralEditFields();
        }
    }
}
#endif

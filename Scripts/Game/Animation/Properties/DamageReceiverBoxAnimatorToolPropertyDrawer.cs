#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageReceiverBoxAnimatorToolPropertyDrawer<RB, DB, I> : ColliderAnimatorToolPropertyDrawer where RB : DamageReceiverBox<RB, DB, I> where DB : DamageDealerBox<DB, RB, I> where I : DamageInfo
    {
        public override void DrawGeneralEditFields()
        {
            DamageReceiverBoxAnimatorProperty<RB, DB, I> damageReceiverBoxProperty = (DamageReceiverBoxAnimatorProperty<RB, DB, I>)this.Property;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(typeof(RB).Name, EditorStyles.boldLabel);
                if (GUILayout.Button("Edit " + typeof(RB).Name))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(), 
                        new InspectorPopup(damageReceiverBoxProperty.gameObject)
                            .DoNotDraw(damageReceiverBoxProperty)
                            .DoNotDraw(damageReceiverBoxProperty.transform)
                            .DoNotMoveOrDelete(damageReceiverBoxProperty.GetComponent<RB>())
                            .DoNotDraw(damageReceiverBoxProperty.GetComponent<Collider2D>())
                        );
                }
            }
            damageReceiverBoxProperty.PlayAudioOnReceived = EditorGUILayout.Toggle("Play Audio On Received", damageReceiverBoxProperty.PlayAudioOnReceived);
            if (damageReceiverBoxProperty.PlayAudioOnReceived)
            {
                damageReceiverBoxProperty.AudioClipOnReceivedByReference = CoreGUILayout.ObjectSerializedReferenceField<AudioClipSerializedReference, AudioClip>("Audio Clip On Received", damageReceiverBoxProperty.AudioClipOnReceivedByReference);
            }
            EditorGUILayout.Space();
            UtilityGUILayout.IndexedList(
                "Material Tweens On Received",
                damageReceiverBoxProperty.GetNumberMaterialTweensOnReceived(),
                damageReceiverBoxProperty.AddMaterialTweenOnReceivedAt,
                damageReceiverBoxProperty.RemoveMaterialTweenOnReceivedAt,
                (int index) => damageReceiverBoxProperty.SetMaterialTweenOnReceivedByReferenceAt(
                    index, 
                    CoreGUILayout.MaterialTweenTemplateSerializedReferenceField("Material Tween On Received", damageReceiverBoxProperty.GetMaterialTweenOnReceivedByReferenceAt(index))
                    )
                );
            base.DrawGeneralEditFields();
        }
    }
}
#endif

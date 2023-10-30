#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class DamageDealerBoxAnimatorToolPropertyDrawer<DB, RB, I> : ColliderAnimatorToolPropertyDrawer where DB : DamageDealerBox<DB, RB, I> where RB : DamageReceiverBox<RB, DB, I> where I : DamageInfo
    {
        public override void DrawGeneralEditFields()
        {
            DamageDealerBoxAnimatorProperty<DB, RB, I> damageDealerBoxProperty = (DamageDealerBoxAnimatorProperty<DB, RB, I>)this.Property;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(typeof(DB).Name, EditorStyles.boldLabel);
                if (GUILayout.Button("Edit " + typeof(DB).Name))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(), 
                        new InspectorPopup(damageDealerBoxProperty.gameObject)
                            .DoNotDraw(damageDealerBoxProperty)
                            .DoNotDraw(damageDealerBoxProperty.transform)
                            .DoNotMoveOrDelete(damageDealerBoxProperty.GetComponent<DB>())
                            .DoNotDraw(damageDealerBoxProperty.GetComponent<Collider2D>())
                            );
                }
            }
            damageDealerBoxProperty.PlayAudioOnDealt = EditorGUILayout.Toggle("Play Audio On Dealt", damageDealerBoxProperty.PlayAudioOnDealt);
            if (damageDealerBoxProperty.PlayAudioOnDealt)
            {
                damageDealerBoxProperty.AudioClipOnDealtByReference = CoreGUILayout.ObjectSerializedReferenceField<AudioClipSerializedReference, AudioClip>("Audio Clip On Dealt", damageDealerBoxProperty.AudioClipOnDealtByReference);
            }
            EditorGUILayout.Space();
            UtilityGUILayout.IndexedList(
                "Material Tweens On Received",
                damageDealerBoxProperty.GetNumberMaterialTweensOnDealt(),
                damageDealerBoxProperty.AddMaterialTweenOnDealtAt,
                damageDealerBoxProperty.RemoveMaterialTweenOnDealtAt,
                (int tweenIndex) => damageDealerBoxProperty.SetMaterialTweenOnDealtByReferenceAt(tweenIndex, CoreGUILayout.MaterialTweenTemplateSerializedReferenceField("Material Tweens On Dealt", damageDealerBoxProperty.GetMaterialTweenOnDealtByReferenceAt(tweenIndex)))
                );
            base.DrawGeneralEditFields();
        }
    }
}
#endif

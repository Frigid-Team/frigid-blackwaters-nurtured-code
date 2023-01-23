#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(RootAnimatorProperty))]
    public class RootAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Root";
            }
        }

        public override Color AccentColor
        {
            get
            {
                return Color.white;
            }
        }

        public override void DrawGeneralEditFields()
        {
            RootAnimatorProperty rootProperty = (RootAnimatorProperty)this.Property;

            EditorGUILayout.LabelField("Sprite Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberSpriteProperties(),
                rootProperty.AddSpritePropertyAt,
                rootProperty.RemoveSpritePropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetSpritePropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Particle Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberParticleProperties(),
                rootProperty.AddParticlePropertyAt,
                rootProperty.RemoveParticlePropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetParticlePropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Sorting Group Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberSortingGroupProperties(),
                rootProperty.AddSortingGroupPropertyAt,
                rootProperty.RemoveSortingGroupPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetSortingGroupPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Sorting Point Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberSortingPointProperties(),
                rootProperty.AddSortingPointPropertyAt,
                rootProperty.RemoveSortingPointPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetSortingPointPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("HitBox Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberHitBoxProperties(),
                rootProperty.AddHitBoxPropertyAt,
                rootProperty.RemoveHitBoxPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetHitBoxPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("HurtBox Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberHurtBoxProperties(),
                rootProperty.AddHurtBoxPropertyAt,
                rootProperty.RemoveHurtBoxPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetHurtBoxPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("BreakBox Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberBreakBoxProperties(),
                rootProperty.AddBreakBoxPropertyAt,
                rootProperty.RemoveBreakBoxPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetBreakBoxPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("ResistBox Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberResistBoxProperties(),
                rootProperty.AddResistBoxPropertyAt,
                rootProperty.RemoveResistBoxPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetResistBoxPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("ThreatBox Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberThreatBoxProperties(),
                rootProperty.AddThreatBoxPropertyAt,
                rootProperty.RemoveThreatBoxPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetThreatBoxPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("LookoutBox Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberLookoutBoxProperties(),
                rootProperty.AddLookoutBoxPropertyAt,
                rootProperty.RemoveLookoutBoxPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetLookoutBoxPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Push Collider Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberPushColliderProperties(),
                rootProperty.AddPushColliderPropertyAt,
                rootProperty.RemovePushColliderPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetPushColliderPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Attack Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberAttackProperties(),
                rootProperty.AddAttackPropertyAt,
                rootProperty.RemoveAttackPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetAttackPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Audio Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberAudioProperties(),
                rootProperty.AddAudioPropertyAt,
                rootProperty.RemoveAudioPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetAudioPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Light Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                rootProperty.GetNumberLightProperties(),
                rootProperty.AddLightPropertyAt,
                rootProperty.RemoveLightPropertyAt,
                (int index) => EditorGUILayout.LabelField(rootProperty.GetLightPropertyAt(index).gameObject.name)
                );
            base.DrawGeneralEditFields();
        }
    }
}
#endif

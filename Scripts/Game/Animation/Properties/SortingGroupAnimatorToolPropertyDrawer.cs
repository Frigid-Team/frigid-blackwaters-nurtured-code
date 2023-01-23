#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(SortingGroupAnimatorProperty))]
    public class SortingGroupAnimatorToolPropertyDrawer : SortingOrderedAnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Sorting Group";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#cc00cc", out Color color);
                return color;
            }
        }

        public override void DrawGeneralEditFields()
        {
            SortingGroupAnimatorProperty sortingGroupProperty = (SortingGroupAnimatorProperty)this.Property;
            EditorGUILayout.LabelField("Sprite Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                sortingGroupProperty.GetNumberSpriteProperties(),
                sortingGroupProperty.AddSpritePropertyAt,
                sortingGroupProperty.RemoveSpritePropertyAt,
                (int index) => EditorGUILayout.LabelField(sortingGroupProperty.GetSpritePropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Particle Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                sortingGroupProperty.GetNumberParticleProperties(),
                sortingGroupProperty.AddParticlePropertyAt,
                sortingGroupProperty.RemoveParticlePropertyAt,
                (int index) => EditorGUILayout.LabelField(sortingGroupProperty.GetParticlePropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Sorting Group Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                sortingGroupProperty.GetNumberSortingGroupProperties(),
                sortingGroupProperty.AddSortingGroupPropertyAt,
                sortingGroupProperty.RemoveSortingGroupPropertyAt,
                (int index) => EditorGUILayout.LabelField(sortingGroupProperty.GetSortingGroupPropertyAt(index).gameObject.name)
                );
            EditorGUILayout.LabelField("Sorting Point Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                sortingGroupProperty.GetNumberSortingPointProperties(),
                sortingGroupProperty.AddSortingPointPropertyAt,
                sortingGroupProperty.RemoveSortingPointPropertyAt,
                (int index) => EditorGUILayout.LabelField(sortingGroupProperty.GetSortingPointPropertyAt(index).gameObject.name)
                );
            base.DrawGeneralEditFields();
        }
    }
}
#endif

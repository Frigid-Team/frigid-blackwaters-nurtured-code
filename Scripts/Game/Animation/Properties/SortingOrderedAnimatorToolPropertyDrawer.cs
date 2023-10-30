#if UNITY_EDITOR
using UnityEngine.Rendering;
using UnityEditor;

namespace FrigidBlackwaters.Game
{
    public abstract class SortingOrderedAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override float[] CalculateChildPreviewOrders(int animationIndex, int frameIndex, int orientationIndex)
        {
            SortingOrderedAnimatorProperty sortingOrderedProperty = (SortingOrderedAnimatorProperty)this.Property;
            return new float[] { WorldObjectPreviewOrder, sortingOrderedProperty.GetSortingOrder(animationIndex, frameIndex, orientationIndex) };
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            SortingOrderedAnimatorProperty sortingOrderedProperty = (SortingOrderedAnimatorProperty)this.Property;
            if (sortingOrderedProperty.transform.parent.GetComponentsInParent<SortingGroup>().Length > 0)
            {
                sortingOrderedProperty.SetSortingOrder(
                    animationIndex,
                    frameIndex, 
                    orientationIndex,
                    EditorGUILayout.IntField("Sorting Order", sortingOrderedProperty.GetSortingOrder(animationIndex, frameIndex, orientationIndex))
                    );
            }
            else
            {
                sortingOrderedProperty.SetSortingOrder(
                    animationIndex,
                    frameIndex, 
                    orientationIndex,
                    ElevationSorting.SortingOrderFromElevation((SortElevation)EditorGUILayout.EnumPopup("Elevation", ElevationSorting.ElevationFromSortingOrder(sortingOrderedProperty.GetSortingOrder(animationIndex, frameIndex, orientationIndex))))
                    );
            }
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

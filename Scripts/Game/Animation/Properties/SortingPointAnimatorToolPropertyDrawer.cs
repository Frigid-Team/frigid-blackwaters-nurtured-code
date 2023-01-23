#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(SortingPointAnimatorProperty))]
    public class SortingPointAnimatorToolPropertyDrawer : SortingOrderedAnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Sorting Point";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#9999ff", out Color color);
                return color;
            }
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            SortingPointAnimatorProperty sortingPointProperty = (SortingPointAnimatorProperty)this.Property;
            sortingPointProperty.SetLocalOffset(
                animationIndex,
                frameIndex,
                orientationIndex, 
                EditorGUILayout.Vector2Field("Local Offset", sortingPointProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex))
                );
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawPreview(
            Vector2 previewSize, 
            Vector2 previewOffset, 
            float worldToWindowScalingFactor,
            int animationIndex, 
            int frameIndex, 
            int orientationIndex,
            bool propertySelected,
            out List<(Rect rect, Action onDrag)> dragRequests
            )
        {
            SortingPointAnimatorProperty sortingPointProperty = (SortingPointAnimatorProperty)this.Property;
            dragRequests = new List<(Rect rect, Action onDrag)>();

            Vector2 currLocalOffset = sortingPointProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex);
            Vector2 drawPoint = previewSize / 2 + currLocalOffset * new Vector2(1, -1) * worldToWindowScalingFactor + previewOffset;
            Vector2 grabSize = new Vector2(this.Config.HandleGrabLength, this.Config.HandleGrabLength);
            using (new GUIHelper.ColorScope(propertySelected ? this.AccentColor : GUIStyling.Darken(this.AccentColor)))
            {
                GUIHelper.DrawLineArc(drawPoint, 0, 2 * Mathf.PI, this.Config.HandleGrabLength / 4);
                GUIHelper.DrawLine(drawPoint + Vector2.up * this.Config.HandleGrabLength / 2, drawPoint + Vector2.down * this.Config.HandleGrabLength / 2);
                GUIHelper.DrawLine(drawPoint + Vector2.left * this.Config.HandleGrabLength / 2, drawPoint + Vector2.right * this.Config.HandleGrabLength / 2);
            }
            if (propertySelected)
            {              
                dragRequests.Add(
                    (new Rect(drawPoint - grabSize / 2, grabSize), 
                    () => sortingPointProperty.SetLocalOffset(animationIndex, frameIndex, orientationIndex, currLocalOffset + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor))
                    );
            }
            base.DrawPreview(previewSize, previewOffset, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected, out List<(Rect rect, Action onDrag)> baseDragRequests);
        }
    }
}
#endif

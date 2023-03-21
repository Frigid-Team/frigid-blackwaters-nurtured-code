#if UNITY_EDITOR
using UnityEngine;

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
    }
}
#endif

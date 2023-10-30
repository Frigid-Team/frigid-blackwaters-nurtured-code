#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(LocalPositionFieldAnimatorProperty))]
    public class LocalPositionFieldAnimatorToolPropertyDrawer : OrientationFieldAnimatorToolPropertyDrawer<AnimatorProperty>
    {
        public override string LabelName
        {
            get
            {
                return "Local Position Field";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#9c988a", out Color color);
                return color;
            }
        }
    }
}
#endif

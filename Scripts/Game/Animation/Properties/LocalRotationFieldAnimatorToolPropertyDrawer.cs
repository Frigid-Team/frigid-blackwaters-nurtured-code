#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(LocalRotationFieldAnimatorProperty))]
    public class LocalRotationFieldAnimatorToolPropertyDrawer : OrientationFieldAnimatorToolPropertyDrawer<AnimatorProperty>
    {
        public override string LabelName
        {
            get
            {
                return "Local Rotation Field";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#919c8a", out Color color);
                return color;
            }
        }
    }
}
#endif

#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(LocalRotationInterpAnimatorProperty))]
    public class LocalRotationInterpAnimatorToolPropertyDrawer : FrameInterpAnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Local Rotation Interpolation";
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

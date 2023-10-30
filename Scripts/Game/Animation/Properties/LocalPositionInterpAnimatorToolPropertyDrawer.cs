#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(LocalPositionInterpAnimatorProperty))]
    public class LocalPositionInterpAnimatorToolPropertyDrawer : InterpAnimatorToolPropertyDrawer<AnimatorProperty>
    {
        public override string LabelName
        {
            get
            {
                return "Local Position Interpolation";
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

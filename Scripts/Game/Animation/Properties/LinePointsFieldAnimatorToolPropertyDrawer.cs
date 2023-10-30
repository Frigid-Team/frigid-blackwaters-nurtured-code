#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(LinePointsFieldAnimatorProperty))]
    public class LinePointsFieldAnimatorToolPropertyDrawer : OrientationFieldAnimatorToolPropertyDrawer<LineAnimatorProperty>
    {
        public override string LabelName
        {
            get
            {
                return "Line Points Field";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#3b43e3", out Color color);
                return color;
            }
        }
    }
}
#endif

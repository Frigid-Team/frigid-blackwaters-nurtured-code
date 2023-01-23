#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(BreakBoxAnimatorProperty))]
    public class BreakBoxAnimatorToolPropertyDrawer : DamageDealerBoxAnimatorToolPropertyDrawer<BreakBox, ResistBox, BreakInfo>
    {
        public override string LabelName
        {
            get
            {
                return "BreakBox";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#ff0000", out Color color);
                return color;
            }
        }
    }
}
#endif

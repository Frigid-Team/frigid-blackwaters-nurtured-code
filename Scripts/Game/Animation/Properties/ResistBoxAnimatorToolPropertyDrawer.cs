#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(ResistBoxAnimatorProperty))]
    public class ResistBoxAnimatorToolPropertyDrawer : DamageReceiverBoxAnimatorToolPropertyDrawer<ResistBox, BreakBox, BreakInfo>
    {
        public override string LabelName
        {
            get
            {
                return "ResistBox";
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

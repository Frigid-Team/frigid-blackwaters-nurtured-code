#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(LookoutBoxAnimatorProperty))]
    public class LookoutBoxAnimatorToolPropertyDrawer : DamageReceiverBoxAnimatorToolPropertyDrawer<LookoutBox, ThreatBox, ThreatInfo>
    {
        public override string LabelName
        {
            get
            {
                return "LookoutBox";
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

#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(ThreatBoxAnimatorProperty))]
    public class ThreatBoxAnimatorToolPropertyDrawer : DamageDealerBoxAnimatorToolPropertyDrawer<ThreatBox, LookoutBox, ThreatInfo>
    {
        public override string LabelName
        {
            get
            {
                return "ThreatBox";
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

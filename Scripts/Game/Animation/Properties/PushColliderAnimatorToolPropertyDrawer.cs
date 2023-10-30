#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(PushColliderAnimatorProperty))]
    public class PushColliderAnimatorToolPropertyDrawer : ColliderAnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Push Collider";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#00ffa2", out Color color);
                return color;
            }
        }
    }
}
#endif

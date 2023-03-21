#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(RootAnimatorProperty))]
    public class RootAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Root";
            }
        }

        public override Color AccentColor
        {
            get
            {
                return Color.white;
            }
        }
    }
}
#endif

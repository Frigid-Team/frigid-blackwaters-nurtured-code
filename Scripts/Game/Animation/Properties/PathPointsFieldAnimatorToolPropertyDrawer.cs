#if UNITY_EDITOR
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(PathPointsFieldAnimatorProperty))]
    public class PathPointsFieldAnimatorToolPropertyDrawer : OrientationFieldAnimatorToolPropertyDrawer<ColliderAnimatorProperty>
    {
        public override string LabelName
        {
            get
            {
                return "Path Points Field";
            }
        }

        public override Color AccentColor
        {
            get
            {
                return Color.green;
            }
        }
    }
}
#endif

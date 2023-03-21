using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MoveInPlace : Move
    {
        public override Vector2 Velocity
        {
            get
            {
                return Vector2.zero;
            }
        }
    }
}

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class PlayerMobDockBehaviour : MobBehaviour
    {
        // This class is used for some extra logic needed for the player when they dock.

        public override void Enter()
        {
            base.Enter();
            CharacterInput.LastInputtedMovementVector = -this.Owner.FacingDirection;
        }
    }
}

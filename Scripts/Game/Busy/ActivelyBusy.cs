using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ActivelyBusy : Busy<ActivelyBusy>
    {
        protected override void Started()
        {
            CharacterInput.Disabled.Request();
            InterfaceInput.Disabled.Request();
            TimePauser.Paused.Request();
            AudioPauser.Paused.Request();
        }

        protected override void Finished()
        {
            CharacterInput.Disabled.Release();
            InterfaceInput.Disabled.Release();
            TimePauser.Paused.Release();
            AudioPauser.Paused.Release();
        }
    }
}

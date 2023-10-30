namespace FrigidBlackwaters.Game
{
    public class PassivelyBusy : Busy<PassivelyBusy>
    {
        protected override void Finished() { }

        protected override void Started() { }
    }
}

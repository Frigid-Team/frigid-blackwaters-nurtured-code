namespace FrigidBlackwaters.Game
{
    public class PushColliderAnimatorProperty : ColliderAnimatorProperty
    {
        public int Layer
        {
            get
            {
                return this.gameObject.layer;
            }
            set
            {
                this.gameObject.layer = value;
            }
        }
    }
}

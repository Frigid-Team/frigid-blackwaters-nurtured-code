namespace FrigidBlackwaters.Game
{
    public abstract class MobBehaviour : FrigidMonoBehaviour
    {
        private Mob owner;
        private AnimatorBody ownerAnimatorBody;

        public abstract bool Finished 
        {
            get; 
        }

        public void Assign(Mob owner, AnimatorBody ownerAnimatorBody)
        {
            this.owner = owner;
            this.ownerAnimatorBody = ownerAnimatorBody;
        }

        public virtual void Apply() { }

        public virtual void Unapply() { }

        public virtual void Perform() { }

        protected Mob Owner
        {
            get
            {
                return this.owner;
            }
        }

        protected AnimatorBody OwnerAnimatorBody
        {
            get
            {
                return this.ownerAnimatorBody;
            }
        }
    }
}

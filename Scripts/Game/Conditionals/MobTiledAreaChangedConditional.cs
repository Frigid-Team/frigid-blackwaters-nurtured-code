using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobTiledAreaChangedConditional : FrameEventConditional
    {
        [SerializeField]
        private MobSerializedHandle mob;

        private Mob currentMob;

        protected override void Start()
        {
            base.Start();
            this.CheckCurrentMob();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            this.CheckCurrentMob();
        }

        private void CheckCurrentMob()
        {
            if (!this.mob.TryGetValue(out Mob nextMob))
            {
                nextMob = null;
            }

            if (this.currentMob != nextMob)
            {
                if (this.currentMob)
                {
                    this.currentMob.OnTiledAreaChanged -= this.Notify;
                }
                this.currentMob = nextMob;
                if (this.currentMob)
                {
                    this.currentMob.OnTiledAreaChanged += this.Notify;
                }
            }
        }

        private void Notify(TiledArea previousTiledArea, TiledArea currentTiledArea)
        {
            this.Notify();
        }
    }
}

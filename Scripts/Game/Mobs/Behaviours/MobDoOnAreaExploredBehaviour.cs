using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobDoOnAreaExploredBehaviour : MobBehaviour
    {
        [SerializeField]
        private List<MobBehaviour> childBehaviours;

        public override void Added()
        {
            base.Added();

            TiledWorldExplorer.OnExploredArea += this.DoBehaviours;
        }

        public override void Removed()
        {
            base.Removed();

            TiledWorldExplorer.OnExploredArea -= this.DoBehaviours;
        }

        private void DoBehaviours(TiledArea tiledArea)
        {
            foreach (MobBehaviour childBehaviour in this.childBehaviours)
            {
                MobBehaviour newBehaviour = CreateInstance<MobBehaviour>(childBehaviour);
                newBehaviour.transform.SetParent(this.Owner.transform);
                newBehaviour.transform.localPosition = Vector3.zero;
                this.Owner.DoBehaviour(newBehaviour, this.Owner.GetIsIgnoringTimeScale(this), () => DestroyInstance(newBehaviour));
            }
        }
    }
}
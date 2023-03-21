using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobDamageDealtBehaver<DB, RB, I> : FrigidMonoBehaviour where DB : DamageDealerBox<DB, RB, I> where RB : DamageReceiverBox<RB, DB, I> where I : DamageInfo
    {
        [SerializeField]
        private DB damageDealerBox;
        [SerializeField]
        private List<MobBehaviour> behaviourOriginals;

        private List<RecyclePool<MobBehaviour>> behaviourPools;

        protected override void Awake()
        {
            base.Awake();
            this.behaviourPools = new List<RecyclePool<MobBehaviour>>();
            foreach (MobBehaviour behaviourOriginal in this.behaviourOriginals)
            {
                this.behaviourPools.Add(
                    new RecyclePool<MobBehaviour>(
                        () => FrigidInstancing.CreateInstance<MobBehaviour>(behaviourOriginal),
                        (MobBehaviour behaviour) => FrigidInstancing.DestroyInstance(behaviour)
                        )
                    );
                this.behaviourPools[this.behaviourPools.Count - 1].Pool(behaviourOriginal);
            }
            this.damageDealerBox.OnDealt += DoBehavioursOnDamage;
        }

        private void DoBehavioursOnDamage(I damageInfo)
        {
            if (damageInfo.IsNonTrivial && damageInfo.Collision.attachedRigidbody.TryGetComponent<Mob>(out Mob target))
            {
                foreach (RecyclePool<MobBehaviour> behaviourPool in this.behaviourPools)
                {
                    MobBehaviour behaviour = behaviourPool.Retrieve();
                    behaviour.transform.SetParent(target.transform);
                    target.DoBehaviour(
                        behaviour,
                        true,
                        () =>
                        {
                            behaviour.transform.SetParent(this.transform);
                            behaviourPool.Pool(behaviour);
                        }
                        );
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobAuraBehaviour : MobBehaviour
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private bool affectsOwner;
        [SerializeField]
        private bool affectsAllies;
        [SerializeField]
        private FloatSerializedReference effectRadius;
        [SerializeField]
        private List<MobBehaviour> auraBehaviourOriginals;

        private Dictionary<Mob, List<MobBehaviour>> inAuraMap;

        public override void Enter()
        {
            base.Enter();
            this.inAuraMap = new Dictionary<Mob, List<MobBehaviour>>();
        }

        public override void Refresh()
        {
            base.Refresh();
            
            foreach (Mob affector in Mob.GetActiveMobsIn(this.Owner.TiledArea).ThatAreOfAlignments(this.affectsAllies ? this.Owner.PassiveAlignments : this.Owner.HostileAlignments).ThatAreNotDead())
            {
                if (Vector2.Distance(affector.Position, this.originTargeter.Calculate(Vector2.zero, 0, 0)) <= this.effectRadius.ImmutableValue && (this.affectsOwner || affector != this.Owner))
                {
                    if (!this.inAuraMap.ContainsKey(affector))
                    {
                        List<MobBehaviour> auraBehaviours = new List<MobBehaviour>();
                        foreach (MobBehaviour auraBehaviourOriginal in this.auraBehaviourOriginals)
                        {
                            auraBehaviours.Add(FrigidInstancing.CreateInstance<MobBehaviour>(auraBehaviourOriginal));
                        }
                        foreach(MobBehaviour behaviour in auraBehaviours)
                        {
                            affector.AddBehaviour(behaviour, this.Owner.GetIsIgnoringTimeScale(this));
                        }
                        this.inAuraMap.Add(affector, auraBehaviours);
                    }
                }
                else
                {
                    if (this.inAuraMap.ContainsKey(affector))
                    {
                        foreach (MobBehaviour behaviour in this.inAuraMap[affector])
                        {
                            affector.RemoveBehaviour(behaviour);
                            FrigidInstancing.DestroyInstance(behaviour);
                        }
                        this.inAuraMap.Remove(affector);
                    }
                }
            }
        }
    }
}
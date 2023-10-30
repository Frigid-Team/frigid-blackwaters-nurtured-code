using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobAuraBehaviour : MobBehaviour
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private MobQuery affecteeQuery;
        [SerializeField]
        private FloatSerializedReference effectRadius;
        [SerializeField]
        private List<MobBehaviour> childBehaviours;

        private Dictionary<Mob, List<MobBehaviour>> affecteesAndBehaviours;

        public override void Enter()
        {
            base.Enter();
            this.affecteesAndBehaviours = new Dictionary<Mob, List<MobBehaviour>>();
        }

        public override void Exit()
        {
            base.Exit();
            List<Mob> affectees = this.affecteesAndBehaviours.Keys.ToList();
            foreach (Mob affectee in affectees)
            {
                this.RemoveAffectee(affectee);
            }
            this.affecteesAndBehaviours = null;
        }

        public override void Refresh()
        {
            base.Refresh();

            List<Mob> potentialAffectees = this.affecteeQuery.Execute();
            foreach (Mob potentialAffectee in potentialAffectees)
            {
                if (this.affecteesAndBehaviours.ContainsKey(potentialAffectee))
                {
                    continue;
                }

                if (Vector2.Distance(potentialAffectee.Position, this.originTargeter.Retrieve(Vector2.zero, 0, 0)) <= this.effectRadius.ImmutableValue && potentialAffectee.Status != MobStatus.Dead)
                {
                    this.AddAffectee(potentialAffectee);
                }
            }

            List<Mob> affectees = this.affecteesAndBehaviours.Keys.ToList();
            foreach (Mob affectee in affectees)
            {
                if (Vector2.Distance(affectee.Position, this.originTargeter.Retrieve(Vector2.zero, 0, 0)) > this.effectRadius.ImmutableValue || affectee.Status == MobStatus.Dead)
                {
                    this.RemoveAffectee(affectee);
                }
            }
        }

        private void AddAffectee(Mob affectee)
        {
            List<MobBehaviour> auraBehaviours = new List<MobBehaviour>();
            foreach (MobBehaviour childBehaviour in this.childBehaviours)
            {
                auraBehaviours.Add(CreateInstance<MobBehaviour>(childBehaviour));
            }
            foreach (MobBehaviour behaviour in auraBehaviours)
            {
                behaviour.transform.SetParent(affectee.transform);
                behaviour.transform.localPosition = Vector3.zero;
                affectee.AddBehaviour(behaviour, this.Owner.GetIsIgnoringTimeScale(this));
            }
            this.affecteesAndBehaviours.Add(affectee, auraBehaviours);
        }

        private void RemoveAffectee(Mob affectee)
        {
            foreach (MobBehaviour behaviour in this.affecteesAndBehaviours[affectee])
            {
                affectee.RemoveBehaviour(behaviour);
                behaviour.transform.SetParent(this.transform);
                DestroyInstance(behaviour);
            }
            this.affecteesAndBehaviours.Remove(affectee);
        }
    }
}
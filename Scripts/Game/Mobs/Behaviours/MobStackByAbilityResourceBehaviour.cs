using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobStackByAbilityResourceBehaviour : MobBehaviour
    {
        [SerializeField]
        private AbilityResource abilityResource;
        [SerializeField]
        private MobBehaviour childBehaviour;

        private List<MobBehaviour> stackedBehaviours;

        public override void Enter()
        {
            base.Enter();
            this.stackedBehaviours = new List<MobBehaviour>();
        }

        public override void Refresh()
        {
            base.Refresh();

            int prevStackCount = this.stackedBehaviours.Count;
            int newStackCount = this.abilityResource.Quantity != -1 ? this.abilityResource.Quantity : 0;

            if (newStackCount == prevStackCount)
            {
                return;
            }

            for (int i = prevStackCount; i > newStackCount; i--)
            {
                MobBehaviour removedBehaviour = this.stackedBehaviours[this.stackedBehaviours.Count - 1];
                this.stackedBehaviours.Remove(removedBehaviour);
                this.Owner.RemoveBehaviour(removedBehaviour);
                removedBehaviour.transform.SetParent(this.transform);
                DestroyInstance(removedBehaviour);
            }

            for (int i = prevStackCount; i < newStackCount; i++)
            {
                MobBehaviour newBehaviour = CreateInstance<MobBehaviour>(this.childBehaviour);
                newBehaviour.transform.SetParent(this.Owner.transform);
                newBehaviour.transform.localPosition = Vector3.zero;
                this.Owner.AddBehaviour(newBehaviour, this.Owner.GetIsIgnoringTimeScale(this));
                this.stackedBehaviours.Add(newBehaviour);
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobDoOnConditionsMetBehaviour : MobBehaviour
    {
        [SerializeField]
        private Conditional condition;
        [SerializeField]
        private List<MobBehaviour> childBehaviours;

        public override void Refresh()
        {
            base.Refresh();
            int numberOccurrences = this.condition.Tally(this.EnterDuration, this.EnterDurationDelta);
            for (int i = 0; i < numberOccurrences; i++)
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
}

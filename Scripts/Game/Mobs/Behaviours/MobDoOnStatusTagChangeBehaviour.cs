using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobDoOnStatusTagChangeBehaviour : MobBehaviour
    {
        [SerializeField]
        private List<MobBehaviour> childBehaviours;
        [SerializeField]
        private List<MobStatusTag> statusTagsToCheck;
        [SerializeField]
        private bool onAdd;
        [SerializeField]
        private bool onRemove;

        public override void Added()
        {
            base.Added();
            if (this.onAdd)
            {
                this.Owner.OnStatusTagAdded += this.DoBehaviours;
            }
            if (this.onRemove)
            {
                this.Owner.OnStatusTagRemoved += this.DoBehaviours;
            }
        }

        public override void Removed()
        {
            base.Removed();
            if (this.onAdd)
            {
                this.Owner.OnStatusTagAdded -= this.DoBehaviours;
            }
            if (this.onRemove)
            {
                this.Owner.OnStatusTagRemoved -= this.DoBehaviours;
            }
        }

        private void DoBehaviours(MobStatusTag statusTag)
        {
            if (this.statusTagsToCheck.Contains(statusTag))
            {
                foreach (MobBehaviour childBehaviour in this.childBehaviours)
                {
                    MobBehaviour newBehaviour = CreateInstance<MobBehaviour>(childBehaviour);
                    newBehaviour.transform.SetParent(this.Owner.transform);
                    newBehaviour.transform.localPosition = Vector2.zero;
                    this.Owner.DoBehaviour(newBehaviour, this.Owner.GetIsIgnoringTimeScale(this), () => DestroyInstance(newBehaviour));
                }
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class ItemRule : FrigidMonoBehaviour
    {
        [SerializeField]
        private string ruleName;
        [SerializeField]
        private List<MobClassification> classifications;
        [SerializeField]
        private List<MobBehaviour> originalBehaviours;

        private Dictionary<Mob, List<MobBehaviour>> appliedBehaviours;

        public void ApplyToMob(Mob mob)
        {
            if (this.classifications.Contains(mob.Classification) && !this.appliedBehaviours.ContainsKey(mob))
            {
                this.appliedBehaviours.Add(mob, new List<MobBehaviour>());
                foreach (MobBehaviour originalBehaviour in this.originalBehaviours)
                {
                    MobBehaviour spawnedBehaviour = FrigidInstancing.CreateInstance<MobBehaviour>(originalBehaviour);
                    spawnedBehaviour.transform.SetParent(mob.transform);
                    spawnedBehaviour.transform.localPosition = Vector2.zero;
                    this.appliedBehaviours[mob].Add(spawnedBehaviour);
                    mob.AddBehaviour(spawnedBehaviour);
                }
            }
        }

        public void UnapplyToMob(Mob mob)
        {
            if (this.appliedBehaviours.ContainsKey(mob))
            {
                foreach (MobBehaviour appliedBehaviour in this.appliedBehaviours[mob])
                {
                    mob.RemoveBehaviour(appliedBehaviour);
                    FrigidInstancing.DestroyInstance(appliedBehaviour);
                }
                this.appliedBehaviours.Remove(mob);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.appliedBehaviours = new Dictionary<Mob, List<MobBehaviour>>();
        }
    }
}

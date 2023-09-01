using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipPointTriggeredConditional : Conditional
    {
        [SerializeField]
        private MobEquipPointSerializedHandle mobEquipPoint;

        protected override bool CustomEvaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mobEquipPoint.TryGetValue(out MobEquipPoint mobEquipPoint) && mobEquipPoint.IsTriggered;
        }
    }
}

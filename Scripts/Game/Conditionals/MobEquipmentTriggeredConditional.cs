using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobEquipmentTriggeredConditional : Conditional
    {
        [SerializeField]
        private MobEquipmentPiece mobEquipmentPiece;

        public override bool Evaluate(float elapsedDuration, float elapsedDurationDelta)
        {
            return this.mobEquipmentPiece.EquipPoint.Triggered;
        }
    }
}

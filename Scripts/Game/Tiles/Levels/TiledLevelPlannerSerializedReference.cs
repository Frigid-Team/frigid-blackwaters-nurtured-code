using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class TiledLevelPlannerSerializedReference : SerializedReference<TiledLevelPlanner>
    {
        public TiledLevelPlannerSerializedReference() : base() { }

        public TiledLevelPlannerSerializedReference(TiledLevelPlannerSerializedReference other) : base(other) { }

        public TiledLevelPlannerSerializedReference(SerializedReferenceType referenceType, TiledLevelPlanner customValue, ScriptableConstant<TiledLevelPlanner> scriptableConstant, List<TiledLevelPlanner> selection, ScriptableVariable<TiledLevelPlanner> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}

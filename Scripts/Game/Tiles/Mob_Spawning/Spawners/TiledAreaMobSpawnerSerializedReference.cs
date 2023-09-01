using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [Serializable]
    public class TiledAreaMobSpawnerSerializedReference : SerializedReference<TiledAreaMobSpawner> 
    {
        public TiledAreaMobSpawnerSerializedReference() : base() { }

        public TiledAreaMobSpawnerSerializedReference(TiledAreaMobSpawnerSerializedReference other) : base(other) { }

        public TiledAreaMobSpawnerSerializedReference(SerializedReferenceType referenceType, TiledAreaMobSpawner customValue, ScriptableConstant<TiledAreaMobSpawner> scriptableConstant, List<TiledAreaMobSpawner> selection, ScriptableVariable<TiledAreaMobSpawner> scriptableVariable) : base(referenceType, customValue, scriptableConstant, selection, scriptableVariable) { }
    }
}

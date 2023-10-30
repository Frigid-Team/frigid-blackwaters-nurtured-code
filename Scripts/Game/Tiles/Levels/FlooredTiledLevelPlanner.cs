using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "FlooredTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "FlooredTiledLevelPlanner")]
    public class FlooredTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private List<RelativeWeightPool<TiledLevelPlanner>> plannersPerFloor;

        private SceneVariable<int> numFloorsCreated;

        protected override void OnBegin()
        {
            base.OnBegin();
            this.numFloorsCreated = new SceneVariable<int>(() => 0);
        }

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            if (this.numFloorsCreated.Current >= this.plannersPerFloor.Count) Debug.LogWarning("FlooredTiledLevelPlanner " + this.name + " is exceeding the number of floors.");
            TiledLevelPlanner chosenFloorPlanner = this.plannersPerFloor[Mathf.Min(this.numFloorsCreated.Current, this.plannersPerFloor.Count - 1)].Retrieve();
            this.numFloorsCreated.Current++;
            return chosenFloorPlanner.CreateLevelPlan(subLevelEntrancesAndContainedAreas);
        }
    }
}


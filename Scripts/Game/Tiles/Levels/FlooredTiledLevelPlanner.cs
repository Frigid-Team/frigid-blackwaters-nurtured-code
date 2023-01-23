using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "FlooredTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "FlooredTiledLevelPlanner")]
    public class FlooredTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private List<RelativeWeightPool<TiledLevelPlanner>> plannersPerFloor;

        private SceneVariable<int> numFloorsCreated;

        protected override void Init()
        {
            base.Init();
            this.numFloorsCreated = new SceneVariable<int>(() => { return 0; });
        }

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledAreaEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            TiledLevelPlanner chosenFloorPlanner = this.plannersPerFloor[this.numFloorsCreated.Current].Retrieve();
            this.numFloorsCreated.Current = Mathf.Min(this.numFloorsCreated.Current + 1, this.plannersPerFloor.Count);
            return chosenFloorPlanner.CreateLevelPlan(subLevelEntrancesAndContainedAreas);
        }
    }
}


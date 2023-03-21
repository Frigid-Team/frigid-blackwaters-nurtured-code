using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class TiledLevelPlanner : FrigidScriptableObject
    {
        public TiledLevelPlan CreateLevelPlan(Dictionary<TiledAreaEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            TiledLevelPlan tiledLevelPlan = CreateInitialLevelPlan(subLevelEntrancesAndContainedAreas);
            SyncConnectionTerrains(tiledLevelPlan);
            CalculateDistancesFromStart(tiledLevelPlan);
            ChooseBlueprints(tiledLevelPlan);
            return tiledLevelPlan;
        }

        protected abstract TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledAreaEntrance, TiledArea> subLevelEntrancesAndContainedAreas);

        private void SyncConnectionTerrains(TiledLevelPlan tiledLevelPlan)
        {
            // We sort the connections by the number of unique entrance terrains they can fill. This is to minimize any
            // terrain conflicts.
            List<TiledLevelPlanConnection> connections = tiledLevelPlan.Connections.ToList<TiledLevelPlanConnection>();
            connections.Sort(
                (TiledLevelPlanConnection firstConnection, TiledLevelPlanConnection secondConnection) =>
                {
                    int CalcScore(TiledLevelPlanConnection connection)
                    {
                        int score = 0;
                        if (!connection.FirstEntrance.IsSubLevelEntrance) score += connection.FirstEntrance.Area.BlueprintGroup.NumberAvailableEntranceTerrains;
                        if (!connection.SecondEntrance.IsSubLevelEntrance) score += connection.SecondEntrance.Area.BlueprintGroup.NumberAvailableEntranceTerrains;
                        return score;
                    }
                    return CalcScore(secondConnection) - CalcScore(firstConnection);
                }
                );
            foreach (TiledLevelPlanConnection planConnection in connections)
            {
                if (planConnection.ConnectionTerrain == TileTerrain.None)
                {
                    List<TileTerrain> availableConnectionTerrains = new List<TileTerrain>();
                    for (int i = (int)TileTerrain.None + 1; i < (int)TileTerrain.Count; i++)
                    {
                        bool available = true;
                        if (!planConnection.FirstEntrance.IsSubLevelEntrance)
                        {
                            TileTerrain[] validEntrances = planConnection.FirstEntrance.Area.EntranceTerrains;
                            validEntrances[TilePositioning.WallArrayIndex(planConnection.Direction)] = (TileTerrain)i;
                            if (planConnection.FirstEntrance.Area.BlueprintGroup.GetMatchingEntranceTerrainBlueprints(validEntrances).Count == 0)
                            {
                                available = false;
                            }
                        }
                        if (!planConnection.SecondEntrance.IsSubLevelEntrance)
                        {
                            TileTerrain[] validEntrances = planConnection.SecondEntrance.Area.EntranceTerrains;
                            validEntrances[TilePositioning.WallArrayIndex(-planConnection.Direction)] = (TileTerrain)i;
                            if (planConnection.SecondEntrance.Area.BlueprintGroup.GetMatchingEntranceTerrainBlueprints(validEntrances).Count == 0)
                            {
                                available = false;
                            }
                        }
                        if (available)
                        {
                            availableConnectionTerrains.Add((TileTerrain)i);
                        }
                    }

                    if (availableConnectionTerrains.Count == 0)
                    {
                        Debug.LogError("It is impossible to generate valid terrain connections for " + this.name + ".");
                        break;
                    }

                    planConnection.ConnectionTerrain = availableConnectionTerrains[Random.Range(0, availableConnectionTerrains.Count)];
                }
            }
        }

        private void CalculateDistancesFromStart(TiledLevelPlan tiledLevelPlan)
        {
            Queue<TiledLevelPlanArea> queuedPlanAreas = new Queue<TiledLevelPlanArea>();
            HashSet<TiledLevelPlanArea> visitedPlanAreas = new HashSet<TiledLevelPlanArea>();
            tiledLevelPlan.StartingArea.NumberAreasFromStart = 0;
            queuedPlanAreas.Enqueue(tiledLevelPlan.StartingArea);
            visitedPlanAreas.Add(tiledLevelPlan.StartingArea);

            int maxNumberAreasFromStart = 0;
            while (queuedPlanAreas.Count > 0)
            {
                TiledLevelPlanArea dequeuedPlanArea = queuedPlanAreas.Dequeue();
                foreach (TiledLevelPlanConnection planConnection in tiledLevelPlan.Connections)
                {
                    bool isConnected = false;
                    TiledLevelPlanArea connectedPlanArea = null;

                    if (planConnection.FirstEntrance.Area == dequeuedPlanArea) isConnected = true;
                    else connectedPlanArea = planConnection.FirstEntrance.Area;

                    if (planConnection.SecondEntrance.Area == dequeuedPlanArea) isConnected = true;
                    else connectedPlanArea = planConnection.SecondEntrance.Area;

                    if (isConnected && !planConnection.IsSubLevelConnection)
                    {
                        if (!visitedPlanAreas.Contains(connectedPlanArea))
                        {
                            connectedPlanArea.NumberAreasFromStart = dequeuedPlanArea.NumberAreasFromStart + 1;
                            if (connectedPlanArea.NumberAreasFromStart > maxNumberAreasFromStart)
                            {
                                maxNumberAreasFromStart = connectedPlanArea.NumberAreasFromStart;
                            }
                            queuedPlanAreas.Enqueue(connectedPlanArea);
                            visitedPlanAreas.Add(connectedPlanArea);
                        }
                    }
                }
            }
            foreach (TiledLevelPlanArea visitedPlanArea in visitedPlanAreas)
            {
                visitedPlanArea.NumberAreasFromStartPercent = maxNumberAreasFromStart == 0 ? 1 : ((float)visitedPlanArea.NumberAreasFromStart / maxNumberAreasFromStart);
            }
        }

        private void ChooseBlueprints(TiledLevelPlan tiledLevelPlan)
        {
            foreach (TiledLevelPlanArea planArea in tiledLevelPlan.Areas)
            {
                List<TiledAreaBlueprint> blueprints = planArea.BlueprintGroup.GetMatchingEntranceTerrainBlueprints(planArea.EntranceTerrains);
                TiledAreaBlueprint chosenBlueprint = blueprints[Random.Range(0, blueprints.Count)];
                planArea.ChosenBlueprint = chosenBlueprint;
            }
        }
    }
}

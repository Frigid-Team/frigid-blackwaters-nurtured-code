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
                    int firstScore = 0;
                    foreach (TiledLevelPlanEntrance planEntrance in firstConnection.PlanEntrances)
                    {
                        if (!planEntrance.IsSubLevelEntrance) firstScore += planEntrance.Area.BlueprintGroup.NumberAvailableEntranceTerrains;
                    }
                    int secondScore = 0;
                    foreach (TiledLevelPlanEntrance planEntrance in secondConnection.PlanEntrances)
                    {
                        if (!planEntrance.IsSubLevelEntrance) secondScore += planEntrance.Area.BlueprintGroup.NumberAvailableEntranceTerrains;
                    }
                    return secondScore - firstScore;
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
                        for (int j = 0; j < planConnection.NumberEntrances; j++)
                        {
                            if (planConnection.PlanEntrances[j].IsSubLevelEntrance) continue;

                            TileTerrain[] validEntrances = planConnection.PlanEntrances[j].Area.EntranceTerrains;
                            validEntrances[TilePositioning.WallArrayIndex(planConnection.EntryDirections[j])] = (TileTerrain)i;
                            if (planConnection.PlanEntrances[j].Area.BlueprintGroup.GetMatchingEntranceTerrainBlueprints(validEntrances).Count == 0)
                            {
                                available = false;
                                break;
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
                    List<TiledLevelPlanArea> connectedPlanAreas = new List<TiledLevelPlanArea>();

                    for (int i = 0; i < planConnection.NumberEntrances; i++)
                    {
                        if (planConnection.PlanEntrances[i].Area == dequeuedPlanArea)
                        {
                            isConnected = true;
                        }
                        else
                        {
                            if (!planConnection.IsSubLevelConnection)
                            {
                                connectedPlanAreas.Add(planConnection.PlanEntrances[i].Area);
                            }
                        }
                    }

                    if (isConnected)
                    {
                        foreach (TiledLevelPlanArea connectedPlanArea in connectedPlanAreas)
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

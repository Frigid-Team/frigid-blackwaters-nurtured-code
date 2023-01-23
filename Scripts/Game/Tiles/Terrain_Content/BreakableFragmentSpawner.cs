using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class BreakableFragmentSpawner : FrigidMonoBehaviour
    {
        [SerializeField]
        private int minNumberFragments;
        [SerializeField]
        private int maxNumberFragments;
        [SerializeField]
        private List<BreakableFragment> fragmentPrefabs;
        [SerializeField]
        private TerrainObstruction terrainObstruction;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.terrainObstruction.OnBroken += SpawnBreakableFragments;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.terrainObstruction.OnBroken -= SpawnBreakableFragments;
        }

        private void SpawnBreakableFragments()
        {
            int numberFragments = Random.Range(this.minNumberFragments, this.maxNumberFragments + 1);
            for (int i = 0; i < numberFragments; i++)
            {
                float randAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
                Vector3 force = new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
                BreakableFragment spawnedFragment =
                    FrigidInstancing.CreateInstance<BreakableFragment>(this.fragmentPrefabs[Random.Range(0, this.fragmentPrefabs.Count)], this.transform.position, this.transform);
                spawnedFragment.LaunchFragment(force);
            }
        }
    }
}

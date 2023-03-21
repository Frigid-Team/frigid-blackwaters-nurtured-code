using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters
{
    public static class FrigidLayerMask
    {
        static Dictionary<FrigidLayer, LayerMask> masks;

        static FrigidLayerMask()
        {
            masks = new Dictionary<FrigidLayer, LayerMask>();
            for (int i = 0; i < (int)FrigidLayer.Count; i++)
            {
                int mask = 0;
                for (int j = 0; j < (int)FrigidLayer.Count; j++)
                {
                    if (!Physics2D.GetIgnoreLayerCollision(i, j))
                    {
                        mask = mask | 1 << j;
                    }
                }
                masks[(FrigidLayer)i] = mask;
            }
        }

        public static LayerMask GetCollidingMask(FrigidLayer layer)
        {
            return masks[layer];
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "BoonExchangeLayout", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Boons + "BoonExchangeLayout")]
    public class BoonExchangeLayout : FrigidScriptableObject
    {
        [SerializeField]
        private List<BoonLocation> boonLocations;
        [SerializeField]
        private Sprite backgroundImage;

        public List<BoonLocation> BoonLocations
        {
            get
            {
                return this.boonLocations;
            }
        }

        public Sprite BackgroundImage
        {
            get
            {
                return this.backgroundImage;
            }
        }

        [Serializable]
        public struct BoonLocation
        {
            [SerializeField]
            private Boon boon;
            [SerializeField]
            private Vector2 position;

            public Boon Boon
            {
                get
                {
                    return this.boon;
                }
            }

            public Vector2 Position
            {
                get
                {
                    return this.position;
                }
            }
        }
    }
}
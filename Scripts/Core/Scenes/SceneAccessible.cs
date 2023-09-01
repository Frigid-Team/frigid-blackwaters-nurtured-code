using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public class SceneAccessible<T> : FrigidMonoBehaviour where T : SceneAccessible<T>
    {
        [SerializeField]
        private List<AccessZone> accessZones;

        private static SceneVariable<HashSet<T>> accessibles;

        static SceneAccessible()
        {
            accessibles = new SceneVariable<HashSet<T>>(() => { return new HashSet<T>(); });
        }

        public static bool TryFindNearest(Vector2 position, out T nearest, out Vector2 nearestAccessPosition)
        {
            return TryFindNearest(position, new List<T>(), out nearest, out nearestAccessPosition);
        }

        public static bool TryFindNearest(Vector2 position, List<T> excluded, out T nearest, out Vector2 nearestAccessPosition)
        {
            float closestDistance = float.MaxValue;
            nearest = null;
            nearestAccessPosition = position;
            bool found = false;
            foreach (T accessible in accessibles.Current)
            {
                if (!excluded.Contains(accessible) &&
                    accessible.IsAccessibleFromPosition(position, out Vector2 accessPosition) &&
                    Vector2.Distance(accessPosition, position) < closestDistance)
                {
                    closestDistance = Vector2.Distance(accessPosition, position);
                    nearest = accessible;
                    nearestAccessPosition = accessPosition;
                    found = true;
                }
            }
            return found;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            accessibles.Current.Add((T)this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            accessibles.Current.Remove((T)this);
        }

        private bool IsAccessibleFromPosition(Vector2 position, out Vector2 accessPosition)
        {
            float closestDistance = float.MaxValue;
            accessPosition = position;
            bool foundAccess = false;
            foreach (AccessZone accessZone in this.accessZones)
            {
                if (position.WithinBox(accessZone.AccessPoint, accessZone.AccessSize))
                {
                    if (Vector2.Distance(position, accessZone.AccessPoint) < closestDistance)
                    {
                        closestDistance = Vector2.Distance(position, accessZone.AccessPoint);
                        accessPosition = accessZone.AccessPoint;
                        foundAccess = true;
                    }
                }
            }
            return foundAccess;
        }

        [Serializable]
        private struct AccessZone
        {
            [SerializeField]
            private Vector2SerializedReference accessSize;
            [SerializeField]
            private Transform accessPointTransform;

            public Vector2 AccessSize
            {
                get
                {
                    return this.accessSize.ImmutableValue;
                }
            }

            public Vector2 AccessPoint
            {
                get
                {
                    return this.accessPointTransform.position;
                }
            }
        }
    }
}

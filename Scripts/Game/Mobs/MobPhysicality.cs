using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobPhysicality
    {
        private const int SIGHT_LAYER_MASK = 0b00000010000111111110000000000000;

        private AnimatorBody animatorBody;
        private MobStateNode rootStateNode;

        private int[] numModeRequests;
        private MobPushMode currentMode;

        public MobPhysicality(AnimatorBody animatorBody, MobStateNode rootStateNode)
        {
            this.animatorBody = animatorBody;
            this.rootStateNode = rootStateNode;
            this.rootStateNode.OnCurrentStateChanged += UpdateAllColliders;

            this.numModeRequests = new int[(int)MobPushMode.Count];
            for (int i = 0; i < this.numModeRequests.Length; i++) this.numModeRequests[i] = 0;
            this.currentMode = MobPushMode.IgnoreNone;
        }

        public MobPushMode CurrentMode
        {
            get
            {
                return this.currentMode;
            }
        }

        public int Layer
        {
            get
            {
                int pushLayer = (int)FrigidLayer.IgnoreRaycast;
                TraversableTerrain traversableTerrain = this.rootStateNode.CurrentState.TraversableTerrain;
                switch (this.currentMode)
                {
                    case MobPushMode.IgnoreNone:
                        if (traversableTerrain.Includes(TileTerrain.Land) && traversableTerrain.Includes(TileTerrain.Water)) pushLayer = (int)FrigidLayer.FloatingMob;
                        else if (traversableTerrain.Includes(TileTerrain.Land)) pushLayer = (int)FrigidLayer.LandMob;
                        else if (traversableTerrain.Includes(TileTerrain.Water)) pushLayer = (int)FrigidLayer.WaterMob;
                        break;
                    case MobPushMode.IgnoreMobs:
                        if (traversableTerrain.Includes(TileTerrain.Land) && traversableTerrain.Includes(TileTerrain.Water)) pushLayer = (int)FrigidLayer.IgnoringMobsFloatingMob;
                        else if (traversableTerrain.Includes(TileTerrain.Land)) pushLayer = (int)FrigidLayer.IgnoringMobsLandMob;
                        else if (traversableTerrain.Includes(TileTerrain.Water)) pushLayer = (int)FrigidLayer.IgnoringMobsWaterMob;
                        break;
                    case MobPushMode.IgnoreMobsAndTerrain:
                        pushLayer = (int)FrigidLayer.IgnoringMobsAndTerrainMob;
                        break;
                }
                return pushLayer;
            }
        }

        public void RequestMode(MobPushMode mode)
        {
            this.numModeRequests[(int)mode]++;
            UpdateCurrentMode();
        }

        public void ReleaseMode(MobPushMode mode)
        {
            this.numModeRequests[(int)mode]--;
            UpdateCurrentMode();
        }

        public bool IsInSightFrom(Vector2 originPosition, Vector2 sightPosition, float blockingRadius)
        {
            List<Collider2D> sightColliders = LineSightCast(originPosition, sightPosition - originPosition, Vector2.Distance(sightPosition, originPosition), blockingRadius);
            foreach (Collider2D sightCollider in sightColliders)
            {
                if (!sightCollider.bounds.Contains(sightPosition))
                {
                    return false;
                }
            }
            return true;
        }

        public List<Collider2D> OverlapPushPoint(Vector2 originPosition)
        {
            List<Collider2D> otherColliders = new List<Collider2D>();
            if (this.currentMode != MobPushMode.IgnoreEverything)
            {
                int layer = this.Layer;
                Vector2 size = this.rootStateNode.CurrentState.Size;
                if (size.magnitude > 0)
                {
                    Collider2D[] overlappedColliders = Physics2D.OverlapBoxAll(originPosition, size, 0, layer);
                    foreach (Collider2D overlappedCollider in overlappedColliders)
                    {
                        if (!IsPartOfBody(overlappedCollider)) otherColliders.Add(overlappedCollider);
                    }
                }
            }
            return otherColliders;
        }

        public List<Collider2D> LineSightCast(Vector2 originPosition, Vector2 direction, float distance, float blockingRadius)
        {
            List<Collider2D> otherColliders = new List<Collider2D>();
            if (this.currentMode != MobPushMode.IgnoreEverything)
            {
                RaycastHit2D[] circleCastHits = Physics2D.CircleCastAll(originPosition, blockingRadius, direction, distance, SIGHT_LAYER_MASK);
                foreach (RaycastHit2D circleCastHit in circleCastHits)
                {
                    if (!IsPartOfBody(circleCastHit.collider)) otherColliders.Add(circleCastHit.collider);
                }
            }
            return otherColliders;
        }

        public List<Collider2D> LinePushCast(Vector2 originPosition, Vector2 direction, float distance)
        {
            List<Collider2D> otherColliders = new List<Collider2D>();
            if (this.currentMode != MobPushMode.IgnoreEverything)
            {
                int layer = this.Layer;
                Vector2 size = this.rootStateNode.CurrentState.Size;
                if (size.magnitude > 0)
                {
                    RaycastHit2D[] boxCastHits = Physics2D.BoxCastAll(originPosition, size, 0, direction, distance, layer);
                    foreach (RaycastHit2D boxCastHit in boxCastHits)
                    {
                        if (!IsPartOfBody(boxCastHit.collider)) otherColliders.Add(boxCastHit.collider);
                    }
                }
            }
            return otherColliders;
        }

        private bool IsPartOfBody(Collider2D collider)
        {
            foreach (PushColliderAnimatorProperty pushColliderProperty in this.animatorBody.GetProperties<PushColliderAnimatorProperty>()) 
            {
                if (pushColliderProperty.Collider == collider)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateCurrentMode()
        {
            MobPushMode newMode = MobPushMode.IgnoreNone;
            for (int i = this.numModeRequests.Length - 1; i >= 0; i--)
            {
                if (this.numModeRequests[i] > 0)
                {
                    newMode = (MobPushMode)i;
                    break;
                }
            }
            if (newMode != this.currentMode)
            {
                this.currentMode = newMode;
                UpdateAllColliders();
            }
        }

        private void UpdateAllColliders(MobState previousState, MobState currentState)
        {
            UpdateAllColliders();
        }

        private void UpdateAllColliders()
        {
            int layer = this.Layer;
            foreach (PushColliderAnimatorProperty pushColliderProperty in this.animatorBody.GetProperties<PushColliderAnimatorProperty>())
            {
                pushColliderProperty.Layer = layer;
            }
        }
    }
}

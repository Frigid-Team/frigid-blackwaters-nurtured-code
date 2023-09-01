using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobDeathState : MobState
    {
        [SerializeField]
        private List<DeathAnimationsOnTerrain> deathAnimationsOnTerrains;

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                return new HashSet<MobStateNode>();
            }
        }

        public override bool AutoEnter
        {
            get
            {
                return this.Owner.RemainingHealth <= 0;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return true;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return true;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return !this.AutoEnter;
            }
        }

        public sealed override MobStatus Status
        {
            get
            {
                return MobStatus.Dead;
            }
        }

        public override void Enter()
        {
            base.Enter();
            this.Owner.StopVelocities.Request();
            this.Owner.StopDealingDamage.Request();
            this.Owner.StopReceivingDamage.Request();
            this.Owner.RequestPushMode(MobPushMode.IgnoreEverything);

            foreach (DeathAnimationsOnTerrain deathAnimationsOnTerrain in this.deathAnimationsOnTerrains)
            {
                TileTerrain currentTerrain = this.Owner.TiledArea.NavigationGrid[this.Owner.IndexPosition].Terrain;
                if (deathAnimationsOnTerrain.Terrains.Contains(currentTerrain))
                {
                    this.OwnerAnimatorBody.Play(deathAnimationsOnTerrain.DeathAnimationName, () => this.OwnerAnimatorBody.Play(deathAnimationsOnTerrain.CorpseAnimationName));
                    break;
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            this.Owner.StopVelocities.Release();
            this.Owner.StopDealingDamage.Release();
            this.Owner.StopReceivingDamage.Release();
            this.Owner.ReleasePushMode(MobPushMode.IgnoreEverything);
        }

        [Serializable]
        private struct DeathAnimationsOnTerrain
        {
            [SerializeField]
            private List<TileTerrain> terrains;
            [SerializeField]
            private string deathAnimationName;
            [SerializeField]
            private string corpseAnimationName;

            public List<TileTerrain> Terrains
            {
                get
                {
                    return this.terrains;
                }
            }

            public string DeathAnimationName
            {
                get
                {
                    return this.deathAnimationName;
                }
            }

            public string CorpseAnimationName
            {
                get
                {
                    return this.corpseAnimationName;
                }
            }
        } 
    }
}

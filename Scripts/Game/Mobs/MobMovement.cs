using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MobMovement
    {
        private Rigidbody2D rigidbody;
        private Mover mover;

        private CountingSemaphore constrained;

        public MobMovement (Rigidbody2D rigidbody, Mover mover, MobStats stats)
        {
            this.rigidbody = rigidbody;
            this.mover = mover;

            this.mover.OnVelocityUpdated += UpdateBodyVelocity;
            this.mover.SpeedBonus += SpeedBonusFromAgility(stats.GetStatValue(MobStat.Agility).Amount);
            stats.GetStatValue(MobStat.Agility).OnAmountChanged += UpdateSpeedBonusFromAgilityChange;

            this.constrained = new CountingSemaphore();
            this.constrained.OnFirstRequest += StopBodyVelocity;
            this.constrained.OnLastRelease += StartBodyVelocity;
            StartBodyVelocity();
        }

        public CountingSemaphore Constrained
        {
            get
            {
                return this.constrained;
            }
        }

        public Vector2 DesiredVelocity
        {
            get
            {
                return this.mover.CalculatedVelocity;
            }
        }

        public Vector2 CurrentVelocity
        {
            get
            {
                return this.rigidbody.velocity;
            }
        }

        public bool TryDoForcedMove(Move move)
        {
            if (this.mover.HighestPriority == (int)MobMovePriority.Count)
            {
                return false;
            }
            this.mover.AddMove(move, (int)MobMovePriority.Count);
            return true;
        }

        public bool TryAbortForcedMove(Move move)
        {
            this.mover.RemoveMove(move);
            return this.mover.HighestPriority != (int)MobMovePriority.Count;
        }

        public void AddMoves(IEnumerable<Move> moves, MobMovePriority priority)
        {
            this.mover.AddMoves(moves, (int)priority);
        } 

        public void AddMove(Move move, MobMovePriority priority)
        {
            this.mover.AddMove(move, (int)priority);
        }

        public void RemoveMoves(IEnumerable<Move> moves)
        {
            this.mover.RemoveMoves(moves);
        }

        public void RemoveMove(Move move)
        {
            this.mover.RemoveMove(move);
        }

        private void UpdateBodyVelocity(Vector2 calculatedVelocity)
        {
            if (!this.constrained)
            {
                this.rigidbody.velocity = calculatedVelocity;
            }
        }

        private void StopBodyVelocity()
        {
            this.rigidbody.velocity = Vector2.zero;
        }

        private void StartBodyVelocity()
        {
            this.rigidbody.velocity = this.mover.CalculatedVelocity;
        }

        private void UpdateSpeedBonusFromAgilityChange(int previousAgility, int currentAgility)
        {
            this.mover.SpeedBonus += SpeedBonusFromAgility(currentAgility) - SpeedBonusFromAgility(previousAgility);
        }

        private float SpeedBonusFromAgility(int agility)
        {
            return agility * 0.1f;
        }
    }
}

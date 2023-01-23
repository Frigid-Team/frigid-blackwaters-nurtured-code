using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class Mover : FrigidMonoBehaviour
    {
        private Dictionary<Move, int> prioritizedMoves;
        private int highestPriority;
        private Vector2 calculatedVelocity;
        private float speedBonus;

        private Action<Vector2> onVelocityUpdated;

        public int HighestPriority
        {
            get
            {
                return this.highestPriority;
            }
        }

        public Vector2 CalculatedVelocity
        {
            get
            {
                return this.calculatedVelocity;
            }
        }

        public float SpeedBonus
        {
            get
            {
                return this.speedBonus;
            }
            set
            {
                this.speedBonus = value;
            }
        }

        public Action<Vector2> OnVelocityUpdated
        {
            get
            {
                return this.onVelocityUpdated;
            }
            set
            {
                this.onVelocityUpdated = value;
            }
        }

        public void AddMoves(IEnumerable<Move> moves, int priority = 0)
        {
            foreach (Move move in moves)
            {
                AddMove(move, priority);
            }
        }

        public void AddMove(Move move, int priority = 0)
        {
            if (!this.prioritizedMoves.ContainsKey(move))
            {
                this.prioritizedMoves.Add(move, priority);
                move.StartMoving(this.transform.position, this.speedBonus);
                UpdateHighestPriority();
                UpdateVelocity();
            }
        }

        public void RemoveMoves(IEnumerable<Move> moves)
        {
            foreach (Move move in moves)
            {
                RemoveMove(move);
            }
        }

        public void RemoveMove(Move move)
        {
            if (this.prioritizedMoves.ContainsKey(move))
            {
                move.StopMoving();
                this.prioritizedMoves.Remove(move);
                UpdateHighestPriority();
                UpdateVelocity();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.prioritizedMoves = new Dictionary<Move, int>();
            this.speedBonus = 0;
            this.highestPriority = int.MinValue;
        }

        protected override void Update()
        {
            base.Update();
            foreach (Move move in this.prioritizedMoves.Keys) move.ContinueMovement(this.transform.position, this.speedBonus);
            UpdateVelocity();
        }

        private void UpdateVelocity()
        {
            Vector2 velocity = Vector2.zero;
            foreach (KeyValuePair<Move, int> prioritizedMove in this.prioritizedMoves)
            {
                if (prioritizedMove.Value != this.highestPriority) continue; 
                velocity += prioritizedMove.Key.Velocity;
            }
            this.calculatedVelocity = velocity;
            this.onVelocityUpdated?.Invoke(this.calculatedVelocity);
        }

        private void UpdateHighestPriority()
        {
            this.highestPriority = int.MinValue;
            foreach (int priority in this.prioritizedMoves.Values)
            {
                this.highestPriority = Mathf.Max(priority, this.highestPriority);
            }
        }
    }
}

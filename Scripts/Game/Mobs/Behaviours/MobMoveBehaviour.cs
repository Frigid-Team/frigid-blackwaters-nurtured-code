using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobMoveBehaviour : MobBehaviour
    {
        [SerializeField]
        private List<Move> moves;
        [SerializeField]
        private MobMovePriority movePriority;

        private int numAddedMoves;
        private int numFinishedMoves;

        public override bool IsFinished
        {
            get
            {
                return this.numFinishedMoves >= this.numAddedMoves;
            }
        }

        public override void Enter()
        {
            base.Enter();
            this.numAddedMoves = 0;
            this.numFinishedMoves = 0;
            foreach (Move move in this.moves)
            {
                if (this.Owner.AddMove(move, this.movePriority, this.Owner.GetIsIgnoringTimeScale(this), () => this.numFinishedMoves++))
                {
                    this.numAddedMoves++;
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            foreach (Move move in this.moves)
            {
                this.Owner.RemoveMove(move);
            }
        }
    }
}

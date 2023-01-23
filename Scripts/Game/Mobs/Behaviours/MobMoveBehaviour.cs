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

        public override bool Finished
        {
            get
            {
                return false;
            }
        }

        public override void Apply()
        {
            base.Apply();
            foreach (Move move in this.moves)
            {
                this.Owner.Movement.AddMove(move, this.movePriority);
            }
        }

        public override void Unapply()
        {
            base.Unapply();
            foreach (Move move in this.moves)
            {
                this.Owner.Movement.RemoveMove(move);
            }
        }
    }
}

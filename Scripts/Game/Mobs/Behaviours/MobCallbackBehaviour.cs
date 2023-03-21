using UnityEngine;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public class MobCallbackBehaviour : MobBehaviour
    {
        [SerializeField]
        private CallbackChannel callbacks;
        [SerializeField]
        private List<MobBehaviour> behavioursOnCallback;

        public override void Enter()
        {
            base.Enter();
            this.callbacks.RegisterListener(OnCallback);
        }

        public override void Exit()
        {
            base.Exit();
            this.callbacks.ClearListener(OnCallback);
        }

        private void OnCallback()
        {
            foreach (MobBehaviour behaviour in this.behavioursOnCallback)
            {
                this.Owner.DoBehaviour(behaviour, this.Owner.GetIsIgnoringTimeScale(this));
            }
        }
    }
}

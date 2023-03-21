using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class PlayerMob : Mob
    {
        private static PlayerMob instance;
        private static Action onExists;
        private static Action onUnexists;

        [Space]
        [SerializeField]
        private List<State> playerStates;

        private State currentPlayerState;
        private Action<bool, MobEquipmentPiece, bool, MobEquipmentPiece> onEquipChange;
        private Action<Timer, Timer> onDashTimerChange;

        public static Action OnExists
        {
            get
            {
                return onExists;
            }
            set
            {
                onExists = value;
            }
        }

        public static Action OnUnexists
        {
            get
            {
                return onUnexists;
            }
            set
            {
                onUnexists = value;
            }
        }

        public Action<bool, MobEquipmentPiece, bool, MobEquipmentPiece> OnEquipChange
        {
            get
            {
                return this.onEquipChange;
            }
            set
            {
                this.onEquipChange = value;
            }
        }

        public Action<Timer, Timer> OnDashTimerChange
        {
            get
            {
                return this.onDashTimerChange;
            }
            set
            {
                this.onDashTimerChange = value;
            }
        }

        public static bool TryGet(out PlayerMob player)
        {
            player = instance;
            return instance != null;
        }

        public bool TryGetEquippedPiece(out MobEquipmentPiece equippedPiece)
        {
            return this.currentPlayerState.EquipPoint.TryGetEquippedPiece(out equippedPiece);
        }

        public Timer GetDashTimer() 
        {
            return this.currentPlayerState.DashTimer;
        }

        protected override void OnActive()
        {
            base.OnActive();
            this.RootStateNode.OnCurrentStateChanged += SwapPlayerStates;
            this.currentPlayerState = GetPlayerStateFromState(this.RootStateNode.CurrentState);
            if (instance == null)
            {
                instance = this;
                onExists?.Invoke();
            }
        }

        protected override void OnInactive()
        {
            base.OnInactive();
            this.RootStateNode.OnCurrentStateChanged -= SwapPlayerStates;
            if (instance == this)
            {
                instance = null;
                onUnexists?.Invoke();
            }
        }

        private void SwapPlayerStates(MobState previousState, MobState currentState)
        {
            State playerState = GetPlayerStateFromState(currentState);
            if (playerState != this.currentPlayerState)
            {
                State formerPlayerState = this.currentPlayerState;
                this.currentPlayerState = playerState;

                formerPlayerState.EquipPoint.OnEquipChange -= HandleEquipChange;
                this.currentPlayerState.EquipPoint.OnEquipChange += HandleEquipChange;

                HandleEquipChange(
                    formerPlayerState.EquipPoint.TryGetEquippedPiece(out MobEquipmentPiece previousEquippedPiece), 
                    previousEquippedPiece, 
                    this.currentPlayerState.EquipPoint.TryGetEquippedPiece(out MobEquipmentPiece currentEquippedPiece),
                    currentEquippedPiece
                    );
                HandleDashTimerChange(formerPlayerState.DashTimer, this.currentPlayerState.DashTimer);
            }
        }

        private State GetPlayerStateFromState(MobState state)
        {
            foreach (State playerState in this.playerStates)
            {
                if (playerState.StateNode.CurrentState == state)
                {
                    return playerState;
                }
            }
            return this.currentPlayerState;
        }

        private void HandleEquipChange(bool hasPrevious, MobEquipmentPiece previousEquippedPiece, bool hasCurrent, MobEquipmentPiece currentEquippedPiece)
        {
            if (hasPrevious != hasCurrent || previousEquippedPiece != currentEquippedPiece)
            {
                this.onEquipChange?.Invoke(hasPrevious, previousEquippedPiece, hasCurrent, currentEquippedPiece);
            } 
        }

        private void HandleDashTimerChange(Timer previousDashTimer, Timer currentDashTimer)
        {
            if (previousDashTimer != currentDashTimer)
            {
                this.onDashTimerChange?.Invoke(previousDashTimer, currentDashTimer);
            }
        }

        [Serializable]
        private class State
        {
            [SerializeField]
            private MobStateNode stateNode;
            [SerializeField]
            private MobEquipPoint equipPoint;
            [SerializeField]
            private Timer dashTimer;

            public MobStateNode StateNode
            {
                get
                {
                    return this.stateNode;
                }
            }

            public Timer DashTimer
            {
                get
                {
                    return this.dashTimer;
                }
            }

            public MobEquipPoint EquipPoint
            {
                get
                {
                    return this.equipPoint;
                }
            }
        }
    }
}

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
        private Action<bool, MobEquipment, bool, MobEquipment> onEquipChange;
        private Action<AbilityResource, AbilityResource> onDashResourceChanged;

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

        public Action<bool, MobEquipment, bool, MobEquipment> OnEquipChange
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

        public Action<AbilityResource, AbilityResource> OnDashResourceChange
        {
            get
            {
                return this.onDashResourceChanged;
            }
            set
            {
                this.onDashResourceChanged = value;
            }
        }

        public static bool TryGet(out PlayerMob player)
        {
            player = instance;
            return instance != null;
        }

        public bool TryGetEquippedEquipment(out MobEquipment equippedEquipment)
        {
            return this.currentPlayerState.EquipPoint.TryGetEquippedEquipment(out equippedEquipment);
        }

        public AbilityResource GetDashResource() 
        {
            return this.currentPlayerState.DashResource;
        }

        protected override void OnActive()
        {
            base.OnActive();
            this.RootStateNode.OnCurrentStateChanged += this.SwapPlayerStates;
            this.currentPlayerState = this.GetPlayerStateFromState(this.RootStateNode.CurrentState);
            this.currentPlayerState.EquipPoint.OnEquipChange += this.HandleEquipChange;
            if (instance == null)
            {
                instance = this;
                onExists?.Invoke();
            }
        }

        protected override void OnInactive()
        {
            base.OnInactive();
            this.RootStateNode.OnCurrentStateChanged -= this.SwapPlayerStates;
            this.currentPlayerState.EquipPoint.OnEquipChange -= this.HandleEquipChange;
            this.currentPlayerState = null;
            if (instance == this)
            {
                instance = null;
                onUnexists?.Invoke();
            }
        }

        private void SwapPlayerStates(MobState previousState, MobState currentState)
        {
            State playerState = this.GetPlayerStateFromState(currentState);
            if (playerState != this.currentPlayerState)
            {
                State formerPlayerState = this.currentPlayerState;
                this.currentPlayerState = playerState;

                formerPlayerState.EquipPoint.OnEquipChange -= this.HandleEquipChange;
                this.currentPlayerState.EquipPoint.OnEquipChange += this.HandleEquipChange;

                this.HandleEquipChange(
                    formerPlayerState.EquipPoint.TryGetEquippedEquipment(out MobEquipment previousEquippedEquipment), 
                    previousEquippedEquipment, 
                    this.currentPlayerState.EquipPoint.TryGetEquippedEquipment(out MobEquipment currentEquippedEquipment),
                    currentEquippedEquipment
                    );
                this.HandleDashResourceChange(formerPlayerState.DashResource, this.currentPlayerState.DashResource);
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

        private void HandleEquipChange(bool hasPrevious, MobEquipment previousEquippedEquipment, bool hasCurrent, MobEquipment currentEquippedEquipment)
        {
            if (hasPrevious != hasCurrent || previousEquippedEquipment != currentEquippedEquipment)
            {
                this.onEquipChange?.Invoke(hasPrevious, previousEquippedEquipment, hasCurrent, currentEquippedEquipment);
            } 
        }

        private void HandleDashResourceChange(AbilityResource previousDashResource, AbilityResource currentDashResource)
        {
            if (previousDashResource != currentDashResource)
            {
                this.onDashResourceChanged?.Invoke(previousDashResource, currentDashResource);
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
            private AbilityResource dashResource;

            public MobStateNode StateNode
            {
                get
                {
                    return this.stateNode;
                }
            }

            public AbilityResource DashResource
            {
                get
                {
                    return this.dashResource;
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

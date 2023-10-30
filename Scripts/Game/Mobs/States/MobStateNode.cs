using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobStateNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobStatusTag> statusTags;
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<AbilityResource> inUseAbilityResources;

        private Mob owner;
        private AnimatorBody ownerAnimatorBody;

        private MobStateNode chosenStateNode;
        private Action<MobState, MobState> onCurrentStateChanged;

        private bool entered;
        private float enterDuration;
        private float enterDurationDelta;

        public MobState CurrentState
        {
            get
            {
                if (this.chosenStateNode != this)
                {
                    return this.chosenStateNode.CurrentState;
                }
                return (MobState)this;
            }
        }

        public Action<MobState, MobState> OnCurrentStateChanged
        {
            get
            {
                return this.onCurrentStateChanged;
            }
            set
            {
                this.onCurrentStateChanged = value;
            }
        }

        public abstract bool AutoEnter
        {
            get;
        }

        public abstract bool AutoExit
        {
            get;
        }

        public abstract bool ShouldEnter
        {
            get;
        }

        public abstract bool ShouldExit
        {
            get;
        }

        public bool CanSpawnAt(TiledArea tiledArea, Vector2 position)
        {
            return this.TryGetChosenStateNodeForSpawn(tiledArea, position, out _);
        }

        public bool CanMoveTo(TiledArea tiledArea, Vector2 position)
        {
            return this.TryGetChosenStateNodeForMove(tiledArea, position, out _);
        }

        public void OwnedBy(Mob owner, AnimatorBody ownerAnimatorBody)
        {
            this.owner = owner;
            this.ownerAnimatorBody = ownerAnimatorBody;

            foreach (MobStateNode childStateNode in this.ChildStateNodes)
            {
                childStateNode.OwnedBy(owner, ownerAnimatorBody);
            }
        }

        public virtual void Spawn() 
        {
            this.entered = false;

            this.Owner.OnRequestedTimeScaleChanged += this.HandleOwnerTimeScaleChange;
            this.HandleOwnerTimeScaleChange();

            HashSet<MobStateNode> spawnStateNodes = this.SpawnStateNodes;
            Debug.Assert(spawnStateNodes.Count > 0, "No spawn state nodes when trying to spawn!");

            this.chosenStateNode = spawnStateNodes.First();

            foreach (MobStateNode childStateNode in this.ChildStateNodes)
            {
                childStateNode.Spawn();
            }

            MobStateNode chosenStateNode;
            if (!this.TryGetChosenStateNodeForSpawn(this.Owner.TiledArea, this.Owner.Position, out chosenStateNode))
            {
                chosenStateNode = spawnStateNodes.First();
            }
            this.SetChosenStateNode(chosenStateNode);
        }

        public virtual void Move()
        {
            HashSet<MobStateNode> moveStateNodes = this.MoveStateNodes;
            Debug.Assert(moveStateNodes.Count > 0, "No move state nodes when trying to move!");

            foreach (MobStateNode childStateNode in this.ChildStateNodes)
            {
                childStateNode.Move();
            }

            MobStateNode chosenStateNode;
            if (!this.TryGetChosenStateNodeForMove(this.Owner.TiledArea, this.Owner.Position, out chosenStateNode))
            {
                chosenStateNode = moveStateNodes.First();
            }
            this.SetChosenStateNode(chosenStateNode);
        }

        public virtual void Enter() 
        {
            this.entered = true;
            foreach (MobStatusTag statusTag in this.statusTags) this.Owner.AddStatusTag(statusTag);
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.AddBehaviour(behaviour, false);
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Request();
            this.enterDuration = 0;
            this.enterDurationDelta = 0;

            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Enter();
            }
            else
            {
                ((MobState)this).EnterSelf();
            }
        }

        public virtual void Exit() 
        {
            this.entered = false;
            foreach (MobStatusTag statusTag in this.statusTags) this.Owner.RemoveStatusTag(statusTag);
            foreach (MobBehaviour behaviour in this.behaviours) this.Owner.RemoveBehaviour(behaviour);
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources) inUseAbilityResource.InUse.Release();

            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Exit();
            }
            else
            {
                ((MobState)this).ExitSelf();
            }
        }

        public virtual void Refresh()
        {
            this.enterDurationDelta = Time.deltaTime * this.Owner.RequestedTimeScale;
            this.enterDuration += this.enterDurationDelta;

            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Refresh();
            }
            else
            {
                ((MobState)this).RefreshSelf();
            }
        }

        protected abstract HashSet<MobStateNode> SpawnStateNodes
        {
            get;
        }

        protected abstract HashSet<MobStateNode> MoveStateNodes
        {
            get;
        }

        protected abstract HashSet<MobStateNode> ChildStateNodes
        {
            get;
        }

        protected MobStateNode ChosenStateNode
        {
            get
            {
                return this.chosenStateNode;
            }
        }

        protected Mob Owner
        {
            get
            {
                return this.owner;
            }
        }

        protected AnimatorBody OwnerAnimatorBody
        {
            get
            {
                return this.ownerAnimatorBody;
            }
        }

        protected bool Entered
        {
            get
            {
                return this.entered;
            }
        }

        protected float EnterDuration
        {
            get
            {
                return this.enterDuration;
            }
        }

        protected float EnterDurationDelta
        {
            get
            {
                return this.enterDurationDelta;
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject()
        {
            return true;
        }
#endif

        private bool TryGetChosenStateNodeForSpawn(TiledArea tiledArea, Vector2 position, out MobStateNode chosenStateNode)
        {
            chosenStateNode = null;
            foreach (MobStateNode spawnStateNode in this.SpawnStateNodes)
            {
                chosenStateNode = spawnStateNode;
                if (chosenStateNode != this)
                {
                    if (chosenStateNode.TryGetChosenStateNodeForSpawn(tiledArea, position, out _))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.Assert(this is MobState, "MobStateNode chooses itself yet is not a state!");
                    MobState state = (MobState)this;
                    if (Mob.CanTraverseAt(tiledArea, position, state.Size, state.TraversableTerrain))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TryGetChosenStateNodeForMove(TiledArea tiledArea, Vector2 position, out MobStateNode chosenStateNode)
        {
            chosenStateNode = null;
            foreach (MobStateNode moveStateNode in this.MoveStateNodes)
            {
                chosenStateNode = moveStateNode;
                if (chosenStateNode != this)
                {
                    if (chosenStateNode.TryGetChosenStateNodeForMove(tiledArea, position, out _))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.Assert(this is MobState, "MobStateNode chooses itself yet is not a state!");
                    MobState state = (MobState)this;
                    if (Mob.CanTraverseAt(tiledArea, position, state.Size, state.TraversableTerrain))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool CanSetChosenStateNode(MobStateNode stateNode)
        {
            MobState newState;
            if (stateNode != this)
            {
                newState = stateNode.CurrentState;
            }
            else
            {
                Debug.Assert(this is MobState, "MobStateNode chooses itself yet is not a state!");
                newState = (MobState)this;
            }
            MobState currentState = this.CurrentState;

            return
                newState.TileSize == currentState.TileSize && newState.TraversableTerrain >= currentState.TraversableTerrain ||
                Mob.CanTraverseAt(this.Owner.TiledArea, this.Owner.Position, newState.Size, newState.TraversableTerrain);
        }

        protected void SetChosenStateNode(MobStateNode stateNode)
        {
            Debug.Assert(stateNode == this || this.ChildStateNodes.Contains(stateNode), "Chosen state node must be itself or a child state node!");
            Debug.Assert(this.CanSetChosenStateNode(stateNode), "MobStateNode " + this.name + " is trying to set a MobStateNode that is unviable.");

            MobState previousState;
            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.OnCurrentStateChanged -= this.CurrentStateChanged;

                if (this.Entered) this.chosenStateNode.Exit();
                previousState = this.chosenStateNode.CurrentState;
            }
            else
            {
                Debug.Assert(this is MobState, "MobStateNode chooses itself yet is not a state!");
                previousState = (MobState)this;
                if (this.Entered) previousState.ExitSelf();
            }

            this.chosenStateNode = stateNode;

            MobState currentState;
            if (this.chosenStateNode != this)
            {
                if (this.Entered) this.chosenStateNode.Enter();
                currentState = this.chosenStateNode.CurrentState;

                this.chosenStateNode.OnCurrentStateChanged += this.CurrentStateChanged;
            }
            else
            {
                Debug.Assert(this is MobState, "MobStateNode chooses itself yet is not a state!");
                currentState = (MobState)this;
                if (this.Entered) currentState.EnterSelf();
            }

            if (previousState != currentState)
            {
                this.CurrentStateChanged(previousState, currentState);
            }
        }

        private void CurrentStateChanged(MobState previousState, MobState currentState)
        {
            this.onCurrentStateChanged?.Invoke(previousState, currentState);
        }

        private void HandleOwnerTimeScaleChange()
        {
            foreach (AbilityResource inUseAbilityResource in this.inUseAbilityResources)
            {
                inUseAbilityResource.LocalTimeScale = this.Owner.RequestedTimeScale;
            }
        }
    }
}

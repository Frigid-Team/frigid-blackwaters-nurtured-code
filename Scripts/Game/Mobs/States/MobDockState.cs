using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public class MobDockState : MobState
    {
        [Space]
        [SerializeField]
        private List<MobStatusTag> statusTagsWhileDocking;
        [SerializeField]
        private List<MobBehaviour> behavioursWhileDocking;
        [Space]
        [SerializeField]
        private string positionInterpPropertyName;
        [SerializeField]
        private List<DockTransition> dockTransitions;
        [SerializeField]
        private Direction dockingDirection;
        [SerializeField]
        private Conditional dockCondition;
        [SerializeField]
        private bool hasDockDisplay;
        [SerializeField]
        [ShowIfBool("hasDockDisplay", true)]
        private MobDockDisplay dockDisplayPrefab;

        private Vector2 dockTryDirection;

        private bool isDocking;
        private Action onDockStarted;
        private Action onDockEnded;

        private bool preparingDock;
        private MobStateNode dockedStateNode;
        private MobStateNode chosenStateNode;

        public Action OnDockStarted
        {
            get
            {
                return this.onDockStarted;
            }
            set
            {
                this.onDockStarted = value;
            }
        }

        public Action OnDockEnded
        {
            get
            {
                return this.onDockEnded;
            }
            set
            {
                this.onDockEnded = value;
            }
        }

        public override HashSet<MobState> InitialStates
        {
            get
            {
                HashSet<MobState> initialStates = new HashSet<MobState>();
                foreach (DockTransition dockTransition in this.dockTransitions)
                {
                    initialStates.UnionWith(dockTransition.FromStateNode.InitialStates);
                    initialStates.UnionWith(dockTransition.ToStateNode.InitialStates);
                }
                return initialStates;
            }
        }

        public override HashSet<MobState> MoveStates
        {
            get
            {
                HashSet<MobState> switchableStates = new HashSet<MobState>();
                foreach (DockTransition dockTransition in this.dockTransitions)
                {
                    switchableStates.UnionWith(dockTransition.FromStateNode.MoveStates);
                    switchableStates.UnionWith(dockTransition.ToStateNode.MoveStates);
                }
                return switchableStates;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                HashSet<MobStateNode> referencedStateNodes = new HashSet<MobStateNode>();
                foreach (DockTransition dockTransition in this.dockTransitions)
                {
                    referencedStateNodes.Add(dockTransition.FromStateNode);
                    referencedStateNodes.Add(dockTransition.ToStateNode);
                }
                return referencedStateNodes;
            }
        }

        public override bool AutoEnter
        {
            get
            {
                return false;
            }
        }

        public override bool AutoExit
        {
            get
            {
                return false;
            }
        }

        public override bool ShouldEnter
        {
            get
            {
                return this.chosenStateNode == this || this.chosenStateNode.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.chosenStateNode == this || this.chosenStateNode.ShouldExit;
            }
        }

        public override bool MovePositionSafe
        {
            get
            {
                return (this.chosenStateNode == this && this.preparingDock) || (this.chosenStateNode != this && this.chosenStateNode.CurrentState.MovePositionSafe);
            }
        }

        public override bool MoveTiledAreaSafe
        {
            get
            {
                return this.chosenStateNode != this && this.chosenStateNode.CurrentState.MoveTiledAreaSafe;
            }
        }

        public sealed override MobStatus Status
        {
            get
            {
                return MobStatus.Acting;
            }
        }

        public override void Init()
        {
            base.Init();
            this.isDocking = false;
            this.preparingDock = false;
            foreach (DockTransition dockTransition in this.dockTransitions)
            {
                if (dockTransition.FromStateNode.InitialStates.Contains(this.CurrentState))
                {
                    this.dockedStateNode = dockTransition.FromStateNode;
                    break;
                }
                if (dockTransition.ToStateNode.InitialStates.Contains(this.CurrentState))
                {
                    this.dockedStateNode = dockTransition.ToStateNode;
                    break;
                }
            }
            this.chosenStateNode = this.dockedStateNode;
            if (this.hasDockDisplay) CreateInstance<MobDockDisplay>(this.dockDisplayPrefab).Spawn(this.Owner, this);
        }

        public override void Move()
        {
            base.Move();
            foreach (DockTransition dockTransition in this.dockTransitions)
            {
                if (dockTransition.FromStateNode.MoveStates.Contains(this.CurrentState))
                {
                    this.dockedStateNode = dockTransition.FromStateNode;
                    break;
                }
                if (dockTransition.ToStateNode.MoveStates.Contains(this.CurrentState))
                {
                    this.dockedStateNode = dockTransition.ToStateNode;
                    break;
                }
            }
            this.chosenStateNode = this.dockedStateNode;
        }

        public override void Enter()
        {
            this.SetChosenStateNode(this.dockedStateNode);
            base.Enter();
            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.OnCurrentStateChanged += this.SetCurrentStateFromChosenStateNode;
                this.chosenStateNode.Enter();
            }
            else
            {
                this.EnterDock();
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Exit();
                this.chosenStateNode.OnCurrentStateChanged -= this.SetCurrentStateFromChosenStateNode;
            }
            else
            {
                this.ExitDock();
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Refresh();
                this.dockTryDirection = this.dockingDirection.Retrieve(Vector2.zero, this.EnterDuration, this.EnterDurationDelta);
            }
            this.CheckTransitions();
        }

        public bool TryGetDockingPosition(out Vector2 dockPosition)
        {
            return this.TryDock(out dockPosition, out _);
        }

        private void CheckTransitions()
        {
            if (this.chosenStateNode != this)
            {
                if (this.TryDock(out _, out _) && this.Owner.IsActingAndNotStunned && this.dockCondition.Evaluate(this.EnterDuration, this.EnterDurationDelta))
                {
                    this.SetChosenStateNode(this);
                }
            }
            else
            {
                if (!this.isDocking)
                {
                    this.SetChosenStateNode(this.dockedStateNode);
                }
            }
        }

        private void EnterDock()
        {
            if (this.TryDock(out Vector2 dockPosition, out DockTransition dockTransition))
            {
                this.preparingDock = true;
                if (this.Owner.CanMoveTo(dockPosition))
                {
                    Vector2 originalPosition = this.Owner.Position;
                    this.Owner.MoveTo(dockPosition);
                    this.OwnerAnimatorBody.Direction = dockPosition - originalPosition;

                    if (this.CanSetChosenStateNode(dockTransition.ToStateNode))
                    {
                        if (!this.OwnerAnimatorBody.TryFindReferencedPropertyIn<LocalPositionInterpAnimatorProperty>(dockTransition.AnimationName, this.positionInterpPropertyName, out LocalPositionInterpAnimatorProperty interpProperty))
                        {
                            Debug.LogWarning("MobDockState " + this.name + " could not find the interp property.");

                            this.Owner.MoveTo(originalPosition);
                            this.preparingDock = false;

                            this.CheckTransitions();
                            return;
                        }

                        interpProperty.StartLocalPosition = originalPosition - dockPosition;
                        interpProperty.FinishLocalPosition = Vector2.zero;

                        this.isDocking = true;
                        this.onDockStarted?.Invoke();

                        this.Owner.StopVelocities.Request();
                        this.Owner.RequestPushMode(MobPushMode.IgnoreEverything);

                        this.dockedStateNode = dockTransition.ToStateNode;

                        foreach (MobStatusTag statusTagWhileDocking in this.statusTagsWhileDocking) this.Owner.AddStatusTag(statusTagWhileDocking);
                        foreach (MobBehaviour behaviourWhileDocking in this.behavioursWhileDocking) this.Owner.AddBehaviour(behaviourWhileDocking, false);

                        this.OwnerAnimatorBody.Play(
                            dockTransition.AnimationName,
                            () =>
                            {
                                this.isDocking = false;
                                this.onDockEnded?.Invoke();

                                this.Owner.ReleasePushMode(MobPushMode.IgnoreEverything);
                                this.Owner.StopVelocities.Release();

                                foreach (MobStatusTag statusTagWhileDocking in this.statusTagsWhileDocking) this.Owner.RemoveStatusTag(statusTagWhileDocking);
                                foreach (MobBehaviour behaviourWhileDocking in this.behavioursWhileDocking) this.Owner.RemoveBehaviour(behaviourWhileDocking);

                                this.CheckTransitions();
                            }
                            );

                        this.preparingDock = false;
                        return;
                    }

                    this.Owner.MoveTo(originalPosition);
                }
                this.preparingDock = false;
            }
            this.CheckTransitions();
        }

        private void ExitDock()
        {
            if (this.isDocking)
            {
                this.OwnerAnimatorBody.Stop();

                this.isDocking = false;
                this.onDockEnded?.Invoke();

                this.Owner.ReleasePushMode(MobPushMode.IgnoreEverything);
                this.Owner.StopVelocities.Release();

                foreach (MobStatusTag statusTagWhileDocking in this.statusTagsWhileDocking) this.Owner.RemoveStatusTag(statusTagWhileDocking);
                foreach (MobBehaviour behaviourWhileDocking in this.behavioursWhileDocking) this.Owner.RemoveBehaviour(behaviourWhileDocking);
            }
        }

        private bool TryDock(out Vector2 dockPosition, out DockTransition dockTransition)
        {
            if (this.Entered)
            {
                TiledArea tiledArea = this.Owner.TiledArea;
                Vector2Int currentIndexPosition = this.Owner.IndexPosition;

                foreach (DockTransition potentialDockTransition in this.dockTransitions)
                {
                    MobStateNode fromStateNode = potentialDockTransition.FromStateNode;
                    MobStateNode toStateNode = potentialDockTransition.ToStateNode;

                    if (fromStateNode != this.dockedStateNode || fromStateNode == toStateNode) continue;

                    Vector2Int substitudeIndexPosition = currentIndexPosition + new Vector2Int(Mathf.RoundToInt(this.dockTryDirection.x) * toStateNode.CurrentState.TileSize.x, -Mathf.RoundToInt(this.dockTryDirection.y) * toStateNode.CurrentState.TileSize.y);

                    if (!AreaTiling.RectIndexPositionWithinBounds(substitudeIndexPosition, tiledArea.MainAreaDimensions, toStateNode.CurrentState.TileSize)) continue;

                    dockPosition = AreaTiling.RectPositionFromIndexPosition(substitudeIndexPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions, toStateNode.CurrentState.TileSize);
                    if (substitudeIndexPosition != currentIndexPosition && Mob.CanTraverseAt(this.Owner.TiledArea, dockPosition, toStateNode.CurrentState.Size, toStateNode.CurrentState.TraversableTerrain))
                    {
                        dockTransition = potentialDockTransition;
                        return true;
                    }
                }
            }
            dockPosition = default;
            dockTransition = default;
            return false;
        }

        private bool CanSetChosenStateNode(MobStateNode chosenStateNode)
        {
            if (chosenStateNode == this) return this.CanSetCurrentState(this);
            else return this.CanSetCurrentState(chosenStateNode.CurrentState);
        }

        private void SetChosenStateNode(MobStateNode chosenStateNode)
        {
            if (this.CanSetChosenStateNode(chosenStateNode) && chosenStateNode != this.chosenStateNode)
            {
                if (this.Entered)
                {
                    if (this.chosenStateNode != this)
                    {
                        this.chosenStateNode.Exit();
                        this.chosenStateNode.OnCurrentStateChanged -= this.SetCurrentStateFromChosenStateNode;
                    }
                    else
                    {
                        this.ExitDock();
                    }
                }

                this.chosenStateNode = chosenStateNode;
                this.SetCurrentStateFromChosenStateNode();

                if (this.Entered)
                {
                    if (this.chosenStateNode != this)
                    {
                        this.chosenStateNode.OnCurrentStateChanged += this.SetCurrentStateFromChosenStateNode;
                        this.chosenStateNode.Enter();
                    }
                    else
                    {
                        this.EnterDock();
                    }
                }
            }
        }

        private void SetCurrentStateFromChosenStateNode(MobState previousState, MobState currentState)
        {
            this.SetCurrentStateFromChosenStateNode();
        }

        private void SetCurrentStateFromChosenStateNode()
        {
            if (this.chosenStateNode == this) this.SetCurrentState(this);
            else this.SetCurrentState(this.chosenStateNode.CurrentState);
        }

        [Serializable]
        private struct DockTransition
        {
            [SerializeField]
            private string animationName;
            [SerializeField]
            private MobStateNode fromStateNode;
            [SerializeField]
            private MobStateNode toStateNode;

            public string AnimationName
            {
                get
                {
                    return this.animationName;
                }
            }

            public MobStateNode FromStateNode
            {
                get
                {
                    return this.fromStateNode;
                }
            }

            public MobStateNode ToStateNode
            {
                get
                {
                    return this.toStateNode;
                }
            }
        }
    }
}

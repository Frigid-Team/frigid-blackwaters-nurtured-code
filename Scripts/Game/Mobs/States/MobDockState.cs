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
                return this.ChosenStateNode == this || this.ChosenStateNode.ShouldEnter;
            }
        }

        public override bool ShouldExit
        {
            get
            {
                return this.ChosenStateNode == this || this.ChosenStateNode.ShouldExit;
            }
        }

        public override bool MovePositionSafe
        {
            get
            {
                return (this.ChosenStateNode == this && this.preparingDock) || (this.ChosenStateNode != this && this.ChosenStateNode.CurrentState.MovePositionSafe);
            }
        }

        public override bool MoveTiledAreaSafe
        {
            get
            {
                return this.ChosenStateNode != this && this.ChosenStateNode.CurrentState.MoveTiledAreaSafe;
            }
        }

        public sealed override MobStatus Status
        {
            get
            {
                return MobStatus.Acting;
            }
        }

        public override void Spawn()
        {
            base.Spawn();
            this.isDocking = false;
            this.preparingDock = false;
            this.dockedStateNode = this.ChosenStateNode;
            if (this.hasDockDisplay) CreateInstance<MobDockDisplay>(this.dockDisplayPrefab).Spawn(this.Owner, this);
        }

        public override void Move()
        {
            base.Move();
            this.dockedStateNode = this.ChosenStateNode;
        }

        public override void Enter()
        {
            if (this.ChosenStateNode == this)
            {
                Debug.Assert(this.CanSetChosenStateNode(this.dockedStateNode), "This should always succeed as MobDockState prevents moving during docking.");
                this.SetChosenStateNode(this.dockedStateNode);
            }
            base.Enter();
        }

        public override void Refresh()
        {
            base.Refresh();
            if (this.ChosenStateNode != this)
            {
                this.dockTryDirection = this.dockingDirection.Retrieve(Vector2.zero, this.EnterDuration, this.EnterDurationDelta);

                if (this.CanSetChosenStateNode(this) && this.TryDock(out _, out _) &&
                    this.Owner.IsActingAndNotStunned && this.dockCondition.Evaluate(this.EnterDuration, this.EnterDurationDelta))
                {
                    this.SetChosenStateNode(this);
                }
            }
        }

        public override void EnterSelf()
        {
            base.EnterSelf();
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

                            Debug.Assert(this.CanSetChosenStateNode(this.dockedStateNode), "This should always succeed as MobDockState prevents moving during docking.");
                            this.SetChosenStateNode(this.dockedStateNode);
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

                                Debug.Assert(this.CanSetChosenStateNode(this.dockedStateNode), "This should always succeed as MobDockState prevents moving during docking.");
                                this.SetChosenStateNode(this.dockedStateNode);
                            }
                            );

                        this.preparingDock = false;
                        return;
                    }

                    this.Owner.MoveTo(originalPosition);
                }
                this.preparingDock = false;
            }
            Debug.Assert(this.CanSetChosenStateNode(this.dockedStateNode), "This should always succeed as MobDockState prevents moving during docking.");
            this.SetChosenStateNode(this.dockedStateNode);
        }

        public override void ExitSelf()
        {
            base.ExitSelf();
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

        public bool TryGetDockingPosition(out Vector2 dockPosition)
        {
            return this.TryDock(out dockPosition, out _);
        }

        protected override HashSet<MobStateNode> SpawnStateNodes
        {
            get
            {
                return this.ChildStateNodes;
            }
        }

        protected override HashSet<MobStateNode> MoveStateNodes
        {
            get
            {
                return this.ChildStateNodes;
            }
        }

        protected override HashSet<MobStateNode> ChildStateNodes
        {
            get
            {
                HashSet<MobStateNode> childStateNodes = new HashSet<MobStateNode>();
                foreach (DockTransition dockTransition in this.dockTransitions)
                {
                    childStateNodes.Add(dockTransition.FromStateNode);
                    childStateNodes.Add(dockTransition.ToStateNode);
                }
                return childStateNodes;
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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class MobDockState : MobState
    {
        [Space]
        [SerializeField]
        private List<DockPreset> dockPresets;
        [SerializeField]
        private List<DockSequence> dockSequences;
        [SerializeField]
        private List<MobBehaviour> behavioursWhileDocking;
        [SerializeField]
        private Direction dockingDirection;
        [SerializeField]
        private ConditionalClause dockConditions;
        [SerializeField]
        private MobDockDisplay dockDisplayPrefab;

        private Vector2 currDockDirection;
        private bool isDocking;
        private bool moveFlag;
        private AnimatorBody currFromAnimatorBody;
        private AnimatorBody currToAnimatorBody;
        private MobStateNode dockedStateNode;
        private MobStateNode chosenStateNode;

        public override HashSet<MobState> InitialStates
        {
            get
            {
                HashSet<MobState> initialStates = new HashSet<MobState>();
                foreach (DockPreset dockPreset in this.dockPresets)
                {
                    initialStates.UnionWith(dockPreset.StateNode.InitialStates);
                }
                return initialStates;
            }
        }

        public override HashSet<MobState> SwitchableStates
        {
            get
            {
                HashSet<MobState> switchableStates = new HashSet<MobState>();
                foreach (DockPreset dockPreset in this.dockPresets)
                {
                    switchableStates.UnionWith(dockPreset.StateNode.SwitchableStates);
                }
                return switchableStates;
            }
        }

        public override HashSet<MobStateNode> ReferencedStateNodes
        {
            get
            {
                HashSet<MobStateNode> referencedStateNodes = new HashSet<MobStateNode>();
                foreach (DockPreset dockPreset in this.dockPresets)
                {
                    referencedStateNodes.Add(dockPreset.StateNode);
                }
                return referencedStateNodes;
            }
        }

        public override Vector2 DisplayPosition
        {
            get
            {
                if (!this.isDocking) return base.DisplayPosition;
                return this.currFromAnimatorBody.transform.position;
            }
        }

        public override bool AutoEnter
        {
            get
            {
                if (this.chosenStateNode == this)
                {
                    return false;
                }
                else
                {
                    return this.chosenStateNode.AutoEnter;
                }
            }
        }

        public override bool AutoExit
        {
            get
            {
                if (this.chosenStateNode == this)
                {
                    return false;
                }
                else
                {
                    return this.chosenStateNode.AutoExit;
                }
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

        public override bool CanSetPosition
        {
            get
            {
                return (this.chosenStateNode == this && this.moveFlag) || (this.chosenStateNode != this && this.chosenStateNode.CurrentState.CanSetPosition);
            }
        }

        public override bool CanSetTiledArea
        {
            get
            {
                return this.chosenStateNode != this && this.chosenStateNode.CurrentState.CanSetTiledArea;
            }
        }

        public override bool Dead
        {
            get
            {
                return false;
            }
        }

        public override bool Waiting
        {
            get
            {
                return false;
            }
        }

        public override void Init()
        {
            base.Init();
            this.isDocking = false;
            this.moveFlag = false;
            foreach (DockPreset dockPreset in this.dockPresets)
            {
                if (dockPreset.StateNode.InitialStates.Contains(this.CurrentState))
                {
                    this.dockedStateNode = dockPreset.StateNode;
                    this.chosenStateNode = this.dockedStateNode;
                    break;
                }
            }
            FrigidInstancing.CreateInstance<MobDockDisplay>(this.dockDisplayPrefab).Spawn(this.Owner, this);
        }

        public override void Switch()
        {
            base.Switch();
            foreach (DockPreset dockPreset in this.dockPresets)
            {
                if (dockPreset.StateNode.SwitchableStates.Contains(this.CurrentState))
                {
                    this.dockedStateNode = dockPreset.StateNode;
                    this.chosenStateNode = this.dockedStateNode;
                    break;
                }
            }
        }

        public override void Enter()
        {
            SetChosenStateNode(this.dockedStateNode);
            base.Enter();
            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
                this.chosenStateNode.Enter();
            }
            else
            {
                EnterDock();
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Exit();
                this.chosenStateNode.OnCurrentStateChanged -= SetCurrentStateFromChosenStateNode;
            }
            else
            {
                ExitDock();
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            if (this.chosenStateNode != this)
            {
                this.currDockDirection = this.dockingDirection.Calculate(this.currDockDirection, this.EnterDuration, this.EnterDurationDelta);
                this.chosenStateNode.Refresh();
            }
            CheckTransitions();
        }

        public bool TryGetPositionOfNextDockState(out Vector2 dockPosition, out MobState nextDockState)
        {
            bool dockTry = TryDock(out dockPosition, out DockSequence dockSequence);
            if (dockTry)
            {
                nextDockState = this.dockPresets[dockSequence.ToDockPresetIndex].StateNode.CurrentState;
                return true;
            }
            else
            {
                nextDockState = null;
                return false;
            }
        }

        private void CheckTransitions()
        {
            if (this.chosenStateNode != this)
            {
                this.chosenStateNode.Refresh();
                if (TryDock(out _, out _) && this.Owner.CanAct && this.dockConditions.Evaluate(this.EnterDuration, this.EnterDurationDelta))
                {
                    SetChosenStateNode(this);
                }
            }
            else
            {
                if (!this.isDocking)
                {
                    SetChosenStateNode(this.dockedStateNode);
                }
            }
        }

        private void EnterDock()
        {
            if (TryDock(out Vector2 dockPosition, out DockSequence dockSequence))
            {
                DockPreset fromDockPreset = this.dockPresets[dockSequence.FromDockPresetIndex];
                DockPreset toDockPreset = this.dockPresets[dockSequence.ToDockPresetIndex];

                this.moveFlag = true;
                this.Owner.StopVelocities.Request();
                this.Owner.RequestPushMode(MobPushMode.IgnoreMobsAndTerrain);

                if (this.Owner.CanMoveTo(dockPosition))
                {
                    Vector2 originalPosition = this.Owner.Position;
                    this.Owner.MoveTo(dockPosition);
                    this.OwnerAnimatorBody.Direction = dockPosition - originalPosition;

                    if (CanSetChosenStateNode(toDockPreset.StateNode))
                    {
                        Vector2 fromStartPosition = originalPosition;
                        Vector2 fromEndPosition = originalPosition;
                        Vector2 toStartPosition = dockPosition;
                        Vector2 toEndPosition = dockPosition;
                        if (fromDockPreset.DoMoveOver)
                        {
                            fromEndPosition = toEndPosition;
                        }
                        if (toDockPreset.DoMoveOver)
                        {
                            toStartPosition = fromStartPosition;
                        }

                        if (this.OwnerAnimatorBody.TryFindPropertyIn<SubAnimatorBodyAnimatorProperty>(dockSequence.AnimationName, fromDockPreset.AnimatorBodyPropertyName, out SubAnimatorBodyAnimatorProperty fromAnimatorBodyProperty) &&
                            this.OwnerAnimatorBody.TryFindPropertyIn<SubAnimatorBodyAnimatorProperty>(dockSequence.AnimationName, toDockPreset.AnimatorBodyPropertyName, out SubAnimatorBodyAnimatorProperty toAnimatorBodyProperty))
                        {
                            this.isDocking = true;
                            this.dockedStateNode = toDockPreset.StateNode;

                            this.currFromAnimatorBody = fromAnimatorBodyProperty.SubBody;
                            this.currToAnimatorBody = toAnimatorBodyProperty.SubBody;
                            this.currFromAnimatorBody.transform.position = fromStartPosition;
                            this.currToAnimatorBody.transform.position = toStartPosition;

                            foreach (MobBehaviour behaviourWhileDocking in this.behavioursWhileDocking) this.Owner.AddBehaviour(behaviourWhileDocking, false);

                            IEnumerator<FrigidCoroutine.Delay> MoveOver()
                            {
                                while (true)
                                {
                                    float progress01 = Mathf.Clamp01(this.OwnerAnimatorBody.ElapsedDuration / this.OwnerAnimatorBody.TotalDuration);
                                    this.currFromAnimatorBody.transform.position = fromStartPosition + (fromEndPosition - fromStartPosition) * progress01;
                                    this.currToAnimatorBody.transform.position = toStartPosition + (toEndPosition - toStartPosition) * progress01;
                                    yield return null;
                                }
                            }
                            FrigidCoroutine moveOverRoutine = FrigidCoroutine.Run(MoveOver());

                            this.OwnerAnimatorBody.Play(
                                dockSequence.AnimationName,
                                () =>
                                {
                                    this.isDocking = false;

                                    FrigidCoroutine.Kill(moveOverRoutine);
                                    this.currFromAnimatorBody.transform.localPosition = Vector2.zero;
                                    this.currToAnimatorBody.transform.localPosition = Vector2.zero;
                                    this.currFromAnimatorBody = null;
                                    this.currToAnimatorBody = null;

                                    foreach (MobBehaviour behaviourWhileDocking in this.behavioursWhileDocking) this.Owner.RemoveBehaviour(behaviourWhileDocking);

                                    CheckTransitions();
                                }
                                );

                            this.moveFlag = false;
                            this.Owner.StopVelocities.Release();
                            this.Owner.ReleasePushMode(MobPushMode.IgnoreMobsAndTerrain);
                            return;
                        }
                        else
                        {
                            Debug.LogWarning("Dock State " + this.name + " seems to have an invalid animator body property name.");
                        }
                    }

                    this.Owner.MoveTo(originalPosition);
                }

                this.moveFlag = false;
                this.Owner.StopVelocities.Release();
                this.Owner.ReleasePushMode(MobPushMode.IgnoreMobsAndTerrain);
            }
            CheckTransitions();
        }

        private void ExitDock()
        {
            if (this.isDocking)
            {
                this.isDocking = false;

                this.currFromAnimatorBody.transform.localPosition = Vector2.zero;
                this.currToAnimatorBody.transform.localPosition = Vector2.zero;
                this.currFromAnimatorBody = null;
                this.currToAnimatorBody = null;

                foreach (MobBehaviour behaviourWhileDocking in this.behavioursWhileDocking) this.Owner.RemoveBehaviour(behaviourWhileDocking);
            }
        }

        private bool TryDock(out Vector2 dockPosition, out DockSequence validDockSequence)
        {
            if (this.Entered)
            {
                TiledArea tiledArea = this.Owner.TiledArea;
                Vector2Int currentPositionIndices = this.Owner.PositionIndices;

                foreach (DockSequence dockSequence in this.dockSequences)
                {
                    MobStateNode fromStateNode = this.dockPresets[dockSequence.FromDockPresetIndex].StateNode;
                    MobStateNode toStateNode = this.dockPresets[dockSequence.ToDockPresetIndex].StateNode;

                    if (fromStateNode != this.dockedStateNode || fromStateNode == toStateNode) continue;

                    Vector2Int substitutePositionIndices =
                        currentPositionIndices +
                        new Vector2Int(Mathf.RoundToInt(this.currDockDirection.x) * toStateNode.CurrentState.TileSize.x, -Mathf.RoundToInt(this.currDockDirection.y) * toStateNode.CurrentState.TileSize.y);

                    if (!TilePositioning.RectIndicesWithinBounds(substitutePositionIndices, tiledArea.MainAreaDimensions, toStateNode.CurrentState.TileSize)) continue;

                    dockPosition = TilePositioning.RectPositionFromIndices(substitutePositionIndices, tiledArea.CenterPosition, tiledArea.MainAreaDimensions, toStateNode.CurrentState.TileSize);
                    if (substitutePositionIndices != currentPositionIndices && Mob.CanFitAt(this.Owner.TiledArea, dockPosition, toStateNode.CurrentState.Size, toStateNode.CurrentState.TraversableTerrain))
                    {
                        validDockSequence = dockSequence;
                        return true;
                    }
                }
            }
            dockPosition = Vector2.zero;
            validDockSequence = new DockSequence();
            return false;
        }

        private bool CanSetChosenStateNode(MobStateNode chosenStateNode)
        {
            if (chosenStateNode == this) return CanSetCurrentState(this);
            else return CanSetCurrentState(chosenStateNode.CurrentState);
        }

        private void SetChosenStateNode(MobStateNode chosenStateNode)
        {
            if (CanSetChosenStateNode(chosenStateNode) && chosenStateNode != this.chosenStateNode)
            {
                if (this.Entered)
                {
                    if (this.chosenStateNode != this)
                    {
                        this.chosenStateNode.Exit();
                        this.chosenStateNode.OnCurrentStateChanged -= SetCurrentStateFromChosenStateNode;
                    }
                    else
                    {
                        ExitDock();
                    }
                }

                this.chosenStateNode = chosenStateNode;
                SetCurrentStateFromChosenStateNode();

                if (this.Entered)
                {
                    if (this.chosenStateNode != this)
                    {
                        this.chosenStateNode.OnCurrentStateChanged += SetCurrentStateFromChosenStateNode;
                        this.chosenStateNode.Enter();
                    }
                    else
                    {
                        EnterDock();
                    }
                }
            }
        }

        private void SetCurrentStateFromChosenStateNode(MobState previousState, MobState currentState)
        {
            SetCurrentStateFromChosenStateNode();
        }

        private void SetCurrentStateFromChosenStateNode()
        {
            if (this.chosenStateNode == this) SetCurrentState(this);
            else SetCurrentState(this.chosenStateNode.CurrentState);
        }

        [Serializable]
        private struct DockPreset
        {
            [SerializeField]
            private MobStateNode stateNode;
            [SerializeField]
            private string animatorBodyPropertyName;
            [SerializeField]
            private bool doMoveOver;

            public MobStateNode StateNode
            {
                get
                {
                    return this.stateNode;
                }
            }

            public string AnimatorBodyPropertyName
            {
                get
                {
                    return this.animatorBodyPropertyName;
                }
            }

            public bool DoMoveOver
            {
                get
                {
                    return this.doMoveOver;
                }
            }
        }

        [Serializable]
        private struct DockSequence
        {
            [SerializeField]
            private string animationName;
            [SerializeField]
            private int fromDockPresetIndex;
            [SerializeField]
            private int toDockPresetIndex;

            public string AnimationName
            {
                get
                {
                    return this.animationName;
                }
            }

            public int FromDockPresetIndex
            {
                get
                {
                    return this.fromDockPresetIndex;
                }
            }

            public int ToDockPresetIndex
            {
                get
                {
                    return this.toDockPresetIndex;
                }
            }
        }
    }
}

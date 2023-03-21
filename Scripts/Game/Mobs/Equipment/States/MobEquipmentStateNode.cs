using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public abstract class MobEquipmentStateNode : FrigidMonoBehaviour
    {
        [SerializeField]
        private List<MobBehaviour> behaviours;
        [SerializeField]
        private List<Timer> timers;

        private MobEquipmentPiece equipmentPiece;
        private AnimatorBody equipmentAnimatorBody;

        private MobEquipmentState currentState;
        private Action<MobEquipmentState, MobEquipmentState> onCurrentStateChanged;

        private bool entered;
        private float enterDuration;
        private float enterDurationDelta;

        public abstract MobEquipmentState InitialState
        {
            get;
        }

        public MobEquipmentState CurrentState
        {
            get
            {
                return this.currentState;
            }
            protected set
            {
                if (value == this.currentState) return;
                MobEquipmentState previousState = this.currentState;
                this.currentState = value;
                this.onCurrentStateChanged?.Invoke(previousState, this.currentState);
            }
        }

        public Action<MobEquipmentState, MobEquipmentState> OnCurrentStateChanged
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

        public abstract HashSet<MobEquipmentStateNode> ReferencedStateNodes
        {
            get;
        }

        public virtual bool AutoEnter
        {
            get
            {
                return false;
            }
        }

        public virtual bool AutoExit
        {
            get
            {
                return false;
            }
        }

        public void Link(MobEquipmentPiece equipmentPiece, AnimatorBody equipmentAnimatorBody)
        {
            this.equipmentPiece = equipmentPiece;
            this.equipmentAnimatorBody = equipmentAnimatorBody;
        }

        public virtual void Init()
        {
            this.currentState = this.InitialState;
        }

        public virtual void Equipped() 
        {
            this.EquipmentPiece.EquipPoint.Equipper.OnRequestedTimeScaleChanged += HandleEquipperTimeScaleChange;
            HandleEquipperTimeScaleChange();
        }

        public virtual void Unequipped() 
        {
            this.EquipmentPiece.EquipPoint.Equipper.OnRequestedTimeScaleChanged -= HandleEquipperTimeScaleChange;
        }

        public virtual void Enter() 
        { 
            foreach (MobBehaviour behaviour in this.behaviours) this.EquipmentPiece.EquipPoint.Equipper.AddBehaviour(behaviour, false);
            foreach (Timer timer in this.timers) timer.InUse.Request();
            this.entered = true;
            this.enterDuration = 0;
        }

        public virtual void Exit() 
        {
            foreach (MobBehaviour behaviour in this.behaviours) this.EquipmentPiece.EquipPoint.Equipper.RemoveBehaviour(behaviour);
            foreach (Timer timer in this.timers) timer.InUse.Release();
            this.entered = false;
        }

        public virtual void Refresh() 
        {
            this.enterDurationDelta = Time.deltaTime * this.EquipmentPiece.EquipPoint.Equipper.RequestedTimeScale;
            this.enterDuration += this.enterDurationDelta;
        }

        protected MobEquipmentPiece EquipmentPiece
        {
            get
            {
                return this.equipmentPiece;
            }
        }

        protected AnimatorBody EquipmentAnimatorBody
        {
            get
            {
                return this.equipmentAnimatorBody;
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

        private void HandleEquipperTimeScaleChange()
        {
            foreach (Timer timer in this.timers)
            {
                timer.LocalTimeScale = this.EquipmentPiece.EquipPoint.Equipper.RequestedTimeScale;
            }
        }
    }
}

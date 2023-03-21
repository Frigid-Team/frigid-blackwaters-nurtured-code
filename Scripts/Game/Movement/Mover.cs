using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class Mover : FrigidMonoBehaviourWithPhysics
    {
        [SerializeField]
        private Rigidbody2D rigidBody;

        private ControlCounter stopVelocities;
        private Dictionary<Move, (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion)> moveMap;
        private int highestPriority;
        private Vector2 calculatedVelocity;
        private float speedBonus;
        private float timeScale;
        private Action onTimeScaleChanged;

        public ControlCounter StopVelocities
        {
            get
            {
                return this.stopVelocities;
            }
        }

        public int HighestPriority
        {
            get
            {
                return this.highestPriority;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this.rigidBody.position;
            }
        }

        public Vector2 CalculatedVelocity
        {
            get
            {
                return this.calculatedVelocity;
            }
        }

        public Vector2 ActualVelocity
        {
            get
            {
                return this.rigidBody.velocity;
            }
        }

        public float SpeedBonus
        {
            get
            {
                return this.speedBonus;
            }
            set
            {
                this.speedBonus = value;
            }
        }

        public float TimeScale
        {
            get
            {
                return this.timeScale;
            }
            set
            {
                if (this.timeScale != value)
                {
                    this.timeScale = value;
                    this.onTimeScaleChanged?.Invoke();
                    UpdateVelocity();
                }
            }
        }

        public Action OnTimeScaleChanged
        {
            get
            {
                return this.onTimeScaleChanged;
            }
            set
            {
                this.onTimeScaleChanged = value;
            }
        }

        public bool DoMove(Move move, int priority, bool ignoreTimeScale, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            return AddMove(
                move,
                priority,
                ignoreTimeScale,
                () => 
                { 
                    RemoveMove(move); 
                    onFinished?.Invoke(); 
                },
                onBeginMotion,
                onEndMotion
                );
        }

        public bool AddMove(Move move, int priority, bool ignoreTimeScale, Action onFinished = null, Action onBeginMotion = null, Action onEndMotion = null)
        {
            if (!this.moveMap.ContainsKey(move))
            {
                this.moveMap.Add(move, (priority, ignoreTimeScale, onFinished, false, onBeginMotion, onEndMotion, false));
                move.Assign(this);
                move.StartMoving();
                if (move.IsFinished)
                {
                    (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion) moveMappingValue = this.moveMap[move];
                    moveMappingValue.currentlyFinished = true;
                    this.moveMap[move] = moveMappingValue;
                    onFinished?.Invoke();
                }
                else
                {
                    if (move.IsInMotion)
                    {
                        (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion) moveMappingValue = this.moveMap[move];
                        moveMappingValue.currentlyInMotion = true;
                        this.moveMap[move] = moveMappingValue;
                        onBeginMotion?.Invoke();
                    }
                }
                UpdateHighestPriority();
                UpdateVelocity();
                return true;
            }
            return false;
        }

        public bool RemoveMove(Move move)
        {
            if (this.moveMap.ContainsKey(move))
            {
                move.StopMoving();
                move.Unassign();
                this.moveMap.Remove(move);
                UpdateHighestPriority();
                UpdateVelocity();
                return true;
            }
            return false;
        }

        public int GetPriority(Move move)
        {
            if (this.moveMap.TryGetValue(move, out (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion) moveMappingValue))
            {
                return moveMappingValue.priority;
            }
            return -1;
        }

        public bool GetIsIgnoringTimeScale(Move move)
        {
            if (this.moveMap.TryGetValue(move, out (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion) moveMappingValue))
            {
                return moveMappingValue.ignoreTimeScale;
            }
            return false;
        }

        protected override void Awake()
        {
            base.Awake();
            this.stopVelocities = new ControlCounter();
            this.stopVelocities.OnFirstRequest += () => { this.rigidBody.velocity = Vector2.zero; };
            this.stopVelocities.OnLastRelease += () => { this.rigidBody.velocity = this.calculatedVelocity; };
            this.moveMap = new Dictionary<Move, (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion)>();
            this.speedBonus = 0;
            this.timeScale = 1f;
            this.highestPriority = int.MinValue;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Dictionary<Move, (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion)> moveMapCopy = 
                new Dictionary<Move, (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion)>(this.moveMap);
            foreach (KeyValuePair<Move, (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion)> moveMapping in moveMapCopy)
            {
                Move move = moveMapping.Key;
                (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion) moveMappingValue = moveMapping.Value;

                if (!this.moveMap.ContainsKey(move)) continue;

                move.ContinueMovement();
                if (move.IsFinished && !moveMappingValue.currentlyFinished)
                {
                    moveMappingValue.currentlyFinished = true;
                    this.moveMap[move] = moveMappingValue;

                    if (moveMappingValue.currentlyInMotion)
                    {
                        moveMappingValue.onEndMotion?.Invoke();
                    }
                    moveMappingValue.onFinished?.Invoke();
                }
                
                if (moveMappingValue.currentlyInMotion)
                {
                    if (!move.IsInMotion)
                    {
                        moveMappingValue.currentlyInMotion = false;
                        this.moveMap[move] = moveMappingValue;

                        moveMappingValue.onEndMotion?.Invoke();
                    }
                }
                else
                {
                    if (move.IsInMotion)
                    {
                        moveMappingValue.currentlyInMotion = true;
                        this.moveMap[move] = moveMappingValue;

                        moveMappingValue.onStartMotion?.Invoke();
                    }
                }
            }
            
            UpdateVelocity();
        }

        private void UpdateVelocity()
        {
            Vector2 velocity = Vector2.zero;
            foreach (KeyValuePair<Move, (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion)> moveMapping in this.moveMap)
            {
                Move move = moveMapping.Key;
                (int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion) moveMappingValue = moveMapping.Value;
                if (moveMappingValue.priority < this.highestPriority || moveMappingValue.currentlyFinished) continue;
                velocity += move.Velocity * (moveMappingValue.ignoreTimeScale ? 1f : this.TimeScale);
            }
            this.calculatedVelocity = velocity;
            if (!this.stopVelocities)
            {
                this.rigidBody.velocity = this.calculatedVelocity;
            }
        }

        private void UpdateHighestPriority()
        {
            this.highestPriority = int.MinValue;
            foreach ((int priority, bool ignoreTimeScale, Action onFinished, bool currentlyFinished, Action onStartMotion, Action onEndMotion, bool currentlyInMotion) moveMappingValue in this.moveMap.Values)
            {
                this.highestPriority = Mathf.Max(moveMappingValue.priority, this.highestPriority);
            }
        }
    }
}

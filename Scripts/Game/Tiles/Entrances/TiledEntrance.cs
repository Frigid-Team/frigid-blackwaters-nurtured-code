using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledEntrance : FrigidMonoBehaviourWithPhysics
    {
        [SerializeField]
        private TiledAreaTransition transition;
        [SerializeField]
        private TiledEntryIndicator entryIndicatorPrefab;
        [SerializeField]
        private Transform arrowPoint;
        [SerializeField]
        private FloatSerializedReference entryTime;
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private string openAnimationName;
        [SerializeField]
        private string openedAnimationName;
        [SerializeField]
        private string closeAnimationName;
        [SerializeField]
        private string closedAnimationName;
        [SerializeField]
        private Transform entryPoint;
        [SerializeField]
        private Vector2Int localEntryIndexDirection;

        private TiledArea containedArea;
        private TileTerrain entranceTerrain;
        private TiledEntrance connectedEntrance;
 
        private float entryDuration;
        private FrigidCoroutine checkPlayerEntryRoutine;
        private TiledEntryIndicator spawnedEntryIndicator;
        private ControlCounter locked;

        public TiledAreaTransition Transition
        {
            get
            {
                return this.transition;
            }
        }

        public TiledArea ContainedArea
        {
            get
            {
                return this.containedArea;
            }
        }

        public TiledEntrance ConnectedEntrance
        {
            get
            {
                return this.connectedEntrance;
            }
        }

        public Vector2Int LocalEntryIndexDirection
        {
            get
            {
                return this.localEntryIndexDirection;
            }
        }

        public Vector2Int EntryIndexDirection
        {
            get
            {
                return this.localEntryIndexDirection.RotateAround(this.transform.rotation.eulerAngles.z);
            }
        }

        public Vector2 LocalEntryPosition
        {
            get
            {
                return this.entryPoint.localPosition;
            }
        }

        public Vector2 EntryPosition
        {
            get
            {
                return this.entryPoint.position;
            }
        }

        public ControlCounter Locked
        {
            get
            {
                return this.locked;
            }
        }

        public void Preview()
        {
            this.animatorBody.Preview(this.openedAnimationName, 0, Vector2.zero);
        }

        public void Spawn(TileTerrain entranceTerrain, TiledArea containedArea)
        {
            this.containedArea = containedArea;
            this.entranceTerrain = entranceTerrain;

            this.animatorBody.Play(this.openedAnimationName);

            this.spawnedEntryIndicator = CreateInstance<TiledEntryIndicator>(
                this.entryIndicatorPrefab,
                this.arrowPoint.position,
                Quaternion.Euler(0, 0, this.EntryIndexDirection.CartesianAngle() - 90),
                this.containedArea.ContentsTransform
                );
            this.spawnedEntryIndicator.ShowEntryProgress(0);

            this.locked = new ControlCounter();
            this.locked.OnFirstRequest += this.Close;
            this.locked.OnLastRelease += this.Open;
        }

        public void ConnectTo(TiledEntrance connectedEntrance)
        {
            this.connectedEntrance = connectedEntrance;
            connectedEntrance.connectedEntrance = this;
        }

        protected override void Awake()
        {
            base.Awake();
            if (!WallTiling.IsValidWallIndexDirection(this.localEntryIndexDirection))
            {
                Debug.LogWarning("TiledEntrance " + this.name + " has a local entry direction that is not a valid wall direction.");
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (this.locked)
            {
                return;
            }

            if (collision.attachedRigidbody.TryGetComponent<Mob>(out Mob mob))
            {
                // Currently only have functionality for the player to leave and enter entrances
                if (PlayerMob.TryGet(out PlayerMob player) && player == mob)
                {
                    this.StartCheckingPlayerEntry();
                }
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            base.OnTriggerExit2D(collision);

            if (this.locked)
            {
                return;
            }

            if (collision.attachedRigidbody.TryGetComponent<Mob>(out Mob mob))
            {
                // Currently only have functionality for the player to leave and enter entrances
                if (PlayerMob.TryGet(out PlayerMob player) && player == mob)
                {
                    this.StopCheckingPlayerEntry();
                }
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void StartCheckingPlayerEntry()
        {
            this.StopCheckingPlayerEntry();
            this.checkPlayerEntryRoutine = FrigidCoroutine.Run(this.CheckPlayerEntryLoop(), this.gameObject);
        }

        private void StopCheckingPlayerEntry()
        {
            FrigidCoroutine.Kill(this.checkPlayerEntryRoutine);
            this.entryDuration = 0;
            this.spawnedEntryIndicator.ShowEntryProgress(0);
        }

        private IEnumerator<FrigidCoroutine.Delay> CheckPlayerEntryLoop()
        {
            if (!PlayerMob.TryGet(out PlayerMob player))
            {
                yield break;
            }

            while (true)
            {
                const float PlayerAngleEntryTolerance = 2.5f;
                if (player.TraversableTerrain.Includes(this.entranceTerrain) && 
                    CharacterInput.CurrentMovementVector.magnitude > 0 && Vector2.Angle(CharacterInput.CurrentMovementVector, this.EntryIndexDirection) <= PlayerAngleEntryTolerance)
                {
                    this.entryDuration += Time.deltaTime;
                    if (this.entryDuration >= this.entryTime.ImmutableValue && player.CanMoveTo(this.connectedEntrance.EntryPosition, false) && 
                        !this.containedArea.IsTransitioning && !this.connectedEntrance.containedArea.IsTransitioning)
                    {
                        this.containedArea.TransitionAway(this.transition, this.EntryPosition);
                        this.connectedEntrance.containedArea.TransitionTo(this.connectedEntrance.transition, this.connectedEntrance.EntryPosition);
                        player.MoveTo(this.connectedEntrance.EntryPosition, false);
                        break;
                    }
                }
                else
                {
                    this.entryDuration = 0;
                }
                this.spawnedEntryIndicator.transform.position = this.arrowPoint.position;
                this.spawnedEntryIndicator.ShowEntryProgress(this.entryDuration / this.entryTime.ImmutableValue);
                yield return null;
            }
        }

        private void Open()
        {
            this.animatorBody.Play(this.openAnimationName, () => this.animatorBody.Play(this.openedAnimationName));
        }

        private void Close()
        {
            this.StopCheckingPlayerEntry();
            this.animatorBody.Play(this.closeAnimationName, () => this.animatorBody.Play(this.closedAnimationName));
        }
    }
}

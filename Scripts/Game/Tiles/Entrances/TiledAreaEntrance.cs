using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledAreaEntrance : FrigidMonoBehaviourWithPhysics
    {
        private const float PLAYER_ANGLE_ENTRY_TOLERANCE = 2.5f;

        [SerializeField]
        private TiledAreaTransition transition;
        [SerializeField]
        private TiledAreaEntryArrow entryArrowPrefab;
        [SerializeField]
        private Transform arrowPoint;
        [SerializeField]
        private FloatSerializedReference entryTime;
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private List<TerrainAppearance> terrainAppearances;
        [SerializeField]
        private Transform entryPoint;

        private TiledArea containedArea;
        private Vector2Int entryDirection;
        private TileTerrain entranceTerrain;
        private EntranceAnimationSet chosenAnimations;
        private TiledAreaEntrance connectedEntrance;
 
        private float entryTimer;
        private FrigidCoroutine checkPlayerEntryRoutine;
        private TiledAreaEntryArrow spawnedEntryArrow;
        private ControlCounter locked;

        private Action<TiledAreaTransition, Vector2> onEntered;
        private Action<TiledAreaTransition, Vector2> onExited;

        public TiledArea ContainedArea
        {
            get
            {
                return this.containedArea;
            }
        }

        public TiledAreaEntrance ConnectedEntrance
        {
            get
            {
                return this.connectedEntrance;
            }
            set
            {
                this.connectedEntrance = value;
            }
        }

        public Action<TiledAreaTransition, Vector2> OnEntered
        {
            get
            {
                return this.onEntered;
            }
            set
            {
                this.onEntered = value;
            }
        }

        public Action<TiledAreaTransition, Vector2> OnExited
        {
            get
            {
                return this.onExited;
            }
            set
            {
                this.onExited = value;
            }
        }

        public Vector2Int EntryDirection
        {
            get
            {
                return this.entryDirection;
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

        public void Populate(Vector2Int entryDirection, TileTerrain entranceTerrain, TiledArea containedArea)
        {
            this.containedArea = containedArea;
            this.entryDirection = entryDirection;
            this.entranceTerrain = entranceTerrain;

            TerrainAppearance? appropriateAppearance = null;
            foreach (TerrainAppearance terrainAppearance in this.terrainAppearances)
            {
                if (terrainAppearance.AssociatedTerrains.Contains(this.entranceTerrain))
                {
                    appropriateAppearance = terrainAppearance;
                    break;
                }
            }
            if (!appropriateAppearance.HasValue)
            {
                Debug.LogError(this.name + " entrance does not have an appearance for terrain " + entranceTerrain.ToString() + ".");
                return;
            }

            this.chosenAnimations = appropriateAppearance.Value.EntranceAnimations[UnityEngine.Random.Range(0, appropriateAppearance.Value.EntranceAnimations.Length)];

            this.animatorBody.Play(this.chosenAnimations.OpenedAnimationName);

            this.spawnedEntryArrow = FrigidInstancing.CreateInstance<TiledAreaEntryArrow>(
                this.entryArrowPrefab,
                this.arrowPoint.position,
                Quaternion.Euler(0, 0, this.entryDirection.CartesianAngle() * Mathf.Rad2Deg - 90),
                containedArea.ContentsTransform
                );
            this.spawnedEntryArrow.ShowEntryProgress(0);

            this.locked = new ControlCounter();
            this.locked.OnFirstRequest += Close;
            this.locked.OnLastRelease += Open;
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
                    StartCheckingPlayerEntry();
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
                    StopCheckingPlayerEntry();
                }
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void StartCheckingPlayerEntry()
        {
            StopCheckingPlayerEntry();
            this.checkPlayerEntryRoutine = FrigidCoroutine.Run(CheckPlayerEntryLoop(), this.gameObject);
        }

        private void StopCheckingPlayerEntry()
        {
            FrigidCoroutine.Kill(this.checkPlayerEntryRoutine);
            this.entryTimer = 0;
            this.spawnedEntryArrow.ShowEntryProgress(0);
        }

        private IEnumerator<FrigidCoroutine.Delay> CheckPlayerEntryLoop()
        {
            if (!PlayerMob.TryGet(out PlayerMob player))
            {
                yield break;
            }

            while (true)
            {
                if (player.TraversableTerrain.Includes(this.entranceTerrain) && 
                    CharacterInput.CurrentMovementVector.magnitude > 0 && Vector2.Angle(CharacterInput.CurrentMovementVector, this.entryDirection) <= PLAYER_ANGLE_ENTRY_TOLERANCE)
                {
                    this.entryTimer += Time.deltaTime;
                    if (this.entryTimer >= this.entryTime.ImmutableValue && player.CanMoveTo(this.connectedEntrance.EntryPosition))
                    {
                        this.onExited?.Invoke(this.transition, this.EntryPosition);
                        this.connectedEntrance.onEntered?.Invoke(this.connectedEntrance.transition, this.connectedEntrance.EntryPosition);
                        player.MoveTo(this.connectedEntrance.EntryPosition);
                        break;
                    }
                }
                else
                {
                    this.entryTimer = 0;
                }
                this.spawnedEntryArrow.transform.position = this.arrowPoint.position;
                this.spawnedEntryArrow.ShowEntryProgress(this.entryTimer / this.entryTime.ImmutableValue);
                yield return null;
            }
        }

        private void Open()
        {
            this.animatorBody.Play(
                this.chosenAnimations.OpenAnimationName, 
                () => this.animatorBody.Play(this.chosenAnimations.OpenedAnimationName)
                );
        }

        private void Close()
        {
            StopCheckingPlayerEntry();
            this.animatorBody.Play(
                this.chosenAnimations.CloseAnimationName,
                () => this.animatorBody.Play(this.chosenAnimations.ClosedAnimationName)
                );
        }

        [Serializable]
        private struct TerrainAppearance
        {
            [SerializeField]
            private List<TileTerrain> associatedTerrains;
            [SerializeField]
            private EntranceAnimationSet[] entranceAnimations;

            public List<TileTerrain> AssociatedTerrains
            {
                get
                {
                    return this.associatedTerrains;
                }
            }

            public EntranceAnimationSet[] EntranceAnimations
            {
                get
                {
                    return this.entranceAnimations;
                }
            }
        }

        [Serializable] 
        private struct EntranceAnimationSet
        {
            [SerializeField]
            private string openAnimationName;
            [SerializeField]
            private string openedAnimationName;

            [SerializeField]
            private string closeAnimationName;
            [SerializeField]
            private string closedAnimationName;

            public string OpenAnimationName
            {
                get
                {
                    return this.openAnimationName;
                }
            }

            public string OpenedAnimationName
            {
                get
                {
                    return this.openedAnimationName;
                }
            }

            public string CloseAnimationName
            {
                get
                {
                    return this.closeAnimationName;
                }
            }

            public string ClosedAnimationName
            {
                get
                {
                    return this.closedAnimationName;
                }
            }
        }
    }
}

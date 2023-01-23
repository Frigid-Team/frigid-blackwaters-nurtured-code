using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledAreaEntrance : FrigidMonoBehaviour
    {
        private const float PLAYER_ANGLE_ENTRY_TOLERANCE = 2.5f;

        [SerializeField]
        private TiledAreaTransition transition;
        [SerializeField]
        private TiledAreaEntryArrow entryArrowPrefab;
        [SerializeField]
        private Transform arrowSpawnPosition;
        [SerializeField]
        private FloatSerializedReference entryTime;
        [SerializeField]
        private AnimatorBody animatorBody;
        [SerializeField]
        private List<TerrainAppearance> terrainAppearances;

        private Vector2Int entryDirection;
        private TileTerrain entranceTerrain;
        private EntranceAnimationSet chosenAnimations;
        private List<TiledAreaEntrance> connectedEntrances;
        private Vector2 positionInFront;
 
        private float entryTimer;
        private FrigidCoroutine checkPlayerEntryRoutine;
        private TiledAreaEntryArrow spawnedEntryArrow;
        private CountingSemaphore locked;

        private Action<TiledAreaTransition, Vector2> onEntered;
        private Action<TiledAreaTransition, Vector2> onExited;

        public List<TiledAreaEntrance> ConnectedEntrances
        {
            get
            {
                return this.connectedEntrances;
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

        public Vector2 PositionInFront
        {
            get
            {
                return this.positionInFront;
            }
        }

        public CountingSemaphore Locked
        {
            get
            {
                return this.locked;
            }
        }

        public void Populate(Vector2Int entryDirection, TileTerrain entranceTerrain)
        {
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

            this.animatorBody.PlayByName(this.chosenAnimations.OpenedAnimationName);

            this.positionInFront = (Vector2)this.transform.position + new Vector2(-this.entryDirection.x, -this.entryDirection.y) * GameConstants.UNIT_WORLD_SIZE;

            this.spawnedEntryArrow = FrigidInstancing.CreateInstance<TiledAreaEntryArrow>(
                this.entryArrowPrefab,
                this.arrowSpawnPosition.position,
                Quaternion.Euler(0, 0, this.entryDirection.CartesianAngle() * Mathf.Rad2Deg - 90),
                this.transform
                );
            this.spawnedEntryArrow.ShowEntryProgress(0);

            this.locked.OnFirstRequest += Close;
            this.locked.OnLastRelease += Open;
        }

        public void AddConnectedEntrance(TiledAreaEntrance tiledAreaEntrance)
        {
            this.connectedEntrances.Add(tiledAreaEntrance);
        }

        protected override void Awake()
        {
            base.Awake();
            this.locked = new CountingSemaphore();
            this.connectedEntrances = new List<TiledAreaEntrance>();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            // Right now, we really only have plans to have a door have 0 or 1 connections (spawn doors and decorative doors have 0
            // connections, normal doors have 1...). We could make a door connect to multiple doors, but that begs of the question of
            // how we would allow the player to choose what door to exit out of. But the world building technically allows a door to
            // connect to multiple other doors! We just cancel entering behaviour for doors that don't only have 1 connection
            if (this.locked || this.connectedEntrances.Count != 1)
            {
                return;
            }

            /*
            if (collision.attachedRigidbody.TryGetComponent<Mob_Legacy>(out Mob_Legacy mob))
            {
                if (!mob.TraversableTerrain.Includes(this.entranceTerrain))
                {
                    return;
                }

                // Currently only have functionality for the player to leave and enter entrances
                if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob) && recentPlayerMob == mob)
                {
                    StartCheckingPlayerEntry(recentPlayerMob);
                }
            }
            */
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            base.OnTriggerExit2D(collision);

            if (this.locked || this.connectedEntrances.Count != 1)
            {
                return;
            }

            /*
            if (collision.attachedRigidbody.TryGetComponent<Mob_Legacy>(out Mob_Legacy mob))
            {
                if (!mob.TraversableTerrain.Includes(this.entranceTerrain))
                {
                    return;
                }

                // Currently only have functionality for the player to leave and enter entrances
                if (Mob_Legacy.GetMobsInGroup(MobGroup_Legacy.Players).TryGetRecentlyPresentMob(out Mob_Legacy recentPlayerMob) && recentPlayerMob == mob)
                {
                    StopCheckingPlayerEntry();
                }
            }
            */
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif

        private void StartCheckingPlayerEntry(Mob playerMob)
        {
            StopCheckingPlayerEntry();
            this.checkPlayerEntryRoutine = FrigidCoroutine.Run(CheckPlayerEntryLoop(playerMob), this.gameObject);
        }

        private void StopCheckingPlayerEntry()
        {
            FrigidCoroutine.Kill(this.checkPlayerEntryRoutine);
            this.entryTimer = 0;
            this.spawnedEntryArrow.ShowEntryProgress(0);
        }

        private IEnumerator<FrigidCoroutine.Delay> CheckPlayerEntryLoop(Mob playerMob)
        {
            while (true)
            {
                if (CharacterInput.CurrentMovementVector.magnitude > 0 && Vector2.Angle(CharacterInput.CurrentMovementVector, this.entryDirection) <= PLAYER_ANGLE_ENTRY_TOLERANCE)
                {
                    this.entryTimer += Time.deltaTime;
                    if (this.entryTimer >= this.entryTime.ImmutableValue)
                    {
                        TiledAreaEntrance nextEntrance = this.connectedEntrances[0];

                        this.onExited?.Invoke(this.transition, this.positionInFront);
                        nextEntrance.onEntered?.Invoke(nextEntrance.transition, nextEntrance.positionInFront);

                        playerMob.AbsolutePosition = nextEntrance.positionInFront;
                        break;
                    }
                }
                else
                {
                    this.entryTimer = 0;
                }
                this.spawnedEntryArrow.ShowEntryProgress(this.entryTimer / this.entryTime.ImmutableValue);
                yield return null;
            }
        }

        private void Open()
        {
            this.animatorBody.PlayByName(
                this.chosenAnimations.OpenAnimationName, 
                () => this.animatorBody.PlayByName(this.chosenAnimations.OpenedAnimationName)
                );
        }

        private void Close()
        {
            StopCheckingPlayerEntry();
            this.animatorBody.PlayByName(
                this.chosenAnimations.CloseAnimationName,
                () => this.animatorBody.PlayByName(this.chosenAnimations.ClosedAnimationName)
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

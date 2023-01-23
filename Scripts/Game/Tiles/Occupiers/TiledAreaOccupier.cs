using System;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TiledAreaOccupier : FrigidMonoBehaviour 
    {
        private bool hasCurrentTiledArea;
        private TiledArea currentTiledArea;
        private Action onCurrentTiledAreaOpened;
        private Action onCurrentTiledAreaClosed;
        private Action onCurrentTiledAreaTransitionStarted;
        private Action onCurrentTiledAreaTransitionFinished;
        private Action<bool, TiledArea, bool, TiledArea> onCurrentTiledAreaChanged;

        public Action OnCurrentTiledAreaOpened
        {
            get
            {
                return this.onCurrentTiledAreaOpened;
            }
            set
            {
                this.onCurrentTiledAreaOpened = value;
            }
        }

        public Action OnCurrentTiledAreaClosed
        {
            get
            {
                return this.onCurrentTiledAreaClosed;
            }
            set
            {
                this.onCurrentTiledAreaClosed = value;
            }
        }

        public Action OnCurrentTiledAreaTransitionStarted
        {
            get
            {
                return this.onCurrentTiledAreaTransitionStarted;
            }
            set
            {
                this.onCurrentTiledAreaTransitionStarted = value;
            }
        }

        public Action OnCurrentTiledAreaTransitionFinished
        {
            get
            {
                return this.onCurrentTiledAreaTransitionFinished;
            }
            set
            {
                this.onCurrentTiledAreaTransitionFinished = value;
            }
        }

        public Action<bool, TiledArea, bool, TiledArea> OnCurrentTiledAreaChanged
        {
            get
            {
                return this.onCurrentTiledAreaChanged;
            }
            set
            {
                this.onCurrentTiledAreaChanged = value;
            }
        }

        public bool TryGetCurrentTiledArea(out TiledArea currentTiledArea)
        {
            currentTiledArea = this.currentTiledArea;
            return this.hasCurrentTiledArea;
        }

        public void OccupyTiledAreaAt(Vector2 newPosition)
        {
            if (!this.hasCurrentTiledArea || !TilePositioning.TileAbsolutePositionWithinBounds(newPosition, this.currentTiledArea.AbsoluteCenterPosition, this.currentTiledArea.WallAreaDimensions))
            {
                bool prevOpened = false;
                bool prevTransitioning = false;

                if (this.hasCurrentTiledArea)
                {
                    prevOpened = this.currentTiledArea.IsOpened;
                    prevTransitioning = this.currentTiledArea.IsTransitioning;

                    this.currentTiledArea.OnOpened -= CurrentTiledAreaOpened;
                    this.currentTiledArea.OnClosed -= CurrentTiledAreaClosed;
                    this.currentTiledArea.OnTransitionStarted -= CurrentTiledAreaTransitionStarted;
                    this.currentTiledArea.OnTransitionFinished -= CurrentTiledAreaTransitionFinished;
                }

                UpdateCurrentTiledArea(newPosition);

                this.currentTiledArea.OnOpened += CurrentTiledAreaOpened;
                this.currentTiledArea.OnClosed += CurrentTiledAreaClosed;
                this.currentTiledArea.OnTransitionStarted += CurrentTiledAreaTransitionStarted;
                this.currentTiledArea.OnTransitionFinished += CurrentTiledAreaTransitionFinished;

                if (prevOpened != this.currentTiledArea.IsOpened)
                {
                    if (this.currentTiledArea.IsOpened) CurrentTiledAreaOpened();
                    else CurrentTiledAreaClosed();
                }

                if (prevTransitioning != this.currentTiledArea.IsTransitioning)
                {
                    if (this.currentTiledArea.IsTransitioning) CurrentTiledAreaTransitionStarted();
                    else CurrentTiledAreaTransitionFinished();
                }

                this.transform.SetParent(this.currentTiledArea.ContentsTransform);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.hasCurrentTiledArea = false;
        }

        private void CurrentTiledAreaOpened()
        {
            this.onCurrentTiledAreaOpened?.Invoke();
        }

        private void CurrentTiledAreaClosed()
        {
            this.onCurrentTiledAreaClosed?.Invoke();
        }

        private void CurrentTiledAreaTransitionStarted()
        {
            this.onCurrentTiledAreaTransitionStarted?.Invoke();
        }

        private void CurrentTiledAreaTransitionFinished()
        {
            this.onCurrentTiledAreaTransitionFinished?.Invoke();
        }

        private void UpdateCurrentTiledArea(Vector2 absolutePosition)
        {
            TiledArea oldTiledArea = this.currentTiledArea;
            bool hadOldTiledArea = this.hasCurrentTiledArea;
            this.hasCurrentTiledArea = TiledArea.TryGetTiledAreaAtPosition(absolutePosition, out this.currentTiledArea);
            this.onCurrentTiledAreaChanged?.Invoke(hadOldTiledArea, oldTiledArea, this.hasCurrentTiledArea, this.currentTiledArea);
        }
    }
}

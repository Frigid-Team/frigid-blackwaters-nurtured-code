using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledWorldMapTeleportCell : TiledWorldMapCell
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private ColorSerializedReference failColor;
        [SerializeField]
        private TweenOptionSetSerializedReference failTween;

        private Color originalColor;
        private FrigidCoroutine failRoutine;

        public override void FillCell(TiledLevel level, TiledArea area, bool isRevealed, float worldToMapScalingFactor, Action onMapActionPerformed)
        {
            base.FillCell(level, area, isRevealed, worldToMapScalingFactor, onMapActionPerformed);
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(
                () =>
                {
                    if (this.TeleportToArea(area))
                    {
                        onMapActionPerformed?.Invoke();
                    }
                    else
                    {
                        FrigidCoroutine.Kill(this.failRoutine);
                        this.button.image.color = this.originalColor;
                        Color failColor = this.failColor.MutableValue;
                        this.failRoutine = FrigidCoroutine.Run(
                            this.failTween.MutableValue.MakeRoutine(true, (float value) => { this.button.image.color = failColor + (this.originalColor - failColor) * value; }), 
                            this.gameObject
                            );
                    }
                }
                );
        }

        protected override void Awake()
        {
            base.Awake();
            this.originalColor = this.button.image.color;
            this.failRoutine = null;
        }

        private bool TeleportToArea(TiledArea targetArea)
        {
            if (PlayerMob.TryGet(out PlayerMob player))
            {
                HashSet<TiledArea> visited = new HashSet<TiledArea>();
                Queue<(TiledArea area, TiledEntrance startingEntrance, TiledEntrance destinationEntrance)> queue = new Queue<(TiledArea area, TiledEntrance entrance, TiledEntrance destinationEntrance)>();
                foreach (TiledEntrance startingEntrance in player.TiledArea.ContainingEntrances)
                {
                    queue.Enqueue((startingEntrance.ConnectedEntrance.ContainedArea, startingEntrance, startingEntrance.ConnectedEntrance));
                }
                visited.Add(player.TiledArea);
                while (queue.Count > 0)
                {
                    (TiledArea area, TiledEntrance startingEntrance, TiledEntrance destinationEntrance) = queue.Dequeue();
                    visited.Add(area);
                    if (area == targetArea)
                    {
                        if (player.CanMoveTo(destinationEntrance.EntryPosition, false) && startingEntrance.ContainedArea != destinationEntrance.ContainedArea &&
                            !startingEntrance.Locked && !destinationEntrance.Locked &&
                            !startingEntrance.ContainedArea.IsTransitioning && !destinationEntrance.ContainedArea.IsTransitioning)
                        {
                            startingEntrance.ContainedArea.TransitionAway(startingEntrance.Transition, player.Position);
                            destinationEntrance.ContainedArea.TransitionTo(destinationEntrance.Transition, destinationEntrance.EntryPosition);
                            player.MoveTo(destinationEntrance.EntryPosition, false);
                            return true;
                        }
                        return false;
                    }
                    foreach (TiledEntrance containingEntrance in area.ContainingEntrances)
                    {
                        if (!visited.Contains(containingEntrance.ConnectedEntrance.ContainedArea) && TiledWorldExplorer.ExploredAreas.Contains(containingEntrance.ConnectedEntrance.ContainedArea))
                        {
                            queue.Enqueue((containingEntrance.ConnectedEntrance.ContainedArea, startingEntrance, containingEntrance.ConnectedEntrance));
                        }
                    }
                }
            }
            return false;
        }
    }
}

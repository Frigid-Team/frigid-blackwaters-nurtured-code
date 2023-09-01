using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "ExpeditionProgress", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.EXPEDITIONS + "ExpeditionProgress")]
    public class ExpeditionProgress : FrigidScriptableObject
    {
        [SerializeField]
        private List<Expedition> startingExpeditions;

        private Expedition currentExpedition;
        private bool departed;
        private List<Expedition> completedExpeditions;
        private List<Expedition> availableExpeditions;
        private Action onUpdated;

        public List<Expedition> CompletedExpeditions
        {
            get
            {
                return this.completedExpeditions;
            }
        }

        public List<Expedition> AvailableExpeditions
        {
            get
            {
                return this.availableExpeditions;
            }
        }

        public Action OnUpdated
        {
            get
            {
                return this.onUpdated;
            }
            set
            {
                this.onUpdated = value;
            }
        }

        public bool TryGetSelectedExpedition(out Expedition selectedExpedition)
        {
            if (this.currentExpedition == null || this.departed)
            {
                selectedExpedition = null;
                return false;
            }
            selectedExpedition = this.currentExpedition;
            return true;
        }

        public bool TryGetActiveExpedition(out Expedition activeExpedition)
        {
            if (this.currentExpedition == null || !this.departed)
            {
                activeExpedition = null;
                return false;
            }
            activeExpedition = this.currentExpedition;
            return true;
        }

        public bool SelectExpedition(Expedition selectedExpedition)
        {
            if (selectedExpedition == null || this.departed)
            {
                return false;
            }
            this.currentExpedition = selectedExpedition;
            this.onUpdated?.Invoke();
            return true;
        }

        public bool DepartOnExpedition()
        {
            if (this.currentExpedition == null || this.departed || !this.currentExpedition.Depart(this.CompleteExpedition))
            {
                return false;
            }
            this.departed = true;
            this.onUpdated?.Invoke();
            return true;
        }

        public bool ReturnFromExpedition()
        {
            if (this.currentExpedition == null || !this.departed || !this.currentExpedition.Return())
            {
                return false;
            }

            this.currentExpedition = null;
            this.departed = false;
            this.onUpdated?.Invoke();
            return true;
        }

        protected override void OnBegin()
        {
            base.OnBegin();
            // Start from blank slate, hook in loading from save later.
            this.currentExpedition = null;
            this.departed = false;
            this.completedExpeditions = new List<Expedition>();
            this.availableExpeditions = new List<Expedition>(this.startingExpeditions);
        }

        private void CompleteExpedition()
        {
            if (this.currentExpedition == null || this.completedExpeditions.Contains(this.currentExpedition))
            {
                return;
            }
            this.completedExpeditions.Add(this.currentExpedition);
            Stamps.TotalStamps += this.currentExpedition.NumberAwardedStamps;

            foreach (Expedition unlockedExpedition in this.currentExpedition.ExpeditionsUnlockedOnComplete)
            {
                if (!this.availableExpeditions.Contains(unlockedExpedition))
                {
                    this.availableExpeditions.Add(unlockedExpedition);
                }
            }
            this.onUpdated?.Invoke();
        }
    }
}
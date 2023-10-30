using UnityEngine;
using System.Collections.Generic;
using System;

namespace FrigidBlackwaters.Game
{
    public abstract class Item : FrigidMonoBehaviour
    {
        private ItemStorage storage;
        private bool inUse;
        private Action onInUseChanged;
        private bool storageChangeable;
        private Action onStorageChangeableChanged;

        private HashSet<ItemEffectNode> currentRootEffectNodes;
        private Dictionary<ItemEffectNode, FrigidCoroutine> currentRootEffectNodeRoutines;
        private List<AbilityResource> abilityResources;

        private HashSet<MobBehaviour> effectBehaviours;
        private Action onInEffectChanged;

        public ItemStorage Storage
        {
            get
            {
                return this.storage;
            }
        }

        public abstract bool IsUsable { get; }

        public bool InUse 
        { 
            get
            {
                return this.inUse;
            }
            protected set
            {
                if (this.inUse != value)
                {
                    this.inUse = value;
                    this.onInUseChanged?.Invoke();
                }
            }
        }

        public Action OnInUseChanged
        {
            get
            {
                return this.onInUseChanged;
            }
            set
            {
                this.onInUseChanged = value;
            }
        }


        public bool StorageChangeable
        {
            get
            {
                return this.storageChangeable;
            }
            protected set
            {
                if (this.storageChangeable != value)
                {
                    this.storageChangeable = value;
                    this.onStorageChangeableChanged?.Invoke();
                }
            }
        }

        public Action OnStorageChangeableChanged
        {
            get
            {
                return this.onStorageChangeableChanged;
            }
            set
            {
                this.onStorageChangeableChanged = value;
            }
        }

        public bool InEffect
        {
            get
            {
                return this.effectBehaviours.Count > 0;
            }
        }

        public Action OnInEffectChanged
        {
            get
            {
                return this.onInEffectChanged;
            }
            set
            {
                this.onInEffectChanged = value;
            }
        }

        public List<AbilityResource> AbilityResources
        {
            get
            {
                return this.abilityResources;
            }
        }

        public virtual void Created()
        {
            this.inUse = false;
            this.storageChangeable = true;
            this.currentRootEffectNodes = new HashSet<ItemEffectNode>();
            this.currentRootEffectNodeRoutines = new Dictionary<ItemEffectNode, FrigidCoroutine>();
            this.currentRootEffectNodes = new HashSet<ItemEffectNode>(this.InitialRootEffectNodes);
            this.abilityResources = new List<AbilityResource>();
            foreach (ItemEffectNode rootEffectNode in this.ReferencedRootEffectNodes)
            {
                rootEffectNode.OwnedBy(this);
                rootEffectNode.CollectAbilityResources(ref this.abilityResources);
                rootEffectNode.Created();
                rootEffectNode.gameObject.SetActive(false);
            }

            this.effectBehaviours = new HashSet<MobBehaviour>();
        }

        public virtual void Discarded()
        {
            foreach (ItemEffectNode rootEffectNode in this.ReferencedRootEffectNodes)
            {
                rootEffectNode.Discarded();
                rootEffectNode.OwnedBy(null);
                rootEffectNode.gameObject.SetActive(false);
            }
        }

        public void StoredBy(ItemStorage storage)
        {
            this.storage = storage;
        }
 
        public virtual void Stored() 
        {
            if (this.Storage.TryGetUsingMob(out Mob usingMob))
            {
                usingMob.OnStatusChanged += this.EnterOrExitRootEffectNodesOnStatusChange;
                if (usingMob.Status != MobStatus.Dead)
                {
                    foreach (ItemEffectNode currentRootEffectNode in this.currentRootEffectNodes)
                    {
                        this.EnterRootEffectNode(currentRootEffectNode);
                    }
                }
            }
        }

        public virtual void Unstored() 
        {
            if (this.Storage.TryGetUsingMob(out Mob usingMob))
            {
                usingMob.OnStatusChanged -= this.EnterOrExitRootEffectNodesOnStatusChange;
                if (usingMob.Status != MobStatus.Dead)
                {
                    foreach (ItemEffectNode currentRootEffectNode in this.currentRootEffectNodes)
                    {
                        this.ExitRootEffectNode(currentRootEffectNode);
                    }
                }
            }
        }
        
        public virtual bool Used() { return false; }

        public void AddStatusTagToMob(MobStatusTag statusTag)
        {
            if (this.Storage.TryGetUsingMob(out Mob usingMob))
            {
                usingMob.AddStatusTag(statusTag);
            }
        }

        public void RemoveStatusTagFromMob(MobStatusTag statusTag)
        {
            if (this.Storage.TryGetUsingMob(out Mob usingMob))
            {
                usingMob.RemoveStatusTag(statusTag);
            }
        }

        public void AddEffectBehaviour(MobBehaviour behaviour, Action onFinished = null)
        {
            if (!this.Storage.TryGetUsingMob(out Mob usingMob))
            {
                onFinished?.Invoke();
                return;
            }

            this.effectBehaviours.Add(behaviour);
            bool success = usingMob.AddBehaviour(
                behaviour,
                true,
                () =>
                {
                    this.effectBehaviours.Remove(behaviour);
                    onFinished?.Invoke();
                    if (this.effectBehaviours.Count == 0)
                    {
                        this.onInEffectChanged?.Invoke();
                    }
                }
                );
            if (!success)
            {
                this.effectBehaviours.Remove(behaviour);
                onFinished?.Invoke();
                return;
            }

            if (this.effectBehaviours.Count == 1)
            {
                this.onInEffectChanged?.Invoke();
            }
        }

        public void RemoveEffectBehaviour(MobBehaviour behaviour)
        {
            if (!this.Storage.TryGetUsingMob(out Mob usingMob))
            {
                return;
            }

            if (!usingMob.RemoveBehaviour(behaviour))
            {
                return;
            }
            this.effectBehaviours.Remove(behaviour);

            if (this.effectBehaviours.Count == 0)
            {
                this.onInEffectChanged?.Invoke();
            }
        }

        protected abstract HashSet<ItemEffectNode> InitialRootEffectNodes { get; }

        protected abstract HashSet<ItemEffectNode> ReferencedRootEffectNodes { get; }

        protected void AddRootEffectNode(ItemEffectNode rootEffectNode)
        {
            if (!this.currentRootEffectNodes.Contains(rootEffectNode))
            {
                rootEffectNode.gameObject.SetActive(true);
                this.currentRootEffectNodes.Add(rootEffectNode);
                if (this.Storage.TryGetUsingMob(out Mob usingMob) && usingMob.Status != MobStatus.Dead)
                {
                    this.EnterRootEffectNode(rootEffectNode);
                }
            }
            else
            {
                Debug.LogError("ItemEffectNode " + rootEffectNode.name + " is already a current root effect node in item " + this.name + "!");
            }
        }

        protected void RemoveRootEffectNode(ItemEffectNode rootEffectNode)
        {
            if (this.currentRootEffectNodes.Contains(rootEffectNode))
            {
                if (this.Storage.TryGetUsingMob(out Mob usingMob) && usingMob.Status != MobStatus.Dead)
                {
                    this.ExitRootEffectNode(rootEffectNode);
                }
                this.currentRootEffectNodes.Remove(rootEffectNode);
                rootEffectNode.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("ItemEffectNode " + rootEffectNode.name + " is not a current root effect node in item " + this.name + "!");
            }
        }

        private void EnterOrExitRootEffectNodesOnStatusChange()
        {
            this.Storage.TryGetUsingMob(out Mob usingMob);
            foreach (ItemEffectNode currentRootEffectNode in this.currentRootEffectNodes)
            {
                if (usingMob.Status != MobStatus.Dead)
                {
                    this.EnterRootEffectNode(currentRootEffectNode);
                }
                else
                {
                    this.ExitRootEffectNode(currentRootEffectNode);
                }
            }
        }

        private void EnterRootEffectNode(ItemEffectNode rootEffectNode)
        {
            if (!this.currentRootEffectNodeRoutines.ContainsKey(rootEffectNode) && this.Storage.TryGetUsingMob(out _))
            {
                this.currentRootEffectNodeRoutines.Add(rootEffectNode, FrigidCoroutine.Run(this.NodeRefresh(rootEffectNode), this.gameObject));
                rootEffectNode.Enter();
            }
        }

        private void ExitRootEffectNode(ItemEffectNode rootEffectNode)
        {
            if (this.currentRootEffectNodeRoutines.TryGetValue(rootEffectNode, out FrigidCoroutine refreshRoutine) && this.Storage.TryGetUsingMob(out _))
            {
                this.currentRootEffectNodeRoutines.Remove(rootEffectNode);
                rootEffectNode.Exit();
                FrigidCoroutine.Kill(refreshRoutine);
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> NodeRefresh(ItemEffectNode effectNode)
        {
            while (true)
            {
                effectNode.Refresh();
                yield return null;
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

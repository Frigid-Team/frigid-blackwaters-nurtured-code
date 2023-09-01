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

        private HashSet<ItemNode> activeRootNodes;
        private Dictionary<ItemNode, FrigidCoroutine> enteredRootNodeRoutines;

        private Action onInEffectChanged;
        private Action onActiveAbilityResourcesChanged;

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
                bool inEffect = false;
                foreach (ItemNode activeRootNode in this.activeRootNodes)
                {
                    foreach (ItemNode currentNode in activeRootNode.CurrentNodes)
                    {
                        inEffect |= currentNode.InEffect;
                    }
                }
                return inEffect;
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

        public List<AbilityResource> ActiveAbilityResources
        {
            get
            {
                List<AbilityResource> allActiveAbilityResources = new List<AbilityResource>();
                foreach (ItemNode activeRootNode in this.activeRootNodes)
                {
                    foreach (ItemNode currentNode in activeRootNode.CurrentNodes)
                    {
                        allActiveAbilityResources.AddRange(currentNode.ActiveAbilityResources);
                    }
                }
                return allActiveAbilityResources;
            }
        }

        public Action OnActiveAbilityResourcesChanged
        {
            get
            {
                return this.onActiveAbilityResourcesChanged;
            }
            set
            {
                this.onActiveAbilityResourcesChanged = value;
            }
        }

        public void Assign(ItemStorage storage)
        {
            this.storage = storage;
        }

        public void Unassign()
        {
            this.storage = null;
        }
 
        public virtual void Stored() 
        {
            foreach (ItemNode activeRootNode in this.activeRootNodes)
            {
                this.EnterRootNode(activeRootNode);
            }
        }

        public virtual void Unstored() 
        {
            foreach (ItemNode activeRootNode in this.activeRootNodes)
            {
                this.ExitRootNode(activeRootNode);
            }
        }
        
        public virtual bool Used() { return false; }

        public void AddBehaviourToMob(MobBehaviour behaviour)
        {
            if (this.Storage.TryGetUsingMob(out Mob usingMob))
            {
                usingMob.AddBehaviour(behaviour, true);
            }
        }

        public void RemoveBehaviourFromMob(MobBehaviour behaviour)
        {
            if (this.Storage.TryGetUsingMob(out Mob usingMob))
            {
                usingMob.RemoveBehaviour(behaviour);
            }
        }

        protected abstract HashSet<ItemNode> RootNodes { get; }

        protected override void Awake()
        {
            base.Awake();
            this.inUse = false;
            this.storageChangeable = true;
            this.activeRootNodes = new HashSet<ItemNode>();
            this.enteredRootNodeRoutines = new Dictionary<ItemNode, FrigidCoroutine>();
            foreach (ItemNode rootNode in this.RootNodes)
            {
                this.VisitRootAndChildren(
                    rootNode, 
                    (ItemNode node) => 
                    {
                        node.Link(this);
                        node.Init();
                    }
                    );
                rootNode.gameObject.SetActive(false);
            }
        }

        protected void ActivateRootNode(ItemNode rootNode)
        {
            if (!this.activeRootNodes.Contains(rootNode))
            {
                this.activeRootNodes.Add(rootNode);
                rootNode.gameObject.SetActive(true);
                this.EnterRootNode(rootNode);
            }
        }

        protected void DeactivateRootNode(ItemNode rootNode)
        {
            if (this.activeRootNodes.Contains(rootNode))
            {
                this.ExitRootNode(rootNode);
                this.activeRootNodes.Remove(rootNode);
                rootNode.gameObject.SetActive(false);
            }
        }

        private void EnterRootNode(ItemNode rootNode)
        {
            if (!this.enteredRootNodeRoutines.ContainsKey(rootNode) && this.Storage.TryGetUsingMob(out _))
            {
                this.enteredRootNodeRoutines.Add(rootNode, FrigidCoroutine.Run(this.NodeRefresh(rootNode), this.gameObject));
                rootNode.OnCurrentNodeAdded += this.HandleCurrentNodeAddition;
                rootNode.OnCurrentNodeRemoved += this.HandleCurrentNodeRemoval;
                foreach (ItemNode currentNode in rootNode.CurrentNodes)
                {
                    this.HandleCurrentNodeAddition(currentNode);
                }
                rootNode.Enter();
            }
        }

        private void ExitRootNode(ItemNode rootNode)
        {
            if (this.enteredRootNodeRoutines.TryGetValue(rootNode, out FrigidCoroutine refreshRoutine) && this.Storage.TryGetUsingMob(out _))
            {
                this.enteredRootNodeRoutines.Remove(rootNode);
                rootNode.Exit();
                rootNode.OnCurrentNodeAdded -= this.HandleCurrentNodeAddition;
                rootNode.OnCurrentNodeRemoved -= this.HandleCurrentNodeRemoval;
                foreach (ItemNode currentNode in rootNode.CurrentNodes)
                {
                    this.HandleCurrentNodeRemoval(currentNode);
                }
                FrigidCoroutine.Kill(refreshRoutine);
            }
        }

        private IEnumerator<FrigidCoroutine.Delay> NodeRefresh(ItemNode node)
        {
            while (true)
            {
                node.Refresh();
                yield return null;
            }
        }

        private void VisitRootAndChildren(ItemNode rootNode, Action<ItemNode> onVisited)
        {
            HashSet<ItemNode> visitedNodes = new HashSet<ItemNode>();
            Queue<ItemNode> nextNodes = new Queue<ItemNode>();
            nextNodes.Enqueue(rootNode);
            while (nextNodes.TryDequeue(out ItemNode nextNode))
            {
                if (!visitedNodes.Contains(nextNode))
                {
                    visitedNodes.Add(nextNode);
                    foreach (ItemNode referencedNode in nextNode.ReferencedNodes)
                    {
                        nextNodes.Enqueue(referencedNode);
                    }
                    onVisited.Invoke(nextNode);
                }
            }
        }

        private void HandleCurrentNodeAddition(ItemNode addedNode)
        {
            if (addedNode.ActiveAbilityResources.Count > 0)
            {
                this.onActiveAbilityResourcesChanged?.Invoke();
            }
            if (addedNode.InEffect)
            {
                foreach (ItemNode activeRootNode in this.activeRootNodes)
                {
                    foreach (ItemNode currentNode in activeRootNode.CurrentNodes)
                    {
                        if (currentNode != addedNode && currentNode.InEffect)
                        {
                            return;
                        }
                    }
                }
                this.onInEffectChanged?.Invoke();
            }
        }

        private void HandleCurrentNodeRemoval(ItemNode removedNode)
        {
            if (removedNode.ActiveAbilityResources.Count > 0)
            {
                this.onActiveAbilityResourcesChanged?.Invoke();
            }
            if (removedNode.InEffect)
            {
                foreach (ItemNode activeRootNode in this.activeRootNodes)
                {
                    foreach (ItemNode currentNode in activeRootNode.CurrentNodes)
                    {
                        if (currentNode.InEffect)
                        {
                            return;
                        }
                    }
                }
                this.onInEffectChanged?.Invoke();
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

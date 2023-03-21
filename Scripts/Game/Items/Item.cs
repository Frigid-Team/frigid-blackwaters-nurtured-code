using System.Collections.Generic;
using System;

namespace FrigidBlackwaters.Game
{
    public abstract class Item : FrigidMonoBehaviour
    {
        private ItemStorage storage;

        private HashSet<ItemNode> activeRootNodes;
        private Dictionary<ItemNode, FrigidCoroutine> enteredRootNodeRoutines;

        public ItemStorage Storage
        {
            get
            {
                return this.storage;
            }
        }

        protected abstract HashSet<ItemNode> RootNodes { get; }

        public abstract bool IsUsable { get; }

        public abstract bool IsInEffect { get; }

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
                EnterRootNode(activeRootNode);
            }
        }

        public virtual void Unstored() 
        {
            foreach (ItemNode activeRootNode in this.activeRootNodes)
            {
                ExitRootNode(activeRootNode);
            }
        }
        
        public virtual bool Used() { return false; }

        public virtual void Stashed() { }

        public virtual void Unstashed() { }

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

        protected override void Awake()
        {
            base.Awake();
            this.activeRootNodes = new HashSet<ItemNode>();
            this.enteredRootNodeRoutines = new Dictionary<ItemNode, FrigidCoroutine>();
            foreach (ItemNode rootNode in this.RootNodes)
            {
                VisitRootAndChildren(
                    rootNode, 
                    (ItemNode node) => 
                    {
                        node.Link(this);
                        node.Init();
                    }
                    );
            }
        }

        protected void ActivateRootNode(ItemNode rootNode)
        {
            if (!this.activeRootNodes.Contains(rootNode))
            {
                this.activeRootNodes.Add(rootNode);
                EnterRootNode(rootNode);
            }
        }

        protected void DeactivateRootNode(ItemNode rootNode)
        {
            if (this.activeRootNodes.Contains(rootNode))
            {
                ExitRootNode(rootNode);
                this.activeRootNodes.Remove(rootNode);
            }
        }

        private void EnterRootNode(ItemNode rootNode)
        {
            if (!this.enteredRootNodeRoutines.ContainsKey(rootNode) && this.Storage.TryGetUsingMob(out _))
            {
                rootNode.Enter();
                this.enteredRootNodeRoutines.Add(rootNode, FrigidCoroutine.Run(NodeRefresh(rootNode), this.gameObject));
            }
        }

        private void ExitRootNode(ItemNode rootNode)
        {
            if (this.enteredRootNodeRoutines.TryGetValue(rootNode, out FrigidCoroutine refreshRoutine) && this.Storage.TryGetUsingMob(out _))
            {
                rootNode.Exit();
                FrigidCoroutine.Kill(refreshRoutine);
                this.enteredRootNodeRoutines.Remove(rootNode);
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

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

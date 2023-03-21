using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class Menu : FrigidMonoBehaviourWithUpdate
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [Header("Prompts")]
        [SerializeField]
        private bool hasPrompt;
        [SerializeField]
        [ShowIfBool("hasPrompt", true)]
        private Sprite promptIcon;
        [SerializeField]
        [ShowIfPreviouslyShown(true)]
        private MenuPrompt menuPromptPrefab;

        private MenuPrompt menuPrompt;

        public void Open()
        {
            this.canvasGroup.interactable = true;
            this.canvasGroup.blocksRaycasts = true;
            Opened();
        }

        public void Close()
        {
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
            Closed();
        }

        public virtual bool IsOpenable() { return true; }

        public virtual bool IsClosable() { return true; }

        protected abstract void Opened();

        protected abstract void Closed();

        protected virtual bool ShouldShowPrompt(out Vector2 trackedPosition)
        {
            trackedPosition = Vector2.zero;
            return false;
        }

        protected override void Awake()
        {
            base.Awake();
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
            if (this.hasPrompt) 
            {
                this.menuPrompt = FrigidInstancing.CreateInstance<MenuPrompt>(this.menuPromptPrefab);
            }
        }

        protected override void Update()
        {
            base.Update();
            if (this.hasPrompt)
            {
                if (ShouldShowPrompt(out Vector2 trackedPosition))
                {
                    this.menuPrompt.ShowPrompt(this.promptIcon, trackedPosition);
                }
                else
                {
                    this.menuPrompt.HidePrompt();
                }
            }
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}

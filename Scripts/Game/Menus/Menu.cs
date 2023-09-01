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

        public virtual bool WantsToOpen() { return false; }

        public virtual bool WantsToClose() { return false; }

        public virtual void Opened()
        {
            this.canvasGroup.interactable = true;
            this.canvasGroup.blocksRaycasts = true;
        }

        public virtual void Closed()
        {
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
        }

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
                this.menuPrompt = CreateInstance<MenuPrompt>(this.menuPromptPrefab);
            }
        }

        protected override void Update()
        {
            base.Update();
            if (this.hasPrompt)
            {
                if (this.ShouldShowPrompt(out Vector2 trackedPosition))
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

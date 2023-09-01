using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class ProgressionInterface : FrigidMonoBehaviour
    {
        [SerializeField]
        private ExpeditionProgress expeditionProgress;
        [SerializeField]
        private Transform contentTransform;
        [SerializeField]
        private CanvasGroup canvasGroup;

        [Header("Text Boxes")]
        [SerializeField]
        private Text stampNumberText;
        [SerializeField]
        private Text expeditionTitle;
        [SerializeField]
        private Text expeditionDescription;
        [SerializeField]
        private Text expeditionDungeonInfo;
        [SerializeField]
        private Text expeditionCompensation;
        [SerializeField]
        private Text expeditionButtonText;
        [SerializeField]
        private Button changeSceneButton;

        [SerializeField]
        private string returnText;
        [SerializeField]
        private string startExpeditionText;


        protected override void Awake()
        {
            base.Awake();
            this.contentTransform.gameObject.SetActive(false);
        }

        public void Opened()
        {
            this.canvasGroup.interactable = true;
            this.canvasGroup.blocksRaycasts = true;
            this.expeditionProgress.OnUpdated += this.RefreshExpeditionDetails;
            this.contentTransform.gameObject.SetActive(true);
            this.changeSceneButton.onClick.AddListener(this.DepartOrReturn);
            this.RefreshExpeditionDetails();
        }

        public void Closed()
        {
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;
            this.expeditionProgress.OnUpdated -= this.RefreshExpeditionDetails;
            this.contentTransform.gameObject.SetActive(false);
            this.changeSceneButton.onClick.RemoveListener(this.DepartOrReturn);
        }

        private void RefreshExpeditionDetails()
        {
            this.stampNumberText.text = "" + Stamps.CurrentStamps + " / " + Stamps.TotalStamps;
            if (this.expeditionProgress.TryGetActiveExpedition(out Expedition activeExpedition))
            {
                this.expeditionButtonText.text = this.returnText;
                this.changeSceneButton.enabled = true;
                this.expeditionTitle.text = activeExpedition.ExpeditionName;
                this.expeditionDescription.text = activeExpedition.FlairDescription;
                this.expeditionDungeonInfo.text = activeExpedition.DungeonInfoDescription;
                this.expeditionCompensation.text = activeExpedition.CompensationFieldDescription;
                if (this.expeditionProgress.CompletedExpeditions.Contains(activeExpedition))
                {
                    this.expeditionTitle.color = Color.green;
                    this.expeditionDescription.color = Color.green;
                    this.expeditionDungeonInfo.color = Color.green;
                    this.expeditionCompensation.color = Color.green;
                }
                return;
            }
            else if (this.expeditionProgress.TryGetSelectedExpedition(out Expedition selectedExpedition))
            {
                this.expeditionButtonText.text = this.startExpeditionText;
                this.changeSceneButton.enabled = true;
                this.expeditionTitle.text = selectedExpedition.ExpeditionName;
                this.expeditionDescription.text = selectedExpedition.FlairDescription;
                this.expeditionDungeonInfo.text = selectedExpedition.DungeonInfoDescription;
                this.expeditionCompensation.text = selectedExpedition.CompensationFieldDescription;
                return;
            }
            this.expeditionTitle.text = "";
            this.expeditionDescription.text = "";
            this.expeditionDungeonInfo.text = "";
            this.expeditionCompensation.text = "";
            this.changeSceneButton.enabled = false;
        }

        private void DepartOrReturn()
        {
            if (!this.expeditionProgress.DepartOnExpedition())
            {
                this.expeditionProgress.ReturnFromExpedition();
            }
        }
    }
}
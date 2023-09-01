using UnityEngine;
using UnityEngine.UI;

namespace FrigidBlackwaters.Game
{
    public class ExpeditionInterfacePosting : FrigidMonoBehaviour
    {
        [Header("Graphics")]
        [SerializeField]
        private Text titleText;
        [SerializeField]
        private Text expeditionTypeText;
        [SerializeField]
        private Text compensationText;
        [SerializeField]
        private Image selectedExpeditionImage;
        [SerializeField]
        private Image expeditionComplete;
        [SerializeField]
        private Button button;

        public void Populate(Expedition expedition, ExpeditionBoard expeditionBoard)
        {
            this.selectedExpeditionImage.gameObject.SetActive(expeditionBoard.ExpeditionProgress.TryGetSelectedExpedition(out Expedition selectedExpedition) && selectedExpedition == expedition);
            this.expeditionComplete.gameObject.SetActive(expeditionBoard.ExpeditionProgress.CompletedExpeditions.Contains(expedition));
            this.titleText.text = expedition.ExpeditionName;
            this.expeditionTypeText.text = expedition.ExpeditionTypeDescription;
            this.compensationText.text = expedition.NumberAwardedStamps + " stamps";
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(
                () => 
                {
                    expeditionBoard.ExpeditionProgress.SelectExpedition(expedition);
                }
                );
        }
    }
}
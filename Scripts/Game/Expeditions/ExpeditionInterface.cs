using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class ExpeditionInterface : FrigidMonoBehaviour
    {
        [SerializeField]
        private RectTransform expeditionContentTransform;
        [SerializeField]
        private RectTransform viewportContentTransform;
        [SerializeField]
        private RectTransform boardBackgroundImageTransform;
        [SerializeField]
        private int boardBuffer;
        [SerializeField]
        private int widthPerPosting;
        [SerializeField]
        private Vector2 variationRange;
        [SerializeField]
        private Button startButton;
        [SerializeField]
        private List<ExpeditionInterfacePosting> expeditionPostingPrefabs;

        [Header("Editable Fields")]
        [SerializeField]
        private Text expeditionName;
        [SerializeField]
        private Text descriptionText;
        [SerializeField]
        private Text dungeonInfoText;
        [SerializeField]
        private Text compensationText;

        private ExpeditionBoard expeditionBoard;
        private RecyclePool<ExpeditionInterfacePosting> postingPool;
        private List<ExpeditionInterfacePosting> currentPostings;
        private List<Vector2> postingPositions;

        public void Open(ExpeditionBoard expeditionBoard)
        {
            this.expeditionBoard = expeditionBoard;
            this.gameObject.SetActive(true);
            this.startButton.onClick.AddListener(this.Depart);
            this.expeditionBoard.ExpeditionProgress.OnUpdated += this.RefreshExpeditionDetails;
            this.RefreshExpeditionDetails();
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
            this.startButton.onClick.RemoveListener(this.Depart);
            this.expeditionBoard.ExpeditionProgress.OnUpdated -= this.RefreshExpeditionDetails;
            this.expeditionBoard = null;
        }

        protected override void Awake()
        {
            base.Awake();
            this.gameObject.SetActive(false);
            this.postingPool = new RecyclePool<ExpeditionInterfacePosting>(
                () => CreateInstance<ExpeditionInterfacePosting>(this.expeditionPostingPrefabs[Random.Range(0, this.expeditionPostingPrefabs.Count)], this.expeditionContentTransform, false), 
                (ExpeditionInterfacePosting posting) => DestroyInstance(posting)
                );
            this.currentPostings = new List<ExpeditionInterfacePosting>();
            this.postingPositions = new List<Vector2>();
        }

        private Vector2 GetPostingPosition(int rowIndex, int colIndex)
        {
            int totalExpeditionsPerRow = Mathf.CeilToInt(this.expeditionBoard.ExpeditionProgress.AvailableExpeditions.Count / 2f);
            int widthPerPosting = Mathf.FloorToInt((this.boardBackgroundImageTransform.rect.width - (2 * this.boardBuffer)) / (totalExpeditionsPerRow));
            int heightPerPosting = Mathf.FloorToInt((this.boardBackgroundImageTransform.rect.height - (2 * this.boardBuffer)) / 2);
            float x = this.boardBuffer + widthPerPosting * colIndex + widthPerPosting / 2;
            float y = this.boardBuffer + (heightPerPosting + this.boardBuffer / 2) * rowIndex + heightPerPosting / 2;

            float xVariation = Mathf.RoundToInt(Probability.NormalDistribution(0, this.variationRange.x / 2, -this.variationRange.x, this.variationRange.x));
            float yVariation = Mathf.RoundToInt(Probability.NormalDistribution(0, this.variationRange.y / 2, -this.variationRange.y, this.variationRange.y));
            return new Vector2(x + xVariation, -(y + yVariation));
        }

        private void RefreshExpeditionDetails()
        {
            Vector2 postingCanvasSize = new Vector2(
                2 * this.boardBuffer + (Mathf.CeilToInt(this.expeditionBoard.ExpeditionProgress.AvailableExpeditions.Count / 2f) * this.widthPerPosting),
                this.boardBackgroundImageTransform.rect.height
                );
            this.boardBackgroundImageTransform.sizeDelta = postingCanvasSize;
            this.viewportContentTransform.sizeDelta = postingCanvasSize;
            int rowIndex = 0;
            int colIndex = 0;
            if (this.currentPostings.Count != this.expeditionBoard.ExpeditionProgress.AvailableExpeditions.Count)
            {
                this.postingPool.Cycle(this.currentPostings, this.expeditionBoard.ExpeditionProgress.AvailableExpeditions.Count);
                for (int i = 0; i < this.currentPostings.Count; i++)
                {
                    this.postingPositions.Add(this.GetPostingPosition(rowIndex, colIndex));
                    if (rowIndex == 0)
                    {
                        rowIndex = 1;
                    }
                    else
                    {
                        rowIndex = 0;
                        colIndex++;
                    }
                }
            }
            for (int i = 0; i < this.expeditionBoard.ExpeditionProgress.AvailableExpeditions.Count; i++)
            {
                ExpeditionInterfacePosting newPosting = this.currentPostings[i];
                newPosting.Populate(this.expeditionBoard.ExpeditionProgress.AvailableExpeditions[i], this.expeditionBoard);
                newPosting.gameObject.transform.localPosition = this.postingPositions[i];
            }
            if (!this.expeditionBoard.ExpeditionProgress.TryGetSelectedExpedition(out Expedition selectedExpedition))
            {
                this.expeditionName.text = "";
                this.descriptionText.text = "";
                this.dungeonInfoText.text = "";
                this.compensationText.text = "";
                return;
            }
            this.expeditionName.text = selectedExpedition.ExpeditionName;
            this.descriptionText.text = selectedExpedition.FlairDescription;
            this.dungeonInfoText.text = selectedExpedition.DungeonInfoDescription;
            this.compensationText.text = selectedExpedition.CompensationFieldDescription;
        }

        private void Depart()
        {
            this.expeditionBoard.ExpeditionProgress.DepartOnExpedition();
        }
    }
}

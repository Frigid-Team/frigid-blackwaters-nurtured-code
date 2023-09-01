using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "AnimatorBodyToolConfig", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.ANIMATION + "AnimatorBodyToolConfig")]
    public class AnimatorBodyToolConfig : FrigidScriptableObject
    {
        [SerializeField]
        private int previewLength;
        [SerializeField]
        private int previewMinNumberTiles;
        [SerializeField]
        private int previewMaxNumberTiles;
        [SerializeField]
        private Texture2D lightPreviewPanelTexture;
        [SerializeField]
        private Texture2D darkPreviewPanelTexture;
        [Space]
        [SerializeField]
        private int borderLength;
        [Space]
        [SerializeField]
        private int indentWidth;
        [SerializeField]
        private int cellLength;
        [SerializeField]
        private int frameNumberPadding;
        [SerializeField]
        private int cellPreviewPadding;
        [SerializeField]
        private int numberFramesPerPage;
        [SerializeField]
        private int propertyBindWidth;
        [SerializeField]
        private int propertyNameWidth;
        [SerializeField]
        private Texture2D indentArrowTexture;
        [SerializeField]
        private Texture2D emptyCellTexture;
        [SerializeField]
        private Texture2D columnMarkerCellTexture;
        [SerializeField]
        private Texture2D frameRowMarkerCellTexture;
        [SerializeField]
        private Texture2D orientationRowMarkerCellTexture;
        [SerializeField]
        private Texture2D mainCellTexture;
        [SerializeField]
        private Texture2D cornerCellTexture;
        [SerializeField]
        private Texture2D cellPreviewDiamondTexture;
        [SerializeField]
        private Texture2D cellPreviewSquareTexture;
        [SerializeField]
        private Texture2D propertyBindTexture;
        [Space]
        [SerializeField]
        private int orientationBarLength;
        [SerializeField]
        private int orientationZeroCircleRadius;
        [SerializeField]
        private int orientationDragCircleRadius;
        [SerializeField]
        private int orientationDragButtonLength;
        [SerializeField]
        private int orientationZeroDirectionThresholdDistance;
        [SerializeField]
        private Texture2D orientationButtonTexture;
        [Space]
        [SerializeField]
        private Color lightColor;
        [SerializeField]
        private Color mediumColor;
        [SerializeField]
        private Color darkColor;
        [SerializeField]
        private Color indentColor;
        [Space]
        [SerializeField]
        private int handleLength;
        [SerializeField]
        private int handleGrabLength;
        [Space]
        [SerializeField]
        private int copyPasteOutlineThickness;

        public int PreviewLength
        {
            get
            {
                return this.previewLength;
            }
        }

        public int PreviewMinNumberTiles
        {
            get
            {
                return this.previewMinNumberTiles;
            }
        }

        public int PreviewMaxNumberTiles
        {
            get
            {
                return this.previewMaxNumberTiles;
            }
        }

        public Texture2D LightPreviewPanelTexture
        {
            get
            {
                return this.lightPreviewPanelTexture;
            }
        }

        public Texture2D DarkPreviewPanelTexture
        {
            get
            {
                return this.darkPreviewPanelTexture;
            }
        }

        public int BorderLength
        {
            get
            {
                return this.borderLength;
            }
        }

        public int IndentWidth
        {
            get
            {
                return this.indentWidth;
            }
        }

        public int CellLength
        {
            get
            {
                return this.cellLength;
            }
        }

        public int FrameNumberPadding
        {
            get
            {
                return this.frameNumberPadding;
            }
        }

        public int CellPreviewPadding
        {
            get
            {
                return this.cellPreviewPadding;
            }
        }

        public int NumberFramesPerPage
        {
            get
            {
                return this.numberFramesPerPage;
            }
        }

        public int PropertyBindWidth
        {
            get
            {
                return this.propertyBindWidth;
            }
        }

        public int PropertyNameWidth
        {
            get
            {
                return this.propertyNameWidth;
            }
        }

        public Texture2D IndentArrowTexture
        {
            get
            {
                return this.indentArrowTexture;
            }
        }

        public Texture2D EmptyCellTexture
        {
            get
            {
                return this.emptyCellTexture;
            }
        }

        public Texture2D ColumnMarkerCellTexture
        {
            get
            {
                return this.columnMarkerCellTexture;
            }
        }

        public Texture2D FrameRowMarkerCellTexture
        {
            get
            {
                return this.frameRowMarkerCellTexture;
            }
        }

        public Texture2D OrientationRowMarkerCellTexture
        {
            get
            {
                return this.orientationRowMarkerCellTexture;
            }
        }

        public Texture2D MainCellTexture
        {
            get
            {
                return this.mainCellTexture;
            }
        }

        public Texture2D CornerCellTexture
        {
            get
            {
                return this.cornerCellTexture;
            }
        }

        public Texture2D CellPreviewDiamondTexture
        {
            get
            {
                return this.cellPreviewDiamondTexture;
            }
        }

        public Texture2D CellPreviewSquareTexture
        {
            get
            {
                return this.cellPreviewSquareTexture;
            }
        }

        public Texture2D PropertyBindTexture
        {
            get
            {
                return this.propertyBindTexture;
            }
        }

        public int OrientationBarLength
        {
            get
            {
                return this.orientationBarLength;
            }
        }

        public int OrientationZeroCircleRadius
        {
            get
            {
                return this.orientationZeroCircleRadius;
            }
        }

        public int OrientationDragCircleRadius
        {
            get
            {
                return this.orientationDragCircleRadius;
            }
        }

        public int OrientationDragButtonLength
        {
            get
            {
                return this.orientationDragButtonLength;
            }
        }

        public int OrientationZeroDirectionThresholdDistance
        {
            get
            {
                return this.orientationZeroDirectionThresholdDistance;
            }
        }

        public Texture2D OrientationButtonTexture
        {
            get
            {
                return this.orientationButtonTexture;
            }
        }

        public Color LightColor
        {
            get
            {
                return this.lightColor;
            }
        }

        public Color MediumColor
        {
            get
            {
                return this.mediumColor;
            }
        }

        public Color DarkColor
        {
            get
            {
                return this.darkColor;
            }
        }

        public Color IndentColor
        {
            get
            {
                return this.indentColor;
            }
        }

        public int HandleLength
        {
            get
            {
                return this.handleLength;
            }
        }

        public int HandleGrabLength
        {
            get
            {
                return this.handleGrabLength;
            }
        }

        public int CopyPasteOutlineThickness
        {
            get
            {
                return this.copyPasteOutlineThickness;
            }
        }
    }
}


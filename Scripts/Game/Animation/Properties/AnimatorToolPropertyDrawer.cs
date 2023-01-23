#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    public abstract class AnimatorToolPropertyDrawer
    {
        protected const float WORLD_OBJECT_PREVIEW_ORDER = 0f;
        protected const float GUI_PREVIEW_ORDER = 1f;

        private AnimatorProperty property;
        private AnimatorBody body;
        private AnimatorBodyToolConfig config;

        public AnimatorToolPropertyDrawer Copy(AnimatorProperty property, AnimatorBody body, AnimatorBodyToolConfig config)
        {
            AnimatorToolPropertyDrawer copiedPropertyEditorDrawer = (AnimatorToolPropertyDrawer)MemberwiseClone();
            copiedPropertyEditorDrawer.property = property;
            copiedPropertyEditorDrawer.body = body;
            copiedPropertyEditorDrawer.config = config;
            return copiedPropertyEditorDrawer;
        }

        public abstract string LabelName
        {
            get;
        }

        public abstract Color AccentColor
        {
            get;
        }

        public virtual float[] CalculateChildPreviewOrders(int animationIndex, int frameIndex, int orientationIndex)
        {
            return new float[0];
        }

        public virtual void DrawGeneralEditFields() { }

        public virtual void DrawAnimationEditFields(int animationIndex) { }

        public virtual void DrawFrameEditFields(int animationIndex, int frameIndex) { }

        public virtual void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex) { }

        public virtual void DrawPreview(
            Vector2 previewSize,
            Vector2 previewOffset,
            float worldToWindowScalingFactor,
            int animationIndex,
            int frameIndex,
            int orientationIndex,
            bool propertySelected,
            out List<(Rect rect, Action onDrag)> dragRequests
            ) 
        {
            dragRequests = new List<(Rect rect, Action onDrag)>();
        }

        public virtual void DrawFrameCellPreview(
            Vector2 cellSize,
            int animationIndex,
            int frameIndex
            ) { }

        public virtual void DrawOrientationCellPreview(
            Vector2 cellSize,
            int animationIndex,
            int frameIndex,
            int orientationIndex
            ) { }

        protected AnimatorProperty Property
        {
            get
            {
                return this.property;
            }
        }

        protected AnimatorBody Body
        {
            get
            {
                return this.body;
            }
        }

        protected AnimatorBodyToolConfig Config
        {
            get
            {
                return this.config;
            }
        }
    }
}
#endif

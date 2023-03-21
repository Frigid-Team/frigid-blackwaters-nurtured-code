#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

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

        public virtual void DrawGeneralEditFields() 
        {
            EditorGUILayout.LabelField("Child Properties", GUIStyling.WordWrapAndCenter(EditorStyles.boldLabel));
            GUILayoutHelper.DrawIndexedList(
                this.property.GetNumberChildProperties(),
                (int index) => 
                {
                    FrigidPopupWindow.Show(
                        GUILayoutUtility.GetLastRect(), 
                        new TypeSelectionPopup(
                            TypeUtility.GetCompleteTypesDerivedFrom(typeof(AnimatorProperty)), 
                            (Type selectedType) => this.property.AddChildPropertyAt(index, selectedType)
                            )
                        );
                },
                this.property.RemoveChildPropertyAt,
                (int index) => EditorGUILayout.LabelField(this.property.GetChildPropertyAt(index).PropertyName)
                );
        }

        public virtual void DrawAnimationEditFields(int animationIndex) { }

        public virtual void DrawFrameEditFields(int animationIndex, int frameIndex) { }

        public virtual void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex) { }

        public virtual Vector2 DrawPreview(
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
            return Vector2.zero;
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

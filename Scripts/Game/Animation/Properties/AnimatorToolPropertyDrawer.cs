#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class AnimatorToolPropertyDrawer
    {
        protected const float WorldObjectPreviewOrder = 0f;
        protected const float GUIPreviewOrder = 1f;

        private AnimatorProperty property;
        private AnimatorBody body;
        private AnimatorBodyToolConfig config;

        public AnimatorToolPropertyDrawer Copy(AnimatorProperty property, AnimatorBody body, AnimatorBodyToolConfig config)
        {
            AnimatorToolPropertyDrawer copiedPropertyEditorDrawer = (AnimatorToolPropertyDrawer)this.MemberwiseClone();
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
            UtilityGUILayout.IndexedList(
                "Child Properties",
                this.Property.GetNumberChildProperties(),
                (int childIndex) => 
                {
                    List<Type> propertyTypes = TypeUtility.GetCompleteTypesDerivedFrom(typeof(AnimatorProperty));
                    SearchPopup typeSelectionPopup = new SearchPopup(
                        propertyTypes.Select((Type type) => type.Name).ToArray(),
                        (int typeIndex) => this.Property.AddChildPropertyAt(childIndex, propertyTypes[typeIndex])
                        );
                    FrigidPopup.Show(GUILayoutUtility.GetLastRect(), typeSelectionPopup);
                },
                this.Property.RemoveChildPropertyAt,
                (int childIndex) => EditorGUILayout.LabelField(this.Property.GetChildPropertyAt(childIndex).PropertyName)
                );
        }

        public virtual void DrawAnimationEditFields(int animationIndex) { }

        public virtual void DrawFrameEditFields(int animationIndex, int frameIndex) { }

        public virtual void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex) 
        {
            this.Property.SetLocalPosition(animationIndex, frameIndex, orientationIndex, EditorGUILayout.Vector2Field("Local Position", this.Property.GetLocalPosition(animationIndex, frameIndex, orientationIndex)));
            this.Property.SetLocalRotation(animationIndex, frameIndex, orientationIndex, EditorGUILayout.Slider("Local Rotation", this.Property.GetLocalRotation(animationIndex, frameIndex, orientationIndex), 0, 360));
        }

        public virtual void DrawPreview(Vector2 previewSize, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected) 
        {
            if (propertySelected)
            {
                Vector2 center = previewSize / 2;
                using (new UtilityGUI.ColorScope(Color.magenta))
                {
                    UtilityGUI.DrawLine(center, center + Vector2.down * this.Config.HandleGrabLength);
                }
                using (new UtilityGUI.ColorScope(Color.cyan))
                {
                    UtilityGUI.DrawLine(center, center + Vector2.right * this.Config.HandleGrabLength);
                }
            }
        }

        public virtual void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex) { }

        public virtual void DrawOrientationCellPreview(Vector2 cellSize, int animationIndex, int frameIndex, int orientationIndex) { }

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

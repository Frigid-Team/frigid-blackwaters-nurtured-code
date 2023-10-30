#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class RendererAnimatorToolPropertyDrawer : SortingOrderedAnimatorToolPropertyDrawer
    {
        public override void DrawGeneralEditFields()
        {
            RendererAnimatorProperty rendererProperty = (RendererAnimatorProperty)this.Property;
            rendererProperty.IsConsideredVisibleArea = EditorGUILayout.Toggle("Is Visible Area", rendererProperty.IsConsideredVisibleArea);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                rendererProperty.OriginalMaterial = (Material)EditorGUILayout.ObjectField("Material", rendererProperty.OriginalMaterial, typeof(Material), false);
                for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
                {
                    MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                    UtilityGUILayout.IndexedList(
                        propertyType.ToString() + " Material Properties",
                        rendererProperty.GetNumberBindings(propertyType),
                        (int bindingIndex) => rendererProperty.AddBindingAt(propertyType, bindingIndex),
                        (int bindingIndex) => rendererProperty.RemoveBindingAt(propertyType, bindingIndex),
                        (int bindingIndex) =>
                        {
                            rendererProperty.SetBindingPropertyId(propertyType, bindingIndex, EditorGUILayout.TextField("Property Id", rendererProperty.GetBindingPropertyId(propertyType, bindingIndex)));
                            rendererProperty.SetBindingCadence(propertyType, bindingIndex, (short)EditorGUILayout.Popup("Cadence", rendererProperty.GetBindingCadence(propertyType, bindingIndex), new string[] { "Animation", "Frame", "Orientation" }));
                        }
                        );

                    if (i < (int)MaterialProperties.Type.Count - 1) EditorGUILayout.Space();
                }
            }
            base.DrawGeneralEditFields();
        }

        public override void DrawAnimationEditFields(int animationIndex)
        {
            RendererAnimatorProperty rendererProperty = (RendererAnimatorProperty)this.Property;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
                {
                    MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                    List<int> bindingIndices = this.GetBindingIndicesWithCadence(propertyType, 0);
                    UtilityGUILayout.IndexedList(
                        propertyType.ToString() + " Material Properties",
                        bindingIndices.Count,
                        null,
                        null,
                        (int bindingIndex) =>
                        {
                            bindingIndex = bindingIndices[bindingIndex];
                            string propertyId = rendererProperty.GetBindingPropertyId(propertyType, bindingIndex);
                            object bindingValue = rendererProperty.GetBindingValueByReference(propertyType, bindingIndex, animationIndex);
                            object propertyValue = CoreGUILayout.MaterialPropertySerializedReferenceField(propertyId, propertyType, bindingValue);
                            rendererProperty.SetBindingValueByReference(propertyType, bindingIndex, animationIndex, propertyValue);
                        }
                        );

                    if (i < (int)MaterialProperties.Type.Count - 1) EditorGUILayout.Space();
                }
            }
            base.DrawAnimationEditFields(animationIndex);
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            RendererAnimatorProperty rendererProperty = (RendererAnimatorProperty)this.Property;
            UtilityGUILayout.IndexedList(
                "Material Tweens In Frame",
                rendererProperty.GetNumberMaterialTweens(animationIndex, frameIndex),
                (int tweenIndex) => rendererProperty.AddMaterialTweenByReferenceAt(animationIndex, frameIndex, tweenIndex),
                (int tweenIndex) => rendererProperty.RemoveMaterialTweenByReferenceAt(animationIndex, frameIndex, tweenIndex),
                (int tweenIndex) =>
                {
                    MaterialTweenOptionSetSerializedReference materialTween = CoreGUILayout.MaterialTweenTemplateSerializedReferenceField(string.Format("Tween [{0}]", tweenIndex), rendererProperty.GetMaterialTweenByReferenceAt(animationIndex, frameIndex, tweenIndex));
                    rendererProperty.SetMaterialTweenByReferenceAt(animationIndex, frameIndex, tweenIndex, materialTween);
                }
                );
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
                {
                    MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                    List<int> bindingIndices = this.GetBindingIndicesWithCadence(propertyType, 1);
                    UtilityGUILayout.IndexedList(
                        propertyType.ToString() + " Material Properties",
                        bindingIndices.Count,
                        null,
                        null,
                        (int bindingIndex) =>
                        {
                            bindingIndex = bindingIndices[bindingIndex];
                            string propertyId = rendererProperty.GetBindingPropertyId(propertyType, bindingIndex);
                            object bindingValue = rendererProperty.GetBindingValueByReference(propertyType, bindingIndex, animationIndex, frameIndex);
                            object propertyValue = CoreGUILayout.MaterialPropertySerializedReferenceField(propertyId, propertyType, bindingValue);
                            rendererProperty.SetBindingValueByReference(propertyType, bindingIndex, animationIndex, frameIndex, propertyValue);
                        }
                        );

                    if (i < (int)MaterialProperties.Type.Count - 1) EditorGUILayout.Space();
                }
            }
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            RendererAnimatorProperty rendererProperty = (RendererAnimatorProperty)this.Property;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                for (int i = 0; i < (int)MaterialProperties.Type.Count; i++)
                {
                    MaterialProperties.Type propertyType = (MaterialProperties.Type)i;
                    List<int> bindingIndices = this.GetBindingIndicesWithCadence(propertyType, 2);
                    UtilityGUILayout.IndexedList(
                        propertyType.ToString() + " Material Properties",
                        bindingIndices.Count,
                        null,
                        null,
                        (int bindingIndex) =>
                        {
                            bindingIndex = bindingIndices[bindingIndex];
                            string propertyId = rendererProperty.GetBindingPropertyId(propertyType, bindingIndex);
                            object bindingValue = rendererProperty.GetBindingValueByReference(propertyType, bindingIndex, animationIndex, frameIndex, orientationIndex);
                            object propertyValue = CoreGUILayout.MaterialPropertySerializedReferenceField(propertyId, propertyType, bindingValue);
                            rendererProperty.SetBindingValueByReference(propertyType, bindingIndex, animationIndex, frameIndex, orientationIndex, propertyValue);
                        }
                        );

                    if (i < (int)MaterialProperties.Type.Count - 1) EditorGUILayout.Space();
                }
            }
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        private List<int> GetBindingIndicesWithCadence(MaterialProperties.Type propertyType, int cadence)
        {
            RendererAnimatorProperty rendererProperty = (RendererAnimatorProperty)this.Property;
            List<int> bindingIndices = new List<int>();
            for (int bindingIndex = 0; bindingIndex < rendererProperty.GetNumberBindings(propertyType); bindingIndex++)
            {
                if (rendererProperty.GetBindingCadence(propertyType, bindingIndex) == cadence)
                {
                    bindingIndices.Add(bindingIndex);
                }
            }
            return bindingIndices;
        }
    }
}
#endif

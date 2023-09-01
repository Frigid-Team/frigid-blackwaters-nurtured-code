#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Core;
using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class RendererAnimatorToolPropertyDrawer : SortingOrderedAnimatorToolPropertyDrawer
    {
        public override void DrawGeneralEditFields()
        {
            base.DrawGeneralEditFields();
            RendererAnimatorProperty rendererProperty = (RendererAnimatorProperty)this.Property;
            rendererProperty.OriginalMaterial = (Material)EditorGUILayout.ObjectField("Material", rendererProperty.OriginalMaterial, typeof(Material), false);
        }

        public override void DrawAnimationEditFields(int animationIndex)
        {
            RendererAnimatorProperty rendererProperty = (RendererAnimatorProperty)this.Property;
            rendererProperty.SetEnableOutline(animationIndex, EditorGUILayout.Toggle("Enable Outline", rendererProperty.GetEnableOutline(animationIndex)));
            base.DrawAnimationEditFields(animationIndex);
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            RendererAnimatorProperty rendererProperty = (RendererAnimatorProperty)this.Property;
            UtilityGUILayout.IndexedList(
                "Material Tweens In Frame",
                rendererProperty.GetNumberMaterialTweensInFrame(animationIndex, frameIndex),
                (int index) => rendererProperty.AddMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index),
                (int index) => rendererProperty.RemoveMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index),
                (int index) =>
                {
                    MaterialTweenOptionSetSerializedReference materialTween = CoreGUILayout.MaterialTweenTemplateSerializedReferenceField(string.Format("Tween [{0}]", index), rendererProperty.GetMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index));
                    rendererProperty.SetMaterialTweenInFrameByReferenceAt(animationIndex, frameIndex, index, materialTween);
                }
                );
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }
    }
}
#endif

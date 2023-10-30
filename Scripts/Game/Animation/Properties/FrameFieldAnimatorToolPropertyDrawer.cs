#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class FrameFieldAnimatorToolPropertyDrawer<P> : ParameterAnimatorToolPropertyDrawer<P> where P : AnimatorProperty
    {
        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            FrameFieldAnimatorProperty<P> frameFieldProperty = (FrameFieldAnimatorProperty<P>)this.Property;
            for (int propertyIndex = 0; propertyIndex < frameFieldProperty.GetNumberParameteredProperties(); propertyIndex++)
            {
                P parameteredProperty = frameFieldProperty.GetParameteredProperty(propertyIndex);

                if (parameteredProperty == null) continue;

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(parameteredProperty.PropertyName, EditorStyles.largeLabel);
                    bool isValueControlled = EditorGUILayout.Toggle("Is Value Controlled", frameFieldProperty.GetIsValueControlled(propertyIndex, animationIndex, frameIndex));
                    frameFieldProperty.SetIsValueControlled(propertyIndex, animationIndex, frameIndex, isValueControlled);
                }
            }
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            FrameFieldAnimatorProperty<P> frameFieldProperty = (FrameFieldAnimatorProperty<P>)this.Property;
            cellSize.y /= frameFieldProperty.GetNumberParameteredProperties();
            Rect fillPosition = new Rect(Vector2.zero, cellSize);
            for (int propertyIndex = 0; propertyIndex < frameFieldProperty.GetNumberParameteredProperties(); propertyIndex++)
            {
                if (frameFieldProperty.GetIsValueControlled(propertyIndex, animationIndex, frameIndex))
                {
                    using (new UtilityGUI.ColorScope(UtilityGUIUtility.Darken(this.AccentColor, propertyIndex)))
                    {
                        UtilityGUI.DrawSolidBox(fillPosition);
                    }
                }
                fillPosition = new Rect(fillPosition.position + new Vector2(0, fillPosition.size.y), fillPosition.size);
            }
            base.DrawFrameCellPreview(cellSize, animationIndex, frameIndex);
        }
    }
}
#endif

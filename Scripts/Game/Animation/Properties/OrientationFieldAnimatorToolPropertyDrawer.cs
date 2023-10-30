#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class OrientationFieldAnimatorToolPropertyDrawer<P> : ParameterAnimatorToolPropertyDrawer<P> where P : AnimatorProperty
    {
        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            OrientationFieldAnimatorProperty<P> orientationFieldProperty = (OrientationFieldAnimatorProperty<P>)this.Property;
            for (int propertyIndex = 0; propertyIndex < orientationFieldProperty.GetNumberParameteredProperties(); propertyIndex++)
            {
                P parameteredProperty = orientationFieldProperty.GetParameteredProperty(propertyIndex);

                if (parameteredProperty == null) continue;

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(parameteredProperty.PropertyName, EditorStyles.largeLabel);
                    bool isValueControlled = EditorGUILayout.Toggle("Is Value Controlled", orientationFieldProperty.GetIsValueControlled(propertyIndex, animationIndex, frameIndex, orientationIndex));
                    orientationFieldProperty.SetIsValueControlled(propertyIndex, animationIndex, frameIndex, orientationIndex, isValueControlled);
                }
            }
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawOrientationCellPreview(Vector2 cellSize, int animationIndex, int frameIndex, int orientationIndex)
        {
            OrientationFieldAnimatorProperty<P> orientationFieldProperty = (OrientationFieldAnimatorProperty<P>)this.Property;
            cellSize.y /= orientationFieldProperty.GetNumberParameteredProperties();
            Rect fillPosition = new Rect(Vector2.zero, cellSize);
            for (int propertyIndex = 0; propertyIndex < orientationFieldProperty.GetNumberParameteredProperties(); propertyIndex++)
            {
                if (orientationFieldProperty.GetIsValueControlled(propertyIndex, animationIndex, frameIndex, orientationIndex))
                {
                    using (new UtilityGUI.ColorScope(UtilityGUIUtility.Darken(this.AccentColor, propertyIndex)))
                    {
                        UtilityGUI.DrawSolidBox(fillPosition);
                    }
                }
                fillPosition = new Rect(fillPosition.position + new Vector2(0, fillPosition.size.y), fillPosition.size);
            }
            base.DrawOrientationCellPreview(cellSize, animationIndex, frameIndex, orientationIndex);
        }
    }
}
#endif

#if UNITY_EDITOR
using UnityEditor;

namespace FrigidBlackwaters.Game
{
    public abstract class AnimationFieldAnimatorToolPropertyDrawer<P> : ParameterAnimatorToolPropertyDrawer<P> where P : AnimatorProperty
    {
        public override void DrawAnimationEditFields(int animationIndex)
        {
            AnimationFieldAnimatorProperty<P> animationFieldProperty = (AnimationFieldAnimatorProperty<P>)this.Property;
            for (int propertyIndex = 0; propertyIndex < animationFieldProperty.GetNumberParameteredProperties(); propertyIndex++)
            {
                P parameteredProperty = animationFieldProperty.GetParameteredProperty(propertyIndex);

                if (parameteredProperty == null) continue;

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(parameteredProperty.PropertyName, EditorStyles.largeLabel);
                    bool isValueControlled = EditorGUILayout.Toggle("Is Value Controlled", animationFieldProperty.GetIsValueControlled(propertyIndex, animationIndex));
                    animationFieldProperty.SetIsValueControlled(propertyIndex, animationIndex, isValueControlled);
                }
            }
            base.DrawAnimationEditFields(animationIndex);
        }
    }
}
#endif

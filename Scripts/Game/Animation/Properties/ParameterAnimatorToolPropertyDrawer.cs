#if UNITY_EDITOR
using UnityEditor;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    public abstract class ParameterAnimatorToolPropertyDrawer<P> : AnimatorToolPropertyDrawer where P : AnimatorProperty
    {
        public override void DrawGeneralEditFields()
        {
            ParameterAnimatorProperty<P> parameterProperty = (ParameterAnimatorProperty<P>)this.Property;
            UtilityGUILayout.IndexedList(
                "Parametered Properties",
                parameterProperty.GetNumberParameteredProperties(),
                (int propertyIndex) => parameterProperty.AddParameteredPropertyAt(propertyIndex),
                (int propertyIndex) => parameterProperty.RemoveParameteredPropertyAt(propertyIndex),
                (int propertyIndex) =>
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        parameterProperty.SetParameteredProperty(propertyIndex, (P)EditorGUILayout.ObjectField(parameterProperty.GetParameteredProperty(propertyIndex), typeof(P), true));
                        this.DrawParameteredPropertyEditFields(propertyIndex);
                    }
                }
                );
            base.DrawGeneralEditFields();
        }

        public virtual void DrawParameteredPropertyEditFields(int propertyIndex) { }
    }
}
#endif

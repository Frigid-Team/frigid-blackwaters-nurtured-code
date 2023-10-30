#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class InterpAnimatorToolPropertyDrawer<P> : ParameterAnimatorToolPropertyDrawer<P> where P : AnimatorProperty
    {
        public override void DrawAnimationEditFields(int animationIndex)
        {
            InterpAnimatorProperty<P> interpProperty = (InterpAnimatorProperty<P>)this.Property;
            for (int propertyIndex = 0; propertyIndex < interpProperty.GetNumberParameteredProperties(); propertyIndex++)
            {
                P parameteredProperty = interpProperty.GetParameteredProperty(propertyIndex);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(parameteredProperty.PropertyName, EditorStyles.largeLabel);
                    UtilityGUILayout.IndexedList(
                        "Frame Ranges",
                        interpProperty.GetNumberCurvesAndRanges(propertyIndex, animationIndex),
                        (int curveAndRangeIndex) => interpProperty.AddCurveAndRangeAt(propertyIndex, animationIndex, curveAndRangeIndex),
                        (int curveAndRangeIndex) => interpProperty.RemoveCurveAndRangeAt(propertyIndex, animationIndex, curveAndRangeIndex),
                        (int curveAndRangeIndex) =>
                        {
                            SerializedValueTuple<AnimationCurveSerializedReference, Span<int>> curveAndRange = interpProperty.GetCurveAndRange(propertyIndex, animationIndex, curveAndRangeIndex);
                            using (new EditorGUILayout.VerticalScope())
                            {
                                curveAndRange.Item1 = CoreGUILayout.CurveSerializedReferenceField(string.Empty, curveAndRange.Item1);
                                curveAndRange.Item2 = CoreGUILayout.IntSliderSpanField(curveAndRange.Item2, 0, this.Body.GetFrameCount(animationIndex) - 1);
                            }
                            interpProperty.SetCurveAndRange(propertyIndex, animationIndex, curveAndRangeIndex, curveAndRange);
                        }
                        );
                }
            }
            base.DrawAnimationEditFields(animationIndex);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            InterpAnimatorProperty<P> interpProperty = (InterpAnimatorProperty<P>)this.Property;
            cellSize.y /= interpProperty.GetNumberParameteredProperties();
            Rect fillPosition = new Rect(Vector2.zero, cellSize);
            for (int propertyIndex = 0; propertyIndex < interpProperty.GetNumberParameteredProperties(); propertyIndex++)
            {
                for (int curveAndRangeIndex = 0; curveAndRangeIndex < interpProperty.GetNumberCurvesAndRanges(propertyIndex, animationIndex); curveAndRangeIndex++)
                {
                    Span<int> range = interpProperty.GetCurveAndRange(propertyIndex, animationIndex, curveAndRangeIndex).Item2;
                    if (frameIndex >= range.Min && frameIndex <= range.Max)
                    {
                        using (new UtilityGUI.ColorScope(UtilityGUIUtility.Darken(this.AccentColor, propertyIndex)))
                        {
                            UtilityGUI.DrawSolidBox(fillPosition);
                        }
                    }
                }
                fillPosition = new Rect(fillPosition.position + new Vector2(0, fillPosition.size.y), fillPosition.size);
            }
            base.DrawFrameCellPreview(cellSize, animationIndex, frameIndex);
        }
    }
}
#endif

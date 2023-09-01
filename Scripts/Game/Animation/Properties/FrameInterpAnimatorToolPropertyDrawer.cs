#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class FrameInterpAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override void DrawAnimationEditFields(int animationIndex)
        {
            FrameInterpAnimatorProperty frameInterpProperty = (FrameInterpAnimatorProperty)this.Property;
            UtilityGUILayout.IndexedList(
                "Interpolated Properties",
                frameInterpProperty.GetNumberInterpolatedProperties(animationIndex),
                (int index) => frameInterpProperty.AddInterpolatedPropertyAt(animationIndex, index),
                (int index) => frameInterpProperty.RemoveInterpolatedPropertyAt(animationIndex, index),
                (int index) =>
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        SerializedValueTuple<AnimatorProperty, AnimationCurveSerializedReference> interpolatedProperty = frameInterpProperty.GetInterpolatedProperty(animationIndex, index);
                        interpolatedProperty.Item1 = (AnimatorProperty)EditorGUILayout.ObjectField("Property", interpolatedProperty.Item1, typeof(AnimatorProperty), true);
                        interpolatedProperty.Item2 = CoreGUILayout.CurveSerializedReferenceField("Curve", interpolatedProperty.Item2);
                        frameInterpProperty.SetInterpolatedProperty(animationIndex, index, interpolatedProperty);
                    }
                }
                );
            UtilityGUILayout.IndexedList(
                "Frame Ranges",
                frameInterpProperty.GetNumberFrameRanges(animationIndex),
                (int index) => frameInterpProperty.AddFrameRangeAt(animationIndex, index),
                (int index) => frameInterpProperty.RemoveFrameRangeAt(animationIndex, index),
                (int index) =>
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        SerializedValueTuple<int, int> frameRange = frameInterpProperty.GetFrameRange(animationIndex, index);
                        frameRange.Item1 = EditorGUILayout.IntSlider("Starting Frame Number", frameRange.Item1 + 1, 1, Mathf.Min(frameRange.Item2 + 1, this.Body.GetFrameCount(animationIndex))) - 1;
                        frameRange.Item2 = EditorGUILayout.IntSlider("Finishing Frame Number", frameRange.Item2 + 1, Mathf.Max(frameRange.Item1 + 1, 1), this.Body.GetFrameCount(animationIndex)) - 1;
                        frameInterpProperty.SetFrameRange(animationIndex, index, frameRange);
                    }
                }
                );
            base.DrawAnimationEditFields(animationIndex);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            FrameInterpAnimatorProperty frameInterpProperty = (FrameInterpAnimatorProperty)this.Property;
            for (int rangeIndex = 0; rangeIndex < frameInterpProperty.GetNumberFrameRanges(animationIndex); rangeIndex++)
            {
                SerializedValueTuple<int, int> frameRange = frameInterpProperty.GetFrameRange(animationIndex, rangeIndex);

                if (frameIndex >= frameRange.Item1 && frameIndex <= frameRange.Item2)
                {
                    using (new UtilityGUI.ColorScope(this.AccentColor))
                    {
                        UtilityGUI.DrawSolidBox(new Rect(Vector2.zero, cellSize));
                    }
                    if (frameRange.Item1 == frameIndex || frameRange.Item2 == frameIndex)
                    {
                        using (new UtilityGUI.ColorScope(this.Config.LightColor))
                        {
                            GUI.DrawTexture(new Rect(Vector2.zero, cellSize), this.Config.CellPreviewDiamondTexture);
                        }
                    }
                }
            }
            base.DrawFrameCellPreview(cellSize, animationIndex, frameIndex);
        }
    }
}
#endif

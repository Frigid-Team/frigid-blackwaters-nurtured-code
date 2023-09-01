#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    public class CoreGUILayout
    {
        public static MaterialTweenOptionSetSerializedReference MaterialTweenTemplateSerializedReferenceField(string label, MaterialTweenOptionSetSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<MaterialTweenOptionSetSerializedReference, MaterialTweenOptionSet>(
                label,
                serializedReference,
                (string label, MaterialTweenOptionSet value) => { return MaterialTweenOptionSetField(label, value); }
                );
        }

        public static MaterialTweenOptionSet MaterialTweenOptionSetField(string label, MaterialTweenOptionSet template)
        {
            if (template == null) return null;

            TweenOptionSet tweenRoutine = template.TweenRoutine;
            bool setToOriginValueAfterIteration = template.SetToOriginValueAfterIteration;
            MaterialProperties.ColorProperty colorProperty = template.ColorProperty;
            Color originColor = template.OriginColor;
            Color targetColor = template.TargetColor;

            using (new EditorGUILayout.VerticalScope())
            {
                if (!string.IsNullOrWhiteSpace(label))
                {
                    EditorGUILayout.LabelField(label);
                }
                tweenRoutine = TweenOptionSetField("Tween Routine", tweenRoutine);
                setToOriginValueAfterIteration = EditorGUILayout.Toggle("Set To Origin Value After Iteration", setToOriginValueAfterIteration);
                colorProperty = (MaterialProperties.ColorProperty)EditorGUILayout.EnumPopup("Color Parameter", colorProperty);
                originColor = EditorGUILayout.ColorField("Origin Color", originColor);
                targetColor = EditorGUILayout.ColorField("Target Color", targetColor);
            }

            return new MaterialTweenOptionSet(tweenRoutine, setToOriginValueAfterIteration, colorProperty, originColor, targetColor);
        } 

        public static TweenOptionSetSerializedReference TweenOptionSetSerializedReferenceField(string label, TweenOptionSetSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<TweenOptionSetSerializedReference, TweenOptionSet>(
                label,
                serializedReference,
                (string label, TweenOptionSet value) => { return TweenOptionSetField(label, value); }
                );
        }

        public static TweenOptionSet TweenOptionSetField(string label, TweenOptionSet template)
        {
            if (template == null) return null;

            FloatSerializedReference iterationDuration = template.IterationDurationByReference;
            EasingType easingType = template.EasingType;
            bool loopInfinitely = template.LoopInfinitely;
            IntSerializedReference additionalNumberIterations = template.AdditionalNumberIterationsByReference;
            FloatSerializedReference initialElapsedDuration = template.InitialElapsedDurationByReference;
            FloatSerializedReference durationBetweenIterations = template.DurationBetweenIterationsByReference;
            bool pingPong = template.PingPong;

            using (new EditorGUILayout.VerticalScope())
            {
                if (!string.IsNullOrWhiteSpace(label))
                {
                    EditorGUILayout.LabelField(label);
                }
                iterationDuration = FloatSerializedReferenceField("Iteration Duration", iterationDuration);
                easingType = (EasingType)EditorGUILayout.EnumPopup("Easing Type", easingType);
                loopInfinitely = EditorGUILayout.Toggle("Loop Infinitely", loopInfinitely);
                if (!loopInfinitely)
                {
                    additionalNumberIterations = IntSerializedReferenceField("Additional Number Iterations", additionalNumberIterations);
                }
                initialElapsedDuration = FloatSerializedReferenceField("Initial Elapsed Duration", initialElapsedDuration);
                durationBetweenIterations = FloatSerializedReferenceField("Duration Between Iterations", durationBetweenIterations);
                pingPong = EditorGUILayout.Toggle("Ping Pong", pingPong);
            }

            return new TweenOptionSet(iterationDuration, easingType, loopInfinitely, additionalNumberIterations, initialElapsedDuration, durationBetweenIterations, pingPong);
        }

        public static IntSerializedReference IntSerializedReferenceField(string label, IntSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<IntSerializedReference, int>(
                label,
                serializedReference,
                (string label, int value) => { return EditorGUILayout.IntField(label, value); },
                () => { return new int[] { serializedReference.LowerValue, serializedReference.UpperValue }; },
                () => { return new int[] { EditorGUILayout.IntField("Lower Value", serializedReference.LowerValue), EditorGUILayout.IntField("Upper Value", serializedReference.UpperValue) }; }
                );
        }

        public static FloatSerializedReference FloatSerializedReferenceField(string label, FloatSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<FloatSerializedReference, float>(
                label,
                serializedReference,
                (string label, float value) => { return EditorGUILayout.FloatField(label, value); },
                () => { return new float[] { serializedReference.LowerValue, serializedReference.UpperValue }; },
                () => { return new float[] { EditorGUILayout.FloatField("Lower Value", serializedReference.LowerValue), EditorGUILayout.FloatField("Upper Value", serializedReference.UpperValue) }; }
                );
        }

        public static StringSerializedReference StringSerializedReferenceField(string label, StringSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<StringSerializedReference, string>(
                label,
                serializedReference,
                (string label, string value) => { return EditorGUILayout.TextField(label, value); }
                );
        }

        public static AnimationCurveSerializedReference CurveSerializedReferenceField(string label, AnimationCurveSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<AnimationCurveSerializedReference, AnimationCurve>(
                label,
                serializedReference,
                (string label, AnimationCurve value) => { return EditorGUILayout.CurveField(label, value); }
                );
        }

        public static ColorSerializedReference ColorSerializedReferenceField(string label, ColorSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<ColorSerializedReference, Color>(
                label,
                serializedReference,
                (string label, Color value) => { return EditorGUILayout.ColorField(label, value); }
                );
        }

        public static SF ObjectSerializedReferenceField<SF, T>(string label, SF serializedReference) where SF : SerializedReference<T> where T : UnityEngine.Object
        {
            return UtilityGUILayout.SerializedReferenceField<SF, T>(
                label,
                serializedReference,
                (string label, T value) => { return (T)EditorGUILayout.ObjectField(label, value, typeof(T), false, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight)); }
                );
        }

        public static Vector2SerializedReference Vector2SerializedReferenceField(string label, Vector2SerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<Vector2SerializedReference, Vector2>(
                label,
                serializedReference,
                (string label, Vector2 value) => { return EditorGUILayout.Vector2Field(label, value); }
                );
        }

        public static Span<int> IntSpanField(Span<int> span)
        {
            return SpanField(span, (int first) => EditorGUILayout.IntField(first), (int second) => EditorGUILayout.IntField(second));
        }

        public static Span<float> FloatSpanField(Span<float> span)
        {
            return SpanField(span, (float first) => EditorGUILayout.FloatField(first), (float second) => EditorGUILayout.FloatField(second));
        }

        public static Span<T> SpanField<T>(Span<T> span, Func<T, T> toDrawFirst, Func<T, T> toDrawSecond) where T : IComparable, IComparable<T>
        {
            using (new GUILayout.HorizontalScope())
            {
                return new Span<T>(toDrawFirst.Invoke(span.First), toDrawSecond.Invoke(span.Second));
            }
        }
    }
}
#endif

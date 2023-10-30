#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    public class CoreGUILayout
    {
        public static object MaterialPropertyField(string label, MaterialProperties.Type propertyType, object value)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    return EditorGUILayout.ColorField(label, (Color)value);
                case MaterialProperties.Type.Boolean:
                    return EditorGUILayout.Toggle(label, (bool)value);
                case MaterialProperties.Type.Float:
                    return EditorGUILayout.FloatField(label, (float)value);
            }
            return default;
        }

        public static object MaterialPropertySerializedReferenceField(string label, MaterialProperties.Type propertyType, object serializedReference)
        {
            switch (propertyType)
            {
                case MaterialProperties.Type.Color:
                    return ColorSerializedReferenceField(label, (ColorSerializedReference)serializedReference);
                case MaterialProperties.Type.Boolean:
                    return BoolSerializedReferenceField(label, (BoolSerializedReference)serializedReference);
                case MaterialProperties.Type.Float:
                    return FloatSerializedReferenceField(label, (FloatSerializedReference)serializedReference);
            }
            return default;
        }

        public static MaterialTweenOptionSetSerializedReference MaterialTweenTemplateSerializedReferenceField(string label, MaterialTweenOptionSetSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<MaterialTweenOptionSetSerializedReference, MaterialTweenOptionSet>(
                label,
                serializedReference,
                (string label, MaterialTweenOptionSet value) => { return MaterialTweenOptionSetField(label, value); }
                );
        }

        public static MaterialTweenOptionSet MaterialTweenOptionSetField(string label, MaterialTweenOptionSet materialTweenOptionSet)
        {
            if (materialTweenOptionSet == null) return null;

            TweenOptionSet tweenRoutine = materialTweenOptionSet.TweenRoutine;
            bool setToOriginValueAfterIteration = materialTweenOptionSet.SetToOriginValueAfterIteration;
            string propertyId = materialTweenOptionSet.PropertyId;
            MaterialProperties.Type propertyType = materialTweenOptionSet.PropertyType;
            Color originColor = materialTweenOptionSet.OriginColor;
            Color targetColor = materialTweenOptionSet.TargetColor;
            float originFloat = materialTweenOptionSet.OriginFloat;
            float targetFloat = materialTweenOptionSet.TargetFloat;

            using (new EditorGUILayout.VerticalScope())
            {
                if (!string.IsNullOrWhiteSpace(label))
                {
                    EditorGUILayout.LabelField(label);
                }
                tweenRoutine = TweenOptionSetField("Tween Routine", tweenRoutine);
                setToOriginValueAfterIteration = EditorGUILayout.Toggle("Set To Origin Value After Iteration", setToOriginValueAfterIteration);

                propertyId = EditorGUILayout.TextField("PropertyID", propertyId);
                propertyType = (MaterialProperties.Type)EditorGUILayout.EnumPopup("Color Parameter", propertyType);
                switch (propertyType)
                {
                    case MaterialProperties.Type.Color:
                        originColor = EditorGUILayout.ColorField("Origin Color", originColor);
                        targetColor = EditorGUILayout.ColorField("Target Color", targetColor);
                        break;
                    case MaterialProperties.Type.Float:
                        originFloat = EditorGUILayout.FloatField("Origin Float", originFloat);
                        targetFloat = EditorGUILayout.FloatField("Target Float", targetFloat);
                        break;
                }
            }

            return new MaterialTweenOptionSet(tweenRoutine, setToOriginValueAfterIteration, propertyId, propertyType, originColor, targetColor, originFloat, targetFloat);
        } 

        public static TweenOptionSetSerializedReference TweenOptionSetSerializedReferenceField(string label, TweenOptionSetSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<TweenOptionSetSerializedReference, TweenOptionSet>(
                label,
                serializedReference,
                (string label, TweenOptionSet value) => { return TweenOptionSetField(label, value); }
                );
        }

        public static TweenOptionSet TweenOptionSetField(string label, TweenOptionSet tweenOptionSet)
        {
            if (tweenOptionSet == null) return null;

            FloatSerializedReference iterationDuration = tweenOptionSet.IterationDurationByReference;
            EasingType easingType = tweenOptionSet.EasingType;
            bool loopInfinitely = tweenOptionSet.LoopInfinitely;
            IntSerializedReference additionalNumberIterations = tweenOptionSet.AdditionalNumberIterationsByReference;
            FloatSerializedReference initialElapsedDuration = tweenOptionSet.InitialElapsedDurationByReference;
            FloatSerializedReference durationBetweenIterations = tweenOptionSet.DurationBetweenIterationsByReference;
            bool pingPong = tweenOptionSet.PingPong;

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

        public static BoolSerializedReference BoolSerializedReferenceField(string label, BoolSerializedReference serializedReference)
        {
            return UtilityGUILayout.SerializedReferenceField<BoolSerializedReference, bool>(
                label,
                serializedReference,
                (string label, bool value) => { return EditorGUILayout.Toggle(label, value); ; }
                );
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

        public static Span<int> IntSliderSpanField(Span<int> span, int min, int max)
        {
            return SpanField<int>(span, (int first) => EditorGUILayout.IntSlider(first, min, max), (int second) => EditorGUILayout.IntSlider(second, min, max));
        }

        public static Span<float> FloatSliderSpanField(Span<float> span, float min, float max)
        {
            return SpanField<float>(span, (float first) => EditorGUILayout.Slider(first, min, max), (float second) => EditorGUILayout.Slider(second, min, max));
        }

        public static Span<int> IntSpanField(Span<int> span)
        {
            return SpanField<int>(span, (int first) => EditorGUILayout.IntField(first), (int second) => EditorGUILayout.IntField(second));
        }

        public static Span<float> FloatSpanField(Span<float> span)
        {
            return SpanField<float>(span, (float first) => EditorGUILayout.FloatField(first), (float second) => EditorGUILayout.FloatField(second));
        }

        private static Span<T> SpanField<T>(Span<T> span, Func<T, T> toDrawFirst, Func<T, T> toDrawSecond) where T : IComparable, IComparable<T>
        {
            using (new GUILayout.HorizontalScope())
            {
                return new Span<T>(toDrawFirst.Invoke(span.First), toDrawSecond.Invoke(span.Second));
            }
        }
    }
}
#endif

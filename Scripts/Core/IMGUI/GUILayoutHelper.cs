#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    public class GUILayoutHelper
    {
        public static MaterialTweenCoroutineTemplateSerializedReference MaterialTweenTemplateSerializedReferenceField(string label, MaterialTweenCoroutineTemplateSerializedReference serializedReference)
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<MaterialTweenCoroutineTemplateSerializedReference, MaterialTweenCoroutineTemplate>(
                label,
                serializedReference,
                (string label, MaterialTweenCoroutineTemplate value) => { return MaterialTweenCoroutineTemplateField(label, value); },
                () => { return null; },
                () => { return null; }
                );
        }

        public static MaterialTweenCoroutineTemplate MaterialTweenCoroutineTemplateField(string label, MaterialTweenCoroutineTemplate template)
        {
            if (template == null) return null;

            TweenCoroutineTemplate tweenRoutine = template.TweenRoutine;
            bool setToOriginValueAfterIteration = template.SetToOriginValueAfterIteration;
            MaterialParameters.ColorParameter colorParameter = template.ColorParameter;
            Color originColor = template.OriginColor;
            Color targetColor = template.TargetColor;

            using (new EditorGUILayout.VerticalScope())
            {
                if (!string.IsNullOrWhiteSpace(label))
                {
                    EditorGUILayout.LabelField(label);
                }
                tweenRoutine = TweenCoroutineTemplateField("Tween Routine", tweenRoutine);
                setToOriginValueAfterIteration = EditorGUILayout.Toggle("Set To Origin Value After Iteration", setToOriginValueAfterIteration);
                colorParameter = (MaterialParameters.ColorParameter)EditorGUILayout.EnumPopup("Color Parameter", colorParameter);
                originColor = EditorGUILayout.ColorField("Origin Color", originColor);
                targetColor = EditorGUILayout.ColorField("Target Color", targetColor);
            }

            return new MaterialTweenCoroutineTemplate(tweenRoutine, setToOriginValueAfterIteration, colorParameter, originColor, targetColor);
        } 

        public static TweenCoroutineTemplateSerializedReference TweenCoroutineTemplateSerializedReferenceField(string label, TweenCoroutineTemplateSerializedReference serializedReference)
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<TweenCoroutineTemplateSerializedReference, TweenCoroutineTemplate>(
                label,
                serializedReference,
                (string label, TweenCoroutineTemplate value) => { return TweenCoroutineTemplateField(label, value); },
                () => { return null; },
                () => { return null; }
                );
        }

        public static TweenCoroutineTemplate TweenCoroutineTemplateField(string label, TweenCoroutineTemplate template)
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

            return new TweenCoroutineTemplate(iterationDuration, easingType, loopInfinitely, additionalNumberIterations, initialElapsedDuration, durationBetweenIterations, pingPong);
        }

        public static IntSerializedReference IntSerializedReferenceField(string label, IntSerializedReference serializedReference)
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<IntSerializedReference, int>(
                label,
                serializedReference,
                (string label, int value) => { return EditorGUILayout.IntField(label, value); },
                () => { return new int[] { serializedReference.LowerValue, serializedReference.UpperValue }; },
                () => { return new int[] { EditorGUILayout.IntField("Lower Value", serializedReference.LowerValue), EditorGUILayout.IntField("Upper Value", serializedReference.UpperValue) }; }
                );
        }

        public static FloatSerializedReference FloatSerializedReferenceField(string label, FloatSerializedReference serializedReference)
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<FloatSerializedReference, float>(
                label,
                serializedReference,
                (string label, float value) => { return EditorGUILayout.FloatField(label, value); },
                () => { return new float[] { serializedReference.LowerValue, serializedReference.UpperValue }; },
                () => { return new float[] { EditorGUILayout.FloatField("Lower Value", serializedReference.LowerValue), EditorGUILayout.FloatField("Upper Value", serializedReference.UpperValue) }; }
                );
        }

        public static StringSerializedReference StringSerializedReferenceField(string label, StringSerializedReference serializedReference)
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<StringSerializedReference, string>(
                label,
                serializedReference,
                (string label, string value) => { return EditorGUILayout.TextField(label, value); },
                () => { return null; },
                () => { return null; }
                );
        }

        public static AnimationCurveSerializedReference CurveSerializedReferenceField(string label, AnimationCurveSerializedReference serializedReference)
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<AnimationCurveSerializedReference, AnimationCurve>(
                label,
                serializedReference,
                (string label, AnimationCurve value) => { return EditorGUILayout.CurveField(label, value); },
                () => { return null; },
                () => { return null; }
                );
        }

        public static ColorSerializedReference ColorSerializedReferenceField(string label, ColorSerializedReference serializedReference)
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<ColorSerializedReference, Color>(
                label,
                serializedReference,
                (string label, Color value) => { return EditorGUILayout.ColorField(label, value); },
                () => { return null; },
                () => { return null; }
                );
        }

        public static SF ObjectSerializedReferenceField<SF, T>(string label, SF serializedReference) where SF : SerializedReference<T> where T : Object
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<SF, T>(
                label,
                serializedReference,
                (string label, T value) => { return (T)EditorGUILayout.ObjectField(label, value, typeof(T), false, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight)); },
                () => { return null; },
                () => { return null; }
                );
        }

        public static Vector2SerializedReference Vector2SerializedReferenceField(string label, Vector2SerializedReference serializedReference)
        {
            return Utility.GUILayoutHelper.SerializedReferenceField<Vector2SerializedReference, Vector2>(
                label,
                serializedReference,
                (string label, Vector2 value) => { return EditorGUILayout.Vector2Field(label, value); },
                () => { return null; },
                () => { return null; }
                );
        }
    }
}
#endif

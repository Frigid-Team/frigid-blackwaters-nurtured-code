#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(SubAnimatorBodyAnimatorProperty))]
    public class SubAnimatorBodyAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Sub Animator Body";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#fe9412", out Color color);
                return color;
            }
        }

        public override void DrawAnimationEditFields(int animationIndex)
        {
            SubAnimatorBodyAnimatorProperty subAnimatorBodyProperty = (SubAnimatorBodyAnimatorProperty)this.Property;
            string[] availableAnimationNames = new string[subAnimatorBodyProperty.SubBody.GetAnimationCount()];
            for (int subAnimationIndex = 0; subAnimationIndex < availableAnimationNames.Length; subAnimationIndex++)
            {
                availableAnimationNames[subAnimationIndex] = subAnimatorBodyProperty.SubBody.GetAnimationName(subAnimationIndex);
            }
            subAnimatorBodyProperty.SetSubAnimationIndex(animationIndex, EditorGUILayout.Popup("Sub Animation", subAnimatorBodyProperty.GetSubAnimationIndex(animationIndex), availableAnimationNames));
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Ignore Loop & Duration");
                subAnimatorBodyProperty.SetIgnoreSubAnimationLoopAndDuration(animationIndex, EditorGUILayout.Toggle(subAnimatorBodyProperty.GetIgnoreSubAnimationLoopAndDuration(animationIndex)));
            }
            base.DrawAnimationEditFields(animationIndex);
        }
    }
}
#endif

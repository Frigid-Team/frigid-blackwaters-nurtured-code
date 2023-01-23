#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(HurtBoxAnimatorProperty))]
    public class HurtBoxAnimatorToolPropertyDrawer : DamageReceiverBoxAnimatorToolPropertyDrawer<HurtBox, HitBox, HitInfo>
    {
        public override string LabelName
        {
            get
            {
                return "HurtBox";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#ff0000", out Color color);
                return color;
            }
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            HurtBoxAnimatorProperty hurtBoxProperty = (HurtBoxAnimatorProperty)this.Property;
            hurtBoxProperty.SetDamageMitigation(animationIndex, frameIndex, EditorGUILayout.IntField("Damage Mitigation", hurtBoxProperty.GetDamageMitigation(animationIndex, frameIndex)));
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }
    }
}
#endif

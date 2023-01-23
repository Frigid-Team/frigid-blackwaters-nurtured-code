#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(HitBoxAnimatorProperty))]
    public class HitBoxAnimatorToolPropertyDrawer : DamageDealerBoxAnimatorToolPropertyDrawer<HitBox, HurtBox, HitInfo>
    {
        public override string LabelName
        { 
            get
            {
                return "HitBox";
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
            base.DrawFrameEditFields(animationIndex, frameIndex);
            HitBoxAnimatorProperty hitBoxProperty = (HitBoxAnimatorProperty)this.Property;
            hitBoxProperty.SetDamageBonus(animationIndex, frameIndex, EditorGUILayout.IntField("Damage Bonus", hitBoxProperty.GetDamageBonus(animationIndex, frameIndex)));
        }
    }
}
#endif

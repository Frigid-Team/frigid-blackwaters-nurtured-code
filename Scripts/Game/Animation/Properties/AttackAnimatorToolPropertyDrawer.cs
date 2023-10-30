#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(AttackAnimatorProperty))]
    public class AttackAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Attack";
            }
        }

        public override Color AccentColor 
        { 
            get
            {
                ColorUtility.TryParseHtmlString("#ff3e1f", out Color color);
                return color;
            }
        }

        public override float[] CalculateChildPreviewOrders(int animationIndex, int frameIndex, int orientationIndex)
        {
            return new float[] { GUIPreviewOrder };
        }

        public override void DrawGeneralEditFields()
        {
            AttackAnimatorProperty attackProperty = (AttackAnimatorProperty)this.Property;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(attackProperty.GetAttackType().Name, EditorStyles.boldLabel);
                if (GUILayout.Button("Set Attack Type"))
                {
                    List<Type> attackTypes = TypeUtility.GetCompleteTypesDerivedFrom(typeof(Attack));
                    SearchPopup typeSelectionPopup = new SearchPopup(
                        attackTypes.Select((Type attackType) => attackType.Name).ToArray(),
                        (int typeIndex) => attackProperty.SetAttackType(attackTypes[typeIndex])
                        );
                    FrigidPopup.Show(GUILayoutUtility.GetLastRect(), typeSelectionPopup);
                }
                if (GUILayout.Button("Edit Attack"))
                {
                    FrigidPopup.Show(GUILayoutUtility.GetLastRect(), new InspectorPopup(attackProperty.gameObject).DoNotDraw(attackProperty).DoNotDraw(attackProperty.transform).DoNotMoveOrDelete(attackProperty.GetComponent<Attack>()));
                }
            }
            base.DrawGeneralEditFields();
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            AttackAnimatorProperty attackProperty = (AttackAnimatorProperty)this.Property;
            attackProperty.SetAttackBehaviour(animationIndex, frameIndex, (AttackAnimatorProperty.AttackBehaviour)EditorGUILayout.EnumPopup(attackProperty.GetAttackBehaviour(animationIndex, frameIndex)));
            using (new EditorGUI.DisabledScope(attackProperty.GetAttackBehaviour(animationIndex, frameIndex) == AttackAnimatorProperty.AttackBehaviour.NoAttack))
            {
                attackProperty.SetForceCompleteBehaviour(animationIndex, frameIndex, (AttackAnimatorProperty.ForceCompleteBehaviour)EditorGUILayout.EnumPopup(attackProperty.GetForceCompleteBehaviour(animationIndex, frameIndex)));
            }
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            AttackAnimatorProperty attackProperty = (AttackAnimatorProperty)this.Property;
            if (attackProperty.GetAttackBehaviour(animationIndex, frameIndex) != AttackAnimatorProperty.AttackBehaviour.NoAttack)
            {
                using (new UtilityGUI.ColorScope(this.AccentColor))
                {
                    GUI.DrawTexture(new Rect(Vector2.zero, cellSize), this.Config.CellPreviewDiamondTexture);
                }
            }
            base.DrawFrameCellPreview(cellSize, animationIndex, frameIndex);
        }
    }
}
#endif

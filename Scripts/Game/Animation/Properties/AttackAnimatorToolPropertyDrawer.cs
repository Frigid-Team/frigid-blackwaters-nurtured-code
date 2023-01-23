#if UNITY_EDITOR
using System;
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
            return new float[] { GUI_PREVIEW_ORDER };
        }

        public override void DrawGeneralEditFields()
        {
            AttackAnimatorProperty attackProperty = (AttackAnimatorProperty)this.Property;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(attackProperty.GetAttackType().Name, EditorStyles.boldLabel);
                if (GUILayout.Button("Set Attack Type"))
                {
                    TypeSelectionPopup typeSelectionPopup = new TypeSelectionPopup(
                        TypeUtility.GetCompleteTypesDerivedFrom(typeof(Attack)),
                        (Type selectedType) => attackProperty.SetAttackType(selectedType)
                        );
                    FrigidPopupWindow.Show(GUILayoutUtility.GetLastRect(), typeSelectionPopup);
                }
            }
            base.DrawGeneralEditFields();
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            AttackAnimatorProperty attackProperty = (AttackAnimatorProperty)this.Property;
            attackProperty.SetAttackThisFrame(animationIndex, frameIndex, EditorGUILayout.Toggle("Attack This Frame", attackProperty.GetAttackThisFrame(animationIndex, frameIndex)));
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawOrientationEditFields(int animationIndex, int frameIndex, int orientationIndex)
        {
            AttackAnimatorProperty attackProperty = (AttackAnimatorProperty)this.Property;
            attackProperty.SetLocalOffset(
                animationIndex,
                frameIndex,
                orientationIndex,
                EditorGUILayout.Vector2Field("Local Offset", attackProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex))
                );
            base.DrawOrientationEditFields(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawPreview(Vector2 previewSize, Vector2 previewOffset, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected, out List<(Rect rect, Action onDrag)> dragRequests)
        {
            AttackAnimatorProperty attackProperty = (AttackAnimatorProperty)this.Property;

            dragRequests = new List<(Rect rect, Action onDrag)>();
            if (propertySelected && attackProperty.GetAttackThisFrame(animationIndex, frameIndex))
            {
                Vector2 handleSize = new Vector2(this.Config.HandleLength, this.Config.HandleLength);
                Vector2 grabSize = new Vector2(this.Config.HandleGrabLength, this.Config.HandleGrabLength);
                Vector2 handlePosition = previewSize / 2 + attackProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex) * new Vector2(1, -1) * worldToWindowScalingFactor - handleSize / 2 + previewOffset;
                Rect handleRect = new Rect(handlePosition, handleSize);
                Rect grabRect = new Rect(handlePosition - (grabSize - handleSize) / 2, grabSize);

                Color handleColor = this.AccentColor;
                if (!grabRect.Contains(Event.current.mousePosition))
                {
                    handleColor = GUIStyling.Darken(handleColor);
                }

                using (new GUIHelper.ColorScope(handleColor))
                {
                    GUIHelper.DrawSolidBox(handleRect);
                    GUIHelper.DrawLine(handleRect.center + Vector2.up * this.Config.HandleGrabLength, handleRect.center + Vector2.down * this.Config.HandleGrabLength);
                    GUIHelper.DrawLine(handleRect.center + Vector2.left * this.Config.HandleGrabLength, handleRect.center + Vector2.right * this.Config.HandleGrabLength);
                    GUIHelper.DrawLineBox(grabRect);
                }

                dragRequests.Add(
                    (grabRect,
                    () =>
                    {
                        Vector2 newLocalOffset = attackProperty.GetLocalOffset(animationIndex, frameIndex, orientationIndex) + Event.current.delta * new Vector2(1, -1) / worldToWindowScalingFactor;
                        attackProperty.SetLocalOffset(animationIndex, frameIndex, orientationIndex, newLocalOffset);
                    })
                    );
            }

            base.DrawPreview(previewSize, previewOffset, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected, out List<(Rect rect, Action onDrag)> baseDragRequests);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            AttackAnimatorProperty attackProperty = (AttackAnimatorProperty)this.Property;
            if (attackProperty.GetAttackThisFrame(animationIndex, frameIndex))
            {
                using (new GUIHelper.ColorScope(this.AccentColor))
                {
                    GUI.DrawTexture(new Rect(Vector2.zero, cellSize), this.Config.CellPreviewDiamondTexture);
                }
            }
            base.DrawFrameCellPreview(cellSize, animationIndex, frameIndex);
        }
    }
}
#endif

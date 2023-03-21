#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(SFXAnimatorProperty))]
    public class SFXAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "SFX";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#9531ff", out Color color);
                return color;
            }
        }

        public override void DrawGeneralEditFields()
        {
            SFXAnimatorProperty sfxProperty = (SFXAnimatorProperty)this.Property;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(sfxProperty.GetSFXType().Name, EditorStyles.boldLabel);
                if (GUILayout.Button("Set SFX Type"))
                {
                    TypeSelectionPopup typeSelectionPopup = new TypeSelectionPopup(
                        TypeUtility.GetCompleteTypesDerivedFrom(typeof(Attack)),
                        (Type selectedType) => sfxProperty.SetSFXType(selectedType)
                        );
                    FrigidPopupWindow.Show(GUILayoutUtility.GetLastRect(), typeSelectionPopup);
                }
            }
            base.DrawGeneralEditFields();
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            SFXAnimatorProperty sfxProperty = (SFXAnimatorProperty)this.Property;
            sfxProperty.SetPlayedThisFrame(animationIndex, frameIndex, EditorGUILayout.Toggle("Played This Frame", sfxProperty.GetPlayedThisFrame(animationIndex, frameIndex)));
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            SFXAnimatorProperty sfxProperty = (SFXAnimatorProperty)this.Property;
            if (sfxProperty.GetPlayedThisFrame(animationIndex, frameIndex))
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

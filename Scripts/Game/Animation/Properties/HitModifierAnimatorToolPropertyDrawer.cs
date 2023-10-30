#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(HitModifierAnimatorProperty))]
    public class HitModifierAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Hit Modifier";
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

        public override void DrawGeneralEditFields()
        {
            HitModifierAnimatorProperty hitModifierProperty = (HitModifierAnimatorProperty)this.Property;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(hitModifierProperty.GetHitModifierType().Name, EditorStyles.boldLabel);
                if (GUILayout.Button("Set Hit Modifier Type"))
                {
                    List<Type> hitModifierTypes = TypeUtility.GetCompleteTypesDerivedFrom(typeof(HitModifier));
                    SearchPopup typeSelectionPopup = new SearchPopup(
                        hitModifierTypes.Select((Type type) => type.Name).ToArray(),
                        (int typeIndex) => hitModifierProperty.SetHitModifierType(hitModifierTypes[typeIndex])
                        );
                    FrigidPopup.Show(GUILayoutUtility.GetLastRect(), typeSelectionPopup);
                }
            }
            UtilityGUILayout.IndexedList(
                "HurtBox Properties",
                hitModifierProperty.GetNumberHurtBoxProperties(),
                hitModifierProperty.AddHurtBoxProperty,
                hitModifierProperty.RemoveHurtBoxProperty,
                (int propertyIndex) => hitModifierProperty.SetHurtBoxProperty(propertyIndex, (HurtBoxAnimatorProperty)EditorGUILayout.ObjectField(hitModifierProperty.GetHurtBoxProperty(propertyIndex), typeof(HurtBoxAnimatorProperty), true))
                );
            hitModifierProperty.PlayAudioWhenModified = EditorGUILayout.Toggle("Play Audio When Modified", hitModifierProperty.PlayAudioWhenModified);
            if (hitModifierProperty.PlayAudioWhenModified)
            {
                hitModifierProperty.AudioClipWhenModifiedByReference = CoreGUILayout.ObjectSerializedReferenceField<AudioClipSerializedReference, AudioClip>("Audio Clip When Modified", hitModifierProperty.AudioClipWhenModifiedByReference);
            }
            base.DrawGeneralEditFields();
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            HitModifierAnimatorProperty hitModifierProperty = (HitModifierAnimatorProperty)this.Property;
            hitModifierProperty.SetAddedThisFrame(animationIndex, frameIndex, EditorGUILayout.Toggle("Added This Frame", hitModifierProperty.GetAddedThisFrame(animationIndex, frameIndex)));
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            HitModifierAnimatorProperty hitModifierProperty = (HitModifierAnimatorProperty)this.Property;
            if (hitModifierProperty.GetAddedThisFrame(animationIndex, frameIndex))
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

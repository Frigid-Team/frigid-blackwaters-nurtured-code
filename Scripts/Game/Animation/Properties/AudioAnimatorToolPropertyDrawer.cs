#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(AudioAnimatorProperty))]
    public class AudioAnimatorToolPropertyDrawer : AnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Audio";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#ffcc00", out Color color);
                return color;
            }
        }


        public override void DrawGeneralEditFields()
        {
            AudioAnimatorProperty audioProperty = (AudioAnimatorProperty)this.Property;
            audioProperty.Loop = EditorGUILayout.Toggle("Is Looped Audio", audioProperty.Loop);
            if (audioProperty.Loop)
            {
                audioProperty.WarmingDuration = EditorGUILayout.FloatField("Warming Duration", audioProperty.WarmingDuration);
                audioProperty.MaxVolume = EditorGUILayout.FloatField("Max Volume", audioProperty.MaxVolume);
                audioProperty.AudioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", audioProperty.AudioClip, typeof(AudioClip), false);
            }
            base.DrawGeneralEditFields();
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            AudioAnimatorProperty audioProperty = (AudioAnimatorProperty)this.Property;
            if (!audioProperty.Loop)
            {
                audioProperty.SetPlayThisFrame(
                    animationIndex,
                    frameIndex,
                    EditorGUILayout.Toggle("Play This Frame", audioProperty.GetPlayThisFrame(animationIndex, frameIndex))
                    );
                if (audioProperty.GetPlayThisFrame(animationIndex, frameIndex))
                {
                    audioProperty.SetWaitForEndOfClip(animationIndex, frameIndex, EditorGUILayout.Toggle("Wait For End Of Clip", audioProperty.GetWaitForEndOfClip(animationIndex, frameIndex)));
                    audioProperty.SetAudioClipByReference(
                        animationIndex,
                        frameIndex,
                        CoreGUILayout.ObjectSerializedReferenceField<AudioClipSerializedReference, AudioClip>("Audio Clip", audioProperty.GetAudioClipByReference(animationIndex, frameIndex))
                        );
                    audioProperty.SetOnlyPlayOnFirstCycle(
                        animationIndex,
                        frameIndex,
                        EditorGUILayout.Toggle("Only Play On First Cycle", audioProperty.GetOnlyPlayOnFirstCycle(animationIndex, frameIndex))
                        );
                }
            }
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            AudioAnimatorProperty audioProperty = (AudioAnimatorProperty)this.Property;
            if (!audioProperty.Loop)
            {
                if (audioProperty.GetPlayThisFrame(animationIndex, frameIndex)) 
                {
                    using (new UtilityGUI.ColorScope(this.AccentColor))
                    {
                        GUI.DrawTexture(new Rect(Vector2.zero, cellSize), this.Config.CellPreviewDiamondTexture);
                    }
                }
            }
            base.DrawFrameCellPreview(cellSize, animationIndex, frameIndex);
        }
    }
}
#endif

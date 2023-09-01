#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Game
{
    [CustomAnimatorToolPropertyDrawer(typeof(ParticleAnimatorProperty))]
    public class ParticleAnimatorToolPropertyDrawer : RendererAnimatorToolPropertyDrawer
    {
        public override string LabelName
        {
            get
            {
                return "Particle";
            }
        }

        public override Color AccentColor
        {
            get
            {
                ColorUtility.TryParseHtmlString("#ff33cc", out Color color);
                return color;
            }
        }

        public override float[] CalculateChildPreviewOrders(int animationIndex, int frameIndex, int orientationIndex)
        {
            return base.CalculateChildPreviewOrders(animationIndex, frameIndex, orientationIndex);
        }

        public override void DrawGeneralEditFields()
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            using (new EditorGUILayout.HorizontalScope())
            {
                particleProperty.Loop = EditorGUILayout.Toggle("Is Looped Particle", particleProperty.Loop);
                if (GUILayout.Button("Edit Particles"))
                {
                    FrigidPopup.Show(
                        GUILayoutUtility.GetLastRect(),
                        new InspectorPopup(particleProperty.gameObject).DoNotDraw(particleProperty).DoNotDraw(particleProperty.transform).DoNotMoveOrDelete(particleProperty.GetComponent<ParticleSystem>()).DoNotMoveOrDelete(particleProperty.GetComponent<ParticleSystemRenderer>())
                        );
                }
            }
            base.DrawGeneralEditFields();
        }

        public override void DrawFrameEditFields(int animationIndex, int frameIndex)
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            if (!particleProperty.Loop)
            {
                particleProperty.SetPlayThisFrame(animationIndex, frameIndex, EditorGUILayout.Toggle("Play This Frame", particleProperty.GetPlayThisFrame(animationIndex, frameIndex)));
                if (particleProperty.GetPlayThisFrame(animationIndex, frameIndex))
                {
                    particleProperty.SetOnlyPlayOnFirstCycle(animationIndex, frameIndex, EditorGUILayout.Toggle("Only Play On First Cycle", particleProperty.GetOnlyPlayOnFirstCycle(animationIndex, frameIndex)));
                }
            }
            base.DrawFrameEditFields(animationIndex, frameIndex);
        }

        public override void DrawPreview(Vector2 previewSize, float worldToWindowScalingFactor, int animationIndex, int frameIndex, int orientationIndex, bool propertySelected)
        {
            (Vector2 position, float radius)[] draws = new (Vector2 position, float radius)[9];
            draws[0] = (previewSize / 2, this.Config.HandleLength);

            for (int i = 0; i < 3; i++)
            {
                float angleRad = (45f + 45f * i) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * new Vector2(1, -1);
                draws[i + 1] = (previewSize / 2 + direction * this.Config.HandleGrabLength / 2, this.Config.HandleLength / 1.25f);
            }
            for (int i = 0; i < 5; i++)
            {
                float angleRad = (45f + 22.5f * i) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * new Vector2(1, -1);
                draws[i + 4] = (previewSize / 2 + direction * this.Config.HandleGrabLength, this.Config.HandleLength / 2.5f);
            }
            using (new UtilityGUI.ColorScope(propertySelected ? this.AccentColor : UtilityGUIUtility.Darken(this.AccentColor)))
            {
                foreach ((Vector2 position, float radius) draw in draws)
                {
                    UtilityGUI.DrawSolidArc(draw.position, 0, 2 * Mathf.PI, draw.radius / 2);
                }
            }
            base.DrawPreview(previewSize, worldToWindowScalingFactor, animationIndex, frameIndex, orientationIndex, propertySelected);
        }

        public override void DrawFrameCellPreview(Vector2 cellSize, int animationIndex, int frameIndex)
        {
            ParticleAnimatorProperty particleProperty = (ParticleAnimatorProperty)this.Property;
            if (!particleProperty.Loop && particleProperty.GetPlayThisFrame(animationIndex, frameIndex))
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

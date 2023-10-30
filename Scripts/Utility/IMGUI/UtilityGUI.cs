#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FrigidBlackwaters.Utility
{
    public static class UtilityGUI
    {
        public static void DrawSprite(Rect position, Sprite sprite)
        {
            Rect texCoords = sprite.rect;

            texCoords.xMin /= sprite.texture.width;
            texCoords.xMax /= sprite.texture.width;
            texCoords.yMin /= sprite.texture.height;
            texCoords.yMax /= sprite.texture.height;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, texCoords);
        }

        public static void DrawLineArc(Vector2 center, float firstAngleRad, float secondAngleRad, float radius)
        {
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = GUI.color;
            Handles.DrawWireArc(center, Vector3.forward, new Vector2(Mathf.Cos(firstAngleRad), Mathf.Sin(firstAngleRad)), (secondAngleRad - firstAngleRad) * Mathf.Rad2Deg, radius);
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        public static void DrawSolidArc(Vector2 center, float firstAngleRad, float secondAngleRad, float radius)
        {
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = GUI.color;
            Handles.DrawSolidArc(center, Vector3.forward, new Vector2(Mathf.Cos(firstAngleRad), Mathf.Sin(firstAngleRad)), (secondAngleRad - firstAngleRad) * Mathf.Rad2Deg, radius);
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        public static void DrawLineSegments(Vector2[] points)
        {
            if (points.Length > 1)
            {
                for (int i = 0; i < points.Length - 1; i++)
                {
                    DrawLine(points[i], points[i + 1]);
                }
            }
        }

        public static void DrawLinePolygon(Vector2[] points)
        {
            if (points.Length > 2)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    DrawLine(points[i], points[(i + 1) % points.Length]);
                }
            }
        }

        public static void DrawOutlineBox(Rect position, int thickness = 1)
        {
            Handles.BeginGUI();
            for (int i = 0; i < thickness; i++)
            {
                position.xMin++;
                position.xMax--;
                position.yMin++;
                position.yMax--;
                Handles.DrawSolidRectangleWithOutline(position, Color.clear, GUI.color);
            }
            Handles.EndGUI();
        }

        public static void DrawSolidBox(Rect position)
        {
            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(position, GUI.color, Color.clear);
            Handles.EndGUI();
        }

        public static void DrawLineCapsule(Rect position)
        {
            if (position.width < position.height)
            {
                float radius = position.width / 2;
                DrawLineArc(position.position + new Vector2(radius, radius), Mathf.PI, Mathf.PI * 2, radius);
                DrawLine(position.position + new Vector2(0, radius), position.position + new Vector2(0, position.height - radius));
                DrawLine(position.position + new Vector2(radius * 2, radius), position.position + new Vector2(radius * 2, position.height - radius));
                DrawLineArc(position.position + new Vector2(radius, position.height - radius), 0, Mathf.PI, radius);
            }
            else
            {
                float radius = position.height / 2;
                DrawLineArc(position.position + new Vector2(radius, radius), 3 * Mathf.PI / 2, Mathf.PI / 2, radius);
                DrawLine(position.position + new Vector2(radius, 0), position.position + new Vector2(position.width - radius, 0));
                DrawLine(position.position + new Vector2(radius, radius * 2), position.position + new Vector2(position.width - radius, radius * 2));
                DrawLineArc(position.position + new Vector2(position.width - radius, radius), -Mathf.PI / 2, Mathf.PI / 2, radius);
            }
        }

        public static void DrawLineBox(Rect position)
        {
            Vector2[] corners = new Vector2[]
            {
                new Vector2(position.xMin, position.yMin),
                new Vector2(position.xMax, position.yMin),
                new Vector2(position.xMax, position.yMax),
                new Vector2(position.xMin, position.yMax)
            };

            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 a = corners[i];
                Vector2 b = corners[(i + 1) % corners.Length];

                DrawLine(a, b);
            }
        }

        public static void DrawLine(Vector2 a, Vector2 b)
        {
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = GUI.color;
            Handles.DrawLine(a, b);
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        public class ColorScope : GUI.Scope
        {
            private Color oldColor;

            public ColorScope(Color color)
            {
                this.oldColor = GUI.color;
                GUI.color = color;
            }

            protected override void CloseScope()
            {
                GUI.color = this.oldColor;
            }
        }

        public class IndentScope : GUI.Scope
        {
            public IndentScope()
            {
                EditorGUI.indentLevel++;
            }

            protected override void CloseScope()
            {
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif

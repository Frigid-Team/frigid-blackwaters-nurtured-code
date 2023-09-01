using UnityEngine;
using System;

namespace FrigidBlackwaters.Core
{
    public static class MaterialProperties
    {
        private const string ENABLE_OUTLINE = "_EnableOutline";

        // Color Parameters
        private const string COLOR = "_Color";
        private const string FLASH_COLOR = "_FlashColor";
        private const string OUTLINE_COLOR = "_OutlineColor";

        private static MaterialPropertyBlock buffer;

        static MaterialProperties()
        {
            buffer = new MaterialPropertyBlock();
        }

        public static bool GetEnableOutline(Renderer renderer)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            return propertyBlock.GetInt(ENABLE_OUTLINE) > 0;
        }

        public static void SetEnableOutline(Renderer renderer, bool enableOutline)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetInt(ENABLE_OUTLINE, enableOutline ? 1 : 0);
            renderer.SetPropertyBlock(propertyBlock);
        }

        public static Color GetColor(Renderer renderer, ColorProperty colorProperty)
        {
            renderer.GetPropertyBlock(buffer);
            switch (colorProperty)
            {
                case ColorProperty.Color:
                    if (!renderer.sharedMaterial.HasColor(COLOR)) break;
                    if (!buffer.HasColor(COLOR)) return renderer.sharedMaterial.GetColor(COLOR);
                    return buffer.GetColor(COLOR);
                case ColorProperty.FlashColor:
                    if (!renderer.sharedMaterial.HasColor(FLASH_COLOR)) break;
                    if (!buffer.HasColor(FLASH_COLOR)) return renderer.sharedMaterial.GetColor(FLASH_COLOR);
                    return buffer.GetColor(FLASH_COLOR);
                case ColorProperty.OutlineColor:
                    if (!renderer.sharedMaterial.HasColor(OUTLINE_COLOR)) break;
                    if (!buffer.HasColor(OUTLINE_COLOR)) return renderer.sharedMaterial.GetColor(OUTLINE_COLOR);
                    return buffer.GetColor(OUTLINE_COLOR);
           }
            return Color.white;
        }

        public static void SetColor(Renderer renderer, ColorProperty colorProperty, Color value)
        {
            renderer.GetPropertyBlock(buffer);
            switch (colorProperty)
            {
                case ColorProperty.Color:
                    buffer.SetColor(COLOR, value);
                    break;
                case ColorProperty.FlashColor:
                    buffer.SetColor(FLASH_COLOR, value);
                    break;
                case ColorProperty.OutlineColor:
                    buffer.SetColor(OUTLINE_COLOR, value);
                    break;
            }
            renderer.SetPropertyBlock(buffer);
        }

        public enum ColorProperty
        {
            Color,
            FlashColor,
            OutlineColor,
            Count
        }
    }
}

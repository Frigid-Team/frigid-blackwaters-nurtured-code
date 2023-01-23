using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class MaterialParameters
    {
        public const string ENABLE_OUTLINE = "_EnableOutline";
        public const string FLASH_COLOR = "_FlashColor";

        public static Color GetColor(Material material, ColorParameter colorParameter)
        {
            switch (colorParameter)
            {
                case ColorParameter.MainColor:
                    return material.color;
                case ColorParameter.FlashColor:
                    return material.GetColor(FLASH_COLOR);
            }
            return Color.white;
        }

        public static void SetColor(Material material, ColorParameter colorParameter, Color value)
        {
            switch (colorParameter)
            {
                case ColorParameter.MainColor:
                    material.color = value;
                    break;
                case ColorParameter.FlashColor:
                    material.SetColor(FLASH_COLOR, value);
                    break;
            }
        }

        public enum ColorParameter
        {
            MainColor,
            FlashColor,
            Count
        }
    }
}

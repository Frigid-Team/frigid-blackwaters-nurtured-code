using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Utility;

namespace FrigidBlackwaters.Core
{
    public static class MaterialProperties
    {
        private static MaterialPropertyBlock buffer;

        static MaterialProperties()
        {
            buffer = new MaterialPropertyBlock();
        }

        public static Color GetColor(Renderer renderer, string propertyId)
        {
            renderer.GetPropertyBlock(buffer);
            if (!renderer.sharedMaterial.HasColor(propertyId)) return Color.white;
            if (!buffer.HasColor(propertyId)) return renderer.sharedMaterial.GetColor(propertyId);
            return buffer.GetColor(propertyId);
        }

        public static void SetColor(Renderer renderer, string propertyId, Color value)
        {
            if (!renderer.sharedMaterial.HasColor(propertyId)) return;
            renderer.GetPropertyBlock(buffer);
            buffer.SetColor(propertyId, value);
            renderer.SetPropertyBlock(buffer);
        }

        public static bool GetBool(Renderer renderer, string propertyId)
        {
            renderer.GetPropertyBlock(buffer);
            if (!renderer.sharedMaterial.HasInt(propertyId)) return false;
            if (!buffer.HasInt(propertyId)) return renderer.sharedMaterial.GetInt(propertyId) > 0;
            return buffer.GetInt(propertyId) > 0;
        }

        public static void SetBool(Renderer renderer, string propertyId, bool value)
        {
            if (!renderer.sharedMaterial.HasInt(propertyId)) return;
            renderer.GetPropertyBlock(buffer);
            buffer.SetInt(propertyId, value ? 1 : 0);
            renderer.SetPropertyBlock(buffer);
        }

        public static float GetFloat(Renderer renderer, string propertyId)
        {
            renderer.GetPropertyBlock(buffer);
            if (!renderer.sharedMaterial.HasFloat(propertyId)) return 0;
            if (!buffer.HasFloat(propertyId)) return renderer.sharedMaterial.GetFloat(propertyId);
            return buffer.GetFloat(propertyId);
        }

        public static void SetFloat(Renderer renderer, string propertyId, float value)
        {
            if (!renderer.sharedMaterial.HasFloat(propertyId)) return;
            renderer.GetPropertyBlock(buffer);
            buffer.SetFloat(propertyId, value);
            renderer.SetPropertyBlock(buffer);
        }

        public static object GetProperty(Renderer renderer, Type propertyType, string propertyId)
        {
            switch (propertyType)
            {
                case Type.Color:
                    return GetColor(renderer, propertyId);
                case Type.Boolean:
                    return GetBool(renderer, propertyId);
                case Type.Float:
                    return GetFloat(renderer, propertyId);
            }
            return default;
        }

        public static object GetPropertyByReference(Renderer renderer, Type propertyType, string propertyId)
        {
            switch (propertyType)
            {
                case Type.Color:
                    return new ColorSerializedReference(SerializedReferenceType.Custom, GetColor(renderer, propertyId), null, new List<Color>(), null);
                case Type.Boolean:
                    return new BoolSerializedReference(SerializedReferenceType.Custom, GetBool(renderer, propertyId), null, new List<bool>(), null);
                case Type.Float:
                    return new FloatSerializedReference(SerializedReferenceType.Custom, GetFloat(renderer, propertyId), null, 0f, 0f, new List<float>(), null);
            }
            return default;
        }

        public static void SetProperty(Renderer renderer, Type propertyType, string propertyId, object value)
        {
            switch (propertyType)
            {
                case Type.Color:
                    SetColor(renderer, propertyId, (Color)value);
                    break;
                case Type.Boolean:
                    SetBool(renderer, propertyId, (bool)value);
                    break;
                case Type.Float:
                    SetFloat(renderer, propertyId, (float)value);
                    break;
            }
        }

        public static void SetPropertyByReference(Renderer renderer, Type propertyType, string propertyId, object serializedReference)
        {
            switch (propertyType)
            {
                case Type.Color:
                    SetColor(renderer, propertyId, ((ColorSerializedReference)serializedReference).MutableValue);
                    break;
                case Type.Boolean:
                    SetBool(renderer, propertyId, ((BoolSerializedReference)serializedReference).MutableValue);
                    break;
                case Type.Float:
                    SetFloat(renderer, propertyId, ((FloatSerializedReference)serializedReference).MutableValue);
                    break;
            }
        }

        public enum Type
        {
            Color,
            Boolean,
            Float,
            Count
        }
    }
}

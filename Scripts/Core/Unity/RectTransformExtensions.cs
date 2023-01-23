using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class RectTransformExtensions
    {
        public static Vector2 CalculateOutsideParentLocalPosition(this RectTransform rectTransform, RectTransform.Edge edge)
        {
            RectTransform parentRectTransform = (RectTransform)rectTransform.parent;
            switch (edge)
            {
                case RectTransform.Edge.Left:
                    return new Vector2(-(parentRectTransform.rect.width + rectTransform.rect.width) / 2, rectTransform.localPosition.y);
                case RectTransform.Edge.Right:
                    return new Vector2((parentRectTransform.rect.width + rectTransform.rect.width) / 2, rectTransform.localPosition.y);
                case RectTransform.Edge.Top:
                    return new Vector2(rectTransform.localPosition.x, (parentRectTransform.rect.height + rectTransform.rect.height) / 2);
                case RectTransform.Edge.Bottom:
                    return new Vector2(rectTransform.localPosition.x, -(parentRectTransform.rect.height + rectTransform.rect.height) / 2);
            }
            return Vector2.zero;
        }
    }
}

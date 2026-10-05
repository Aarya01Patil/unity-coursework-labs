using UnityEngine;

namespace SoulSucker.Utilities
{
    public static class PlaceholderSpriteLibrary
    {
        private static Sprite squareSprite;
        private static Sprite circleSprite;

        public static Sprite GetSquareSprite()
        {
            if (squareSprite == null)
            {
                squareSprite = CreateFilledSprite("Square", 64, IsSquarePixelFilled);
            }

            return squareSprite;
        }

        public static Sprite GetCircleSprite()
        {
            if (circleSprite == null)
            {
                circleSprite = CreateFilledSprite("Circle", 64, IsCirclePixelFilled);
            }

            return circleSprite;
        }

        private static Sprite CreateFilledSprite(string textureName, int size, PixelFillRule fillRule)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = $"Placeholder_{textureName}",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };

            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    pixels[(y * size) + x] = fillRule(x, y, size) ? Color.white : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(
                texture,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit: size);
        }

        private static bool IsSquarePixelFilled(int x, int y, int size)
        {
            return x >= 0 && y >= 0 && x < size && y < size;
        }

        private static bool IsCirclePixelFilled(int x, int y, int size)
        {
            float radius = size * 0.48f;
            float center = (size - 1) * 0.5f;
            float dx = x - center;
            float dy = y - center;
            return (dx * dx) + (dy * dy) <= radius * radius;
        }

        private delegate bool PixelFillRule(int x, int y, int size);
    }
}

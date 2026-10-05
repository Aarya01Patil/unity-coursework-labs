using System;
using UnityEngine;

namespace SoulSucker.Data
{
    [Serializable]
    public struct IntRect
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public IntRect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool IsValid => width > 0 && height > 0;

        public Vector2Int Position => new Vector2Int(x, y);

        public Vector2Int Size => new Vector2Int(width, height);
    }
}

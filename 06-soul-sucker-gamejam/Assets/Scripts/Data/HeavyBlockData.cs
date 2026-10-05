using System;
using UnityEngine;

namespace SoulSucker.Data
{
    [Serializable]
    public sealed class HeavyBlockData
    {
        public string id;
        public Vector2Int cell;
        public Vector2Int size;

        public HeavyBlockData(string id, Vector2Int cell, Vector2Int size)
        {
            this.id = id ?? string.Empty;
            this.cell = cell;
            this.size = size;
        }
    }
}

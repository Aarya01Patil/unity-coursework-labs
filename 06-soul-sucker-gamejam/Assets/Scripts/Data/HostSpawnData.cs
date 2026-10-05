using System;
using UnityEngine;

namespace SoulSucker.Data
{
    [Serializable]
    public struct HostSpawnData
    {
        public HostType type;
        public Vector2Int cell;

        public HostSpawnData(HostType type, Vector2Int cell)
        {
            this.type = type;
            this.cell = cell;
        }
    }
}

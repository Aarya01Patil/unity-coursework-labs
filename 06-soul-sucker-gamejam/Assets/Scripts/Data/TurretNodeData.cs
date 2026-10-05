using System;
using UnityEngine;

namespace SoulSucker.Data
{
    [Serializable]
    public sealed class TurretNodeData
    {
        public string id;
        public Vector2Int cell;
        public IntRect beamBounds;
        public bool startsEnabled;

        public TurretNodeData(string id, Vector2Int cell, IntRect beamBounds, bool startsEnabled = true)
        {
            this.id = id ?? string.Empty;
            this.cell = cell;
            this.beamBounds = beamBounds;
            this.startsEnabled = startsEnabled;
        }
    }
}

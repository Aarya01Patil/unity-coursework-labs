using System;
using UnityEngine;

namespace SoulSucker.Data
{
    [Serializable]
    public sealed class FanZoneData
    {
        public string id;
        public IntRect bounds;
        public Vector2 force;
        public bool startsEnabled;

        public FanZoneData(string id, IntRect bounds, Vector2 force, bool startsEnabled = true)
        {
            this.id = id ?? string.Empty;
            this.bounds = bounds;
            this.force = force;
            this.startsEnabled = startsEnabled;
        }
    }
}

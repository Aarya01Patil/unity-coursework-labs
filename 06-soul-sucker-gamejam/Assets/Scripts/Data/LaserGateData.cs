using System;

namespace SoulSucker.Data
{
    [Serializable]
    public sealed class LaserGateData
    {
        public string id;
        public IntRect bounds;
        public float activeSeconds;
        public float inactiveSeconds;
        public float phaseOffsetSeconds;
        public bool startsEnabled;

        public LaserGateData(IntRect bounds, float activeSeconds, float inactiveSeconds, float phaseOffsetSeconds = 0f, string id = "", bool startsEnabled = true)
        {
            this.id = id ?? string.Empty;
            this.bounds = bounds;
            this.activeSeconds = activeSeconds;
            this.inactiveSeconds = inactiveSeconds;
            this.phaseOffsetSeconds = phaseOffsetSeconds;
            this.startsEnabled = startsEnabled;
        }
    }
}

using System;

namespace SoulSucker.Data
{
    [Serializable]
    public sealed class PhaseGateData
    {
        public string id;
        public IntRect bounds;

        public PhaseGateData(string id, IntRect bounds)
        {
            this.id = id ?? string.Empty;
            this.bounds = bounds;
        }
    }
}

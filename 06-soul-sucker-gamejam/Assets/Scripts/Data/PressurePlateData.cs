using System;

namespace SoulSucker.Data
{
    [Serializable]
    public sealed class PressurePlateData
    {
        public string id;
        public IntRect bounds;
        public string[] targetIds;
        public bool targetsEnabledWhenPressed;

        public PressurePlateData(string id, IntRect bounds, string[] targetIds, bool targetsEnabledWhenPressed)
        {
            this.id = id ?? string.Empty;
            this.bounds = bounds;
            this.targetIds = targetIds ?? Array.Empty<string>();
            this.targetsEnabledWhenPressed = targetsEnabledWhenPressed;
        }
    }
}

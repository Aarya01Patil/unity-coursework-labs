using System;
using UnityEngine;

namespace SoulSucker.Data
{
    [Serializable]
    public sealed class RemoteSwitchData
    {
        public string id;
        public Vector2Int cell;
        public string[] targetIds;
        public bool targetsEnabledWhenTriggered;

        public RemoteSwitchData(string id, Vector2Int cell, string[] targetIds, bool targetsEnabledWhenTriggered)
        {
            this.id = id ?? string.Empty;
            this.cell = cell;
            this.targetIds = targetIds ?? Array.Empty<string>();
            this.targetsEnabledWhenTriggered = targetsEnabledWhenTriggered;
        }
    }
}

using System;
using UnityEngine;

namespace SoulSucker.Data
{
    [Serializable]
    public sealed class LevelBlueprint
    {
        public string id;
        public string title;
        public Vector2Int levelSize;
        public IntRect cameraBounds;
        public Vector2Int playerStart;
        public Vector2Int exitCell;
        public IntRect[] solids;
        public IntRect[] hazards;
        public IntRect[] breakables;
        public LaserGateData[] laserGates;
        public RemoteSwitchData[] remoteSwitches;
        public TurretNodeData[] turretNodes;
        public PhaseGateData[] phaseGates;
        public PressurePlateData[] pressurePlates;
        public HeavyBlockData[] heavyBlocks;
        public FanZoneData[] fanZones;
        public HostSpawnData[] hosts;
        public string[] tutorialHints;

        public Vector2Int roomSize => levelSize;

        public LevelBlueprint(
            string id,
            string title,
            Vector2Int levelSize,
            IntRect cameraBounds,
            Vector2Int playerStart,
            Vector2Int exitCell,
            IntRect[] solids,
            IntRect[] hazards,
            IntRect[] breakables,
            LaserGateData[] laserGates,
            RemoteSwitchData[] remoteSwitches,
            TurretNodeData[] turretNodes,
            PhaseGateData[] phaseGates,
            PressurePlateData[] pressurePlates,
            HeavyBlockData[] heavyBlocks,
            FanZoneData[] fanZones,
            HostSpawnData[] hosts,
            string[] tutorialHints)
        {
            this.id = id;
            this.title = title;
            this.levelSize = levelSize;
            this.cameraBounds = cameraBounds;
            this.playerStart = playerStart;
            this.exitCell = exitCell;
            this.solids = solids ?? Array.Empty<IntRect>();
            this.hazards = hazards ?? Array.Empty<IntRect>();
            this.breakables = breakables ?? Array.Empty<IntRect>();
            this.laserGates = laserGates ?? Array.Empty<LaserGateData>();
            this.remoteSwitches = remoteSwitches ?? Array.Empty<RemoteSwitchData>();
            this.turretNodes = turretNodes ?? Array.Empty<TurretNodeData>();
            this.phaseGates = phaseGates ?? Array.Empty<PhaseGateData>();
            this.pressurePlates = pressurePlates ?? Array.Empty<PressurePlateData>();
            this.heavyBlocks = heavyBlocks ?? Array.Empty<HeavyBlockData>();
            this.fanZones = fanZones ?? Array.Empty<FanZoneData>();
            this.hosts = hosts ?? Array.Empty<HostSpawnData>();
            this.tutorialHints = tutorialHints ?? Array.Empty<string>();
        }

        public LevelBlueprint(
            string id,
            string title,
            Vector2Int roomSize,
            Vector2Int playerStart,
            Vector2Int exitCell,
            IntRect[] solids,
            IntRect[] hazards,
            IntRect[] breakables,
            HostSpawnData[] hosts,
            string[] tutorialHints)
            : this(
                id,
                title,
                roomSize,
                new IntRect(0, 0, roomSize.x, roomSize.y),
                playerStart,
                exitCell,
                solids,
                hazards,
                breakables,
                Array.Empty<LaserGateData>(),
                Array.Empty<RemoteSwitchData>(),
                Array.Empty<TurretNodeData>(),
                Array.Empty<PhaseGateData>(),
                Array.Empty<PressurePlateData>(),
                Array.Empty<HeavyBlockData>(),
                Array.Empty<FanZoneData>(),
                hosts,
                tutorialHints)
        {
        }

        public bool HasExit()
        {
            return exitCell.x >= 0 && exitCell.y >= 0;
        }
    }
}

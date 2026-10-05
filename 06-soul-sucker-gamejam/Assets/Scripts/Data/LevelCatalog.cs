using System.Collections.Generic;
using UnityEngine;

namespace SoulSucker.Data
{
    public static class LevelCatalog
    {
        public const int ExpectedLevelHeight = 18;

        private static readonly IReadOnlyList<LevelBlueprint> roomsInternal = new[]
        {
            CreateJumperTutorial(),
            CreateDasherTutorial(),
            CreateShooterTutorial(),
            CreateAnchorTutorial(),
            CreatePhaseTutorial(),
            CreateThreeHostPuzzle(),
            CreateFourHostChain(),
            CreateFinalExam(),
        };

        public static IReadOnlyList<LevelBlueprint> Rooms => roomsInternal;

        public static LevelBlueprint GetLevel(int index)
        {
            return roomsInternal[index];
        }

        private static LevelBlueprint CreateJumperTutorial()
        {
            const int width = 32;
            List<IntRect> solids = CreateFrame(width);
            solids.Add(new IntRect(0, 0, width, 1));
            solids.Add(new IntRect(9, 3, 4, 1));
            solids.Add(new IntRect(14, 6, 4, 1));
            solids.Add(new IntRect(20, 9, 4, 1));
            solids.Add(new IntRect(25, 12, 4, 1));

            return new LevelBlueprint(
                id: "level_01_jumper",
                title: "Jumper Tutorial",
                levelSize: Size(width),
                cameraBounds: FullBounds(width),
                playerStart: new Vector2Int(3, 2),
                exitCell: new Vector2Int(27, 13),
                solids: solids.ToArray(),
                hazards: System.Array.Empty<IntRect>(),
                breakables: System.Array.Empty<IntRect>(),
                laserGates: System.Array.Empty<LaserGateData>(),
                remoteSwitches: System.Array.Empty<RemoteSwitchData>(),
                turretNodes: System.Array.Empty<TurretNodeData>(),
                phaseGates: System.Array.Empty<PhaseGateData>(),
                pressurePlates: System.Array.Empty<PressurePlateData>(),
                heavyBlocks: System.Array.Empty<HeavyBlockData>(),
                fanZones: System.Array.Empty<FanZoneData>(),
                hosts: new[]
                {
                    new HostSpawnData(HostType.Jumper, new Vector2Int(6, 2)),
                },
                tutorialHints: new[]
                {
                    "Possess the Jumper. Use short vertical bursts to climb the ledges."
                });
        }

        private static LevelBlueprint CreateDasherTutorial()
        {
            const int width = 40;
            List<IntRect> solids = CreateFrame(width);
            solids.Add(new IntRect(0, 0, width, 1));
            solids.Add(new IntRect(24, 4, 9, 1));

            return new LevelBlueprint(
                id: "level_02_dasher",
                title: "Dasher Tutorial",
                levelSize: Size(width),
                cameraBounds: FullBounds(width),
                playerStart: new Vector2Int(3, 2),
                exitCell: new Vector2Int(36, 2),
                solids: solids.ToArray(),
                hazards: System.Array.Empty<IntRect>(),
                breakables: System.Array.Empty<IntRect>(),
                laserGates: new[]
                {
                    new LaserGateData(new IntRect(28, 1, 1, 3), 1.1f, 1.0f, id: "level2_lane_gate"),
                },
                remoteSwitches: System.Array.Empty<RemoteSwitchData>(),
                turretNodes: System.Array.Empty<TurretNodeData>(),
                phaseGates: System.Array.Empty<PhaseGateData>(),
                pressurePlates: System.Array.Empty<PressurePlateData>(),
                heavyBlocks: System.Array.Empty<HeavyBlockData>(),
                fanZones: System.Array.Empty<FanZoneData>(),
                hosts: new[]
                {
                    new HostSpawnData(HostType.Dasher, new Vector2Int(6, 2)),
                },
                tutorialHints: new[]
                {
                    "Possess the Dasher. Burst across the lane, then take the upper route."
                });
        }

        private static LevelBlueprint CreateShooterTutorial()
        {
            const int width = 48;
            List<IntRect> solids = CreateFrame(width);
            solids.Add(new IntRect(0, 0, width, 1));

            return new LevelBlueprint(
                id: "level_03_shooter",
                title: "Shooter Tutorial",
                levelSize: Size(width),
                cameraBounds: FullBounds(width),
                playerStart: new Vector2Int(3, 2),
                exitCell: new Vector2Int(43, 2),
                solids: solids.ToArray(),
                hazards: System.Array.Empty<IntRect>(),
                breakables: new[]
                {
                    new IntRect(18, 1, 1, 4),
                },
                laserGates: new[]
                {
                    new LaserGateData(new IntRect(36, 1, 1, 4), 10f, 0f, id: "level3_gate"),
                },
                remoteSwitches: new[]
                {
                    new RemoteSwitchData("level3_switch", new Vector2Int(31, 2), new[] { "level3_gate" }, false),
                },
                turretNodes: System.Array.Empty<TurretNodeData>(),
                phaseGates: System.Array.Empty<PhaseGateData>(),
                pressurePlates: System.Array.Empty<PressurePlateData>(),
                heavyBlocks: System.Array.Empty<HeavyBlockData>(),
                fanZones: System.Array.Empty<FanZoneData>(),
                hosts: new[]
                {
                    new HostSpawnData(HostType.Shooter, new Vector2Int(8, 2)),
                },
                tutorialHints: new[]
                {
                    "Break the wall, then hit the switch to open the route."
                });
        }

        private static LevelBlueprint CreateAnchorTutorial()
        {
            const int width = 52;
            List<IntRect> solids = CreateFrame(width);
            solids.Add(new IntRect(0, 0, width, 1));

            return new LevelBlueprint(
                id: "level_04_anchor",
                title: "Anchor Tutorial",
                levelSize: Size(width),
                cameraBounds: FullBounds(width),
                playerStart: new Vector2Int(13, 2),
                exitCell: new Vector2Int(46, 2),
                solids: solids.ToArray(),
                hazards: System.Array.Empty<IntRect>(),
                breakables: System.Array.Empty<IntRect>(),
                laserGates: new[]
                {
                    new LaserGateData(new IntRect(24, 1, 1, 4), 10f, 0f, id: "level4_plate_gate"),
                },
                remoteSwitches: System.Array.Empty<RemoteSwitchData>(),
                turretNodes: System.Array.Empty<TurretNodeData>(),
                phaseGates: System.Array.Empty<PhaseGateData>(),
                pressurePlates: new[]
                {
                    new PressurePlateData("level4_plate", new IntRect(8, 1, 2, 1), new[] { "level4_plate_gate" }, false),
                },
                heavyBlocks: new[]
                {
                    new HeavyBlockData("level4_block", new Vector2Int(11, 1), new Vector2Int(2, 2)),
                },
                fanZones: System.Array.Empty<FanZoneData>(),
                hosts: new[]
                {
                    new HostSpawnData(HostType.Anchor, new Vector2Int(15, 2)),
                },
                tutorialHints: new[]
                {
                    "Use Anchor to push the block left onto the plate and open the gate."
                });
        }

        private static LevelBlueprint CreatePhaseTutorial()
        {
            const int width = 56;
            List<IntRect> solids = CreateFrame(width);
            solids.Add(new IntRect(0, 0, width, 1));

            return new LevelBlueprint(
                id: "level_05_phase",
                title: "Phase Tutorial",
                levelSize: Size(width),
                cameraBounds: FullBounds(width),
                playerStart: new Vector2Int(3, 2),
                exitCell: new Vector2Int(50, 2),
                solids: solids.ToArray(),
                hazards: System.Array.Empty<IntRect>(),
                breakables: System.Array.Empty<IntRect>(),
                laserGates: System.Array.Empty<LaserGateData>(),
                remoteSwitches: System.Array.Empty<RemoteSwitchData>(),
                turretNodes: System.Array.Empty<TurretNodeData>(),
                phaseGates: new[]
                {
                    new PhaseGateData("level5_phase_gate_a", new IntRect(18, 1, 1, 6)),
                    new PhaseGateData("level5_phase_gate_b", new IntRect(34, 1, 1, 6)),
                },
                pressurePlates: System.Array.Empty<PressurePlateData>(),
                heavyBlocks: System.Array.Empty<HeavyBlockData>(),
                fanZones: System.Array.Empty<FanZoneData>(),
                hosts: new[]
                {
                    new HostSpawnData(HostType.Phase, new Vector2Int(8, 2)),
                },
                tutorialHints: new[]
                {
                    "Use Phase to slip through the spirit barriers and keep moving."
                });
        }

        private static LevelBlueprint CreateThreeHostPuzzle()
        {
            const int width = 62;
            List<IntRect> solids = CreateFrame(width);
            solids.Add(new IntRect(0, 0, 50, 1));
            solids.Add(new IntRect(54, 0, 8, 1));
            solids.Add(new IntRect(10, 4, 4, 1));
            solids.Add(new IntRect(16, 7, 4, 1));
            solids.Add(new IntRect(22, 10, 5, 1));
            solids.Add(new IntRect(28, 13, 7, 1));

            return new LevelBlueprint(
                id: "level_06_three_host_puzzle",
                title: "Three Host Puzzle",
                levelSize: Size(width),
                cameraBounds: FullBounds(width),
                playerStart: new Vector2Int(3, 2),
                exitCell: new Vector2Int(58, 2),
                solids: solids.ToArray(),
                hazards: new[]
                {
                    new IntRect(50, 0, 4, 1),
                },
                breakables: System.Array.Empty<IntRect>(),
                laserGates: new[]
                {
                    new LaserGateData(new IntRect(48, 1, 1, 4), 10f, 0f, id: "level6_exit_gate"),
                },
                remoteSwitches: new[]
                {
                    new RemoteSwitchData("level6_switch", new Vector2Int(36, 14), new[] { "level6_turret", "level6_exit_gate" }, false),
                },
                turretNodes: new[]
                {
                    new TurretNodeData("level6_turret", new Vector2Int(40, 2), new IntRect(41, 1, 6, 3)),
                },
                phaseGates: System.Array.Empty<PhaseGateData>(),
                pressurePlates: System.Array.Empty<PressurePlateData>(),
                heavyBlocks: System.Array.Empty<HeavyBlockData>(),
                fanZones: System.Array.Empty<FanZoneData>(),
                hosts: new[]
                {
                    new HostSpawnData(HostType.Jumper, new Vector2Int(6, 2)),
                    new HostSpawnData(HostType.Shooter, new Vector2Int(31, 14)),
                    new HostSpawnData(HostType.Dasher, new Vector2Int(45, 2)),
                },
                tutorialHints: new[]
                {
                    "Climb to the Shooter, hit the switch, then go back and use Dasher to clear the exit lane."
                });
        }

        private static LevelBlueprint CreateFourHostChain()
        {
            const int width = 68;
            List<IntRect> solids = CreateFrame(width);
            solids.Add(new IntRect(0, 0, width, 1));
            solids.Add(new IntRect(54, 5, 4, 1));
            solids.Add(new IntRect(58, 8, 4, 1));
            solids.Add(new IntRect(62, 11, 4, 1));

            return new LevelBlueprint(
                id: "level_07_four_host_chain",
                title: "Four Host Chain",
                levelSize: Size(width),
                cameraBounds: FullBounds(width),
                playerStart: new Vector2Int(13, 2),
                exitCell: new Vector2Int(64, 12),
                solids: solids.ToArray(),
                hazards: System.Array.Empty<IntRect>(),
                breakables: System.Array.Empty<IntRect>(),
                laserGates: new[]
                {
                    new LaserGateData(new IntRect(20, 1, 1, 4), 10f, 0f, id: "level7_plate_gate"),
                    new LaserGateData(new IntRect(52, 1, 1, 4), 10f, 0f, id: "level7_upper_gate"),
                },
                remoteSwitches: new[]
                {
                    new RemoteSwitchData("level7_switch", new Vector2Int(44, 2), new[] { "level7_upper_gate" }, false),
                },
                turretNodes: System.Array.Empty<TurretNodeData>(),
                phaseGates: new[]
                {
                    new PhaseGateData("level7_phase_gate", new IntRect(32, 1, 1, 6)),
                },
                pressurePlates: new[]
                {
                    new PressurePlateData("level7_plate", new IntRect(8, 1, 2, 1), new[] { "level7_plate_gate" }, false),
                },
                heavyBlocks: new[]
                {
                    new HeavyBlockData("level7_block", new Vector2Int(11, 1), new Vector2Int(2, 2)),
                },
                fanZones: System.Array.Empty<FanZoneData>(),
                hosts: new[]
                {
                    new HostSpawnData(HostType.Anchor, new Vector2Int(15, 2)),
                    new HostSpawnData(HostType.Phase, new Vector2Int(24, 2)),
                    new HostSpawnData(HostType.Shooter, new Vector2Int(38, 2)),
                    new HostSpawnData(HostType.Jumper, new Vector2Int(34, 2)),
                },
                tutorialHints: new[]
                {
                    "Use Anchor to open the route, phase through the barrier, shoot the switch, then go back to Jumper for the final climb."
                });
        }

        private static LevelBlueprint CreateFinalExam()
        {
            const int width = 72;
            List<IntRect> solids = CreateFrame(width);
            solids.Add(new IntRect(0, 0, 64, 1));
            solids.Add(new IntRect(58, 4, 4, 1));
            solids.Add(new IntRect(62, 7, 4, 1));
            solids.Add(new IntRect(69, 7, 3, 1));

            return new LevelBlueprint(
                id: "level_08_final_exam",
                title: "Final Exam",
                levelSize: Size(width),
                cameraBounds: FullBounds(width),
                playerStart: new Vector2Int(13, 2),
                exitCell: new Vector2Int(70, 8),
                solids: solids.ToArray(),
                hazards: new[]
                {
                    new IntRect(66, 7, 3, 1),
                },
                breakables: System.Array.Empty<IntRect>(),
                laserGates: new[]
                {
                    new LaserGateData(new IntRect(20, 1, 1, 4), 10f, 0f, id: "level8_plate_gate"),
                    new LaserGateData(new IntRect(54, 1, 1, 4), 10f, 0f, id: "level8_upper_gate"),
                    new LaserGateData(new IntRect(69, 7, 1, 4), 10f, 0f, id: "level8_exit_laser"),
                },
                remoteSwitches: new[]
                {
                    new RemoteSwitchData("level8_switch", new Vector2Int(44, 2), new[] { "level8_upper_gate", "level8_exit_laser" }, false),
                },
                turretNodes: System.Array.Empty<TurretNodeData>(),
                phaseGates: new[]
                {
                    new PhaseGateData("level8_phase_gate", new IntRect(32, 1, 1, 6)),
                },
                pressurePlates: new[]
                {
                    new PressurePlateData("level8_plate", new IntRect(8, 1, 2, 1), new[] { "level8_plate_gate" }, false),
                },
                heavyBlocks: new[]
                {
                    new HeavyBlockData("level8_block", new Vector2Int(11, 1), new Vector2Int(2, 2)),
                },
                fanZones: new[]
                {
                    new FanZoneData("level8_fan", new IntRect(22, 1, 5, 4), new Vector2(-34f, 0f), false),
                },
                hosts: new[]
                {
                    new HostSpawnData(HostType.Anchor, new Vector2Int(15, 2)),
                    new HostSpawnData(HostType.Phase, new Vector2Int(24, 2)),
                    new HostSpawnData(HostType.Shooter, new Vector2Int(38, 2)),
                    new HostSpawnData(HostType.Jumper, new Vector2Int(34, 2)),
                    new HostSpawnData(HostType.Dasher, new Vector2Int(64, 8)),
                },
                tutorialHints: new[]
                {
                    "Anchor opens the route, Phase crosses the barrier, Shooter unlocks the top path, then return to Jumper before dashing to the finish."
                });
        }

        private static List<IntRect> CreateFrame(int width)
        {
            return new List<IntRect>
            {
                new IntRect(0, 0, 1, ExpectedLevelHeight),
                new IntRect(width - 1, 0, 1, ExpectedLevelHeight),
                new IntRect(0, ExpectedLevelHeight - 1, width, 1),
            };
        }

        private static Vector2Int Size(int width)
        {
            return new Vector2Int(width, ExpectedLevelHeight);
        }

        private static IntRect FullBounds(int width)
        {
            return new IntRect(0, 0, width, ExpectedLevelHeight);
        }
    }
}

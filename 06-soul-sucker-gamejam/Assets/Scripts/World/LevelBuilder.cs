using System.Collections.Generic;
using SoulSucker.Data;
using UnityEngine;

namespace SoulSucker.World
{
    public sealed class LevelBuilder : MonoBehaviour
    {
        public static Vector3 CellToLocalPosition(Vector2Int cell, Vector2Int levelSize)
        {
            return BlockFactory.CellToLocalPosition(cell, levelSize);
        }

        public static Rect RectToLocalRect(IntRect rect, Vector2Int levelSize)
        {
            return BlockFactory.RectToLocalRect(rect, levelSize);
        }

        public GameObject BuildLevel(LevelBlueprint blueprint, int levelIndex)
        {
            if (blueprint == null)
            {
                Debug.LogError("Cannot build a null level blueprint.");
                return null;
            }

            GameObject levelRoot = new GameObject($"RuntimeLevelRoot_{levelIndex + 1}_{blueprint.id}");
            levelRoot.transform.SetParent(transform, false);

            Transform environmentRoot = CreateGroup("Environment", levelRoot.transform);
            Transform solidsRoot = CreateGroup("Solids", levelRoot.transform);
            Transform hazardsRoot = CreateGroup("Hazards", levelRoot.transform);
            Transform breakablesRoot = CreateGroup("Breakables", levelRoot.transform);
            Transform laserGatesRoot = CreateGroup("LaserGates", levelRoot.transform);
            Transform phaseGatesRoot = CreateGroup("PhaseGates", levelRoot.transform);
            Transform turretRoot = CreateGroup("TurretNodes", levelRoot.transform);
            Transform switchRoot = CreateGroup("RemoteSwitches", levelRoot.transform);
            Transform fanRoot = CreateGroup("FanZones", levelRoot.transform);
            Transform plateRoot = CreateGroup("PressurePlates", levelRoot.transform);
            Transform blockRoot = CreateGroup("HeavyBlocks", levelRoot.transform);
            Transform hostsRoot = CreateGroup("Hosts", levelRoot.transform);
            Transform exitRoot = CreateGroup("Exit", levelRoot.transform);

            Vector2 levelSize = blueprint.levelSize;
            BlockFactory.CreateBackdrop(environmentRoot, levelSize);

            Dictionary<string, ILinkedToggleTarget> linkedTargets = new();
            var switches = new List<(RemoteSwitchData data, RemoteSwitch component)>();
            var plates = new List<(PressurePlateData data, PressurePlate component)>();

            foreach (IntRect rect in blueprint.solids)
            {
                BlockFactory.CreateSolid(solidsRoot, rect, levelSize);
            }

            foreach (IntRect rect in blueprint.hazards)
            {
                BlockFactory.CreateHazard(hazardsRoot, rect, levelSize);
            }

            foreach (IntRect rect in blueprint.breakables)
            {
                BlockFactory.CreateBreakable(breakablesRoot, rect, levelSize);
            }

            foreach (LaserGateData gate in blueprint.laserGates)
            {
                GameObject gateObject = BlockFactory.CreateLaserGate(laserGatesRoot, gate, levelSize);
                RegisterTarget(linkedTargets, gateObject.GetComponent<TimedLaserGate>());
            }

            foreach (PhaseGateData gate in blueprint.phaseGates)
            {
                BlockFactory.CreatePhaseGate(phaseGatesRoot, gate, levelSize);
            }

            foreach (TurretNodeData turretNode in blueprint.turretNodes)
            {
                GameObject nodeObject = BlockFactory.CreateTurretNode(turretRoot, turretNode, levelSize);
                RegisterTarget(linkedTargets, nodeObject.GetComponent<TurretNode>());
            }

            foreach (FanZoneData fanZone in blueprint.fanZones)
            {
                GameObject fanObject = BlockFactory.CreateFanZone(fanRoot, fanZone, levelSize);
                RegisterTarget(linkedTargets, fanObject.GetComponent<FanZone>());
            }

            foreach (HeavyBlockData heavyBlock in blueprint.heavyBlocks)
            {
                BlockFactory.CreateHeavyBlock(blockRoot, heavyBlock, levelSize);
            }

            foreach (PressurePlateData plate in blueprint.pressurePlates)
            {
                GameObject plateObject = BlockFactory.CreatePressurePlate(plateRoot, plate, levelSize);
                plates.Add((plate, plateObject.GetComponent<PressurePlate>()));
            }

            foreach (RemoteSwitchData remoteSwitch in blueprint.remoteSwitches)
            {
                GameObject switchObject = BlockFactory.CreateRemoteSwitch(switchRoot, remoteSwitch, levelSize);
                switches.Add((remoteSwitch, switchObject.GetComponent<RemoteSwitch>()));
            }

            foreach (HostSpawnData host in blueprint.hosts)
            {
                BlockFactory.CreateHostPlaceholder(hostsRoot, host, levelSize);
            }

            BlockFactory.CreateExit(exitRoot, blueprint.exitCell, levelSize);

            for (int i = 0; i < switches.Count; i++)
            {
                switches[i].component.BindTargets(ResolveTargets(linkedTargets, switches[i].data.targetIds));
            }

            for (int i = 0; i < plates.Count; i++)
            {
                plates[i].component.BindTargets(ResolveTargets(linkedTargets, plates[i].data.targetIds));
            }

            return levelRoot;
        }

        private static Transform CreateGroup(string groupName, Transform parent)
        {
            GameObject group = new GameObject(groupName);
            group.transform.SetParent(parent, false);
            return group.transform;
        }

        private static void RegisterTarget(Dictionary<string, ILinkedToggleTarget> targetMap, ILinkedToggleTarget target)
        {
            if (target == null || string.IsNullOrEmpty(target.LinkId))
            {
                return;
            }

            targetMap[target.LinkId] = target;
        }

        private static IReadOnlyList<ILinkedToggleTarget> ResolveTargets(Dictionary<string, ILinkedToggleTarget> targetMap, string[] targetIds)
        {
            var resolvedTargets = new List<ILinkedToggleTarget>();
            if (targetIds == null)
            {
                return resolvedTargets;
            }

            for (int i = 0; i < targetIds.Length; i++)
            {
                string targetId = targetIds[i];
                if (string.IsNullOrEmpty(targetId))
                {
                    continue;
                }

                if (targetMap.TryGetValue(targetId, out ILinkedToggleTarget target))
                {
                    resolvedTargets.Add(target);
                }
            }

            return resolvedTargets;
        }
    }
}

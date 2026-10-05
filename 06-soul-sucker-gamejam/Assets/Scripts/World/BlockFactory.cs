using SoulSucker.Data;
using SoulSucker.Hosts;
using SoulSucker.Utilities;
using UnityEngine;

namespace SoulSucker.World
{
    public static class BlockFactory
    {
        private static readonly Color BackdropColor = new Color(0.03f, 0.05f, 0.09f);
        private static readonly Color BackdropBandColor = new Color(0.16f, 0.28f, 0.42f, 0.12f);
        private static readonly Color SolidColor = new Color(0.23f, 0.28f, 0.36f);
        private static readonly Color SolidTopEdgeColor = new Color(0.40f, 0.88f, 1f, 0.24f);
        private static readonly Color SolidBottomEdgeColor = new Color(0.02f, 0.05f, 0.08f, 0.30f);
        private static readonly Color HazardColor = new Color(0.90f, 0.26f, 0.30f);
        private static readonly Color BreakableColor = new Color(0.90f, 0.72f, 0.24f);
        private static readonly Color ExitColor = new Color(0.28f, 0.92f, 0.56f);
        private static readonly Color LaserColor = new Color(0.96f, 0.24f, 0.36f, 0.92f);
        private static readonly Color PhaseGateColor = new Color(0.72f, 0.55f, 1f, 0.58f);
        private static readonly Color SwitchColor = new Color(1f, 0.76f, 0.24f);
        private static readonly Color PlateColor = new Color(0.26f, 0.50f, 0.38f);
        private static readonly Color FanColor = new Color(0.32f, 0.84f, 1f, 0.16f);

        public static GameObject CreateSolid(Transform parent, IntRect rect, Vector2 levelSize)
        {
            GameObject block = CreateRectObject("Solid", parent, rect, levelSize, SolidColor, 10);
            block.AddComponent<BoxCollider2D>().size = Vector2.one;
            block.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            block.AddComponent<SolidBlock>().Initialize(rect);
            AddOverlay(block.transform, "TopEdge", PlaceholderSpriteLibrary.GetSquareSprite(), SolidTopEdgeColor, 11, new Vector3(0f, 0.42f, -0.01f), new Vector3(1f, 0.12f, 1f));
            AddOverlay(block.transform, "BottomShade", PlaceholderSpriteLibrary.GetSquareSprite(), SolidBottomEdgeColor, 11, new Vector3(0f, -0.44f, -0.01f), new Vector3(1f, 0.14f, 1f));
            return block;
        }

        public static GameObject CreateHazard(Transform parent, IntRect rect, Vector2 levelSize)
        {
            GameObject hazard = CreateRectObject("Hazard", parent, rect, levelSize, HazardColor, 20);
            BoxCollider2D collider = hazard.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.isTrigger = true;
            hazard.AddComponent<HazardZone>().Initialize(rect);
            AddOverlay(hazard.transform, "Glow", PlaceholderSpriteLibrary.GetCircleSprite(), new Color(1f, 0.38f, 0.32f, 0.22f), 19, Vector3.zero, new Vector3(1.08f, 1.08f, 1f));
            return hazard;
        }

        public static GameObject CreateBreakable(Transform parent, IntRect rect, Vector2 levelSize)
        {
            GameObject breakable = CreateRectObject("Breakable", parent, rect, levelSize, BreakableColor, 15);
            breakable.AddComponent<BoxCollider2D>().size = Vector2.one;
            breakable.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            breakable.AddComponent<BreakableBlock>().Initialize(rect);
            AddOverlay(breakable.transform, "CrackA", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.10f, 0.12f, 0.18f, 0.48f), 16, new Vector3(-0.08f, 0.02f, -0.02f), new Vector3(0.12f, 0.92f, 1f), 34f);
            AddOverlay(breakable.transform, "CrackB", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.10f, 0.12f, 0.18f, 0.48f), 16, new Vector3(0.07f, -0.04f, -0.02f), new Vector3(0.12f, 0.92f, 1f), -32f);
            AddOverlay(breakable.transform, "Glow", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(1f, 0.82f, 0.18f, 0.18f), 14, Vector3.zero, new Vector3(1.14f, 1.14f, 1f));
            return breakable;
        }

        public static GameObject CreateLaserGate(Transform parent, LaserGateData gate, Vector2 levelSize)
        {
            GameObject gateObject = CreateRectObject("LaserGate", parent, gate.bounds, levelSize, LaserColor, 35);
            AddOverlay(gateObject.transform, "BeamCore", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(1f, 0.96f, 0.92f, 0.22f), 36, Vector3.zero, new Vector3(0.42f, 0.96f, 1f));
            AddOverlay(gateObject.transform, "BeamGlow", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(1f, 0.40f, 0.46f, 0.18f), 34, Vector3.zero, new Vector3(1.16f, 1.16f, 1f));
            TimedLaserGate laserGate = gateObject.AddComponent<TimedLaserGate>();
            laserGate.Initialize(gate);
            return gateObject;
        }

        public static GameObject CreatePhaseGate(Transform parent, PhaseGateData gate, Vector2 levelSize)
        {
            GameObject gateObject = CreateRectObject("PhaseGate", parent, gate.bounds, levelSize, PhaseGateColor, 28);
            AddOverlay(gateObject.transform, "Edge", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.90f, 0.82f, 1f, 0.18f), 29, Vector3.zero, new Vector3(0.88f, 1.02f, 1f));
            PhaseGate phaseGate = gateObject.AddComponent<PhaseGate>();
            phaseGate.Initialize(gate);
            return gateObject;
        }

        public static GameObject CreateRemoteSwitch(Transform parent, RemoteSwitchData switchData, Vector2 levelSize)
        {
            IntRect switchRect = new IntRect(switchData.cell.x, switchData.cell.y, 1, 1);
            GameObject switchObject = CreateRectObject("RemoteSwitch", parent, switchRect, levelSize, SwitchColor, 31);
            switchObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            AddOverlay(switchObject.transform, "Glow", PlaceholderSpriteLibrary.GetCircleSprite(), new Color(1f, 0.92f, 0.58f, 0.18f), 30, Vector3.zero, new Vector3(1.4f, 1.4f, 1f));
            RemoteSwitch remoteSwitch = switchObject.AddComponent<RemoteSwitch>();
            remoteSwitch.Initialize(switchData);
            return switchObject;
        }

        public static GameObject CreateTurretNode(Transform parent, TurretNodeData turretData, Vector2 levelSize)
        {
            IntRect nodeRect = new IntRect(turretData.cell.x, turretData.cell.y, 1, 1);
            GameObject nodeObject = CreateRectObject("TurretNode", parent, nodeRect, levelSize, new Color(1f, 0.58f, 0.20f, 1f), 32);
            nodeObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            AddOverlay(nodeObject.transform, "Barrel", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(1f, 0.88f, 0.56f, 0.82f), 33, new Vector3(0.36f, 0f, -0.02f), new Vector3(0.42f, 0.16f, 1f));

            GameObject beamObject = CreateRectObject("TurretBeam", parent, turretData.beamBounds, levelSize, new Color(1f, 0.40f, 0.38f, 0.2f), 24);
            beamObject.AddComponent<BoxCollider2D>().size = Vector2.one;
            beamObject.GetComponent<BoxCollider2D>().isTrigger = true;
            beamObject.AddComponent<HazardZone>().Initialize(turretData.beamBounds);
            AddOverlay(beamObject.transform, "BeamCore", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(1f, 0.94f, 0.80f, 0.14f), 25, Vector3.zero, new Vector3(0.26f, 0.96f, 1f));

            TurretNode turretNode = nodeObject.AddComponent<TurretNode>();
            turretNode.Initialize(turretData, beamObject);
            return nodeObject;
        }

        public static GameObject CreatePressurePlate(Transform parent, PressurePlateData plateData, Vector2 levelSize)
        {
            GameObject plateObject = CreateRectObject("PressurePlate", parent, plateData.bounds, levelSize, PlateColor, 14);
            plateObject.transform.localScale = new Vector3(plateData.bounds.width, Mathf.Max(0.28f, plateData.bounds.height * 0.35f), 1f);
            PressurePlate pressurePlate = plateObject.AddComponent<PressurePlate>();
            pressurePlate.Initialize(plateData);
            return plateObject;
        }

        public static GameObject CreateHeavyBlock(Transform parent, HeavyBlockData blockData, Vector2 levelSize)
        {
            IntRect bounds = new IntRect(blockData.cell.x, blockData.cell.y, blockData.size.x, blockData.size.y);
            GameObject blockObject = CreateRectObject("HeavyBlock", parent, bounds, levelSize, new Color(0.44f, 0.86f, 0.42f, 1f), 22);
            AddOverlay(blockObject.transform, "Inset", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.18f, 0.28f, 0.14f, 0.32f), 23, Vector3.zero, new Vector3(0.84f, 0.84f, 1f));
            HeavyBlock heavyBlock = blockObject.AddComponent<HeavyBlock>();
            heavyBlock.Initialize(blockData);
            return blockObject;
        }

        public static GameObject CreateFanZone(Transform parent, FanZoneData fanData, Vector2 levelSize)
        {
            GameObject fanObject = CreateRectObject("FanZone", parent, fanData.bounds, levelSize, FanColor, 18);
            AddOverlay(fanObject.transform, "WindBandA", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.72f, 0.96f, 1f, 0.12f), 19, new Vector3(0f, 0.28f, -0.01f), new Vector3(0.86f, 0.12f, 1f));
            AddOverlay(fanObject.transform, "WindBandB", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.72f, 0.96f, 1f, 0.12f), 19, new Vector3(0f, -0.18f, -0.01f), new Vector3(0.76f, 0.12f, 1f));
            FanZone fanZone = fanObject.AddComponent<FanZone>();
            fanZone.Initialize(fanData);
            return fanObject;
        }

        public static GameObject CreateExit(Transform parent, Vector2Int cell, Vector2 levelSize)
        {
            GameObject exitDoor = new GameObject("ExitDoor");
            exitDoor.transform.SetParent(parent, false);
            exitDoor.transform.localPosition = CellToLocalPosition(cell, levelSize) + new Vector3(0f, 0.5f, 0f);
            exitDoor.transform.localScale = new Vector3(1f, 2f, 1f);

            SpriteRenderer renderer = exitDoor.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderSpriteLibrary.GetSquareSprite();
            renderer.color = ExitColor;
            renderer.sortingOrder = 30;

            AddOverlay(exitDoor.transform, "Glow", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.30f, 1f, 0.72f, 0.26f), 29, Vector3.zero, new Vector3(1.48f, 2.5f, 1f));
            AddOverlay(exitDoor.transform, "Pulse", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.92f, 1f, 0.94f, 0.26f), 31, Vector3.zero, new Vector3(1.15f, 2.05f, 1f));
            AddOverlay(exitDoor.transform, "Core", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.98f, 1f, 0.96f, 0.32f), 32, Vector3.zero, new Vector3(0.52f, 1.48f, 1f));

            BoxCollider2D collider = exitDoor.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.isTrigger = true;

            exitDoor.AddComponent<ExitDoor>().Initialize(cell);
            return exitDoor;
        }

        public static GameObject CreateHostPlaceholder(Transform parent, HostSpawnData host, Vector2 levelSize)
        {
            GameObject hostObject = new GameObject($"{host.type}Host");
            hostObject.transform.SetParent(parent, false);
            hostObject.transform.localPosition = CellToLocalPosition(host.cell, levelSize) + new Vector3(0f, 0.14f, 0f);
            hostObject.transform.localScale = GetHostBodyScale(host.type);

            SpriteRenderer renderer = hostObject.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderSpriteLibrary.GetSquareSprite();
            renderer.color = GetHostColor(host.type);
            renderer.sortingOrder = 25;

            AddOverlay(hostObject.transform, "Shadow", PlaceholderSpriteLibrary.GetCircleSprite(), new Color(0.01f, 0.03f, 0.05f, 0.24f), 24, new Vector3(0f, -0.22f, -0.03f), new Vector3(1.16f, 0.58f, 1f));
            AddOverlay(hostObject.transform, "Aura", PlaceholderSpriteLibrary.GetCircleSprite(), GetHostGlowColor(host.type), 23, Vector3.zero, GetHostAuraScale(host.type));
            AddOverlay(hostObject.transform, "Ring", PlaceholderSpriteLibrary.GetCircleSprite(), GetHostRingColor(host.type), 26, Vector3.zero, GetHostRingScale(host.type));
            AddHostSignature(hostObject.transform, host.type);

            BoxCollider2D collider = hostObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.9f, 1.2f);
            collider.isTrigger = false;

            Rigidbody2D rigidbody = hostObject.AddComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.freezeRotation = true;

            GameObject releaseAnchor = new GameObject("ReleaseAnchor");
            releaseAnchor.transform.SetParent(hostObject.transform, false);
            releaseAnchor.transform.localPosition = new Vector3(0f, 1.1f, 0f);

            HostBase hostComponent = CreateHostComponent(hostObject, host.type);
            hostComponent.Initialize(host.type, releaseAnchor.transform);
            return hostObject;
        }

        public static GameObject CreateBackdrop(Transform parent, Vector2 levelSize)
        {
            Transform existing = parent.Find("Backdrop");
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject backdrop = new GameObject("Backdrop");
            backdrop.transform.SetParent(parent, false);
            backdrop.transform.localPosition = new Vector3(0f, 0f, -5f);
            backdrop.transform.localScale = new Vector3(levelSize.x + 2f, levelSize.y + 2f, 1f);

            SpriteRenderer renderer = backdrop.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderSpriteLibrary.GetSquareSprite();
            renderer.color = BackdropColor;
            renderer.sortingOrder = -100;

            AddOverlay(backdrop.transform, "BandTop", PlaceholderSpriteLibrary.GetSquareSprite(), BackdropBandColor, -99, new Vector3(0f, 3.2f, -0.01f), new Vector3(1.06f, 0.08f, 1f));
            AddOverlay(backdrop.transform, "BandMid", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.22f, 0.54f, 0.78f, 0.05f), -99, new Vector3(0.14f, 0.2f, -0.01f), new Vector3(0.56f, 0.62f, 1f));
            AddOverlay(backdrop.transform, "BandBottom", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.46f, 0.28f, 0.74f, 0.06f), -99, new Vector3(-0.22f, -3.0f, -0.01f), new Vector3(0.66f, 0.10f, 1f));
            CreateAmbientMotes(backdrop.transform, levelSize);

            return backdrop;
        }

        public static Rect RectToLocalRect(IntRect rect, Vector2Int levelSize)
        {
            Vector3 center = RectToLocalPosition(rect, levelSize);
            return new Rect(center.x - (rect.width * 0.5f), center.y - (rect.height * 0.5f), rect.width, rect.height);
        }

        public static Vector3 CellToLocalPosition(Vector2Int cell, Vector2 levelSize)
        {
            float x = (-levelSize.x * 0.5f) + cell.x + 0.5f;
            float y = (-levelSize.y * 0.5f) + cell.y + 0.5f;
            return new Vector3(x, y, 0f);
        }

        private static GameObject CreateRectObject(string objectName, Transform parent, IntRect rect, Vector2 levelSize, Color color, int sortingOrder)
        {
            GameObject block = new GameObject(objectName);
            block.transform.SetParent(parent, false);
            block.transform.localPosition = RectToLocalPosition(rect, levelSize);
            block.transform.localScale = new Vector3(rect.width, rect.height, 1f);

            SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderSpriteLibrary.GetSquareSprite();
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return block;
        }

        private static GameObject AddOverlay(Transform parent, string name, Sprite sprite, Color color, int sortingOrder, Vector3 localPosition, Vector3 localScale, float localRotation = 0f)
        {
            GameObject overlay = new GameObject(name);
            overlay.transform.SetParent(parent, false);
            overlay.transform.localPosition = localPosition;
            overlay.transform.localScale = localScale;
            overlay.transform.localRotation = Quaternion.Euler(0f, 0f, localRotation);

            SpriteRenderer renderer = overlay.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return overlay;
        }

        private static void CreateAmbientMotes(Transform parent, Vector2 levelSize)
        {
            GameObject motes = new GameObject("AmbientMotes");
            motes.transform.SetParent(parent, false);

            Vector2[] positions =
            {
                new Vector2(-0.34f, 0.26f),
                new Vector2(-0.12f, -0.1f),
                new Vector2(0.12f, 0.32f),
                new Vector2(0.28f, -0.22f),
                new Vector2(0.44f, 0.12f),
                new Vector2(-0.44f, -0.28f),
            };

            for (int i = 0; i < positions.Length; i++)
            {
                Vector2 normalized = positions[i];
                GameObject mote = AddOverlay(
                    motes.transform,
                    $"Mote_{i + 1}",
                    PlaceholderSpriteLibrary.GetCircleSprite(),
                    new Color(0.60f, 0.90f, 1f, 0.05f + (i * 0.01f)),
                    -96,
                    new Vector3(normalized.x * levelSize.x * 0.58f, normalized.y * levelSize.y * 0.5f, -0.02f),
                    Vector3.one * (0.18f + (i * 0.04f)));

                AmbientMote ambientMote = mote.AddComponent<AmbientMote>();
                ambientMote.Configure(
                    new Vector2(0.05f + (i * 0.01f), 0.08f + (i * 0.015f)),
                    0.26f + (i * 0.05f),
                    0.05f + (i * 0.01f),
                    i * 0.7f);
            }
        }

        private static Vector3 RectToLocalPosition(IntRect rect, Vector2 levelSize)
        {
            float x = (-levelSize.x * 0.5f) + rect.x + (rect.width * 0.5f);
            float y = (-levelSize.y * 0.5f) + rect.y + (rect.height * 0.5f);
            return new Vector3(x, y, 0f);
        }

        private static Color GetHostColor(HostType hostType)
        {
            switch (hostType)
            {
                case HostType.Jumper:
                    return new Color(0.28f, 0.76f, 1f);
                case HostType.Dasher:
                    return new Color(0.46f, 0.96f, 0.42f);
                case HostType.Shooter:
                    return new Color(1f, 0.76f, 0.24f);
                case HostType.Phase:
                    return new Color(0.74f, 0.56f, 1f);
                case HostType.Anchor:
                    return new Color(0.42f, 0.86f, 0.40f);
                default:
                    return Color.gray;
            }
        }

        private static Color GetHostGlowColor(HostType hostType)
        {
            Color baseColor = GetHostColor(hostType);
            return new Color(baseColor.r, baseColor.g, baseColor.b, 0.18f);
        }

        private static Color GetHostRingColor(HostType hostType)
        {
            Color baseColor = GetHostColor(hostType);
            return new Color(baseColor.r, baseColor.g, baseColor.b, 0.28f);
        }

        private static Vector3 GetHostAuraScale(HostType hostType)
        {
            switch (hostType)
            {
                case HostType.Dasher:
                    return new Vector3(1.18f, 1.02f, 1f);
                case HostType.Phase:
                    return new Vector3(1.26f, 1.12f, 1f);
                case HostType.Anchor:
                    return new Vector3(1.30f, 1.22f, 1f);
                default:
                    return new Vector3(1.20f, 1.18f, 1f);
            }
        }

        private static Vector3 GetHostRingScale(HostType hostType)
        {
            return new Vector3(1.06f, 1.06f, 1f);
        }

        private static Vector3 GetHostBodyScale(HostType hostType)
        {
            switch (hostType)
            {
                case HostType.Jumper:
                    return new Vector3(0.88f, 1.10f, 1f);
                case HostType.Dasher:
                    return new Vector3(1.02f, 0.94f, 1f);
                case HostType.Shooter:
                    return new Vector3(0.96f, 1.02f, 1f);
                case HostType.Phase:
                    return new Vector3(0.90f, 1.04f, 1f);
                case HostType.Anchor:
                    return new Vector3(1.14f, 1.18f, 1f);
                default:
                    return Vector3.one;
            }
        }

        private static void AddHostSignature(Transform parent, HostType hostType)
        {
            switch (hostType)
            {
                case HostType.Jumper:
                    AddOverlay(parent, "Crest", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.88f, 0.98f, 1f, 0.86f), 27, new Vector3(0f, 0.18f, -0.08f), new Vector3(0.28f, 0.36f, 1f));
                    AddOverlay(parent, "FootL", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.12f, 0.22f, 0.34f, 0.65f), 24, new Vector3(-0.18f, -0.52f, -0.08f), new Vector3(0.12f, 0.20f, 1f));
                    AddOverlay(parent, "FootR", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.12f, 0.22f, 0.34f, 0.65f), 24, new Vector3(0.18f, -0.52f, -0.08f), new Vector3(0.12f, 0.20f, 1f));
                    break;
                case HostType.Dasher:
                    AddOverlay(parent, "SpeedLineL", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.86f, 1f, 0.84f, 0.68f), 24, new Vector3(-0.34f, 0.02f, -0.08f), new Vector3(0.34f, 0.08f, 1f));
                    AddOverlay(parent, "SpeedLineR", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.86f, 1f, 0.84f, 0.68f), 24, new Vector3(0.34f, 0.02f, -0.08f), new Vector3(0.34f, 0.08f, 1f));
                    break;
                case HostType.Shooter:
                    AddOverlay(parent, "Barrel", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(1f, 0.86f, 0.62f, 0.84f), 27, new Vector3(0.34f, 0.04f, -0.08f), new Vector3(0.42f, 0.14f, 1f));
                    AddOverlay(parent, "Visor", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.18f, 0.12f, 0.10f, 0.66f), 27, new Vector3(0f, 0.18f, -0.08f), new Vector3(0.38f, 0.12f, 1f));
                    break;
                case HostType.Phase:
                    AddOverlay(parent, "PhaseCore", PlaceholderSpriteLibrary.GetCircleSprite(), new Color(0.94f, 0.88f, 1f, 0.54f), 27, Vector3.zero, new Vector3(0.44f, 0.44f, 1f));
                    AddOverlay(parent, "PhaseTail", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.86f, 0.74f, 1f, 0.36f), 24, new Vector3(-0.22f, -0.18f, -0.08f), new Vector3(0.14f, 0.48f, 1f), 18f);
                    break;
                case HostType.Anchor:
                    AddOverlay(parent, "AnchorTop", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.90f, 1f, 0.86f, 0.74f), 27, new Vector3(0f, 0.28f, -0.08f), new Vector3(0.48f, 0.16f, 1f));
                    AddOverlay(parent, "AnchorBase", PlaceholderSpriteLibrary.GetSquareSprite(), new Color(0.18f, 0.28f, 0.14f, 0.52f), 24, new Vector3(0f, -0.26f, -0.08f), new Vector3(0.56f, 0.22f, 1f));
                    break;
                default:
                    break;
            }
        }

        private static HostBase CreateHostComponent(GameObject hostObject, HostType hostType)
        {
            switch (hostType)
            {
                case HostType.Jumper:
                    return hostObject.AddComponent<JumperHost>();
                case HostType.Dasher:
                    return hostObject.AddComponent<DasherHost>();
                case HostType.Shooter:
                    return hostObject.AddComponent<ShooterHost>();
                case HostType.Phase:
                    return hostObject.AddComponent<PhaseHost>();
                case HostType.Anchor:
                    return hostObject.AddComponent<AnchorHost>();
                default:
                    return hostObject.AddComponent<JumperHost>();
            }
        }
    }
}

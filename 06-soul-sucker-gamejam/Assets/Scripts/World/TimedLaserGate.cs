using SoulSucker.Data;
using SoulSucker.Hosts;
using SoulSucker.Player;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class TimedLaserGate : MonoBehaviour, ILinkedToggleTarget
    {
        [SerializeField] private LaserGateData gateData;

        private static readonly Color ActiveColor = new Color(0.92f, 0.26f, 0.34f, 0.96f);
        private static readonly Color InactiveColor = new Color(0.58f, 0.32f, 0.72f, 0.18f);
        private static readonly Color GlowActiveColor = new Color(1f, 0.36f, 0.42f, 0.24f);
        private static readonly Color GlowInactiveColor = new Color(0.42f, 0.32f, 0.65f, 0.10f);
        private static readonly Color WarningColor = new Color(1f, 0.82f, 0.38f, 0.22f);

        private BoxCollider2D gateCollider;
        private SpriteRenderer beamRenderer;
        private SpriteRenderer glowRenderer;
        private SpriteRenderer warningRenderer;
        private float elapsedTime;
        private bool isActive;
        private bool linkEnabled = true;
        private Vector3 glowBaseScale = Vector3.one;
        private Vector3 warningBaseScale = Vector3.one;

        public bool IsActive => isActive;
        public LaserGateData Data => gateData;
        public string LinkId => gateData != null ? gateData.id : string.Empty;

        private void Awake()
        {
            EnsureComponents();
            UpdateState(forceRefresh: true);
        }

        public void Initialize(LaserGateData data)
        {
            gateData = data;
            EnsureComponents();
            elapsedTime = 0f;
            linkEnabled = data != null && data.startsEnabled;
            UpdateState(forceRefresh: true);
        }

        public void SetLinkedActive(bool active)
        {
            linkEnabled = active;
            UpdateState(forceRefresh: true);
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            UpdateState(forceRefresh: false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryKillOccupant(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryKillOccupant(other);
        }

        private void EnsureComponents()
        {
            if (gateCollider == null)
            {
                gateCollider = GetComponent<BoxCollider2D>();
                gateCollider.size = Vector2.one;
                gateCollider.isTrigger = true;
            }

            if (beamRenderer == null)
            {
                beamRenderer = GetComponent<SpriteRenderer>();
            }

            if (glowRenderer == null)
            {
                glowRenderer = CreateLayerIfMissing(
                    "Glow",
                    34,
                    new Vector3(1.22f, 1.22f, 1f),
                    GlowInactiveColor);
                glowBaseScale = glowRenderer.transform.localScale;
            }

            if (warningRenderer == null)
            {
                warningRenderer = CreateLayerIfMissing(
                    "Warning",
                    36,
                    new Vector3(1.04f, 1.04f, 1f),
                    WarningColor);
                warningBaseScale = warningRenderer.transform.localScale;
            }
        }

        private SpriteRenderer CreateLayerIfMissing(string name, int sortingOrder, Vector3 localScale, Color color)
        {
            Transform existing = transform.Find(name);
            GameObject layerObject = existing != null ? existing.gameObject : new GameObject(name);
            if (existing == null)
            {
                layerObject.transform.SetParent(transform, false);
            }

            layerObject.transform.localPosition = Vector3.zero;
            layerObject.transform.localScale = localScale;

            SpriteRenderer renderer = layerObject.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = layerObject.AddComponent<SpriteRenderer>();
            }

            renderer.sprite = Utilities.PlaceholderSpriteLibrary.GetSquareSprite();
            renderer.sortingOrder = sortingOrder;
            renderer.color = color;
            return renderer;
        }

        private void UpdateState(bool forceRefresh)
        {
            EnsureComponents();
            if (gateData == null)
            {
                beamRenderer.color = InactiveColor;
                glowRenderer.color = GlowInactiveColor;
                warningRenderer.color = new Color(0.52f, 0.38f, 0.72f, 0.10f);
                gateCollider.enabled = true;
                return;
            }

            bool nextIsActive = ComputeIsActive();
            if (!forceRefresh && nextIsActive == isActive)
            {
                AnimateLayers();
                return;
            }

            isActive = nextIsActive;
            gateCollider.enabled = true;
            beamRenderer.color = isActive ? ActiveColor : InactiveColor;
            glowRenderer.color = isActive ? GlowActiveColor : GlowInactiveColor;
            warningRenderer.color = isActive
                ? new Color(1f, 0.92f, 0.56f, 0.14f)
                : new Color(0.52f, 0.38f, 0.72f, 0.10f);
            AnimateLayers();
        }

        private void AnimateLayers()
        {
            float pulse = 1f + (Mathf.Sin((Time.time * (isActive ? 8f : 2f)) + gateData.phaseOffsetSeconds) * (isActive ? 0.08f : 0.03f));
            glowRenderer.transform.localScale = glowBaseScale * pulse;
            warningRenderer.transform.localScale = warningBaseScale * (isActive ? 1f + (Mathf.Sin(Time.time * 6f) * 0.06f) : 1f);
        }

        private bool ComputeIsActive()
        {
            if (gateData == null)
            {
                return false;
            }

            if (!linkEnabled)
            {
                return false;
            }

            float activeDuration = Mathf.Max(0.01f, gateData.activeSeconds);
            float inactiveDuration = Mathf.Max(0f, gateData.inactiveSeconds);
            float cycleDuration = activeDuration + inactiveDuration;
            if (cycleDuration <= 0.01f)
            {
                return true;
            }

            float cycleTime = Mathf.Repeat(elapsedTime + gateData.phaseOffsetSeconds, cycleDuration);
            return cycleTime < activeDuration;
        }

        private void TryKillOccupant(Collider2D other)
        {
            if (!isActive)
            {
                return;
            }

            ParasiteBody parasite = other.GetComponent<ParasiteBody>();
            HostBase host = other.GetComponent<HostBase>();
            if (parasite == null && host == null)
            {
                return;
            }

            if (host != null && !host.IsControlled)
            {
                return;
            }

            PlayerBrain brain = Object.FindFirstObjectByType<PlayerBrain>();
            if (brain != null)
            {
                brain.KillPlayer();
            }
        }
    }
}

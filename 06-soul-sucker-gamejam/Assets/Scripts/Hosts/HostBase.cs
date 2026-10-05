using SoulSucker.Data;
using SoulSucker.Player;
using SoulSucker.Utilities;
using UnityEngine;
using SoulSucker.World;

namespace SoulSucker.Hosts
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class HostBase : MonoBehaviour
    {
        [SerializeField] private HostType hostType;
        [SerializeField] private Transform releaseAnchor;
        [SerializeField] private bool isControlled;
        [SerializeField] private bool isHighlighted;

        private SpriteRenderer spriteRenderer;
        private SpriteRenderer auraRenderer;
        private SpriteRenderer ringRenderer;
        private SpriteRenderer shadowRenderer;
        private Rigidbody2D body;
        private Collider2D hostCollider;
        private Vector3 originalScale = Vector3.one;
        private Color baseColor = Color.white;
        private PlayerBrain controllingBrain;
        private Vector3 auraBaseScale = Vector3.one;
        private Vector3 ringBaseScale = Vector3.one;
        private Vector3 shadowBaseScale = Vector3.one;
        private Vector3 auraBasePosition = Vector3.zero;
        private Vector3 ringBasePosition = Vector3.zero;
        private Vector3 shadowBasePosition = Vector3.zero;

        public HostType HostType => hostType;
        public Transform ReleaseAnchor => releaseAnchor != null ? releaseAnchor : transform;
        public bool IsControlled => isControlled;
        public bool IsHighlighted => isHighlighted;
        public bool IsPossessable => !isControlled;
        public Rigidbody2D Body => body;
        public Collider2D HostCollider => hostCollider;
        public virtual bool IsFanImmune => hostType == HostType.Anchor;
        protected virtual bool IgnoreHazardsWhileControlled => false;
        public bool IgnoreHazards => isControlled && IgnoreHazardsWhileControlled;

        protected virtual void Awake()
        {
            EnsureReferences();
            ApplyVisualState();
        }

        protected virtual void Update()
        {
            ApplyVisualState();
        }

        public void Initialize(HostType type, Transform anchor)
        {
            hostType = type;
            releaseAnchor = anchor;
            EnsureReferences();
            ConfigureDefaults();
            ApplyVisualState();
        }

        public virtual void OnPossessed(PlayerBrain brain)
        {
            EnsureReferences();
            controllingBrain = brain;
            isControlled = true;
            isHighlighted = false;
            body.simulated = true;
            ApplyVisualState();
        }

        public virtual void OnReleased(PlayerBrain brain)
        {
            if (controllingBrain == brain)
            {
                controllingBrain = null;
            }

            isControlled = false;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            ApplyVisualState();
        }

        public virtual void TickControlled(InputFrame inputFrame, float deltaTime)
        {
        }

        protected bool IsGrounded(float probeDistance, float probeInset = 0.08f)
        {
            EnsureReferences();
            Bounds bounds = hostCollider.bounds;
            Vector2 boxSize = new Vector2(
                Mathf.Max(0.25f, bounds.size.x - probeInset),
                Mathf.Max(0.1f, probeDistance));
            Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - (probeDistance * 0.5f));
            Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hit = hits[i];
                if (hit != null && hit != hostCollider && !hit.isTrigger)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetHighlighted(bool value)
        {
            if (isControlled)
            {
                value = false;
            }

            if (isHighlighted == value)
            {
                return;
            }

            isHighlighted = value;
            ApplyVisualState();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isControlled || controllingBrain == null)
            {
                return;
            }

            if (other.GetComponent<SoulSucker.World.HazardZone>() != null)
            {
                if (IgnoreHazardsWhileControlled)
                {
                    return;
                }

                controllingBrain.KillPlayer();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!isControlled || controllingBrain == null)
            {
                return;
            }

            if (other.GetComponent<SoulSucker.World.HazardZone>() != null)
            {
                if (IgnoreHazardsWhileControlled)
                {
                    return;
                }

                controllingBrain.KillPlayer();
            }
        }

        private void EnsureReferences()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
                body.freezeRotation = true;
            }

            if (hostCollider == null)
            {
                hostCollider = GetComponent<Collider2D>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                baseColor = spriteRenderer.color;
                originalScale = transform.localScale;
            }

            EnsureVisualLayers();
        }

        private void EnsureVisualLayers()
        {
            if (auraRenderer == null)
            {
                auraRenderer = CreateLayerIfMissing(
                    "Aura",
                    PlaceholderSpriteLibrary.GetCircleSprite(),
                    spriteRenderer.sortingOrder - 2,
                    new Vector3(0f, 0f, -0.03f),
                    new Vector3(1.22f, 1.16f, 1f),
                    new Color(0.7f, 0.92f, 1f, 0.16f));
                auraBaseScale = auraRenderer.transform.localScale;
                auraBasePosition = auraRenderer.transform.localPosition;
            }

            if (ringRenderer == null)
            {
                ringRenderer = CreateLayerIfMissing(
                    "Ring",
                    PlaceholderSpriteLibrary.GetCircleSprite(),
                    spriteRenderer.sortingOrder + 1,
                    Vector3.zero,
                    new Vector3(1.04f, 1.04f, 1f),
                    new Color(0.86f, 0.95f, 1f, 0.18f));
                ringBaseScale = ringRenderer.transform.localScale;
                ringBasePosition = ringRenderer.transform.localPosition;
            }

            if (shadowRenderer == null)
            {
                shadowRenderer = CreateLayerIfMissing(
                    "Shadow",
                    PlaceholderSpriteLibrary.GetCircleSprite(),
                    spriteRenderer.sortingOrder - 3,
                    new Vector3(0f, -0.22f, -0.05f),
                    new Vector3(1.16f, 0.56f, 1f),
                    new Color(0f, 0f, 0f, 0.22f));
                shadowBaseScale = shadowRenderer.transform.localScale;
                shadowBasePosition = shadowRenderer.transform.localPosition;
            }
        }

        private SpriteRenderer CreateLayerIfMissing(string layerName, Sprite sprite, int sortingOrder, Vector3 localPosition, Vector3 localScale, Color color)
        {
            Transform existing = transform.Find(layerName);
            GameObject layerObject = existing != null ? existing.gameObject : new GameObject(layerName);
            if (existing == null)
            {
                layerObject.transform.SetParent(transform, false);
            }

            layerObject.transform.localPosition = localPosition;
            layerObject.transform.localScale = localScale;

            SpriteRenderer renderer = layerObject.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = layerObject.AddComponent<SpriteRenderer>();
            }

            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            renderer.color = color;
            return renderer;
        }

        protected virtual void ConfigureDefaults()
        {
            EnsureReferences();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 2.5f;
            body.mass = 1f;
            body.linearDamping = 0f;
            body.angularDamping = 0.05f;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            hostCollider.isTrigger = false;
        }

        private void ApplyVisualState()
        {
            EnsureReferences();

            Color accent = GetAccentColor();
            float phase = ((int)hostType + 1) * 0.63f;
            float idlePulse = 0.95f + (Mathf.Sin(Time.time * 2.8f + phase) * 0.03f);
            float auraPulse = isControlled ? 1.22f + (Mathf.Sin(Time.time * 6f + phase) * 0.06f) : isHighlighted ? 1.12f + (Mathf.Sin(Time.time * 6.5f + phase) * 0.05f) : idlePulse;
            float ringPulse = isControlled ? 1.12f + (Mathf.Sin(Time.time * 7f + phase) * 0.05f) : isHighlighted ? 1.06f + (Mathf.Sin(Time.time * 7.5f + phase) * 0.04f) : 0.98f + (Mathf.Sin(Time.time * 4.2f + phase) * 0.02f);
            float shadowPulse = isControlled ? 1.04f : 1f;

            if (isControlled)
            {
                spriteRenderer.color = Color.Lerp(baseColor, Color.white, 0.54f);
            }
            else if (isHighlighted)
            {
                spriteRenderer.color = Color.Lerp(baseColor, Color.white, 0.28f);
            }
            else
            {
                spriteRenderer.color = baseColor;
            }

            if (shadowRenderer != null)
            {
                shadowRenderer.color = new Color(0f, 0f, 0f, isControlled ? 0.32f : isHighlighted ? 0.26f : 0.20f);
                shadowRenderer.transform.localPosition = shadowBasePosition + new Vector3(0f, Mathf.Sin(Time.time * 2f + phase) * 0.015f, 0f);
                shadowRenderer.transform.localScale = shadowBaseScale * shadowPulse;
            }

            if (auraRenderer != null)
            {
                auraRenderer.color = new Color(accent.r, accent.g, accent.b, isControlled ? 0.40f : isHighlighted ? 0.26f : 0.14f);
                auraRenderer.transform.localPosition = auraBasePosition + new Vector3(0f, Mathf.Sin(Time.time * 2.2f + phase) * 0.02f, 0f);
                auraRenderer.transform.localScale = auraBaseScale * auraPulse;
            }

            if (ringRenderer != null)
            {
                ringRenderer.color = new Color(accent.r, accent.g, accent.b, isControlled ? 0.50f : isHighlighted ? 0.32f : 0.18f);
                ringRenderer.transform.localPosition = ringBasePosition + new Vector3(0f, Mathf.Sin(Time.time * 3f + phase) * 0.014f, 0f);
                ringRenderer.transform.localScale = ringBaseScale * ringPulse;
            }

            transform.localScale = originalScale;
        }

        private Color GetAccentColor()
        {
            switch (hostType)
            {
                case HostType.Jumper:
                    return new Color(0.24f, 0.85f, 1f);
                case HostType.Dasher:
                    return new Color(0.58f, 0.96f, 0.36f);
                case HostType.Shooter:
                    return new Color(1f, 0.72f, 0.30f);
                case HostType.Phase:
                    return new Color(0.76f, 0.56f, 1f);
                case HostType.Anchor:
                    return new Color(0.44f, 0.86f, 0.42f);
                default:
                    return new Color(0.9f, 0.95f, 1f);
            }
        }
    }
}

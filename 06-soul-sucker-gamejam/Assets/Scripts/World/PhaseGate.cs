using SoulSucker.Data;
using SoulSucker.Hosts;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PhaseGate : MonoBehaviour
    {
        [SerializeField] private PhaseGateData gateData;

        private BoxCollider2D gateCollider;
        private SpriteRenderer gateRenderer;

        public string GateId => gateData != null ? gateData.id : string.Empty;
        public BoxCollider2D GateCollider => gateCollider;

        private void Awake()
        {
            EnsureComponents();
        }

        private void Update()
        {
            if (gateRenderer == null)
            {
                return;
            }

            float pulse = 0.84f + (Mathf.Sin(Time.time * 2.8f) * 0.08f);
            gateRenderer.color = new Color(0.76f, 0.56f, 1f, 0.56f * pulse);
        }

        public void Initialize(PhaseGateData data)
        {
            gateData = data;
            EnsureComponents();
        }

        public void EjectBody(HostBase host, int preferredDirection)
        {
            if (host == null || gateCollider == null)
            {
                return;
            }

            Bounds gateBounds = gateCollider.bounds;
            Bounds hostBounds = host.HostCollider.bounds;
            Vector3 position = host.transform.position;
            const float padding = 0.08f;

            if (gateBounds.size.x <= gateBounds.size.y)
            {
                bool moveRight = preferredDirection >= 0;
                position.x = moveRight
                    ? gateBounds.max.x + hostBounds.extents.x + padding
                    : gateBounds.min.x - hostBounds.extents.x - padding;
            }
            else
            {
                bool moveUp = host.transform.position.y >= gateBounds.center.y;
                position.y = moveUp
                    ? gateBounds.max.y + hostBounds.extents.y + padding
                    : gateBounds.min.y - hostBounds.extents.y - padding;
            }

            host.transform.position = position;
            host.Body.position = position;
            host.Body.linearVelocity = Vector2.zero;
        }

        private void EnsureComponents()
        {
            if (gateCollider == null)
            {
                gateCollider = GetComponent<BoxCollider2D>();
                gateCollider.size = Vector2.one;
                gateCollider.isTrigger = false;
            }

            if (gateRenderer == null)
            {
                gateRenderer = GetComponent<SpriteRenderer>();
            }
        }
    }
}

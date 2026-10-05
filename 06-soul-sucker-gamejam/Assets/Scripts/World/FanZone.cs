using SoulSucker.Data;
using SoulSucker.Hosts;
using SoulSucker.Player;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class FanZone : MonoBehaviour, ILinkedToggleTarget
    {
        [SerializeField] private FanZoneData fanData;

        private BoxCollider2D zoneCollider;
        private SpriteRenderer spriteRenderer;
        private bool isActive;

        public string LinkId => fanData != null ? fanData.id : string.Empty;
        public bool IsActive => isActive;

        private void Awake()
        {
            EnsureComponents();
        }

        private void Update()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            float pulse = 0.84f + (Mathf.Sin(Time.time * 5f) * 0.08f);
            spriteRenderer.color = isActive
                ? new Color(0.34f, 0.86f, 1f, 0.18f * pulse)
                : new Color(0.18f, 0.28f, 0.36f, 0.08f);
        }

        public void Initialize(FanZoneData data)
        {
            fanData = data;
            EnsureComponents();
            isActive = data != null && data.startsEnabled;
        }

        public void SetLinkedActive(bool active)
        {
            isActive = active;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!isActive || fanData == null)
            {
                return;
            }

            HostBase host = other.GetComponent<HostBase>();
            if (host != null)
            {
                if (host.IsFanImmune)
                {
                    return;
                }

                host.Body.AddForce(fanData.force, ForceMode2D.Force);
                return;
            }

            ParasiteBody parasiteBody = other.GetComponent<ParasiteBody>();
            if (parasiteBody != null)
            {
                parasiteBody.Rigidbody.AddForce(fanData.force, ForceMode2D.Force);
            }
        }

        private void EnsureComponents()
        {
            if (zoneCollider == null)
            {
                zoneCollider = GetComponent<BoxCollider2D>();
                zoneCollider.size = Vector2.one;
                zoneCollider.isTrigger = true;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }
    }
}

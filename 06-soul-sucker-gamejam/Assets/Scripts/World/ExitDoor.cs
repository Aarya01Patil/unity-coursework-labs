using SoulSucker.Hosts;
using SoulSucker.Player;
using SoulSucker.Audio;
using UnityEngine;

namespace SoulSucker.World
{
    public sealed class ExitDoor : MonoBehaviour
    {
        [SerializeField] private Vector2Int exitCell;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer glowRenderer;
        private SpriteRenderer pulseRenderer;
        private SpriteRenderer coreRenderer;
        private Transform glowTransform;
        private Transform pulseTransform;
        private Transform coreTransform;

        public Vector2Int ExitCell => exitCell;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            glowTransform = transform.Find("Glow");
            pulseTransform = transform.Find("Pulse");
            coreTransform = transform.Find("Core");
            if (glowTransform != null)
            {
                glowRenderer = glowTransform.GetComponent<SpriteRenderer>();
            }

            if (pulseTransform != null)
            {
                pulseRenderer = pulseTransform.GetComponent<SpriteRenderer>();
            }

            if (coreTransform != null)
            {
                coreRenderer = coreTransform.GetComponent<SpriteRenderer>();
            }
        }

        private void Update()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            Color baseColor = new Color(0.22f, 0.82f, 0.54f);
            float pulse = 0.80f + (Mathf.Sin(Time.time * 3.2f) * 0.16f);
            spriteRenderer.color = Color.Lerp(baseColor, new Color(0.92f, 1f, 0.96f), 0.22f) * pulse;

            if (glowTransform != null)
            {
                float glowScale = 1.08f + (Mathf.Sin(Time.time * 2.4f) * 0.08f);
                glowTransform.localScale = new Vector3(1.48f * glowScale, 2.5f * glowScale, 1f);
                if (glowRenderer != null)
                {
                    glowRenderer.color = new Color(0.30f, 1f, 0.72f, 0.18f + (Mathf.Sin(Time.time * 2.9f) * 0.04f));
                }
            }

            if (pulseTransform != null)
            {
                float pulseScale = 1.0f + (Mathf.Sin(Time.time * 3.1f + 0.4f) * 0.05f);
                pulseTransform.localScale = new Vector3(1.15f * pulseScale, 2.05f * pulseScale, 1f);
                if (pulseRenderer != null)
                {
                    pulseRenderer.color = new Color(0.96f, 1f, 0.92f, 0.22f + (Mathf.Sin(Time.time * 3.6f) * 0.05f));
                }
            }

            if (coreTransform != null)
            {
                float coreScale = 1.0f + (Mathf.Sin(Time.time * 4.2f + 0.8f) * 0.04f);
                coreTransform.localScale = new Vector3(0.52f * coreScale, 1.48f * coreScale, 1f);
                if (coreRenderer != null)
                {
                    coreRenderer.color = new Color(0.98f, 1f, 0.96f, 0.30f + (Mathf.Sin(Time.time * 4.1f) * 0.04f));
                }
            }
        }

        public void Initialize(Vector2Int cell)
        {
            exitCell = cell;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerBrain brain = Object.FindFirstObjectByType<PlayerBrain>();
            LevelManager levelManager = Object.FindFirstObjectByType<LevelManager>();
            if (brain == null || levelManager == null)
            {
                return;
            }

            ParasiteBody parasiteBody = other.GetComponent<ParasiteBody>();
            if (parasiteBody != null && brain.CurrentState == PlayerBrain.PlayerState.ParasiteFree)
            {
                AudioRouter.EnsureInstance().PlayExit();
                levelManager.CompleteCurrentRoom();
                return;
            }

            HostBase host = other.GetComponent<HostBase>();
            if (host != null && host.IsControlled)
            {
                AudioRouter.EnsureInstance().PlayExit();
                levelManager.CompleteCurrentRoom();
            }
        }
    }
}

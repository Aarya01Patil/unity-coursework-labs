using SoulSucker.Data;
using UnityEngine;

namespace SoulSucker.World
{
    public sealed class BreakableBlock : MonoBehaviour
    {
        [SerializeField] private IntRect bounds;

        private Collider2D[] colliders;
        private SpriteRenderer[] renderers;

        public IntRect Bounds => bounds;

        private void Awake()
        {
            CacheComponents();
        }

        public void Initialize(IntRect rect)
        {
            bounds = rect;
            CacheComponents();
        }

        public void DestroyBlock()
        {
            CacheComponents();

            if (colliders != null)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        colliders[i].enabled = false;
                    }
                }
            }

            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] != null)
                    {
                        Color color = renderers[i].color;
                        renderers[i].color = new Color(color.r, color.g, color.b, 0f);
                    }
                }
            }

            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        private void CacheComponents()
        {
            if (colliders == null || colliders.Length == 0)
            {
                colliders = GetComponentsInChildren<Collider2D>(true);
            }

            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<SpriteRenderer>(true);
            }
        }
    }
}

using SoulSucker.Utilities;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetimeSeconds = 2f;
        [SerializeField] private float impactFlashSeconds = 0.14f;

        private Vector2 direction;
        private float speed;
        private float age;
        private bool isImpacting;
        private float impactAge;
        private Rigidbody2D body;
        private CircleCollider2D projectileCollider;
        private SpriteRenderer projectileRenderer;
        private SpriteRenderer impactRenderer;
        private Transform impactFlashTransform;

        public static Projectile Create(Vector3 worldPosition, Vector2 moveDirection, float projectileSpeed, Transform parent)
        {
            var projectileObject = new GameObject("Projectile");
            if (parent != null)
            {
                projectileObject.transform.SetParent(parent.parent, true);
            }

            projectileObject.transform.position = worldPosition;
            var projectile = projectileObject.AddComponent<Projectile>();
            projectile.Initialize(moveDirection, projectileSpeed);
            return projectile;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            projectileCollider = GetComponent<CircleCollider2D>();
            projectileCollider.radius = 0.18f;
            projectileCollider.isTrigger = true;

            projectileRenderer = GetComponent<SpriteRenderer>();
            projectileRenderer.sprite = PlaceholderSpriteLibrary.GetCircleSprite();
            projectileRenderer.color = new Color(1f, 0.92f, 0.44f);
            projectileRenderer.sortingOrder = 45;

            transform.localScale = new Vector3(0.35f, 0.35f, 1f);

            var impactFlashObject = new GameObject("ImpactFlash");
            impactFlashObject.transform.SetParent(transform, false);
            impactFlashObject.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            impactFlashTransform = impactFlashObject.transform;
            impactRenderer = impactFlashObject.AddComponent<SpriteRenderer>();
            impactRenderer.sprite = PlaceholderSpriteLibrary.GetCircleSprite();
            impactRenderer.color = new Color(1f, 0.98f, 0.82f, 0f);
            impactRenderer.sortingOrder = 46;
            impactFlashObject.SetActive(false);
        }

        private void Update()
        {
            if (isImpacting)
            {
                impactAge += Time.deltaTime;
                float impactProgress = Mathf.Clamp01(impactAge / Mathf.Max(0.01f, impactFlashSeconds));
                float flashScale = 0.5f + (impactProgress * 1.1f);
                Color flashColor = Color.Lerp(new Color(1f, 0.98f, 0.82f, 0.9f), Color.clear, impactProgress);
                impactFlashTransform.gameObject.SetActive(true);
                impactFlashTransform.localScale = new Vector3(flashScale, flashScale, 1f);
                impactRenderer.color = flashColor;
                projectileRenderer.color = new Color(projectileRenderer.color.r, projectileRenderer.color.g, projectileRenderer.color.b, Mathf.Lerp(0.18f, 0f, impactProgress));

                if (impactProgress >= 1f)
                {
                    DestroySelf();
                }
                return;
            }

            transform.position += (Vector3)(direction * (speed * Time.deltaTime));
            age += Time.deltaTime;
            if (age >= lifetimeSeconds)
            {
                DestroySelf();
            }
        }

        public void Initialize(Vector2 moveDirection, float projectileSpeed)
        {
            direction = moveDirection.normalized;
            speed = projectileSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isImpacting)
            {
                return;
            }

            BreakableBlock breakableBlock = other.GetComponent<BreakableBlock>();
            if (breakableBlock != null)
            {
                breakableBlock.DestroyBlock();
                BeginImpact(new Color(1f, 0.94f, 0.45f, 1f));
                return;
            }

            RemoteSwitch remoteSwitch = other.GetComponent<RemoteSwitch>();
            if (remoteSwitch != null)
            {
                remoteSwitch.HitByProjectile();
                BeginImpact(new Color(0.42f, 1f, 0.76f, 1f));
                return;
            }

            TurretNode turretNode = other.GetComponent<TurretNode>();
            if (turretNode != null)
            {
                turretNode.HitByProjectile();
                BeginImpact(new Color(0.42f, 1f, 0.76f, 1f));
                return;
            }

            if (other.GetComponent<SolidBlock>() != null || other.GetComponent<HazardZone>() != null || other.GetComponent<SoulSucker.Hosts.HostBase>() != null)
            {
                BeginImpact(new Color(0.48f, 0.90f, 1f, 1f));
            }
        }

        private void BeginImpact(Color flashColor)
        {
            isImpacting = true;
            impactAge = 0f;
            speed = 0f;
            direction = Vector2.zero;
            if (body != null)
            {
                body.simulated = false;
            }

            if (projectileCollider != null)
            {
                projectileCollider.enabled = false;
            }

            if (impactRenderer != null)
            {
                impactRenderer.color = flashColor;
            }
        }

        private void DestroySelf()
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}

using SoulSucker.World;
using UnityEngine;

namespace SoulSucker.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ParasiteBody : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float hopImpulse = 3f;
        [SerializeField] private float groundProbeDistance = 0.1f;

        private Rigidbody2D body;
        private CircleCollider2D circleCollider;
        private SpriteRenderer spriteRenderer;
        private PlayerBrain owner;
        private bool isActive = true;

        public Rigidbody2D Rigidbody => body;
        public CircleCollider2D Collider => circleCollider;
        public SpriteRenderer Renderer => spriteRenderer;
        public bool IsActive => isActive;
        public float HopImpulse => hopImpulse;

        private void Awake()
        {
            EnsureComponents();
        }

        public void Initialize(PlayerBrain playerBrain)
        {
            owner = playerBrain;
            EnsureComponents();
            ConfigureAppearance();
            SetParasiteActive(true);
        }

        public void Tick(InputFrame inputFrame)
        {
            if (!isActive)
            {
                return;
            }

            Vector2 velocity = body.linearVelocity;
            velocity.x = inputFrame.moveX * moveSpeed;
            body.linearVelocity = velocity;

            if (inputFrame.jumpPressed && IsGrounded())
            {
                body.AddForce(Vector2.up * hopImpulse, ForceMode2D.Impulse);
            }
        }

        public void TeleportTo(Vector3 worldPosition)
        {
            EnsureComponents();
            transform.position = worldPosition;
            body.position = worldPosition;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        public void SetParasiteActive(bool value)
        {
            EnsureComponents();
            isActive = value;
            spriteRenderer.enabled = value;
            circleCollider.enabled = value;
            body.simulated = value;

            if (!value)
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive || owner == null)
            {
                return;
            }

            if (other.GetComponent<HazardZone>() != null)
            {
                owner.KillPlayer();
            }
        }

        private void EnsureComponents()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
                body.gravityScale = 2.5f;
                body.freezeRotation = true;
                body.interpolation = RigidbodyInterpolation2D.Interpolate;
                body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }

            if (circleCollider == null)
            {
                circleCollider = GetComponent<CircleCollider2D>();
                circleCollider.radius = 0.42f;
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void ConfigureAppearance()
        {
            spriteRenderer.sprite = Utilities.PlaceholderSpriteLibrary.GetCircleSprite();
            spriteRenderer.color = new Color(0.68f, 0.94f, 0.34f);
            spriteRenderer.sortingOrder = 40;
            transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        }

        private bool IsGrounded()
        {
            Vector2 origin = body.position;
            float radius = circleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y) * 0.9f;
            RaycastHit2D hit = Physics2D.CircleCast(origin, radius, Vector2.down, groundProbeDistance);
            return hit.collider != null && hit.collider != circleCollider;
        }
    }
}

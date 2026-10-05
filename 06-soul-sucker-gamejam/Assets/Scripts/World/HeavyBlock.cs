using SoulSucker.Data;
using SoulSucker.Hosts;
using SoulSucker.Utilities;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class HeavyBlock : MonoBehaviour
    {
        [SerializeField] private HeavyBlockData blockData;

        private Rigidbody2D body;
        private float anchorContactGraceTimer;

        public string BlockId => blockData != null ? blockData.id : string.Empty;

        private void Awake()
        {
            EnsureComponents();
        }

        public void Initialize(HeavyBlockData data)
        {
            blockData = data;
            EnsureComponents();
        }

        private void FixedUpdate()
        {
            EnsureComponents();
            anchorContactGraceTimer = Mathf.Max(0f, anchorContactGraceTimer - Time.fixedDeltaTime);
            if (anchorContactGraceTimer > 0f)
            {
                return;
            }

            Vector2 velocity = body.linearVelocity;
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, 24f * Time.fixedDeltaTime);
            body.linearVelocity = velocity;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.collider.GetComponent<AnchorHost>() != null)
            {
                anchorContactGraceTimer = 0.08f;
            }
        }

        private void EnsureComponents()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.isTrigger = false;

            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            body.bodyType = RigidbodyType2D.Dynamic;
            body.mass = 8f;
            body.gravityScale = 3.5f;
            body.linearDamping = 1f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderSpriteLibrary.GetSquareSprite();
            renderer.color = new Color(0.44f, 0.86f, 0.42f, 1f);
            renderer.sortingOrder = 22;
        }
    }
}

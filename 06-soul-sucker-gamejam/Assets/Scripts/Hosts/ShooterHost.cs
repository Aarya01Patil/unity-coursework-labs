using SoulSucker.Player;
using SoulSucker.World;
using UnityEngine;
using SoulSucker.Audio;

namespace SoulSucker.Hosts
{
    public sealed class ShooterHost : HostBase
    {
        [SerializeField] private float moveSpeed = 3.85f;
        [SerializeField] private float jumpImpulse = 4.8f;
        [SerializeField] private float groundProbeDistance = 0.18f;
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private float fireCooldown = 0.35f;

        private float fireCooldownTimer;
        private int facingDirection = 1;

        public override void TickControlled(InputFrame inputFrame, float deltaTime)
        {
            if (!IsControlled)
            {
                return;
            }

            fireCooldownTimer = Mathf.Max(0f, fireCooldownTimer - deltaTime);

            Vector2 velocity = Body.linearVelocity;
            velocity.x = inputFrame.moveX * moveSpeed;
            Body.linearVelocity = velocity;

            if (Mathf.Abs(inputFrame.moveX) > 0.01f)
            {
                facingDirection = inputFrame.moveX > 0f ? 1 : -1;
            }

            if (inputFrame.jumpPressed && IsGrounded(groundProbeDistance))
            {
                Body.linearVelocity = new Vector2(Body.linearVelocity.x, 0f);
                Body.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            }

            if (inputFrame.firePressed && fireCooldownTimer <= 0f)
            {
                FireProjectile();
                fireCooldownTimer = fireCooldown;
                AudioRouter.EnsureInstance().PlayShoot();
            }
        }

        protected override void ConfigureDefaults()
        {
            base.ConfigureDefaults();
            Body.gravityScale = 2.6f;
        }

        private void FireProjectile()
        {
            Vector3 spawnPosition = transform.position + new Vector3(facingDirection * 0.9f, 0.1f, 0f);
            Projectile.Create(spawnPosition, Vector2.right * facingDirection, projectileSpeed, transform);
        }

    }
}

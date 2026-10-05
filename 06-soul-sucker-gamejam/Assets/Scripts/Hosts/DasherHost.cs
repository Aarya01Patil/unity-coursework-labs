using SoulSucker.Audio;
using SoulSucker.Player;
using UnityEngine;

namespace SoulSucker.Hosts
{
    public sealed class DasherHost : HostBase
    {
        [SerializeField] private float moveSpeed = 4.2f;
        [SerializeField] private float dashSpeed = 24f;
        [SerializeField] private float dashDuration = 0.25f;
        [SerializeField] private float groundProbeDistance = 0.16f;
        [SerializeField] private float dashGravityScale = 0f;

        private bool canDash = true;
        private bool isDashing;
        private float dashTimer;
        private int facingDirection = 1;
        private float normalGravityScale = 3f;

        protected override bool IgnoreHazardsWhileControlled => isDashing;

        public override void TickControlled(InputFrame inputFrame, float deltaTime)
        {
            if (!IsControlled)
            {
                return;
            }

            bool grounded = IsGrounded(groundProbeDistance);
            if (grounded)
            {
                canDash = true;
            }

            if (Mathf.Abs(inputFrame.moveX) > 0.01f)
            {
                facingDirection = inputFrame.moveX > 0f ? 1 : -1;
            }

            if (isDashing)
            {
                dashTimer -= deltaTime;
                Body.gravityScale = dashGravityScale;
                Body.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);
                if (dashTimer <= 0f)
                {
                    isDashing = false;
                    Body.gravityScale = normalGravityScale;
                }
                return;
            }

            Body.gravityScale = normalGravityScale;
            Vector2 velocity = Body.linearVelocity;
            velocity.x = inputFrame.moveX * moveSpeed;
            Body.linearVelocity = velocity;

            if (inputFrame.jumpPressed && canDash)
            {
                isDashing = true;
                canDash = false;
                dashTimer = dashDuration;
                Body.gravityScale = dashGravityScale;
                Body.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);
                AudioRouter.EnsureInstance().PlayJump();
            }
        }

        public override void OnReleased(PlayerBrain brain)
        {
            isDashing = false;
            dashTimer = 0f;
            if (Body != null)
            {
                Body.gravityScale = normalGravityScale;
            }

            base.OnReleased(brain);
        }

        protected override void ConfigureDefaults()
        {
            base.ConfigureDefaults();
            normalGravityScale = 3f;
            Body.gravityScale = normalGravityScale;
            Body.mass = 0.95f;
        }
    }
}

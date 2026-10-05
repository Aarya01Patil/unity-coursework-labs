using SoulSucker.Player;
using SoulSucker.Audio;
using UnityEngine;

namespace SoulSucker.Hosts
{
    public sealed class JumperHost : HostBase
    {
        [SerializeField] private float groundMoveSpeed = 4.8f;
        [SerializeField] private float airMoveSpeed = 3.0f;
        [SerializeField] private float jumpImpulse = 13.5f;
        [SerializeField] private float groundProbeDistance = 0.22f;
        [SerializeField] private float minimumJumpInterval = 0.08f;

        private float lastJumpTime = -1f;

        public float JumpImpulse => jumpImpulse;

        public override void TickControlled(InputFrame inputFrame, float deltaTime)
        {
            if (!IsControlled)
            {
                return;
            }

            bool grounded = IsGrounded(groundProbeDistance);
            Vector2 velocity = Body.linearVelocity;
            velocity.x = inputFrame.moveX * (grounded ? groundMoveSpeed : airMoveSpeed);
            Body.linearVelocity = velocity;

            bool canJump = Time.time >= lastJumpTime + minimumJumpInterval;
            if (inputFrame.jumpPressed && canJump && grounded)
            {
                Body.linearVelocity = new Vector2(Body.linearVelocity.x, 0f);
                Body.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
                lastJumpTime = Time.time;
                AudioRouter.EnsureInstance().PlayJump();
            }
        }

        protected override void ConfigureDefaults()
        {
            base.ConfigureDefaults();
            Body.gravityScale = 2.2f;
        }
    }
}

using SoulSucker.Player;
using UnityEngine;

namespace SoulSucker.Hosts
{
    public sealed class AnchorHost : HostBase
    {
        [SerializeField] private float moveSpeed = 2.4f;
        [SerializeField] private float jumpImpulse = 2.8f;
        [SerializeField] private float groundProbeDistance = 0.18f;

        public override void TickControlled(InputFrame inputFrame, float deltaTime)
        {
            if (!IsControlled)
            {
                return;
            }

            Vector2 velocity = Body.linearVelocity;
            velocity.x = inputFrame.moveX * moveSpeed;
            Body.linearVelocity = velocity;

            if (inputFrame.jumpPressed && IsGrounded(groundProbeDistance))
            {
                Body.linearVelocity = new Vector2(Body.linearVelocity.x, 0f);
                Body.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            }
        }

        protected override void ConfigureDefaults()
        {
            base.ConfigureDefaults();
            Body.gravityScale = 3.4f;
            Body.mass = 10f;
            Body.linearDamping = 1.5f;
        }
    }
}

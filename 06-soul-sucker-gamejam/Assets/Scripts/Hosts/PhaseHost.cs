using System.Collections.Generic;
using SoulSucker.Audio;
using SoulSucker.Player;
using SoulSucker.World;
using UnityEngine;

namespace SoulSucker.Hosts
{
    public sealed class PhaseHost : HostBase
    {
        [SerializeField] private float moveSpeed = 4.1f;
        [SerializeField] private float phaseDuration = 1f;

        private readonly List<PhaseGate> activeGates = new();
        private float phaseTimer;
        private bool isPhasing;
        private int facingDirection = 1;

        public override void TickControlled(InputFrame inputFrame, float deltaTime)
        {
            if (!IsControlled)
            {
                return;
            }

            if (Mathf.Abs(inputFrame.moveX) > 0.01f)
            {
                facingDirection = inputFrame.moveX > 0f ? 1 : -1;
            }

            Vector2 velocity = Body.linearVelocity;
            velocity.x = inputFrame.moveX * moveSpeed;
            Body.linearVelocity = velocity;

            if (inputFrame.jumpPressed && !isPhasing)
            {
                BeginPhase();
            }

            if (!isPhasing)
            {
                return;
            }

            phaseTimer -= deltaTime;
            if (phaseTimer <= 0f)
            {
                EndPhase();
            }
        }

        public override void OnReleased(PlayerBrain brain)
        {
            EndPhase();
            base.OnReleased(brain);
        }

        protected override void ConfigureDefaults()
        {
            base.ConfigureDefaults();
            Body.gravityScale = 2.3f;
        }

        private void BeginPhase()
        {
            activeGates.Clear();
            foreach (PhaseGate gate in Object.FindObjectsByType<PhaseGate>(FindObjectsSortMode.None))
            {
                if (gate == null || gate.GateCollider == null)
                {
                    continue;
                }

                Physics2D.IgnoreCollision(HostCollider, gate.GateCollider, true);
                activeGates.Add(gate);
            }

            isPhasing = activeGates.Count > 0;
            phaseTimer = phaseDuration;
            if (isPhasing)
            {
                AudioRouter.EnsureInstance().PlayRelease();
            }
        }

        private void EndPhase()
        {
            if (!isPhasing && activeGates.Count == 0)
            {
                return;
            }

            for (int i = 0; i < activeGates.Count; i++)
            {
                PhaseGate gate = activeGates[i];
                if (gate == null || gate.GateCollider == null)
                {
                    continue;
                }

                Physics2D.IgnoreCollision(HostCollider, gate.GateCollider, false);
                if (HostCollider.bounds.Intersects(gate.GateCollider.bounds))
                {
                    gate.EjectBody(this, facingDirection);
                }
            }

            activeGates.Clear();
            isPhasing = false;
        }
    }
}

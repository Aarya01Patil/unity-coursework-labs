using SoulSucker.Data;
using SoulSucker.Hosts;
using SoulSucker.Player;
using UnityEngine;

namespace SoulSucker.World
{
    public sealed class HazardZone : MonoBehaviour
    {
        [SerializeField] private IntRect bounds;

        public IntRect Bounds => bounds;

        public void Initialize(IntRect rect)
        {
            bounds = rect;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ParasiteBody parasiteBody = other.GetComponent<ParasiteBody>();
            HostBase host = other.GetComponent<HostBase>();

            if (parasiteBody == null && host == null)
            {
                return;
            }

            if (host != null && !host.IsControlled)
            {
                return;
            }

            if (host != null && host.IgnoreHazards)
            {
                return;
            }

            PlayerBrain brain = Object.FindFirstObjectByType<PlayerBrain>();
            if (brain != null)
            {
                brain.KillPlayer();
            }
        }
    }
}

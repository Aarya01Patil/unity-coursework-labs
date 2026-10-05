using SoulSucker.Hosts;
using UnityEngine;

namespace SoulSucker.Player
{
    [DisallowMultipleComponent]
    public sealed class HostDetector : MonoBehaviour
    {
        [SerializeField] private float possessionRadius = 1.6f;

        private HostBase currentCandidate;

        public float PossessionRadius => possessionRadius;
        public HostBase CurrentCandidate => currentCandidate;

        public HostBase FindNearestHostCandidate(Vector3 sourcePosition)
        {
            HostBase[] hosts = Object.FindObjectsByType<HostBase>(FindObjectsSortMode.None);
            HostBase bestCandidate = null;
            float bestDistance = float.MaxValue;

            foreach (HostBase host in hosts)
            {
                if (host == null || !host.IsPossessable)
                {
                    continue;
                }

                float distance = Vector2.Distance(sourcePosition, host.transform.position);
                if (distance > possessionRadius || distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestCandidate = host;
            }

            return bestCandidate;
        }

        public HostBase RefreshCandidate(Vector3 sourcePosition)
        {
            HostBase nextCandidate = FindNearestHostCandidate(sourcePosition);
            if (currentCandidate == nextCandidate)
            {
                return currentCandidate;
            }

            if (currentCandidate != null)
            {
                currentCandidate.SetHighlighted(false);
            }

            currentCandidate = nextCandidate;

            if (currentCandidate != null)
            {
                currentCandidate.SetHighlighted(true);
            }

            return currentCandidate;
        }

        public void ClearCandidate()
        {
            if (currentCandidate != null)
            {
                currentCandidate.SetHighlighted(false);
                currentCandidate = null;
            }
        }
    }
}

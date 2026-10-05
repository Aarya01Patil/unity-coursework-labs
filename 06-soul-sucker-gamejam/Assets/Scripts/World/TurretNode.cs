using SoulSucker.Data;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class TurretNode : MonoBehaviour, ILinkedToggleTarget
    {
        [SerializeField] private TurretNodeData turretData;

        private GameObject beamRoot;
        private SpriteRenderer nodeRenderer;
        private bool isActive;

        public string LinkId => turretData != null ? turretData.id : string.Empty;
        public bool IsActive => isActive;

        private void Awake()
        {
            nodeRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(TurretNodeData data, GameObject beamObject)
        {
            turretData = data;
            beamRoot = beamObject;
            nodeRenderer = GetComponent<SpriteRenderer>();
            isActive = data != null && data.startsEnabled;
            ApplyState();
        }

        public void SetLinkedActive(bool active)
        {
            isActive = active;
            ApplyState();
        }

        public void HitByProjectile()
        {
            isActive = false;
            ApplyState();
        }

        private void ApplyState()
        {
            if (nodeRenderer != null)
            {
                nodeRenderer.color = isActive
                    ? new Color(1f, 0.58f, 0.20f, 1f)
                    : new Color(0.28f, 0.34f, 0.42f, 0.9f);
            }

            if (beamRoot != null)
            {
                beamRoot.SetActive(isActive);
            }
        }
    }
}

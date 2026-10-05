using System.Collections.Generic;
using SoulSucker.Data;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class RemoteSwitch : MonoBehaviour
    {
        [SerializeField] private RemoteSwitchData switchData;

        private readonly List<ILinkedToggleTarget> targets = new();
        private SpriteRenderer spriteRenderer;
        private bool isTriggered;

        public string SwitchId => switchData != null ? switchData.id : string.Empty;
        public bool IsTriggered => isTriggered;

        private void Awake()
        {
            EnsureComponents();
            ApplyVisualState();
        }

        public void Initialize(RemoteSwitchData data)
        {
            switchData = data;
            EnsureComponents();
            ApplyVisualState();
        }

        public void BindTargets(IEnumerable<ILinkedToggleTarget> linkedTargets)
        {
            targets.Clear();
            if (linkedTargets != null)
            {
                targets.AddRange(linkedTargets);
            }
        }

        public void HitByProjectile()
        {
            if (isTriggered)
            {
                return;
            }

            isTriggered = true;
            ApplyVisualState();

            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].SetLinkedActive(switchData.targetsEnabledWhenTriggered);
            }
        }

        private void EnsureComponents()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            collider.isTrigger = true;
        }

        private void ApplyVisualState()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.color = isTriggered
                ? new Color(0.24f, 0.92f, 0.66f, 1f)
                : new Color(1f, 0.76f, 0.24f, 1f);
        }
    }
}

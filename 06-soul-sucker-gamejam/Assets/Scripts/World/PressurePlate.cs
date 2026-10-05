using System.Collections.Generic;
using SoulSucker.Data;
using SoulSucker.Hosts;
using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PressurePlate : MonoBehaviour
    {
        [SerializeField] private PressurePlateData plateData;

        private readonly List<ILinkedToggleTarget> targets = new();
        private BoxCollider2D plateCollider;
        private SpriteRenderer plateRenderer;
        private bool isPressed;
        private Vector3 baseScale = Vector3.one;

        public bool IsPressed => isPressed;

        private void Awake()
        {
            EnsureComponents();
        }

        private void Update()
        {
            bool nextPressed = ComputePressedState();
            if (nextPressed != isPressed)
            {
                isPressed = nextPressed;
                SyncTargets();
            }

            if (plateRenderer != null)
            {
                plateRenderer.color = isPressed
                    ? new Color(0.44f, 0.96f, 0.56f, 1f)
                    : new Color(0.22f, 0.46f, 0.34f, 1f);
            }

            transform.localScale = isPressed ? new Vector3(baseScale.x, 0.75f * baseScale.y, baseScale.z) : baseScale;
        }

        public void Initialize(PressurePlateData data)
        {
            plateData = data;
            EnsureComponents();
            baseScale = transform.localScale;
            SyncTargets();
        }

        public void BindTargets(IEnumerable<ILinkedToggleTarget> linkedTargets)
        {
            targets.Clear();
            if (linkedTargets != null)
            {
                targets.AddRange(linkedTargets);
            }

            SyncTargets();
        }

        private void EnsureComponents()
        {
            if (plateCollider == null)
            {
                plateCollider = GetComponent<BoxCollider2D>();
                plateCollider.size = Vector2.one;
                plateCollider.isTrigger = true;
            }

            if (plateRenderer == null)
            {
                plateRenderer = GetComponent<SpriteRenderer>();
                baseScale = transform.localScale;
            }
        }

        private bool ComputePressedState()
        {
            if (plateCollider == null)
            {
                return false;
            }

            Collider2D[] hits = Physics2D.OverlapBoxAll(plateCollider.bounds.center, plateCollider.bounds.size * 0.95f, 0f);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hit = hits[i];
                if (hit == null || hit == plateCollider)
                {
                    continue;
                }

                if (hit.GetComponent<AnchorHost>() != null)
                {
                    return true;
                }

                if (hit.GetComponent<HeavyBlock>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void SyncTargets()
        {
            if (plateData == null)
            {
                return;
            }

            bool enabledState = isPressed
                ? plateData.targetsEnabledWhenPressed
                : !plateData.targetsEnabledWhenPressed;

            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].SetLinkedActive(enabledState);
            }
        }
    }
}

using UnityEngine;

namespace SoulSucker.World
{
    [DisallowMultipleComponent]
    public sealed class AmbientMote : MonoBehaviour
    {
        [SerializeField] private Vector3 baseLocalPosition;
        [SerializeField] private Vector3 baseScale = Vector3.one;
        [SerializeField] private Vector2 driftAmplitude = new Vector2(0.18f, 0.28f);
        [SerializeField] private float driftSpeed = 0.4f;
        [SerializeField] private float pulseAmount = 0.06f;
        [SerializeField] private float phaseOffset;

        private void Awake()
        {
            baseLocalPosition = transform.localPosition;
            baseScale = transform.localScale;
        }

        public void Configure(Vector2 amplitude, float speed, float pulse, float phase)
        {
            driftAmplitude = amplitude;
            driftSpeed = speed;
            pulseAmount = pulse;
            phaseOffset = phase;
            baseLocalPosition = transform.localPosition;
            baseScale = transform.localScale;
        }

        private void Update()
        {
            float phase = Time.time * driftSpeed + phaseOffset;
            transform.localPosition = baseLocalPosition + new Vector3(
                Mathf.Sin(phase * 0.7f) * driftAmplitude.x,
                Mathf.Cos(phase) * driftAmplitude.y,
                0f);

            float scalePulse = 1f + (Mathf.Sin((phase * 1.4f) + 0.3f) * pulseAmount);
            transform.localScale = baseScale * scalePulse;
        }
    }
}

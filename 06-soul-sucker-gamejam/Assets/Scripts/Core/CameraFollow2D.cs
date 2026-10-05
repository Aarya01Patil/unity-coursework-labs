using SoulSucker.Player;
using SoulSucker.Data;
using SoulSucker.World;
using UnityEngine;

namespace SoulSucker.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private float smoothTime = 0.18f;
        [SerializeField] private float horizontalLookAhead = 3.5f;
        [SerializeField] private float lookAheadReturnSpeed = 6f;
        [SerializeField] private bool followVertical = true;
        [SerializeField] private float verticalSmoothTime = 0.24f;

        private Camera gameplayCamera;
        private PlayerBrain playerBrain;
        private LevelManager levelManager;
        private Vector3 followVelocity;
        private float currentLookAhead;
        private float lookAheadVelocity;
        private Transform lastTarget;
        private Vector3 lastTargetPosition;

        private void Awake()
        {
            gameplayCamera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            EnsureReferences();
            if (playerBrain == null || levelManager == null)
            {
                return;
            }

            Transform target = playerBrain.CurrentCameraTarget;
            if (target == null)
            {
                return;
            }

            if (lastTarget != target)
            {
                lastTarget = target;
                lastTargetPosition = target.position;
            }

            Vector3 targetPosition = target.position;
            float deltaX = targetPosition.x - lastTargetPosition.x;
            float desiredLookAhead = Mathf.Abs(deltaX) > 0.01f ? Mathf.Sign(deltaX) * horizontalLookAhead : 0f;
            currentLookAhead = Mathf.SmoothDamp(currentLookAhead, desiredLookAhead, ref lookAheadVelocity, 1f / Mathf.Max(0.01f, lookAheadReturnSpeed));

            Vector3 desiredPosition = transform.position;
            desiredPosition.x = targetPosition.x + currentLookAhead;
            if (followVertical)
            {
                desiredPosition.y = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref followVelocity.y, verticalSmoothTime);
            }
            else
            {
                desiredPosition.y = 0f;
            }

            Rect bounds = GetCurrentBounds();
            Vector2 extents = GetCameraExtents();
            desiredPosition.x = ClampAxis(desiredPosition.x, bounds.xMin + extents.x, bounds.xMax - extents.x);
            desiredPosition.y = ClampAxis(desiredPosition.y, bounds.yMin + extents.y, bounds.yMax - extents.y);
            desiredPosition.z = transform.position.z;

            if (Vector3.Distance(transform.position, desiredPosition) > 14f)
            {
                transform.position = desiredPosition;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, smoothTime);
            }

            lastTargetPosition = targetPosition;
        }

        public void SnapToTarget()
        {
            EnsureReferences();
            if (playerBrain == null || levelManager == null || playerBrain.CurrentCameraTarget == null)
            {
                return;
            }

            Vector3 position = playerBrain.CurrentCameraTarget.position;
            Rect bounds = GetCurrentBounds();
            Vector2 extents = GetCameraExtents();
            position.x = ClampAxis(position.x, bounds.xMin + extents.x, bounds.xMax - extents.x);
            position.y = ClampAxis(followVertical ? position.y : 0f, bounds.yMin + extents.y, bounds.yMax - extents.y);
            position.z = transform.position.z;
            transform.position = position;
            followVelocity = Vector3.zero;
            lookAheadVelocity = 0f;
            currentLookAhead = 0f;
        }

        private void EnsureReferences()
        {
            if (playerBrain == null)
            {
                playerBrain = Object.FindFirstObjectByType<PlayerBrain>();
            }

            if (levelManager == null)
            {
                levelManager = Object.FindFirstObjectByType<LevelManager>();
            }

            if (gameplayCamera == null)
            {
                gameplayCamera = GetComponent<Camera>();
            }
        }

        private Vector2 GetCameraExtents()
        {
            float height = gameplayCamera != null ? gameplayCamera.orthographicSize : 9f;
            float width = height * (gameplayCamera != null ? gameplayCamera.aspect : (16f / 9f));
            return new Vector2(width, height);
        }

        private Rect GetCurrentBounds()
        {
            if (levelManager == null)
            {
                return new Rect(-16f, -9f, 32f, 18f);
            }

            return levelManager.GetCurrentCameraBoundsWorld();
        }

        private static float ClampAxis(float value, float min, float max)
        {
            if (min > max)
            {
                return (min + max) * 0.5f;
            }

            return Mathf.Clamp(value, min, max);
        }
    }
}

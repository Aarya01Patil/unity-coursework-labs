using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float currentAngle;

    void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        currentAngle -= rotationSpeed * Time.deltaTime;
        transform.SetPositionAndRotation(
            initialPosition,
            initialRotation * Quaternion.Euler(0f, 0f, currentAngle)
        );
    }
}

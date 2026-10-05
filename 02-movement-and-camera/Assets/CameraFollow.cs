using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPos = target.position;
        newPos.z = -10f;

        transform.position = Vector3.Lerp(
            transform.position,
            newPos,
            smoothSpeed * Time.deltaTime
        );
    }
}
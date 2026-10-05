using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float currentDistance;
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPos = target.position;
        newPos.z = -currentDistance;

        transform.position = newPos;
    }
}
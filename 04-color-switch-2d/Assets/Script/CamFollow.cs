using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float yOffset;
    [SerializeField] private bool useInitialOffset = true;

    private float fixedX;
    private float fixedZ;

    void Awake()
    {
        fixedX = transform.position.x;
        fixedZ = transform.position.z;

        if (target == null)
        {
            PlayerMove player = FindFirstObjectByType<PlayerMove>();
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void Start()
    {
        if (target != null && useInitialOffset)
        {
            yOffset = transform.position.y - target.position.y;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        transform.position = new Vector3(fixedX, target.position.y + yOffset, fixedZ);
    }
}

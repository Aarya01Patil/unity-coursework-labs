using UnityEngine;
using UnityEngine.InputSystem;
// Infinity Jump , 
public class BallController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        float move = 0f;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                move -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                move += 1f;
            }
        }

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
    }
}

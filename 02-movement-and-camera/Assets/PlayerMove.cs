using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float movementSpeed ;
    public float jumpForce;

    private Rigidbody2D rgbd;
    private bool isGrounded;
    private bool isTransitioning;

    public KeyCode leftbinding;
    public KeyCode rightbinding;
    public KeyCode jumpbinding;

    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        isGrounded = true;
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float direction = 0f;

        if (Input.GetKey(leftbinding))
            direction = -2f;

        if (Input.GetKey(rightbinding))
            direction = 2f;

        rgbd.linearVelocity = new Vector2(direction * movementSpeed, rgbd.linearVelocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(jumpbinding) && isGrounded)
        {
            rgbd.linearVelocity = new Vector2(rgbd.linearVelocity.x, 0f);
            rgbd.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollisionTags(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollisionTags(other.gameObject);
    }

    private void HandleCollisionTags(GameObject other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            return;
        }

        if (isTransitioning)
        {
            return;
        }

        if (other.CompareTag("Obstacle"))
        {
            isTransitioning = true;
            Debug.Log("Game Over");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (other.CompareTag("Goal"))
        {   
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

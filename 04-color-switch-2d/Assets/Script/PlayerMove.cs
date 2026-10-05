using UnityEngine;

public class PlayerMove : MonoBehaviour

{
    Rigidbody2D playerrgb2d;
    public float jumpForce = 10f;
    bool isInput = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerrgb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if(playerrgb2d != null)
        {
            if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                isInput = true;
            }
        }
    }
    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if(isInput){
            playerrgb2d.linearVelocity = new Vector2(0,1) * jumpForce;
            isInput = false;
        }
    }
}

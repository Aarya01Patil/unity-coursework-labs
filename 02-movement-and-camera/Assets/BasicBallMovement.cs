using UnityEngine;
using UnityEngine.InputSystem;

public class BasicBallMovement : MonoBehaviour

{

    public float movementUnit, jumpForce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BallMovement();   
    }
    void BallMovement(){
        Vector3 currPosition = transform.position;
        Vector3 newPos = currPosition;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed){
            newPos = new Vector3(currPosition.x - 1f, currPosition.y, currPosition.z);
        } 
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed){
            newPos = new Vector3(currPosition.x + 1f, currPosition.y, currPosition.z);
        }
        if (keyboard.wKey.isPressed || keyboard.spaceKey.isPressed){
            newPos = new Vector3(currPosition.x, currPosition.y + 1f, currPosition.z);
        }  
        transform.position = newPos;
    }
}

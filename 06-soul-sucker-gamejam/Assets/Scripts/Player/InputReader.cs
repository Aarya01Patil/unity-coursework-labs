using UnityEngine;

namespace SoulSucker.Player
{
    public sealed class InputReader
    {
        public InputFrame Read()
        {
            return new InputFrame
            {
                moveX = Input.GetAxisRaw("Horizontal"),
                jumpPressed = Input.GetKeyDown(KeyCode.Space),
                jumpHeld = Input.GetKey(KeyCode.Space),
                possessPressed = Input.GetKeyDown(KeyCode.E),
                firePressed = Input.GetKeyDown(KeyCode.J),
                restartPressed = Input.GetKeyDown(KeyCode.R),
            };
        }
    }
}

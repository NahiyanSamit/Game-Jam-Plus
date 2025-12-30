using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            JumpPressed = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttackPressed = true;
    }

    public void ConsumeJump() => JumpPressed = false;
    public void ConsumeAttack() => AttackPressed = false;
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 15f;
    public float jumpForce = 8f;

    private Rigidbody _rb;
    private PlayerInputHandler _input;
    private float _groundCheckDist;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInputHandler>();
        _groundCheckDist = GetComponent<Collider>().bounds.extents.y;
    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    void Move()
    {
        Vector3 move = new Vector3(_input.MoveInput.x, 0, _input.MoveInput.y);
        _rb.MovePosition(_rb.position + move * moveSpeed * Time.fixedDeltaTime);

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            _rb.MoveRotation(
                Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime)
            );
        }
    }

    void Jump()
    {
        if (_input.JumpPressed && IsGrounded())
        {
            if (GameManager.Instance.HasAbility(AbilityType.Jump))
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            _input.ConsumeJump();
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDist + 0.3f);
    }
}
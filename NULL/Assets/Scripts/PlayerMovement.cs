using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f;
    public float jumpForce = 8f;

    [Header("Sound")]
    public AudioClip footstepSound;
    public AudioClip jumpSound;
    public float footstepRate = 0.4f;
    public float footstepBlockAfterJump = 1.0f;

    private Rigidbody _rb;
    private PlayerInputHandler _input;
    private PlayerAnimationDriver _anim;

    private float _groundCheckDist;
    private float _nextFootstepTime;
    private float _footstepBlockUntil; 

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInputHandler>();
        _anim = GetComponent<PlayerAnimationDriver>();

        _groundCheckDist = GetComponent<Collider>().bounds.extents.y;
    }

    void FixedUpdate()
    {
        Move();
        HandleJump();
        HandleFootsteps();
    }

    // ================= MOVE =================
    void Move()
    {
        Vector3 move = new Vector3(_input.MoveInput.x, 0f, _input.MoveInput.y);
        _rb.MovePosition(_rb.position + move * (moveSpeed * Time.fixedDeltaTime));

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            _rb.MoveRotation(
                Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime)
            );
        }
    }

    // ================= JUMP =================
    void HandleJump()
    {
        if (_input.JumpPressed && IsGrounded())
        {
            if (GameManager.Instance.HasAbility(AbilityType.Jump))
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                // 🎬 Animation
                _anim?.PlayJump();

                //  Block footsteps for 0.75s
                _footstepBlockUntil = Time.time + footstepBlockAfterJump;

                //  Jump to sound (only if unlocked)
                if (GameManager.Instance.HasAbility(AbilityType.Sound))
                {
                    SoundManager.Instance?.PlaySFX(jumpSound);
                }
            }

            _input.ConsumeJump();
        }
    }

    // ================= FOOTSTEPS =================
    void HandleFootsteps()
    {
        if (Time.time < _footstepBlockUntil) return; // block after jump
        if (!IsGrounded()) return;
        if (_input.MoveInput.magnitude < 0.2f) return;
        if (Time.time < _nextFootstepTime) return;

        if (GameManager.Instance.HasAbility(AbilityType.Sound))
        {
            SoundManager.Instance?.PlaySFX(footstepSound);
        }

        _nextFootstepTime = Time.time + footstepRate;
    }

    // ================= GROUND CHECK =================
    bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            _groundCheckDist + 0.01f
        );
    }
}

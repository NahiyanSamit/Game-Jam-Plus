using UnityEngine;

public class PlayerAnimationDriver : MonoBehaviour
{
    // Animator parameter hashes
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Punch = Animator.StringToHash("Punch");

    [Header("References")]
    public Animator animator;

    private PlayerInputHandler _input;

    void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        if (!animator || _input == null) return;

        // Run / Idle animation
        animator.SetFloat(Speed, _input.MoveInput.magnitude);
    }

    // ===== Called by other scripts =====

    public void PlayJump()
    {
        if (!animator) return;
        animator.ResetTrigger(Jump);   // safety
        animator.SetTrigger(Jump);
    }

    public void PlayPunch()
    {
        if (!animator) return;
        animator.SetTrigger(Punch);
    }
}
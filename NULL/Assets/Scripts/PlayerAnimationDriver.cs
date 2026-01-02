using UnityEngine;

public class PlayerAnimationDriver : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Jump  = Animator.StringToHash("Jump");
    private static readonly int Punch = Animator.StringToHash("Punch");

    public Animator animator;
    private PlayerInputHandler _input;

    void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        if (!animator || _input == null) return;
        animator.SetFloat(Speed, _input.MoveInput.magnitude);
    }

    public void PlayJump()
    {
        animator.ResetTrigger(Jump);
        animator.SetTrigger(Jump);
    }

    public void PlayPunch()
    {
        animator.ResetTrigger(Punch);   
        animator.SetTrigger(Punch);
    }
}
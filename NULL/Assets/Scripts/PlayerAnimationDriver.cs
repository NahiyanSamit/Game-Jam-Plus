using UnityEngine;

public class PlayerAnimationDriver : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    public Animator animator;
    private PlayerInputHandler _input;

    void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        if (!animator) return;
        animator.SetFloat(Speed, _input.MoveInput.magnitude);
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CameraFollow gameCamera; 
    
    // DRAG YOUR 'Main Character' (the one with the Animator) HERE
    public Animator characterAnimator; 

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f; 
    public float jumpForce = 8f;

    private Rigidbody _rb;
    private float _distToGround;
    private bool _jumpRequest; 
    
    private bool _isCameraActive = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _distToGround = GetComponent<Collider>().bounds.extents.y;
        
        // Only show message if it exists (prevents errors if you removed MessageManager)
        if (MessageManager.Instance != null)
            MessageManager.Instance.ShowMessage("Welcome to the game!\n\n" + "<b>Now you will understand the PAIN behind FUN -_- <b>", 5f);
    }
    
    void Update()
    {
        if (GameManager.Instance == null) return;

        // --- JUMP LOGIC ---
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            if (GameManager.Instance.HasAbility(AbilityType.Jump))
            {
                _jumpRequest = true;

                // TRIGGER JUMP ANIMATION
                // We check if the animator is assigned and if the GameObject is actually active
                if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
                {
                    characterAnimator.SetTrigger("Jump");
                }
            }
            else
            {
                Debug.Log("You haven't unlocked Jump yet!");
            }
        }
        
        // --- CAMERA LOGIC ---
        if (!_isCameraActive && GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            if (gameCamera != null)
            {
                gameCamera.StartFollowing(transform);
                _isCameraActive = true; 
            }
        }
    }
    
    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputVector = new Vector3(horizontalInput, 0f, verticalInput);
        
        // 1. Move Logic
        Vector3 movement = inputVector * moveSpeed;
        Vector3 targetPos = _rb.position + movement * Time.fixedDeltaTime;
        _rb.MovePosition(targetPos);

        // 2. Rotate Smoothly
        if (inputVector.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputVector);
            Quaternion nextRotation = Quaternion.Slerp(_rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(nextRotation);
        }

        // --- ANIMATION MOVEMENT LOGIC ---
        if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
        {
            // Pass the movement speed to the Animator
            // 0 = Idle, 1 (or more) = Run
            float currentSpeed = inputVector.magnitude; 
            characterAnimator.SetFloat("Speed", currentSpeed);
        }
        
        // 3. Jump Physics
        if (_jumpRequest)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequest = false;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.5f);
    }
}
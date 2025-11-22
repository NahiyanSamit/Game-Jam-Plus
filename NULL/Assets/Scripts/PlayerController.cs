using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CameraFollow gameCamera; 
    public Animator characterAnimator; 

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f; 
    public float jumpForce = 8f;

    private Rigidbody _rb;
    private float _distToGround;
    private bool _jumpRequest; 
    private bool _isCameraActive = false;

    // Store model positions to fix animation drift
    private Vector3 _initialModelLocalPos;
    private Quaternion _initialModelLocalRot;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _distToGround = GetComponent<Collider>().bounds.extents.y;
        
        if (MessageManager.Instance != null)
            MessageManager.Instance.ShowMessage("Welcome to the game!\n\n" + "<b>Now you will understand the PAIN behind FUN -_- <b>", 5f);

        if (characterAnimator != null)
        {
            _initialModelLocalPos = characterAnimator.transform.localPosition;
            _initialModelLocalRot = characterAnimator.transform.localRotation;
        }
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
                if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
                    characterAnimator.SetTrigger("Jump");
            }
            else
            {
                Debug.Log("You haven't unlocked Jump yet!");
            }
        }

        // ----------------------------------------------------
        // --- NEW: PUNCH LOGIC (Left Click) ---
        // ----------------------------------------------------
        if (Input.GetMouseButtonDown(0)) // 0 is Left Click
        {
            if (GameManager.Instance.HasAbility(AbilityType.Punch))
            {
                if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
                {
                    characterAnimator.SetTrigger("Punch");
                    // Optional: Add sound effect here later
                }
            }
            else
            {
                Debug.Log("You haven't unlocked Punching yet!");
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
        
        // Move Logic
        Vector3 movement = inputVector * moveSpeed;
        Vector3 targetPos = _rb.position + movement * Time.fixedDeltaTime;
        _rb.MovePosition(targetPos);

        // Rotate Smoothly
        if (inputVector.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputVector);
            Quaternion nextRotation = Quaternion.Slerp(_rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(nextRotation);
        }

        // Animation Parameters
        if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
        {
            float currentSpeed = inputVector.magnitude; 
            characterAnimator.SetFloat("Speed", currentSpeed);
        }
        
        // Jump Physics
        if (_jumpRequest)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequest = false;
        }
    }

    void LateUpdate()
    {
        if (characterAnimator != null)
        {
            characterAnimator.transform.localPosition = _initialModelLocalPos;
            characterAnimator.transform.localRotation = _initialModelLocalRot;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.5f);
    }
}
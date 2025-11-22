using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CameraFollow gameCamera; 

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f; // Controls rotation smoothness
    public float jumpForce = 8f;

    private Rigidbody _rb;
    private float _distToGround;
    private bool _jumpRequest; 
    
    private bool _isCameraActive = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _distToGround = GetComponent<Collider>().bounds.extents.y;
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
            }
            else
            {
                Debug.Log("You haven't unlocked Jump yet!");
            }
        }
        
        if (!_isCameraActive && GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            if (gameCamera != null)
            {
                gameCamera.StartFollowing(transform);
                _isCameraActive = true; 
                Debug.Log("Camera activated!");
            }
            else
            {
                Debug.LogWarning("Camera ability unlocked, but 'Game Camera' is not assigned!");
            }
        }
    }
    
    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputVector = new Vector3(horizontalInput, 0f, verticalInput);
        
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
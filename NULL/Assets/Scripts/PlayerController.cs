using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    // Drag your Main Camera (with CameraFollow script) here in the Inspector
    public CameraFollow gameCamera; 

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    private Rigidbody _rb;
    private float _distToGround;
    private bool _jumpRequest; 
    
    // Internal flag to ensure we only activate the camera once
    private bool _isCameraActive = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _distToGround = GetComponent<Collider>().bounds.extents.y;
    }
    
    void Update()
    {
        // --- Safety Check for GameManager ---
        if (GameManager.Instance == null) return;

        // --- 1. JUMP LOGIC ---
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

        // --- 2. CAMERA LOGIC ---
        // If we have the ability, but haven't turned on the camera yet...
        if (!_isCameraActive && GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            if (gameCamera != null)
            {
                // Tell the camera to start following THIS player
                gameCamera.StartFollowing(transform);
                _isCameraActive = true; 
                Debug.Log("Camera activated!");
            }
            else
            {
                Debug.LogWarning("Camera ability unlocked, but 'Game Camera' is not assigned in PlayerController!");
            }
        }
    }
    
    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed;

        Vector3 targetPos = _rb.position + movement * Time.fixedDeltaTime;
        _rb.MovePosition(targetPos);

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
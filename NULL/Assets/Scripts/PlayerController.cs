using UnityEngine;
using System.Collections.Generic; // <--- REQUIRED for Queue

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CameraFollow gameCamera; 
    public Animator characterAnimator; 

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f; 
    public float jumpForce = 8f;

    [Header("Punch Settings")]
    public float punchRange = 1.5f;             
    public Vector3 hitOffset = new Vector3(0, 1f, 1f); 
    public LayerMask breakableLayer;            

    [Header("Respawn Settings")]
    public float fallThreshold = -10f; // If Y is lower than this, respawn
    public float historyDuration = 2.0f; // How many seconds to remember

    private Rigidbody _rb;
    private float _distToGround;
    private bool _jumpRequest; 
    private bool _isCameraActive = false;

    // Fix for animation drift
    private Vector3 _initialModelLocalPos;
    private Quaternion _initialModelLocalRot;

    // --- RESPAWN HISTORY ---
    // A Queue acts like a line of people. First in, First out.
    private Queue<Vector3> _positionHistory = new Queue<Vector3>();

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _distToGround = GetComponent<Collider>().bounds.extents.y;
        
        if (MessageManager.Instance != null)
            MessageManager.Instance.ShowMessage("Welcome!", 5f);

        if (characterAnimator != null)
        {
            _initialModelLocalPos = characterAnimator.transform.localPosition;
            _initialModelLocalRot = characterAnimator.transform.localRotation;
        }
    }
    
    void Update()
    {
        if (GameManager.Instance == null) return;

        // --------------------------------------------
        // 1. CHECK FOR FALLING (RESPAWN LOGIC)
        // --------------------------------------------
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }

        // --- JUMP LOGIC ---
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            if (GameManager.Instance.HasAbility(AbilityType.Jump))
            {
                _jumpRequest = true;
                if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
                    characterAnimator.SetTrigger("Jump");
            }
        }

        // --- PUNCH LOGIC ---
        if (Input.GetMouseButtonDown(0)) 
        {
            if (GameManager.Instance.HasAbility(AbilityType.Punch))
            {
                if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
                    characterAnimator.SetTrigger("Punch");

                CheckForBreakables();
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

    void CheckForBreakables()
    {
        Vector3 spherePos = transform.position + (transform.up * hitOffset.y) + (transform.forward * hitOffset.z);
        Collider[] hitColliders = Physics.OverlapSphere(spherePos, punchRange, breakableLayer);

        foreach (var hit in hitColliders)
        {
            BreakableBox box = hit.GetComponent<BreakableBox>();
            if (box != null) box.TakeDamage(); 
        }
    }

    void FixedUpdate()
    {
        // --- MOVEMENT ---
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputVector = new Vector3(horizontalInput, 0f, verticalInput);
        
        Vector3 movement = inputVector * moveSpeed;
        Vector3 targetPos = _rb.position + movement * Time.fixedDeltaTime;
        _rb.MovePosition(targetPos);

        if (inputVector.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputVector);
            Quaternion nextRotation = Quaternion.Slerp(_rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(nextRotation);
        }

        if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
            characterAnimator.SetFloat("Speed", inputVector.magnitude);
        
        if (_jumpRequest)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequest = false;
        }

        // --------------------------------------------
        // 2. RECORD POSITION HISTORY
        // --------------------------------------------
        // Only record history if we are SAFELY on the ground
        // This prevents respawning in mid-air over the pit
        if (IsGrounded())
        {
            // Add current position to the back of the line
            _positionHistory.Enqueue(transform.position);

            // If the line is too long (older than 2 seconds), remove the oldest point
            // FixedDeltaTime is usually 0.02s. 
            // So 50 frames = 1 second. 100 frames = 2 seconds.
            // Formula: Count > Seconds / DeltaTime
            if (_positionHistory.Count > (historyDuration / Time.fixedDeltaTime))
            {
                _positionHistory.Dequeue(); // Remove the oldest position
            }
        }
    }

    // --------------------------------------------
    // 3. THE RESPAWN FUNCTION
    // --------------------------------------------
    void Respawn()
    {
        if (_positionHistory.Count > 0)
        {
            // Peek looks at the oldest item in the list (the one from 2 seconds ago)
            Vector3 safeSpot = _positionHistory.Peek();
            
            // Teleport player
            transform.position = safeSpot;
            
            // IMPORTANT: Stop the falling physics!
            _rb.linearVelocity = Vector3.zero; 
            _rb.angularVelocity = Vector3.zero;
            
            Debug.Log("Respawned at position from 2 seconds ago!");
        }
        else
        {
            // Fallback if history is empty (start of game)
            transform.position = new Vector3(0, 2, 0);
            _rb.linearVelocity = Vector3.zero;
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

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Vector3 spherePos = transform.position + (transform.up * hitOffset.y) + (transform.forward * hitOffset.z);
        Gizmos.DrawWireSphere(spherePos, punchRange);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.5f);
    }
}
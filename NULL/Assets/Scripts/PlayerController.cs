using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems; // Required for UI blocking

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CameraFollow gameCamera; 
    public Animator characterAnimator; 

    [Header("Audio Clips")]
    public AudioClip jumpSound;
    public AudioClip punchSound;
    public AudioClip footstepSound;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f; 
    public float jumpForce = 8f;
    public float footstepRate = 0.5f; 

    [Header("Punch Settings")]
    public float punchRange = 1.5f;             
    public Vector3 hitOffset = new Vector3(0, 1f, 1f); 
    public LayerMask breakableLayer; 

    [Header("Respawn Settings")]
    public float fallThreshold = -10f; 
    public float historyDuration = 2.0f; 

    private Rigidbody _rb;
    private float _distToGround;
    private bool _jumpRequest; 
    private bool _isCameraActive = false;

    // Footstep timer
    private float _nextStepTime;

    // Fix for animation drift
    private Vector3 _initialModelLocalPos;
    private Quaternion _initialModelLocalRot;

    // History Queue
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

        // SAFETY CHECK: If we changed scenes, try to find the camera again automatically
        if (gameCamera == null)
        {
            gameCamera = FindFirstObjectByType<CameraFollow>();
        }
    }
    
    void Update()
    {
        if (GameManager.Instance == null) return;

        // 1. Fall Check
        if (transform.position.y < fallThreshold) Respawn();

        // 2. Jump Logic
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            if (GameManager.Instance.HasAbility(AbilityType.Jump))
            {
                _jumpRequest = true;
                if (characterAnimator != null) characterAnimator.SetTrigger("Jump");
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(jumpSound);
            }
        }

        // 3. PUNCH LOGIC (DEBUG VERSION)
        if (Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("1. Mouse Click Detected!"); // Check Console for this

            // Check UI Blocking
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                
                return;
            }

            // Check Ability
            if (!GameManager.Instance.HasAbility(AbilityType.Punch))
            {
                return;
            }

            // Check Animator
            if (characterAnimator == null)
            {
                return;
            }

            // SUCCESS
            Debug.Log("5. PUNCH SUCCESS! Playing Animation...");
            
            characterAnimator.SetTrigger("Punch");
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(punchSound);
            CheckForBreakables();
        }
        
        // 4. Camera
        if (!_isCameraActive && GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            if (gameCamera != null) { gameCamera.StartFollowing(transform); _isCameraActive = true; }
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
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputVector = new Vector3(horizontalInput, 0f, verticalInput);
        
        // Move
        Vector3 movement = inputVector * moveSpeed;
        Vector3 targetPos = _rb.position + movement * Time.fixedDeltaTime;
        _rb.MovePosition(targetPos);

        // Rotate
        if (inputVector.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputVector);
            Quaternion nextRotation = Quaternion.Slerp(_rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(nextRotation);
        }

        // Anim Speed
        if (characterAnimator != null && characterAnimator.isActiveAndEnabled)
            characterAnimator.SetFloat("Speed", inputVector.magnitude);
        
        // Jump Physics
        if (_jumpRequest)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequest = false;
        }

        // Footsteps
        if (inputVector.magnitude > 0.1f && IsGrounded() && Time.time > _nextStepTime)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(footstepSound);

            _nextStepTime = Time.time + footstepRate;
        }

        // History Recording
        if (IsGrounded())
        {
            _positionHistory.Enqueue(transform.position);
            if (_positionHistory.Count > (historyDuration / Time.fixedDeltaTime))
            {
                _positionHistory.Dequeue(); 
            }
        }
    }

    void Respawn()
    {
        if (_positionHistory.Count > 0)
        {
            transform.position = _positionHistory.Peek();
            _rb.linearVelocity = Vector3.zero; 
        }
        else
        {
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
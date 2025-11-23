using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CameraFollow gameCamera; 
    public Animator characterAnimator; 

    [Header("Gun Settings")]
    public GameObject gunModel;         
    public Transform muzzlePoint;       
    
    // --- CHANGE 1: We use GameObject now, it is easier to assign ---
    public GameObject muzzleFlashObject;  // Drag 'VFX_M4' here
    private ParticleSystem _muzzleFlashParticles; // Internal reference
    // ---------------------------------------------------------------

    public float shootingRange = 20f;   
    public LayerMask enemyLayer;        
    private int shootableMask; 

    [Header("Audio Clips")]
    public AudioClip jumpSound;
    public AudioClip punchSound;
    public AudioClip footstepSound;
    public AudioClip shootSound;

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
    private float _nextStepTime;
    
    private Vector3 _initialModelLocalPos;
    private Quaternion _initialModelLocalRot;
    private Queue<Vector3> _positionHistory = new Queue<Vector3>();

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _distToGround = GetComponent<Collider>().bounds.extents.y;
        shootableMask = enemyLayer | breakableLayer;
        
        if (characterAnimator != null)
        {
            _initialModelLocalPos = characterAnimator.transform.localPosition;
            _initialModelLocalRot = characterAnimator.transform.localRotation;
        }

        if (gameCamera == null) gameCamera = FindFirstObjectByType<CameraFollow>();

        // --- CHANGE 2: Auto-Find the Particle System ---
        if (muzzleFlashObject != null)
        {
            // We look for the particle component on the object you dragged in
            _muzzleFlashParticles = muzzleFlashObject.GetComponent<ParticleSystem>();
            
            // Safety: Stop it from playing at the start
            if (_muzzleFlashParticles != null) _muzzleFlashParticles.Stop(); 
        }
        // -----------------------------------------------

        if (gunModel != null)
        {
            gunModel.SetActive(false);
            if (GameManager.Instance != null && GameManager.Instance.HasAbility(AbilityType.Gun))
            {
                gunModel.SetActive(true);
            }
        }
        
        if (MessageManager.Instance != null)
            MessageManager.Instance.ShowMessage("Level Start!", 3f);
    }
    
    void Update()
    {
        if (GameManager.Instance == null) return;

        if (transform.position.y < fallThreshold) Respawn();

        if (gunModel != null && !gunModel.activeSelf)
        {
            if (GameManager.Instance.HasAbility(AbilityType.Gun))
                gunModel.SetActive(true);
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            if (GameManager.Instance.HasAbility(AbilityType.Jump))
            {
                _jumpRequest = true;
                if (characterAnimator != null) characterAnimator.SetTrigger("Jump");
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(jumpSound);
            }
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (GameManager.Instance.HasAbility(AbilityType.Gun))
            {
                ShootGun();
            }
            else if (GameManager.Instance.HasAbility(AbilityType.Punch))
            {
                PerformPunch();
            }
        }
        
        if (!_isCameraActive && GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            if (gameCamera != null) { gameCamera.StartFollowing(transform); _isCameraActive = true; }
        }
    }

    void ShootGun()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(shootSound);

        // --- CHANGE 3: Play the internal particle system ---
        if (_muzzleFlashParticles != null) 
        {
            _muzzleFlashParticles.Stop(); 
            _muzzleFlashParticles.Play();
        }
        // ---------------------------------------------------

        RaycastHit hit;
        Vector3 startPos = (muzzlePoint != null) ? muzzlePoint.position : transform.position + Vector3.up;
        
        if (Physics.Raycast(startPos, transform.forward, out hit, shootingRange, shootableMask))
        {
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage(1);

            BreakableBox box = hit.transform.GetComponent<BreakableBox>();
            if (box != null) box.TakeDamage();
        }
    }

    void PerformPunch()
    {
        if (characterAnimator != null) characterAnimator.SetTrigger("Punch");
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(punchSound);
        CheckForBreakables();
    }

    void CheckForBreakables()
    {
        Vector3 spherePos = transform.position + (transform.up * hitOffset.y) + (transform.forward * hitOffset.z);
        Collider[] hitColliders = Physics.OverlapSphere(spherePos, punchRange, breakableLayer);
        foreach (var hit in hitColliders) {
            BreakableBox box = hit.GetComponent<BreakableBox>();
            if (box != null) box.TakeDamage(); 
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

        if (inputVector.sqrMagnitude > 0.01f) {
            Quaternion targetRotation = Quaternion.LookRotation(inputVector);
            Quaternion nextRotation = Quaternion.Slerp(_rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(nextRotation);
        }

        if (characterAnimator != null) characterAnimator.SetFloat("Speed", inputVector.magnitude);
        if (_jumpRequest) { _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); _jumpRequest = false; }

        if (inputVector.magnitude > 0.1f && IsGrounded() && Time.time > _nextStepTime) {
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(footstepSound);
            _nextStepTime = Time.time + footstepRate;
        }

        if (IsGrounded()) { 
            _positionHistory.Enqueue(transform.position); 
            if (_positionHistory.Count > (historyDuration / Time.fixedDeltaTime)) _positionHistory.Dequeue(); 
        }
    }

    void Respawn() {
        if (_positionHistory.Count > 0) { transform.position = _positionHistory.Peek(); _rb.linearVelocity = Vector3.zero; }
        else { transform.position = new Vector3(0, 2, 0); _rb.linearVelocity = Vector3.zero; }
    }

    void LateUpdate() {
        if (characterAnimator != null) {
            characterAnimator.transform.localPosition = _initialModelLocalPos;
            characterAnimator.transform.localRotation = _initialModelLocalRot;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Vector3 spherePos = transform.position + (transform.up * hitOffset.y) + (transform.forward * hitOffset.z);
        Gizmos.DrawWireSphere(spherePos, punchRange);
        
        Gizmos.color = Color.yellow;
        Vector3 startPos = (muzzlePoint != null) ? muzzlePoint.position : transform.position + Vector3.up;
        Gizmos.DrawRay(startPos, transform.forward * shootingRange);
    }

    bool IsGrounded() { return Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.5f); }
}
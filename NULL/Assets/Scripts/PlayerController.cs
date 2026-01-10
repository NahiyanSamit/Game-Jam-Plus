using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Punch1 = Animator.StringToHash("Punch");

    [Header("References")]
    public CameraFollow gameCamera;
    public Animator characterAnimator;

    [Header("Gun Settings")]
    public GameObject gunModel;
    public Transform muzzlePoint;
    public GameObject muzzleFlashObject;

    [Header("Combat Settings")]
    public float shootingRange = 20f;
    public LayerMask enemyLayer;
    public LayerMask breakableLayer;

    [Header("Audio Clips")]
    public AudioClip jumpSound;
    public AudioClip punchSound;
    public AudioClip footstepSound;
    public AudioClip shootSound;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f;
    public float jumpForce = 8f;
    public float footstepRate = 0.5f;

    [Header("Punch")]
    public float punchRange = 1.5f;
    public Vector3 hitOffset = new Vector3(0, 1f, 1f);

    [Header("Respawn")]
    public float fallThreshold = -10f;
    public float historyDuration = 2f;

    private const int Damage = 2;

    private Rigidbody _rb;
    private ParticleSystem _muzzleFlash;
    private float _groundDist;
    private bool _jumpRequest;
    private bool _cameraActive;
    private float _nextStepTime;
    private int _shootMask;

    private Vector3 _initialModelPos;
    private Quaternion _initialModelRot;
    private Queue<Vector3> _positionHistory = new Queue<Vector3>();

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _groundDist = GetComponent<Collider>().bounds.extents.y;
        _shootMask = enemyLayer | breakableLayer;

        if (characterAnimator != null)
        {
            _initialModelPos = characterAnimator.transform.localPosition;
            _initialModelRot = characterAnimator.transform.localRotation;
        }

        if (!gameCamera)
            gameCamera = FindFirstObjectByType<CameraFollow>();

        if (muzzleFlashObject)
        {
            _muzzleFlash = muzzleFlashObject.GetComponent<ParticleSystem>();
            _muzzleFlash?.Stop();
        }

        gunModel?.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        if (transform.position.y < fallThreshold)
            Respawn();

        // Enable gun only if equipped
        gunModel?.SetActive(
            GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun &&
            GameManager.Instance.HasAbility(AbilityType.Gun)
        );

        HandleJump();
        HandleWeaponSwitch();
        HandleAttack();
        HandleCamera();
    }

    void FixedUpdate()
    {
        Move();
        HandleJumpPhysics();
        HandleFootsteps();
        SaveGroundedPosition();
    }

    // ================= MOVEMENT =================

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        _rb.MovePosition(_rb.position + input * (moveSpeed * Time.fixedDeltaTime));

        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(input);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, rot, turnSpeed * Time.fixedDeltaTime));
        }

        characterAnimator?.SetFloat(Speed, input.magnitude);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded() &&
            GameManager.Instance.HasAbility(AbilityType.Jump))
        {
            _jumpRequest = true;
            characterAnimator?.SetTrigger(Jump);
            SoundManager.Instance?.PlaySFX(jumpSound);
        }
    }

    void HandleJumpPhysics()
    {
        if (!_jumpRequest) return;

        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        _jumpRequest = false;
    }

    void HandleFootsteps()
    {
        if (!IsGrounded()) return;

        float speed = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z).magnitude;
        if (speed > 0.1f && Time.time > _nextStepTime)
        {
            SoundManager.Instance?.PlaySFX(footstepSound);
            _nextStepTime = Time.time + footstepRate;
        }
    }

    // ================= COMBAT =================

    void HandleWeaponSwitch()
    {
        if (!Input.GetButtonDown("Fire2")) return;
        if (!GameManager.Instance.HasAbility(AbilityType.Gun)) return;

        if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun &&
            GameManager.Instance.HasAbility(AbilityType.Punch))
        {
            GameManager.Instance.ChangeWeapon(AbilityType.Punch);
            gunModel.SetActive(false);
        }
        else
        {
            GameManager.Instance.ChangeWeapon(AbilityType.Gun);
            gunModel.SetActive(true);
        }
    }

    void HandleAttack()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun &&
            GameManager.Instance.HasAbility(AbilityType.Gun))
        {
            Shoot();
        }
        else if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Punch &&
                 GameManager.Instance.HasAbility(AbilityType.Punch))
        {
            Punch();
        }
    }

    void Shoot()
    {
        SoundManager.Instance?.PlaySFX(shootSound);
        _muzzleFlash?.Play();

        Vector3 start = muzzlePoint ? muzzlePoint.position : transform.position + Vector3.up;

        if (Physics.Raycast(start, transform.forward, out RaycastHit hit, shootingRange, _shootMask))
        {
            hit.collider.GetComponent<Health>()?.TakeDamage(Damage);
            hit.collider.GetComponent<BreakableBox>()?.TakeDamage();
        }
    }

    void Punch()
    {
        characterAnimator?.SetTrigger(Punch1);
        SoundManager.Instance?.PlaySFX(punchSound);

        Vector3 pos = transform.position + transform.forward * hitOffset.z + Vector3.up * hitOffset.y;
        Collider[] hits = Physics.OverlapSphere(pos, punchRange, breakableLayer);

        foreach (var h in hits)
            h.GetComponent<BreakableBox>()?.TakeDamage();
    }

    // ================= CAMERA =================

    void HandleCamera()
    {
        if (_cameraActive) return;
        if (!GameManager.Instance.HasAbility(AbilityType.Camera)) return;

        gameCamera?.StartFollowing(transform);
        _cameraActive = true;
    }

    // ================= UTIL =================

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundDist + 0.5f);
    }

    void SaveGroundedPosition()
    {
        if (!IsGrounded()) return;

        _positionHistory.Enqueue(transform.position);
        if (_positionHistory.Count > historyDuration / Time.fixedDeltaTime)
            _positionHistory.Dequeue();
    }

    public void Respawn()
    {
        transform.position = _positionHistory.Count > 0
            ? _positionHistory.Peek()
            : new Vector3(0, 2, 0);

        _rb.linearVelocity = Vector3.zero;
    }

    void LateUpdate()
    {
        if (!characterAnimator) return;
        characterAnimator.transform.localPosition = _initialModelPos;
        characterAnimator.transform.localRotation = _initialModelRot;
    }
}

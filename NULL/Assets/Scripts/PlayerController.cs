using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Punch1 = Animator.StringToHash("Punch");

    [Header("References")]
    public CameraFollow gameCamera;
    public Animator characterAnimator;

    [Header("Gun Settings")]
    public GameObject gunModel;
    public Transform muzzlePoint;
    public GameObject muzzleFlashObject;

    public float shootingRange = 20f;
    public LayerMask enemyLayer;
    public LayerMask breakableLayer;

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

    [Header("Respawn Settings")]
    public float fallThreshold = -10f;
    public float historyDuration = 2f;

    private const int Damage = 2;

    private Rigidbody _rb;
    private float _distToGround;
    private bool _jumpRequest;
    private bool _isCameraActive;
    private float _nextStepTime;

    private ParticleSystem _muzzleFlash;
    private int _shootMask;

    private Vector3 _initialModelPos;
    private Quaternion _initialModelRot;
    private Queue<Vector3> _positionHistory = new Queue<Vector3>();

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _distToGround = GetComponent<Collider>().bounds.extents.y;
        _shootMask = enemyLayer | breakableLayer;

        if (characterAnimator != null)
        {
            _initialModelPos = characterAnimator.transform.localPosition;
            _initialModelRot = characterAnimator.transform.localRotation;
        }

        if (gameCamera == null)
            gameCamera = FindFirstObjectByType<CameraFollow>();

        if (muzzleFlashObject != null)
        {
            _muzzleFlash = muzzleFlashObject.GetComponent<ParticleSystem>();
            if (_muzzleFlash != null) _muzzleFlash.Stop();
        }

        if (gunModel != null)
            gunModel.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        if (transform.position.y < fallThreshold)
            Respawn();

        // Enable gun if equipped
        if (gunModel != null && !gunModel.activeSelf &&
            GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun)
        {
            gunModel.SetActive(true);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            if (GameManager.Instance.HasAbility(AbilityType.Jump))
            {
                _jumpRequest = true;
                characterAnimator?.SetTrigger(Jump);
                SoundManager.Instance?.PlaySFX(jumpSound);
            }
        }

        // Switch weapon
        if (Input.GetButtonDown("Fire2") && GameManager.Instance.HasAbility(AbilityType.Gun))
        {
            if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun)
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

        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun)
                Shoot();
            else
                Punch();
        }

        // Camera ability
        if (!_isCameraActive && GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            gameCamera?.StartFollowing(transform);
            _isCameraActive = true;
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v);
        _rb.MovePosition(_rb.position + move * (moveSpeed * Time.fixedDeltaTime));

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, rot, turnSpeed * Time.fixedDeltaTime));
        }

        characterAnimator?.SetFloat(Speed, move.magnitude);

        if (_jumpRequest)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequest = false;
        }

        if (move.magnitude > 0.1f && IsGrounded() && Time.time > _nextStepTime)
        {
            SoundManager.Instance?.PlaySFX(footstepSound);
            _nextStepTime = Time.time + footstepRate;
        }

        if (IsGrounded())
        {
            _positionHistory.Enqueue(transform.position);
            if (_positionHistory.Count > historyDuration / Time.fixedDeltaTime)
                _positionHistory.Dequeue();
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

    public void Respawn()
    {
        transform.position = _positionHistory.Count > 0
            ? _positionHistory.Peek()
            : new Vector3(0, 2, 0);

        _rb.linearVelocity = Vector3.zero;
    }

    void LateUpdate()
    {
        if (characterAnimator != null)
        {
            characterAnimator.transform.localPosition = _initialModelPos;
            characterAnimator.transform.localRotation = _initialModelRot;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.5f);
    }
}

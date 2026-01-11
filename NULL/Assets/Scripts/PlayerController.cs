using System.Collections.Generic;
using DG.Tweening;
using SmallHedge.SoundManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CameraFollow gameCamera;
    public Animator characterAnimator;

    [Header("Gun Settings")]
    public GameObject gunModel;
    public Transform muzzlePoint;
    public GameObject muzzleFlashObject;
    private ParticleSystem _muzzleFlashParticles;

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

    [SerializeField] private int damage = 2;

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

        if (gameCamera == null)
            gameCamera = FindFirstObjectByType<CameraFollow>();

        if (muzzleFlashObject != null)
        {
            _muzzleFlashParticles = muzzleFlashObject.GetComponent<ParticleSystem>();
            if (_muzzleFlashParticles != null)
                _muzzleFlashParticles.Stop();
        }

        if (gunModel != null)
        {
            gunModel.SetActive(false);
            if (GameManager.Instance != null && GameManager.Instance.HasAbility(AbilityType.Gun))
                gunModel.SetActive(true);
        }

        if (MessageManager.Instance != null)
            MessageManager.Instance.ShowMessage("Level Start!", 3f);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        if (transform.position.y < fallThreshold)
            Respawn();

        if (gunModel != null && !gunModel.activeSelf)
        {
            if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun)
                gunModel.SetActive(true);
        }

        // ================= JUMP =================
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            if (GameManager.Instance.HasAbility(AbilityType.Jump))
            {
                _jumpRequest = true;
                if (characterAnimator != null)
                    characterAnimator.SetTrigger("Jump");

                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX(jumpSound);
            }
        }

        // ================= WEAPON SWITCH =================
        if (Input.GetButtonDown("Fire2"))
        {
            if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun &&
                GameManager.Instance.HasAbility(AbilityType.Gun))
            {
                GameManager.Instance.ChangeWeapon(AbilityType.Punch);
                gunModel.SetActive(false);
            }
            else if (GameManager.Instance.HasAbility(AbilityType.Gun))
            {
                GameManager.Instance.ChangeWeapon(AbilityType.Gun);
                gunModel.SetActive(true);
            }
        }

        // ================= ATTACK =================
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // 🔫 GUN
            if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun &&
                GameManager.Instance.HasAbility(AbilityType.Gun))
            {
                ShootGun();
            }
            // 👊 PUNCH (FIXED — ability required)
            else if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Punch &&
                     GameManager.Instance.HasAbility(AbilityType.Punch))
            {
                PerformPunch();
            }
        }

        // ================= CAMERA =================
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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputVector = new Vector3(h, 0f, v);

        Vector3 movement = inputVector * moveSpeed;
        _rb.MovePosition(_rb.position + movement * Time.fixedDeltaTime);

        if (inputVector.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputVector);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
        }

        if (characterAnimator != null)
            characterAnimator.SetFloat("Speed", inputVector.magnitude);

        if (_jumpRequest)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequest = false;
        }

        if (inputVector.magnitude > 0.1f && IsGrounded() && Time.time > _nextStepTime)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(footstepSound);

            _nextStepTime = Time.time + footstepRate;
        }

        if (IsGrounded())
        {
            _positionHistory.Enqueue(transform.position);
            if (_positionHistory.Count > (historyDuration / Time.fixedDeltaTime))
                _positionHistory.Dequeue();
        }
    }

    void ShootGun()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(shootSound);

        if (_muzzleFlashParticles != null)
        {
            _muzzleFlashParticles.Stop();
            _muzzleFlashParticles.Play();
        }

        RaycastHit hit;
        Vector3 startPos = muzzlePoint != null ? muzzlePoint.position : transform.position + Vector3.up;

        if (Physics.Raycast(startPos, transform.forward, out hit, shootingRange, shootableMask))
        {
            Health health = hit.collider.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage);

            BreakableBox box = hit.collider.GetComponent<BreakableBox>();
            if (box != null)
                box.TakeDamage();
        }

        Debug.DrawRay(startPos, transform.forward * shootingRange, Color.red, 1f);
    }

    void PerformPunch()
    {
        if (characterAnimator != null)
            characterAnimator.SetTrigger("Punch");

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(punchSound);

        Vector3 spherePos = transform.position + (transform.up * hitOffset.y) + (transform.forward * hitOffset.z);
        Collider[] hits = Physics.OverlapSphere(spherePos, punchRange, breakableLayer);

        foreach (var hit in hits)
        {
            BreakableBox box = hit.GetComponent<BreakableBox>();
            if (box != null)
                box.TakeDamage();
        }
    }

    public void Respawn()
    {
        if (_positionHistory.Count > 0)
            transform.position = _positionHistory.Peek();
        else
            transform.position = new Vector3(0, 2, 0);

        _rb.linearVelocity = Vector3.zero;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.5f);
    }
    
    public void DisableGun()
    {
        if (gunModel != null)
            gunModel.SetActive(false);
    }

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SmallHedge.SoundManager;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CameraFollow gameCamera;
    public Animator characterAnimator;

    [Header("Gun")]
    public GameObject gunModel;
    public Transform muzzlePoint;
    public GameObject muzzleFlashObject;
    private ParticleSystem muzzleFlash;

    public float shootingRange = 20f;
    public LayerMask enemyLayer;
    public LayerMask breakableLayer;
    private int shootMask;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip punchSound;
    public AudioClip shootSound;
    public AudioClip footstepSound;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f;
    public float jumpForce = 8f;
    public float footstepRate = 0.5f;

    [Header("Punch")]
    public float punchRange = 1.5f;
    public Vector3 hitOffset;

    private Rigidbody rb;
    private float groundDist;
    private bool jumpRequest;
    private bool cameraActive;
    private float nextFootstep;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundDist = GetComponent<Collider>().bounds.extents.y;
        shootMask = enemyLayer | breakableLayer;

        if (!gameCamera)
            gameCamera = FindFirstObjectByType<CameraFollow>();

        if (muzzleFlashObject)
        {
            muzzleFlash = muzzleFlashObject.GetComponent<ParticleSystem>();
            muzzleFlash?.Stop();
        }

        ApplyAbilitiesFromGameManager();
    }

    void Update()
    {
        if (!GameManager.Instance) return;

        // Jump
        if (Input.GetButtonDown("Jump") && IsGrounded() &&
            GameManager.Instance.HasAbility(AbilityType.Jump))
        {
            jumpRequest = true;
            characterAnimator?.SetTrigger("Jump");
            SoundManager.Instance?.PlaySFX(jumpSound);
        }

        // Weapon switch
        if (Input.GetButtonDown("Fire2"))
        {
            if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun &&
                GameManager.Instance.HasAbility(AbilityType.Punch))
            {
                GameManager.Instance.ChangeWeapon(AbilityType.Punch);
                gunModel.SetActive(false);
            }
            else if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Punch &&
                     GameManager.Instance.HasAbility(AbilityType.Gun))
            {
                GameManager.Instance.ChangeWeapon(AbilityType.Gun);
                gunModel.SetActive(true);
            }
        }

        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun &&
                GameManager.Instance.HasAbility(AbilityType.Gun))
                ShootGun();

            else if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Punch &&
                     GameManager.Instance.HasAbility(AbilityType.Punch))
                Punch();
        }

        // Camera
        if (!cameraActive && GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            gameCamera?.StartFollowing(transform);
            cameraActive = true;
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);

        if (move.sqrMagnitude > 0.01f)
        {
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation,
                Quaternion.LookRotation(move),
                turnSpeed * Time.fixedDeltaTime));
        }

        if (jumpRequest)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequest = false;
        }

        if (move.magnitude > 0.1f && IsGrounded() && Time.time > nextFootstep)
        {
            SoundManager.Instance?.PlaySFX(footstepSound);
            nextFootstep = Time.time + footstepRate;
        }
    }

    void ShootGun()
    {
        SoundManager.Instance?.PlaySFX(shootSound);
        muzzleFlash?.Play();

        if (Physics.Raycast(muzzlePoint.position,
            transform.forward, out RaycastHit hit, shootingRange, shootMask))
        {
            hit.collider.GetComponent<Health>()?.TakeDamage(2);
            hit.collider.GetComponent<BreakableBox>()?.TakeDamage();
        }
    }

    void Punch()
    {
        characterAnimator?.SetTrigger("Punch");
        SoundManager.Instance?.PlaySFX(punchSound);

        Vector3 pos = transform.position + transform.forward * hitOffset.z
                      + transform.up * hitOffset.y;

        Collider[] hits = Physics.OverlapSphere(pos, punchRange, breakableLayer);
        foreach (var h in hits)
            h.GetComponent<BreakableBox>()?.TakeDamage();
    }

    // 🔥 THIS MAKES ABILITY LOSS ACTUALLY WORK
    public void ApplyAbilitiesFromGameManager()
    {
        gunModel?.SetActive(false);
        cameraActive = false;
        jumpRequest = false;

        GameManager.Instance.ChangeWeapon(AbilityType.None);

        if (GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            gameCamera?.StartFollowing(transform);
            cameraActive = true;
        }

        if (GameManager.Instance.HasAbility(AbilityType.Gun))
        {
            GameManager.Instance.ChangeWeapon(AbilityType.Gun);
            gunModel.SetActive(true);
        }
        else if (GameManager.Instance.HasAbility(AbilityType.Punch))
        {
            GameManager.Instance.ChangeWeapon(AbilityType.Punch);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDist + 0.4f);
    }
}

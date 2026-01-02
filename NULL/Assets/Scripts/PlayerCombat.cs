using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    [Header("Gun")]
    public GameObject gunModel;
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public float shootingRange = 20f;
    public LayerMask shootMask;
    public AudioClip shootSound;

    [Header("Punch")]
    public float punchRange = 1.5f;
    public Vector3 hitOffset;
    public LayerMask breakableLayer;
    public AudioClip punchSound;

    private PlayerInputHandler _input;
    private PlayerAnimationDriver _anim;

    void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        _anim  = GetComponent<PlayerAnimationDriver>();

        if (muzzleFlash != null)
            muzzleFlash.Stop();
    }

    void Update()
    {
        if (!_input.AttackPressed)
            return;

        _input.ConsumeAttack();

        // Prevent attacking through UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (GameManager.Instance.HasAbility(AbilityType.Gun))
            Shoot();
        else if (GameManager.Instance.HasAbility(AbilityType.Punch))
            Punch();
    }

    // ================= SHOOT =================
    void Shoot()
    {
        SoundManager.Instance?.PlaySFX(shootSound);
        if (muzzleFlash) muzzleFlash.Play();

        Vector3 start = muzzlePoint ? muzzlePoint.position : transform.position + Vector3.up;

        if (Physics.Raycast(start, transform.forward, out RaycastHit hit, shootingRange, shootMask))
        {
            hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(1);
            hit.collider.GetComponent<BreakableBox>()?.TakeDamage();
        }
    }

    // ================= PUNCH =================
    void Punch()
    {
        _anim?.PlayPunch();

        // Sound
        SoundManager.Instance?.PlaySFX(punchSound);

        // Damage
        Vector3 pos =
            transform.position +
            transform.forward * hitOffset.z +
            Vector3.up * hitOffset.y;

        Collider[] hits =
            Physics.OverlapSphere(pos, punchRange, breakableLayer);

        foreach (var hit in hits)
            hit.GetComponent<BreakableBox>()?.TakeDamage();
    }

    // ================= DEBUG =================
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos =
            transform.position +
            transform.forward * hitOffset.z +
            Vector3.up * hitOffset.y;

        Gizmos.DrawWireSphere(pos, punchRange);
    }
}

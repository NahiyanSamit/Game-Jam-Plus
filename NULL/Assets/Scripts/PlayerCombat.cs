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

    void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        if (muzzleFlash != null) muzzleFlash.Stop();
    }

    void Update()
    {
        if (!_input.AttackPressed) return;
        _input.ConsumeAttack();

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (GameManager.Instance.HasAbility(AbilityType.Gun))
            Shoot();
        else if (GameManager.Instance.HasAbility(AbilityType.Punch))
            Punch();
    }

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

    void Punch()
    {
        SoundManager.Instance?.PlaySFX(punchSound);

        Vector3 pos = transform.position + transform.forward * hitOffset.z + Vector3.up * hitOffset.y;
        Collider[] hits = Physics.OverlapSphere(pos, punchRange, breakableLayer);

        foreach (var hit in hits)
            hit.GetComponent<BreakableBox>()?.TakeDamage();
    }
}
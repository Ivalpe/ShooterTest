using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Refs")]
    public Camera playerCamera;
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab; // pequeña partícula/decál de impacto
    public AudioSource audioSource;
    public AudioClip fireSFX;

    [Header("Stats")]
    public float damage = 25f;
    public float range = 200f;
    public float fireRate = 10f; // bullets per second
    public bool automatic = true;

    [Header("Animation")]
    public Animator animator; // opcional, trigger "Shoot"

    private float nextFireTime;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        bool wantsToFire = automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        if (wantsToFire && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Fire()
    {
        // FX
        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && fireSFX != null) audioSource.PlayOneShot(fireSFX);
        if (animator != null) animator.SetTrigger("Shoot");

        // Raycast
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            // Daño
            var health = 0;//hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Impact FX
            if (hitEffectPrefab != null)
            {
                var fx = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx, 2f);
            }

            // Pequeño empuje si hay Rigidbody
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(ray.direction * 5f, hit.point, ForceMode.Impulse);
            }
        }
    }
}

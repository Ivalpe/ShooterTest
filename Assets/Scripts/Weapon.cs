using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public GunRecoil gunRecoil; // referencia al script de retroceso

    [Header("References")]
    public Camera playerCamera;          // la cámara del jugador
    public Transform muzzlePoint;        // punto de salida del disparo
    public ParticleSystem muzzleFlash;   // efecto de fogonazo
    public AudioSource audioSource;      // fuente de audio
    public AudioClip fireSFX;            // sonido de disparo
    public GameObject hitEffectPrefab;   // partícula al impactar

    [Header("Stats")]
    public float range = 200f;           // distancia máxima del disparo
    public float fireRate = 10f;         // balas por segundo
    public bool automatic = true;        // mantener click o solo un disparo

    public GameObject bulletPrefab;   // asigna tu prefab de bala
    public float bulletSpeed = 50f;

    private float nextFireTime;

    public float spreadAngle = 2f;       // dispersión máxima
    public float spreadIncreaseRate = 0.5f; // cuánto aumenta por segundo disparando
    public float spreadRecoveryRate = 1f;   // cuánto se recupera por segundo sin disparar

    private float currentSpread = 0f;
    private bool isFiring = false;


    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;  // por si no la asignas manualmente
    }

    void Update()
    {
        bool wantsToFire = automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (wantsToFire && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }

        if (wantsToFire)
        {
            currentSpread += spreadIncreaseRate * Time.deltaTime;
            currentSpread = Mathf.Min(currentSpread, spreadAngle); // no superar el máximo
        }
        else
        {
            currentSpread -= spreadRecoveryRate * Time.deltaTime;
            currentSpread = Mathf.Max(currentSpread, 0f); // no ir a negativo
        }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float spreadAngle = 2f;
            Vector3 direction = -muzzlePoint.forward;
            direction = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * direction;

            rb.linearVelocity = direction * bulletSpeed;
        }

        Destroy(bullet, 5f);

        GunRecoil recoil = GetComponent<GunRecoil>();
        if (recoil != null) recoil.ApplyRecoil();

        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && fireSFX != null) audioSource.PlayOneShot(fireSFX);
    }

}

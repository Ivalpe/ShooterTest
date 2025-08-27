using UnityEngine;
using System.Collections;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip fireSFX;
    public GameObject hitEffectPrefab;
    public TextMeshProUGUI ammoText;

    [Header("Stats")]
    public int damage = 10;
    public float range = 200f;
    public float fireRate = 10f;
    public bool automatic = true;

    [Header("Ammo")]
    public int magazineSize = 30;
    public float reloadTime = 2f;
    private int bulletsLeft;
    private bool reloading;

    [Header("Projectile Mode")]
    public bool useProjectile = false;
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    [Header("Spread")]
    public float spreadAngle = 2f;
    public float spreadIncreaseRate = 0.5f;
    public float spreadRecoveryRate = 1f;
    private float currentSpread = 0f;

    private float nextFireTime;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        bulletsLeft = magazineSize;
    }

    private void Update()
    {
        // Recarga
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            StartCoroutine(Reload());
            return;
        }

        bool wantsToFire = automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (wantsToFire && Time.time >= nextFireTime && !reloading && bulletsLeft > 0)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }

        // Control de spread
        if (wantsToFire)
            currentSpread = Mathf.Min(currentSpread + spreadIncreaseRate * Time.deltaTime, spreadAngle);
        else
            currentSpread = Mathf.Max(currentSpread - spreadRecoveryRate * Time.deltaTime, 0f);

        // UI
        if (ammoText != null)
            ammoText.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void Fire()
    {
        bulletsLeft--;

        Vector3 shootDir = playerCamera.transform.forward;
        shootDir = Quaternion.Euler(Random.Range(-currentSpread, currentSpread), Random.Range(-currentSpread, currentSpread), 0) * shootDir;

        if (useProjectile)
        {
            // Balas físicas
            GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = shootDir * bulletSpeed;

            Destroy(bullet, 5f);
        }
        else
        {
            // Hitscan (Raycast)
            if (Physics.Raycast(playerCamera.transform.position, shootDir, out RaycastHit hit, range))
            {
                if (hitEffectPrefab != null)
                    Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                
                if (hit.collider.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(20); // daño configurable
                    }
                }
            }
        }

        // Recoil
        GunRecoil recoil = GetComponent<GunRecoil>();
        if (recoil != null) recoil.ApplyRecoil();

        // Efectos
        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && fireSFX != null) audioSource.PlayOneShot(fireSFX);
    }

    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        bulletsLeft = magazineSize;
        reloading = false;
    }
}

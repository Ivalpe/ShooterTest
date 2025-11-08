using UnityEngine;
using System.Collections;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip fireSFX;
    public GameObject hitEffectPrefab;

    [Header("Stats")]
    public int damage = 10;
    public float range = 200f;
    public float fireRate = 10f;
    public bool automatic = true;

    [Header("Ammo")]
    public int magazineSize = 30;
    public float reloadTime = 2f;
    public int bulletsLeft;
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
    private GameObject owner;

    private void Awake()
    {
        bulletsLeft = magazineSize;
    }
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    private void Update()
    {
        bool wantsToFire = automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
    }

    public void TryShoot()
    {
        currentSpread = Mathf.Min(currentSpread + spreadIncreaseRate * Time.deltaTime, spreadAngle);

        if (Time.time >= nextFireTime && !reloading && bulletsLeft > 0)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }

    }
    public void TryReload()
    {
        if (bulletsLeft < magazineSize && !reloading)
        {
            StartCoroutine(Reload());
        }
    }

    private void Fire()
    {
        bulletsLeft--;

        Vector3 shootDir = muzzlePoint.forward;
        shootDir = Quaternion.Euler(Random.Range(-currentSpread, currentSpread), Random.Range(-currentSpread, currentSpread), 0) * shootDir;

        Faction shootingTeam = Faction.Blue;
        currentSpread = Mathf.Max(currentSpread - spreadRecoveryRate * Time.deltaTime, 0f);

        if (owner != null)
        {
            Enemy enemyOwner = owner.GetComponent<Enemy>();
            if (enemyOwner != null)
            {
                shootingTeam = enemyOwner.myTeam;
            }
        }

        if (BulletPooler.Instance != null)
        {
            // 1. Obtener la bala del Pooler en lugar de instanciarla
            GameObject bulletGO = BulletPooler.Instance.GetBullet();

            // 2. Posicionar y orientar la bala
            bulletGO.transform.position = muzzlePoint.position;
            bulletGO.transform.rotation = muzzlePoint.rotation;

            // 3. Asignar propiedades y disparar
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;
                bulletScript.killerTeam = shootingTeam;
            }

            Rigidbody rb = bulletGO.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = shootDir * bulletSpeed;

            // NOTA: ELIMINAMOS la línea 'Destroy(bulletGO, 5f);' de aquí.
            // La bala se destruirá por colisión o por su propio script (ver paso 3).
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

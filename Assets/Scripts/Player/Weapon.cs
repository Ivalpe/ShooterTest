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

        if (useProjectile)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

            // --- PASAR LA INFORMACIÓN CLAVE A LA BALA ---
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = damage;         // Asegurarse de que la bala tenga el daño correcto
                bulletScript.killerTeam = shootingTeam; // Pasar la facción del que disparó
            }
            // ---------------------------------------------

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = shootDir * bulletSpeed;

            Destroy(bullet, 5f);
        }
        else
        {
            if (Physics.Raycast(muzzlePoint.transform.position, shootDir, out RaycastHit hit, range))
            {
                if (hitEffectPrefab != null)
                    Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

                // --- 1. Lógica para dañar al ENEMIGO ---
                Enemy enemy = hit.collider.GetComponent<Enemy>();

                if (enemy != null)
                {
                    Enemy shooterEnemy = owner.GetComponent<Enemy>();
                    Faction shooterTeam = (shooterEnemy != null) ? shooterEnemy.myTeam : Faction.Blue; // Asumo que el jugador es Azul si el dueño no es un enemigo

                    if (shooterEnemy != null && shooterEnemy.myTeam != enemy.myTeam)
                    {
                        // El enemigo dispara al enemigo
                        enemy.TakeDamage(damage, shooterTeam);
                    }
                    else if (shooterEnemy == null)
                    {
                        // El jugador dispara al enemigo
                        enemy.TakeDamage(damage, Faction.Blue); //Player Shoots Enemy
                    }
                }

                // --- 2. Lógica para dañar al JUGADOR ---
                // Buscamos el componente PlayerController en el objeto golpeado
                PlayerController player = hit.collider.GetComponent<PlayerController>();

                if (player != null)
                {
                    // Asumimos que el jugador no puede dañar al jugador, 
                    // solo un enemigo puede dañar al jugador.
                    Enemy shooterEnemy = owner.GetComponent<Enemy>();

                    if (shooterEnemy != null)
                    {
                        // Nota: Debes tener un método TakeDamage en PlayerController
                        // Si playerController tiene un TakeDamage, se llamaría así:
                        // player.TakeDamage(damage); 
                        Debug.Log("Jugador golpeado. ¡Implementar TakeDamage en PlayerController!");
                        // Alternativamente, si playerController es el script principal
                        // y tiene la vida, asegúrate de que tiene un método de daño.
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

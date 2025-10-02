using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 40f;
    public float fireRate = 0.5f; // segundos entre disparos
    public int magazineSize = 10;
    public float reloadTime = 2f;

    private int bulletsLeft;
    private bool isReloading = false;
    private float lastFireTime;

    void Start()
    {
        bulletsLeft = magazineSize;
    }

    public void TryShootAt(Transform target)
    {
        if (isReloading) return;

        if (bulletsLeft <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Time.time - lastFireTime >= fireRate)
        {
            Shoot(target);
            lastFireTime = Time.time;
        }
    }

    void Shoot(Transform target)
    {
        bulletsLeft--;

        Vector3 dir = (target.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * bulletSpeed;

        Destroy(bullet, 5f);
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Enemy reloading...");
        yield return new WaitForSeconds(reloadTime);
        bulletsLeft = magazineSize;
        isReloading = false;
    }
}

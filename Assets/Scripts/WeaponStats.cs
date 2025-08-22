using UnityEngine;

[System.Serializable]
public class WeaponStats
{
    public float fireRate = 0.2f;        // tiempo entre disparos
    public float bulletSpeed = 50f;      // velocidad de la bala
    public float spreadAngle = 2f;       // dispersión
    public GameObject bulletPrefab;      // prefab de la bala
    public ParticleSystem muzzleFlash;   // prefab del fuego del cañón
    public AudioClip fireSFX;            // sonido de disparo
}

using UnityEngine;
using System.Collections.Generic;

public class BulletPooler : MonoBehaviour
{
    // Singleton Pattern: Acceso f�cil desde cualquier script
    public static BulletPooler Instance;

    [Header("Configuraci�n del Pool")]
    public GameObject bulletPrefab; // El prefab de tu bala (Assets/Prefabs/Bullet.prefab)
    public int poolSize = 100;     // Cu�ntas balas precargar al inicio

    private Queue<GameObject> bulletPool;
    private Transform poolContainer; // Para mantener la jerarqu�a limpia

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PreloadBullets();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void PreloadBullets()
    {
        bulletPool = new Queue<GameObject>();

        // Creamos un GameObject padre para almacenar las balas desactivadas
        GameObject container = new GameObject("Bullet Pool Container");
        poolContainer = container.transform;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, poolContainer);

            // Lo hacemos cinem�tico para que no vuele mientras est� en el pool
            bullet.GetComponent<Rigidbody>().isKinematic = true;

            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count == 0)
        {
            // Opcional: Si el pool se agota, puedes crear m�s (NO recomendado para optimizaci�n extrema)
            Debug.LogWarning("Pool vac�o. Creando m�s balas. Considera aumentar el 'poolSize' inicial.");
            GameObject newBullet = Instantiate(bulletPrefab, poolContainer);
            return newBullet;
        }

        GameObject bullet = bulletPool.Dequeue();
        bullet.SetActive(true);

        // Lo hacemos NO cinem�tico para que la f�sica funcione y pueda ser disparada
        bullet.GetComponent<Rigidbody>().isKinematic = false;

        return bullet;
    }

    // --- FUNCI�N PARA DEVOLVER UNA BALA ---
    public void ReturnBullet(GameObject bullet)
    {
        // 1. Limpiar el estado de la bala (posici�n, velocidad, etc.)
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // Lo hacemos cinem�tico

        // 2. Desactivar y devolver
        bullet.SetActive(false);
        bullet.transform.SetParent(poolContainer);
        bulletPool.Enqueue(bullet);
    }
}
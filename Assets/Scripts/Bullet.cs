using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage;
    public GameObject bulletHolePrefab;
    public Faction killerTeam;
    public float maxLifeTime = 3f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable() // Se llama cuando la bala es tomada del pool y activada
    {
        // Comenzar el temporizador de seguridad para las balas perdidas
        StartCoroutine(AutoReturnToPool());
    }

    void OnDisable() // Se llama cuando la bala es devuelta y desactivada
    {
        // Detener la corrutina para que no intente devolver una bala desactivada
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Solo daña si la facción de la bala es diferente a la facción del enemigo
            if (killerTeam != enemy.myTeam)
            {
                // NOTA: La función TakeDamage en Enemy.cs DEBE aceptar el killerTeam
                enemy.TakeDamage(damage, killerTeam);
            }
        }

        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            // Asumimos que el jugador es del equipo Blue. Si la bala viene de Red, hay daño.
            if (killerTeam == Faction.Red)
            {
                // NOTA: La función TakeDamage en PlayerController DEBE aceptar el killerTeam
                player.TakeDamage(damage, killerTeam);
            }
        }

        // 3. Crear el agujero de bala si no es una unidad
        if (enemy == null && player == null)
        {
            if (bulletHolePrefab != null && other.contacts.Length > 0)
            {
                ContactPoint contact = other.contacts[0];
                Quaternion rot = Quaternion.LookRotation(contact.normal) * Quaternion.Euler(0, 180, 0);

                GameObject hole = Instantiate(
                    bulletHolePrefab,
                    contact.point + contact.normal * 0.01f,
                    rot
                );

                Destroy(hole, 1f);
            }
        }

        StopAllCoroutines();

        // 2. DEVOLVER AL POOL
        if (BulletPooler.Instance != null)
            BulletPooler.Instance.ReturnBullet(gameObject); // Devolver al pool


    }

    IEnumerator AutoReturnToPool()
    {
        yield return new WaitForSeconds(maxLifeTime);

        // Solo devolver al pool si la bala no ha colisionado
        if (gameObject.activeInHierarchy && BulletPooler.Instance != null)
        {
            BulletPooler.Instance.ReturnBullet(gameObject);
        }
    }


}

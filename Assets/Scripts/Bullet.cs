using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage;
    public GameObject bulletHolePrefab;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Primero comprobamos si es un enemigo
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage); // restamos vida
        }
        else
        {
            // Si no es enemigo, hacemos el bullet hole
            if (bulletHolePrefab != null && other.contacts.Length > 0)
            {
                ContactPoint contact = other.contacts[0];
                Quaternion rot = Quaternion.LookRotation(contact.normal) * Quaternion.Euler(0, 180, 0);

                GameObject hole = Instantiate(
                    bulletHolePrefab,
                    contact.point + contact.normal * 0.01f, // pequeño offset para que no se meta en la pared
                    rot
                );

                Destroy(hole, 5f);
            }
        }

        Destroy(gameObject); // destruir bala al impactar
    }


}

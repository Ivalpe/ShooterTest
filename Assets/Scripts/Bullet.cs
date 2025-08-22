using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletHolePrefab;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Instanciar bullet hole si hay prefab
        if(bulletHolePrefab != null)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.LookRotation(contact.normal) * Quaternion.Euler(0, 180, 0);
            GameObject hole = Instantiate(bulletHolePrefab, contact.point + contact.normal * 0.01f, rot);
            Destroy(hole, 60f);
        }

        // Destruir la bala
        Destroy(gameObject);
    }
}

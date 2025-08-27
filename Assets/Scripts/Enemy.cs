using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator anim;
    public int health = 100;
    public HitMarker hitMarker;

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (hitMarker != null)
            hitMarker.Show();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(RandomDance());
    }

    private IEnumerator RandomDance()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f)); // espera 2-5 segundos

            anim.SetTrigger("Dance");
        }
    }

    void Die()
    {
        // Aquí puedes poner animación o efectos
        Debug.Log(name + " murió!");
        Destroy(gameObject);
    }
}

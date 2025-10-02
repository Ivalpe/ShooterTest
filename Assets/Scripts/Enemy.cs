using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Animator anim;
    public int health = 100;
    public HitMarker hitMarker;
    public EnemyController gun;
    public List<Transform> wayPoints;
    NavMeshAgent agent;

    public int currentWayPointIndex = 0;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if (wayPoints.Count == 0) return;

        float distanceToWayPoint = Vector3.Distance(wayPoints[currentWayPointIndex].position, transform.position);

        if (distanceToWayPoint < 3f || wayPoints[currentWayPointIndex].GetComponentInChildren<ZonesCapture>().currentTeam == ZonesCapture.ZoneTeam.Red)
        {
            wayPoints[currentWayPointIndex].GetComponentInChildren<ZonesCapture>().SetTeam(ZonesCapture.ZoneTeam.Red);
            currentWayPointIndex = (currentWayPointIndex + 1) % wayPoints.Count;
        }

        agent.SetDestination(wayPoints[currentWayPointIndex].position);
    }

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

            anim.SetTrigger("RifleJump");
        }
    }

    void Die()
    {
        // Aquí puedes poner animación o efectos
        Debug.Log(name + " murió!");
        Destroy(gameObject);
    }
}

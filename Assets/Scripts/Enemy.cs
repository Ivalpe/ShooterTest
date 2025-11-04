using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Puedes usar el mismo enum ZoneTeam del script ZonesCapture o definir uno simple aquí
public enum Faction
{
    Blue,
    Red
}

public class Enemy : MonoBehaviour
{
    int health = 100;
    public HitMarker hitMarker;
    public NavMeshAgent agent;

    public ZonesManager zonesManager;

    public Faction myTeam;
    private ZonesCapture currentTargetZone = null;
    public float decisionDelay = 2f;
    private float nextDecisionTime;


    void Start()
    {
        nextDecisionTime = Time.time;
    }

    void Update()
    {
        if (Time.time >= nextDecisionTime)
        {
            DecideTargetAndMove();
            nextDecisionTime = Time.time + decisionDelay;
        }
    }

    private void DecideTargetAndMove()
    {
        // 1. Llama al método de búsqueda del ZonesManager
        // Nota: Convertimos nuestro 'Faction' a 'ZonesCapture.ZoneTeam' para que sea compatible.
        ZonesCapture.ZoneTeam zoneTeam = (ZonesCapture.ZoneTeam)myTeam;

        currentTargetZone = zonesManager.FindBestTargetZone(zoneTeam, transform.position);

        // 2. Si hay un objetivo válido, muévete hacia él
        if (currentTargetZone != null)
        {
            agent.SetDestination(currentTargetZone.transform.position);
            // Debug.Log($"Enemigo {myTeam} dirigiéndose a la zona: {currentTargetZone.gameObject.name}");
        }
        else
        {
            // Opcional: Si no hay objetivos, haz que el agente se detenga o defienda su posición actual
            agent.isStopped = true;
        }
    }

    public void TakeDamage(int amount)
    {
        // ... (Tu código de daño permanece igual) ...
        health -= amount;

        if (!hitMarker.isActiveAndEnabled)
            hitMarker.Show();

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
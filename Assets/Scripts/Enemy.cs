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
    //public HitMarker hitMarker;
    public NavMeshAgent agent;

    public ZonesManager zonesManager;

    public Faction myTeam;
    private ZonesCapture currentTargetZone = null;
    public float decisionDelay = 2f;
    private float nextDecisionTime;

    public Weapon currentWeapon;
    public float attackRange = 25f;
    public LayerMask targetMask;
    private Transform targetPlayer = null;
    public GameManager gameManager;


    void Start()
    {
        nextDecisionTime = Time.time;
        currentWeapon.SetOwner(gameObject);
    }

    void Update()
    {
        if (Time.time >= nextDecisionTime)
        {
            DecideTargetAndMove();
            nextDecisionTime = Time.time + decisionDelay;
        }

        CheckForTargetsAndShoot();
    }

    public void Initialize(Faction team, ZonesManager zManager, GameManager tManager, LayerMask tMask, Weapon weaponPrefab)
    {
        myTeam = team;
        zonesManager = zManager;
        gameManager = tManager;
        targetMask = tMask;

        // 1. Instanciar el arma (Si no viene ya como parte del prefab)
        // Si el arma es parte del prefab base, solo debes obtenerla:
        currentWeapon = GetComponentInChildren<Weapon>();

        if (currentWeapon != null)
        {
            currentWeapon.SetOwner(gameObject);
        }
        else if (weaponPrefab != null)
        {
            // Lógica si necesitas instanciar el arma por separado
            Weapon newWeapon = Instantiate(weaponPrefab, transform);
            currentWeapon = newWeapon;
            currentWeapon.SetOwner(gameObject);
            // Configurar punto de muzzle, etc.
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

    private void CheckForTargetsAndShoot()
    {
        if (currentWeapon == null) return;

        // 1. Buscar objetivos en el rango de ataque
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, targetMask);

        // Por simplicidad, tomaremos el primer objetivo válido (no es la mejor IA, pero funciona)
        if (targets.Length > 0)
        {
            targetPlayer = targets[0].transform;

            // 2. Orientar al objetivo
            Vector3 directionToTarget = (targetPlayer.position - transform.position).normalized;
            // Solo rotamos en el eje Y para no inclinar al enemigo
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // 3. Detener movimiento y disparar
            agent.isStopped = true;
            currentWeapon.TryShoot(); // El arma ya maneja la cadencia de fuego

            if (currentWeapon.bulletsLeft <= 0)
            {
                currentWeapon.TryReload();
            }
        }
        else
        {
            // Si no hay objetivos, reanudar la marcha hacia la zona
            agent.isStopped = false;
        }
    }

    public void TakeDamage(int amount, Faction killerTeam)
    {
        // ... (Tu código de daño permanece igual) ...
        health -= amount;

        //if (!hitMarker.isActiveAndEnabled)
        //    hitMarker.Show();

        if (health <= 0)
        {
            gameManager.HandleUnitDeath(gameObject, myTeam, killerTeam);
            Destroy(gameObject); // Destrucción de fallback
        }
    }
}
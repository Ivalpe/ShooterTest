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

        if (currentWeapon.bulletsLeft <= 0)
            currentWeapon.TryReload();


        // 1. Buscar objetivos en el rango de ataque
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, targetMask);

        Transform bestTarget = null;
        float closestDistance = float.MaxValue;

        // 2. Iterar para encontrar el objetivo más cercano Y VIVO
        foreach (Collider targetCollider in targets)
        {
            // Intentar obtener el script del JUGADOR
            PlayerController player = targetCollider.GetComponent<PlayerController>();

            // Intentar obtener el script de otro ENEMIGO
            Enemy otherEnemy = targetCollider.GetComponent<Enemy>();

            bool isTargetAlive = false;

            // Comprobación de vida
            if (player != null)
            {
                // 
                // Si el PlayerController tiene una variable pública de salud, léela
                // Asumimos que la salud se gestiona internamente en PlayerController
                isTargetAlive = player.currentHealth > 0;
            }
            else if (otherEnemy != null)
            {
                // El fuego amigo ya está controlado en el arma, pero confirmamos que esté vivo.
                isTargetAlive = otherEnemy.health > 0;
            }

            // Si el objetivo está vivo y es el más cercano hasta ahora
            if (isTargetAlive)
            {
                float distance = (targetCollider.transform.position - transform.position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = targetCollider.transform;
                }
            }
        }

        // 3. Si se encontró un objetivo VIVO, atacar
        if (bestTarget != null)
        {
            // Orientar al objetivo
            Vector3 directionToTarget = (bestTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Detener movimiento y disparar
            agent.isStopped = true;
            currentWeapon.TryShoot();
        }
        else
        {
            // Si no hay objetivos vivos cerca, reanudar la marcha hacia la zona
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
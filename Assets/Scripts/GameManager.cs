using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Necesario para las Corrutinas

public class GameManager : MonoBehaviour
{
    public ZonesManager zonesManagerInstance;
    public GameManager teamManagerInstance;

    public int blueTeamKills = 0;
    public int redTeamKills = 0;

    public float respawnDelay = 5.0f;

    [Header("Configuración de la Partida")]
    public int initialBlueUnits = 5;
    public int initialRedUnits = 5;

    public Transform[] blueSpawnPoints;
    public Transform[] redSpawnPoints;

    [Header("Prefabs y Configuración")]
    public GameObject blueUnitPrefab;
    public GameObject redUnitPrefab;
    public Weapon defaultWeaponPrefab;
    public LayerMask blueTargetMask;
    public LayerMask redTargetMask;

    private List<GameObject> aliveBlueUnits = new List<GameObject>();
    private List<GameObject> aliveRedUnits = new List<GameObject>();

    void Start()
    {
        // 1. Inicializa los managers si usas el patrón Singleton (opcional, pero recomendado)
        if (zonesManagerInstance == null)
            zonesManagerInstance = FindObjectsByType<ZonesManager>(FindObjectsSortMode.None)[0];

        // 2. ¡Llamada para crear el equipo inicial solo una vez!
        SpawnInitialUnits();
    }

    private void SpawnInitialUnits()
    {

        aliveBlueUnits.Clear();
        aliveRedUnits.Clear();

        // Spawn del Equipo Azul
        for (int i = 0; i < initialBlueUnits; i++)
        {
            SpawnNewUnit(Faction.Blue);
        }

        // Spawn del Equipo Rojo
        for (int i = 0; i < initialRedUnits; i++)
        {
            SpawnNewUnit(Faction.Red);
        }
    }

    private void SpawnNewUnit(Faction team)
    {
        GameObject prefabToSpawn;
        Transform[] spawnPoints;
        LayerMask targetMaskToUse;

        if (team == Faction.Blue)
        {
            prefabToSpawn = blueUnitPrefab;
            spawnPoints = blueSpawnPoints;
            targetMaskToUse = blueTargetMask;
        }
        else // Faction.Red
        {
            prefabToSpawn = redUnitPrefab;
            spawnPoints = redSpawnPoints;
            targetMaskToUse = redTargetMask;
        }

        if (prefabToSpawn != null && spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instanciar el nuevo GameObject
            GameObject newUnitGO = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

            if (team == Faction.Blue)
            {
                aliveBlueUnits.Add(newUnitGO);
            }
            else
            {
                aliveRedUnits.Add(newUnitGO);
            }

            // 1. OBTENER EL SCRIPT DE IA
            Enemy newUnit = newUnitGO.GetComponent<Enemy>();

            if (newUnit != null)
            {
                // 2. INYECTAR LAS REFERENCIAS Y VALORES INICIALES
                newUnit.Initialize(
                    team,
                    zonesManagerInstance,
                    this,                 // Referencia al propio TeamManager
                    targetMaskToUse,
                    null                  // Si el arma está en el prefab del enemigo, pasa null aquí.
                );
            }
        }
    }

    public void HandleUnitDeath(GameObject deadUnit, Faction team, Faction killerTeam)
    {
        // 1. Contabilizar la muerte (el equipo contrario obtiene el punto)
        if (killerTeam == Faction.Blue)
        {
            blueTeamKills++;
            Debug.Log($"Punto para el equipo Azul. Azules: {blueTeamKills} - Rojos: {redTeamKills}");
        }
        else if (killerTeam == Faction.Red)
        {
            redTeamKills++;
            Debug.Log($"Punto para el equipo Rojo. Azules: {blueTeamKills} - Rojos: {redTeamKills}");
        }

        if (team == Faction.Blue)
        {
            aliveBlueUnits.Remove(deadUnit);
        }
        else
        {
            aliveRedUnits.Remove(deadUnit);
        }

        // 2. Destruir la unidad inmediatamente
        Destroy(deadUnit);

        // 3. Iniciar la Corrutina de Reaparición
        StartCoroutine(RespawnCoroutine(team));
    }

    private IEnumerator RespawnCoroutine(Faction team)
    {
        yield return new WaitForSeconds(respawnDelay);

        // 5. ¡COMPROBACIÓN DE LÍMITE AL FINAL DE LA ESPERA!
        // Solo hacemos el spawn si el equipo aún no ha alcanzado su límite máximo.
        if (team == Faction.Blue && aliveBlueUnits.Count < initialBlueUnits)
        {
            SpawnNewUnit(team);
        }
        else if (team == Faction.Red && aliveRedUnits.Count < initialRedUnits)
        {
            SpawnNewUnit(team);
        }
        else
        {
            Debug.Log($"Equipo {team} ya tiene su número máximo ({initialBlueUnits} unidades) antes del respawn.");
        }
    }
}


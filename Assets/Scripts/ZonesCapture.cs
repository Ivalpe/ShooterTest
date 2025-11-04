using NUnit.Framework;
using UnityEngine;

public class ZonesCapture : MonoBehaviour
{
    public enum ZoneTeam
    {
        Blue,
        Red,
        Neutral
    }

    public LayerMask blueTeamLayer;
    public LayerMask redTeamLayer;
    public float detectionRadius = 10f;

    int blueCount = 0;
    int redCount = 0;
    public float updateInterval = 1.0f;
    private float nextUpdateTime;
    private int percetageToCapture = 10;

    int porcentageBlue = 0;
    int porcentageRed = 0;
    bool captured = false;

    public Material material;
    public ZoneTeam currentTeam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTeam = ZoneTeam.Neutral;
        GetComponent<Renderer>().material = material;
        SetTeam(ZoneTeam.Neutral);
        nextUpdateTime = Time.time + updateInterval;
    }

    // Update is called once per frame
    void Update()
    {
        CountPlayersInRange();
    }

    // Function to count players in range
    public void CountPlayersInRange()
    {
        blueCount = 0;
        redCount = 0;
        // Combina las LayerMasks para buscar ambos equipos en una sola pasada
        LayerMask combinedMask = blueTeamLayer | redTeamLayer;

        // Usa OverlapSphere para obtener todos los colliders dentro del radio.
        // `transform.position` es el centro de la zona.
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, combinedMask);

        // Itera sobre los colliders encontrados para contarlos
        foreach (var hitCollider in hitColliders)
        {
            // Obtiene la capa del objeto que colisionó
            int objectLayer = hitCollider.gameObject.layer;

            // Comprueba si la capa del objeto está en la máscara del Equipo Azul
            if (((1 << objectLayer) & blueTeamLayer) != 0)
            {
                blueCount++;
            }
            // Comprueba si la capa del objeto está en la máscara del Equipo Rojo
            else if (((1 << objectLayer) & redTeamLayer) != 0)
            {
                redCount++;
            }
        }

        if (Time.time >= nextUpdateTime)
        {
            if (blueCount > redCount && currentTeam != ZoneTeam.Blue)
            {
                if (porcentageRed > 0)
                    porcentageRed -= percetageToCapture;
                else if (porcentageBlue < 100)
                    porcentageBlue += percetageToCapture;
            }
            else if (redCount > blueCount && currentTeam != ZoneTeam.Red)
            {
                if (porcentageBlue > 0)
                    porcentageBlue -= percetageToCapture;
                else if (porcentageRed < 100)
                    porcentageRed += percetageToCapture;
            }

            nextUpdateTime += updateInterval;
        }

        if (porcentageBlue >= 100 || (porcentageBlue > 0 && captured))
        {
            SetTeam(ZoneTeam.Blue);
            captured = true;
        }
        else if (porcentageRed >= 100 || (porcentageRed > 0 && captured))
        {
            SetTeam(ZoneTeam.Red);
            captured = true;
        }
        else
        {
            SetTeam(ZoneTeam.Neutral);
            captured = false;
        }

    }

    public void CalculeTeam()
    {

    }

    public void SetTeam(ZoneTeam team)
    {
        currentTeam = team;
        switch (team)
        {
            case ZoneTeam.Blue:
                GetComponent<Renderer>().material.color = Color.blue;
                break;
            case ZoneTeam.Red:
                GetComponent<Renderer>().material.color = Color.red;
                break;
            case ZoneTeam.Neutral:
                GetComponent<Renderer>().material.color = Color.grey;
                break;
        }
    }
}

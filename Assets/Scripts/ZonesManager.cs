using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ZonesManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<ZonesCapture> zonesList;
    void Start()
    {
        // Find all ZonesCapture components in the scene and add them to the list
        zonesList = new List<ZonesCapture>(FindObjectsByType<ZonesCapture>(FindObjectsSortMode.None));
    }

    // Update is called once per frame
    void Update()
    {


    }

    public ZonesCapture FindBestTargetZone(ZonesCapture.ZoneTeam myTeam, Vector3 currentPosition)
    {
        ZonesCapture bestTarget = null;
        float closestDistanceSqr = float.MaxValue; // Usamos la distancia al cuadrado (sqrMagnitude) para optimizar

        // 1. Filtrar y Buscar
        foreach (ZonesCapture currentZone in zonesList)
        {
            // Condición de Filtrado: Excluir zonas que son de mi equipo
            if (currentZone.currentTeam != myTeam)
            {
                // Distancia vectorial entre la posición actual y la zona
                Vector3 differenceVector = currentZone.transform.position - currentPosition;

                // Distancia al cuadrado (más rápido que usar Vector3.Distance o magnitude)
                float distanceSqr = differenceVector.sqrMagnitude;

                // 3. Selección del Mejor Objetivo (más cercano)
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    bestTarget = currentZone;
                }
            }
        }

        return bestTarget;
    }
}

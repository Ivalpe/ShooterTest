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

    public Material neutralTexture, blueTexture, redTexture;
    public ZoneTeam currentTeam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTeam = ZoneTeam.Neutral;
        GetComponent<Renderer>().material = neutralTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTeam(ZoneTeam team)
    {
        currentTeam = team;
        switch (team)
        {
            case ZoneTeam.Blue:
                GetComponent<Renderer>().material = blueTexture;
                break;
            case ZoneTeam.Red:
                GetComponent<Renderer>().material = redTexture;
                break;
            case ZoneTeam.Neutral:
                GetComponent<Renderer>().material = neutralTexture;
                break;
        }
    }
}

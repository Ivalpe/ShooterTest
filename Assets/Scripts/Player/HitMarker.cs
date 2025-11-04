using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    public Image hitMarkerImage;
    public float displayTime;

    private void Awake()
    {
        hitMarkerImage.gameObject.SetActive(false);
    }

    public void Show()
    {
        hitMarkerImage.gameObject.SetActive(true); 
        Invoke("Hide", displayTime);               
    }

    private void Hide()
    {
        hitMarkerImage.gameObject.SetActive(false);
    }
}

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
        hitMarkerImage.gameObject.SetActive(true); // se activa inmediatamente
        Invoke("Hide", displayTime);               // se oculta después del tiempo
    }

    private void Hide()
    {
        hitMarkerImage.gameObject.SetActive(false);
    }
}

using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float verticalRecoilMin = 0.3f;
    public float verticalRecoilMax = 0.6f;
    public float horizontalRecoilMin = -0.5f;
    public float horizontalRecoilMax = 0.5f;
    public float recoilSpeed = 10f;
    public float maxUpAngle = 10f;

    private Quaternion originalRotation;
    private Vector2 currentRecoil = Vector2.zero; // x = horizontal, y = vertical

    void Awake()
    {
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        // Suaviza la recuperación
        currentRecoil = Vector2.Lerp(currentRecoil, Vector2.zero, Time.deltaTime * recoilSpeed);

        // Aplica la rotación combinando vertical y horizontal
        transform.localRotation = originalRotation * Quaternion.Euler(-currentRecoil.y, currentRecoil.x, 0);
    }

    public void ApplyRecoil()
    {
        // Recoil vertical aleatorio
        float vertical = Random.Range(verticalRecoilMin, verticalRecoilMax);
        currentRecoil.y += vertical;
        if (currentRecoil.y > maxUpAngle)
            currentRecoil.y = maxUpAngle;

        // Recoil horizontal aleatorio
        float horizontal = Random.Range(horizontalRecoilMin, horizontalRecoilMax);
        currentRecoil.x += horizontal;
    }
}

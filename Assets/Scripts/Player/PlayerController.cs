using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int maxHealth = 100;
    public int currentHealth;
    private Faction playerTeam = Faction.Blue;
    public GameManager gameManagerInstance;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    public float minPitch = -89f;
    public float maxPitch = 89f;

    private CharacterController controller;
    private float yVelocity;
    private float pitch; // vertical rotation (camera only)
    private Weapon weapon;
    public TextMeshProUGUI ammoText;

    void Awake()
    {
        currentHealth = maxHealth;

        weapon = GetComponentInChildren<Weapon>();
        weapon.spreadAngle = 100;
        controller = GetComponent<CharacterController>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (currentHealth <= 0)
            return;

        HandleLook();
        HandleMovement();

        //Fire
        if (Input.GetButton("Fire1"))
            weapon.TryShoot();

        //Reload
        if (Input.GetKeyDown(KeyCode.R))
            weapon.TryReload();

        if (ammoText != null)
            ammoText.SetText(weapon.bulletsLeft + " / " + weapon.magazineSize);
    }
    public void TakeDamage(int amount, Faction killerTeam)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"Jugador golpeado por Equipo {killerTeam}. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            this.enabled = false;

            gameManagerInstance.HandlePlayerDeath(gameObject, playerTeam, killerTeam);
        }
    }
    public void Respawn(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;

        currentHealth = maxHealth;
        this.enabled = true; // Habilitar el script
        controller.enabled = true; // Habilitar movimiento
        yVelocity = 0f;
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Horizontal yaw rotates the player (body)
        transform.Rotate(Vector3.up * mouseX);

        // Vertical pitch rotates only the camera
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        // Grounded vertical velocity reset
        if (isGrounded && yVelocity < 0f)
            yVelocity = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Move relative to player forward/right
        Vector3 move = (transform.right * x + transform.forward * z);
        float currentSpeed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        // Jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // v = sqrt(2 * h * -g)
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        yVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * yVelocity * Time.deltaTime);
    }
}

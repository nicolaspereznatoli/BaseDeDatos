using UnityEngine;
using TMPro;
using DevionGames.InventorySystem;
using UnityEngine.UI;
public class SeedDispenser : MonoBehaviour
{

    // --- Lógica de recolección ---
    public int maxSeeds = 1;                // Máximo de semillas disponibles (puede ser >1 si quieres)
    public float cooldownTime = 10f;        // Tiempo de espera entre recogidas
    private int currentSeeds = 1;           // Cuántas semillas quedan disponibles
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;


    // --- Variables de Interacción y UI ---
    public TextMeshProUGUI interactionText;     // Texto "Presiona R para recoger semilla"
    public TextMeshProUGUI feedbackMessageText; // Mensaje temporal (ej. "Semilla recogida", "Inventario lleno")

    // --- Configuración del Ítem ---
    public string seedItemName = "Semilla";     // Nombre del ítem a dar
    public Sprite seedIcon;                     // Icono del ítem (asignar en Inspector)

    // --- Referencias del Jugador ---
    public GameObject player;                   // Referencia al GameObject del jugador
    public ItemContainer playerInventory;       // Inventario del jugador
    public float interactionDistance = 3f;      // Distancia para interactuar (ajusta si es necesario)

    // --- Estado Interno ---
    private bool playerInRange = false;

    void Start()
    {
        // Inicializar y comprobar textos de UI
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("SeedDispenser: 'interactionText' NO está asignado en el Inspector.");
        }

        if (feedbackMessageText != null)
        {
            feedbackMessageText.text = ""; // Limpiar mensaje de feedback inicial
            feedbackMessageText.gameObject.SetActive(false); // Ocultar al inicio
        }
        else
        {
            // Este es opcional, pero bueno tenerlo para feedback
            Debug.LogWarning("SeedDispenser: 'feedbackMessageText' NO está asignado en el Inspector. No se mostrarán mensajes de feedback.");
        }

        // Comprobar referencias críticas
        if (player == null)
        {
            Debug.LogError("SeedDispenser: 'player' NO está asignado en el Inspector.");
        }

        if (playerInventory == null)
        {
            Debug.LogError("SeedDispenser: 'playerInventory' NO está asignado en el Inspector.");
        }

        if (seedIcon == null)
        {
            Debug.LogWarning("SeedDispenser: 'seedIcon' NO está asignado en el Inspector. La semilla se añadirá sin icono.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= interactionDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (interactionText != null)
                {
                    interactionText.text = "Presiona [R] para recoger " + seedItemName;
                    interactionText.gameObject.SetActive(true);
                }

                if (feedbackMessageText != null)
                {
                    feedbackMessageText.gameObject.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!isOnCooldown && currentSeeds > 0)
                {
                    GiveSeed();
                }
                else if (isOnCooldown)
                {
                    if (feedbackMessageText != null)
                        feedbackMessageText.text = "Semillas agotadas. Espera...";
                }
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (interactionText != null)
                {
                    interactionText.gameObject.SetActive(false);
                }
                if (feedbackMessageText != null)
                {
                    feedbackMessageText.text = "";
                    feedbackMessageText.gameObject.SetActive(false);
                }
            }
        }

        // Control del cooldown
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                currentSeeds = maxSeeds;

                if (feedbackMessageText != null && playerInRange)
                {
                    feedbackMessageText.text = "¡Semillas disponibles otra vez!";
                }
            }
        }
    }


    void GiveSeed()
    {
        if (playerInventory == null)
        {
            Debug.LogError("SeedDispenser: No se puede dar semilla, 'playerInventory' no está asignado.");
            if (feedbackMessageText != null) feedbackMessageText.text = "Error: Inventario no configurado.";
            return;
        }

        // Crear la instancia del ítem semilla
        Item seedInstance = ScriptableObject.CreateInstance<Item>();
        seedInstance.DisplayName = seedItemName;
        seedInstance.Icon = seedIcon;
        seedInstance.Stack = 1;

        bool addedSuccessfully = playerInventory.AddItem(seedInstance);

        if (addedSuccessfully)
        {
            Debug.Log(seedItemName + " recogida y añadida al inventario.");
            if (feedbackMessageText != null)
            {
                feedbackMessageText.text = "¡" + seedItemName + " recogida!";
            }

            currentSeeds--;

            if (currentSeeds <= 0)
            {
                StartCooldown();
            }
        }
        else
        {
            Debug.LogWarning("No se pudo añadir " + seedItemName + " al inventario.");
            if (feedbackMessageText != null)
            {
                feedbackMessageText.text = "Inventario lleno o no se puede añadir " + seedItemName + ".";
            }
        }
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        if (feedbackMessageText != null)
        {
            feedbackMessageText.text = "Semillas agotadas. Espera " + cooldownTime + " segundos.";
        }
    }


}
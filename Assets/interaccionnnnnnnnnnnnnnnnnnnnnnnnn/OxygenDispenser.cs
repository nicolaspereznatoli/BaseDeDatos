using UnityEngine;
using TMPro;
using DevionGames.InventorySystem;
using UnityEngine.UI;

public class OxygenDispenser : MonoBehaviour
{
    // --- UI ---
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI feedbackMessageText;

    // --- Configuración del Ítem ---
    public string oxygenItemName = "Oxigeno";
    public Sprite oxygenIcon;

    // --- Referencias del Jugador ---
    public GameObject player;
    public ItemContainer playerInventory;
    public float interactionDistance = 3f;

    // --- Estado Interno ---
    private bool playerInRange = false;

    // --- Lógica de recolección ---
    public int maxUnits = 1;
    public float cooldownTime = 10f;
    private int currentUnits = 1;
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
        else
            Debug.LogError("OxygenDispenser: 'interactionText' NO está asignado.");

        if (feedbackMessageText != null)
        {
            feedbackMessageText.text = "";
            feedbackMessageText.gameObject.SetActive(false);
        }

        if (player == null)
            Debug.LogError("OxygenDispenser: 'player' NO está asignado.");

        if (playerInventory == null)
            Debug.LogError("OxygenDispenser: 'playerInventory' NO está asignado.");

        if (oxygenIcon == null)
            Debug.LogWarning("OxygenDispenser: 'oxygenIcon' no está asignado.");
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
                    interactionText.text = "Presiona [R] para obtener " + oxygenItemName;
                    interactionText.gameObject.SetActive(true);
                }

                if (feedbackMessageText != null)
                    feedbackMessageText.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!isOnCooldown && currentUnits > 0)
                {
                    GiveOxygen();
                }
                else if (isOnCooldown && feedbackMessageText != null)
                {
                    feedbackMessageText.text = "Oxígeno agotado. Espera...";
                }
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (interactionText != null)
                    interactionText.gameObject.SetActive(false);
                if (feedbackMessageText != null)
                {
                    feedbackMessageText.text = "";
                    feedbackMessageText.gameObject.SetActive(false);
                }
            }
        }

        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                currentUnits = maxUnits;
                if (feedbackMessageText != null && playerInRange)
                    feedbackMessageText.text = "¡Oxígeno disponible otra vez!";
            }
        }
    }

    void GiveOxygen()
    {
        if (playerInventory == null)
        {
            Debug.LogError("OxygenDispenser: Inventario no asignado.");
            if (feedbackMessageText != null)
                feedbackMessageText.text = "Error: Inventario no disponible.";
            return;
        }

        Item oxygenItem = ScriptableObject.CreateInstance<Item>();
        oxygenItem.DisplayName = oxygenItemName;
        oxygenItem.Icon = oxygenIcon;
        oxygenItem.Stack = 1;

        bool added = playerInventory.AddItem(oxygenItem);

        if (added)
        {
            Debug.Log(oxygenItemName + " entregado.");
            if (feedbackMessageText != null)
                feedbackMessageText.text = "¡" + oxygenItemName + " recibido!";
            currentUnits--;
            if (currentUnits <= 0)
                StartCooldown();
        }
        else
        {
            Debug.LogWarning("Inventario lleno, no se pudo añadir " + oxygenItemName);
            if (feedbackMessageText != null)
                feedbackMessageText.text = "Inventario lleno. No se pudo obtener " + oxygenItemName + ".";
        }
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        if (feedbackMessageText != null)
            feedbackMessageText.text = "Oxígeno agotado. Espera " + cooldownTime + " segundos.";
    }
}

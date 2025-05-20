using UnityEngine;
using TMPro; // Si usas TextMeshPro
using UnityEngine.UI; // Si usas la UI de Unity
using DevionGames.InventorySystem; // Importa el sistema de inventario

public class WaterTank : MonoBehaviour
{
    public int waterLevel = 100; // Nivel de agua inicial
    public int waterToCollect = 5; // Cantidad de agua a recoger

    public TextMeshProUGUI waterLevelText; // Texto del nivel de agua del tanque
    public TextMeshProUGUI interactionText; // Texto "Presiona R para recoger agua"
    public TextMeshProUGUI mensajeAgua; // Mensaje temporal (ej. "Cubo de agua añadido")
    public TextMeshProUGUI playerWaterText; // NUEVO: Texto en pantalla con agua acumulada
    public Text playerstack; // NUEVO: Texto en pantalla con agua acumulada

    public GameObject player; // Referencia al jugador
    public float interactionDistance = 5f; // Distancia para interactuar

    public ItemContainer playerInventory; // Inventario del jugador
    public string itemName = "Cubo de Agua"; // Nombre del ítem
    public Sprite waterCuboIcon; // Icono del ítem

    private bool playerInRange = false;
    private int playerWater = 0; // Agua acumulada por el jugador
    private bool hasCollectedWater = false;

    void Start()
    {
        interactionText.gameObject.SetActive(false);
        mensajeAgua.text = "";
        UpdateWaterLevelText();
        UpdatePlayerWaterText(); // Inicializa el texto del agua acumulada

        if (player == null)
        {
            Debug.LogError("No se ha asignado el objeto del jugador en el Inspector.");
        }

        if (playerInventory == null)
        {
            Debug.LogError("No se ha asignado el ItemContainer del jugador en el Inspector.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= interactionDistance)
            {
                playerInRange = true;
                interactionText.gameObject.SetActive(true);
                UpdateWaterLevelText();

                if (Input.GetKeyDown(KeyCode.R))
                {
                    CollectWater();
                }
            }
            else
            {
                playerInRange = false;
                interactionText.gameObject.SetActive(false);
                mensajeAgua.text = "";
            }
        }
    }

    void CollectWater()
    {
        if (waterLevel >= waterToCollect)
        {
            waterLevel -= waterToCollect;
            playerWater += waterToCollect;
            UpdateWaterLevelText();
            UpdatePlayerWaterText(); // NUEVO: actualiza texto en pantalla
            mensajeAgua.text = "Tienes " + playerWater + " de agua";
            Debug.Log("Agua recogida. Nivel de agua restante: " + waterLevel);
            hasCollectedWater = true;
            AddWaterToInventory();
        }
        else
        {
            Debug.Log("No hay suficiente agua en el tanque.");
        }
    }

    void UpdateWaterLevelText()
    {
        if (waterLevelText != null)
        {
            waterLevelText.text = "Nivel de Agua: " + waterLevel + "/100";
        }
    }

    void UpdatePlayerWaterText() // NUEVO: muestra agua acumulada en la pantalla
    {
        if (playerWaterText != null)
        {
            playerWaterText.text = "Agua: " + playerWater;
        }
    }

    void stacck()
    {
        playerstack.text = "" + playerWater + "";
    }

    void AddWaterToInventory()
    {
        if (playerInventory != null && hasCollectedWater)
        {
            WaterItem waterItem = ScriptableObject.CreateInstance<WaterItem>();
            waterItem.DisplayName = itemName;
            waterItem.Icon = waterCuboIcon;
            waterItem.Stack = 1;

            bool success = playerInventory.AddItem(waterItem);

            if (success)
            {
                Debug.Log("Cubo de Agua añadido al inventario.");
                mensajeAgua.text = "Cubo de Agua añadido al inventario.";
            }
            else
            {
                Debug.LogWarning("No se pudo añadir Cubo de Agua al inventario.");
                mensajeAgua.text = "No se pudo añadir Cubo de Agua.";
            }
        }
        else if (playerInventory == null)
        {
            Debug.LogError("No se ha asignado el inventario del jugador en el Inspector.");
        }
    }

    void MostrarMensajeAgua()
    {
        mensajeAgua.text = "Tienes " + playerWater + " de agua";
    }

    

}

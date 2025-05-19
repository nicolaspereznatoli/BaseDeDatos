using UnityEngine;
using TMPro; // Si usas TextMeshPro
using UnityEngine.UI; // Si usas la UI de Unity
using DevionGames.InventorySystem; // Importa el sistema de inventario

public class WaterTank : MonoBehaviour
{
    public int waterLevel = 100; // Nivel de agua inicial
    public int waterToCollect = 5; // Cantidad de agua a recoger
    public TextMeshProUGUI waterLevelText; // Referencia al texto de la UI del nivel de agua
    public TextMeshProUGUI interactionText; // Texto para mostrar "Presiona R para recoger agua"
    public GameObject player; // Referencia al objeto del jugador ¡PÚBLICA!
    public float interactionDistance = 5f; // Distancia máxima para interactuar
    public TextMeshProUGUI mensajeAgua; // Referencia al texto para el mensaje de agua ¡PÚBLICA!
    public ItemContainer playerInventory; // Referencia al inventario del jugador.  ¡Arrastrar desde el Inspector!
    public string itemName = "Cubo de Agua"; // Nombre del item en el inventario
    public Sprite waterCuboIcon; // Icono del cubo de agua. ¡Arrastrar desde el Inspector!

    private bool playerInRange = false;
    private int playerWater = 0; // Cantidad de agua que tiene el jugador
    private bool hasCollectedWater = false; // Nuevo flag para verificar si se recogió agua

    void Start()
    {
        interactionText.gameObject.SetActive(false); // Oculta el texto al inicio
        UpdateWaterLevelText();
        if (player == null)
        {
            Debug.LogError("No se ha asignado el objeto del jugador en el Inspector. Asegúrate de arrastrar el objeto del jugador al campo 'Player' del script WaterTank.");
        }
        if (playerInventory == null)
        {
            Debug.LogError("No se ha asignado el ItemContainer del jugador en el Inspector. Asegúrate de arrastrar el objeto del inventario del jugador al campo 'Player Inventory' del script WaterTank.");
        }
        mensajeAgua.text = ""; // Inicializa el mensaje de agua
    }

    void Update()
    {
        if (player != null) // Comprobamos que el jugador ha sido asignado
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position); //calculo de distancia
            if (distanceToPlayer <= interactionDistance)
            {
                playerInRange = true;
                interactionText.gameObject.SetActive(true);
                UpdateWaterLevelText();
                if (Input.GetKeyDown(KeyCode.R))
                {
                    CollectWater(); // Llama a CollectWater al presionar R
                }
            }
            else
            {
                playerInRange = false;
                interactionText.gameObject.SetActive(false);
                mensajeAgua.text = ""; // Limpia el mensaje al salir
            }
        }
    }

    void CollectWater()
    {
        if (waterLevel >= waterToCollect)
        {
            waterLevel -= waterToCollect;
            playerWater += waterToCollect; // Añade agua al jugador
            UpdateWaterLevelText();
            mensajeAgua.text = "Tienes " + playerWater + " de agua"; // Muestra el agua del jugador
            Debug.Log("Agua recogida. Nivel de agua restante: " + waterLevel);
            hasCollectedWater = true; // Actualiza el flag a verdadero
            AddWaterToInventory(); // Añadimos el agua al inventario
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

    void AddWaterToInventory()
    {
        if (playerInventory != null && hasCollectedWater) // Verifica si se recogió agua
        {
            // Crear una instancia del objeto Item "Cubo de Agua"
            WaterItem waterItem = ScriptableObject.CreateInstance<WaterItem>();
            waterItem.DisplayName = itemName;  // Nombre que se mostrará en el inventario
            //waterItem.Description = "Un cubo lleno de agua fresca."; // Descripción del objeto
            waterItem.Icon = waterCuboIcon;  // Usamos el icono asignado en el Inspector
            waterItem.Stack = 1; // Cantidad inicial del objeto

            // Añadir el objeto al inventario
            bool success = playerInventory.AddItem(waterItem);

            if (success)
            {
                Debug.Log("Cubo de Agua añadido al inventario.");
                mensajeAgua.text = "Cubo de Agua añadido al inventario."; // Actualizar el mensaje
            }
            else
            {
                Debug.LogWarning("No se pudo añadir Cubo de Agua al inventario.");
                mensajeAgua.text = "No se pudo añadir Cubo de Agua."; // Actualizar el mensaje
            }
        }
        else if (playerInventory == null)
        {
            Debug.LogError("No se ha asignado el inventario del jugador en el Inspector.");
        }
        // No se añade nada al inventario si no se ha recogido agua.
    }

    void MostrarMensajeAgua()
    {
        mensajeAgua.text = "Tienes " + playerWater + " de agua"; // Muestra el agua del jugador
    }
}

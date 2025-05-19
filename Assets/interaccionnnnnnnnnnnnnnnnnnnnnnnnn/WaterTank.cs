using UnityEngine;
using TMPro; // Si usas TextMeshPro
using UnityEngine.UI; // Si usas la UI de Unity

public class WaterTank : MonoBehaviour
{
    public int waterLevel = 100; // Nivel de agua inicial
    public int waterToCollect = 5; // Cantidad de agua a recoger
    public TextMeshProUGUI waterLevelText; // Referencia al texto de la UI
    public TextMeshProUGUI interactionText; // Texto para mostrar "Presiona R para recoger agua"
    public GameObject waterCubo; // Prefab del objeto "cubo de agua" del inventario
    public Transform inventoryParent; // Transform del objeto padre del inventario en la UI
    public GameObject player;  // Referencia al objeto del jugador ¡PÚBLICA!
    public float interactionDistance = 5f; // Distancia máxima para interactuar
    public TextMeshProUGUI mensajeAgua; // Referencia al texto para el mensaje de agua ¡PÚBLICA!

    private bool playerInRange = false;
    private int playerWater = 0; // Cantidad de agua que tiene el jugador

    void Start()
    {
        interactionText.gameObject.SetActive(false); // Oculta el texto al inicio
        UpdateWaterLevelText();
        if (player == null)
        {
            Debug.LogError("No se ha asignado el objeto del jugador en el Inspector. Asegúrate de arrastrar el objeto del jugador al campo 'Player' del script WaterTank.");
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

    void AddWaterToInventory()
    {
        if (waterCubo != null && inventoryParent != null)
        {
            //Instanciar el cubo de agua y añadirlo al inventario
            GameObject cuboDeAguaInstanciado = Instantiate(waterCubo, inventoryParent);
        }
    }

    void MostrarMensajeAgua()
    {
        mensajeAgua.text = "Tienes " + playerWater + " de agua"; // Muestra el agua del jugador
    }
}


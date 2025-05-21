using UnityEngine;
using TMPro;
using DevionGames.InventorySystem;
using System.Collections;

public class FoodPlant : MonoBehaviour
{
    [Header("Referencias visuales (modelos)")]
    // public GameObject tomatoPot; // Si es parte del objeto FoodPlant principal o siempre visible, no necesita controlarse aquí
    // public GameObject tomatoSoil; // Idem
    public GameObject tomatoReadyVisual;  // El visual de la planta con fruto (antes TomatoRed)

    [Header("Parámetros de crecimiento")]
    public float tiempoCrecimiento = 10f;
    public string nombreComidaArecolectar = "Tomate";
    public Sprite iconoComida;

    [Header("UI e interacción")]
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI feedbackText;
    public GameObject player;
    public ItemContainer playerInventory;
    public float interactionDistance = 3f;

    private enum PlantState { Empty, Growing, Ready }
    private PlantState currentState = PlantState.Empty;
    private bool playerInRange = false;
    private Coroutine growthCoroutine;

    void Start()
    {
        if (player == null) Debug.LogError("FoodPlant: Player no asignado.", this);
        if (playerInventory == null) Debug.LogError("FoodPlant: PlayerInventory no asignado.", this);
        if (interactionText == null) Debug.LogError("FoodPlant: InteractionText no asignado.", this);
        // Ya no necesitamos tomatoSoil como referencia si siempre está visible como parte del FoodPlant base
        if (tomatoReadyVisual == null) Debug.LogError("FoodPlant: 'tomatoReadyVisual' (planta con fruto) NO asignado.", this);
        if (iconoComida == null && !string.IsNullOrEmpty(nombreComidaArecolectar)) Debug.LogWarning($"FoodPlant: Icono para '{nombreComidaArecolectar}' no asignado.", this);

        UpdateVisuals(); // Esto ahora solo controlará tomatoReadyVisual
        if (interactionText != null) interactionText.gameObject.SetActive(false);
        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector3.Distance(transform.position, player.transform.position);
        bool previouslyInRange = playerInRange;
        playerInRange = distancia <= interactionDistance;

        if (playerInRange)
        {
            if (!previouslyInRange)
            {
                if (interactionText != null) interactionText.gameObject.SetActive(true);
                if (feedbackText != null) feedbackText.gameObject.SetActive(true);
                UpdateInteractionText();
            }
            if (Input.GetKeyDown(KeyCode.E)) HandleInteraction();
        }
        else
        {
            if (previouslyInRange)
            {
                if (interactionText != null) interactionText.gameObject.SetActive(false);
                if (feedbackText != null) { feedbackText.text = ""; feedbackText.gameObject.SetActive(false); }
            }
        }
    }

    void UpdateInteractionText()
    {
        if (interactionText == null || !playerInRange) return;
        string textToShow = "";
        switch (currentState)
        {
            case PlantState.Empty: textToShow = "Presiona [E] para plantar"; break;
            case PlantState.Growing: textToShow = "La planta está creciendo..."; break;
            case PlantState.Ready: textToShow = "Presiona [E] para recolectar " + nombreComidaArecolectar; break;
        }
        interactionText.text = textToShow;
    }

    void HandleInteraction()
    {
        switch (currentState)
        {
            case PlantState.Empty: IntentarPlantar(); break;
            case PlantState.Growing: ShowFeedback("La planta aún está creciendo."); break;
            case PlantState.Ready: RecolectarComida(); break;
        }
    }

    void IntentarPlantar()
    {
        currentState = PlantState.Growing;
        ShowFeedback("Planta sembrada.");
        UpdateVisuals(); // tomatoReadyVisual se desactivará aquí si estaba activo
        UpdateInteractionText();
        if (growthCoroutine != null) StopCoroutine(growthCoroutine);
        growthCoroutine = StartCoroutine(CrecerPlantaCoroutine());
    }

    IEnumerator CrecerPlantaCoroutine()
    {
        yield return new WaitForSeconds(tiempoCrecimiento);
        if (currentState == PlantState.Growing)
        {
            currentState = PlantState.Ready;
            ShowFeedback("¡Planta lista para recolectar!");
            UpdateVisuals(); // tomatoReadyVisual se activará aquí
            UpdateInteractionText();
        }
        growthCoroutine = null;
    }

    void RecolectarComida()
    {
        if (playerInventory == null) {
            ShowFeedback("Error: Inventario del jugador no asignado.");
            Debug.LogError("FoodPlant: No se puede recolectar comida, playerInventory es null.", this);
            return;
        }

        Item comidaInstance = ScriptableObject.CreateInstance<Item>();
        comidaInstance.DisplayName = nombreComidaArecolectar;
        comidaInstance.Icon = iconoComida;
        comidaInstance.Stack = 1;

        bool añadido = playerInventory.AddItem(comidaInstance);
        if (añadido)
        {
            ShowFeedback(nombreComidaArecolectar + " recolectado.");
            ResetearPlanta();
        }
        else
        {
            ShowFeedback("Inventario lleno.");
        }
    }

    void ResetearPlanta()
    {
        currentState = PlantState.Empty;
        UpdateVisuals(); // tomatoReadyVisual se desactivará aquí
        UpdateInteractionText();
    }

    void UpdateVisuals()
    {
        // TomatoPot y TomatoSoil se asume que son parte del objeto FoodPlant
        // y siempre visibles, o no necesitan ser controlados por este script
        // si son estáticos o controlados por otro sistema.

        // Solo controlamos la visibilidad de la planta con fruto (tomatoReadyVisual)
        if (tomatoReadyVisual != null)
        {
            tomatoReadyVisual.SetActive(currentState == PlantState.Ready);
        }
    }

    void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            if (playerInRange) feedbackText.gameObject.SetActive(true);
        }
        Debug.Log("FoodPlant Feedback: " + message);
    }

    void OnDisable()
    {
        if (growthCoroutine != null) { StopCoroutine(growthCoroutine); growthCoroutine = null; }
    }
}
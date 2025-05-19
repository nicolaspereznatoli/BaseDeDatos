using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro
public class CharacterEnergy : MonoBehaviour
{

    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyDecreaseRate = 0.1f; // Energía disminuida por segundo
    public float sleepRestoreAmount = 50f;

    [Header("UI Elements")]
    public Image energyBar; // Opcional: Asigna la Image para la barra de energía aquí
    public TMP_Text energyText_TMP; // Opcional: Asigna un TextMeshPro Text aquí
    public Text energyText_Legacy; // Opcional: Asigna un Legacy UI Text aquí

    [Header("Sleep Detection")]
    public SimulateSleep sleepScript; // Asigna tu script SimulateSleep aquí

    private float lastEnergyValue;

    void Start()
    {
        currentEnergy = maxEnergy;
        lastEnergyValue = currentEnergy;
        UpdateUI();

        // Asegurarse de que el script SimulateSleep esté asignado
        if (sleepScript == null)
        {
            Debug.LogError("¡El script SimulateSleep no está asignado a CharacterEnergy!");
        }
    }

    void Update()
    {
        // Disminuir lentamente la energía con el tiempo
        currentEnergy -= energyDecreaseRate * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        // Comprobar si la energía ha cambiado significativamente para actualizar la UI
        if (Mathf.Abs(currentEnergy - lastEnergyValue) > 0.1f)
        {
            UpdateUI();
            lastEnergyValue = currentEnergy;
        }

        // Comprobar si dormir debería restaurar energía
        if (sleepScript != null && !sleepScript.isFading) // Asumiendo que 'isFading' en tu script de sueño indica que está durmiendo
        {
            if (sleepScript.hasSlept) // Asumiendo que añades un flag 'hasSlept' a tu script de sueño
            {
                currentEnergy = Mathf.Min(currentEnergy + sleepRestoreAmount, maxEnergy);
                UpdateUI();
                sleepScript.hasSlept = false; // Resetear el flag
            }
        }

        if (currentEnergy <= 0)
        {
            // Manejar la muerte o el agotamiento del personaje aquí
            Debug.Log("¡El personaje se ha quedado sin energía!");
            // Podrías querer deshabilitar el control del jugador o activar un estado de game over.
        }
    }

    void UpdateUI()
    {
        if (energyBar != null)
        {
            energyBar.fillAmount = currentEnergy / maxEnergy;
        }

        if (energyText_TMP != null)
        {
            energyText_TMP.text = Mathf.RoundToInt(currentEnergy).ToString();
        }
        else if (energyText_Legacy != null)
        {
            energyText_Legacy.text = Mathf.RoundToInt(currentEnergy).ToString();
        }
    }

    // Método público que tu script de sueño puede llamar cuando el personaje se despierta
    public void RestoreEnergyFromSleep()
    {
        currentEnergy = Mathf.Min(currentEnergy + sleepRestoreAmount, maxEnergy);
        UpdateUI();
    }
}

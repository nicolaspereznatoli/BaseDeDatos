using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro
public class CharacterEnergy : MonoBehaviour
{

    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyDecreaseRate = 0.1f; // Energ�a disminuida por segundo
    public float sleepRestoreAmount = 50f;

    [Header("UI Elements")]
    public Image energyBar; // Opcional: Asigna la Image para la barra de energ�a aqu�
    public TMP_Text energyText_TMP; // Opcional: Asigna un TextMeshPro Text aqu�
    public Text energyText_Legacy; // Opcional: Asigna un Legacy UI Text aqu�

    [Header("Sleep Detection")]
    public SimulateSleep sleepScript; // Asigna tu script SimulateSleep aqu�

    private float lastEnergyValue;

    void Start()
    {
        currentEnergy = maxEnergy;
        lastEnergyValue = currentEnergy;
        UpdateUI();

        // Asegurarse de que el script SimulateSleep est� asignado
        if (sleepScript == null)
        {
            Debug.LogError("�El script SimulateSleep no est� asignado a CharacterEnergy!");
        }
    }

    void Update()
    {
        // Disminuir lentamente la energ�a con el tiempo
        currentEnergy -= energyDecreaseRate * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        // Comprobar si la energ�a ha cambiado significativamente para actualizar la UI
        if (Mathf.Abs(currentEnergy - lastEnergyValue) > 0.1f)
        {
            UpdateUI();
            lastEnergyValue = currentEnergy;
        }

        // Comprobar si dormir deber�a restaurar energ�a
        if (sleepScript != null && !sleepScript.isFading) // Asumiendo que 'isFading' en tu script de sue�o indica que est� durmiendo
        {
            if (sleepScript.hasSlept) // Asumiendo que a�ades un flag 'hasSlept' a tu script de sue�o
            {
                currentEnergy = Mathf.Min(currentEnergy + sleepRestoreAmount, maxEnergy);
                UpdateUI();
                sleepScript.hasSlept = false; // Resetear el flag
            }
        }

        if (currentEnergy <= 0)
        {
            // Manejar la muerte o el agotamiento del personaje aqu�
            Debug.Log("�El personaje se ha quedado sin energ�a!");
            // Podr�as querer deshabilitar el control del jugador o activar un estado de game over.
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

    // M�todo p�blico que tu script de sue�o puede llamar cuando el personaje se despierta
    public void RestoreEnergyFromSleep()
    {
        currentEnergy = Mathf.Min(currentEnergy + sleepRestoreAmount, maxEnergy);
        UpdateUI();
    }
}

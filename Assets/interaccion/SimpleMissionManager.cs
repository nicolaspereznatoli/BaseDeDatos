using UnityEngine;
using TMPro;

public class SimpleMissionManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI missionText; // Arrastra aquí el TMP Text desde la escena

    [Header("Misión")]
    public string currentMission = "";
    public bool missionActive = false;
    public bool missionCompleted = false;
    public int experience = 0;

    void Start()
    {
        Invoke("StartMission", 5f); // Lanza la misión tras 5 segundos
    }

    void StartMission()
    {
        currentMission = "🌱 Planta una semilla para conseguir comida";
        missionText.text = currentMission;
        missionActive = true;
    }

    // Llama a esta función cuando el jugador plante una semilla
    public void OnSeedPlanted()
    {
        if (missionActive && !missionCompleted)
        {
            missionCompleted = true;
            missionActive = false;
            missionText.text = ""; // Oculta texto
            experience += 10;
            Debug.Log("✅ Misión completada +10 XP. Total XP: " + experience);
        }
    }
}



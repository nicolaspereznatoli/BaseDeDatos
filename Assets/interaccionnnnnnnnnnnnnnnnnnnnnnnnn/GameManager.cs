using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int experiencia = 5;
    public event Action<int> OnExperienciaCambiada;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Notificamos la XP inicial al iniciar
        OnExperienciaCambiada?.Invoke(experiencia);
    }

    public void SumarXP(int cantidad)
    {
        experiencia += cantidad;
        if (experiencia < 0) experiencia = 0;

        Debug.Log("XP actual: " + experiencia);
        OnExperienciaCambiada?.Invoke(experiencia);
    }
}

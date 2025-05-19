using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class barragasolina : MonoBehaviour
{
    public Image barraDeGasolinaImagen;
    public TMP_Text textoGasolina; // Referencia al objeto TextMeshPro Text
    public float gasolinaMaxima = 100f;
    private float gasolinaActual;
    public float perdidaDeGasolinaPorSegundo = 1f;

    void Start()
    {
        gasolinaActual = gasolinaMaxima;
        ActualizarUI();
        InvokeRepeating("PerderGasolina", 1f, 1f);
    }

    void PerderGasolina()
    {
        gasolinaActual -= perdidaDeGasolinaPorSegundo * Time.deltaTime;
        if (gasolinaActual < 0)
        {
            gasolinaActual = 0;
            Debug.Log("¡Gasolina agotada!");
            // Opcional: CancelInvoke("PerderGasolina");
        }
        ActualizarUI();
    }

    void ActualizarUI()
    {
        // Actualizar la barra de gasolina (imagen)
        if (barraDeGasolinaImagen != null)
        {
            barraDeGasolinaImagen.fillAmount = gasolinaActual / gasolinaMaxima;
        }
        else
        {
            Debug.LogError("La referencia a la imagen de la barra de gasolina no está asignada en el Inspector.");
        }

        // Actualizar el texto de la gasolina
        if (textoGasolina != null)
        {
            textoGasolina.text = Mathf.RoundToInt(gasolinaActual).ToString(); // Muestra el valor entero
            // Opcionalmente, podrías mostrar decimales:
            // textoGasolina.text = gasolinaActual.ToString("F0"); // Sin decimales
            // textoGasolina.text = gasolinaActual.ToString("F1"); // Un decimal
        }
        else
        {
            Debug.LogError("La referencia al texto de la gasolina (TextMeshPro) no está asignada en el Inspector.");
        }
    }

    public void ConsumirGasolina(float cantidad)
    {
        gasolinaActual -= cantidad;
        if (gasolinaActual < 0)
        {
            gasolinaActual = 0;
            Debug.Log("¡Gasolina agotada por consumo!");
            // Opcional: CancelInvoke("PerderGasolina");
        }
        ActualizarUI();
    }

    public void RellenarGasolina(float cantidad)
    {
        gasolinaActual += cantidad;
        if (gasolinaActual > gasolinaMaxima)
        {
            gasolinaActual = gasolinaMaxima;
        }
        ActualizarUI();
    }
}

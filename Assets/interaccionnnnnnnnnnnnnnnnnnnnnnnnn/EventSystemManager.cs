using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EventSystemManager : MonoBehaviour
{
    public List<Evento> eventosPosibles;
    private NuevoDBManager dbManager;
    public TextMeshProUGUI eventosTexto;

    void Awake()
    {
        dbManager = FindObjectOfType<NuevoDBManager>();
        if (dbManager == null)
        {
            Debug.LogError("No se encontró NuevoDBManager en la escena.");
        }
        else
        {
            Debug.Log("NuevoDBManager encontrado.");
        }
    }

    void Start()
    {
        if (eventosPosibles != null && eventosPosibles.Count > 0)
        {
            StartCoroutine(MostrarEventosSecuencialmente());
        }
        else
        {
            Debug.LogWarning("No hay eventos para mostrar.");
            eventosTexto.text = "No hay eventos disponibles.";
        }
    }

    IEnumerator MostrarEventosSecuencialmente()
    {
        foreach (Evento eventoElegido in eventosPosibles)
        {
            yield return new WaitForSeconds(3f);

            // Mostrar "Nuevo evento sorpresa" en rojo
            eventosTexto.text = "<color=red>Nuevo evento sorpresa</color>\n";

            // Esperar 1 segundo para efecto dramático
            yield return new WaitForSeconds(3f);

            // Mostrar nombre y descripción en color normal
            eventosTexto.text = $"Evento: {eventoElegido.tipo}\nDescripción: {eventoElegido.descripcion}";

            AplicarEvento(eventoElegido);

            // Esperar 5 segundos antes del siguiente evento
            yield return new WaitForSeconds(10f);
        }

        eventosTexto.text = "No hay más eventos";
    }

    void AplicarEvento(Evento e)
    {
        if (e.impacto != 0)
        {
            if (e.impacto > 0)
                dbManager.AumentarCantidadSuministroEnNave(e.suministroAfectado, e.impacto);
            else
                dbManager.RestarCantidadSuministroEnNave(e.suministroAfectado, Mathf.Abs(e.impacto));
        }

        int idEvento = dbManager.InsertarEvento(e.tipo, e.descripcion);
        int idSuministro = dbManager.ObtenerIDSuministroPorTipo(e.suministroAfectado);
        dbManager.InsertarAfecta(idEvento, 0, false);
    }
}

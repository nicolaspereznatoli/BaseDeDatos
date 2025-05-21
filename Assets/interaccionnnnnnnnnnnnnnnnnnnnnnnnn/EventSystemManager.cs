using UnityEngine;
using System.Collections.Generic;

public class EventSystemManager : MonoBehaviour
{
    public List<Evento> eventosPosibles;
    private NuevoDBManager dbManager;

    void Awake()
    {
        dbManager = FindObjectOfType<NuevoDBManager>();
        if (dbManager == null)
        {
            Debug.LogError("No se encontr� NuevoDBManager en la escena.");
        }
        else
        {
            Debug.Log("NuevoDBManager encontrado.");
        }
    }

    void Start()
    {
        GenerarEventos();
    }

    public void GenerarEventos()
    {
        if (eventosPosibles == null || eventosPosibles.Count == 0)
        {
            Debug.LogWarning("No hay eventos para generar.");
            return;
        }

        foreach (Evento eventoElegido in eventosPosibles)
        {
            Debug.Log("Nuevo evento generado:");
            Debug.Log($"Tipo: {eventoElegido.tipo}");
            Debug.Log($"Descripci�n: {eventoElegido.descripcion}");
            Debug.Log($"Afecta a: {eventoElegido.suministroAfectado} con impacto de {eventoElegido.impacto}");

            AplicarEvento(eventoElegido);
        }
    }

    void AplicarEvento(Evento e)
    {
        // 1. Modificar los recursos en la nave seg�n impacto
        if (e.impacto != 0)
        {
            if (e.impacto > 0)
                dbManager.AumentarCantidadSuministroEnNave(e.suministroAfectado, e.impacto);
            else
                dbManager.RestarCantidadSuministroEnNave(e.suministroAfectado, Mathf.Abs(e.impacto));
        }

        // 2. Insertar el evento en la base de datos y obtener su ID
        int idEvento = dbManager.InsertarEvento(e.tipo, e.descripcion);

        // 3. Obtener el ID del suministro afectado
        int idSuministro = dbManager.ObtenerIDSuministroPorTipo(e.suministroAfectado);

        // 4. Insertar el registro de qu� afecta el evento (no afecta m�quina aqu�)
        dbManager.InsertarAfecta(idEvento, idSuministro, 0, false);
    }
}

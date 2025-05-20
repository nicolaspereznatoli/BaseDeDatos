using UnityEngine;
using System.Collections.Generic;

public class EventSystemManager : MonoBehaviour
{
    public List<Evento> eventosPosibles;

    void Start()
    {
        GenerarEventoAleatorio();
    }

    public void GenerarEventoAleatorio()
    {
        if (eventosPosibles.Count == 0) return;

        int index = Random.Range(0, eventosPosibles.Count);
        Evento eventoElegido = eventosPosibles[index];

        Debug.Log("Nuevo evento generado:");
        Debug.Log($"Tipo: {eventoElegido.tipo}");
        Debug.Log($"Descripción: {eventoElegido.descripcion}");
        Debug.Log($"Afecta a: {eventoElegido.suministroAfectado} con impacto de {eventoElegido.impacto}");

        AplicarEvento(eventoElegido);
    }

    void AplicarEvento(Evento e)
    {
        // Aquí modificarías valores reales del juego, por ejemplo:
        if (e.suministroAfectado == "chatarra")
        {
            Inventario.instance.ModificarChatarra(e.impacto);
        }
        else if (e.suministroAfectado == "semillas")
        {
            Inventario.instance.ModificarSemillas(e.impacto);
        }

        // Puedes disparar eventos visuales, UI, sonidos, etc.
    }
}


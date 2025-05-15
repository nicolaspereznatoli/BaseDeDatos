using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractivo : MonoBehaviour
{
    public Canvas canvasObjetivo;

    public void activarObjeto()
    {
        if (canvasObjetivo != null)
        {
            StartCoroutine(MostrarCanvasConFlickeo());
        }
        else
        {
            Debug.LogWarning("No se asignó un Canvas al objeto interactivo.");
        }
    }

    private IEnumerator MostrarCanvasConFlickeo()
    {
        canvasObjetivo.gameObject.SetActive(true);
        Debug.Log("Canvas activado");

        yield return new WaitForSeconds(5.0f); // Tiempo visible normal antes de flickeo

        // Flickeo
        for (int i = 0; i < 4; i++)
        {
            canvasObjetivo.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            canvasObjetivo.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.05f);
        }

        canvasObjetivo.gameObject.SetActive(false);
        Debug.Log("Canvas desactivado después del flickeo");
    }
}

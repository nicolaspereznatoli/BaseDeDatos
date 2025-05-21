using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;
using Mono.Data.Sqlite;
using System.Data;


public class ItemInteractivo : MonoBehaviour
{
    public Canvas canvasObjetivo;
   


    public void activarObjeto()
    {
        if (gameObject.name == "JuegoChatarra")
        {
            SceneManager.LoadScene("SnakeGame");

        }
        if (gameObject.name == "Cuborefineria")
        {
            Debug.Log("hola");
                GenerarGasolina();
                Debug.Log("adios");
        }
       // if (canvasObjetivo != null)
       // {
       //     StartCoroutine(MostrarCanvasConFlickeo());
       // }
       // else
       // {
       //     Debug.LogWarning("No se asign� un Canvas al objeto interactivo.");
       // }
    }

    //private IEnumerator MostrarCanvasConFlickeo()
    //{
    //    canvasObjetivo.gameObject.SetActive(true);
    //    Debug.Log("Canvas activado");
    //
    //    yield return new WaitForSeconds(4.2f); // Tiempo visible normal antes de flickeo
    //
    //    // Flickeo
    //    for (int i = 0; i < 4; i++)
    //    {
    //        canvasObjetivo.gameObject.SetActive(false);
    //        yield return new WaitForSeconds(0.1f);
    //        canvasObjetivo.gameObject.SetActive(true);
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //
    //    canvasObjetivo.gameObject.SetActive(false);
    //    Debug.Log("Canvas desactivado despu�s del flickeo");
    //}
    public TextMeshProUGUI mensajeTexto;
    private string dbPath => "URI=file:" + Application.persistentDataPath + "/NaveDB.db";

    private IDbConnection OpenConnection()
    {
        IDbConnection connection = new SqliteConnection(dbPath);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Llama esta función para intentar generar gasolina
    /// </summary>
    public void GenerarGasolina()
    {
        if (TieneSuficiente("Chatarra", 3))
        {
            ModificarRecurso("Chatarra", -3);
            ModificarRecurso("Gasolina", 1);
            mensajeTexto.text = "Has generado 1 unidad de Gasolina";
        }
        else
        {
            StartCoroutine(Esperar());
        }
    }
    public IEnumerator Esperar()
    {
        mensajeTexto.text = "No tienes suficiente Chatarra para generar Gasolina";

        yield return new WaitForSeconds(5f); // espera de 2 segundos
        mensajeTexto.text = "";

    }


    /// <summary>
    /// Verifica si hay suficiente cantidad de un recurso
    /// </summary>
    private bool TieneSuficiente(string tipo, int cantidadNecesaria)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Cantidad FROM SuministroNave WHERE TipoSuministro = @tipo";
            cmd.Parameters.Add(new SqliteParameter("@tipo", tipo));

            using (IDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int cantidadActual = reader.GetInt32(0);
                    return cantidadActual >= cantidadNecesaria;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Modifica la cantidad de un suministro (puede sumar o restar)
    /// </summary>
    private void ModificarRecurso(string tipo, int delta)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE SuministroNave SET Cantidad = Cantidad + @delta WHERE TipoSuministro = @tipo";
            cmd.Parameters.Add(new SqliteParameter("@delta", delta));
            cmd.Parameters.Add(new SqliteParameter("@tipo", tipo));
            cmd.ExecuteNonQuery();
        }
    }
}

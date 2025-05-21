using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;
using System;

public class barragasolina : MonoBehaviour
{
    public Image barraDeGasolinaImagen;
    public TMP_Text textoGasolina;
    public float gasolinaMaxima = 100f;
    private float gasolinaActual = 0f;
    public float perdidaDeGasolinaPorSegundo = 1f;

    private float tiempoGuardado = 0f;
    private float intervaloGuardado = 1f; // guarda cada 1 segundo

    private string dbPath => "URI=file:" + Application.persistentDataPath + "/NaveDB.db";

    void Start()
    {
       
        CargarGasolina();
        InvokeRepeating("PerderGasolina", 1f, 1f);
    }

    void Update()
    {
        
        ActualizarUI();

        tiempoGuardado += Time.deltaTime;
        if (tiempoGuardado >= intervaloGuardado)
        {
            GuardarGasolina();
            tiempoGuardado = 0f;
        }
    }

    void PerderGasolina()
    {
        gasolinaActual -= perdidaDeGasolinaPorSegundo * Time.deltaTime;
        if (gasolinaActual < 0) gasolinaActual = 0;
    }

    void ActualizarUI()
    {
        if (barraDeGasolinaImagen != null)
            barraDeGasolinaImagen.fillAmount = gasolinaActual / gasolinaMaxima;

        if (textoGasolina != null)
            textoGasolina.text = Mathf.RoundToInt(gasolinaActual).ToString();
    }

    public void ConsumirGasolina(float cantidad)
    {
        gasolinaActual -= cantidad;
        if (gasolinaActual < 0) gasolinaActual = 0;
        GuardarGasolina(); // <-- Guardar inmediatamente
    }

    public void RellenarGasolina(float cantidad)
    {
        gasolinaActual += cantidad;
        if (gasolinaActual > gasolinaMaxima) gasolinaActual = gasolinaMaxima;
        GuardarGasolina(); // <-- Guardar inmediatamente
    }


    private IDbConnection OpenConnection()
    {
        IDbConnection connection = new SqliteConnection(dbPath);
        connection.Open();
        return connection;
    }

   public void CargarGasolina()
    {
        using (var connection = OpenConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT Cantidad FROM SuministroNave WHERE TipoSuministro = 'Gasolina'";
            var result = command.ExecuteScalar();

            if (result != null && result != DBNull.Value)
                gasolinaActual = System.Convert.ToSingle(result);
            else
                gasolinaActual = 0f;
        }
    }

    void GuardarGasolina()
    {
        using (var connection = OpenConnection())
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "UPDATE SuministroNave SET Cantidad = @cantidad WHERE TipoSuministro = 'Gasolina'";
            var param = command.CreateParameter();
            param.ParameterName = "@cantidad";
            param.Value = gasolinaActual;
            command.Parameters.Add(param);
            command.ExecuteNonQuery();
        }
    }
    

  
}

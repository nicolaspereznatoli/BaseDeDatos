using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

using Unity.VisualScripting.Dependencies.Sqlite;
using System;

public class NuevoDBManager : MonoBehaviour
{

    private string dbPath;

    void Awake()
    {
        dbPath = "URI=file:" + Path.Combine(Application.persistentDataPath, "NaveDB.db");
        Debug.Log("Ruta de la base de datos: " + dbPath);

    }

    private IDbConnection OpenConnection()
    {
        IDbConnection connection = new SqliteConnection(dbPath);
        connection.Open();
        return connection;
    }

    //Inicializar valores iniciales de ciertas tablas con filas predeterminadas

    private void Start()
    {
        InTripulante();
        INMaquina();
        InSuministros();
    }

    private void INMaquina()
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();

            // Verificar si ya existen máquinas
            cmd.CommandText = "SELECT COUNT(*) FROM Maquina";
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
            {
                // ------------------------------
                // Insertar Maquina_Agua
                cmd.CommandText = "INSERT INTO Maquina (Nombre, TipoSuministro, Cantidad, Estado) " +
                                  "VALUES (@nombre1, @tipo1, @cantidad1, @estado1)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@nombre1", "Maquina_Agua"));
                cmd.Parameters.Add(new SqliteParameter("@tipo1", "Agua"));
                cmd.Parameters.Add(new SqliteParameter("@cantidad1", 0));
                cmd.Parameters.Add(new SqliteParameter("@estado1", "Funcional"));
                cmd.ExecuteNonQuery();

                // ------------------------------
                // Insertar Maquina_Gasolina
                cmd.CommandText = "INSERT INTO Maquina (Nombre, TipoSuministro, Cantidad, Estado) " +
                                  "VALUES (@nombre2, @tipo2, @cantidad2, @estado2)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@nombre2", "Maquina_Gasolina"));
                cmd.Parameters.Add(new SqliteParameter("@tipo2", "Gasolina"));
                cmd.Parameters.Add(new SqliteParameter("@cantidad2", 0));
                cmd.Parameters.Add(new SqliteParameter("@estado2", "Funcional"));
                cmd.ExecuteNonQuery();

                // ------------------------------
                // Insertar Maquina_Comida
                cmd.CommandText = "INSERT INTO Maquina (Nombre, TipoSuministro, Cantidad, Estado) " +
                                  "VALUES (@nombre3, @tipo3, @cantidad3, @estado3)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@nombre3", "Maquina_Comida"));
                cmd.Parameters.Add(new SqliteParameter("@tipo3", "Comida"));
                cmd.Parameters.Add(new SqliteParameter("@cantidad3", 0));
                cmd.Parameters.Add(new SqliteParameter("@estado3", "Funcional"));
                cmd.ExecuteNonQuery();

                //Insertar Maquina de Oxigeno

                cmd.CommandText = "INSERT INTO Maquina (Nombre, TipoSuministro, Cantidad, Estado) " +
                                  "VALUES (@nombre4, @tipo4, @cantidad4, @estado4)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@nombre4", "Maquina_Oxigeno"));
                cmd.Parameters.Add(new SqliteParameter("@tipo4", "Oxigeno"));
                cmd.Parameters.Add(new SqliteParameter("@cantidad4", 0));
                cmd.Parameters.Add(new SqliteParameter("@estado4", "Funcional"));
                cmd.ExecuteNonQuery();
            }
        }

    }

    private void InSuministros()
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();

            // Verificar si ya existen máquinas
            cmd.CommandText = "SELECT COUNT(*) FROM SuministroNave";
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
            {
                cmd.CommandText = "INSERT INTO SuministroNave (TipoSuministro, Cantidad) " +
                  "VALUES (@tipo1, @cantidad1)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@tipo1", "Gasolina")); 
                cmd.Parameters.Add(new SqliteParameter("@cantidad1", 80));       
                cmd.ExecuteNonQuery();

                // ------------------------------

                cmd.CommandText = "INSERT INTO SuministroNave (TipoSuministro, Cantidad) " +
                  "VALUES (@tipo2, @cantidad2)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@tipo2", "Agua"));  
                cmd.Parameters.Add(new SqliteParameter("@cantidad2", 100));        
                cmd.ExecuteNonQuery();

                // ------------------------------

                cmd.CommandText = "INSERT INTO SuministroNave (TipoSuministro, Cantidad) " +
                  "VALUES (@tipo3, @cantidad3)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@tipo3", "Oxigeno"));  
                cmd.Parameters.Add(new SqliteParameter("@cantidad3", 100));        
                cmd.ExecuteNonQuery();

                //

                cmd.CommandText = "INSERT INTO SuministroNave (TipoSuministro, Cantidad) " +
                  "VALUES (@tipo4, @cantidad4)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@tipo4", "Comida")); 
                cmd.Parameters.Add(new SqliteParameter("@cantidad4", 20));       
                cmd.ExecuteNonQuery();

                //

                cmd.CommandText = "INSERT INTO SuministroNave (TipoSuministro, Cantidad) " +
                  "VALUES (@tipo5, @cantidad5)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@tipo5", "Chatarra"));
                cmd.Parameters.Add(new SqliteParameter("@cantidad5", 10));
                cmd.ExecuteNonQuery();

            }
        }


    }

    private void InTripulante()
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();

            // Verificar si ya existen máquinas
            cmd.CommandText = "SELECT COUNT(*) FROM Tripulante";
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count == 0)
            {
                cmd.CommandText = "INSERT INTO Tripulante (Energia, Sed, Hambre) " +
                  "VALUES (@energia, @sed, @hambre)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@energia", 100));   
                cmd.Parameters.Add(new SqliteParameter("@sed", 0));         
                cmd.Parameters.Add(new SqliteParameter("@hambre", 0));      
                cmd.ExecuteNonQuery();

            }
        }

    }


    // Hasta aqui es solo para abrir y poder usar la base de datos

    //METODOS

    public void ConsigueChatarra(int c)
    {

        //Primero a la tabla de recompensas

        InsertarRecompensa("Chatarra", c);

        //Aumentar recurso

        AumentarCantidadSuministroEnNave("Chatarra", c);

    }

    public void CosecharComida(int c)
    {
        InsertarRecompensa("Comida", c);

        AumentarCantidadSuministroEnNave("Comida", c);

    }

    public void GenerarGasolina()
    {

        if (HaySuficientesSuministro("Chatarra", 3) == true)
        {
            AumentarCantidadSuministroEnNave("Gasolina", 1);

        }

    }

    

    


    //IN TERACCIÓN CON LABASE DEDATOS

    //Aumentar y disminuir recursos

    public void AumentarCantidadSuministroEnNave(string tipoSuministro, int cantidadExtra)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE SuministroNave SET Cantidad = Cantidad + @cantidad WHERE TipoSuministro = @tipo";
            cmd.Parameters.Add(new SqliteParameter("@cantidad", cantidadExtra));
            cmd.Parameters.Add(new SqliteParameter("@tipo", tipoSuministro));
            cmd.ExecuteNonQuery();
        }
    }

    public void RestarCantidadSuministroEnNave(string tipoSuministro, int cantidadExtra)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE SuministroNave SET Cantidad = Cantidad - @cantidad WHERE TipoSuministro = @tipo";
            cmd.Parameters.Add(new SqliteParameter("@cantidad", cantidadExtra));
            cmd.Parameters.Add(new SqliteParameter("@tipo", tipoSuministro));
            cmd.ExecuteNonQuery();
        }
    }


    //Para ser usado cuando realice una tarea
    public void InsertarTarea(string descripcion, int idMaquina, int idRecompensa)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Tarea (Descripcion, ID_Maquina, ID_Recompensa) " +
                              "VALUES (@desc, @idMaq, @idRec)";
            cmd.Parameters.Add(new SqliteParameter("@desc", descripcion));
            cmd.Parameters.Add(new SqliteParameter("@idMaq", idMaquina));
            cmd.Parameters.Add(new SqliteParameter("@idRec", idRecompensa));
            cmd.ExecuteNonQuery();
        }
    }


    //Para ser usado cuando use una recompensa

    public int InsertarRecompensa(string tipo, int contenido)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Recompensa (Tipo, Contenido) VALUES (@tipo, @contenido)";
            cmd.Parameters.Add(new SqliteParameter("@tipo", tipo));
            cmd.Parameters.Add(new SqliteParameter("@contenido", contenido));
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT last_insert_rowid()";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }


    //para ser usado cuando se genere una misión

    public void InsertarMision(string nombre, string descripcion, int duracion, int idRecompensa, string estado)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Mision (Nombre, Descripcion, Duracion, ID_Recompensa, Estado) " +
                              "VALUES (@nombre, @desc, @duracion, @idRec, @estado)";
            cmd.Parameters.Add(new SqliteParameter("@nombre", nombre));
            cmd.Parameters.Add(new SqliteParameter("@desc", descripcion));
            cmd.Parameters.Add(new SqliteParameter("@duracion", duracion));
            cmd.Parameters.Add(new SqliteParameter("@idRec", idRecompensa));
            cmd.Parameters.Add(new SqliteParameter("@estado", estado));
            cmd.ExecuteNonQuery();
        }
    }




    //Para ser usado cuando ocurra un evento
    public void CambiarEstadoMaquinaPorNombre(string nombreMaquina, string nuevoEstado)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Maquina SET Estado = @estado WHERE Nombre = @nombre";
            cmd.Parameters.Add(new SqliteParameter("@estado", nuevoEstado));
            cmd.Parameters.Add(new SqliteParameter("@nombre", nombreMaquina));
            cmd.ExecuteNonQuery();
        }
    }
    //lo que permite 
    //CambiarEstadoMaquinaPorNombre("Maquina_Agua", "Activa");



    //para ser usado cuando surga un evento

    public void InsertarAfecta(int idEvento, int idMaquina, bool afectamaquina)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Afecta (ID_Evento, ID_Maquina) " +
                              "VALUES (@idEvento, @idMaquina)";
            cmd.Parameters.Add(new SqliteParameter("@idEvento", idEvento));
            cmd.Parameters.Add(new SqliteParameter("@idMaquina", idMaquina));
            cmd.ExecuteNonQuery();


            //Si afecta a una maquina

            if (afectamaquina == true)
            {
                cmd.CommandText = "SELECT Nombre FROM Maquina WHERE ID_Maquina = @idMaquina";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new SqliteParameter("@idMaquina", idMaquina));
                var nombreMaquina = cmd.ExecuteScalar()?.ToString();

                if (!string.IsNullOrEmpty(nombreMaquina))
                {
                    CambiarEstadoMaquinaPorNombre(nombreMaquina, "averiada");
                }

            }


        }
    }



    //COMPROBACIÓN PARA INTERCAMBIO

    public bool HaySuficientesSuministro(string tipo, int cantidad)
    {
        using (IDbConnection connection = OpenConnection())
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Cantidad FROM SuministroNave WHERE TipoSuministro = @tipo";
            cmd.Parameters.Add(new SqliteParameter("@tipo", tipo));


            //
            using (IDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int cantidadActual = reader.GetInt32(0);
                    return cantidadActual >= cantidad;
                }
            }

            // No se encontró el suministro
            return false;
        }

    }

}

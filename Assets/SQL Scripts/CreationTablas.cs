using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

using Unity.VisualScripting.Dependencies.Sqlite;

public class CreationTablas : MonoBehaviour
{

    private string dbUri = "URI=file:";
    private IDbConnection dbConnection;

    void Start()
    {
        string dbPath = dbUri + Path.Combine(Application.persistentDataPath, "NaveDB.db");
        Debug.Log("Database created at: " + dbPath);
        dbConnection = new SqliteConnection(dbPath);
        dbConnection.Open();

        CreateTabas();

        dbConnection.Close();

    }

    

    void CreateTabas()
    {
        IDbCommand command = dbConnection.CreateCommand();

        command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Tripulante (
            ID_Tripulante INTEGER PRIMARY KEY AUTOINCREMENT,
            Energia INTEGER,
            Sed INTEGER,
            Hambre INTEGER
        );

        CREATE TABLE IF NOT EXISTS Recompensa (
            ID_Recompensa INTEGER PRIMARY KEY AUTOINCREMENT,
            Tipo TEXT,
            Contenido INTEGER
        );

        CREATE TABLE IF NOT EXISTS Maquina (
            ID_Maquina INTEGER PRIMARY KEY AUTOINCREMENT,
            Nombre TEXT,
            TipoSuministro TEXT,
            Cantidad INTEGER,
            Estado TEXT

        );

        CREATE TABLE IF NOT EXISTS SuministroNave (
            ID_SuministroNave INTEGER PRIMARY KEY AUTOINCREMENT,
            TipoSuministro TEXT,
            Cantidad INTEGER
        );

        CREATE TABLE IF NOT EXISTS Tarea (
            ID_Tarea INTEGER PRIMARY KEY AUTOINCREMENT,
            Descripcion TEXT,
            ID_Maquina INTEGER,
            ID_Recompensa INTEGER,
            FOREIGN KEY (ID_Recompensa) REFERENCES Recompensa(ID_Recompensa)
        );

        CREATE TABLE IF NOT EXISTS Mision (
            ID_Mision INTEGER PRIMARY KEY AUTOINCREMENT,
            Nombre TEXT,
            Descripcion TEXT,
            Duracion INTEGER,
            ID_Recompensa INTEGER,
            Estado TEXT,
            FOREIGN KEY (ID_Recompensa) REFERENCES Recompensa(ID_Recompensa)
        );

        CREATE TABLE IF NOT EXISTS Evento (
            ID_Evento INTEGER PRIMARY KEY AUTOINCREMENT,
            Tipo TEXT,
            Descripcion TEXT,
            TiempoLimite INTEGER,
            SuministroAfectado TEXT,
            Impacto INTEGER,
            ID_Mision INTEGER,
            FOREIGN KEY (ID_Mision) REFERENCES Mision(ID_Mision)
        );


        CREATE TABLE IF NOT EXISTS Afecta (
            ID_Evento INTEGER,
            ID_SuministroNave INTEGER,
            ID_Maquina INTEGER,
            PRIMARY KEY (ID_Evento, ID_SuministroNave, ID_Maquina),
            FOREIGN KEY (ID_Evento) REFERENCES Evento(ID_Evento),
            FOREIGN KEY (ID_SuministroNave) REFERENCES SuministroNave(ID_SuministroNave),
            FOREIGN KEY (ID_Maquina) REFERENCES Maquina(ID_Maquina)
        );
        ";

        command.ExecuteNonQuery();
    }


}









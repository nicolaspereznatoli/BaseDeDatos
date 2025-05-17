// En Assets/Scripts/DBManager.cs
using UnityEngine;
using Mono.Data.Sqlite; 
using System.Data;      
using System;           
using System.Collections.Generic; 
using System.IO;        

// Gestiona todas las interacciones con la base de datos SQLite del juego.
// Se encarga de la conexión, creación de esquemas, población inicial de datos,
// y proporciona métodos para realizar operaciones CRUD (Crear, Leer, Actualizar, Borrar)
// sobre las entidades del juego.

public class DBManager : MonoBehaviour
{
    private const string DB_BASE_URI_PREFIX = "URI=file:"; 
    private const string DB_FILE_NAME = "juego_nave.db"; 
    private IDbConnection dbConnection; 

    #region Ciclo de Vida de Unity y Gestión Principal de la Conexión

    // Se ejecuta cuando el script es cargado. Prepara la conexión a la base de datos,
    // asegura la existencia del esquema y los datos iniciales, y ejecuta pruebas.
    void Start()
    {
        string persistentFilePath = Path.Combine(Application.persistentDataPath, DB_FILE_NAME);
        string connectionString = DB_BASE_URI_PREFIX + persistentFilePath;

        Debug.Log($"[DBManager] Ruta de la base de datos: {persistentFilePath}");
        Debug.Log($"[DBManager] Cadena de conexión: {connectionString}");

        // Descomentar para borrar la base de datos en cada inicio. 
        // if (File.Exists(persistentFilePath))
        // {
        //     File.Delete(persistentFilePath);
        //     Debug.Log("[DBManager] Base de datos existente borrada para pruebas.");
        // }

        dbConnection = new SqliteConnection(connectionString);
        try
        {
            dbConnection.Open();
            Debug.Log("[DBManager] Conexión a la base de datos abierta satisfactoriamente.");

            // Activa las restricciones de clave foránea para la sesión actual.
            ExecutePragmaForeignKeysOn();

            CreateTablesFromDDL();
            PopulateDatabaseFromDML();

            // --- Ejecución de Métodos de Prueba ---
            RunDatabaseTests(); 
        }
        catch (Exception e)
        {
            Debug.LogError($"[DBManager] Error crítico durante la inicialización de la BD: {e.Message}\nStackTrace: {e.StackTrace}");
        }
        finally
        {
            // Asegura que la conexión se cierre si fue abierta en este método.
            if (dbConnection != null && dbConnection.State == ConnectionState.Open)
            {
                dbConnection.Close();
                Debug.Log("[DBManager] Conexión a la base de datos cerrada al finalizar Start().");
            }
        }
    }

    // Ejecuta el PRAGMA de SQLite para activar el soporte de claves foráneas.
    private void ExecutePragmaForeignKeysOn()
    {
        using (IDbCommand pragmaCmd = dbConnection.CreateCommand())
        {
            pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaCmd.ExecuteNonQuery();
            Debug.Log("[DBManager] PRAGMA foreign_keys = ON ejecutado.");
        }
    }

    // Crea las tablas de la base de datos ejecutando las sentencias DDL
    // leídas desde el archivo especificado (normalmente "Sql/ddl.sql").
    private void CreateTablesFromDDL()
    {
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            string ddlScriptContent = ReadSqlFileContent(Path.Combine("Sql", "ddl.sql"));
            if (!string.IsNullOrEmpty(ddlScriptContent))
            {
                string[] ddlStatements = ddlScriptContent.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string rawStatement in ddlStatements)
                {
                    string statement = rawStatement.Trim();
                    if (!string.IsNullOrWhiteSpace(statement))
                    {
                        if (!statement.EndsWith(";")) statement += ";"; 
                        if (statement.ToUpperInvariant().StartsWith("PRAGMA")) continue; 

                        dbCommand.CommandText = statement;
                        Debug.Log($"[DBManager] Ejecutando DDL: {dbCommand.CommandText}");
                        dbCommand.ExecuteNonQuery();
                    }
                }
                Debug.Log("[DBManager] Scripts DDL ejecutados.");
            }
            else
            {
                Debug.LogError("[DBManager] El script DDL está vacío o no se pudo leer.");
            }
        }
    }

    // Puebla la base de datos con datos iniciales ejecutando las sentencias DML desde el archivo especificado "Sql/dml.sql".
    // Solo se ejecuta si la tabla principal (ej. "Nave") no contiene datos.
    private void PopulateDatabaseFromDML()
    {
        if (GetNumberOfRowsInTable("Nave") > 0)
        {
            Debug.Log("[DBManager] La tabla Nave ya contiene datos. La población inicial (DML) será omitida.");
            return;
        }

        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            string dmlScriptContent = ReadSqlFileContent(Path.Combine("Sql", "dml.sql"));
            if(!string.IsNullOrEmpty(dmlScriptContent))
            {
                string[] dmlStatements = dmlScriptContent.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Log($"[DBManager] Ejecutando DML script. Número de sentencias: {dmlStatements.Length}");
                foreach (string rawStatement in dmlStatements)
                {
                    string statement = rawStatement.Trim();
                     if (!string.IsNullOrWhiteSpace(statement))
                    {
                        if (!statement.EndsWith(";")) statement += ";";
                        if (statement.ToUpperInvariant().StartsWith("PRAGMA")) continue;

                        dbCommand.CommandText = statement;
                        Debug.Log($"[DBManager] Ejecutando DML: {dbCommand.CommandText}");
                        dbCommand.ExecuteNonQuery();
                    }
                }
                Debug.Log("[DBManager] Scripts DML ejecutados.");
            }
            else
            {
                 Debug.LogError("[DBManager] El script DML está vacío o no se pudo leer.");
            }
        }
    }

    // Lee el contenido de un archivo SQL ubicado en la carpeta Assets del proyecto.
    private string ReadSqlFileContent(string relativePathInAssets)
    {
        string fullPath = Path.Combine(Application.dataPath, relativePathInAssets);
        Debug.Log($"[DBManager] Intentando leer archivo SQL desde: {fullPath}");
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"[DBManager] Archivo SQL no encontrado en la ruta: {fullPath}");
            return string.Empty;
        }
        try { return File.ReadAllText(fullPath); }
        catch (Exception e) { Debug.LogError($"[DBManager] Error al leer el archivo {fullPath}: {e.Message}"); return string.Empty; }
    }

    // Obtiene el número de filas en una tabla especificada.
    public int GetNumberOfRowsInTable(string tableName)
    {
        int count = 0;
        bool connectionWasOpenedByThisMethod = EnsureConnectionIsOpen(); 
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = $"SELECT COUNT(*) FROM [{tableName}]"; 
            try
            {
                object result = dbCommand.ExecuteScalar(); 
                if (result != null && result != DBNull.Value) count = Convert.ToInt32(result);
            }
            catch (Exception e) { Debug.LogError($"[DBManager] Error en GetNumberOfRowsInTable para la tabla [{tableName}]: {e.Message}"); }
        }
        if (connectionWasOpenedByThisMethod) EnsureConnectionIsClosed(); 
        return count;
    }
    
    // Asegura que la conexión a la base de datos esté abierta.
    private bool EnsureConnectionIsOpen()
    {
        if (dbConnection == null) 
        {
            string persistentFilePath = Path.Combine(Application.persistentDataPath, DB_FILE_NAME);
            string connectionString = DB_BASE_URI_PREFIX + persistentFilePath;
            dbConnection = new SqliteConnection(connectionString);
            Debug.LogWarning("[DBManager] La instancia de dbConnection era null. Recreada.");
        }
        if (dbConnection.State != ConnectionState.Open)
        {
            dbConnection.Open();
            return true; 
        }
        return false; 
    }

    // Cierra la conexión a la base de datos si está actualmente abierta.
    private void EnsureConnectionIsClosed()
    {
        if (dbConnection != null && dbConnection.State == ConnectionState.Open) dbConnection.Close();
    }

    // Se llama cuando el MonoBehaviour es destruido.
    void OnDestroy()
    {
        EnsureConnectionIsClosed();
        if (dbConnection != null) { dbConnection.Dispose(); dbConnection = null; }
        Debug.Log("[DBManager] Recursos de la base de datos gestionados en OnDestroy.");
    }

    // Se llama cuando la aplicación se está cerrando.
    void OnApplicationQuit()
    {
        EnsureConnectionIsClosed();
        if (dbConnection != null) { dbConnection.Dispose(); dbConnection = null; }
        Debug.Log("[DBManager] Recursos de la base de datos gestionados en OnApplicationQuit.");
    }
    #endregion

    // ------------------------------------------------------------------------------------
    // SECCIÓN DE MÉTODOS DE ACCESO A DATOS (CRUD) POR ENTIDAD
    // ------------------------------------------------------------------------------------

    #region Métodos para la Entidad: Nave
    public Nave GetNaveById(int naveId)
    {
        Nave nave = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Nave, Estado FROM Nave WHERE ID_Nave = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", naveId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read()) 
                    {
                        nave = new Nave {
                            ID_Nave = reader.GetInt32(reader.GetOrdinal("ID_Nave")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetNaveById({naveId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return nave;
    }
    #endregion

    #region Métodos para la Entidad: Nivel
    public Nivel GetNivelById(int nivelId)
    {
        Nivel nivel = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Nivel, Nombre, Descripcion, ExperienciaRequerida, Estado FROM Nivel WHERE ID_Nivel = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", nivelId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nivel = new Nivel {
                            ID_Nivel = reader.GetInt32(reader.GetOrdinal("ID_Nivel")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                            ExperienciaRequerida = reader.GetInt32(reader.GetOrdinal("ExperienciaRequerida")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetNivelById({nivelId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return nivel;
    }

    public List<Nivel> GetAllNiveles()
    {
        List<Nivel> niveles = new List<Nivel>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Nivel, Nombre, Descripcion, ExperienciaRequerida, Estado FROM Nivel ORDER BY ID_Nivel;";
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        niveles.Add(new Nivel {
                            ID_Nivel = reader.GetInt32(reader.GetOrdinal("ID_Nivel")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                            ExperienciaRequerida = reader.GetInt32(reader.GetOrdinal("ExperienciaRequerida")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado"))
                        });
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetAllNiveles: {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return niveles;
    }
    #endregion

    #region Métodos para la Entidad: Recompensa
    public Recompensa GetRecompensaById(int recompensaId)
    {
        Recompensa recompensa = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Recompensa, Tipo, Contenido, CantidadExperiencia, ID_EventoDesbloqueado FROM Recompensa WHERE ID_Recompensa = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", recompensaId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        recompensa = new Recompensa {
                            ID_Recompensa = reader.GetInt32(reader.GetOrdinal("ID_Recompensa")),
                            Tipo = reader.GetString(reader.GetOrdinal("Tipo")),
                            Contenido = reader.IsDBNull(reader.GetOrdinal("Contenido")) ? null : reader.GetString(reader.GetOrdinal("Contenido")),
                            CantidadExperiencia = reader.IsDBNull(reader.GetOrdinal("CantidadExperiencia")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CantidadExperiencia")),
                            ID_EventoDesbloqueado = reader.IsDBNull(reader.GetOrdinal("ID_EventoDesbloqueado")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_EventoDesbloqueado"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetRecompensaById({recompensaId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return recompensa;
    }

    public List<Recompensa> GetAllRecompensas()
    {
        List<Recompensa> recompensas = new List<Recompensa>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Recompensa, Tipo, Contenido, CantidadExperiencia, ID_EventoDesbloqueado FROM Recompensa ORDER BY ID_Recompensa;";
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recompensas.Add(new Recompensa {
                            ID_Recompensa = reader.GetInt32(reader.GetOrdinal("ID_Recompensa")),
                            Tipo = reader.GetString(reader.GetOrdinal("Tipo")),
                            Contenido = reader.IsDBNull(reader.GetOrdinal("Contenido")) ? null : reader.GetString(reader.GetOrdinal("Contenido")),
                            CantidadExperiencia = reader.IsDBNull(reader.GetOrdinal("CantidadExperiencia")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CantidadExperiencia")),
                            ID_EventoDesbloqueado = reader.IsDBNull(reader.GetOrdinal("ID_EventoDesbloqueado")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_EventoDesbloqueado"))
                        });
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetAllRecompensas: {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return recompensas;
    }
    #endregion

    #region Métodos para la Entidad: Mision
    public Mision GetMisionById(int misionId)
    {
        Mision mision = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Mision, Nombre, Descripcion, Duracion, ID_Recompensa, Estado FROM Mision WHERE ID_Mision = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", misionId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mision = new Mision {
                            ID_Mision = reader.GetInt32(reader.GetOrdinal("ID_Mision")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                            Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                            ID_Recompensa = reader.GetInt32(reader.GetOrdinal("ID_Recompensa")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetMisionById({misionId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return mision;
    }

    public List<Mision> GetAllMisiones()
    {
        List<Mision> misiones = new List<Mision>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Mision, Nombre, Descripcion, Duracion, ID_Recompensa, Estado FROM Mision ORDER BY ID_Mision;";
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        misiones.Add(new Mision {
                            ID_Mision = reader.GetInt32(reader.GetOrdinal("ID_Mision")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                            Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                            ID_Recompensa = reader.GetInt32(reader.GetOrdinal("ID_Recompensa")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado"))
                        });
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetAllMisiones: {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return misiones;
    }
    #endregion

    #region Métodos para la Entidad: Evento
    public Evento GetEventoById(int eventoId)
    {
        Evento evento = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Evento, Tipo, Descripcion, TiempoLimite, SuministroAfectado, Impacto, ID_Mision FROM Evento WHERE ID_Evento = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", eventoId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        evento = new Evento {
                            ID_Evento = reader.GetInt32(reader.GetOrdinal("ID_Evento")),
                            Tipo = reader.GetString(reader.GetOrdinal("Tipo")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                            TiempoLimite = reader.IsDBNull(reader.GetOrdinal("TiempoLimite")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TiempoLimite")),
                            SuministroAfectado = reader.IsDBNull(reader.GetOrdinal("SuministroAfectado")) ? null : reader.GetString(reader.GetOrdinal("SuministroAfectado")),
                            Impacto = reader.IsDBNull(reader.GetOrdinal("Impacto")) ? null : reader.GetString(reader.GetOrdinal("Impacto")),
                            ID_Mision = reader.IsDBNull(reader.GetOrdinal("ID_Mision")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_Mision"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetEventoById({eventoId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return evento;
    }

    public List<Evento> GetAllEventos()
    {
        List<Evento> eventos = new List<Evento>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Evento, Tipo, Descripcion, TiempoLimite, SuministroAfectado, Impacto, ID_Mision FROM Evento ORDER BY ID_Evento;";
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        eventos.Add(new Evento {
                            ID_Evento = reader.GetInt32(reader.GetOrdinal("ID_Evento")),
                            Tipo = reader.GetString(reader.GetOrdinal("Tipo")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                            TiempoLimite = reader.IsDBNull(reader.GetOrdinal("TiempoLimite")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TiempoLimite")),
                            SuministroAfectado = reader.IsDBNull(reader.GetOrdinal("SuministroAfectado")) ? null : reader.GetString(reader.GetOrdinal("SuministroAfectado")),
                            Impacto = reader.IsDBNull(reader.GetOrdinal("Impacto")) ? null : reader.GetString(reader.GetOrdinal("Impacto")),
                            ID_Mision = reader.IsDBNull(reader.GetOrdinal("ID_Mision")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_Mision"))
                        });
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetAllEventos: {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return eventos;
    }
    #endregion

    #region Métodos para la Entidad: SuministroNave
    public SuministroNave GetSuministroNaveById(int suministroNaveId)
    {
        SuministroNave suministro = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_SuministroNave, ID_Nave, TipoSuministro, Cantidad FROM SuministroNave WHERE ID_SuministroNave = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", suministroNaveId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        suministro = new SuministroNave {
                            ID_SuministroNave = reader.GetInt32(reader.GetOrdinal("ID_SuministroNave")),
                            ID_Nave = reader.GetInt32(reader.GetOrdinal("ID_Nave")),
                            TipoSuministro = reader.GetString(reader.GetOrdinal("TipoSuministro")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetSuministroNaveById({suministroNaveId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return suministro;
    }

    public List<SuministroNave> GetSuministrosDeNave(int naveId) 
    {
        List<SuministroNave> suministros = new List<SuministroNave>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_SuministroNave, ID_Nave, TipoSuministro, Cantidad FROM SuministroNave WHERE ID_Nave = @naveId ORDER BY TipoSuministro;";
            DatabaseUtils.AddParameter(dbCommand, "@naveId", naveId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        suministros.Add(new SuministroNave {
                            ID_SuministroNave = reader.GetInt32(reader.GetOrdinal("ID_SuministroNave")),
                            ID_Nave = reader.GetInt32(reader.GetOrdinal("ID_Nave")),
                            TipoSuministro = reader.GetString(reader.GetOrdinal("TipoSuministro")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad"))
                        });
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetSuministrosDeNave({naveId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return suministros;
    }
    #endregion

    #region Métodos para la Entidad: Modulo
    public Modulo GetModuloById(int moduloId)
    {
        Modulo modulo = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Modulo, Nombre, Estado, ID_Nave FROM Modulo WHERE ID_Modulo = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", moduloId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        modulo = new Modulo {
                            ID_Modulo = reader.GetInt32(reader.GetOrdinal("ID_Modulo")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            ID_Nave = reader.GetInt32(reader.GetOrdinal("ID_Nave"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetModuloById({moduloId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return modulo;
    }

    public List<Modulo> GetModulosDeNave(int naveId)
    {
        List<Modulo> modulos = new List<Modulo>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Modulo, Nombre, Estado, ID_Nave FROM Modulo WHERE ID_Nave = @naveId ORDER BY Nombre;";
            DatabaseUtils.AddParameter(dbCommand, "@naveId", naveId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        modulos.Add(new Modulo {
                            ID_Modulo = reader.GetInt32(reader.GetOrdinal("ID_Modulo")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            ID_Nave = reader.GetInt32(reader.GetOrdinal("ID_Nave"))
                        });
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetModulosDeNave({naveId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return modulos;
    }
    #endregion

    #region Métodos para la Entidad: Tarea
    public Tarea GetTareaById(int tareaId)
    {
        Tarea tarea = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Tarea, Descripcion, Duracion, Estado, ID_Modulo, ID_Evento, ID_Mision, NivelNecesario, ID_Recompensa FROM Tarea WHERE ID_Tarea = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", tareaId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tarea = new Tarea {
                            ID_Tarea = reader.GetInt32(reader.GetOrdinal("ID_Tarea")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                            Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            ID_Modulo = reader.GetInt32(reader.GetOrdinal("ID_Modulo")),
                            ID_Evento = reader.IsDBNull(reader.GetOrdinal("ID_Evento")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_Evento")),
                            ID_Mision = reader.IsDBNull(reader.GetOrdinal("ID_Mision")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_Mision")),
                            NivelNecesario = reader.IsDBNull(reader.GetOrdinal("NivelNecesario")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("NivelNecesario")),
                            ID_Recompensa = reader.IsDBNull(reader.GetOrdinal("ID_Recompensa")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_Recompensa"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetTareaById({tareaId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return tarea;
    }

    public List<Tarea> GetAllTareas() 
    {
        List<Tarea> tareas = new List<Tarea>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Tarea, Descripcion, Duracion, Estado, ID_Modulo, ID_Evento, ID_Mision, NivelNecesario, ID_Recompensa FROM Tarea ORDER BY ID_Tarea;";
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tareas.Add(new Tarea {
                            ID_Tarea = reader.GetInt32(reader.GetOrdinal("ID_Tarea")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                            Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            ID_Modulo = reader.GetInt32(reader.GetOrdinal("ID_Modulo")),
                            ID_Evento = reader.IsDBNull(reader.GetOrdinal("ID_Evento")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_Evento")),
                            ID_Mision = reader.IsDBNull(reader.GetOrdinal("ID_Mision")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_Mision")),
                            NivelNecesario = reader.IsDBNull(reader.GetOrdinal("NivelNecesario")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("NivelNecesario")),
                            ID_Recompensa = reader.IsDBNull(reader.GetOrdinal("ID_Recompensa")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_Recompensa"))
                        });
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetAllTareas: {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return tareas;
    }
    #endregion

    #region Métodos para la Entidad: Tripulante
    public void InsertTripulante(Tripulante nuevoTripulante)
    {
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "INSERT INTO Tripulante (Nombre, Salud, Hambre, ID_TareaActual, ExperienciaActual) " +
                                    "VALUES (@Nombre, @Salud, @Hambre, @ID_TareaActual, @ExperienciaActual);";
            DatabaseUtils.AddParameter(dbCommand, "@Nombre", nuevoTripulante.Nombre);
            DatabaseUtils.AddParameter(dbCommand, "@Salud", nuevoTripulante.Salud);
            DatabaseUtils.AddParameter(dbCommand, "@Hambre", nuevoTripulante.Hambre);
            DatabaseUtils.AddParameter(dbCommand, "@ID_TareaActual", nuevoTripulante.ID_TareaActual); 
            DatabaseUtils.AddParameter(dbCommand, "@ExperienciaActual", nuevoTripulante.ExperienciaActual);
            Debug.Log($"[DBManager] Ejecutando INSERT para Tripulante: {nuevoTripulante.Nombre}");
            try { dbCommand.ExecuteNonQuery(); } 
            catch (Exception e) { Debug.LogError($"[DBManager] Error insertando Tripulante ({nuevoTripulante.Nombre}): {e.Message}");}
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
    }

    public Tripulante GetTripulanteById(int tripulanteId)
    {
        Tripulante tripulante = null;
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Tripulante, Nombre, Salud, Hambre, ID_TareaActual, ExperienciaActual FROM Tripulante WHERE ID_Tripulante = @id;";
            DatabaseUtils.AddParameter(dbCommand, "@id", tripulanteId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tripulante = new Tripulante {
                            ID_Tripulante = reader.GetInt32(reader.GetOrdinal("ID_Tripulante")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            Salud = reader.GetInt32(reader.GetOrdinal("Salud")),
                            Hambre = reader.GetInt32(reader.GetOrdinal("Hambre")),
                            ID_TareaActual = reader.IsDBNull(reader.GetOrdinal("ID_TareaActual")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_TareaActual")),
                            ExperienciaActual = reader.GetInt32(reader.GetOrdinal("ExperienciaActual"))
                        };
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetTripulanteById({tripulanteId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return tripulante;
    }

    public List<Tripulante> GetAllTripulantes()
    {
        List<Tripulante> tripulantes = new List<Tripulante>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Tripulante, Nombre, Salud, Hambre, ID_TareaActual, ExperienciaActual FROM Tripulante ORDER BY ID_Tripulante;";
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tripulantes.Add(new Tripulante {
                            ID_Tripulante = reader.GetInt32(reader.GetOrdinal("ID_Tripulante")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                            Salud = reader.GetInt32(reader.GetOrdinal("Salud")),
                            Hambre = reader.GetInt32(reader.GetOrdinal("Hambre")),
                            ID_TareaActual = reader.IsDBNull(reader.GetOrdinal("ID_TareaActual")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_TareaActual")),
                            ExperienciaActual = reader.GetInt32(reader.GetOrdinal("ExperienciaActual"))
                        });
                    }
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetAllTripulantes: {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return tripulantes;
    }
    #endregion

    #region Métodos para la Tabla de Unión: TripulanteMision
    public void AsignarTripulanteAMision(int tripulanteId, int misionId)
    {
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "INSERT INTO Tripulante_Mision (ID_Tripulante, ID_Mision) VALUES (@tripulanteId, @misionId);";
            DatabaseUtils.AddParameter(dbCommand, "@tripulanteId", tripulanteId);
            DatabaseUtils.AddParameter(dbCommand, "@misionId", misionId);
            try
            {
                dbCommand.ExecuteNonQuery();
                Debug.Log($"[DBManager] Tripulante {tripulanteId} asignado a Misión {misionId}.");
            }
            catch (SqliteException ex) when (ex.ErrorCode == 19) 
            {
                Debug.LogWarning($"[DBManager] El tripulante {tripulanteId} ya está asignado a la misión {misionId} o se violó otra constraint.");
            }
            catch (Exception e) { Debug.LogError($"[DBManager] Error asignando tripulante {tripulanteId} a misión {misionId}: {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
    }

    public void DesasignarTripulanteDeMision(int tripulanteId, int misionId)
    {
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "DELETE FROM Tripulante_Mision WHERE ID_Tripulante = @tripulanteId AND ID_Mision = @misionId;";
            DatabaseUtils.AddParameter(dbCommand, "@tripulanteId", tripulanteId);
            DatabaseUtils.AddParameter(dbCommand, "@misionId", misionId);
            try
            {
                int rowsAffected = dbCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Debug.Log($"[DBManager] Tripulante {tripulanteId} desasignado de Misión {misionId}.");
                else
                    Debug.LogWarning($"[DBManager] No se encontró asignación para Tripulante {tripulanteId} y Misión {misionId} para desasignar.");
            } catch (Exception e) { Debug.LogError($"[DBManager] Error desasignando tripulante {tripulanteId} de misión {misionId}: {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
    }

    public List<int> GetMisionIdsPorTripulante(int tripulanteId)
    {
        List<int> misionIds = new List<int>();
        bool connectionWasOpened = EnsureConnectionIsOpen();
        using (IDbCommand dbCommand = dbConnection.CreateCommand())
        {
            dbCommand.CommandText = "SELECT ID_Mision FROM Tripulante_Mision WHERE ID_Tripulante = @tripulanteId;";
            DatabaseUtils.AddParameter(dbCommand, "@tripulanteId", tripulanteId);
            try
            {
                using (IDataReader reader = dbCommand.ExecuteReader())
                {
                    while(reader.Read()) { misionIds.Add(reader.GetInt32(0)); } 
                }
            } catch (Exception e) { Debug.LogError($"[DBManager] Error en GetMisionIdsPorTripulante({tripulanteId}): {e.Message}"); }
        }
        if (connectionWasOpened) EnsureConnectionIsClosed();
        return misionIds;
    }
    
    #endregion

    #region Métodos de Prueba Internos
    // Ejecuta una serie de pruebas predefinidas contra la base de datos.
    private void RunDatabaseTests()
    {
        Debug.Log("[DBManager] --- INICIO DE PRUEBAS DE BASE DE DATOS ---");
        Test_Nave_GetById();
        Test_Nivel_GetAll();
        Test_Tripulante_GetByIdAndInsert();
        Test_Mision_GetAll(); 
        Test_AsignacionTripulanteMision();
        // Puedes añadir más pruebas aquí
        // Test_Recompensa_GetAll();
        // Test_Evento_GetAll();
        // Test_Suministro_GetDeNave();
        // Test_Modulo_GetDeNave();
        // Test_Tarea_GetAll();
        Debug.Log("[DBManager] --- FIN DE PRUEBAS DE BASE DE DATOS ---");
    }

    private void Test_Nave_GetById()
    {
        Nave nave = GetNaveById(1); 
        if (nave != null) Debug.Log($"[DBManager-Test] Nave Recuperada: {nave.ToString()}");
        else Debug.LogWarning("[DBManager-Test] No se pudo recuperar la Nave con ID 1.");
    }
     private void Test_Nivel_GetAll()
    {
        List<Nivel> niveles = GetAllNiveles();
        Debug.Log($"[DBManager-Test] Total de Niveles recuperados: {niveles.Count}");
        foreach(var nivel in niveles) Debug.Log($"[DBManager-Test] Nivel: {nivel.ToString()}");
    }
     private void Test_Tripulante_GetByIdAndInsert()
    {
        Tripulante trip1 = GetTripulanteById(1); 
        if (trip1 != null) Debug.Log($"[DBManager-Test] Tripulante Recuperado (ID 1): {trip1.ToString()}");
        
        Tripulante nuevo = new Tripulante { Nombre = "Nuevo Recluta", Salud = 90, Hambre = 10, ExperienciaActual = 0 };
        InsertTripulante(nuevo);
        Debug.Log($"[DBManager-Test] Intento de inserción de Tripulante: {nuevo.Nombre}");
    }
    private void Test_Mision_GetAll() 
    {
        List<Mision> misiones = GetAllMisiones(); 
        Debug.Log($"[DBManager-Test] Total de Misiones recuperadas: {misiones.Count}");
        foreach(var mision in misiones) Debug.Log($"[DBManager-Test] Misión: {mision.ToString()}");
    }
    private void Test_AsignacionTripulanteMision()
    {
        Debug.Log("[DBManager-Test] Intentando asignar Tripulante 2 a Misión 2...");
        AsignarTripulanteAMision(2, 2); 
        List<int> misionesTrip2 = GetMisionIdsPorTripulante(2);
        Debug.Log($"[DBManager-Test] Misiones para Tripulante 2: {string.Join(", ", misionesTrip2)}");
    }
    #endregion
}

// Clase de utilidad estática para operaciones comunes de base de datos, como añadir parámetros.
public static class DatabaseUtils
{
    // Añade un parámetro a un objeto IDbCommand.
    public static void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value; 
        command.Parameters.Add(parameter);
    }
}
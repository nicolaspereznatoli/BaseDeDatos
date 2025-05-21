using UnityEngine;
using TMPro;

public class SimpleMissionManagerTMP : MonoBehaviour
{
    public TextMeshProUGUI missionText;
    public TextMeshProUGUI mensajeMision;
    public Mision misionActual;
    public int experiencia = 0;
    private NuevoDBManager dbManager;

    public class Mision
    {
        public int id;
        public string nombre;
        public string descripcion;
        public float duracion;
        public int id_recompensa;
        public string estado;

        public Mision(int id, string nombre, string descripcion, float duracion, int recompensa)
        {
            this.id = id;
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.duracion = duracion;
            this.id_recompensa = recompensa;
            this.estado = "pendiente";
        }
    }

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
        mensajeMision.text = "";
        experiencia = 0;
        LanzarMision1();
    }

    void LanzarMision1()
    {
        misionActual = new Mision(1, "Plantar semilla", "🌱 Planta una semilla para conseguir comida.", 0f, 10);
        missionText.text = misionActual.descripcion;
        misionActual.estado = "activa";

        Debug.Log("Misión 1 lanzada: " + misionActual.nombre);
        if (dbManager != null)
            dbManager.InsertarMision(misionActual.nombre, misionActual.descripcion, 0, misionActual.id_recompensa, misionActual.estado);
    }

    void LanzarMision2()
    {
        misionActual = new Mision(2, "Recoger agua", "💧 Recoge agua para regar la planta.", 0f, 15);
        missionText.text = misionActual.descripcion;
        misionActual.estado = "activa";

        Debug.Log("Misión 2 lanzada: " + misionActual.nombre);
        if (dbManager != null)
            dbManager.InsertarMision(misionActual.nombre, misionActual.descripcion, 0, misionActual.id_recompensa, misionActual.estado);
    }

    void LanzarMision3()
    {
        misionActual = new Mision(3, "Ver Inventario", "⛏ Pulsa I para ver el inventario.", 0f, 5);
        missionText.text = misionActual.descripcion;
        misionActual.estado = "activa";

        Debug.Log("Misión 3 lanzada: " + misionActual.nombre);
        if (dbManager != null)
            dbManager.InsertarMision(misionActual.nombre, misionActual.descripcion, 0, misionActual.id_recompensa, misionActual.estado);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Tecla I pulsada.");
            Ikeyed();
        }
    }

    public void OnSeedPlanted()
    {
        Debug.Log("OnSeedPlanted llamado.");
        if (misionActual != null && misionActual.estado == "activa" && misionActual.id == 1)
        {
            Debug.Log("Misión 1 completada.");
            CompletarMision();
        }
        else
        {
            Debug.Log("OnSeedPlanted: No es misión 1 activa o misionActual es null.");
        }
    }

    public void OnWaterCollected()
    {
        Debug.Log("OnWaterCollected llamado.");
        if (misionActual != null && misionActual.estado == "activa" && misionActual.id == 2)
        {
            Debug.Log("Misión 2 completada.");
            CompletarMision();
        }
        else
        {
            Debug.Log("OnWaterCollected: No es misión 2 activa o misionActual es null.");
        }
    }

    public void Ikeyed()
    {
        Debug.Log("Ikeyed llamado.");
        if (misionActual != null && misionActual.estado == "activa" && misionActual.id == 3)
        {
            Debug.Log("Misión 3 completada.");
            CompletarMision();
        }
        else
        {
            Debug.Log("Ikeyed: No es misión 3 activa o misionActual es null.");
        }
    }

    void CompletarMision()
    {
        misionActual.estado = "completada";
        experiencia += misionActual.id_recompensa;
        missionText.text = "";
        mensajeMision.text = "✅ Misión completada: " + misionActual.nombre + " +" + misionActual.id_recompensa + " XP";
        Debug.Log(mensajeMision.text);

        Invoke("LimpiarMensajeMision", 4f);

        // Lanzar siguiente misión
        if (misionActual.id == 1)
            LanzarMision2();
        else if (misionActual.id == 2)
            LanzarMision3();
        else if (misionActual.id == 3)
            Debug.Log("Todas las misiones completadas. Experiencia total: " + experiencia);
    }

    void LimpiarMensajeMision()
    {
        mensajeMision.text = "";
        Debug.Log("Mensaje de misión limpiado.");
    }
}

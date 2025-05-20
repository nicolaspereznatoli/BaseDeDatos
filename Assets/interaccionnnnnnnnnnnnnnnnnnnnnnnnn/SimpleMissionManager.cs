using UnityEngine;
using TMPro;

public class SimpleMissionManagerTMP : MonoBehaviour
{
    public TextMeshProUGUI missionText;
    public TextMeshProUGUI mensajeMision; // Nuevo texto para mostrar los mensajes de misión completada
    public Mision misionActual;
    public int experiencia = 0;

    public class Mision
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public float duracion { get; set; }
        public int id_recompensa { get; set; }
        public string estado { get; set; }

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

    void Start()
    {
        mensajeMision.text = ""; // Vaciar mensaje al inicio
        Invoke("LanzarMision1", 1f);
        Invoke("LanzarMision2", 5f);
        Invoke("LanzarMision3", 10f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Ikeyed();
        }
    }

    void LanzarMision1()
    {
        misionActual = new Mision(
            id: 1,
            nombre: "Plantar semilla",
            descripcion: "🌱 Planta una semilla para conseguir comida.",
            duracion: 0f,
            recompensa: 10
        );

        missionText.text = misionActual.descripcion;
        misionActual.estado = "activa";
    }

    void LanzarMision2()
    {
        misionActual = new Mision(
            id: 2,
            nombre: "Recoger agua",
            descripcion: "💧 Recoge agua para regar la planta.",
            duracion: 0f,
            recompensa: 15
        );

        missionText.text = misionActual.descripcion;
        misionActual.estado = "activa";
    }

    void LanzarMision3()
    {
        misionActual = new Mision(
            id: 3,
            nombre: "Ver Inventario",
            descripcion: "⛏ Pulsa I para ver el inventario.",
            duracion: 0f,
            recompensa: 5
        );

        missionText.text = misionActual.descripcion;
        misionActual.estado = "activa";
    }

    public void OnSeedPlanted()
    {
        if (misionActual != null && misionActual.estado == "activa" && misionActual.id == 1)
        {
            CompletarMision();
        }
    }

    public void OnWaterCollected()
    {
        if (misionActual != null && misionActual.estado == "activa" && misionActual.id == 2)
        {
            CompletarMision();
        }
    }

    public void Ikeyed()
    {
        if (misionActual != null && misionActual.estado == "activa" && misionActual.id == 3)
        {
            CompletarMision();
        }
    }

    void CompletarMision()
    {
        misionActual.estado = "completada";
        experiencia += misionActual.id_recompensa;
        missionText.text = "";
        mensajeMision.text = "✅ Misión completada: " + misionActual.nombre + " +" + misionActual.id_recompensa + " XP";
        Invoke("LimpiarMensajeMision", 4f); // Quita el mensaje tras 4 segundos
    }

    void LimpiarMensajeMision()
    {
        mensajeMision.text = "";
    }
}

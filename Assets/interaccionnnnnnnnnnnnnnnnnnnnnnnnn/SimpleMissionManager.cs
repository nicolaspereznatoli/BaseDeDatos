using UnityEngine;
using TMPro;

public class SimpleMissionManagerTMP : MonoBehaviour
{
    public TextMeshProUGUI missionText;
    public Mision misionActual;
    public int experiencia = 0;

    // Nueva clase para definir una misión
    public class Mision
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public float duracion { get; set; }  // Añadimos duración
        public int id_recompensa { get; set; }
        public string estado { get; set; }

        public Mision(int id, string nombre, string descripcion, float duracion, int recompensa)
        {
            this.id = id;
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.duracion = duracion;
            this.id_recompensa = recompensa;
            this.estado = "pendiente"; // Estado inicial
        }
    }

    void Start()
    {
        // Lanza la primera misión después de 5 segundos
        Invoke("LanzarMision1", 1f);
        // Lanza la segunda misión después de 12 segundos
        Invoke("LanzarMision2", 3f);
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

    // Llamar esta función desde otro script cuando el jugador planta la semilla
    public void OnSeedPlanted()
    {
        if (misionActual != null && misionActual.estado == "activa" && misionActual.id == 1)
        {
            misionActual.estado = "completada";
            experiencia += misionActual.id_recompensa;
            missionText.text = "";
            Debug.Log("✅ Misión completada: " + misionActual.nombre + ". +" + misionActual.id_recompensa + " XP");
        }
    }

    public void OnWaterCollected()
    {
        if (misionActual != null && misionActual.estado == "activa" && misionActual.id == 2)
        {
            misionActual.estado = "completada";
            experiencia += misionActual.id_recompensa;
            missionText.text = "";
            Debug.Log("✅ Misión completada: " + misionActual.nombre + ". +" + misionActual.id_recompensa + " XP");
        }
    }
}

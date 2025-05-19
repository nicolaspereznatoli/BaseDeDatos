[System.Serializable]
public class Mision
{
    public int id_mision;
    public string nombre;
    public string descripcion;
    public float duracion;
    public int id_recompensa;
    public string estado; // "activa", "completada", "fallida", etc.

    public Mision(int id, string nombre, string descripcion, float duracion, int recompensa)
    {
        this.id_mision = id;
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.duracion = duracion;
        this.id_recompensa = recompensa;
        this.estado = "activa";
    }
}

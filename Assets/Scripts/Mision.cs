public class Mision
{
    public int ID_Mision { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Duracion { get; set; }
    public int ID_Recompensa { get; set; } // FK
    public string Estado { get; set; } // 'pendiente', 'activa', 'completada'

    public override string ToString() => $"Mision [ID: {ID_Mision}, Nombre: {Nombre}, Estado: {Estado}]";
}
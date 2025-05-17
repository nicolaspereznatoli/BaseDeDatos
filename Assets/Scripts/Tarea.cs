public class Tarea
{
    public int ID_Tarea { get; set; }
    public string Descripcion { get; set; }
    public int Duracion { get; set; }
    public string Estado { get; set; } // 'pendiente', 'en curso', 'completada'
    public int ID_Modulo { get; set; } // FK
    public int? ID_Evento { get; set; } // FK, Nullable
    public int? ID_Mision { get; set; } // FK, Nullable
    public int? NivelNecesario { get; set; } // FK a Nivel.ID_Nivel, Nullable
    public int? ID_Recompensa { get; set; } // FK, Nullable

    public override string ToString() => $"Tarea [ID: {ID_Tarea}, Desc: {Descripcion}, Estado: {Estado}]";
}
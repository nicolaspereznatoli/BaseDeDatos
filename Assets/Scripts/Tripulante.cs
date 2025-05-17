public class Tripulante
{
    public int ID_Tripulante { get; set; }
    public string Nombre { get; set; }
    public int Salud { get; set; } // 0-100
    public int Hambre { get; set; } // 0-100
    public int? ID_TareaActual { get; set; } // FK, Nullable
    public int ExperienciaActual { get; set; }

    public override string ToString() => $"Tripulante [ID: {ID_Tripulante}, Nombre: {Nombre}, Salud: {Salud}, Exp: {ExperienciaActual}]";
}
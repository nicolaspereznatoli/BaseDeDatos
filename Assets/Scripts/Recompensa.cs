public class Recompensa
{
    public int ID_Recompensa { get; set; }
    public string Tipo { get; set; } // 'suministro', 'experiencia', 'desbloqueo'
    public string Contenido { get; set; } // Puede ser JSON o texto descriptivo
    public int? CantidadExperiencia { get; set; } // Nullable
    public int? ID_EventoDesbloqueado { get; set; } // Nullable

    public override string ToString() => $"Recompensa [ID: {ID_Recompensa}, Tipo: {Tipo}, Contenido: {Contenido}]";
}
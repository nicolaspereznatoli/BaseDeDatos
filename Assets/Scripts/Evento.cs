public class Evento
{
    public int ID_Evento { get; set; }
    public string Tipo { get; set; } // 'fallo', 'sabotaje', 'etc.'
    public string Descripcion { get; set; }
    public int? TiempoLimite { get; set; } // Nullable
    public string SuministroAfectado { get; set; } // 'oxígeno', 'energía', 'comida', 'chatarra', o NULL
    public string Impacto { get; set; }
    public int? ID_Mision { get; set; } // FK, Nullable

    public override string ToString() => $"Evento [ID: {ID_Evento}, Tipo: {Tipo}, Desc: {Descripcion}]";
}
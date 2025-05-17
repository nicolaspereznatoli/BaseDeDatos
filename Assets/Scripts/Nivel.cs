public class Nivel
{
    public int ID_Nivel { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int ExperienciaRequerida { get; set; }
    public string Estado { get; set; } // 'desbloqueado', 'completado', 'bloqueado'

    public override string ToString() => $"Nivel [ID: {ID_Nivel}, Nombre: {Nombre}, ExpReq: {ExperienciaRequerida}, Estado: {Estado}]";
}
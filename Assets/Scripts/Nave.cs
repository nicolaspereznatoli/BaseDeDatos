public class Nave
{
    public int ID_Nave { get; set; }
    public string Estado { get; set; } // 'funcional', 'averiada'

    public override string ToString() => $"Nave [ID: {ID_Nave}, Estado: {Estado}]";
}
public class Modulo
{
    public int ID_Modulo { get; set; }
    public string Nombre { get; set; }
    public string Estado { get; set; } // 'funcional', 'dañado', 'destruido'
    public int ID_Nave { get; set; } // FK

    public override string ToString() => $"Modulo [ID: {ID_Modulo}, Nombre: {Nombre}, Estado: {Estado}, NaveID: {ID_Nave}]";
}
public class SuministroNave
{
    public int ID_SuministroNave { get; set; }
    public int ID_Nave { get; set; } // FK
    public string TipoSuministro { get; set; } // 'oxígeno', 'energía', 'comida', 'chatarra'
    public int Cantidad { get; set; }

    public override string ToString() => $"SuministroNave [ID: {ID_SuministroNave}, Tipo: {TipoSuministro}, Cantidad: {Cantidad}, NaveID: {ID_Nave}]";
}
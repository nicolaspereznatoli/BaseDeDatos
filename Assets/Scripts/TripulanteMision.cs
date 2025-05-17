public class TripulanteMision 
{
    public int ID_Tripulante_Mision { get; set; } // PK de la tabla de unión
    public int ID_Tripulante { get; set; } // FK
    public int ID_Mision { get; set; }     // FK

    public override string ToString() => $"TripulanteMision [TripulanteID: {ID_Tripulante}, MisionID: {ID_Mision}]";
}
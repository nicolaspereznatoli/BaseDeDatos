using UnityEngine;
using DevionGames.InventorySystem;

public class AddItemToInventory : MonoBehaviour
{
    public ItemContainer playerInventory; // Referencia al inventario del jugador (arrástralo desde el Inspector)

    void Start()
    {
        // 1. Verificar si tenemos una referencia al inventario del jugador
        if (playerInventory == null)
        {
            Debug.LogError("No se ha asignado el inventario del jugador en el Inspector.");
            return; // Importante: Salir si no hay inventario
        }

        // 2. Crear una instancia del objeto Item "Cubo de Agua"
        WaterItem waterItem = ScriptableObject.CreateInstance<WaterItem>();
        waterItem.DisplayName = "Cubo de Agua";  // Nombre que se mostrará en el inventario
        //waterItem.Description = "Un cubo lleno de agua fresca."; // Descripción del objeto
        waterItem.Icon = Resources.Load<Sprite>("Empty_Beer_Jug");  //  MUY IMPORTANTE:  Reemplaza con la ruta a tu icono
        waterItem.Stack = 1; // Cantidad inicial del objeto

        // 3. Añadir el objeto al inventario
        bool success = playerInventory.AddItem(waterItem);

        if (success)
        {
            Debug.Log("Cubo de Agua añadido al inventario.");
        }
        else
        {
            Debug.LogWarning("No se pudo añadir Cubo de Agua al inventario.");
        }
    }
}
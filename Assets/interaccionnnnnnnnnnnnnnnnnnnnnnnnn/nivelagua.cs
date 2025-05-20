using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nivelagua : MonoBehaviour
{

    public Transform cilindro;         // El cilindro que quieres mover
    public float distanciaBajada = 0.1f;  // Cuánto baja cada vez que pulsas R

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (cilindro != null)
            {
                // Baja el cilindro solo en el eje Y una pequeña cantidad
                cilindro.position = new Vector3(
                    cilindro.position.x,
                    cilindro.position.y - distanciaBajada,
                    cilindro.position.z
                );
            }
        }
    }
}

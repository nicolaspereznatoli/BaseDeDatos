using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mundo : MonoBehaviour
{
  // Velocidad de rotación en grados por segundo
    public float rotationSpeed = 10f;

    void Update()
    {
        // Rota el objeto alrededor del eje Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alpha : MonoBehaviour
{
    public float windStrength = 5f;   // Cuánto se inclina la planta
    public float windSpeed = 1f;      // Qué tan rápido oscila
    private float noiseSeed;

    void Start()
    {
        // Semilla aleatoria para que varias plantas no se muevan igual
        noiseSeed = Random.Range(0f, 100f);
    }

    void Update()
    {
        float angle = Mathf.PerlinNoise(Time.time * windSpeed, noiseSeed) * 2f - 1f;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle * windStrength);
    }
}

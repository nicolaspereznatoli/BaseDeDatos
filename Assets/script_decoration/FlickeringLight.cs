using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    private Light _light;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f;

    private void Start()
    {
        _light = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    private System.Collections.IEnumerator Flicker()
    {
        while (true)
        {
            _light.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(Random.Range(flickerSpeed * 0.5f, flickerSpeed * 1.5f));
        }
    }
}

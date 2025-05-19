using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para usar RawImage

public class SimulateSleep : MonoBehaviour
{
    public Camera mainCamera;
    public Camera bedCamera;
    public float fadeDuration = 1f;
    public Image fadeImage;
    public Transform bedTransform; // Asigna aquí la Transform del objeto cama
    public float proximityDistance = 2f; // Distancia máxima para considerar que está cerca de la cama
    private bool isFading = false;
    private Color fadeColor = Color.black;
    private bool isNearBed = false;
    public Transform playerTransform; // Asigna aquí la Transform del jugador

    void Update()
    {
        // Comprobar la distancia al objeto cama
        if (bedTransform != null && playerTransform != null)
        {
            float distanceToBed = Vector3.Distance(playerTransform.position, bedTransform.position);
            isNearBed = distanceToBed <= proximityDistance;
        }
        else
        {
            Debug.LogWarning("Bed Transform or Player Transform not assigned!");
            isNearBed = false; // Si no están asignados, no se considera cerca
        }

        if (isNearBed && (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q) || Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Q)) && !isFading)
        {
            StartCoroutine(GoToSleep());
        }
    }

    IEnumerator GoToSleep()
    {
        isFading = true;

        // Switch camera
        if (mainCamera != null && bedCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            bedCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Main Camera or Bed Camera not assigned in the Inspector!");
            isFading = false;
            yield break;
        }

        // Fade out
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }
        SetFadeAlpha(1f); // Ensure fully black

        // Wait
        yield return new WaitForSeconds(3f);

        // Fade in
        time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }
        SetFadeAlpha(0f); // Ensure fully transparent

        // Switch back camera
        if (mainCamera != null && bedCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            bedCamera.gameObject.SetActive(false);
        }

        isFading = false;
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color currentColor = fadeImage.color;
            currentColor.a = alpha;
            fadeImage.color = currentColor;
        }
        else
        {
            Debug.LogWarning("Fade Image not assigned!");
        }
    }
}

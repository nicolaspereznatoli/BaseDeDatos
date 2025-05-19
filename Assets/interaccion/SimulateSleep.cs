using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para usar RawImage
using TMPro;

public class SimulateSleep : MonoBehaviour
{

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera bedCamera;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public Image fadeImage;

    [Header("Bed Proximity")]
    public Transform bedTransform;
    public float proximityDistance = 2f;
    public Transform playerTransform;

    [Header("Sleep Cooldown Message")]
    public TMP_Text sleepMessageText; // Para TextMeshPro
    public float messageDisplayTime = 2f;

    [Header("Energy Restoration")]
    public CharacterEnergy energyScript; // Asigna el script CharacterEnergy aquí
    public bool hasSlept { get; set; } // Nuevo flag para indicar que se ha dormido

    public bool isFading = false;
    public bool isFadingPublic => isFading; // Propiedad pública de solo lectura para isFading
    private Color fadeColor = Color.black;
    private bool isNearBed = false;
    private bool canSleepAgain = true;
    private float sleepCooldown = 10f;

    void Start()
    {
        // Ocultar el mensaje al inicio
        if (sleepMessageText != null)
        {
            sleepMessageText.gameObject.SetActive(false);
        }
        hasSlept = false;
        if (energyScript == null)
        {
            Debug.LogError("¡El script CharacterEnergy no está asignado a SimulateSleep!");
        }
    }

    void Update()
    {
        CheckProximityToBed();
        HandleSleepInput();
    }

    void CheckProximityToBed()
    {
        if (bedTransform != null && playerTransform != null)
        {
            float distanceToBed = Vector3.Distance(playerTransform.position, bedTransform.position);
            isNearBed = distanceToBed <= proximityDistance;
        }
        else
        {
            Debug.LogWarning("Bed Transform o Player Transform no asignados.");
            isNearBed = false;
        }
    }

    void HandleSleepInput()
    {
        if (isNearBed && (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Q) || Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Q)))
        {
            if (canSleepAgain)
            {
                StartCoroutine(GoToSleep());
            }
            else
            {
                ShowSleepMessage("Espera para poder dormir de nuevo.");
            }
        }
    }

    IEnumerator GoToSleep()
    {
        canSleepAgain = false;
        isFading = true;
        hasSlept = false; // Resetear el flag al empezar a dormir

        // Cambiar cámara
        if (mainCamera != null && bedCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            bedCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Cámaras no asignadas.");
            isFading = false;
            canSleepAgain = true;
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
        SetFadeAlpha(1f); // Asegurar negro completo

        // Esperar
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
        SetFadeAlpha(0f); // Asegurar transparencia completa

        // Volver a la cámara principal
        if (mainCamera != null && bedCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            bedCamera.gameObject.SetActive(false);
        }

        isFading = false;
        hasSlept = true; // Se ha completado el dormir
        if (energyScript != null)
        {
            energyScript.RestoreEnergyFromSleep();
        }

        // Iniciar el cooldown para poder dormir de nuevo
        StartCoroutine(EnableSleepAgain());
    }

    IEnumerator EnableSleepAgain()
    {
        yield return new WaitForSeconds(sleepCooldown);
        canSleepAgain = true;
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
            Debug.LogWarning("Imagen de fundido no asignada.");
        }
    }

    void ShowSleepMessage(string message)
    {
        if (sleepMessageText != null)
        {
            sleepMessageText.text = message;
            sleepMessageText.gameObject.SetActive(true);
            StartCoroutine(HideSleepMessage());
        }
        else
        {
            Debug.LogWarning("Texto del mensaje de sueño no asignado.");
        }
    }

    IEnumerator HideSleepMessage()
    {
        yield return new WaitForSeconds(messageDisplayTime);
        if (sleepMessageText != null)
        {
            sleepMessageText.gameObject.SetActive(false);
        }
    }

}

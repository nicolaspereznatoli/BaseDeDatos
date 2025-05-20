using UnityEngine;
using TMPro;

public class XPshow : MonoBehaviour
{
    public TMP_Text xpText;

    void Start()
    {
        // Nos aseguramos de mostrar la XP inicial, incluso antes de recibir eventos
        if (GameManager.instance != null)
        {
            ActualizarXP(GameManager.instance.experiencia);
            GameManager.instance.OnExperienciaCambiada += ActualizarXP;
        }
        else
        {
            Debug.LogError("GameManager.instance es null");
        }
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnExperienciaCambiada -= ActualizarXP;
        }
    }

    void ActualizarXP(int nuevaXP)
    {
        if (xpText != null)
        {
            xpText.text = "" + nuevaXP;
            Debug.Log("XP mostrada: " + nuevaXP);
        }
    }
}

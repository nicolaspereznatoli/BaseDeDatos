using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject PanelOpciones;
    [SerializeField] private GameObject PanelMenu;

    [SerializeField] private Slider SlVelocidadPers;
    [SerializeField] private Slider SlMovRaton;



    public void Start()
    {
        PanelMenu.SetActive(true);
        PanelOpciones.SetActive(false);
    }



    //Menu

    public void EscenaJuego()
    {
        SceneManager.LoadScene(1);

    }

    public void Salir()
    {
        Application.Quit();

    }

    public void Opciones()
    {

        PanelOpciones.SetActive(true);
        PanelMenu.SetActive(false);


    }

    //Opciones

    public void CambiarVelocidad()
    {




    }
  

}


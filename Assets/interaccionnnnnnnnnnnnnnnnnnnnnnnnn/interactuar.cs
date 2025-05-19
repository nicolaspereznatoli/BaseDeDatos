using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public class interactuar : MonoBehaviour
{
    LayerMask mask;
    public float distancia = 1.5f;
    public Texture2D puntero;
    public GameObject TextDetect;
    GameObject ultimoReconocido = null;
     void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");
        TextDetect.SetActive(false);
    }
     void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit, distancia, mask))
        {
            Deselect();
            selectedObject(hit.transform);
            if (hit.collider.tag == "objetoInteractivo")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.transform.GetComponent<ItemInteractivo>().activarObjeto();
                    // aqui va la funcion de cada objeto
                }
            }
            Debug.DrawRay(transform.position,transform.TransformDirection(Vector3.forward)*distancia,Color.red);
        }
        else
        {
            Deselect();
        }


    }
    void selectedObject(Transform transform)
    {
        transform.GetComponent<MeshRenderer>().material.color= Color.green;
        ultimoReconocido = transform.gameObject;


    }
    void Deselect()
    {
        if (ultimoReconocido)
        {
            ultimoReconocido.GetComponent<MeshRenderer>().material.color = Color.white;
            ultimoReconocido = null;
        }
    }

    private void OnGUI()
    {
        Rect rect = new Rect(Screen.width / 2, Screen.height / 2, puntero.width, puntero.height);
        GUI.DrawTexture(rect,puntero);
        if (ultimoReconocido)
        {
            TextDetect.SetActive(true);
        }
        else
        {
            TextDetect.SetActive(false);

        }
    }

}

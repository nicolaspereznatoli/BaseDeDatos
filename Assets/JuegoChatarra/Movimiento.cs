using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movimiento : MonoBehaviour
{
    int contador = 0;
    public float Velocidad = 5f ;
    public float SteerSpeed = 180;
    public int Gap = 10;
    public int Chatarra = 0;
    public Text miTexto;
    NuevoDBManager nuevoDBManager = new NuevoDBManager();

    public GameObject Cuerpoprefab;
    private List<GameObject>PartesDelCuerpo = new List<GameObject>();
    private List<Vector3>PositionHistory = new List<Vector3>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        miTexto.text = Chatarra.ToString();

        transform.position += transform.forward * Velocidad * Time.deltaTime;
        float steerDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerDirection * SteerSpeed * Time.deltaTime);
        PositionHistory.Insert(0, transform.position);
        int index = 0;
        foreach(var body in PartesDelCuerpo)
        {
            Vector3 point = PositionHistory[Mathf.Min(index*Gap, PositionHistory.Count-1)];
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * Velocidad * Time.deltaTime;
            body.transform.LookAt(point);

            index++;
        }
    }
    private void SerpienteCrece()
    {
        GameObject Cuerpo = Instantiate(Cuerpoprefab);
        PartesDelCuerpo.Add(Cuerpo);
        contador++;
        if(contador == 5)
        {
            Velocidad = Velocidad * 1.3f;
            contador = 0;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            FindObjectOfType<Spawneocomida>().SpawnFood();
            
            Destroy(other.gameObject);
            SerpienteCrece();
            Chatarra++;
            nuevoDBManager.ConsigueChatarra(1); 
        }
        if (other.gameObject.CompareTag("Body"))
        {
            GameOver();
        }
        if (other.gameObject.CompareTag("Borde"))
        {
            GameOver();
        }


    }
    public void GameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0;
        SceneManager.LoadScene("EscenaJugar");

    }
}

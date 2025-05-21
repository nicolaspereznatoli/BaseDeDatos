using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawneocomida : MonoBehaviour
{
    public GameObject foodPrefab;
    public BoxCollider spawnArea;

    public void Start()
    {
        SpawnFood();
    }

    public void Update()
    {
        


    }
    public void SpawnFood()
    {
        Vector3 center = spawnArea.center + spawnArea.transform.position;
        Vector3 size = spawnArea.size;

        float z = Random.Range(center.z - (size.z / 2), center.z + (size.z / 2));
        float y = Random.Range(center.y - (size.y / 2), center.y + (size.y / 2));
        float x = center.x;

        Vector3 pos = new Vector3(x, y, z);
        Instantiate(foodPrefab, pos, Quaternion.identity);
    }
}

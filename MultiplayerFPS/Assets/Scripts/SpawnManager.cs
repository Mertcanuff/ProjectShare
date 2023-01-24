using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    private void Awake()
    {
        instance = this;
    }
    public Transform[] spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform spawn in spawnPoint)
        {
            spawn.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint[Random.Range(0, spawnPoint.Length)];
    }
}

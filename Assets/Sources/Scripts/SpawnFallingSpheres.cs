using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFallingSpheres : MonoBehaviour
{
    // Reference to what we're going to spawn
    public GameObject fallingSpherePrefab;

    // Spawn times
    public float firstSpawnDelay = 5.0f;
    public float spawnInterval = 3.0f;

    // Spawn location
    public Vector3 spawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        // Where to spawn the falling spheres?
        spawnPoint = new Vector3(0.0f, 6, 0.0f);

        // When to spawn the falling spheres?
        InvokeRepeating("SpawnFallingSphere", firstSpawnDelay, spawnInterval);



        // Coroutine (advanced, please calm down and die silently)
        // StartCoroutine(CoSpawnFallingSphere());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Spawning a falling sphere function
    void SpawnFallingSphere()
    {
        // Instantiate(GameObject prefab, Vector3 position, Quaternion rotation);
        Instantiate(fallingSpherePrefab, spawnPoint, fallingSpherePrefab.transform.rotation);
    }





    // Coroutines, proceed with caution
    IEnumerator CoSpawnFallingSphere()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Instantiate(fallingSpherePrefab, spawnPoint, fallingSpherePrefab.transform.rotation);
        }
    }
}
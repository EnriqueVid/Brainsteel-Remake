using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnFallingSpheres : MonoBehaviour
{
    // Reference to what we're going to spawn
    public GameObject fallingSpherePrefab;

    // Spawn times
    public float firstSpawnDelay = 5.0f;
    public float spawnInterval = 3.0f;

    private IEnumerator corutina;

    // Spawn location
    public Vector3 spawnPoint;

    // Whether to spawn or not, toggle box
    public Toggle spawningToggl;

    // Start is called before the first frame update
    void Start()
    {
        // Where to spawn the falling spheres?
        spawnPoint = new Vector3(0.0f, 6, 0.0f);


        // When to spawn the falling spheres?
        //InvokeRepeating("SpawnFallingSphere", firstSpawnDelay, spawnInterval);

        // Coroutine (advanced, please calm down and die silently)
        corutina = CoSpawnFallingSphere();
        SpawningControl();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Coroutines, proceed with caution
    IEnumerator CoSpawnFallingSphere()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnFallingSphere();
        }
    }

    // Spawning a falling sphere function
    public void SpawnFallingSphere()
    {
        // Instantiate(GameObject prefab, Vector3 position, Quaternion rotation);
        Instantiate(fallingSpherePrefab, spawnPoint, fallingSpherePrefab.transform.rotation);
    }








    // Called only when the Toggle box has been checked or unchecked
    public void SpawningControl()
    {
        if (spawningToggl.isOn)
        {
            StartCoroutine(corutina);
        }
        else
        {
            StopCoroutine(corutina);
        }
    }

}
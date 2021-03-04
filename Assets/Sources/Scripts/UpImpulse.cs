using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpImpulse : MonoBehaviour
{

    private Rigidbody sphereRb;

    public float forceValue = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        sphereRb = gameObject.GetComponent<Rigidbody>();

        sphereRb.AddForce(Vector3.up * forceValue, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else
        {
            //collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * forceValue, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }



}

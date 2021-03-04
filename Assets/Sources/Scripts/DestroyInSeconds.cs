using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{

    public float secondsLeft = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyThis", secondsLeft);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CancelInvoke();
            DestroyThis();
        }
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }

}

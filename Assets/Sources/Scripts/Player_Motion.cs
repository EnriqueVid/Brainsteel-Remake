using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Motion : MonoBehaviour
{

    public float cameraSensitivity = 100.0f;
    public float vertRotation = 0;
    public float velocity = 3.0f;
    public float gravity = -9.0f;

    private Camera playerCamera;
    private Vector3 vSpeed;
    

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        //ROTATE CAMERA & PLAYER
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        vertRotation -= mouseY;
        vertRotation = Mathf.Clamp(vertRotation, -85f, 85f);
        playerCamera.transform.localEulerAngles = Vector3.right * vertRotation;



        //MOVE PLAYER
        vSpeed.y -= gravity * Time.deltaTime;
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        movement *= velocity;
        movement += vSpeed;



        transform.Translate(movement * Time.deltaTime);




    }
}

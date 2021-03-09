using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Motion : MonoBehaviour
{
    //[Header("References")]                                            -> Añade un titulo en el inspector
    //[Tooltip("Reference to the main camera used for the player")]     -> Añade una descripcion a la variable declarada debajo cuando haces "Hover" 
    //[SerializeField]                                                  -> Permite modificar el valor de la variable en el inspector aunque sea PRIVADA
    //[Range(0.0f, 0.5f)]                                               -> Genera un slider con los rangos establecidos en el inspector para la variable seleccionada


    [Header("Camera Options")]
    [Tooltip("Modifica la velocidad de movimiento de la cámara")]
    public float cameraSensitivity = 100.0f;
    [Tooltip("Indica el ángulo inicial de la cámara")]
    [SerializeField] [Range(-90.0f, 90.0f)] float vertRotation = 0;


    [Header("Movement Options")]
    [SerializeField] float walkSpeed = 3.0f;
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float crouchSpeed = 1.5f;
    [SerializeField] float gravity = -9.0f;
    [SerializeField] float vSpeed = 0.0f;
    [SerializeField] bool dJump = false;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float dJumpHeight = 1f;



    [SerializeField] bool grounded;



    private CharacterController controller;
    private Camera playerCamera;
    
    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.enableOverlapRecovery = true;
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = controller.isGrounded;


        //ROTATE CAMERA & PLAYER
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        vertRotation -= mouseY;
        vertRotation = Mathf.Clamp(vertRotation, -85f, 85f);
        playerCamera.transform.localEulerAngles = Vector3.right * vertRotation;



        //MOVE PLAYER
        

        //Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));       -> GetAxis() devuelve un valor "smooth" lo que añade delay al movimiento
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //-> GetAxisRaw() obtiene el valor "Raw"
        inputDir.Normalize();

        Vector3 movement = (transform.forward * inputDir.y + transform.right * inputDir.x);

        movement *= walkSpeed;

        

        if (controller.isGrounded)
        {
            vSpeed = -2f;
            dJump = true;
        }

        
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if (controller.isGrounded)  //Salto normal
            {
                vSpeed = Mathf.Sqrt(jumpHeight * 2f * gravity);
            }
            else if (dJump) //Doble Salto
            {
                dJump = false;
                vSpeed = Mathf.Sqrt(dJumpHeight * 2f * gravity);
            }
        }

        vSpeed -= gravity * Time.deltaTime;

        movement += Vector3.up * vSpeed;

        controller.Move(movement * Time.deltaTime);





    }
}

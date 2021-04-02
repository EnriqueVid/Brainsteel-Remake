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
    
    // Movimiento
    [SerializeField] float walkSpeed = 5.0f;
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float crouchSpeed = 1.5f;
    [SerializeField] float airSpeed = 5.0f;
    [SerializeField] float gravity = 15.0f;

    // Agacharse y levantarse
    [SerializeField] float crouchHeight = 1.0f;
    [SerializeField] float standHeight = 1.8f;
    [SerializeField] float crouchInterpolation = 0.25f;
    
    // Salto y doble salto
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float dJumpHeight = 1f;

    bool dJump = false;
    bool crouched = false;

    [SerializeField] [Range(0.0f, 60.000000f)] float velocityInterpolation = 15;
    [Tooltip("[60 FPS], indica cuanto tarda el jugador en cambiar de velocidad | 1/vI ~= tiempo en segundos")]

    // Velocidad persistente a la que se mueve el jugador.
    Vector3 characterSpeed;

    // Variables que gestionan la respuesta del character controller al tocar el suelo.
    bool grounded;
    Vector3 groundNormal;

    float speedMultiplier = 1.0f;


    [Header("Debug Options")]
    public GameObject normalPos;
    public GameObject directorPos;
    public float t1 = 1.0000f;
    public float t2 = 1.0000f;


    private CharacterController controller;
    private Camera playerCamera;





    // Función que se llama una vez al crearse el objeto una vez que el juego está ejecutándose
    void Start()
    {
        QualitySettings.vSyncCount = 0;  // Deshabilitar VSync
        Application.targetFrameRate = 60;

        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        characterSpeed = Vector3.zero;
    }

    // Función que se ejecuta cada frame
    void Update()
    {
        GroundCheck();

        Look();

        Move();
    }

    // Comprueba si justo debajo del jugador hay suelo y si no se trata de una rampa demasiado inclinada.
    void GroundCheck()
    {
        grounded = false;
        groundNormal = Vector3.up;

        float checkDistance = 0.03f + controller.skinWidth;
        Vector3 initPoint = transform.position + (Vector3.up * controller.radius);

        if (Physics.SphereCast(initPoint, controller.radius, Vector3.down, out RaycastHit hit, checkDistance, -1, QueryTriggerInteraction.Ignore))
        {
            groundNormal = hit.normal;

            if (Vector3.Angle(Vector3.up, hit.normal) <= controller.slopeLimit)
            {
                grounded = true;
                dJump = true;

                // Si está un poco alejado del suelo y está cayendo, lo pegamos al suelo.
                if (hit.distance > controller.skinWidth && characterSpeed.y <= 0f)
                {
                    controller.Move(Vector3.down * hit.distance);
                }

            }
        }
    }

    void Look()
    {
        //Rotar jugador, character controller horizontalmente y cámara verticalmente.
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        vertRotation -= mouseY;
        vertRotation = Mathf.Clamp(vertRotation, -85f, 85f);
        playerCamera.transform.localEulerAngles = Vector3.right * vertRotation;
    }


    void Move()
    {

        speedMultiplier = 1.0f;


        // Agacharse y levantarse
        // El jugador se agacha mientras se mantenga el botón de agacharse pulsado y esté en el suelo.
        // Si el jugador está en el aire, se levanta.
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && grounded)
        {
            speedMultiplier = crouchSpeed;
            Crouch();
        }
        else
        {
            UnCrouch();
        }

        // Control de la dirección de movimiento por input
        Vector3 moveDir = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");
        moveDir = Vector3.ClampMagnitude(moveDir, 1);

        if (grounded)
        {
            if (groundNormal != Vector3.up)
            {
                // Reorientar vector del movimiento
                moveDir = GetReorientedVector(moveDir.normalized, groundNormal) * moveDir.magnitude;
            }

            moveDir *= walkSpeed * speedMultiplier;

            // Interpolación de velocidades, permite tener una pequeña inercia en el suelo
            characterSpeed = Vector3.Lerp(characterSpeed, moveDir, velocityInterpolation * Time.deltaTime);


            // Primer salto
            if (Input.GetButtonDown("Jump"))
            {
                characterSpeed.y = Mathf.Sqrt(jumpHeight * 2f * gravity);

                grounded = false;
                groundNormal = Vector3.up;
            }
        }
        else
        {
            // Interpolación de velocidades, en el aire
            characterSpeed += moveDir * airSpeed * velocityInterpolation * 0.5f * Time.deltaTime;
            
            // Limitador de la velocidad horizontal aérea
            float verticalVelocity = characterSpeed.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterSpeed, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, airSpeed);

            // Doble salto 
            if (Input.GetButtonDown("Jump") && dJump)
            {
                characterSpeed.y = Mathf.Sqrt(dJumpHeight * 2f * gravity);
                dJump = false;
            }
            else
            {
                characterSpeed = horizontalVelocity + (Vector3.up * verticalVelocity);
            }


            // Le aplicamos la gravedad a la velocidad del jugador
            characterSpeed += Vector3.down * gravity * Time.deltaTime;
        }

        // Movemos al character controller del jugador a la velocidad resultante de los cálculos previos
        controller.Move(characterSpeed * Time.deltaTime);



        // Si colisiona con algo, modifica su velocidad en función de la colisión.
            // Si colisiona con rampas muy inclinadas, se desliza.
            // Si colisiona con el suelo, la velocidad en Y se anula.
            // Si colisiona con una pared, la velocidad en dirección a esa pared se anula.
        Vector3 point1 = transform.position + (Vector3.up * controller.radius);
        Vector3 point2 = transform.position + (Vector3.up * (controller.height - controller.radius));

        if (Physics.CapsuleCast(point1, point2, controller.radius, characterSpeed.normalized, out RaycastHit hit, characterSpeed.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            characterSpeed = Vector3.ProjectOnPlane(characterSpeed, hit.normal);
        }

    }


    // Dados un vector director "direction" y la normal "normal" de un plano,
    // devuelve el vector "direction" proyectado sobre el plano de "normal".
    Vector3 GetReorientedVector(Vector3 direction, Vector3 normal)
    {
        Vector3 resul = Vector3.Cross(direction, Vector3.up);
        return Vector3.Cross(normal, resul).normalized;
    }

    // Función de agacharse que solo la realiza el jugador si estaba levantado anteriormente.
    void Crouch()
    {
        // Encogemos el character controller y decrementamos la posición de la cámara
        float heightStep = Time.deltaTime / crouchInterpolation;
            
        if (controller.height - heightStep <= crouchHeight)
        {
            heightStep = controller.height - crouchHeight;
        }

        controller.height -= heightStep;
        controller.center = Vector3.up * (controller.height / 2.0f);
        playerCamera.transform.position += Vector3.down * heightStep;

        crouched = true;
    }

    // Función de levantarse que solo la realiza el jugador si estaba agachado anteriormente.
    void UnCrouch()
    {
        if (crouched)
        {
            // Comprobamos si hay algún obstáculo que nos impida levantarnos
            Vector3 initPoint = transform.position + (Vector3.up * (controller.height - controller.radius));

            if (!Physics.SphereCast(initPoint, controller.radius, Vector3.up, out RaycastHit hit, crouchHeight, -1, QueryTriggerInteraction.Ignore))
            {
                // Agrandamos el character controller e incrementamos la posición de la cámara
                float heightStep = Time.deltaTime / crouchInterpolation;

                if (controller.height + heightStep >= standHeight)
                {
                    heightStep = standHeight - controller.height;
                    crouched = false;
                }

                controller.height += heightStep;
                controller.center = Vector3.up * (controller.height / 2.0f);
                playerCamera.transform.position += Vector3.up * heightStep;
            }
        }
    }
}

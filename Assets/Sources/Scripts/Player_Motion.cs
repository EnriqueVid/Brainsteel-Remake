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
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float dJumpHeight = 1f;

    [SerializeField] [Range(0.0f, 60.000000f)] float velocityInterpolation = 15;
    [Tooltip("[60 FPS], indica cuanto tarda el jugador en cambiar de velocidad | 1/vI ~= tiempo en segundos")]

    Vector3 characterSpeed;



    bool grounded;
    Vector3 groundNormal;

    float lastTimeInGround = 0.0f;
    float lastTimeInGroundMargin = 0.1f;


    bool touching;
    float angle = 0.0f;


    [Header("Debug Options")]
    public GameObject normalPos;
    public GameObject directorPos;
    public float t1 = 1.0000f;
    public float t2 = 1.0000f;



    private CharacterController controller;
    private Camera playerCamera;

    
    
    

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;

        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        characterSpeed = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();

        Look();

        Move();

        

        /*

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
                lastTimeInGround = Time.time;
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

        // DETECT COLLISIONS

        Vector3 p1 = transform.position + Vector3.up * controller.radius;
        Vector3 p2 = transform.position + Vector3.up * (controller.height - controller.radius);
        Vector3 dir = Vector3.down;

        touching = Physics.CapsuleCast(p1, p2, controller.radius, dir, out RaycastHit hit, controller.skinWidth + 0.01f);

        if (touching)
        {
            normalPos.transform.position = hit.point;

            LineRenderer lr = normalPos.GetComponentInChildren<LineRenderer>();
            lr.SetPosition(1, hit.normal);


            //angle = Mathf.Acos(Vector3.Dot(hit.normal, Vector3.up)/ (hit.normal.magnitude * Vector3.up.magnitude));

            angle = Vector3.Angle(hit.normal, Vector3.up);
            angle = 90 - angle;

            if (angle < 70)
            {
                movement += hit.normal * 90 * Time.deltaTime;
            }
        }



        controller.Move(movement * Time.deltaTime);

        //LineRenderer lr2 = GetComponentInChildren<LineRenderer>();
        //lr2.SetPosition(1, movement);

        UpdateDebug(movement);

        */
    }

    void GroundCheck() 
    {
        grounded = false;
        groundNormal = Vector3.up;

        //Comprobar si ha pasado un tiempo para poder saltar
        if (Time.time >= lastTimeInGround + lastTimeInGroundMargin)
        {
            //Sphere cast para comprobar suelo (0.041 - 0.07)
            float checkDistance = 0.04f + controller.skinWidth;
            Vector3 initPoint = transform.position + (Vector3.up * controller.radius);

            if (Physics.SphereCast(initPoint, controller.radius, Vector3.down, out RaycastHit hit, checkDistance, -1, QueryTriggerInteraction.Ignore))
            {
                groundNormal = hit.normal;

                if (Vector3.Angle(Vector3.up, hit.normal) <= controller.slopeLimit)
                {
                    grounded = true;
                    dJump = true;

                    // handle snapping to the ground
                    if (hit.distance > controller.skinWidth)
                    {
                        controller.Move(Vector3.down * hit.distance);
                    }

                }
            }
        }
    }

    void Look() 
    {
        //ROTATE CAMERA & PLAYER
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        vertRotation -= mouseY;
        vertRotation = Mathf.Clamp(vertRotation, -85f, 85f);
        playerCamera.transform.localEulerAngles = Vector3.right * vertRotation;
    }


    void Move() 
    {
        //Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        //moveDir = Vector3.ClampMagnitude(moveDir, 1);
        //Vector3 globalMoveDir = transform.TransformVector(moveDir);


        Vector3 moveDir = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");
        moveDir = Vector3.ClampMagnitude(moveDir, 1);

        if (grounded)
        {
            if (groundNormal != Vector3.up) 
            {
                //Reorientar vector del movimiento
                moveDir = GetReorientedVector(moveDir.normalized) * moveDir.magnitude;
            }

            moveDir *= walkSpeed;
                                         
            characterSpeed = Vector3.Lerp(characterSpeed, moveDir, velocityInterpolation * Time.deltaTime);

            if (Input.GetButtonDown("Jump"))
            {
                characterSpeed = Vector3.ProjectOnPlane(characterSpeed, Vector3.up);
                characterSpeed += Vector3.up * Mathf.Sqrt(jumpHeight * 2f * gravity);

                lastTimeInGround = Time.time;
                grounded = false;
                groundNormal = Vector3.up;
            }

        }
        else 
        {
            characterSpeed += moveDir * 25 * Time.deltaTime;

            // limit air speed to a maximum, but only horizontally
            float verticalVelocity = characterSpeed.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterSpeed, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, 10);
            
            if (Input.GetButtonDown("Jump") && dJump)
            {
                characterSpeed += Vector3.up * Mathf.Sqrt(dJumpHeight * 2f * gravity);
                dJump = false;
            }
            else 
            {
                characterSpeed = horizontalVelocity + (Vector3.up * verticalVelocity);
            }


            // apply the gravity to the velocity
            characterSpeed += Vector3.down * gravity * Time.deltaTime;

        }


        controller.Move(characterSpeed * Time.deltaTime);

        Vector3 point1 = transform.position + (Vector3.up * controller.radius);
        Vector3 point2 = transform.position + (Vector3.up * (controller.height - controller.radius));

        if (Physics.CapsuleCast(point1, point2, controller.radius, characterSpeed.normalized, out RaycastHit hit, characterSpeed.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore)) 
        {
            characterSpeed = Vector3.ProjectOnPlane(characterSpeed, hit.normal);
        }

    }

    Vector3 GetReorientedVector(Vector3 direction) 
    {
        Vector3 resul = Vector3.Cross(direction, Vector3.up);
        return Vector3.Cross(groundNormal, resul).normalized;
    }


    void UpdateDebug(Vector3 mov) 
    {
        Vector3 mov_norm = mov.normalized;
        directorPos.transform.position = transform.position;
        directorPos.transform.rotation = Quaternion.LookRotation(mov_norm, Vector3.up);

    }
}

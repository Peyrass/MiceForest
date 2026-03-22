using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SC_TPChar_Controller : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityScale;
    [SerializeField] private float movementSmoothFactor = 0.3f;
    [SerializeField] private float rotationSmoothFactor = 0.3f;

    [Header("Ground Detection")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip coinSound;
    private AudioSource audioSource;
        
    private int coinCount = 0;
        
    private CharacterController controller;
    private Camera cam;

    private Animator anim;
    private bool isGrounded;
       
    private Vector2 inputVector;
    private Vector3 verticalMovement;
        
    private float currentSpeed;
    private float speedVelocity;
    private float rotationVelocity;
        
    private float currentInputX;
    private float currentInputY;
    private float inputXVelocity;
    private float inputYVelocity;

    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
        anim = GetComponentInChildren<Animator>();
        cam = Camera.main;
            
        audioSource = GetComponent<AudioSource>();
            
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        PlayerInput.actions["Move"].canceled += UpdateMovement;
        PlayerInput.actions["Move"].performed += UpdateMovement;
        PlayerInput.actions["Jump"].started += Jump;
    }
        
    private void OnDisable() //implementación de buenas prácticas
    {
        PlayerInput.actions["Move"].canceled -= UpdateMovement;
        PlayerInput.actions["Move"].performed -= UpdateMovement;
        PlayerInput.actions["Jump"].started -= Jump;
    }

    private void UpdateMovement(InputAction.CallbackContext ctx)
    {
        inputVector = ctx.ReadValue<Vector2>();
    }
        
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            anim.SetTrigger("Jump");
            verticalMovement.y = Mathf.Sqrt(-2 * gravityScale * jumpHeight);
        }
    }

    void Update()
    {
        GroundCheck();
        ApplyGravity();
        MoveAndRotate();
            
        anim.SetBool("isGrounded", isGrounded);
    }

    private void MoveAndRotate()
    {
        //Se aplica al cuerpo la rotación que tenga la cámara.
        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            
           
        currentInputX = Mathf.SmoothDamp(currentInputX, inputVector.x, ref inputXVelocity, movementSmoothFactor);
        currentInputY = Mathf.SmoothDamp(currentInputY, inputVector.y, ref inputYVelocity, movementSmoothFactor);
            
        // Calcular velocidad objetivo (respeta magnitud del joystick)
        float targetSpeed = movementSpeed * inputVector.magnitude;
            
        // Suavizar velocidad actual
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, movementSmoothFactor);
            
        // Calcular movimiento
        Vector3 movement = Vector3.zero;
            
        if (inputVector.magnitude > 0)
        {
            // Calcular ángulo basado en input y rotación Y de la cámara
            float angle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                
            // Rotar Vector3.forward al ángulo calculado (solo en Y para mantener horizontal)
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                
            movement = direction * currentSpeed;
        }
            
        // Aplicar movimiento
        controller.Move((movement + verticalMovement) * Time.deltaTime);
            
        // Actualizar animación con valores suavizados
        anim.SetFloat("x", currentInputX);
        anim.SetFloat("y", currentInputY);
    }

    private void ApplyGravity()
    {
        if (isGrounded && verticalMovement.y < 0)
        {
            verticalMovement.y = -2f;
        }
        else
        {
            verticalMovement.y += gravityScale * Time.deltaTime;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        if (feet != null)
        {
            Gizmos.DrawSphere(feet.position, detectionRadius);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
            
        if (other.CompareTag("DeathZone"))
        {
            RestartScene();
        }

        if (other.CompareTag("CoinTag"))
        {
            audioSource.PlayOneShot(coinSound);
            coinCount++;
            Destroy(other.gameObject);

            if (coinCount >= 3)
            {
                NextScene();
            }
        }
    }
        
    private void NextScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("EndScene");
    }
        
    private void PrepareRestart()
    {
        audioSource.PlayOneShot(deathSound);
        this.enabled = false;
        Invoke("RestartScene", 1f);
    }

    private void RestartScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
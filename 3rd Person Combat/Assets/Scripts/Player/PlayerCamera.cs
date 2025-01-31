using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    private PlayerInputManager InputManager;

    [Header("References")]
    public Camera cameraObject;
    public Player player;
    public PlayerCameraData cameraData;

    [Header("Camera Settings")]
    private float cameraSmoothSpeed = 1;  //TONOTE: Higher the number, Longer the camera will take to follow player
    private float horizontalRotationSpeed;
    private float verticalRotationSpeed;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    float horizontalLookAngle;
    float verticalLookAngle;

    private Vector2 CameraInput;
    private float verticalInput;
    private float horizontalInput;
    private Vector3 cameraRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(cameraObject.transform.root.gameObject);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            horizontalRotationSpeed = cameraData.mouseHorizontalRotationSpeed;
            verticalRotationSpeed = cameraData.mouseVerticalRotationSpeed;
        }
        else
        {
            horizontalRotationSpeed = cameraData.gamepadHorizontalRotationSpeed;
            verticalRotationSpeed = cameraData.gamepadVerticalRotationSpeed;
        }

    }

    public void SetInputManager(PlayerInputManager playerInputManager)
    {
        InputManager = playerInputManager;
    }

    public void OnCameraInput(InputAction.CallbackContext context)
    {
        CameraInput = context.ReadValue<Vector2>();
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowPLayer();
            HandleCameraInput();
            HandleCameraRotation();
        }

    }

    private void HandleCameraInput()
    {
        verticalInput = CameraInput.y;
        horizontalInput = CameraInput.x;

    }

    private void HandleFollowPLayer()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleCameraRotation()
    {
        cameraObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

        horizontalLookAngle += (horizontalInput * horizontalRotationSpeed) * Time.deltaTime;
        verticalLookAngle -= (verticalInput * verticalRotationSpeed) * Time.deltaTime;

        verticalLookAngle = Mathf.Clamp(verticalLookAngle, cameraData.minimumPivot, cameraData.maximumPivot);

        cameraRotation = Vector3.zero;

        cameraRotation.y = horizontalLookAngle;
        cameraRotation.x = verticalLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;
    }

}

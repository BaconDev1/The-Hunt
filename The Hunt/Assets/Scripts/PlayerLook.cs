using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerLook : MonoBehaviour
{

    public Transform PlayerCamera;
    public Vector2 Sensitivities;

    private Vector2 XYRotation;

    [Header("Camera Tilt")]
    [SerializeField] private float tiltAmount;
    [SerializeField] private float tiltStartSpeed;
    [SerializeField] private float tiltEndSpeed;

    private float currentTilt = 0f;
    private float targetTilt = 0f;
    public bool RotationEnabled = true;       // Normal mouse rotation
    public bool IgnoreCamera = false;         // Completely stops writing to camera

    public bool canLook = true;

    public void SetCanMove(bool value)
    {
        // look doesn't handle movement, ignore this
    }

    public void SetCanLook(bool value)
    {
        canLook = value;

       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canLook)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (RotationEnabled)
        {
            Vector2 MouseInput = new Vector2 { x = Input.GetAxis("Mouse X"), y = Input.GetAxis("Mouse Y") };

            XYRotation.x -= MouseInput.y * Sensitivities.y;
            XYRotation.y += MouseInput.x * Sensitivities.x;

            XYRotation.x = Mathf.Clamp(XYRotation.x, -90f, 90f);

            transform.eulerAngles = new Vector3(0f, XYRotation.y, 0);
          
            // Only update camera if we're not ignoring it
            if (!IgnoreCamera)
                PlayerCamera.localEulerAngles = new Vector3(XYRotation.x, 0f, currentTilt);

            
        }

        CameraTilt();
    }
    public void ForceSetRotation(Quaternion worldRotation)
    {
        // Convert to local Euler relative to the player body
        Vector3 euler = worldRotation.eulerAngles;

        // Convert 0–360 Unity angles into −180…180 range
        float pitch = euler.x > 180f ? euler.x - 360f : euler.x;
        float yaw = euler.y;

        XYRotation.x = Mathf.Clamp(pitch, -90f, 90f);
        XYRotation.y = yaw;
    }
    private void CameraTilt()
    {
        bool leftStrafe = Input.GetKey(KeyCode.A);
        bool rightStrafe = Input.GetKey(KeyCode.D);

        if(leftStrafe && !rightStrafe)
        {
            targetTilt = tiltAmount;
        }else if (rightStrafe && !leftStrafe)
        {
            targetTilt = -tiltAmount;
        }
        else
        {
            targetTilt = 0f;
        }

        float smoothTilt;
        if(targetTilt == 0)
        {
            smoothTilt = tiltEndSpeed;
        }
        else
        {
            smoothTilt = tiltStartSpeed;
        }

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, smoothTilt * Time.deltaTime);
    }
}

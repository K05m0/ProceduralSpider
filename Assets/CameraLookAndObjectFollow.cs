using UnityEngine;

public class CameraLookAndObjectFollow : MonoBehaviour
{
    [Header("Ustawienia kamery")]
    public Transform cameraTransform; // np. Main Camera
    public Transform playerBody;      // np. obiekt gracza
    public float mouseSensitivity = 100f;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MouseLook();
    }

    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Obracanie kamery góra/dół
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Obracanie ciała tylko gdy nie trzymasz X
        if (!Input.GetKey(KeyCode.X))
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}

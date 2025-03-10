using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{
    public float speed = 10f;
    public float lookSpeed = 2f;
    public float sprintMultiplier = 2f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse look
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Movement
        float moveSpeed = speed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        // Vertical movement (Q/E)
        if (Input.GetKey(KeyCode.Q)) move.y -= 1;
        if (Input.GetKey(KeyCode.E)) move.y += 1;

        transform.position += transform.TransformDirection(move) * moveSpeed * Time.deltaTime;

        // Unlock cursor on Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
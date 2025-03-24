using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Camera speed
    public float minX = -40f;
    public float maxX = 40f;

    void Start()
    {
        // Set the camera to start at the leftmost position
        transform.position = new Vector3(minX, transform.position.y, transform.position.z);
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float newX = transform.position.x + horizontal * moveSpeed * Time.deltaTime;

        // Restrict the movement of the camera to the X axis
        newX = Mathf.Clamp(newX, minX, maxX);

        // Apply new position
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Move camera with mouse
        if (Input.mousePosition.x <= 10)
            transform.position += new Vector3(-moveSpeed * Time.deltaTime, 0, 0);
        if (Input.mousePosition.x >= Screen.width - 10)
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
    }
}



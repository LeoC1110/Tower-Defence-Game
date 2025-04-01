using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float minX = -40f;
    public float maxX = 40f;

    [Header("Touch Control")]
    private Vector2 touchStartPos;
    private bool isDragging = false;

    void Start()
    {
        transform.position = new Vector3(minX, transform.position.y, transform.position.z);
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandlePCInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif
    }

    private void HandlePCInput()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (Input.mousePosition.x <= 10) horizontal = -1f;
        if (Input.mousePosition.x >= Screen.width - 10) horizontal = 1f;

        MoveCamera(horizontal);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        float deltaX = touch.position.x - touchStartPos.x;
                        float direction = -Mathf.Sign(deltaX); // Invert: drag left ? move right

                        MoveCamera(direction);
                        touchStartPos = touch.position; // Update for continuous swipe
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
    }

    private void MoveCamera(float direction)
    {
        float newX = transform.position.x + direction * moveSpeed * Time.deltaTime;
        newX = Mathf.Clamp(newX, minX, maxX);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}

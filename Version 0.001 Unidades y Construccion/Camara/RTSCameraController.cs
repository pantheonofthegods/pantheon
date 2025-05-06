using UnityEngine;
using UnityEngine.EventSystems;

public class RTSCameraController : MonoBehaviour
{
    public static RTSCameraController instance;

    [Header("General")]
    [SerializeField] Transform cameraTransform;
    public Transform followTransform;
    Vector3 newPosition;
    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;

    [Header("Optional Functionality")]
    [SerializeField] bool moveWithKeyboad;
    [SerializeField] bool moveWithEdgeScrolling;
    [SerializeField] bool moveWithMouseDrag;

    [Header("Keyboard Movement")]
    [SerializeField] float fastSpeed = 5f;
    [SerializeField] float normalSpeed = 3f;
    float movementSpeed;

    [Header("Mouse Wheel Zoom")]
    [SerializeField] float zoomSpeed = 10f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 20f;
    [SerializeField] float zoomSensitivity = 1f;

    [Header("Edge Scrolling Movement")]
    [SerializeField] float edgeSize = 50f;
    bool isCursorSet = false;
    public Texture2D cursorArrowUp;
    public Texture2D cursorArrowDown;
    public Texture2D cursorArrowLeft;
    public Texture2D cursorArrowRight;

    CursorArrow currentCursor = CursorArrow.DEFAULT;
    enum CursorArrow
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        DEFAULT
    }

    private void Start()
    {
        instance = this;
        newPosition = transform.position;
        movementSpeed = normalSpeed;
    }

    private void Update()
    {
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }
        else
        {
            HandleCameraMovement();
            HandleMouseWheelZoom();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    void HandleCameraMovement()
    {
        if (moveWithMouseDrag)
        {
            HandleMouseDragInput();
        }

        if (moveWithKeyboad)
        {
            HandleKeyboardInput();
        }

        if (moveWithEdgeScrolling)
        {
            HandleEdgeScrolling();
        }

        transform.position = newPosition;
    }

    private void HandleMouseWheelZoom()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // Movimiento directo del zoom sin inercia
            Vector3 zoomDirection = cameraTransform.forward * scroll * zoomSpeed * zoomSensitivity;
            cameraTransform.position += zoomDirection;

            // Limitar zoom entre los valores mínimo y máximo
            float currentHeight = cameraTransform.localPosition.y;
            currentHeight = Mathf.Clamp(currentHeight, minZoom, maxZoom);
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                currentHeight,
                cameraTransform.localPosition.z
            );
        }
    }


    void HandleKeyboardInput()
    {
        movementSpeed = Input.GetKey(KeyCode.LeftCommand) ? fastSpeed : normalSpeed;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += transform.forward * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += transform.forward * -movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += transform.right * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += transform.right * -movementSpeed * Time.deltaTime;
        }
    }

    void HandleEdgeScrolling()
    {
        // Move Right
        if (Input.mousePosition.x > Screen.width - edgeSize)
        {
            newPosition += transform.right * movementSpeed * Time.deltaTime;
            ChangeCursor(CursorArrow.RIGHT);
            isCursorSet = true;
        }
        // Move Left
        else if (Input.mousePosition.x < edgeSize)
        {
            newPosition += transform.right * -movementSpeed * Time.deltaTime;
            ChangeCursor(CursorArrow.LEFT);
            isCursorSet = true;
        }
        // Move Up
        else if (Input.mousePosition.y > Screen.height - edgeSize)
        {
            newPosition += transform.forward * movementSpeed * Time.deltaTime;
            ChangeCursor(CursorArrow.UP);
            isCursorSet = true;
        }
        // Move Down
        else if (Input.mousePosition.y < edgeSize)
        {
            newPosition += transform.forward * -movementSpeed * Time.deltaTime;
            ChangeCursor(CursorArrow.DOWN);
            isCursorSet = true;
        }
        else if (isCursorSet)
        {
            ChangeCursor(CursorArrow.DEFAULT);
            isCursorSet = false;
        }
    }

    private void ChangeCursor(CursorArrow newCursor)
    {
        if (currentCursor != newCursor)
        {
            switch (newCursor)
            {
                case CursorArrow.UP:
                    Cursor.SetCursor(cursorArrowUp, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.DOWN:
                    Cursor.SetCursor(cursorArrowDown, new Vector2(cursorArrowDown.width, cursorArrowDown.height), CursorMode.Auto);
                    break;
                case CursorArrow.LEFT:
                    Cursor.SetCursor(cursorArrowLeft, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorArrow.RIGHT:
                    Cursor.SetCursor(cursorArrowRight, new Vector2(cursorArrowRight.width, cursorArrowRight.height), CursorMode.Auto);
                    break;
                case CursorArrow.DEFAULT:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }
            currentCursor = newCursor;
        }
    }

    private void HandleMouseDragInput()
    {
        if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject())
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(2) && !EventSystem.current.IsPointerOverGameObject())
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }
}
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Range(0, 0.1f)] private float mouseScreenEdgeFactor;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float pitchSpeed;
    [SerializeField] private int maxZoomLevel;
    [SerializeField] private int minZoomLevel;
    [SerializeField] private float maxPitch;
    [SerializeField] private float minPitch;
    [SerializeField] private float zoomLevelScaling;
    [SerializeField] private float zoomBaseAmount;
    [SerializeField] private float zoomMinAdjustmentSpeed;
    [SerializeField] private float zoomAdjustmentFactor;
    [SerializeField] private float zoomAmountInfluenceOnBounds;
    [SerializeField] private KeyCode moveUpKeyCode;
    [SerializeField] private KeyCode moveLeftKeyCode;
    [SerializeField] private KeyCode moveRightKeyCode;
    [SerializeField] private KeyCode moveDownKeyCode;
    [SerializeField] private KeyCode rotateLeftKeyCode;
    [SerializeField] private KeyCode rotateRightKeyCode;
    [SerializeField] private Transform cameraPitchTransform;
    [SerializeField] private Transform cameraZoomTransform;

    private int targetZoomLevel;
    private float currentZoomLevel;
    private Vector3 lastMousePosition;
    private bool mouseLocked;
    private Vector2 bottomLeftBounds;
    private Vector2 topRightBounds;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    private void Update()
    {
        Update_CameraPosition();
        Update_CameraRotation();
        Update_CameraZoom();
        Update_CameraOrientation();
        Update_CameraPositionInBounds();
    }

    private void Update_CameraOrientation()
    {
        if (mouseLocked)
        {
            float verticalMovement = lastMousePosition.y - Input.mousePosition.y;
            float horizontalMovement = lastMousePosition.x - Input.mousePosition.x;

            float currentPitch = cameraPitchTransform.rotation.eulerAngles.x;
            currentPitch += verticalMovement * pitchSpeed;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
            cameraPitchTransform.localRotation = Quaternion.Euler(currentPitch, 0, 0);
            transform.Rotate(0, -horizontalMovement * pitchSpeed, 0);

            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
            mouseLocked = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            mouseLocked = false;
        }
    }

    private void Update_CameraZoom()
    {
        targetZoomLevel = Mathf.Clamp((int) (targetZoomLevel - Input.mouseScrollDelta.y), minZoomLevel, maxZoomLevel);
        float difference = targetZoomLevel - currentZoomLevel;
        float zoomAdjustmentAmount;
        if (difference < 0)
        {
            zoomAdjustmentAmount = (-zoomMinAdjustmentSpeed + difference * zoomAdjustmentFactor) * Time.deltaTime;
            zoomAdjustmentAmount = Mathf.Max(difference, zoomAdjustmentAmount);
        }
        else
        {
            zoomAdjustmentAmount = (zoomMinAdjustmentSpeed + difference * zoomAdjustmentFactor) * Time.deltaTime;
            zoomAdjustmentAmount = Mathf.Min(difference, zoomAdjustmentAmount);
        }
        currentZoomLevel += zoomAdjustmentAmount;
        Vector3 localPosition = cameraZoomTransform.localPosition;
        localPosition.z = -GetZoomAmount();
        cameraZoomTransform.localPosition = localPosition;
    }

    private float GetZoomAmount()
    {
        return Mathf.Pow(currentZoomLevel, zoomLevelScaling) + zoomBaseAmount;
    }

    private void Update_CameraRotation()
    {
        float angleAdjustment = 0;

        if (Input.GetKey(rotateLeftKeyCode))
            angleAdjustment -= rotateSpeed * Time.deltaTime;

        if (Input.GetKey(rotateRightKeyCode))
            angleAdjustment += rotateSpeed * Time.deltaTime;

        transform.Rotate(0, angleAdjustment, 0);
    }

    private void Update_CameraPosition()
    {
        Vector3 desiredVelocity = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.x /= Screen.width;
        mousePosition.y /= Screen.height;

        if (Input.GetKey(moveDownKeyCode) || mousePosition.y <= mouseScreenEdgeFactor && mousePosition.y >= 0)
            desiredVelocity += Vector3.back;

        if (Input.GetKey(moveUpKeyCode) || mousePosition.y >= 1 - mouseScreenEdgeFactor && mousePosition.y <= 1)
            desiredVelocity += Vector3.forward ;

        if (Input.GetKey(moveLeftKeyCode) || mousePosition.x <= mouseScreenEdgeFactor && mousePosition.x >= 0)
            desiredVelocity += Vector3.left;

        if (Input.GetKey(moveRightKeyCode) || mousePosition.x >= 1 - mouseScreenEdgeFactor && mousePosition.x <= 1)
            desiredVelocity += Vector3.right;

        Vector3 moveVelocity = transform.rotation * desiredVelocity.normalized * moveSpeed * GetZoomAmount() * Time.deltaTime;
        transform.position += moveVelocity;
    }

    private void Update_CameraPositionInBounds()
    {
        Vector3 position = transform.position;
        float zoomAmount = GetZoomAmount() * zoomAmountInfluenceOnBounds;
        position.x = Mathf.Clamp(position.x, bottomLeftBounds.x + zoomAmount, topRightBounds.x - zoomAmount);
        position.z = Mathf.Clamp(position.z, bottomLeftBounds.y + zoomAmount, topRightBounds.y - zoomAmount);
        transform.position = position;
    }

    public void InitializeCameraBounds(Vector2 bottomLeftBounds, Vector2 topRightBounds)
    {
        this.bottomLeftBounds = bottomLeftBounds;
        this.topRightBounds = topRightBounds;
    }

    public void SetCameraPosition(Vector2 position)
    {
        Vector3 worldPosition = new Vector3(position.x, 0, position.y);
        transform.position = worldPosition;
    }
}
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 14f, -10f);
    [SerializeField] private float smoothSpeed = 8f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoomY = 10f;
    [SerializeField] private float maxZoomY = 18f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        HandleZoom();
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            offset.y -= scroll * zoomSpeed;
            offset.y = Mathf.Clamp(offset.y, minZoomY, maxZoomY);

            offset.z = -offset.y * 0.7f;
        }
    }
}
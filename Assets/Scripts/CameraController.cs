using UnityEngine;
using Lean.Touch;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    private const string MouseScrollwheel = "Mouse ScrollWheel";

    [SerializeField] private CinemachineCamera virtualCamera;

    public float zoomSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 20f;
    public float rotationSpeed = 1f;

    private CinemachinePositionComposer composer;
    private CinemachinePanTilt panTilt;

    private void Start()
    {
        composer = virtualCamera.GetComponent<CinemachinePositionComposer>();
        panTilt = virtualCamera.GetComponent<CinemachinePanTilt>();
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    private void Update()
    {
        HandleMouseScroll();
    }

    private void HandleMouseScroll()
    {
        float scroll = Input.GetAxis(MouseScrollwheel);
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newDistance = composer.CameraDistance - scroll * zoomSpeed;
            newDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
            composer.CameraDistance = newDistance;
        }
    }

    private void HandleFingerUpdate(LeanFinger finger)
    {
        if (finger.IsActive && finger.ScreenDelta.magnitude > 0)
        {
            float panAxis = panTilt.PanAxis.Value;
            panAxis += finger.ScreenDelta.x * rotationSpeed * 0.1f;
            panAxis = Mathf.Repeat(panAxis, 360f);
            panTilt.PanAxis.Value = panAxis;
        }
    }
}
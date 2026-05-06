using UnityEngine;

public class Cameraorbit: MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;             // ContainerCenter

    [Header("Distancia")]
    public float distance = 8f;
    public float minDistance = 3f;
    public float maxDistance = 18f;
    public float zoomSpeed = 3f;
    public float zoomSmoothing = 8f;

    [Header("Rotación")]
    public float rotationSpeed = 200f;
    public float minVerticalAngle = 5f;
    public float maxVerticalAngle = 85f;
    public float rotationSmoothing = 10f;

    [Header("Rotación")]
    public float fixedPitch=30f;

    private float currentYaw = 0f;
    private float currentPitch = 30f;
    private float targetYaw = 0f;
    private float targetPitch = 30f;
    private float targetDistance;
    private bool isRotating = false;

    void Start()
    {
        targetDistance = distance;
        currentYaw = transform.eulerAngles.y;
        //currentPitch = transform.eulerAngles.x;
        targetYaw = currentYaw;
        targetPitch = fixedPitch;
        currentPitch = fixedPitch;

    }

    void Update()
    {
        HandleRotationInput();
        HandleZoomInput();
    }

    void LateUpdate()
    {
        if (target == null) return;
        ApplyCameraTransform();
    }

    void HandleRotationInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (isRotating)
        {
            targetYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            //targetPitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            //targetPitch = Mathf.Clamp(targetPitch, minVerticalAngle, maxVerticalAngle);
        }
        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, rotationSmoothing * Time.deltaTime);
        //currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, rotationSmoothing * Time.deltaTime);
    }

    void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            targetDistance -= scroll * zoomSpeed;
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        distance = Mathf.Lerp(distance, targetDistance, zoomSmoothing * Time.deltaTime);
    }

    void ApplyCameraTransform()
    {
        Quaternion rotation = Quaternion.Euler(fixedPitch, currentYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }

    public void SetLocked(bool locked)
    {
        enabled = !locked;

        // Si se bloquea, liberar el cursor por si quedó atrapado
        if (locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isRotating = false;
        }
    }


}

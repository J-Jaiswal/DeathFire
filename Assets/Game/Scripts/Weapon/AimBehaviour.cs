using UnityEngine;

public class AimBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform aim;      
    [SerializeField] private Transform muzzle;  
    [SerializeField] private Camera mainCamera;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float planeDistance = 15f;

    [Header("Clamp")]
    [SerializeField] private Vector2 clampX = new Vector2(-6f, 6f);
    [SerializeField] private Vector2 clampY = new Vector2(-3f, 4f);

    private float mouseX;
    private float mouseY;

    void Awake()
    {
        if (!mainCamera)
            mainCamera = Camera.main;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ResetAimToCenter();
    }

    void Update()
    {
        HandleMouseInput();
        UpdateAimPosition();
        //RotateGun(); // enable only if needed
    }

    void HandleMouseInput()
    {
        mouseX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        mouseX = Mathf.Clamp(mouseX, clampX.x, clampX.y);
        mouseY = Mathf.Clamp(mouseY, clampY.x, clampY.y);
    }

    void UpdateAimPosition()
    {
        // Aim moves relative to camera
        Vector3 localAimOffset = new Vector3(mouseX, mouseY, planeDistance);
        aim.position = mainCamera.transform.TransformPoint(localAimOffset);
    }

    void ResetAimToCenter()
    {
        mouseX = 0f;
        mouseY = 0f;

        Vector3 centerOffset = new Vector3(0f, 0f, planeDistance);
        aim.position = mainCamera.transform.TransformPoint(centerOffset);
    }


    void RotateGun()
    {
        if (!muzzle || !aim) return;
        muzzle.forward = GetDirection();
    }

    public Vector3 GetDirection()
    {
        return (aim.position - muzzle.position).normalized;
    }
}

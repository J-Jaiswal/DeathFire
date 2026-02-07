using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gunRoot;     // Parent of arms + gun
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform aim;         // World-space aim target

    [Header("Rotation Settings")]
    [SerializeField] private float rotationStrength = 20f; // small values (2–6)
    [SerializeField] private Vector2 maxRotation = new Vector2(20f, 5f); // yaw, pitch

    [Header("Pitch Position Settings")]
    [SerializeField] private float pitchPositionStrength = 0.0025f;
    [SerializeField] private float pitchLerpSpeed = 8f;
    [SerializeField] private float maxPitchOffset = 0.015f; // safety clamp
    [Header("Visual Pitch Pivot")]
    [SerializeField] private Transform gunVisualPivot; // child mesh or pivot
   



    private Quaternion defaultLocalRotation;
    private Quaternion defaultVisualRotation;


    void Awake()
    {
        defaultLocalRotation = gunRoot.localRotation;
        defaultVisualRotation = gunVisualPivot.localRotation; // pitch
     }

    void Update()
    {
        ApplyYawRotation();
        ApplyPitchAlignment();

    }

    void ApplyYawRotation()
    {
        Vector3 dir = (aim.position - muzzle.position).normalized;
        Vector3 localDir = gunRoot.InverseTransformDirection(dir);

        // Y rotation ONLY (yaw: left / right)
        float yRotation = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        // Very small clamp to avoid arm exposure
        yRotation = Mathf.Clamp(yRotation, -20.0f, 20.0f);

        Quaternion target =
            Quaternion.Euler(0f, yRotation, 0f) * defaultLocalRotation;

        gunRoot.localRotation =
            Quaternion.Slerp(
                gunRoot.localRotation,
                target,
                Time.deltaTime * rotationStrength
            );
    }
    void ApplyPitchAlignment()
    {
        Vector3 dir = (aim.position - muzzle.position).normalized;
        Vector3 localDir = gunRoot.InverseTransformDirection(dir);

        // True pitch (up / down)
        float pitchAngle = -Mathf.Asin(localDir.y) * Mathf.Rad2Deg;
        pitchAngle = Mathf.Clamp(pitchAngle, -maxRotation.y, maxRotation.y);

        Quaternion pitchRot = Quaternion.Euler(pitchAngle, 0f, 0f);
        Quaternion targetRot = pitchRot * defaultVisualRotation;

        gunVisualPivot.localRotation = Quaternion.Slerp(
            gunVisualPivot.localRotation,
            targetRot,
            Time.deltaTime * rotationStrength
        );
    }


    void Shoot()
    {

    }






}

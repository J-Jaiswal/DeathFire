using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gunRoot;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform aim;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationStrength = 20f;
   private Vector2 maxRotation = new Vector2(20f, 5f);

    //[Header("Pitch Position Settings")]
     private float pitchPositionStrength = 0.0025f;
     private float pitchLerpSpeed = 8f;
     private float maxPitchOffset = 0.015f;

    //[Header("Visual Pitch Pivot")]
    private Transform gunVisualPivot;
    private Quaternion defaultLocalRotation;
    private Quaternion defaultVisualRotation;
    private Vector3 cachedLocalDir;

   

    void Update()
    { 
        LookATMethod();
    }

    void LookATMethod()
    {  
       Vector3 dir = (aim.position - muzzle.position).normalized;
       Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

        gunRoot.rotation = Quaternion.Slerp(
           gunRoot.rotation,
           targetRot,
           Time.deltaTime * rotationStrength
        );
        
    }
    void ApplyYawRotation()
    {
        float yRotation = Mathf.Atan2(cachedLocalDir.x, cachedLocalDir.z) * Mathf.Rad2Deg;
        yRotation = Mathf.Clamp(yRotation, -20f, 20f);

        Quaternion target = Quaternion.Euler(0f, yRotation, 0f) * defaultLocalRotation;

        gunRoot.localRotation = Quaternion.Slerp(
            gunRoot.localRotation,
            target,
            Time.deltaTime * rotationStrength
        );
    }

    void ApplyPitchAlignment()
    {
  
        Vector3 localDir = gunRoot.InverseTransformDirection(
            (aim.position - muzzle.position).normalized
        );

        float pitchAngle = -Mathf.Asin(Mathf.Clamp(localDir.y, -1f, 1f)) * Mathf.Rad2Deg;
        pitchAngle = Mathf.Clamp(pitchAngle, -maxRotation.y, maxRotation.y);

        Quaternion targetRot = Quaternion.Euler(pitchAngle, 0f, 0f) * defaultVisualRotation;

        gunVisualPivot.localRotation = Quaternion.Slerp(
            gunVisualPivot.localRotation,
            targetRot,
            Time.deltaTime * rotationStrength
        );
    }



}
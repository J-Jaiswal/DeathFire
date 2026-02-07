using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 100f;
    [SerializeField] private float lifeTime = 3f;

    [Header("Visuals")]
    [SerializeField] private float meshVisibleTime = 0.02f; 

    private Rigidbody bulletRB;
    private Vector3 bulletDir;

    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;
    AimBehaviour aimController;
    private void Awake()
    {
        bulletRB = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

        aimController = FindAnyObjectByType<AimBehaviour>();
        bulletDir = aimController.GetDirection().normalized;

        // Initial state
        trailRenderer.emitting = false;
        meshRenderer.enabled = false;
    }

    private void Start()
    {
        bulletRB.linearVelocity = bulletDir * speed;

        StartCoroutine(VisualRoutine());

        Destroy(gameObject, lifeTime);
    }

    private IEnumerator VisualRoutine()
    {
        // Small delay so spawn doesn't look broken
        yield return new WaitForSeconds(meshVisibleTime);

        
        trailRenderer.emitting = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Optional: impact VFX here

        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 100f;
    [SerializeField] private float lifeTime = 3f;

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
    }

    private void Start()
    {
        bulletRB.linearVelocity = bulletDir * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
       Vector3 hitpoint = other.ClosestPoint(transform.position);   
        if ( other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(40 ,hitpoint );
                Debug.Log("Enemy took damage: " + 40  + other.gameObject.name);
            }
        Destroy(gameObject);    
        }       

    }

  
}

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
    AimBehaviour aimController;
    private void Awake()
    {
        bulletRB = GetComponent<Rigidbody>();
      
        aimController = FindAnyObjectByType<AimBehaviour>();
        bulletDir = aimController.GetDirection().normalized;
    }

    private void Start()
    {
        bulletRB.linearVelocity = bulletDir * speed;
        Death( lifeTime);
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

   void Death ( float duration)
    {
        Destroy(gameObject , duration);
    }   

}

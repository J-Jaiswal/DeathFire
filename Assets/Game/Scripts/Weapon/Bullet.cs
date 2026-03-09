using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 100f;
     private float lifeTime = 1f;


    private bool hasHit = false;
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
        StartCoroutine(DestroyAfterDelay(lifeTime));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("EnemyHead"))
        {
            hasHit = true;
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(60 , true);

            WaveStatsTracker.Instance.RegisterHeadShot();
            WaveStatsTracker.Instance.RegisterHit();


            Destroy(transform.parent.gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            hasHit = true;
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(40 , false); 
            }
            WaveStatsTracker.Instance.RegisterHit();
            Destroy(transform.parent.gameObject);
        }
    }


    private IEnumerator DestroyAfterDelay(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(transform.parent.gameObject);
        Debug.Log("Bullet destroyed after lifetime expired.");
    }

}

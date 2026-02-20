using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

  
public class EnemyMovement : MonoBehaviour
{
    
    [SerializeField] Transform player;
    public MovementType movementType;


    private float moveSpeed;

    [SerializeField] private float zigzagFrequency = 3f;
    [SerializeField] private float zigzagAmplitude = 2f;
    [SerializeField] private float circleFrequency = 2f;
    [SerializeField] private float circleRadius = 1f;  

    

    public void Initialize (MovementType type, float Speed)
    {
       movementType = type;
        
         moveSpeed = Speed; 
    }   


    void Update()
    {
        if (player == null) return; 
        Vector3 toPlayer = (player.position - transform.position).normalized;

        Vector3 offset = Vector3.zero;

        switch (movementType)
        {

            case MovementType.Straight:
                break;

            case MovementType.ZigZag: 
                offset = transform.right * Mathf.Sin(Time.time * zigzagFrequency) * 
                    zigzagAmplitude; 
                break;


            case MovementType.Circle:
                offset = new Vector3(
                    Mathf.Cos(Time.time * circleFrequency),
                    Mathf.Sin(Time.time * circleFrequency),
                    0f
                ) * circleRadius;
                break;
        }

        Vector3 finalDir = (toPlayer + offset).normalized;
  


        if ( movementType != MovementType.Static)
        {
            transform.position +=  moveSpeed * finalDir * Time.deltaTime;

        }

       ProvideDanage();
    }

    void ProvideDanage()
    {
        if ( Vector3.Distance(transform.position, player.position) < 1f)
        {
            Debug.Log("Enemy provided damage to player and destroyed!");  
            Destroy( this.gameObject);
            // Implement damage logic here, e.g.:
            // player.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
        }   
    }



}

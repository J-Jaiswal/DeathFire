using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

  
public class EnemyMovement : MonoBehaviour
{
    
    [SerializeField] Transform player;
    private MovementType movementType;


    private float moveSpeed;

    [SerializeField] private float zigzagFrequency = 3f;
    [SerializeField] private float zigzagAmplitude = 2f;
    [SerializeField] private float circleFrequency = 2f;
    [SerializeField] private float circleRadius = 1f;  

    //private WaveManager waveManager;  // FIX 4: Cache instead of FindAnyObjectByType every frame



    public void Initialize (MovementType type, float Speed)
    {
       movementType = type;
        
       moveSpeed = Speed; 
    }
    private void Awake()
    {
        //waveManager = FindAnyObjectByType<WaveManager>(); // Cache reference to WaveManager
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

            //waveManager.NotifyEnemyDeath();  
            Debug.Log("Enemy provided damage to player and destroyed!");
            Debug.Log($"[Enemy] 🔄 {gameObject.name} calling EnemyPool.Return()...");
            EnemyPool.Instance.Return(gameObject);
        }   
    }



}

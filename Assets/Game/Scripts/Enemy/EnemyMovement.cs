using UnityEditor;
using UnityEngine;


public enum MovementType
{
    Straight,
    ZigZag,
    Circle
}   
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform player;
    public MovementType movementType;
    [SerializeField] private float moveSpeed = 2f;
  
    [SerializeField] private float zigzagFrequency = 3f;
    [SerializeField] private float zigzagAmplitude = 2f;
    [SerializeField] private float circleFrequency = 2f;
    [SerializeField] private float circleRadius = 1f;    



    void Update()
    {
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

        transform.position +=  moveSpeed * finalDir * Time.deltaTime;
    }

}

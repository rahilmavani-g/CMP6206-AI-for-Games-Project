using UnityEngine;

public class AlienChase : MonoBehaviour
{
    public Transform player;      
    public float speed = 3f;
    public float stopDistance = 1.5f;

    void Update()
    {
        Vector3 toPlayer = player.position - transform.position;   
        toPlayer.y = 0;                                            
        float distance = toPlayer.magnitude;                       

        if (distance > stopDistance)
        {
            Vector3 direction = toPlayer.normalized;                    
            transform.position += direction * speed * Time.deltaTime;  
            transform.forward = direction;                              
        }
    }
}
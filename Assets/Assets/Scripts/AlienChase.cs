using UnityEngine;

public class AlienChase : MonoBehaviour
{
    //variables
    public Transform player;      
    public float speed = 3f;
    public float stopDistance = 1.5f;

    public float viewDistance = 10.0f;
    public float viewAngle = 90.0f;
    bool canSee;

    void Update()
    {
        Vector3 toPlayer = player.position - transform.position;   
        toPlayer.y = 0;                                            
        float distance = toPlayer.magnitude;

        canSee = CanSeePlayer();
        if(canSee)
        {
            if (distance > stopDistance)
            {
                Vector3 direction = toPlayer.normalized;
                transform.position += direction * speed * Time.deltaTime;
                transform.forward = direction;
            }
        }
        
    }

    bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;
        float distance = toPlayer.magnitude;

        float angletoPlayer = Vector3.Angle(transform.forward, toPlayer);
        //Debug.Log("Angle : " + angletoPlayer);

        
        if (distance > viewDistance)
        {
            return false;
        }
        if (angletoPlayer >  viewAngle / 2f)
        {
            return false;
        }
        if (Physics.Linecast(transform.position, player.position, out RaycastHit hit))
        {
            if (!hit.transform.CompareTag("Player"))
            {
                return false;
            }
        }

        return true;
    }

    void OnDrawGizmos()
    {
        if (canSee)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        Vector3 start = transform.position;
        Vector3 end = transform.position + transform.forward * viewDistance;
        Gizmos.DrawLine(start, end);

        Vector3 leftDir = Quaternion.AngleAxis(-viewAngle / 2f, Vector3.up) * transform.forward;
        Vector3 leftEnd = transform.position + leftDir * viewDistance;
        Gizmos.DrawLine(start, leftEnd);

        Vector3 rightDir = Quaternion.AngleAxis(viewAngle / 2f, Vector3.up) * transform.forward;
        Vector3 rightEnd = transform.position + rightDir * viewDistance;
        Gizmos.DrawLine(start, rightEnd);
    }
}
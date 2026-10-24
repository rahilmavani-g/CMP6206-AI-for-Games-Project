using UnityEngine;

public class AlienChase : MonoBehaviour
{
    //Enum + switch
    enum AlienState { Patrol, Chase, Search }
    AlienState state = AlienState.Patrol;

    //variables
    public Transform player;
    public float speed = 3f;
    public float stopDistance = 1.5f;

    public float viewDistance = 10.0f;
    public float viewAngle = 90.0f;
    bool canSee;

    //search timer
    float searchTimer;
    public float searchTime = 4f;

    Vector3 lastKnownPosition;

    //patrol points pos
    public Transform[] patrolPoints;
    int patrolIndex = 0;

    private void Start()
    {
        lastKnownPosition = transform.position;
    }

    //Calculating distance to points/player and chase, patrol, search every frame condition/case
    void Update()
    {
        canSee = CanSeePlayer();

        if (canSee)
        {
            lastKnownPosition = player.position;
        }

        switch (state)
        {
            //Patrol between points
            case AlienState.Patrol:

               bool patrolArrived =  MoveTo(patrolPoints[patrolIndex].position);

                if (patrolArrived)
                {
                    patrolIndex++;
                }

                if(patrolIndex == patrolPoints.Length)
                {
                    patrolIndex = 0;
                }

                if (canSee)
                {
                    state = AlienState.Chase;
                }
                break;

                //Chase the player
            case AlienState.Chase:

                bool arrived = MoveTo(lastKnownPosition);

                if (arrived && !canSee)
                {
                    state = AlienState.Search;
                    searchTimer = searchTime;
                }
                break;

                //Search by rotating
            case AlienState.Search:

                searchTimer -= Time.deltaTime;
                transform.Rotate(0, 90f * Time.deltaTime, 0);
                if(canSee)
                {
                    state = AlienState.Chase;
                }
                else
                {
                    if(searchTimer <= 0)
                    {
                        state = AlienState.Patrol;
                    }
                }
                break;
        }

        //Logs
        Debug.Log(state);
    }

    //Adding memory
    bool MoveTo(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0;
        float distance = toTarget.magnitude;

        if (distance > stopDistance)
        {
            Vector3 direction = toTarget.normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.forward = direction;
            return false;
        }
        return true;
    }

    //Adding distance, angle and raycast as if conditions
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
        if (angletoPlayer > viewAngle / 2f)
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

    //Drawing gizmos
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
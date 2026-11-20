using UnityEngine;
using System.Collections.Generic;

public class AlienChase : MonoBehaviour
{
    //Enum + switch
    enum AlienState { Patrol, Chase, Search }
    AlienState state = AlienState.Patrol;

    //variables
    public Transform player;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

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

    //ref to pathfinding
    public Pathfinding pathfinding;

    public LayerMask wallMask;

    public float bodyRadius = 0.5f;

    //Animator
    public Animator animator;

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

                animator.SetFloat("Speed", 0.5f);     
                animator.SetBool("Searching", false);

                bool patrolArrived =  MoveTo(patrolPoints[patrolIndex].position, patrolSpeed);

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

                animator.SetFloat("Speed", 1.0f);
                animator.SetBool("Searching", false);

                bool arrived = MoveTo(lastKnownPosition, chaseSpeed);

                if (arrived && !canSee)
                {
                    state = AlienState.Search;
                    searchTimer = searchTime;
                }

                if (arrived)
                {
                    animator.SetFloat("Speed", 0f);
                }
                else
                {
                    animator.SetFloat("Speed", 1f);
                }

                break;

                //Search by rotating
            case AlienState.Search:

                animator.SetFloat("Speed", 0.0f);
                animator.SetBool("Searching", true);

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

    //Walk to target along the A* path (with raycast smoothing)
    bool MoveTo(Vector3 target, float moveSpeed)
    {
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0;
        if (toTarget.magnitude <= stopDistance)
        {
            return true;
        }

        List<Node> path = pathfinding.FindPath(transform.position, target);

        if (path == null || path.Count == 0)
        {
            return false;
        }

        Vector3 nextStep = path[0].worldPosition;

        //Smoothing & AgentBody
        for (int i = path.Count - 1; i >= 0; i--)
        {
            Vector3 candidate = path[i].worldPosition;
            candidate.y = transform.position.y;

            Vector3 toCandidate = candidate - transform.position;   
            float dist = toCandidate.magnitude;                     

            if (!Physics.SphereCast(transform.position, bodyRadius, toCandidate.normalized, out RaycastHit hit, dist, wallMask))
            {
                nextStep = candidate;
                break;
            }
        }

        Vector3 toStep = nextStep - transform.position;
        toStep.y = 0;

        Vector3 direction = toStep.normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.forward = direction;
        return false;

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private PlayerMovement player;
    private NavMeshAgent navMeshAgent;
    private Vector3 initialPosition;
    private Vector3 pacingTargetOne;
    private Vector3 pacingTargetTwo;
    private Vector3 currentPaceTarget;
    private float timer;

    public enum MovementTypes { Pacing, Wander, Idle, Chase}
    [SerializeField] private MovementTypes movementType;
    [SerializeField] private float pacingDistance = 3f;
    [SerializeField] private float wanderRadius = 6f;
    [SerializeField] private float wanderTimer = 3f;
    [SerializeField] private LayerMask layerMask;
    private MovementTypes previousMovementType;
    [SerializeField] private float detectionRadius = 16f;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
        pacingTargetOne = transform.forward * pacingDistance + transform.position;
        pacingTargetTwo = transform.forward * -pacingDistance + transform.position;
    }

    private void Start()
    {
        if (movementType == MovementTypes.Pacing)
        {
            navMeshAgent.SetDestination(pacingTargetOne);
            currentPaceTarget = pacingTargetOne;
        }
    }

    public void ChangeMovementType(MovementTypes newMovementType)
    {
        previousMovementType = movementType;
        movementType = newMovementType;
    }

    private void CheckForPlayerDistance()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < detectionRadius && movementType != MovementTypes.Chase)
        {
            ChangeMovementType(MovementTypes.Chase);
        }

        else if (movementType == MovementTypes.Chase)
        {
            ChangeMovementType(previousMovementType);
        }
    }

    private void AIMovement()
    {
        if (movementType == MovementTypes.Pacing)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (currentPaceTarget == pacingTargetOne)
                {
                    navMeshAgent.SetDestination(pacingTargetTwo);
                    currentPaceTarget = pacingTargetTwo;
                }

                else
                {
                    navMeshAgent.SetDestination(pacingTargetOne);
                    currentPaceTarget = pacingTargetOne;
                }
            }
        }

        if (movementType == MovementTypes.Wander)
        {
            if (timer >= wanderTimer)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += initialPosition;
                NavMeshHit navHit;
                NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, layerMask);
                navMeshAgent.SetDestination(navHit.position);
                timer = 0f;
            }
        }

        if (movementType == MovementTypes.Chase)
        {
            if (timer >= 0.25f)
            {
                navMeshAgent.SetDestination(player.transform.position);
                timer = 0f;
            }
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        CheckForPlayerDistance();
        AIMovement();
    }
}

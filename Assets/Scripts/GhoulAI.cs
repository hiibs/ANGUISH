using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhoulAI : MonoBehaviour
{
    [SerializeField] private EnemyWaypoint waypoint;

    private NavMeshAgent agent;
    private bool waypointReached = false;


    private void Start() {
        agent = GetComponent<NavMeshAgent>();

        if (waypoint != null)
            agent.destination = waypoint.transform.position;

    }

    private void Update() {
        if ((agent.destination - transform.position).magnitude < 1f) {
            waypointReached = true;
            agent.isStopped = true;
        }

        if (waypointReached && waypoint.next != null) {
            waypoint = waypoint.next;
            agent.destination = waypoint.transform.position;
            waypointReached = false;
            agent.isStopped = false;
        }

    }

}

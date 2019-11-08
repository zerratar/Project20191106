using UnityEngine;
using UnityEngine.AI;

public abstract class UnitController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;

    public void Awake()
    {
        if (!navMeshAgent) navMeshAgent = GetComponent<NavMeshAgent>();
        //if (!navMeshAgent) return;
        //velocity = navMeshAgent.velocity;
    }

    public void StopMoving()
    {
        if (!navMeshAgent.enabled) return;
        navMeshAgent.SetDestination(transform.position);
        //navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.enabled = false;
    }

    public void MoveTo(Vector3 point)
    {
        navMeshAgent.enabled = true;
        //navMeshAgent.velocity = velocity;
        navMeshAgent.SetDestination(point);
    }
}
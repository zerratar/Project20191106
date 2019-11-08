using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WorkerUnit : UnitController
{
    [SerializeField] private float minResourceGatherDistance = 4f;
    [SerializeField] private float minResourceDeliveryDistance = 20f;
    [SerializeField] private int maxResourceCapacity = 10;

    private ResourceController resourceGatherTarget;
    private float generateResourceTimer;

    public long ResourcesHold;
    private PlayerController player;
    private MotherBase[] motherBases;
    private MotherBase targetMotherBase;

    // Update is called once per frame
    void Update()
    {
        if (resourceGatherTarget)
        {

            if (ResourcesHold >= maxResourceCapacity)
            {
                UpdateResourceDelivery();
                return;
            }

            if (CanReachTarget())
            {
                StopMoving();
                UpdateResourceGathering();
            }
        }
    }

    private void UpdateResourceDelivery()
    {
        var targetMotherBase = GetNearestMotherBase();
        if (!targetMotherBase) return;

        if (!CanReachTarget(targetMotherBase.transform, minResourceDeliveryDistance))
        {
            MoveTo(targetMotherBase.transform.position);
            return;
        }

        player.Money += ResourcesHold;
        ResourcesHold = 0;

        UpdateResourceGathering();
    }

    private MotherBase GetNearestMotherBase()
    {
        if (targetMotherBase) return targetMotherBase;
        if (!player) player = transform.root.GetComponent<PlayerController>();
        if (motherBases == null || motherBases.Length == 0) motherBases = player.GetComponentsInChildren<MotherBase>();
        targetMotherBase = motherBases.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault();
        return targetMotherBase;
    }

    private void UpdateResourceGathering()
    {
        if (!resourceGatherTarget)
        {
            return;
        }

        if (!CanReachTarget())
        {
            MoveTo(resourceGatherTarget.transform.position);
            return;
        }

        generateResourceTimer -= Time.deltaTime;
        if (generateResourceTimer <= 0f && --resourceGatherTarget.ResourceLimit >= 0)
        {
            ++ResourcesHold;
            generateResourceTimer = resourceGatherTarget.ResourceGenerationTime;
            Debug.Log("Resource gained!");
        }
        else if (resourceGatherTarget.ResourceLimit <= 0)
        {
            // find all resources, find closest with resources in it. Go to it.
        }
    }

    internal void GatherResources(ResourceController resource)
    {
        resourceGatherTarget = resource;
        // in case supply NULL as argument to clear current action
        if (resource)
        {
            generateResourceTimer = resourceGatherTarget.ResourceGenerationTime;
            MoveTo(resource.transform.position);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CanReachTarget()
    {
        return CanReachTarget(resourceGatherTarget.transform, minResourceGatherDistance);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CanReachTarget(Transform target, float dist)
    {
        return Vector3.Distance(transform.position, target.position) <= dist;
    }
}

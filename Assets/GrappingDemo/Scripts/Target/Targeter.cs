using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Targeter : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup cinemachineTargetGroup;
    private List<Target> targets = new List<Target>();
    private const float MinDistance = 2.0f;
    private const float AllowedDistance = 10.0f;
    private const float MaxDistance = 15.0f;
    public Target CurrentTarget { get; private set; }
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (targets.Count == 0) { return; }

        Target closestTarget = null;
        float closestTargetDistance = Mathf.Infinity;

        foreach (Target target in targets)
        {
                        
            MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;

            float distance = Vector3.Distance(target.transform.position, cinemachineTargetGroup.transform.position);
            if (distance is < MinDistance or > MaxDistance)
            {
                continue;
            }
            
            Vector2 positionVS = mainCamera.WorldToViewportPoint(target.transform.position);
            if (positionVS.x < 0 || positionVS.x > 1 || positionVS.x < 0 || positionVS.x > 1)
            {
                continue;
            }

            Vector2 toCenter = positionVS - new Vector2(0.5f, 0.5f);
            if (toCenter.sqrMagnitude < closestTargetDistance)
            {
                closestTarget = target;
                closestTargetDistance = toCenter.sqrMagnitude;
            }
        }

        if (closestTarget == null)
        {
            return;
        }
        
        float closestDistance = Vector3.Distance(closestTarget.transform.position, cinemachineTargetGroup.transform.position);
        MeshRenderer closestTargetMeshRenderer = closestTarget.GetComponent<MeshRenderer>();
        closestTargetMeshRenderer.enabled = true;
        Material closestTargetMaterial = closestTargetMeshRenderer.material;
        if (closestDistance < AllowedDistance)
        {
            closestTargetMaterial.color = Color.green;
        }
        else
        {
            closestTargetMaterial.color = Color.gray;
        }
        

    }

    private void OnBecameInvisible()
    {
        enabled = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target))
        {
            return;
        }
        targets.Add(target);
        target.onDestoryed += RemoveTarget;

        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target))
        {
            return;
        }
        RemoveTarget(target);
    }

    private void RemoveTarget(Target target)
    {
        if (CurrentTarget == target)
        {
            cinemachineTargetGroup.RemoveMember(CurrentTarget.transform);
            CurrentTarget = null;
        }
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        target.onDestoryed -= RemoveTarget;
        targets.Remove(target);
    }

    public bool SelectTarget()
    {
        if (targets.Count == 0) { return false; }

        Target closestTarget = null;
        float closestTargetDistance = Mathf.Infinity;

        foreach (Target target in targets)
        {
            // if (target.transform.position.y < cinemachineTargetGroup.transform.position.y)
            // {
            //     continue;
            // }
            
            float distance = Vector3.Distance(target.transform.position, cinemachineTargetGroup.transform.position);
            if (distance is < MinDistance or > AllowedDistance)
            {
                continue;
            }
            
            Vector2 positionVS = mainCamera.WorldToViewportPoint(target.transform.position);
            if (positionVS.x < 0 || positionVS.x > 1 || positionVS.x < 0 || positionVS.x > 1)
            {
                continue;
            }
            
            Vector2 toCenter = positionVS - new Vector2(0.5f, 0.5f);
            if (toCenter.sqrMagnitude < closestTargetDistance)
            {
                closestTarget = target;
                closestTargetDistance = toCenter.sqrMagnitude;
            }
        }

        if (closestTarget == null)
        {
            return false;
        }

        CurrentTarget = closestTarget;
        cinemachineTargetGroup.AddMember(CurrentTarget.transform, 1f, 2f);
        return true;
    }

    public void Cancel()
    {
        if (CurrentTarget == null)
        {
            return;
        }
        cinemachineTargetGroup.RemoveMember(CurrentTarget.transform);
        CurrentTarget = null;
    }
    
}

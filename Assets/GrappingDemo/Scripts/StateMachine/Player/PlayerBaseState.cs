using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        this.stateMachine = playerStateMachine;
    }

    protected void Move(Vector3 motion, float deltaTime)
    {
        Vector3 movement = (motion + stateMachine.ForceReceiver.Movement) * deltaTime;
        stateMachine.CharacterController.Move(movement);
    }

    protected void FaceTarget()
    {
        Target currentTarget = stateMachine.Targeter.CurrentTarget;
        if (currentTarget == null)
        {
            return;
        }

        Vector3 faceDirection = currentTarget.transform.position - stateMachine.CharacterController.transform.position;
        faceDirection.y = 0f;

        stateMachine.transform.rotation = Quaternion.LookRotation(faceDirection);
    }

    protected void ReturnToLocomotion()
    {
        if (stateMachine.Targeter.CurrentTarget != null)
        {
            stateMachine.SwtichState(new PlayerFreeLookState(stateMachine));
        }
        else
        {
            stateMachine.SwtichState(new PlayerTargetState(stateMachine));
        }
    }
}

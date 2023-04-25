using UnityEngine;

public class PlayerGrapplingState : PlayerBaseState
{
    private bool isAppliedForce = false;
    public PlayerGrapplingState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void Enter()
    {
        stateMachine.InputManager.GrapplingEvent += OnGrappling;
        stateMachine.InputManager.JumpEvent += OnJump;
        stateMachine.LineRenderer.enabled = true;
        stateMachine.LineRenderer.SetPosition(1,  stateMachine.Targeter.CurrentTarget.transform.position);
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Targeter.CurrentTarget == null)
        {
            stateMachine.SwtichState(new PlayerFreeLookState(stateMachine));
            return;
        }
        Vector3 currentPos = stateMachine.CharacterController.transform.position;
        var transform = stateMachine.Targeter.CurrentTarget.transform;
        var position = transform.position;
        Vector3 targetPos = position;
        
        FaceTarget();
        
        currentPos += Vector3.up * 1.2f;
        stateMachine.LineRenderer.SetPosition(0, currentPos);
        
        Vector3 movement = CalculateGrapplingVelocity(currentPos, targetPos);
        Vector3 force = movement * stateMachine.GrapplingForce * 1.2f;
        TryApplyForce(force);
    }

    public override void Exit()
    {
        stateMachine.InputManager.GrapplingEvent -= OnGrappling;
        stateMachine.InputManager.JumpEvent -= OnJump;
    }

    private void OnJump()
    {
        stateMachine.SwtichState(new PlayerJumpState(stateMachine));
    }

    private void OnGrappling()
    {
        if (!stateMachine.Targeter.SelectTarget())
        {
            return;
        }
        stateMachine.SwtichState(new PlayerGrapplingState(stateMachine));
    }

    private void CancelGrappling()
    {
        stateMachine.LineRenderer.enabled = false;
        stateMachine.Targeter.Cancel();
        stateMachine.SwtichState(new PlayerFreeLookState(stateMachine));
    }

    private void TryApplyForce(Vector3 force)
    {
        if (isAppliedForce)
        {
            CancelGrappling();
        }
        stateMachine.ForceReceiver.AddForce(force);
        isAppliedForce = true;
    }

    /*
     * https://www.omnicalculator.com/physics/trajectory-projectile-motion
     * Trajectory formula:
     * y = h + xtan(α) - gx²/2V₀²cos²(α)
     */
    private Vector3 CalculateGrapplingVelocity(Vector3 currentPos, Vector3 targetPos)
    {
        float gravity = Physics.gravity.y;
        float displacementY = targetPos.y - currentPos.y;
        Vector3 displacementXZ = new Vector3(targetPos.x - currentPos.x, 0f, targetPos.z - currentPos.z);

        float trajectoryHeight = CalculateTrajectoryHeight(currentPos, targetPos);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
                                               + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private float CalculateTrajectoryHeight(Vector3 currentPos, Vector3 targetPos)
    {
        float displacementY;
        float trajectoryHeight;
        if (targetPos.y >= currentPos.y)
        {
            displacementY = targetPos.y - currentPos.y;
            trajectoryHeight = displacementY * 1.5f;
        }
        else
        {
            displacementY = currentPos.y - targetPos.y;
            trajectoryHeight = displacementY;
        }
        return trajectoryHeight;
    } 

}

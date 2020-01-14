using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPhysics : PhysicsComponent
{
    [SerializeField]
    [Tooltip("Should be a positive value because it is just the raw force applied.")]
    private float gravityForce = 9.81f;

    [SerializeField]
    [Tooltip("The object is unable to fall faster than this.")]
    private float maxGravityForce = 3f;

    private float fallTime = 0f;

    //These will be changed by public functions from other classes.
    private bool gravityEnabled = true;
    private bool disabledForTime = false;
    private float timeUntilGravityEnabled = 0f;
    private float _timeUntilGravityEnabled = 0f;

    public override Vector3 ApplyPhysics(Vector3 currentPosition)
    {
        if (!CheckIfGravityIsApplied()) return currentPosition;

        if (!PhysicsObject.collisionInfo.grounded)
        {
            fallTime += Time.deltaTime;
        }
        else
        {
            fallTime = 0f;
        }

        currentPosition += Vector3.up * Mathf.Max(-gravityForce * Mathf.Pow(fallTime, 2) * Time.deltaTime, -maxGravityForce);

        return currentPosition;
    }
    private bool CheckIfGravityIsApplied()
    {
        if (gravityEnabled) return true;

        if(timeUntilGravityEnabled > 0f && _timeUntilGravityEnabled < timeUntilGravityEnabled)
        {
            _timeUntilGravityEnabled += Time.deltaTime;
            return false;
        } else if (disabledForTime)
        {
            ResetGravityParameteresToEnabled();
            return true;
        }

        return false;

    }
    private void ResetGravityParameteresToEnabled()
    {
        _timeUntilGravityEnabled = 0f;
        timeUntilGravityEnabled = 0f;
        fallTime = 0f;
        disabledForTime = false;
        gravityEnabled = true;
    }

    public void EnableGravity()
    {
        ResetGravityParameteresToEnabled();
    }
    public void DisableGravity()
    {
        gravityEnabled = false;
    }
    public void DisableGravity(float duration)
    {
        gravityEnabled = false;
        timeUntilGravityEnabled = duration;
        disabledForTime = true;
    }
    public bool IsEnabled()
    {
        return gravityEnabled;
    }

    protected override void OnModifiersChanged(PhysicsModifier physicsModifier, bool added)
    {
    }
}

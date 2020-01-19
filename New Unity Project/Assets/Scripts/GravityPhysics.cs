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

    [SerializeField]
    private bool startWithGravEnabled = true;
    [SerializeField]
    private bool gravitySideways = false;
    [SerializeField]
    private bool startWithGravFlipped = false;

    [SerializeField]
    private Material upGravityMaterial;

    private Material downGravityMaterial;
    private MeshRenderer renderer;

    private float fallTime = 0f;

    private Vector3 gravityDirection = Vector3.down;

    //These will be changed by public functions from other classes.
    private bool gravityEnabled = true;
    private bool disabledForTime = false;
    private float timeUntilGravityEnabled = 0f;
    private float _timeUntilGravityEnabled = 0f;

    private void Start()
    {
        gravityEnabled = startWithGravEnabled;
        renderer = PhysicsObject.body.GetComponent<MeshRenderer>();
        downGravityMaterial = renderer.material;

        if(gravitySideways)
        {
            gravityDirection = Vector3.back;
        }

        if (startWithGravFlipped) FlipGravity();
        else PhysicsObject.ChangeGravityDirection(gravityDirection);
    }

    public override Vector3 ApplyPhysics(Vector3 currentPosition)
    {
        if (!CheckIfGravityIsApplied())
        {
            return currentPosition;
        }

        if (PhysicsObject.collisionInfo.grounded || PhysicsObject.collisionInfo.collidingWithPlayer)
        {
            fallTime = 0f;
        }
        else
        {
            fallTime += Time.deltaTime;
        }

        currentPosition += -gravityDirection * Mathf.Max(-gravityForce * Mathf.Pow(fallTime, 2) * Time.deltaTime, -maxGravityForce);

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

    public void FlipGravity()
    {
        gravityDirection *= -1;
        fallTime = 0f;
        PhysicsObject.ChangeGravityDirection(gravityDirection);
        if(Mathf.Sign(gravityDirection.x + gravityDirection.y + gravityDirection.z) < 0)
        {
            renderer.material = downGravityMaterial;
        } else
        {
            renderer.material = upGravityMaterial;
        }
    }

    protected override void OnModifiersChanged(PhysicsModifier physicsModifier, bool added)
    {
    }
}

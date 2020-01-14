using UnityEngine;

public abstract class ControllerBase : PhysicsComponent
{
    [SerializeField]
    private float speed = 1f;

    public float Speed => speed * modifier;

    private float modifier
    {
        get
        {
            float value = 1f;
            physicsModifiers.ForEach(m => value += m.Value);
            return value;
        }
    }

    public override abstract Vector3 ApplyPhysics(Vector3 currentPosition);

    protected override void OnModifiersChanged(PhysicsModifier physicsModifier, bool added)
    {
        //modifier += physicsModifier.Value * (added ? 1 : -1);
    }
}
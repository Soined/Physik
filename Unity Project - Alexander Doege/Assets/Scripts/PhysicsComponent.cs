using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhysicsObject))]
public abstract class PhysicsComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("higher order => will be executed later")]
    private int order = 0;

    protected PhysicsObject PhysicsObject { get; private set; }

    public int ExecutionOrder => order;

    protected List<PhysicsModifier> physicsModifiers = new List<PhysicsModifier>();

    private void Awake()
    {
        PhysicsObject = GetComponent<PhysicsObject>();

        PhysicsObject.RegisterComponent(this);
    }

    private void OnDestroy()
    {
        PhysicsObject.UnregisterComponent(this);
    }

    /// <summary>
    /// This function must return the currentPosition after applying the physics of this Component.
    /// Collision will be calculated after all Components are applied.
    /// </summary>
    /// <param name="currentPosition">The position in the current Frame after applying other modifiers with a higher order.</param>
    /// <returns></returns>
    public abstract Vector3 ApplyPhysics(Vector3 currentPosition);

    public void RegisterModifier(PhysicsModifier physicsModifier)
    {
        physicsModifiers.Add(physicsModifier);
        OnModifiersChanged(physicsModifier, true);
    }
    public void UnregisterModifier(PhysicsModifier physicsModifier)
    {
        physicsModifiers.Remove(physicsModifier);
        OnModifiersChanged(physicsModifier, false);
    }
    /// <summary>
    /// Will be called whenever a new PhysicsModifier is added or removed from this physics object.
    /// </summary>
    /// <param name="physicsModifier"> The Modifier that just got added or removed from the object</param>
    /// <param name="added"> Returns true if the modifier just got added, returns false if it got removed</param>
    protected abstract void OnModifiersChanged(PhysicsModifier physicsModifier, bool added);
}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MathExtensionZ;

public class PhysicsObject : MonoBehaviour
{
    private enum ColliderType
    {
        Sphere,
        Capsule,
        Box
    }

    public GameObject body;

    private ColliderType colType = ColliderType.Capsule;

    public int collisionChecksPerFrame = 3;

    private Collider col;

    public CollisionInfo collisionInfo;

    private Vector3 gravityDirection = Vector3.down;

    private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();

    void Start()
    {
        col = body.GetComponent<Collider>();

        if (col.GetType() == typeof(CapsuleCollider))
        {
            colType = ColliderType.Capsule;
        }
        else if (col.GetType() == typeof(BoxCollider))
        {
            colType = ColliderType.Box;
        }
        else
        {
            colType = ColliderType.Sphere;
        }
    }

    public void RegisterComponent(PhysicsComponent physicsComponent)
    {
        physicsComponents.Add(physicsComponent);
        physicsComponents = physicsComponents.OrderBy(comp => comp.ExecutionOrder).ToList();
    }
    public void UnregisterComponent(PhysicsComponent physicsComponent)
    {
        physicsComponents.Remove(physicsComponent);
    }

    private void Update()
    {
        ApplyPhysics();
    }

    public void ApplyPhysics()
    {
        //we need to do this before passing our position to the physics Components
        Vector3 newPosition = transform.position;

        foreach (var physicsComponent in physicsComponents)
        {
            newPosition = physicsComponent.ApplyPhysics(newPosition);
        }

        //This will reset all collisionInfo from the last frame right before we check for new collisionInfos in CheckCollision()
        collisionInfo.ResetCollisionInfo();

        //This will try to move us towards the newPosition calculated from all physicsComponents.
        //If we happen to find a non-Trigger collider on our way, we will stop right in front of it.
        CheckCollision(ref newPosition);

        //This is where we actually apply our new position
        transform.position = newPosition;
    }

    private void CheckCollision(ref Vector3 position)
    {
        switch(colType)
        {
            case ColliderType.Capsule:
                CheckCapsuleCollision(ref position);
                break;
            case ColliderType.Box:
                CheckBoxCollision(ref position);
                break;
            case ColliderType.Sphere:
                CheckSphereCollision(ref position);
                break;
        }
    }

    #region CollisionTypes

    private void CheckCapsuleCollision(ref Vector3 position)
    {
        CapsuleCollider capCol = col as CapsuleCollider;

        Vector3 direction = position - transform.position;

        
        //We cast only more than once if we move in a single frame more than our radius.
        //This is to prevent us teleporting through a collider in case we move too fast.
        int numberToCast = (int)(direction.magnitude / capCol.radius) + 1;

        //Debug.Log($"numberToCast: {numberToCast}");

        for (int i = 0; i < numberToCast; i++)
        {
            //if our movement in this frame is larger than our Radius, we will check for collision through our moveVector based on our Radius.
            Vector3 positionToCheck = transform.position + ((direction / numberToCast) * (i + 1));
            bool hitCollider = false;
            Vector3 justChecked = CheckAtPositionCapsule(positionToCheck, ref hitCollider, capCol);

            if (hitCollider) //Will only be called if we hit a non-Trigger collider which also isn't attached on this object
            {
                position = justChecked;
                break;
                //So if we hit a collider at our MoveVector we do not go further after pushing us out of it.
            }
        } //If we didn't find a collider at all we do not change the position we want to go to
    }

    private Vector3 CheckAtPositionCapsule(Vector3 toCheckAt, ref bool hitCollider, CapsuleCollider capCol)
    {
        for (int i = 0; i < collisionChecksPerFrame; i++)
        {
            Vector3 point1 = new Vector3(toCheckAt.x, toCheckAt.y - ((capCol.height / 2) - capCol.radius), toCheckAt.z);
            Vector3 point2 = new Vector3(toCheckAt.x, toCheckAt.y + ((capCol.height / 2) - capCol.radius), toCheckAt.z);

            var collidingObjects = Physics.OverlapCapsule(point1, point2, capCol.radius);

            //Just a potential future bugfix if we wouldn't find our own collider anymore to save performance
            if (collidingObjects.Length == 0) break;

            foreach (var collider in collidingObjects)
            {
                //If we only find only our own collider there is no reason to keep going for this position
                if (collider == col && collidingObjects.Length == 1) break;
                //we will also find our own collider or triggers in which case we just skip it
                if (collider.isTrigger || collider == col) continue;

                /* We take the line from the top of our collider to the very bottom through the exact middle.
                 * After we calculate the closest point on our line to the closest point on the object we are colliding with
                 * to push this object away from the other object by the vector of those 2 points. */
                Vector3 closestPointOnOther = collider.ClosestPoint(toCheckAt);
                Vector3 pointOnUs = MathZ.FindNearestPointOnLine(point1, point2, closestPointOnOther);
                Vector3 collisionVector = closestPointOnOther - pointOnUs;

                //After this we need to calculate the distance we need to be pushed so our collider will be outside of the other collider in the next Frame.
                Vector3 pushVector = collisionVector - (capCol.radius * collisionVector.normalized);
                toCheckAt += pushVector; //We apply the pushVector to our Position in the next Frame. Because we might collide with more objects and need to do additional pushing.

                hitCollider = true; // At this state we only know we hit anything. Could be optimized by returning the pushVector so we know where we hit it.
            }
        }
        Ray groundRay = new Ray(toCheckAt, gravityDirection);
        Ray topRay = new Ray(toCheckAt, -gravityDirection);
        //Note: Do better ground collision checking later (standing on an edge won't count as being grounded
        collisionInfo.grounded = Physics.Raycast(groundRay, out RaycastHit hit, capCol.height / 2 + .01f);
        collisionInfo.collisionOnTop = Physics.Raycast(topRay, out RaycastHit hit2, capCol.height / 2 + .01f);
        if (collisionInfo.grounded) collisionInfo.standingOn = hit.collider.gameObject;

        return toCheckAt;
    }

    private void CheckBoxCollision(ref Vector3 position)
    {
        BoxCollider boxCol = col as BoxCollider;

        Vector3 moveVector = position - transform.position;

        //Boxcollider.size gives all x y and z size.

        //We do this so we always check at the most frequent way necessary for any side.
        //This is at the cost of performance could be optimized by only checking this if we actually move into this direction.
        float minSize = Mathf.Min(boxCol.size.x, boxCol.size.y, boxCol.size.z);

        int numberToCast = Mathf.FloorToInt(moveVector.magnitude / minSize) + 1;

        for (int i = 0; i < numberToCast; i++)
        {
            Vector3 positionToCheck = transform.position + ((moveVector / numberToCast) * (i + 1));
            bool hitCollider = false;
            Vector3 justChecked = CheckAtPositionBox(positionToCheck, ref hitCollider, boxCol);

            if (hitCollider) //Will only be called if we hit a non-Trigger collider which also isn't attached on this object
            {
                position = justChecked;
                break;
            }
        } //If we didn't find a collider at all we do not change the position we want to go to
    }

    private Vector3 CheckAtPositionBox(Vector3 toCheckAt, ref bool hitCollider, BoxCollider boxCol)
    {
        for (int i = 0; i < collisionChecksPerFrame; i++)
        {
            var collidingObjects = Physics.OverlapBox(toCheckAt, boxCol.bounds.extents, transform.rotation);

            //Just a potential future bugfix if we wouldn't find out own collider anymore to save performance
            if (collidingObjects.Length == 0) break;

            foreach (var collider in collidingObjects)
            {
                //If we only find only our own collider there is no reason to keep going for this position
                if (collider == col && collidingObjects.Length == 1) break;
                //we will also find our own collider or triggers in which case we just skip it
                //|| collider.tag == "Player"
                if (collider.isTrigger || collider == col ) continue;

                if(collider.tag == "Player")
                {
                    collisionInfo.collidingWithPlayer = true;
                }

                Vector3 closestPointOnOther = collider.ClosestPointOnBounds(toCheckAt);
                //we need to do this because we haven't actually moved so we the boxColliders position is still wrong.
                Vector3 collisionDirection = (closestPointOnOther - toCheckAt).normalized;
                Vector3 pushDirection = MathZ.GetMainDirectionForBox(boxCol, collisionDirection);
                Vector3 pointOnCollider = MathZ.FindNearestPointOnBoxBounds(boxCol, toCheckAt, closestPointOnOther, pushDirection);

                Vector3 pushVector = pushDirection * (pointOnCollider - closestPointOnOther).magnitude;

                toCheckAt += pushVector; // we actually push us out of the other collider with this.

                //We tell our script to not check for any remaining  OverlapBoxes because we already hit something.
                hitCollider = true;
            }
        }
        Ray groundRay = new Ray(toCheckAt, gravityDirection);
        Ray topRay = new Ray(toCheckAt, -gravityDirection);

        //Note: Do better ground collision checking later (standing at an edge won't count as being grounded
        collisionInfo.grounded = Physics.Raycast(groundRay, out RaycastHit hit, boxCol.bounds.extents.y + .01f);
        collisionInfo.collisionOnTop = Physics.Raycast(topRay, out RaycastHit hit2, boxCol.bounds.extents.y + .01f);
        if(collisionInfo.grounded) collisionInfo.standingOn = hit.collider.gameObject;

        //Debug.Log($"raylength: {((boxCol.bounds.min.y + toCheckAt.y - transform.position.y)) + .01f}");
        return toCheckAt;
    }



    private void CheckSphereCollision(ref Vector3 position)
    {
        SphereCollider sphereCol = col as SphereCollider;

        Vector3 direction = position - transform.position;

        int numberToCast = (int)(direction.magnitude / sphereCol.radius) + 1;

        for (int i = 0; i < numberToCast; i++)
        {
            Vector3 positionToCheck = transform.position + ((direction / numberToCast) * (i + 1));
            bool hitCollider = false;
            Vector3 justChecked = CheckAtPositionSphere(positionToCheck, ref hitCollider, sphereCol);

            if (hitCollider) //Will only be called if we hit a non-Trigger collider which also isn't attached on this object
            {
                position = justChecked;
                break;
            }
        }
    }
    private Vector3 CheckAtPositionSphere(Vector3 toCheckAt, ref bool hitCollider, SphereCollider sphereCol)
    {
        for (int i = 0; i < collisionChecksPerFrame; i++)
        {

            var collidingObjects = Physics.OverlapSphere(toCheckAt, sphereCol.radius);

            //Just a potential future bugfix if we wouldn't find out own collider anymore to save performance
            if (collidingObjects.Length == 0) break;

            foreach (var collider in collidingObjects)
            {
                //If we only find only our own collider there is no reason to keep going for this position
                if (collider == col && collidingObjects.Length == 1) break;
                //we will also find our own collider or triggers in which case we just skip it
                if (collider.isTrigger || collider == col) continue;

                Vector3 closestPoint = collider.ClosestPoint(toCheckAt);

                Vector3 collisionVector = closestPoint - toCheckAt;
                Vector3 pushVector = collisionVector - (sphereCol.radius * collisionVector.normalized);
                toCheckAt += pushVector;

                hitCollider = true;
            }
        }
        Ray groundRay = new Ray(toCheckAt, gravityDirection);
        Ray topRay = new Ray(toCheckAt, -gravityDirection);
        collisionInfo.collisionOnTop = Physics.Raycast(topRay, out RaycastHit hit, sphereCol.radius + .01f);
        collisionInfo.grounded = Physics.Raycast(groundRay, out RaycastHit hit2, sphereCol.radius + .01f);
        if (collisionInfo.grounded) collisionInfo.standingOn = hit2.collider.gameObject;

        return toCheckAt;
    }

    #endregion

    #region PublicFunctions

    public void ChangeGravityDirection(Vector3 newDirection)
    {
        gravityDirection = newDirection;
    }

    #endregion


    //private void OnDrawGizmosSelected()
    //{
    //    switch(colType)
    //    {
    //        case ColliderType.Capsule:
    //            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - ((2 / 2) - .5f), transform.position.z), .5f);
    //            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + ((2 / 2) - .5f), transform.position.z), .5f);
    //            break;
    //        case ColliderType.Box:
    //            break;
    //        case ColliderType.Sphere:
    //            Gizmos.DrawSphere(transform.position, .5f);
    //            break;
    //    }

    //}

    public struct CollisionInfo
    {
        public bool grounded;
        public bool collisionOnTop;
        public bool collidingWithPlayer;
        public GameObject standingOn;

        public void ResetCollisionInfo()
        {
            grounded = false;
            standingOn = null;
            collisionOnTop = false;
            collidingWithPlayer = false;
        }
    }
}

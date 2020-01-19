using UnityEngine;

namespace MathExtensionZ
{
    public static class MathZ
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"> The start of the line you want to check at</param>
        /// <param name="end"> The end of the line you want to check at</param>
        /// <param name="point"> The point from which you want the closest point on your line</param>
        /// <returns></returns>
        public static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
        {
            //Get heading
            Vector3 heading = (end - origin);
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            //Do projection from the point but clamp it
            Vector3 lhs = point - origin;
            float dotP = Vector3.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
            return origin + heading * dotP;
        }
        /// <summary>
        /// Use this function to not offset your boxCollider.
        /// </summary>
        /// <param name="boxCollider"></param>
        /// <param name="position"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 FindNearestPointOnBox(BoxCollider boxCollider, Vector3 position, Vector3 point)
        {
            Vector3 originalCenter = boxCollider.center;
            boxCollider.center = position;
            Vector3 closestPoint = boxCollider.ClosestPoint(point);
            boxCollider.center = originalCenter;
            return closestPoint;
        }
        /// <summary>
        /// Use this function to not offset your boxCollider. It will return a point closest on the box that surrounds the collider. 
        /// </summary>
        /// <param name="boxCollider"></param>
        /// <param name="positionNextFrame"> the position from which you want to cast the collider</param>
        /// <param name="point">the point from which we get the closest point on bounds of the collider</param>
        /// <returns></returns>
        public static Vector3 FindNearestPointOnBoxBounds(BoxCollider boxCollider, Vector3 posNextFrame, Vector3 point, Vector3 pushDirection)
        {
            //We move our box collider to the position that we need it at.
            Vector3 originalCenter = boxCollider.center;
            boxCollider.center = boxCollider.transform.InverseTransformPoint(posNextFrame);

            Vector3 returnPoint;
            Vector3 boxBoundsInWorld;

            if(pushDirection.x >= 0 && pushDirection.y >= 0 && pushDirection.z >= 0)
            {
                //boxBoundsInWorld = boxCollider.transform.TransformPoint(boxCollider.bounds.min);
                boxBoundsInWorld = boxCollider.bounds.min;
                returnPoint = new Vector3(
                    pushDirection.x * boxBoundsInWorld.x,
                    pushDirection.y * boxBoundsInWorld.y,
                    pushDirection.z * boxBoundsInWorld.z); 
            } else
            {
                //boxBoundsInWorld = boxCollider.transform.TransformPoint(boxCollider.bounds.max);
                boxBoundsInWorld = boxCollider.bounds.max;
                returnPoint = new Vector3(
                    pushDirection.x * boxBoundsInWorld.x,
                    pushDirection.y * boxBoundsInWorld.y,
                    pushDirection.z * boxBoundsInWorld.z);
            }

            //We set everything we didnt push back to our original point to have a straight pushVector
            if (returnPoint.x == 0) returnPoint.x = point.x;
            if (returnPoint.y == 0) returnPoint.y = point.y;
            if (returnPoint.z == 0) returnPoint.z = point.z;

            //We move our boxCollider back to its original center
            boxCollider.center = originalCenter;

            return returnPoint;
        }

        public static float GetBoxSizeTowardsPoint(BoxCollider boxCollider, Vector3 position, Vector3 point)
        {
            Vector3 direction = (point - position).normalized;
            Vector3 bounds = boxCollider.size / 2;
            float distanceX = bounds.x / direction.x;
            float distanceY = bounds.y / direction.y;
            float distanceZ = bounds.z / direction.z;

            if (distanceX < distanceY && distanceX < distanceZ)
            {
                return boxCollider.size.x;
            } else if (distanceZ <= distanceX && distanceZ < distanceY)
            {
                return boxCollider.size.z;
            } else
            {
                return boxCollider.size.y;
            }
        }

        public static Vector3 GetMainDirectionForBox(BoxCollider boxCollider, Vector3 position, Vector3 point)
        {
            Vector3 direction = (point - position).normalized;
            Vector3 bounds = boxCollider.size / 2;
            float distanceX = Mathf.Abs(bounds.x / direction.x);
            float distanceY = Mathf.Abs(bounds.y / direction.y);
            float distanceZ = Mathf.Abs(bounds.z / direction.z);

            if (distanceX < distanceY && distanceX < distanceZ)
            {
                if (direction.x > 0) return boxCollider.transform.right;

                return -boxCollider.transform.right;
            }
            else if (distanceZ <= distanceX && distanceZ < distanceY)
            {
                if (direction.z > 0) return boxCollider.transform.forward;

                return -boxCollider.transform.forward;
            }
            else
            {
                if (direction.y > 0) return boxCollider.transform.up;

                return -boxCollider.transform.up;
            }
        }
        public static Vector3 GetMainDirectionForBox(BoxCollider boxCollider, Vector3 direction)
        {
            Vector3 bounds = boxCollider.size / 2;
            float distanceX = Mathf.Abs(bounds.x / direction.x);
            float distanceY = Mathf.Abs(bounds.y / direction.y);
            float distanceZ = Mathf.Abs(bounds.z / direction.z);

            if (distanceX < distanceY && distanceX < distanceZ)
            {
                if (direction.x > 0) return -boxCollider.transform.right;

                return boxCollider.transform.right;
            }
            else if (distanceZ <= distanceX && distanceZ < distanceY)
            {
                if (direction.z > 0) return -boxCollider.transform.forward;

                return boxCollider.transform.forward;
            }
            else
            {
                if (direction.y > 0) return -boxCollider.transform.up;

                return boxCollider.transform.up;
            }
        }

        public static float GetBoxDistanceTowardsPoint(BoxCollider boxCollider, Vector3 position, Vector3 point)
        {
            Vector3 pointOnBounds = FindNearestPointOnBox(boxCollider, position, point);

            return (pointOnBounds - position).magnitude;
        }
    }
}

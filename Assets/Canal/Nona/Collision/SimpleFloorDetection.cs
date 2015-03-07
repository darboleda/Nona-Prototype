using UnityEngine;
using System.Collections;

public class SimpleFloorDetection : NonaDeltaLimit
{

    public LayerMask FloorLayer;
    private Transform cachedTransform;

    public Transform Transform
    {
        get
        {
            cachedTransform = cachedTransform ?? transform;
            return cachedTransform;
        }
    }

    public override ApplicationResult Apply(Vector2 desiredDelta, NonaCollision previousInfo, NonaCollision currentInfo)
    {
        ApplicationResult result = new ApplicationResult();

        result.updatedDelta = desiredDelta;
        result.updatedInfo = currentInfo;

        RaycastHit hit;
        Ray ray = new Ray(Transform.position + (Vector3)(desiredDelta) + Vector3.up * 0.5f, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (currentInfo.Nona.Velocity.y <= 0 && Physics.Raycast(ray, out hit, 1f, FloorLayer))
        {
            Vector3 finalDelta = hit.point - Transform.position;
            if (finalDelta.y <= 0 && desiredDelta.y <= finalDelta.y || Mathf.Abs(desiredDelta.y - finalDelta.y) < 0.01f)
            {
                result.updatedInfo.Grounded = true;
                result.updatedDelta = finalDelta;
                return result;
            }
        }
        return result;
    }
}

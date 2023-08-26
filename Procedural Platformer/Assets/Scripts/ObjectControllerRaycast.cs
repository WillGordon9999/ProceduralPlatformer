using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dreamteck/Splines/Object Controller Rules/Raycast")]
public class ObjectControllerRaycast : Dreamteck.Splines.ObjectControllerCustomRuleBase
{
    public float dummyDist = 0.0f;

    public override Vector3 GetOffset()
    {
        SplineData data = currentController.GetComponent<SplineData>();
        Collider collider = data.colliders[currentObjectIndex];

        Vector3 pos = collider.transform.position + collider.bounds.center;
        Quaternion rot = collider.transform.rotation;

        Collider[] colliders = Physics.OverlapBox(pos, collider.bounds.extents, rot);

        Vector3 direction = Vector3.zero;
        float totalDist = 0.0f;

        if (colliders != null && colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Vector3 posB = colliders[i].transform.position + colliders[i].bounds.center;
                Quaternion rotB = colliders[i].transform.rotation;

                if (Physics.ComputePenetration(collider, pos, rot, colliders[i], posB, rotB, out Vector3 dir, out float dist))
                {
                    direction += dir;
                    totalDist += dist;
                }
            }

            direction *= totalDist;
        }

        Vector3 rayPos = data.rayPoses[currentObjectIndex];
        rayPos.y += data.pass.spawnOnTopRayDist - 1.0f;
        rayPos = collider.transform.TransformPoint(rayPos); //was currentController aka the Spline Object transform

        if (Physics.Raycast(new Ray(rayPos, Vector3.down), out RaycastHit hit, data.pass.spawnOnTopRayDist))
        {
            if (hit.collider != collider)
            {
                direction += (hit.point - pos);
            }
        }
        
        //Trying without defaulting to moving up
        //if (direction != Vector3.zero)
        //{
        //    direction = Vector3.up * totalDist;
        //}

        return direction;
    }

    //public override Vector3 GetScale()
    //{
    //    return Vector3.one;
    //}
}

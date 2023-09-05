using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineData : MonoBehaviour
{    
    public bool initialized = false;
    public List<GameObject> objects;
    public List<Collider> colliders;
    public List<Vector3> rayPoses;
    public List<SpawnPointData> spawnDatas;
    public SplinePass pass;

    public Vector3 mainLandCenter;
    public Vector3 debugStart;
    public Vector3 debugStartDir;
    public float startDist = 50.0f;
    public Vector3 debugEnd;

    public List<RaycastHit> hits;
    public GameObject pointList;

    //public GameObject totalBoundsObj;
    //public Bounds totalBounds;
    //public BoxCollider totalBox;

    // Start is called before the first frame update
    void Start()
    {
        //hits = new List<RaycastHit>();
        //Helper

        //while (!foundSpotOnTop)
        //{
        //    data = spawnedObjects[index].GetComponent<SpawnPointData>();
        //    pos = data.transform.TransformPoint(data.spawnPoints.yPoints[Random.Range(0, data.spawnPoints.yPoints.Count)]);
        //
        //    Vector3 rayPos = pos + Vector3.up * (pass.spawnOnTopRayDist - 1.0f);
        //
        //    if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, pass.spawnOnTopRayDist))
        //    {
        //        if (hit.collider.gameObject == data.gameObject)
        //        {
        //
        //        }
        //    }
        //
        //    yield return null;
        //}

        //Transform closest = null;
        //float dist = -1.0f;
        //for (int k = 0; k < current.neighbors[j].rayPoints.Length; k++)
        //{
        //    if (closest == null)
        //    {
        //        closest = current.neighbors[j].rayPoints[k];
        //        dist = Vector3.Distance(point.position, current.neighbors[j].rayPoints[k].position);
        //    }
        //
        //    if (closest != null)
        //    {
        //        float dist2 = Vector3.Distance(point.position, current.neighbors[j].rayPoints[k].position);
        //        
        //        if (dist2 < dist)
        //        {
        //
        //        }
        //    }
        //}

    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = MyColor.orange;
        
        if (pointList != null)
        {
            for (int i = 0; i < pointList.transform.childCount; i++)
            {
                Gizmos.DrawSphere(pointList.transform.GetChild(i).position, 0.5f);
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(debugStart, debugStart + debugStartDir * startDist);
    
        //Gizmos.DrawWireCube(totalBox.center, totalBox.size);
    
    }

#endif
}


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

    //public GameObject totalBoundsObj;
    //public Bounds totalBounds;
    //public BoxCollider totalBox;

    // Start is called before the first frame update
    void Start()
    {
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

    }

#if UNITY_EDITOR

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawWireCube(totalBox.center, totalBox.size);
    //}

#endif
}


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
    public List<Vector3> pointList;

    [Range(0.0f, 1.0f)]
    public float xScale = 1.0f;
    public float maxWidth = 5.0f;
    [Range(0.0f, 1.0f)]
    public float yScale = 1.0f;
    public float maxHeight = 10.0f;

    float prevXScale = 0.0f;
    float prevYScale = 0.0f;
    float prevHeight = 0.0f;
    float prevWidth = 0.0f;

    List<Vector3> origPos;
    bool initOrigList = false;
    float[] sign;
    //public GameObject totalBoundsObj;
    //public Bounds totalBounds;
    //public BoxCollider totalBox;

    // Start is called before the first frame update
    void Start()
    {
        pointList = new List<Vector3>();
        prevXScale = xScale;
        prevYScale = yScale;
        prevWidth = maxWidth;
        prevHeight = maxHeight;
        sign = new float[] { 1.0f, -1.0f };
    }

    //private void Update()
    //{
    //    bool update = false;
    //
    //    if (xScale != prevXScale)
    //    {
    //        prevXScale = xScale;
    //        update = true;
    //    }
    //
    //    if (maxWidth != prevWidth)
    //    {
    //        prevWidth = maxWidth;
    //        update = true;
    //    }
    //
    //    if (yScale != prevYScale)
    //    {
    //        prevYScale = yScale;
    //        update = true;
    //    }
    //
    //    if (maxHeight != prevHeight)
    //    {
    //        prevHeight = maxHeight;
    //        update = true;
    //    }
    //
    //    if (update)
    //    {
    //        if (!initOrigList)
    //        {
    //            origPos = new List<Vector3>(transform.childCount);
    //
    //            for (int i = 0; i < transform.childCount; i++)
    //            {
    //                origPos.Add(transform.GetChild(i).position);
    //            }
    //
    //            initOrigList = true;
    //        }
    //
    //        for (int i = 0; i < transform.childCount; i++)
    //        {
    //            Transform child = transform.GetChild(i);
    //            Vector3 pos = origPos[i];
    //            Vector3 dir = child.TransformDirection(Vector3.right);
    //            dir *= sign[Random.Range(0, 2)];
    //            pos.y += Mathf.PerlinNoise1D(i * yScale) * maxHeight;
    //            pos += dir * Mathf.PerlinNoise1D(i * xScale) * maxWidth;                
    //            child.position = pos;
    //        }
    //        
    //        update = false;
    //    }        
    //}

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = MyColor.orange;
        
        if (pointList != null)
        {
            for (int i = 0; i < pointList.Count; i++)
            {
                Gizmos.DrawSphere(pointList[i], 0.5f);
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(debugStart, debugStart + debugStartDir * startDist);
    
        //Gizmos.DrawWireCube(totalBox.center, totalBox.size);
    
    }

#endif
}


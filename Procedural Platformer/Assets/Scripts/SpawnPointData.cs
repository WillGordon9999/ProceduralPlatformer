using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class SpawnPointDataEditor
{        
    //[MenuItem("Window/Generate Point Data for Mesh")]
    //public static void GenerateData()
    //{
    //    GameObject obj = Selection.activeGameObject;
    //
    //    if (obj != null)
    //    {
    //        SpawnPointData data = obj.GetComponent<SpawnPointData>();
    //        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
    //
    //        Vector3[] verts = mesh.vertices;
    //        Vector3[] normals = mesh.normals;
    //
    //        if (data.xPoints == null)
    //            data.xPoints = new List<Vector3>();
    //
    //        if (data.yPoints == null)
    //            data.yPoints = new List<Vector3>();
    //
    //        if (data.zPoints == null)
    //            data.zPoints = new List<Vector3>();
    //
    //
    //        for (int i = 0; i < verts.Length; i++)
    //        {
    //            if (Mathf.Abs(Vector3.Dot(normals[i], Vector3.right)) >= 0.9f)
    //                data.xPoints.Add(verts[i]);
    //
    //            if (Vector3.Dot(normals[i], Vector3.up) >= 0.9f)
    //                data.yPoints.Add(verts[i]);
    //
    //            if (Mathf.Abs(Vector3.Dot(normals[i], Vector3.forward)) >= 0.9f)
    //                data.zPoints.Add(verts[i]);
    //        }
    //    }
    //}
}
#endif

public class SpawnPointData : MonoBehaviour
{
    public bool test = false;

    public Vector3 center;    
    public Vector3 size;  //This is actually the extents I think

    public SpawnPointAsset spawnPoints;

    public SpawnObjectList stackables;
    public SpawnObjectList adjacents;

    public List<SpawnPointData> neighbors;

    public Collider boxCollider;
    public Vector3 rayPos;
   
    //public Bounds testBounds;
    //public Vector3[] rays;
    //public int[] indices;

    //public int attempts = 0;
    //public int hits = 0;    

    public Transform[] rayPoints;
   

    //public List<Vector3> xPoints;
    //public List<Vector3> yPoints;
    //public List<Vector3> zPoints;
    //
    //public List<GameObject> stackables;
    //public List<GameObject> adjacents;

    // Start is called before the first frame update
    void Start()
    {
        neighbors = new List<SpawnPointData>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(transform.TransformPoint(testBounds.center), testBounds.size);   
    }

#endif

    //IEnumerator Testing()
    //{
    //    GameObject objX = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    objX.GetComponent<Renderer>().material.color = Color.green;
    //
    //    GameObject objY = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    objY.GetComponent<Renderer>().material.color = Color.yellow;
    //
    //    GameObject objZ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    objZ.GetComponent<Renderer>().material.color = Color.blue;
    //
    //    while (true)
    //    {
    //        objX.transform.position = transform.TransformPoint(xPoints[Random.Range(0, xPoints.Count)]);
    //        objY.transform.position = transform.TransformPoint(yPoints[Random.Range(0, yPoints.Count)]);
    //        objZ.transform.position = transform.TransformPoint(zPoints[Random.Range(0, zPoints.Count)]);
    //        yield return new WaitForSeconds(3.0f);
    //    }
    //}
}


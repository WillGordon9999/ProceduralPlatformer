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
    public Vector3 center;    
    public Vector3 size;  //This is actually the extents I think

    public SpawnPointAsset spawnPoints;

    public SpawnObjectList stackables;
    public SpawnObjectList adjacents;

    public BoxCollider boxCollider;
    //public BoxCollider yBounds;
    //RayPoints on the bottom of the collider
    public Vector3 rayPos; //center
    public Vector3 topRight;
    public Vector3 topLeft;
    public Vector3 downRight;
    public Vector3 downLeft;

    public Bounds yBounds;
    public int mainIndex = -1;

    //Inspector-assigned poses
    public Transform[] rayPoints;
    public Transform[] neighborPoints;
    //[HideInInspector] public bool[] rayPointHits;
    public RaycastHit[] rayPointHits;
    public int rayPointHitCount = 0;

    public float generateRadius = 10.0f;

    public List<SpawnPointData> neighbors;
    public List<Bounds> neighborBounds;
    public List<Vector3> neighborDirs;

    [Header("Debug")]
    //public float textOffset = -5.0f;
    public List<Vector3> neighborRayPoses;
    public List<Vector3> neighborRayDirs;
    public List<RaycastHit> neighborHits;
    //public List<Renderer> renderers;
    //public List<MeshFilter> meshes;    
    bool render = false;

    public MeshFilter mesh;
    public Renderer renderer;
    public SplineData spline;
    //public HashSet<SpawnPointData> neighbors;
    //public HashSet<Bounds> neighborBounds;

    //public bool markedForDeath = false; //Had to add this because for Destroy taking too long dunno about DestroyImmediate here

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
        neighborBounds = new List<Bounds>();
        neighborDirs = new List<Vector3>();
        //rayPointHits = new bool[rayPoints.Length];
        rayPointHits = new RaycastHit[rayPoints.Length];

        neighborRayPoses = new List<Vector3>();
        neighborHits = new List<RaycastHit>();
        neighborRayDirs = new List<Vector3>();
        //renderers = new List<Renderer>();
        //meshes = new List<MeshFilter>();

        renderer = GetComponent<Renderer>();
        mesh = GetComponent<MeshFilter>();

        //neighbors = new HashSet<SpawnPointData>();
        //neighborBounds = new HashSet<Bounds>();
    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;        
        int meshDrawn = 0;
        for (int i = 0; i < neighbors.Count; i++)
        {
            MeshFilter mesh = neighbors[i].mesh;
            Gizmos.DrawWireMesh(mesh.sharedMesh, mesh.transform.position, mesh.transform.rotation, mesh.transform.localScale);
            meshDrawn++;
        }

        //for (int i = 0; i < neighborBounds.Count; i++)
        //{
        //    //Gizmos.DrawWireCube(neighborBounds[i].center, neighborBounds[i].size);
        //    Gizmos.DrawWireCube(neighborBounds[i].center, neighborBounds[i].extents);            
        //}


        Debug.Log($"Drawn {meshDrawn} meshes for {neighbors.Count}");

        Color blue = Color.blue;
        blue.a = 0.5f;

        Gizmos.color = blue;
        Gizmos.DrawMesh(mesh.sharedMesh, transform.position, transform.rotation, transform.localScale);

        Gizmos.color = Color.red;
        for (int i = 0; i < neighborRayPoses.Count; i++)
        {
            Vector3 pos = neighborRayPoses[i];
            //Gizmos.DrawLine(pos, pos + Vector3.down * spline.pass.neighborCastDist);
            //Gizmos.DrawLine(pos, pos + neighborRayDirs[i] * spline.pass.spawnOnTopRayDist);
            Gizmos.DrawLine(pos, pos + neighborRayDirs[i] * spline.pass.neighborCastDist);
        }

        Gizmos.color = MyColor.orange;

        for (int i = 0; i < rayPointHits.Length; i++)
        {
            Gizmos.DrawSphere(rayPointHits[i].point, 0.5f);
        }

        //for (int i = 0; i < neighborRayPoses.Count; i++)
        //{
        //    Gizmos.DrawSphere(neighborRayPoses[i], spline.pass.checkRadius);
        //}

        Gizmos.color = MyColor.purple;
        for (int i = 0; i < neighborHits.Count; i++)
        {
            Gizmos.DrawSphere(neighborHits[i].point, spline.pass.checkRadius);
        }

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


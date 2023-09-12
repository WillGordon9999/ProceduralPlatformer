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
    public Vector3 rayPos; 
    public Vector3 topRight;
    public Vector3 topLeft;
    public Vector3 downRight;
    public Vector3 downLeft;

    public Bounds yBounds;
    public int mainIndex = -1;

    //Inspector-assigned poses
    
    [Header("Top Side")]
    public Transform[] rayPoints; //The Target Points to try and hit for land pruning
    public Transform[] neighborPoints;
    public RaycastHit[] rayPointHits; //The actual hits for land prune check
    
    [Header("Bottom Side")]
    public Transform[] rayPoses;
    public int rayPointHitCount = 0;
    public RaycastHit[] groundHits;
    public int minGroundHitCount = 3;

    [Header("Neighbors")]
    public float generateRadius = 10.0f;
    public List<SpawnPointData> neighbors;
    public List<Bounds> neighborBounds;
    public List<Vector3> neighborDirs;

    [Header("Debug")]
    public float diff = 0.0f;
    public Vector3 scalePos;
    public Bounds testBounds;
    //public bool testMove = false;
    public Vector3 origPos;
    public Vector3 targetPos;
    public float targetHeight = 0.0f;
    float prevHeight = 0.0f;
    GameObject targetObj = null;

    public List<Vector3> neighborRayPoses;
    public List<Vector3> neighborRayDirs;
    public List<RaycastHit> neighborHits;    
    bool render = false;

    public MeshFilter mesh;
    public Renderer renderer;
    public SplineData spline;

    void Awake()
    {
        neighbors = new List<SpawnPointData>();
        neighborBounds = new List<Bounds>();
        neighborDirs = new List<Vector3>();        
        rayPointHits = new RaycastHit[rayPoints.Length];
        boxCollider = GetComponent<BoxCollider>();

        neighborRayPoses = new List<Vector3>();
        neighborHits = new List<RaycastHit>();
        neighborRayDirs = new List<Vector3>();
        

        renderer = GetComponent<Renderer>();
        mesh = GetComponent<MeshFilter>();

        prevHeight = targetHeight;
        //groundHits = new RaycastHit[4];

        //neighbors = new HashSet<SpawnPointData>();
        //neighborBounds = new HashSet<Bounds>();
    }

    public void SetRayPosesLocal()
    {
        Bounds bounds = boxCollider.bounds;

        rayPos = new Vector3(bounds.center.x, bounds.center.y - bounds.extents.y, bounds.center.z);
        rayPos = new Vector3(bounds.center.x, -bounds.extents.y, bounds.center.z);
        topRight = new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
        topLeft = new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
        downRight = new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
        downLeft = new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
    }

    public void SetRayPosesLocal(Vector3 offset)
    {
        SetRayPosesLocal();
        rayPos += offset;
        topRight += offset;
        topLeft += offset;
        downRight += offset;
        downLeft += offset;
    }

    public void SetRayPosesWorld()
    {
        rayPos = rayPoses[0].position;
        topRight = rayPoses[1].position;
        topLeft = rayPoses[2].position;
        downRight = rayPoses[3].position;
        downLeft = rayPoses[4].position;
    }

    public void SetRayPosesWorld(Vector3 offset)
    {
        rayPos = rayPoses[0].TransformPoint(offset);
        topRight = rayPoses[1].TransformPoint(offset);
        topLeft = rayPoses[2].TransformPoint(offset);
        downRight = rayPoses[3].TransformPoint(offset);
        downLeft = rayPoses[4].TransformPoint(offset);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (testMove)
    //    {
    //        Vector3 target = transform.TransformPoint(Vector3.up * 10.0f);
    //        SplineLevelCreator.AddSphere(target, "Test target", spline.gameObject, Color.green);
    //        Transform origParent = transform.parent;
    //        Transform rayPos = rayPoses[0];
    //        rayPos.parent = null;
    //        transform.parent = rayPos;
    //        rayPos.position = target;
    //        transform.parent = origParent;
    //        rayPos.parent = transform;
    //
    //        testMove = false;
    //    }
    //
    //    bool update = false;
    //    if (targetHeight != prevHeight)
    //    {
    //        prevHeight = targetHeight;
    //        update = true;
    //    }
    //    
    //    if (update)
    //    {
    //        //Vector3 pos = new Vector3(boxCollider.bounds.center.x, boxCollider.bounds.center.y + boxCollider.bounds.extents.y, boxCollider.bounds.center.z);
    //        ////pos = transform.TransformPoint(pos); //was transformVector
    //        //pos = transform.TransformVector(pos);
    //        //Vector3 targetPos = new Vector3(pos.x, pos.y + targetHeight, pos.z);
    //        Vector3 pos = rayPoints[0].position;
    //        Vector3 targetPos = pos;
    //        targetPos.y += targetHeight;
    //    
    //        if (targetObj != null)
    //            Destroy(targetObj);
    //    
    //        targetObj = SplineLevelCreator.AddSphere(targetPos, "targetHeight", null, Color.blue);
    //        float diff = targetPos.y - pos.y;
    //        Bounds bounds = boxCollider.bounds;
    //        Bounds orig = boxCollider.bounds;
    //        bounds.Expand(targetHeight);
    //        bounds.Expand(diff);
    //        //bounds.Encapsulate(targetPos);
    //        //float scale = Mathf.Min(bounds.extents.x / orig.extents.x, bounds.extents.y / orig.extents.y, bounds.extents.z / orig.extents.z);
    //        float scale = Mathf.Max(bounds.extents.x / orig.extents.x, bounds.extents.y / orig.extents.y, bounds.extents.z / orig.extents.z);
    //        transform.localScale = Vector3.one * scale;
    //        //bounds.extents = new Vector3(bounds.extents.x, bounds.extents.y + targetHeight, bounds.extents.z);
    //        update = false;
    //    }
    //}

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
        
        //Debug.Log($"Drawn {meshDrawn} meshes for {neighbors.Count}");
    
        Color blue = Color.blue;
        blue.a = 0.5f;
    
        Gizmos.color = blue;
        Gizmos.DrawMesh(mesh.sharedMesh, transform.position, transform.rotation, transform.localScale);
    
        Gizmos.color = Color.red;

        if (neighborRayPoses != null)
        {
            for (int i = 0; i < neighborRayPoses.Count; i++)
            {
                Vector3 pos = neighborRayPoses[i];
                //Gizmos.DrawLine(pos, pos + Vector3.down * spline.pass.neighborCastDist);
                //Gizmos.DrawLine(pos, pos + neighborRayDirs[i] * spline.pass.spawnOnTopRayDist);
                Gizmos.DrawLine(pos, pos + neighborRayDirs[i] * spline.pass.neighborCastDist);
            }
        }

        Gizmos.color = MyColor.orange;

        if (rayPointHits != null)
        {
            for (int i = 0; i < rayPointHits.Length; i++)
            {
                Gizmos.DrawSphere(rayPointHits[i].point, 0.5f);
            }
        }
        //for (int i = 0; i < neighborRayPoses.Count; i++)
        //{
        //    Gizmos.DrawSphere(neighborRayPoses[i], spline.pass.checkRadius);
        //}
    
        //Gizmos.color = MyColor.purple;
        //for (int i = 0; i < neighborHits.Count; i++)
        //{
        //    Gizmos.DrawSphere(neighborHits[i].point, spline.pass.checkRadius);
        //}
    
        Gizmos.color = Color.cyan;

        Vector3 p1 = rayPoints[(int)RayPointOrder.TopRight].position;
        Vector3 p2 = rayPoints[(int)RayPointOrder.TopLeft].position;
        Vector3 p3 = rayPoints[(int)RayPointOrder.DownRight].position;
        Vector3 p4 = rayPoints[(int)RayPointOrder.DownLeft].position;

        Gizmos.DrawSphere(p1, 0.5f);
        Gizmos.DrawSphere(p2, 0.5f);
        Gizmos.DrawSphere(p3, 0.5f);
        Gizmos.DrawSphere(p4, 0.5f);
    
        Gizmos.DrawLine(p1, p1 + Vector3.down * (spline.pass.groundCheckDist + boxCollider.bounds.size.y));
        Gizmos.DrawLine(p2, p2 + Vector3.down * (spline.pass.groundCheckDist + boxCollider.bounds.size.y));
        Gizmos.DrawLine(p3, p3 + Vector3.down * (spline.pass.groundCheckDist + boxCollider.bounds.size.y));
        Gizmos.DrawLine(p4, p4 + Vector3.down * (spline.pass.groundCheckDist + boxCollider.bounds.size.y));

        Gizmos.color = Color.magenta;

        if (groundHits != null)
        {
            for (int i = 0; i < groundHits.Length; i++)
            {
                Gizmos.DrawSphere(groundHits[i].point, spline.pass.groundCheckRadius);
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPos, 0.5f);
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(rayPoints[0].position, 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(testBounds.center, testBounds.size);
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


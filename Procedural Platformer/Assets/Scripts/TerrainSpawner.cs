using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Procedural Platformer Algorithm:

- Set location of Kill plane

//May want multiple passes each for props various sizes:
//If you want may want to randomize start and end points
//Would some overlap not be bad?

1st pass: Largest scale props to serve as main landmasses, Use Random.Range to generate positions and Boxcast only the immediate area to ensure no overlap.  

2nd pass: Add smaller props to better layout the main landmasses, Use either Boxcast or GetClosestPointOnBounds to determine positions for smaller props

3rd pass: Add either smaller props or connections to other props to make a more cohesive 'playground' 

4th pass: Add collectibles and/or enemies to make a complete level. 
 */

public class TerrainSpawner : MonoBehaviour
{
    public static TerrainSpawner Instance { get { return mInstance; } private set { } }
    private static TerrainSpawner mInstance;

    [Header("Spawn Counts")]
    public uint spawnCountPass1 = 100;
    public uint spawnCountPass2 = 50;
    public uint spawnCountPass3 = 100;
    public uint collectibleSpawnCount = 200;

    public uint largePassSpawnCount = 10;
    public uint tileSpawnCount = 100;

    [Header("Iterations")]
    public uint pass1ExtraIterations = 0;
   
    public List<GameObject> spawnObjects;
    public List<GameObject> spawnObjectsLarge;
    public List<GameObject> tiles;
    public List<GameObject> collectibles;
    List<GameObject> mainLandMasses;

    [Header("Ranges")]
    public float minX = -500.0f;
    public float minY = -100.0f;
    public float minZ = -500.0f;

    public float maxX = 500.0f;
    public float maxY = 300.0f;
    public float maxZ = 500.0f;

    [Header("First Pass Controls")]
    public float defaultMinScale = 1.0f;
    public float defaultMaxScale = 2.0f;

    [Space(10)]
    public bool useRandomScale = false;    
    public float minScaleRangeMin = 1.0f;
    public float minScaleRangeMax = 2.0f;

    [Space(5)]
    public float maxScaleRangeMin = 2.0f;
    public float maxScaleRangeMax = 3.0f;

    [Space(10)]
    public bool doNotDelete = false;

    [Space(10)]
    public bool useRandomSignChance = false;
    public int maxSignRange = 2;
    int defaultSignRange = 2;

    public enum TransformOptions { TransformPoint, TransformVector, Random }
    [Space(10)]
    public TransformOptions transformOptions = TransformOptions.TransformPoint;

    public enum PositionOptions { None, ClosestPoint, ClosestPointOnBounds, Lerp, Random_Lerp, All_Of_The_Above_Random };

    [Space(10)]
    public PositionOptions options = PositionOptions.None;
    [Range(0.0f, 1.0f)]
    public float defaultLerpVal = 0.5f;
    [Range(0.0f, 1.0f)]
    public float minLerpVal = 0.0f;
    [Range(0.0f, 1.0f)]
    public float maxLerpVal = 1.0f;

    public bool useHeight = false;
    int heightChance = 100;

    float maxScalePass1 = 100.0f;
    float minScalePass1 = 75.0f;

    float maxScalePass2 = 50.0f;
    float minScalePass2 = 25.0f;
       
    float maxDistX = 20.0f;
    float minDistX = 5.0f;

    float maxDistY = 20.0f;
    float minDistY = 5.0f;

    float maxDistZ = 20.0f;
    float minDistZ = 5.0f;

    float terrainBound = 256.0f;

    GameObject gizmoObj = null;
    RaycastHit gizmoRay;
    Vector3 gizmoPos;
    Vector3 gizmoDir;

    private void Awake()
    {
        if (mInstance == null)
            mInstance = this;
    }

    void Start()
    {
        mainLandMasses = new List<GameObject>();

        //StartCoroutine("TerrainBoxCastPass");
        //StartCoroutine("CreateLandMass", Vector3.zero);
        //StartCoroutine("LargePassUpdate");
        StartCoroutine(NewPassUpdate(Vector3.zero));

        for (int i = 0; i < pass1ExtraIterations; i++)
        {
            float x = Random.Range(minX, maxX);
            float y = useHeight ? Random.Range(minY, maxY) : 0.0f;
            float z = Random.Range(minZ, maxZ);
            StartCoroutine(NewPassUpdate(new Vector3(x, y, z)));
        }
    }

    IEnumerator CreateLandMass(Vector3 startPos)
    {
        int spawnCounter = 1;
        //float randX = 0.0f;
        //float randY = 0.0f;
        //float randZ = 0.0f;
        Vector3[] tileDist = new Vector3[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
                
        int index = Random.Range(0, tiles.Count);
        
        GameObject obj = Instantiate(tiles[index], startPos, Quaternion.identity);        
        BoxCollider box = obj.GetComponent<BoxCollider>();
        mainLandMasses.Add(obj);

        while (spawnCounter < tileSpawnCount)
        {                        
            //randX = tileDist[Random.Range(0, tileDist.Length)];            
            //randZ = tileDist[Random.Range(0, tileDist.Length)];

            Vector3 pos = obj.transform.TransformPoint(tileDist[Random.Range(0, tileDist.Length)]);            
            GameObject newObj = Instantiate(tiles[index], pos, Quaternion.identity);
            
            Collider[] colliders = Physics.OverlapBox(pos, newObj.GetComponent<BoxCollider>().size, Quaternion.identity);
            bool deleted = false;

            if (colliders != null)
            {
                foreach (Collider col in colliders)
                {
                    if (col.gameObject != newObj && col.gameObject != obj)
                    {
                        Destroy(newObj);
                        deleted = true;
                        break;
                    }
                }
            }

            if (!deleted)
            {
                obj = newObj;
                box = obj.GetComponent<BoxCollider>();
                mainLandMasses.Add(obj);
                spawnCounter++;
            }

            yield return null;
        }

        //StartCoroutine("BoxCastPass");
    }
   
    IEnumerator NewPassUpdate(object startPos)
    {
        Vector3 initPos = (Vector3)startPos;
        int spawnCounter = 1;
        float randX = 0.0f;
        float randY = 0.0f;
        float randZ = 0.0f;

        int height = Random.Range(0, heightChance);

        Vector3 randDir = Random.onUnitSphere;
        Quaternion rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));
        
        //Spawn the first land mass to work with
        int index = Random.Range(0, spawnObjectsLarge.Count);

        GameObject obj = Instantiate(spawnObjectsLarge[index], initPos, rot);
        MeshCollider mesh = obj.GetComponent<MeshCollider>();
        mainLandMasses.Add(obj);

        while (spawnCounter < spawnCountPass1)
        {
            //height = Random.Range(0, heightChance);

            randDir = Random.onUnitSphere;
            rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));            

            index = Random.Range(0, spawnObjectsLarge.Count);

            BoxCollider testBox = obj.GetComponent<BoxCollider>();

            //randX = Random.Range(-mesh.bounds.size.x, mesh.bounds.size.x);
            //randY = height == 1 ? Random.Range(-mesh.bounds.size.y, mesh.bounds.size.y) : 0.0f;
            //randZ = Random.Range(-mesh.bounds.size.z, mesh.bounds.size.z);

            int signRange = useRandomSignChance ? maxSignRange : defaultSignRange;
            int signChance = Random.Range(0, signRange);

            float minScale = useRandomScale ? Random.Range(minScaleRangeMin, minScaleRangeMax) : defaultMinScale;
            float maxScale = useRandomScale ? Random.Range(maxScaleRangeMin, maxScaleRangeMax) : defaultMaxScale;

            if (signChance == 1)
            {               
                randX = Random.Range(testBox.size.x * minScale, testBox.size.x * maxScale);                
                randY = useHeight ? Random.Range(testBox.bounds.size.y * minScale, testBox.bounds.size.y * maxScale) : 0.0f;
                randZ = Random.Range(testBox.size.z * minScale, testBox.size.z * maxScale);
            }
            
            else
            {                
                randX = Random.Range(-testBox.size.x * minScale, -testBox.size.x * maxScale);                
                randY = useHeight ? Random.Range(-testBox.bounds.size.y * minScale, -testBox.bounds.size.y * maxScale) : 0.0f;
                randZ = Random.Range(-testBox.size.z * minScale, -testBox.size.z * maxScale);
            }

            //randX = Random.Range(-testBox.size.x * minScale, testBox.size.x * maxScale);
            //randY = height == 1 ? Random.Range(-testBox.bounds.size.y * minScale, testBox.bounds.size.y * maxScale) : 0.0f;
            //randZ = Random.Range(-testBox.size.z * minScale, testBox.size.z * maxScale);

            Vector3 pos = Vector3.zero;

            if (transformOptions == TransformOptions.TransformPoint)
                pos = obj.transform.TransformPoint(randX, randY, randZ);            

            if (transformOptions == TransformOptions.TransformVector)
                pos = obj.transform.TransformVector(randX, randY, randZ);

            if (transformOptions == TransformOptions.Random)
            {
                TransformOptions options = (TransformOptions)Random.Range(0, (int)TransformOptions.Random);

                if (options == TransformOptions.TransformPoint)
                    pos = obj.transform.TransformPoint(randX, randY, randZ);

                if (options == TransformOptions.TransformVector)
                    pos = obj.transform.TransformVector(randX, randY, randZ);
            }

            GameObject newObj = Instantiate(spawnObjectsLarge[index], pos, rot);

            bool deleted = false;

            if (!doNotDelete)
            {
                Collider[] colliders = Physics.OverlapBox(pos, newObj.GetComponent<BoxCollider>().size, rot);

                if (colliders != null)
                {
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject != newObj && !col.gameObject.transform.IsChildOf(newObj.transform))
                        {
                            Destroy(newObj);
                            deleted = true;
                            break;
                        }
                    }
                }
            }

            if (!deleted)
            {
                Vector3 dir = (obj.transform.position - newObj.transform.position).normalized;
                Ray ray = new Ray(newObj.transform.position, dir);
                RaycastHit hit;
                BoxCollider box = newObj.GetComponent<BoxCollider>();
                BoxCollider origBox = obj.GetComponent<BoxCollider>();
                origBox.enabled = true;
                box.enabled = true;
                
                if (origBox.Raycast(ray, out hit, 1000.0f))
                {
                    int chance = Random.Range(0, 2);

                    //float lerpVal = Random.Range(0.0f, 0.7f);
                    //newObj.transform.position = Vector3.Lerp(newObj.transform.position, obj.transform.position, lerpVal);

                    Vector3 closest = origBox.ClosestPoint(hit.point);
                    Vector3 onBounds = origBox.ClosestPointOnBounds(hit.point);
                    
                    if (options == PositionOptions.ClosestPoint)
                        newObj.transform.position = closest;

                    if (options == PositionOptions.ClosestPointOnBounds)
                        newObj.transform.position = onBounds;

                    if (options == PositionOptions.Lerp)
                        newObj.transform.position = Vector3.Lerp(newObj.transform.position, onBounds, defaultLerpVal);

                    if (options == PositionOptions.Random_Lerp)
                        newObj.transform.position = Vector3.Lerp(newObj.transform.position, onBounds, Random.Range(minLerpVal, maxLerpVal));

                    if (options == PositionOptions.All_Of_The_Above_Random)
                    {                        
                        PositionOptions randPos = (PositionOptions)Random.Range(0, (int)PositionOptions.All_Of_The_Above_Random);

                        if (randPos == PositionOptions.ClosestPoint)
                            newObj.transform.position = closest;

                        if (randPos == PositionOptions.ClosestPointOnBounds)
                            newObj.transform.position = onBounds;

                        if (randPos == PositionOptions.Lerp)
                            newObj.transform.position = Vector3.Lerp(newObj.transform.position, onBounds, defaultLerpVal);

                        if (randPos == PositionOptions.Random_Lerp)
                            newObj.transform.position = Vector3.Lerp(newObj.transform.position, onBounds, Random.Range(minLerpVal, maxLerpVal));
                    }

                    Vector3 resolveDir;
                    float dist;

                    if (Physics.ComputePenetration(box, newObj.transform.position, newObj.transform.rotation, mesh, obj.transform.position, obj.transform.rotation, out resolveDir, out dist))                    
                    {                        
                        newObj.transform.Translate(resolveDir * dist);
                    }
                    
                }

                box.enabled = false;
                origBox.enabled = false;

                obj = newObj;
                mesh = obj.GetComponent<MeshCollider>();
                mainLandMasses.Add(obj);
                spawnCounter++;
            }

            yield return null;
        }

        print("Finished New Pass Update");
        //StartCoroutine("BoxCastPass");
        StartCoroutine("CollectiblePass");
    }

    IEnumerator CollectiblePass()
    {
        foreach(GameObject obj in collectibles)
        {
            int spawnCounter = 0;
            while (spawnCounter < collectibleSpawnCount)
            {
                int index = Random.Range(0, mainLandMasses.Count);
                Bounds bounds = mainLandMasses[index].GetComponent<MeshCollider>().bounds;
                
                float randX = Random.Range(bounds.min.x, bounds.max.x);
                float randZ = Random.Range(bounds.min.z, bounds.max.z);

                Vector3 randDir = Random.onUnitSphere;
                Quaternion rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));

                //Bounds objBounds = obj.GetComponent<Collider>().bounds;
                float radius = obj.GetComponent<SphereCollider>().radius;

                RaycastHit hit;
                Vector3 pos = new Vector3(randX, maxY, randZ);

                if (Physics.SphereCast(pos, radius, Vector3.down, out hit))
                {
                    if (hit.collider.gameObject.GetComponent<Collectible>() == null)
                    {
                        Vector3 spawnPos = Vector3.Lerp(pos, hit.point, 0.95f);                        
                        GameObject newObj = Instantiate(obj, spawnPos, rot);
                        
                        bool deleted = false;                        
                        Collider[] colliders = Physics.OverlapSphere(newObj.transform.position, radius);

                        if (colliders != null)
                        {
                            foreach(Collider col in colliders)
                            {
                                if (col.gameObject != newObj)
                                {
                                    print("Destroying overlapping collectible");
                                    Destroy(newObj);
                                    deleted = true;
                                    break;
                                }
                            }
                        }

                        if (!deleted)
                        {
                            CollectibleManager.Instance.Add(newObj);
                            spawnCounter++;
                        }
                    }
                }

                yield return null;
            }

            //yield return null;
        }
    }

    IEnumerator LargePassUpdate()
    {
        int spawnCounter = 1;
        float randX = 0.0f;
        float randY = 0.0f;
        float randZ = 0.0f;

        int height = Random.Range(0, heightChance);

        Vector3 randDir = Random.onUnitSphere;
        Quaternion rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));

        int index = Random.Range(0, spawnObjectsLarge.Count);
        Vector3 scale = Vector3.one * Random.Range(minScalePass1, maxScalePass1);

        GameObject obj = Instantiate(spawnObjectsLarge[index], Vector3.zero, rot);
        obj.transform.localScale = scale;
        MeshCollider mesh = obj.GetComponent<MeshCollider>();
        mainLandMasses.Add(obj);

        while (spawnCounter < largePassSpawnCount)
        {
            height = Random.Range(0, heightChance);

            randDir = Random.onUnitSphere;
            rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));

            index = Random.Range(0, spawnObjectsLarge.Count);
            
            //randX = Random.Range(mesh.bounds.size.x, maxDistX);
            //randY = height == 1 ? Random.Range(mesh.bounds.size.y, maxDistY) : 0.0f;
            //randZ = Random.Range(mesh.bounds.size.z, maxDistZ);

            randX = Random.Range(-mesh.bounds.size.x, mesh.bounds.size.x);
            randY = height == 1 ? Random.Range(-mesh.bounds.size.y, mesh.bounds.size.y) : 0.0f;
            randZ = Random.Range(-mesh.bounds.size.z, mesh.bounds.size.z);

            scale = Vector3.one * Random.Range(minScalePass1, maxScalePass1);

            //Vector3 pos = obj.transform.TransformPoint(randX, randY, randZ);
            Vector3 pos = obj.transform.TransformVector(randX, randY, randZ);
            GameObject newObj = Instantiate(spawnObjectsLarge[index], pos, rot);
            newObj.transform.localScale = scale;
            
            Collider[] colliders = Physics.OverlapBox(pos, newObj.GetComponent<BoxCollider>().size * scale.x, rot);
            bool deleted = false;

            if (colliders != null)
            {
                foreach (Collider col in colliders)
                {
                    if (col.gameObject != newObj)
                    {
                        Destroy(newObj);
                        deleted = true;
                        break;
                    }
                }
            }

            if (!deleted)
            {                               
                obj = newObj;
                mesh = obj.GetComponent<MeshCollider>();
                mainLandMasses.Add(obj);
                spawnCounter++;
            }

            yield return null;
        }

        StartCoroutine("BoxCastPass");
    }

    IEnumerator BoxCastPass()
    {        
        foreach(GameObject land in mainLandMasses)
        {
            uint spawnCounter = 0;

            while (spawnCounter < spawnCountPass2)
            {
                //int mainIndex = Random.Range(0, mainLandMasses.Count);
                //Bounds bounds = mainLandMasses[mainIndex].GetComponent<MeshCollider>().bounds;
                Bounds bounds = land.GetComponent<MeshCollider>().bounds;

                float randX = Random.Range(bounds.min.x, bounds.max.x);
                float randZ = Random.Range(bounds.min.z, bounds.max.z);

                Vector3 randDir = Random.onUnitSphere;
                Quaternion rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));

                int index = Random.Range(0, spawnObjects.Count);
                MeshCollider mesh = spawnObjects[index].GetComponent<MeshCollider>();

                RaycastHit hit;
                Vector3 pos = new Vector3(randX, maxY, randZ);

                if (Physics.BoxCast(pos, (mesh.bounds.size) * 0.5f, Vector3.down, out hit, rot))
                {
                    GameObject newObj = Instantiate(spawnObjects[index], hit.collider.ClosestPointOnBounds(hit.point), rot);
                    spawnCounter++;
                }

                yield return null;
            }
        }        
    }

    IEnumerator TerrainBoxCastPass()
    { 
       uint spawnCounter = 0;

       while (spawnCounter < spawnCountPass2)
       {
           float randX = Random.Range(minX, maxX);
           float randZ = Random.Range(minZ, maxZ);

           Vector3 randDir = Random.onUnitSphere;
           Quaternion rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));

           int index = Random.Range(0, spawnObjectsLarge.Count);
           MeshCollider mesh = spawnObjectsLarge[index].GetComponent<MeshCollider>();

           RaycastHit hit;
           Vector3 pos = new Vector3(randX, maxY, randZ);

           if (Physics.BoxCast(pos, (mesh.bounds.size) * 0.5f, Vector3.down, out hit, rot))
           {
                if (hit.collider.gameObject.name == "Terrain")
                {
                    GameObject newObj = Instantiate(spawnObjectsLarge[index], hit.collider.ClosestPointOnBounds(hit.point), rot);
                    spawnCounter++;
                }
                
                //else
                //{
                //    Bounds bounds = hit.collider.bounds;
                //
                //    //if (bounds.min.x > mesh.bounds.min.x && 
                //    //    bounds.max.x > mesh.bounds.max.x &&
                //    //    bounds.min.z > mesh.bounds.min.z &&
                //    //    bounds.max.z > mesh.bounds.max.z)
                //    if (bounds.size.magnitude > mesh.bounds.size.magnitude)
                //    {
                //        GameObject newObj = Instantiate(spawnObjectsLarge[index], hit.collider.ClosestPointOnBounds(hit.point), rot);
                //        spawnCounter++;
                //    }
                //
                //}
           }

           yield return null;
       }        
    }

    void NewPass1()
    {
        float randX = Random.Range(minX, maxX);
        float randY = Random.Range(minY, maxY);
        float randZ = Random.Range(minZ, maxZ);

        Vector3 randDir = Random.onUnitSphere;
        Quaternion rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));
        //Vector3 scale = Vector3.one * Random.Range(minScalePass1, maxScalePass1);

        int index = Random.Range(0, spawnObjectsLarge.Count);

        GameObject newObj = Instantiate(spawnObjectsLarge[index], Vector3.zero, rot);
        //newObj.transform.localScale = scale;
        MeshCollider mesh = newObj.GetComponent<MeshCollider>();
        mainLandMasses.Add(newObj);

        int spawnCounter = 1;

        while (spawnCounter < spawnCountPass1)
        {
            int height = Random.Range(0, heightChance);

            randX = Random.Range(mesh.bounds.size.x, maxDistX);
            //randX = Random.Range(minDistX, maxDistX);

            if (height == 1)
                randY = Random.Range(mesh.bounds.size.y, maxDistY);
            //randY = Random.Range(minDistY, maxDistY);
            else
                randY = 0.0f;

            randZ = Random.Range(mesh.bounds.size.z, maxDistZ);
            //randZ = Random.Range(minDistZ, maxDistZ);

            randDir = Random.onUnitSphere;
            rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));
            index = Random.Range(0, spawnObjectsLarge.Count);

            //Bounds test = spawnObjectsLarge[index].GetComponent<MeshCollider>().bounds;

            Vector3 pos = newObj.transform.TransformPoint(randX, randY, randZ);

            GameObject newObj2 = Instantiate(spawnObjectsLarge[index], pos, rot);
            MeshCollider mesh2 = newObj2.GetComponent<MeshCollider>();

            //Vector3 dir;
            //float dist;
            //if (Physics.ComputePenetration(mesh, newObj.transform.position, newObj.transform.rotation, mesh2, newObj2.transform.position, newObj2.transform.rotation, out dir, out dist))
            //{
            //    print($"dir {dir.ToString()} dist {dist}");
            //    newObj.transform.Translate(dir * dist);
            //}

            //Collider[] colliders = Physics.OverlapBox(pos, mesh2.bounds.size * 0.5f, rot);
            Collider[] colliders = Physics.OverlapBox(pos, newObj2.GetComponent<BoxCollider>().size, rot);
            bool deleted = false;


            if (colliders != null)
            {
                foreach (Collider col in colliders)
                {
                    if (col.gameObject != newObj2)
                    {
                        print($"Hit {col.gameObject.name} for {newObj2.name}");
                        //if (col.gameObject == newObj)
                        //    print("Collider is the previous object");
                        Destroy(newObj2);
                        deleted = true;
                        break;
                    }
                }
            }

            if (!deleted)
            {
                newObj = newObj2;
                mesh = newObj.GetComponent<MeshCollider>();
                //spawnCounter++;
            }

            //newObj = newObj2;
            //mesh = newObj.GetComponent<MeshCollider>();

            spawnCounter++;
        }
    }



    void TerrainPass()
    {
        int spawnCounter = 0;

        while (spawnCounter < spawnCountPass1)
        {
            float randX = Random.Range(0.0f, terrainBound);
            float randZ = Random.Range(0.0f, terrainBound);

            Vector3 randDir = Random.onUnitSphere;
            Quaternion rot = Quaternion.LookRotation(new Vector3(randDir.x, 0.0f, randDir.z));

            int index = Random.Range(0, spawnObjectsLarge.Count);

            MeshCollider mesh = spawnObjectsLarge[index].GetComponent<MeshCollider>();
            RaycastHit hit;

            if (Physics.BoxCast(new Vector3(randX, maxY, randZ), (mesh.bounds.size) * 0.5f, Vector3.down, out hit, rot))
            {
                if (hit.collider.gameObject.name == "Terrain")
                {
                    GameObject newObj = Instantiate(spawnObjects[index], hit.point, rot);
                    //spawnCounter++;
                }                      
                //spawnCounter++;
            }
            spawnCounter++;
        }

    }

    void Pass1() //To-Do make this use a dedicated list of select gameobjects for spawning the massive landmasses
    {
        int spawnCounter = 0;
        
        while (spawnCounter < spawnCountPass1)
        {
            float randX = Random.Range(minX, maxX);
            float randY = Random.Range(minY, maxY);
            float randZ = Random.Range(minZ, maxZ);

            int index = Random.Range(0, spawnObjectsLarge.Count);
            //int chance = Random.Range(0, spawnChancePass1);
            
            MeshCollider mesh = spawnObjectsLarge[index].GetComponent<MeshCollider>();
            BoxCollider box = spawnObjectsLarge[index].GetComponent<BoxCollider>();
            box.enabled = false;

            //Vector3 scale = Vector3.one * Random.Range(minScalePass1, maxScalePass1);
            Vector3 rot = Random.onUnitSphere;
            Vector3 rotation = new Vector3(rot.x, 0.0f, rot.z);                        
            Vector3 newPos = new Vector3(randX, randY, randZ);

            GameObject obj = Instantiate(spawnObjectsLarge[index], newPos, Quaternion.LookRotation(rotation));
            //obj.transform.localScale = scale;            

            //Vector3 size = box.size * scale.x;
            //newPos += (box.center * scale.x);

            //Collider[] colliders = Physics.OverlapBox(newPos, size, Quaternion.LookRotation(rotation));

            Collider[] colliders = Physics.OverlapBox(box.center, box.size, Quaternion.LookRotation(rotation));

            if (colliders != null)
            {
                bool isDeleted = false;

                foreach(Collider col in colliders)
                {
                    if (col.gameObject != obj)
                    {                        
                        Destroy(obj);
                        isDeleted = true;
                        break;
                    }                   
                }

                if (isDeleted)
                    continue;
            }

            mainLandMasses.Add(obj);
            spawnCounter++;
        }
    }

    void Pass2()
    {
        foreach(GameObject obj in mainLandMasses)
        {
            MeshCollider mesh = obj.GetComponent<MeshCollider>();
            BoxCollider box = obj.GetComponent<BoxCollider>();
            Bounds bounds = mesh.bounds;

            //int spawnCounter = 0;
            for (uint i = 0; i < spawnCountPass2; i++)
            //while (spawnCounter < spawnCountPass2)
            {                              
                float objScale = obj.transform.localScale.x;
                
                float randX = Random.Range(bounds.min.x, bounds.max.x);
                float randZ = Random.Range(bounds.min.z, bounds.max.z);
                
                int index = Random.Range(0, spawnObjects.Count);

                MeshCollider objMesh = spawnObjects[index].GetComponent<MeshCollider>();
                
                //Vector3 scale = Vector3.one * Random.Range(minScalePass2, maxScalePass2);
                Vector3 rot = Random.onUnitSphere;
                Vector3 rotation = new Vector3(rot.x, 0.0f, rot.z);

                RaycastHit hit;
                
                if (Physics.BoxCast(new Vector3(randX, bounds.max.y * objScale, randZ), (objMesh.bounds.size) * 0.5f, Vector3.down, out hit, Quaternion.LookRotation(rotation)))
                {
                    GameObject newObj = Instantiate(spawnObjects[index], hit.point, Quaternion.LookRotation(rotation));

                    Vector3 dir;
                    float dist;

                    if (Physics.ComputePenetration(newObj.GetComponent<MeshCollider>(), newObj.transform.position, newObj.transform.rotation, mesh, obj.transform.position, obj.transform.rotation, out dir, out dist))                    
                        newObj.transform.Translate(dir * dist);
                    
                    newObj.transform.parent = obj.transform;
                    //spawnCounter++;
                }
            }            
        }
    }

    void ScalePass()
    {
        foreach(GameObject obj in mainLandMasses)
        {
            MeshCollider mesh = obj.GetComponent<MeshCollider>();
            Vector3 scale = Vector3.one * Random.Range(minScalePass1, maxScalePass1);

            Vector3 size = (mesh.bounds.size * scale.x) * 0.5f;
            Vector3 newPos = mesh.bounds.center * scale.x;

            Collider[] colliders = Physics.OverlapBox(newPos, size, obj.transform.rotation);

            bool intersection = false;

            if (colliders != null)
            {
                foreach (Collider col in colliders)
                {
                    if (col.gameObject != obj && col.gameObject.transform.parent != obj.transform)
                    {
                        intersection = true;
                        break;
                    }
                }
            }

            if (!intersection)
            {
                obj.transform.localScale = scale;
            }

            Transform[] children = obj.GetComponentsInChildren<Transform>();

            foreach(Transform t in children)
            {
                MeshCollider mesh2 = t.GetComponent<MeshCollider>();
                Vector3 scale2 = Vector3.one * Random.Range(minScalePass2, maxScalePass2);

                Vector3 size2 = (mesh2.bounds.size * scale2.x) * 0.5f;
                Vector3 newPos2 = mesh2.bounds.center * scale2.x;

                Collider[] colliders2 = Physics.OverlapBox(newPos2, size2, t.rotation);

                bool intersection2 = false;

                if (colliders2 != null)
                {
                    foreach (Collider col in colliders2)
                    {
                        if (col.gameObject != t.gameObject && col.gameObject != obj)
                        {
                            intersection2 = true;
                            break;
                        }
                    }
                }

                if (!intersection2)
                    t.localScale = scale2;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (gizmoObj != null)
        {
            if (gizmoRay.collider != null)
            {
                //Draw a Ray forward from GameObject toward the hit
                Gizmos.DrawRay(gizmoPos, gizmoDir * gizmoRay.distance);
                //Draw a cube that extends to where the hit exists
                Gizmos.DrawWireCube(gizmoPos + gizmoDir * gizmoRay.distance, gizmoObj.transform.localScale);
            }
            //If there hasn't been a hit yet, draw the ray at the maximum distance
            else
            {
                //Draw a Ray forward from GameObject toward the maximum distance
                Gizmos.DrawRay(gizmoPos, gizmoDir * Mathf.Infinity);
                //Draw a cube at the maximum distance
                Gizmos.DrawWireCube(gizmoPos + gizmoDir * Mathf.Infinity, gizmoObj.transform.localScale);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

/*
Pass 2 old transform TransformPoint solution
//float randX = Random.Range(bounds.min.x * objScale, bounds.max.x * objScale);
//float randZ = Random.Range(bounds.min.z * objScale, bounds.max.z * objScale);

//Vector3 newPos = obj.transform.TransformPoint(randX, randY, randZ);

//GameObject newObj = Instantiate(spawnObjects[index], newPos, Quaternion.LookRotation(rotation));
//newObj.transform.localScale = scale;
//newObj.name = newObj.name + $"On {obj.name}";
*/

/*
 void Pass1Old()
    {
        uint spawnCounter = 0;
        //for (uint i = 0; i < spawnCountPass1; i++)
        while (spawnCounter < spawnCountPass1)
        {
            float randX = Random.Range(minX, maxX);
            float randY = Random.Range(minY, maxY);
            float randZ = Random.Range(minZ, maxZ);
            
            int index = Random.Range(0, spawnObjects.Count);
            int chance = Random.Range(0, spawnChancePass1);

            //if (chance == 1)
            MeshCollider mesh = spawnObjects[index].GetComponent<MeshCollider>();            
            BoxCollider box = spawnObjects[index].GetComponent<BoxCollider>();
            mesh.enabled = false;
            //print($"mesh bounds data: min {mesh.bounds.min} max {mesh.bounds.max} size {mesh.bounds.size} center {mesh.bounds.center}");
            //print($"box bounds data: min {box.bounds.min} max {box.bounds.max} size {box.bounds.size} center {box.bounds.center}");

            //mesh.convex = true;

            Vector3 scale = Vector3.one * Random.Range(minScalePass1, maxScalePass1);
            Vector3 rot = Random.onUnitSphere;
            Vector3 rotation = new Vector3(rot.x, 0.0f, rot.z);
            //Vector3 rotation = Vector3.zero;
            Vector3 newPos = new Vector3(randX, randY, randZ);

            GameObject obj = Instantiate(spawnObjects[index], newPos, Quaternion.LookRotation(rotation));
            obj.transform.localScale = scale;
            Destroy(obj.GetComponent<MeshCollider>());

            Vector3 size = box.size * scale.x;
            newPos += (box.center * scale.x);
            
            //Vector3 size = mesh.bounds.size * 0.5f;            
            //Vector3 rotation = Vector3.zero;

            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.angularDrag = 1000000.0f;
            rb.drag = 1000000.0f;

            //rb.angularDrag = 100.0f;
            //rb.drag = 100.0f;

            mainLandMasses.Add(obj);

            //GameObject debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Destroy(debugCube.GetComponent<BoxCollider>());
            //debugCube.transform.localScale = size;
            //debugCube.transform.rotation = Quaternion.LookRotation(rotation);
            //debugCube.GetComponent<Renderer>().material.color = Color.green;
            //debugCube.transform.position = newPos;

            //RaycastHit hit;
            //
            //if (Physics.BoxCast(newPos, size, Vector3.down, out hit, Quaternion.LookRotation(rotation)))            
            //{
            //    print("Hit an object");
            //    Destroy(obj);
            //    //Destroy(debugCube);
            //    //continue;
            //}
            //
            //if (Physics.BoxCast(newPos, size, Vector3.up, out hit, Quaternion.LookRotation(rotation)))            
            //{
            //    print("Hit an object");
            //    Destroy(obj);
            //    //Destroy(debugCube);
            //    //continue;
            //}

            //if (Physics.BoxCast(new Vector3(randX, maxY, randZ), size, Vector3.down, out hit, Quaternion.LookRotation(rotation)))
            //{
            //    print("Hit an object");
            //    Destroy(obj);
            //    //Destroy(debugCube);
            //    //continue;
            //}
            //
            //if (Physics.BoxCast(new Vector3(randX, minY, randZ), size, Vector3.up, out hit, Quaternion.LookRotation(rotation)))
            //{
            //    print("Hit an object");
            //    Destroy(obj);
            //    //Destroy(debugCube);
            //    //continue;
            //}
            
            //spawnCounter++;
            //if (!Physics.BoxCast(newPos, size, Vector3.down, out hit, Quaternion.LookRotation(rotation)))
            //{
            //    //GameObject debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    //Destroy(debugCube.GetComponent<BoxCollider>());
            //    //debugCube.transform.localScale = size * scale.x;
            //    //debugCube.transform.rotation = Quaternion.LookRotation(rotation);
            //    //debugCube.GetComponent<Renderer>().material.color = Color.red;
            //    //debugCube.transform.position = newPos;
            //
            //    if (!Physics.BoxCast(newPos, size, Vector3.up, out hit, Quaternion.LookRotation(rotation)))
            //    {
            //        GameObject obj = Instantiate(spawnObjects[index], newPos, Quaternion.LookRotation(rotation));
            //        obj.transform.localScale = scale;
            //        spawnCounter++;
            //        //GameObject debugCube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //        //Destroy(debugCube2.GetComponent<BoxCollider>());
            //        //debugCube2.transform.localScale = size * scale.x;
            //        //debugCube2.transform.rotation = Quaternion.LookRotation(rotation);
            //        //debugCube2.GetComponent<Renderer>().material.color = Color.green;
            //        //debugCube2.transform.position = newPos;
            //
            //        //Rigidbody rb = obj.AddComponent<Rigidbody>();
            //        //rb.useGravity = false;
            //        //rb.constraints = RigidbodyConstraints.FreezeRotation;
            //
            //        mainLandMasses.Add(obj);
            //    }
            //}
        }
    }
*/

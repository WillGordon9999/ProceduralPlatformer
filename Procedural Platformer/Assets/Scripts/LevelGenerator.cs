using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public GameObject obj;    
    public bool mustTouchGround; //Must be on top of a floor
    public bool mustTouchObject; //Must be on top of a object
    public bool isFloor;

    [Header("Rotation Settings")]
    public bool canRotate;
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;

    [Header("Scale Settings")]
    public bool canScale;
    public float minScale = 1;
    public float maxScale = 1;

    [HideInInspector] public Bounds bounds;

    public void Init()
    {
        //bounds = LevelGenerator.GetObjectBounds(obj);
    }

}

public enum RandomPassMode { Complete_Floor, Spaced_Objects, Adjacent_Objects, Collectibles }

//A Structure to guide the Level Generator how to construct the level in order
[System.Serializable]
public class RandomPass
{
    public RandomPassMode mode;
    public bool enabled = true;
    public Bounds bounds;
    
    
    public int minObjects;
    public int maxObjects;
    [Header("Platform Settings")]
    public int maxChain = 5;
    public int maxChainAttempts = 10;
    public float minPlatformDistance;
    public float maxPlatformDistance;    

    [Header("Objects")]
    public List<ObjectData> objects;

    [HideInInspector] public GameObject mainFloor;
    [HideInInspector] public Bounds mainFloorBounds;

    public void Init()
    {
        foreach(ObjectData data in objects)
        {
            data.Init();
        }
    }

}


public class LevelGenerator : MonoBehaviour
{
    public Bounds levelBounds;
    public int maxSectionCount = 8;
    public Vector3 minSectionSize;
    public Vector3 maxSectionSize;
    public float boxCastDist = 100.0f;

    public List<RandomPass> passes;

    [Header("Floor Settings")]
    public float minFloorHeight = 1.0f;
    public float maxFloorHeight = 2.0f;

    [Header("Platform Sections")]
    public int minPlatforms = 3;
    public int maxPlatforms = 15;
    public int maxAttempts = 20;
    public float minPlatformDistance;
    public float maxPlatformDistance;
    public float maxDistFromFloor;
    public Vector3 minPlatformSize;
    public Vector3 maxPlatformSize;
    public List<GameObject> groundPlatforms;
    public List<GameObject> floatingPlatforms;

    List<Bounds> levelSections;
    List<GameObject> floors;
    List<GameObject> objects;
    List<AdjacentSpawnData> adjacents; 
    Bounds mainFloor;
    GameObject mainFloorObj;

    bool passComplete = false;

    //GameObject cube;
    //GameObject sphere;
    //GameObject cylinder;
    GameObject cube;
    GameObject parent;

    void Start()
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(cube.GetComponent<BoxCollider>());
        cube.GetComponent<Renderer>().material.color = Color.red;
        parent = new GameObject("Parent");

        levelSections = new List<Bounds>();
        floors = new List<GameObject>();
        objects = new List<GameObject>();
        adjacents = new List<AdjacentSpawnData>();

        foreach(RandomPass pass in passes)
        {
            pass.Init();
        }

        StartCoroutine(MainLoop());
    }
    
    void Update()
    {
        
    }

    IEnumerator MainLoop()
    {       
        foreach(RandomPass pass in passes)
        {
            passComplete = false;
            StartCoroutine(PassLoop(pass));
            yield return new WaitUntil(() => { return passComplete; });
        }


        //int currentCount = 0;
        ////Create Main Floor
        //CreateFloorSection(levelBounds, 1.0f, true);
        //yield return null;
        //StartCoroutine(CreateSpacedPlatformSection(levelBounds, mainFloorObj));
        //yield return null;

        //while (currentCount < maxSectionCount)
        //{
        //    Bounds bounds;
        //    if (CreateLevelSection(out bounds))
        //    {
        //        CreateFloorSection(bounds, Random.Range(minFloorHeight, maxFloorHeight));
        //        currentCount++;
        //    }
        //
        //    yield return null;
        //}
        //
        //for (int i = 0; i < levelSections.Count; i++)
        //{
        //    StartCoroutine(CreateSpacedPlatformSection(levelSections[i], floors[i]));
        //    yield return null;
        //}
    }

    IEnumerator PassLoop(RandomPass pass)
    {
        Collider[] cache = new Collider[5];

        if (!pass.enabled)
        {
            passComplete = true;
            yield break;
        }

        if (pass.mode == RandomPassMode.Complete_Floor)
        {
            int currentCount = 0;
            int targetCount = Random.Range(pass.minObjects, pass.maxObjects + 1);

            while (currentCount < targetCount)
            {                
                ObjectData data = pass.objects[Random.Range(0, pass.objects.Count)];

                Vector3 pos = RandomVector(pass.bounds.min, pass.bounds.max);
                
                //Needs to be instantiated because otherwise the bounds will always be zero
                GameObject newObj = Instantiate(data.obj);
                newObj.transform.position = pos;

                if (data.canRotate)
                {
                    float x = Random.Range(0, 360);
                    float y = Random.Range(0, 360);
                    float z = Random.Range(0, 360);

                    if (!data.rotateX)
                        x = 0.0f;

                    if (!data.rotateY)
                        y = 0.0f;

                    if (!data.rotateZ)
                        z = 0.0f;

                    Quaternion rot = Quaternion.Euler(x, y, z);

                    newObj.transform.rotation = rot;
                }

                if (data.canScale)
                {
                    Vector3 scale = Vector3.one * Random.Range(data.minScale, data.maxScale);
                    newObj.transform.localScale = scale;
                }

                Bounds bounds = newObj.GetComponentInChildren<Collider>().bounds;

                if (bounds.size == Vector3.zero)
                {
                    print("Bounds are zero in Complete Floor");
                }
                
                //int count = Physics.OverlapBoxNonAlloc(pos, data.bounds.extents, cache);                
                Collider[] colliders = Physics.OverlapBox(pos, bounds.extents, newObj.transform.rotation);                                
                if (colliders.Length == 0)
                {                                      
                    floors.Add(newObj);
                    objects.Add(newObj);

                    AdjacentSpawnData adjData = newObj.GetComponent<AdjacentSpawnData>();

                    if (adjData != null)
                    {
                        adjacents.Add(adjData);
                    }

                    currentCount++;
                }

                else
                {
                    Destroy(newObj);                    
                }

                yield return null;
            }
        }
       
        if (pass.mode == RandomPassMode.Spaced_Objects)
        {
            int currentCount = 0;
            int targetCount = Random.Range(pass.minObjects, pass.maxObjects + 1);
            GameObject prevObject = null;
            int attempt = 0;

            while (currentCount < targetCount)
            {                
                ObjectData data = pass.objects[Random.Range(0, pass.objects.Count)];                

                if (data.mustTouchGround)
                {
                    //print("Entering ground");
                    if (HandleSpawnObjectOnTop(pass, data, cache, floors, prevObject, true, ref attempt))
                    {
                        currentCount++;                        
                    }
                }

                if (data.mustTouchObject)
                {
                    //print("Entering object");
                    if (HandleSpawnObjectOnTop(pass, data, cache, objects, prevObject, true, ref attempt))
                    {
                        currentCount++;                        
                    }                  
                }

                if (!data.mustTouchGround && !data.mustTouchObject)
                {
                    Vector3 pos = RandomVector(pass.bounds.min, pass.bounds.max);
                    GameObject newObj = Instantiate(data.obj);
                    newObj.transform.position = pos;

                    if (data.canScale)
                    {
                        Vector3 scale = Vector3.one * Random.Range(data.minScale, data.maxScale);
                        newObj.transform.localScale = scale;
                    }

                    if (data.canRotate)
                    {
                        float x = Random.Range(0, 360);
                        float y = Random.Range(0, 360);
                        float z = Random.Range(0, 360);

                        if (!data.rotateX)
                            x = 0.0f;

                        if (!data.rotateY)
                            y = 0.0f;

                        if (!data.rotateZ)
                            z = 0.0f;

                        Quaternion rot = Quaternion.Euler(x, y, z);

                        newObj.transform.rotation = rot;
                    }

                    Bounds bounds = newObj.GetComponentInChildren<Collider>().bounds;

                    //int count = Physics.OverlapBoxNonAlloc(pos, data.bounds.extents, cache);
                    Collider[] colliders = Physics.OverlapBox(pos, bounds.extents, newObj.transform.rotation);

                    //if (count == 0)
                    if (colliders.Length == 0)
                    {
                        if (data.isFloor)
                            floors.Add(newObj);

                        objects.Add(newObj);

                        AdjacentSpawnData adjData = newObj.GetComponent<AdjacentSpawnData>();

                        if (adjData != null)
                        {
                            adjacents.Add(adjData);
                        }

                        currentCount++;
                    }

                    else
                    {
                        Destroy(newObj);                        
                    }
                }
                
                yield return null;
            }

        }

        if (pass.mode == RandomPassMode.Adjacent_Objects)
        {
            print("In Adjacent Objects");
            foreach(AdjacentSpawnData adjData in adjacents)
            {
                print("In Adjacent Objects foreach");
                int currentCount = 0;
                int targetCount = Random.Range(adjData.minCount, adjData.maxCount + 1);
                GameObject prevObject = null;
                int attempt = 0;
                
                while (currentCount <= targetCount)
                {
                    print("In Adjacent Objects while loop");
                    int index = Random.Range(0, pass.objects.Count);
                    AdjacentData data = adjData.objects[index];

                    if (attempt >= maxAttempts)
                    {
                        print("Attempt limit reached on Adjacent");
                        break;
                    }

                    if (HandleSpawnAdjacentObject(adjData, data, prevObject, false, ref attempt))
                    {
                        print("Successfully added an object in adjacent");
                        currentCount++;
                    }

                    yield return null;
                }

                yield return null;
            }
           
        }

        if (pass.mode == RandomPassMode.Collectibles)
        {
            int currentCount = 0;
            int targetCount = Random.Range(pass.minObjects, pass.maxObjects + 1);
            GameObject prevObject = null;
            int attempt = 0;

            while (currentCount <= targetCount)
            {
                int index = Random.Range(0, pass.objects.Count);
                ObjectData data = pass.objects[index];

                if (HandleSpawnObjectOnTop(pass, data, cache, objects, prevObject, false, ref attempt))
                {
                    currentCount++;
                }

                yield return null;
            }
        }

        yield return null;
        print("Pass Complete, Moving To Next");
        passComplete = true;
    }

    bool HandleSpawnObjectOnTop(RandomPass pass, ObjectData data, Collider[] cache, List<GameObject> theObjects, GameObject prevObject, bool addToObjects, ref int attempt)
    {
        //Spawn the new Object to get it's bounds
        GameObject newObj = Instantiate(data.obj);
        if (data.canScale)
        {
            Vector3 scale = Vector3.one * Random.Range(data.minScale, data.maxScale);
            newObj.transform.localScale = scale;
        }

        if (data.canRotate)
        {
            float x = Random.Range(0, 360);
            float y = Random.Range(0, 360);
            float z = Random.Range(0, 360);

            if (!data.rotateX)
                x = 0.0f;

            if (!data.rotateY)
                y = 0.0f;

            if (!data.rotateZ)
                z = 0.0f;

            Quaternion rot = Quaternion.Euler(x, y, z);

            newObj.transform.rotation = rot;
        }

        Bounds bounds = newObj.GetComponentInChildren<Collider>().bounds;
        data.bounds = bounds;

        Vector3 pos = Vector3.zero;

        //Choose the Object to spawn on top of
        GameObject target = theObjects[Random.Range(0, theObjects.Count)];
        Bounds targetBounds = target.GetComponentInChildren<Collider>().bounds;

        //print($"target name {target.name} Pass Type {pass.mode.ToString()}");

        //Handle Chain Spawning
        if (prevObject != null)
        {
            if (attempt >= pass.maxChainAttempts)
            {
                prevObject = null;
                attempt = 0;
            }
        
            else
            {
                Vector3 dir = Random.onUnitSphere * Random.Range(pass.minPlatformDistance, pass.maxPlatformDistance);
                pos = prevObject.transform.TransformPoint(dir);
                newObj.transform.position = pos;
            }
        }

        //If Selecting a random point
        if (prevObject == null)
        {
            Vector3 min = new Vector3(targetBounds.min.x, targetBounds.max.y, targetBounds.min.z);
            Vector3 max = targetBounds.max;

            pos = RandomVector(min, max);
            pos.y += data.bounds.extents.y;
            newObj.transform.position = pos;
        }

        if (CheckToSpawnObjectOnTop(pass, newObj, data, target, cache, ref pos))
        {            
            if (data.isFloor)
                floors.Add(newObj);

            if (addToObjects)
                objects.Add(newObj);

            prevObject = newObj;
            return true;
        }

        else
        {
            Destroy(newObj);
            attempt++;
        }

        return false;
    }

    bool CheckToSpawnObjectOnTop(RandomPass pass, GameObject newObj, ObjectData data, GameObject target, Collider[] cache, ref Vector3 pos, bool keepInBounds = true)
    {        
        //Bounds bounds = GetObjectBounds(target);

        if (data.bounds.size == Vector3.zero)
        {
            pos = Vector3.zero;
            return false;
        }
        
        //Make sure you're actually on collider
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit, data.bounds.size.y + 10.0f))                
        {
            if (hit.collider.gameObject == target)
            {                
                //Get the bottom point on the collider no matter what
                Collider col = newObj.GetComponentInChildren<Collider>();
                Vector3 min = col.bounds.min;
                Vector3 max = new Vector3(col.bounds.max.x, col.bounds.min.y, col.bounds.max.z);
                parent.transform.position = newObj.transform.position + Vector3.Lerp(min, max, 0.5f);
                
                newObj.transform.parent = parent.transform;
                parent.transform.position = hit.point;
                newObj.transform.parent = null;

                //parent.transform.position = newObj.transform.TransformPoint(Vector3.down * col.bounds.extents.y); 
                //int count = Physics.OverlapBoxNonAlloc(pos, data.bounds.size, cache);

                //Ensure not colliding with anything else
                //Collider[] colliders = Physics.OverlapBox(newObj.transform.position, data.bounds.size);
                Collider[] colliders = Physics.OverlapBox(newObj.transform.position, data.bounds.extents, newObj.transform.rotation);

                if (colliders.Length == 1) 
                {
                    if (colliders[0].gameObject == target)
                    {
                        //Ensure Object on Top stays within the X and Z of the target object
                        if (keepInBounds)
                        {
                            Bounds targetBounds = target.GetComponentInChildren<Collider>().bounds;
                            Vector3 objPos = newObj.transform.position;

                            Vector3 objMin = new Vector3(objPos.x + col.bounds.min.x, 0.0f, objPos.z + col.bounds.min.z);
                            Vector3 objMax = new Vector3(objPos.x + col.bounds.max.x, 0.0f, objPos.z + col.bounds.max.z);

                            //Vector3 objMin = new Vector3(col.bounds.min.x, 0.0f, col.bounds.min.z);
                            //Vector3 objMax = new Vector3(col.bounds.max.x, 0.0f, col.bounds.max.z);

                            Vector3 targetMin = new Vector3(targetBounds.min.x, 0.0f, targetBounds.min.z);
                            Vector3 targetMax = new Vector3(targetBounds.max.x, 0.0f, targetBounds.max.z);

                            Bounds final = new Bounds();
                            final.SetMinMax(targetMin, targetMax);

                            //print($"col min {newObj.transform.position + col.bounds.min} col max {newObj.transform.position + col.bounds.max} target min {targetBounds.min} target max {targetBounds.max}");                            
                            //print($"{objMin} {objMax} {targetMin} {targetMax}");

                            //if (targetBounds.Contains(objMin) && targetBounds.Contains(objMax))
                            if (final.Contains(objMin) && final.Contains(objMax))
                            {
                                return true;
                            }

                            else
                            {
                                return false;
                            }
                        }

                        else
                        {
                            return true;
                        }
                    }
                }                
            }
        }

        return false;
    }

    public bool HandleSpawnAdjacentObject(AdjacentSpawnData spawnData, AdjacentData adjData, GameObject prevObject, bool addToObjects, ref int attempts)
    {
        //Calculate the center of the Object
        GameObject newObj = Instantiate(adjData.obj);

        if (adjData.canScale)
        {
            Vector3 scale = Vector3.one * Random.Range(adjData.minScale, adjData.maxScale);
            newObj.transform.localScale = scale;
        }

        if (adjData.canRotate)
        {
            float x = Random.Range(0, 360);
            float y = Random.Range(0, 360);
            float z = Random.Range(0, 360);

            if (!adjData.rotateX)
                x = 0.0f;

            if (!adjData.rotateY)
                y = 0.0f;

            if (!adjData.rotateZ)
                z = 0.0f;

            Quaternion rot = Quaternion.Euler(x, y, z);

            newObj.transform.rotation = rot;
        }

        Collider col = newObj.GetComponentInChildren<Collider>();
        Vector3 min = col.bounds.min;
        Vector3 max = col.bounds.max;
        Vector3 center = newObj.transform.position + Vector3.Lerp(min, max, 0.5f);

        //Now Calculate the adjacent point to put this object
        Bounds targetBounds = spawnData.gameObject.GetComponentInChildren<Collider>().bounds;

        //Get the Center of the Bottom of the target object
        //Vector3 targetMin = targetBounds.min;
        //Vector3 targetMax = new Vector3(targetBounds.max.x, targetBounds.min.y, targetBounds.max.z);
        //Vector3 targetBottomCenter = spawnData.gameObject.transform.position + Vector3.Lerp(targetMin, targetMax, 0.5f);

        //float randX = Mathf.Lerp(targetBounds.min.x, targetBounds.max.x, Random.Range(adjData.minXBounds, adjData.maxXBounds));
        //float randY = Mathf.Lerp(targetBounds.min.y, targetBounds.max.y, Random.Range(adjData.minYBounds, adjData.maxYBounds));
        //float randZ = Mathf.Lerp(targetBounds.min.z, targetBounds.max.z, Random.Range(adjData.minZBounds, adjData.maxZBounds));
        //
        //Vector3 pos = spawnData.gameObject.transform.position + new Vector3(randX, randY, randZ);

        float xRand = Random.Range(0, 360);
        float zRand = Random.Range(0, 360);
        float randY = Random.Range(targetBounds.min.y, targetBounds.max.y);

        Vector3 pos = new Vector3(Mathf.Cos(xRand * Mathf.Deg2Rad), randY, Mathf.Sin(zRand * Mathf.Deg2Rad));
        pos *= targetBounds.extents.magnitude;
        pos = spawnData.gameObject.transform.TransformPoint(pos);

        Collider[] colliders = Physics.OverlapBox(pos, col.bounds.extents, newObj.transform.rotation);

        if (colliders.Length == 1)
        {
            if (colliders[0].gameObject == spawnData.gameObject)
            {
                //Perform some kind of raycast here to ensure it's on the edge of the target bounds
                newObj.transform.position = pos;
                //newObj.transform.position = spawnData.gameObject.transform.TransformPoint(pos);
                objects.Add(newObj);
                return true;
            }
        }

        if (colliders.Length == 0)
        {
            newObj.transform.position = pos;
            //newObj.transform.position = spawnData.gameObject.transform.TransformPoint(pos);
            objects.Add(newObj);
            return true;
        }

        Destroy(newObj);

        return false;
    }

    public static Bounds GetObjectBounds(GameObject obj)
    {
        if (obj != null)
        {
            Collider col = obj.GetComponent<Collider>();

            if (col != null)
            {
                //print("Get Object Bounds collider not null");
                return col.bounds;
            }

            else
            {
                col = obj.GetComponentInChildren<Collider>();

                if (col != null)
                {
                    //print("Get Object Bounds returning collider in child");
                    return col.bounds;
                }

                else
                {
                    //Debug.LogError($"{obj.name} does not have any colliders");
                    //print("Get Object Bounds, no collider found");
                    return new Bounds();
                }
            }
        }

        else
        {
            Debug.LogError("Obj is null in ObjectData");
            return new Bounds();
        }
    }

    //Create the individual section bounds of the levels here
    public bool CreateLevelSection(out Bounds theBounds)
    {
        //Vector3 min = levelBounds.center - levelBounds.extents;
        //Vector3 max = levelBounds.center + levelBounds.extents;
        Vector3 min = levelBounds.min;
        Vector3 max = levelBounds.max;

        //Vector3 newCenter = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        Vector3 newCenter = RandomVector(min, max);

        //Vector3 newSize = new Vector3(Random.Range(minSectionSize.x, maxSectionSize.x), Random.Range(minSectionSize.y, maxSectionSize.y), Random.Range(minSectionSize.z, maxSectionSize.z));
        Vector3 newSize = RandomVector(minSectionSize, maxSectionSize);

        Bounds newBounds = new Bounds(newCenter, newSize);
        theBounds = newBounds;

        //Is it within the overall bounds?
        if (levelBounds.Contains(newBounds.min) && levelBounds.Contains(newBounds.max))
        {
            bool clear = true;

            if (newBounds.Intersects(mainFloor))
            {
                return false;
            }

            foreach(Bounds section in levelSections)
            {
                if (newBounds.Intersects(section))
                {
                    clear = false;
                    break;
                }
            }

            if (clear)
            {
                levelSections.Add(newBounds);                
                return true;
            }
        }
        
        return false;

    }

    //Create a whole rectangle floor space for a section, no gaps here
    public void CreateFloorSection(Bounds sectionBounds, float floorHeight, bool createMainFloor = false)
    {
        GameObject newFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //Vector3 extents = sectionBounds.extents;
        Vector3 size = sectionBounds.size;
        size.y = floorHeight;

        Vector3 center = sectionBounds.center;
        center.y = sectionBounds.min.y + (floorHeight * 0.5f);

        newFloor.transform.localScale = size;
        newFloor.transform.position = center;
        newFloor.transform.parent = transform;

        newFloor.GetComponent<Renderer>().material.color = Random.ColorHSV();

        if (createMainFloor)
        {
            mainFloor = new Bounds(center, size);            
            mainFloorObj = newFloor;
        }

        else
        {
            floors.Add(newFloor);
        }

    }

    IEnumerator CreateSpacedPlatformSection(Bounds sectionBounds, GameObject floor)
    {        
        int count = 0;
        int platformCount = Random.Range(minPlatforms, maxPlatforms + 1);
        GameObject previousPlatform = null;
        Collider[] cache = new Collider[5];
        Color color = floor.GetComponent<Renderer>().material.color;

        while (count < platformCount)
        {
            Vector3 min = sectionBounds.min;
            Vector3 max = sectionBounds.max;
            int num = Random.Range(0, 2);
            bool mustTouchGround = (num == 1) ? true : false;

            if (previousPlatform == null)
            {
                Vector3 center = RandomVector(min, max);
                Vector3 size = RandomVector(minPlatformSize, maxPlatformSize);
                Bounds newBounds = new Bounds(center, size);

                if (sectionBounds.Contains(newBounds.min) && sectionBounds.Contains(newBounds.max))
                {
                    Vector3 closestPoint = floor.GetComponent<Collider>().ClosestPointOnBounds(center);

                    if (Vector3.Distance(center, closestPoint) <= maxDistFromFloor)
                    {
                        int colliderCount = Physics.OverlapBoxNonAlloc(newBounds.center, newBounds.extents, cache);

                        if (colliderCount == 0)
                        {
                            count++;
                            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            obj.transform.position = newBounds.center;
                            obj.transform.localScale = newBounds.size;
                            obj.transform.parent = floor.transform;
                            obj.GetComponent<Renderer>().material.color = color;
                            previousPlatform = obj;
                            yield return null;
                        }
                    }
                }
            }

            else
            {
                Vector3 dir = Random.onUnitSphere;
                float dist = Random.Range(minPlatformDistance, maxPlatformDistance);

                Vector3 center = previousPlatform.transform.TransformPoint(dir * dist);
                Vector3 size = RandomVector(minPlatformSize, maxPlatformSize);
                Bounds newBounds = new Bounds(center, size);

                if (sectionBounds.Contains(newBounds.min) && sectionBounds.Contains(newBounds.max))
                {
                    int colliderCount = Physics.OverlapBoxNonAlloc(newBounds.center, newBounds.extents, cache);

                    if (colliderCount == 0)
                    {
                        count++;
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        obj.transform.position = newBounds.center;
                        obj.transform.localScale = newBounds.size;
                        obj.transform.parent = floor.transform;
                        obj.GetComponent<Renderer>().material.color = color;
                        previousPlatform = obj;
                        yield return null;
                    }
                }
            }
                        
            yield return null;
        }
        
    }

    Vector3 RandomVector(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

}

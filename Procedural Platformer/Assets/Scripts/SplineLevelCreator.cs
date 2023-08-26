using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
//using Sirenix.OdinInspector;

#if UNITY_EDITOR
/*
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.UI;

[CustomPropertyDrawer(typeof(SplinePass))]
public class SplinePassDrawer : PropertyDrawer
{
    SerializedProperty prop;
    Rect rect;
    bool foldout = false;

    void FoldOut(Rect rect)
    {
        foldout = !foldout;
        this.rect = rect;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        prop = property;
        rect = position;
        EditorGUI.BeginProperty(position, label, property);

        //EditorGUI.BeginFoldoutHeaderGroup(position, foldout, label.text, null, FoldOut);
        //rect.height = EditorGUI.GetPropertyHeight(property);
        rect.height = 800;
        //foldout = EditorGUI.Foldout(rect, foldout, label);

        if (foldout)
        {
            SerializedProperty passModeProp = property.FindPropertyRelative(nameof(SplinePass.passMode));
            PassMode mode = (PassMode)passModeProp.intValue;

            Draw(nameof(SplinePass.name));
            Draw(nameof(SplinePass.enabled));
            Draw(nameof(SplinePass.passMode));
            Draw(nameof(SplinePass.runMode));
            Draw(nameof(SplinePass.spawnOptions));
            Draw(nameof(SplinePass.waitUntilComplete));

            if (mode == PassMode.OriginalSpline)
            {
                RenderSplineCommon(property);
                Draw(nameof(SplinePass.splineObjectPrefabs));
                Draw(nameof(SplinePass.subpasses));
            }

            else if (mode == PassMode.AdditiveSpline)
            {
                RenderSplineCommon(property);
                Draw(nameof(SplinePass.spawnOnTopRayDist));
                Draw(nameof(SplinePass.maxRayAttempts));
                Draw(nameof(SplinePass.maxHeightDiff));

                Draw(nameof(SplinePass.splineObjectPrefabs));
                Draw(nameof(SplinePass.subpasses));
            }

            else if (mode == PassMode.PointSpawn)
            {
                Draw(nameof(SplinePass.maxMainPassSpawnCount));
                Draw(nameof(SplinePass.spawnOnTopRayDist));

                Draw(nameof(SplinePass.maxRayAttempts));
                Draw(nameof(SplinePass.objectSpawnCount));

                Draw(nameof(SplinePass.canRotate));
                Draw(nameof(SplinePass.minRotation));
                Draw(nameof(SplinePass.maxRotation));

                Draw(nameof(SplinePass.canScale));
                Draw(nameof(SplinePass.minScale));
                Draw(nameof(SplinePass.maxScale));

                Draw(nameof(SplinePass.splineObjectPrefabs));
                Draw(nameof(SplinePass.subpasses));
            }

            else if (mode == PassMode.Stack)
            {
                Draw(nameof(SplinePass.maxMainPassSpawnCount));
                Draw(nameof(SplinePass.minObjectSpawnCount));
                Draw(nameof(SplinePass.maxObjectSpawnCount));

                Draw(nameof(SplinePass.spawnOnTopRayDist));
                Draw(nameof(SplinePass.maxRayAttempts));
                Draw(nameof(SplinePass.stackRayDist));
                Draw(nameof(SplinePass.stackRayOffset));

                Draw(nameof(SplinePass.splineObjectPrefabs));
                Draw(nameof(SplinePass.subpasses));
            }

            else if (mode == PassMode.Adjacents)
            {
                Draw(nameof(SplinePass.maxMainPassSpawnCount));
                Draw(nameof(SplinePass.minObjectSpawnCount));
                Draw(nameof(SplinePass.maxObjectSpawnCount));
                Draw(nameof(SplinePass.splineObjectPrefabs));
                Draw(nameof(SplinePass.subpasses));
            }
        }
        //EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    void Draw(string name)
    {
        SerializedProperty field = prop.FindPropertyRelative(name);
        //Rect rect = this.rect;
        //rect.height = EditorGUI.GetPropertyHeight(field);
        EditorGUI.PropertyField(rect, field);
        rect.y += 20.0f;
    }

    void RenderSplineCommon(SerializedProperty property)
    {
        Draw(nameof(SplinePass.maxMainPassSpawnCount));
        Draw(nameof(SplinePass.useStartingPos));
        Draw(nameof(SplinePass.startPos));
        Draw(nameof(SplinePass.canRotateSpline));
        Draw(nameof(SplinePass.minSplineRotation));
        Draw(nameof(SplinePass.maxSplineRotation));
        Draw(nameof(SplinePass.splinePointCount));
        Draw(nameof(SplinePass.objectSpawnCount));
        Draw(nameof(SplinePass.minSplinePointDist));
        Draw(nameof(SplinePass.maxSplinePointDist));
        Draw(nameof(SplinePass.minAngles));
        Draw(nameof(SplinePass.maxAngles));
        Draw(nameof(SplinePass.pointSize));
        Draw(nameof(SplinePass.normal));
        Draw(nameof(SplinePass.useRandomNormal));
        Draw(nameof(SplinePass.minNormal));
        Draw(nameof(SplinePass.maxNormal));
        Draw(nameof(SplinePass.useObjectDist));
        Draw(nameof(SplinePass.minObjectDist));
        Draw(nameof(SplinePass.maxObjectDist));
        Draw(nameof(SplinePass.useRandomSeedRange));
        Draw(nameof(SplinePass.minSeedRange));
        Draw(nameof(SplinePass.maxSeedRange));
        Draw(nameof(SplinePass.canOffset));
        Draw(nameof(SplinePass.minOffset));
        Draw(nameof(SplinePass.maxOffset));
        Draw(nameof(SplinePass.canRotate));
        Draw(nameof(SplinePass.minRotation));
        Draw(nameof(SplinePass.maxRotation));
        Draw(nameof(SplinePass.canScale));
        Draw(nameof(SplinePass.minScale));
        Draw(nameof(SplinePass.maxScale));
    }
}
//*/
#endif

public enum RunMode { None, EndOfCycle, EndOfPass }
public enum PassMode { OriginalSpline, AdditiveSpline, PointSpawn, Stack, Adjacents, NewSubSpline }
public enum SpawnOptions { Default, SpawnedObjects, AllSpawnedObjects, UseTargetObject, UseTargetSpline }
public enum SpawnSource { Default, SplinePrefabs, ObjectList, Stackables, Adjacents }

[System.Serializable]
public class SplinePass
{    
    public string name = "";    
    public bool enabled = true;

    //public bool newPointSpawn = false;    
    //public bool newSplineSpawn4 = false;
    //public bool newSplineSpawn5 = false;

    [Space(10)]
    [Header("Pass Mode Control")]
    public PassMode passMode;
    public RunMode runMode = RunMode.None;
    public SpawnOptions spawnOptions = SpawnOptions.Default;
    public SpawnSource spawnSource = SpawnSource.Default;
    public bool waitUntilComplete = false;
    public bool runSubPassesAsync = false;
    //public bool stack = false;
    //public bool adjacents = false;

    //[Space(10)]
    //public bool treasureSplineSpawn = false;
    //public GameObject treasurePrefab;
    
    [Space(20)]
    [Header("========== Spline Spawn Settings ==========")]          
    public int maxMainPassSpawnCount = 50;
    public bool useStartingPos = true;        
    public GameObject splinePrefab;        
    public float maxHeightDiff = 2.0f;

    [Space(10)]
    [Header("Main Land Specific")]
    public bool offsetSplineObject = false;
    public Vector3 splineOffset = new Vector3(0, 0, 0);
    public float neighborCastHeight = 10.0f;
    public float maxNeighborCastDist = 100.0f;
    public float neighborScale = 3.0f;
    public float pruneWaitTime = 0.3f;

    [Space(10)]
    [Header("Sub Spline Specific")]
    public bool useAltSubSpline = false;
    public bool useOrigIndex = false;
    public bool useOrigY = false;
    public bool useSpawnedObjects = true;

    [Space(10)]
    public bool canRotateSpline = true;
    public Vector3 minSplineRotation = new Vector3(0, 0, 0);
    public Vector3 maxSplineRotation = new Vector3(0, 360.0f, 0);

    //////////////////////////

    [Space(20)]
    [Header("========== Main Spline Settings ==========")]     
    public int splinePointCount = 5;    
    public int objectSpawnCount = 10;  
    
    public int minObjectSpawnCount = 5;    
    public int maxObjectSpawnCount = 10;

    [Space(10)]    
    public float minSplinePointDist = 3.0f;    
    public float maxSplinePointDist = 10.0f;

    [Space(10)]    
    public Vector3 minAngles = new Vector3(-90f, 0.0f, -90f);    
    public Vector3 maxAngles = new Vector3(90f, 0.0f, 90f);    
    public float pointSize = 1.0f;

    [Space(10)]    
    public Vector3 normal = new Vector3(0, 1, 0);    
    public bool useRandomNormal = false;    
    public Vector3 minNormal = new Vector3(-1, -1, -1);    
    public Vector3 maxNormal = new Vector3(1, 1, 1);

    [Space(10)]
    
    public bool useObjectDist = true;    
    public float minObjectDist = 3.0f;    
    public float maxObjectDist = 10.0f;
  
    [Space(10)]
    
    public bool useRandomSeedRange = false;    
    public int minSeedRange = 0;    
    public int maxSeedRange = 99999999;

    ///////////////////////////

    [Space(20)]
    [Header("========== Object Spawn Settings ==========")]
    public float spawnOnTopRayDist = 0.3f;
    public int maxRayAttempts = 10;
    public int maxPassAttempts = 200;

    [Space(10)]
    [Header("Offset")]
    public bool canOffset = true;
    public Vector3 minOffset = new Vector3(0, 0, 0);
    public Vector3 maxOffset = new Vector3(20, 0, 20);

    [Space(10)]
    [Header("Rotate")]
    public bool canRotate = true;
    public Vector3 minRotation = new Vector3(0, 0, 0);
    public Vector3 maxRotation = new Vector3(0, 360.0f, 0);

    [Space(10)]
    [Header("Scale")]
    public bool canScale = true;
    public float minScale = 1.0f;
    public float maxScale = 3.0f;

    [Space(10)]
    [Header("Stack Specific")]    
    public Vector3 stackRayOffset = new Vector3(0, 1, 0);    
    public float stackRayDist = 3.0f;
    public int stackSpawnChance = 30;

    //[Space(10)]    
    public List<GameObject> splineObjectPrefabs;
    public SpawnObjectList spawnObjects;

    //[Space(10)]
    //public SubSplinePass subPass;

    //[Space(10)]
    public List<SplinePass> subpasses;

    public Vector3 GetRandomVector(Vector3 min, Vector3 max)
    {
        float randX = Random.Range(min.x, max.x);
        float randY = Random.Range(min.y, max.y);
        float randZ = Random.Range(min.z, max.z);
        return new Vector3(randX, randY, randZ);
    }

    public Quaternion GetRandomRotation(Vector3 min, Vector3 max)
    {
        float randRotX = Random.Range(min.x, max.x);
        float randRotY = Random.Range(min.y, max.y);
        float randRotZ = Random.Range(min.z, max.z);
        Quaternion randRot = Quaternion.Euler(new Vector3(randRotX, randRotY, randRotZ));
        return randRot;
    }

}



public class SplineLevelCreator : MonoBehaviour
{
    public static SplineLevelCreator Instance { get { return mInstance; } private set { } }
    private static SplineLevelCreator mInstance;

    public GameObject splinePrefab;
    public Vector3 playerStartPos = new Vector3(0, 100, 0);
    public bool regenerate = false;  
            
    public List<SplinePass> passes;

    [Header("Debug")]
    public bool topRightDebug;
    public bool topLeftDebug;
    public bool downRightDebug;
    public bool downLeftDebug;
    public List<GameObject> spawnedObjects;
    public List<SplineComputer> splineObjects;
    public List<GameObject> allSpawnedObjects;
    public List<GameObject> pendingObjects;
    bool passComplete = false;
    List<GameObject> landSpawns;

    //SplineComputer spline;
    //SplinePoint[] splinePoints;
    GameObject currentSpline;
    GameObject player;
    GameObject currentSpawn;
    GameObject spawnHelper;
    BoxCollider helperBox;
    GameObject splineHelper;
    bool newSplinePassComplete = false;
    bool canRunAdjacents = false;
    bool canRunStack = false;

    //Literally a wrapper for a bool so we can pass them by reference to a Coroutine which doesn't allow ref
    class PassBool
    {
        public bool val = false;
    }

    private void Awake()
    {
        if (mInstance == null)
            mInstance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spawnedObjects = new List<GameObject>();
        allSpawnedObjects = new List<GameObject>();
        splineObjects = new List<SplineComputer>();
        pendingObjects = new List<GameObject>();
        spawnHelper = new GameObject("Spawn Helper");
        splineHelper = new GameObject("Spline Helper");

        helperBox = spawnHelper.AddComponent<BoxCollider>();
        helperBox.isTrigger = true;

        StartCoroutine(CreateSplines());
    }

    private void Update()
    {
        if (regenerate)
        {
            StopAllCoroutines();

            foreach(GameObject obj in pendingObjects)
            {
                Destroy(obj);
            }

            foreach (GameObject obj in allSpawnedObjects) //was originally spawnedObjects
            {
                Destroy(obj);
            }

            foreach (SplineComputer spline in splineObjects)
            {
                Destroy(spline.gameObject);
            }

            allSpawnedObjects.Clear();
            spawnedObjects.Clear();
            pendingObjects.Clear();
            splineObjects.Clear();

            if (player != null)
            {
                player.transform.position = playerStartPos;
            }

            regenerate = false;
            StartCoroutine(CreateSplines());
        }
    }

    IEnumerator CreateSplines()
    {
        for (int i = 0; i < passes.Count; i++)
        {
            if (passes[i].enabled)
            {
                //passComplete = false;
                PassBool complete = new PassBool();
                StartCoroutine(MainPass(passes[i], null, null, Vector3.zero, complete));
                yield return new WaitUntil(() => { return complete.val; });
            }
        }
    }

    IEnumerator MainPass(SplinePass pass, GameObject targetSpline, GameObject targetObj, Vector3 startPos, PassBool mainComplete)
    {
        int spawnCount = 0;

        while (spawnCount < pass.maxMainPassSpawnCount)
        {
            Vector3 randPos = Vector3.zero;
            //Collider collider = null;
            //bool canSpawn = true;

            //if (pass.newPointSpawn)
            if (pass.passMode == PassMode.PointSpawn)
            {
                //newSplinePassComplete = false;
                PassBool complete = new PassBool();
                StartCoroutine(NewPointSpawn(pass, targetSpline, targetObj, startPos, complete));
                //yield return new WaitUntil(() => { return newSplinePassComplete; });
                if (pass.waitUntilComplete)                    
                    yield return new WaitUntil(() => { return complete.val; });

                spawnCount++;
                continue;
            }

            //if (pass.newSplineSpawn5)
            if (pass.passMode == PassMode.AdditiveSpline)
            {
                //newSplinePassComplete = false;
                PassBool complete = new PassBool();
                StartCoroutine(NewSplineSetup5(pass, targetSpline, targetObj, startPos, complete));
                //yield return new WaitUntil(() => { return newSplinePassComplete; });
                if (pass.waitUntilComplete)
                    yield return new WaitUntil(() => { return complete.val; });

                spawnCount++;
                continue;
            }

            if (pass.passMode == PassMode.Stack)
            {
                PassBool complete = new PassBool();
                //StartCoroutine(StackSpline(pass, targetSpline, targetObj, startPos, complete));
                StartCoroutine(NewStackSpline(pass, targetSpline, targetObj, startPos, complete));

                if (pass.waitUntilComplete)
                    yield return new WaitUntil(() => { return complete.val; });

                spawnCount++;
                continue;
            }

            if (pass.passMode == PassMode.Adjacents)
            {
                PassBool complete = new PassBool();
                StartCoroutine(SpawnWallAttachments(pass, targetSpline, targetObj, startPos, complete));

                if (pass.waitUntilComplete)
                    yield return new WaitUntil(() => { return complete.val; });

                spawnCount++;
                continue;
            }

            if (pass.passMode == PassMode.NewSubSpline)
            {
                PassBool complete = new PassBool();
                StartCoroutine(NewSubSpline(pass, targetSpline, targetObj, startPos, complete));

                if (pass.waitUntilComplete)
                    yield return new WaitUntil(() => { return complete.val; });

                spawnCount++;
                continue;
            }

            //if (canSpawn)
            if (pass.passMode == PassMode.OriginalSpline)
            {
                Quaternion randRot = Quaternion.identity;

                if (pass.canRotateSpline)
                    randRot = pass.GetRandomRotation(pass.minSplineRotation, pass.maxSplineRotation);

                GameObject prefab = (pass.splinePrefab != null) ? pass.splinePrefab : splinePrefab;
                GameObject newSpline = Instantiate(prefab, randPos, randRot);

                PassBool complete = new PassBool();
                //newSplinePassComplete = false;
                StartCoroutine(SetupSpline(pass, newSpline, targetObj, startPos, complete));
                //yield return new WaitUntil(() => { return newSplinePassComplete; });
                if (pass.waitUntilComplete)
                    yield return new WaitUntil(() => { return complete.val; });

                spawnCount++;
            }

            yield return null;
        }

        if (pass.runMode == RunMode.EndOfPass)
        {
            if (pass.subpasses != null && pass.subpasses.Count > 0)
            {
                if (pass.runSubPassesAsync)
                {
                    StartCoroutine(RunSubPassAsync(pass.subpasses, targetSpline, targetObj, Vector3.zero));
                }

                else
                {
                    for (int i = 0; i < pass.subpasses.Count; i++)
                    {
                        if (pass.subpasses[i].enabled)
                        {
                            PassBool complete = new PassBool();
                            //StartCoroutine(MainSubPass(pass.subpasses[i], null, Vector3.zero, complete));
                            StartCoroutine(MainPass(pass.subpasses[i], targetSpline, targetObj, Vector3.zero, complete));

                            if (pass.waitUntilComplete)
                                yield return new WaitUntil(() => { return complete.val; });
                        }
                    }
                }
                
            }
        }

        //passComplete = true;
        mainComplete.val = true;
    }

    IEnumerator RunSubPassAsync(List<SplinePass> passes, GameObject targetSpline, GameObject targetObj, Vector3 startPos, List<GameObject> objectList = null)
    {
        if (passes != null && passes.Count > 0)
        {
            for (int i = 0; i < passes.Count; i++)
            {
                if (passes[i].enabled)
                {
                    PassBool subComplete = new PassBool();
                    StartCoroutine(MainPass(passes[i], targetSpline, targetObj, startPos, subComplete));

                    if (passes[i].waitUntilComplete)
                        yield return new WaitUntil(() => { return subComplete.val; });
                }
            }
        }
    }

    IEnumerator NewPointSpawn(SplinePass pass, GameObject targetSpline, GameObject targetObj, Vector3 startPos, PassBool complete, List<GameObject> objectList = null)
    {
        while (spawnedObjects.Count < 1)
            yield return null;

        //newSplinePassComplete = false;

        bool foundSpot = false;

        while (!foundSpot)
        {
            int index = -1;
            GameObject spawnObj = null;

            if (pass.spawnOptions == SpawnOptions.Default || pass.spawnOptions == SpawnOptions.SpawnedObjects)
            {
                index = Random.Range(0, spawnedObjects.Count);
                spawnObj = spawnedObjects[index];
            }

            else if (pass.spawnOptions == SpawnOptions.AllSpawnedObjects)
            {
                index = Random.Range(0, allSpawnedObjects.Count);
                spawnObj = allSpawnedObjects[index];
            }

            else if (pass.spawnOptions == SpawnOptions.UseTargetObject)
            {
                spawnObj = targetObj;
            }

            else if (pass.spawnOptions == SpawnOptions.UseTargetSpline)
            {
                SplineData splineData = targetSpline.GetComponent<SplineData>();
                index = Random.Range(0, splineData.objects.Count);
                spawnObj = splineData.objects[index];
            }

            if (spawnObj == null)
            {                
                yield return null;
                continue;
            }

            SpawnPointData data = spawnObj.GetComponent<SpawnPointData>();

            Vector3 spawnPos = data.spawnPoints.yPoints[Random.Range(0, data.spawnPoints.yPoints.Count)];

            Quaternion randRot = Quaternion.identity;
            Vector3 scale = Vector3.one;

            GameObject prefab = null;

            if (pass.spawnSource == SpawnSource.Default || pass.spawnSource == SpawnSource.SplinePrefabs)
            {
                if (pass.splineObjectPrefabs != null && pass.splineObjectPrefabs.Count > 0)
                    prefab = pass.splineObjectPrefabs[Random.Range(0, pass.splineObjectPrefabs.Count)];

                else break;                    
            }

            else if (pass.spawnSource == SpawnSource.ObjectList)
            {
                if (pass.spawnObjects != null && pass.spawnObjects.objects.Count > 0)
                    prefab = pass.spawnObjects.objects[Random.Range(0, pass.spawnObjects.objects.Count)].obj;

                else break;
            }
            
            else if (pass.spawnSource == SpawnSource.Stackables)
            {
                if (data.stackables != null && data.stackables.objects.Count > 0)
                    prefab = data.stackables.objects[Random.Range(0, data.stackables.objects.Count)].obj;

                else continue;
            }

            else if (pass.spawnSource == SpawnSource.Adjacents)
            {
                if (data.adjacents != null && data.adjacents.objects.Count > 0)
                    prefab = data.adjacents.objects[Random.Range(0, data.adjacents.objects.Count)].obj;

                else continue;
            }

            if (pass.canOffset)
            {
                spawnPos += pass.GetRandomVector(pass.minOffset, pass.maxOffset);
            }

            if (pass.canRotate)
            {
                randRot = pass.GetRandomRotation(pass.minRotation, pass.maxRotation);
            }

            if (pass.canScale)
            {
                scale = Vector3.one * Random.Range(pass.minScale, pass.maxScale);
            }

            spawnPos = spawnObj.transform.TransformPoint(spawnPos);
            Vector3 rayPos = spawnPos + (Vector3.up * (pass.spawnOnTopRayDist - 1.0f));

            if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, pass.spawnOnTopRayDist))
            {
                //GameObject newObj = Instantiate(pass.splineObjectPrefabs[spawnIndex], spawnPos, randRot);
                GameObject newObj = Instantiate(prefab, spawnPos, randRot);
                newObj.transform.localScale = scale;
                foundSpot = true;
                allSpawnedObjects.Add(newObj);
                //newSplinePassComplete = true;

                if (pass.runMode == RunMode.EndOfCycle)
                {
                    if (pass.subpasses != null && pass.subpasses.Count > 0)
                    {
                        if (pass.runSubPassesAsync)
                        {
                            StartCoroutine(RunSubPassAsync(pass.subpasses, targetSpline, newObj, newObj.transform.position));
                        }

                        else
                        {
                            for (int i = 0; i < pass.subpasses.Count; i++)
                            {
                                PassBool subComplete = new PassBool();
                                StartCoroutine(MainPass(pass.subpasses[i], targetSpline, newObj, newObj.transform.position, subComplete));

                                if (pass.subpasses[i].waitUntilComplete)
                                    yield return new WaitUntil(() => { return subComplete.val; });
                            }
                        }
                    }
                }

                break;
            }

            //spawnCount++;
            yield return null;
        }

        //newSplinePassComplete = true;
        complete.val = true;
    }

    IEnumerator NewSubSpline(SplinePass pass, GameObject targetSpline, GameObject targetObj, Vector3 startPos, PassBool complete, List<GameObject> objectList = null)
    {
        //while (spawnedObjects.Count < 1)
        //    yield return null;
        //Debug.Log("In New Sub Spline");

        //Do we want to use target spline or get all the objects via spawnedObjects?
        Vector3 spawnPos = new Vector3(0, 0, 0);
        Quaternion randRot = Quaternion.identity;

        GameObject prefab = (pass.splinePrefab != null) ? pass.splinePrefab : splinePrefab;
        GameObject newSpline = Instantiate(prefab, spawnPos, randRot);
        newSpline.name = "SubSpline";

        SplineComputer spline = newSpline.GetComponent<SplineComputer>();
        SplinePoint[] splinePoints = new SplinePoint[pass.splinePointCount];
        ObjectController controller = newSpline.GetComponent<ObjectController>();
        spline.space = SplineComputer.Space.Local;

        int index = 0;
        SplineData targetData = null;

        bool useSpawn = pass.useSpawnedObjects;

        if (!pass.useSpawnedObjects)
        {            
            targetData = targetSpline.GetComponent<SplineData>();

            if (targetData == null)
            {                
                complete.val = true;
                yield break;
            }

            else
            {              
                while (!targetData.initialized)
                    yield return null;
            }
        }

        index = (useSpawn) ? Random.Range(0, spawnedObjects.Count) : Random.Range(0, targetData.spawnDatas.Count);
        SpawnPointData prev = null;
        Vector3 prevPos = Vector3.zero;
        float origY = 0.0f;

        SpawnPointData data = null;

        for (int i = 0; i < splinePoints.Length; i++)
        {
            //SpawnPointData data = (useSpawn) ? spawnedObjects[index].GetComponent<SpawnPointData>() : targetData.spawnDatas[index];

            if (useSpawn) data = spawnedObjects[index].GetComponent<SpawnPointData>();
            else data = targetData.spawnDatas[index];

            Vector3 pos = data.transform.TransformPoint(data.spawnPoints.yPoints[Random.Range(0, data.spawnPoints.yPoints.Count)]);
            Vector3 rayPos = pos + Vector3.up * (pass.spawnOnTopRayDist - 1.0f);

            if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, pass.spawnOnTopRayDist))
            {
                if (hit.point.y > pos.y)
                    pos = hit.point;
            }

            if (i == 0)
            {
                newSpline.transform.position = pos;
                splinePoints[i] = new SplinePoint();
                splinePoints[i].position = pos;
                origY = pos.y;

                if (!pass.useRandomNormal)
                    splinePoints[i].normal = pass.normal;
                else
                    splinePoints[i].normal = pass.GetRandomVector(pass.minNormal, pass.maxNormal);

                splinePoints[i].size = pass.pointSize;
            }

            else
            {                
                splinePoints[i] = new SplinePoint();

                if (pass.useAltSubSpline)
                {
                    Vector3 dir = (pos - prevPos).normalized;
                    dir.y = 0.0f;
                    float dist = Random.Range(pass.minSplinePointDist, pass.maxSplinePointDist);
                    pos += dir * dist;
                }

                if (pass.useOrigY)
                    pos.y = origY;

                splinePoints[i].position = pos;

                if (!pass.useRandomNormal)
                    splinePoints[i].normal = pass.normal;
                else
                    splinePoints[i].normal = pass.GetRandomVector(pass.minNormal, pass.maxNormal);

                splinePoints[i].size = pass.pointSize;                
            }

            bool canGoNext = (useSpawn) ? index + 1 < spawnedObjects.Count : index + 1 < targetData.spawnDatas.Count;
            bool canGoPrevious = index - 1 > -1;

            if (!pass.useOrigIndex)
            {
                if (canGoNext)
                    index += 1;

                if (canGoPrevious && !canGoNext)
                    index -= 1;
            }

            else
            {
                if (canGoNext && canGoPrevious)
                    index += (Random.Range(0, 2) == 0) ? 1 : -1;
                
                else if (canGoNext && !canGoPrevious)
                    index += 1;
                
                else if (canGoPrevious && !canGoNext)
                    index -= 1;
            }

            prev = data;
            prevPos = pos;
        }

        spline.SetPoints(splinePoints);
        splineObjects.Add(spline);

        controller.objects = pass.splineObjectPrefabs.ToArray();
        controller.useCustomObjectDistance = pass.useObjectDist;
        controller.minObjectDistance = pass.minObjectDist;
        controller.maxObjectDistance = pass.maxObjectDist;

        if (pass.canRotate)
        {
            controller.minRotation = pass.minRotation;
            controller.maxRotation = pass.maxRotation;
        }

        //controller.customOffsetRule = pass.rule;

        if (pass.canScale)
        {
            controller.minScaleMultiplier = Vector3.one * pass.minScale;
            controller.maxScaleMultiplier = Vector3.one * pass.maxScale;
        }

        //controller.spawnCount = pass.objectSpawnCount;
        controller.spawnCount = Random.Range(pass.minObjectSpawnCount, pass.maxObjectSpawnCount);

        if (pass.useRandomSeedRange)
        {
            controller.randomSeed = Random.Range(pass.minSeedRange, pass.maxSeedRange + 1);
        }

        controller.Rebuild();

        SplineData splineData = newSpline.GetComponent<SplineData>();
        Transform[] children = null;

        controller.onPostBuild += () =>
        {
            if (!splineData.initialized)
            {
                children = newSpline.GetComponentsInChildren<Transform>();
                splineData.colliders = new List<Collider>(children.Length);
                splineData.spawnDatas = new List<SpawnPointData>(children.Length);
                splineData.objects = new List<GameObject>(children.Length);
                splineData.rayPoses = new List<Vector3>(children.Length);

                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].transform != newSpline.transform)
                        {
                            //Collider col = children[i].GetComponent<Collider>();
                            if (children[i].TryGetComponent<Collider>(out Collider col))
                            {
                                allSpawnedObjects.Add(children[i].gameObject);
                                splineData.colliders.Add(col);
                                splineData.spawnDatas.Add(children[i].GetComponent<SpawnPointData>());
                                splineData.rayPoses.Add(new Vector3(col.bounds.center.x, -col.bounds.extents.y, col.bounds.center.z));
                                splineData.objects.Add(children[i].gameObject);
                            }
                        }
                    }
                }

                splineData.pass = pass;
                splineData.initialized = true;
            }
        };

        while (!splineData.initialized)
            yield return null;

        if (pass.runMode == RunMode.EndOfCycle)
        {
            if (pass.subpasses != null && pass.subpasses.Count > 0)
            {
                if (pass.runSubPassesAsync)
                {
                    StartCoroutine(RunSubPassAsync(pass.subpasses, newSpline, targetObj, newSpline.transform.position));
                }

                else
                {
                    for (int i = 0; i < pass.subpasses.Count; i++)
                    {
                        PassBool subComplete = new PassBool();
                        StartCoroutine(MainPass(pass.subpasses[i], newSpline, targetObj, newSpline.transform.position, subComplete));

                        if (pass.subpasses[i].waitUntilComplete)
                            yield return new WaitUntil(() => { return subComplete.val; });
                    }
                }
            }
        }

        complete.val = true;
    }

    IEnumerator NewSplineSetup5(SplinePass pass, GameObject targetSpline, GameObject targetObj, Vector3 startPos, PassBool complete, List<GameObject> objectList = null)
    {
        //newSplinePassComplete = false;
        int passAttempt = 0;
        Restart:
        //=============== SPLINE CONSTRUCTION ================
        Vector3 spawnPos = new Vector3(0, 0, 0);
        Quaternion randRot = Quaternion.identity;

        GameObject prefab = (pass.splinePrefab != null) ? pass.splinePrefab : splinePrefab;
        GameObject newSpline = Instantiate(prefab, spawnPos, randRot);

        SplineComputer spline = newSpline.GetComponent<SplineComputer>();
        SplinePoint[] splinePoints = new SplinePoint[pass.splinePointCount];
        ObjectController controller = newSpline.GetComponent<ObjectController>();
        spline.space = SplineComputer.Space.Local;

        Vector3 currentPos = Vector3.zero;

        spawnHelper.transform.rotation = newSpline.transform.rotation;
        spawnHelper.transform.position = newSpline.transform.position;
        Vector3 dir = spawnHelper.transform.TransformDirection(Vector3.forward);

        for (int i = 0; i < splinePoints.Length; i++)
        {
            spawnHelper.transform.rotation = newSpline.transform.rotation;
            spawnHelper.transform.localScale = Vector3.one;

            if (pass.useStartingPos && i == 0)
            {
                splinePoints[i].position = newSpline.transform.position;
                currentPos = splinePoints[i].position;
                spawnHelper.transform.position = currentPos;
            }

            else
            {
                splinePoints[i] = new SplinePoint();

                float dist = Random.Range(pass.minSplinePointDist, pass.maxSplinePointDist);
                Vector3 pos = currentPos + dir * dist;
                spawnHelper.transform.position = pos;

                if (pass.canOffset)
                {
                    Vector3 offset = pass.GetRandomVector(pass.minOffset, pass.maxOffset);
                    pos = spawnHelper.transform.TransformPoint(offset);
                }

                //Right now ignoring ground placement
                splinePoints[i].position = pos;
                currentPos = pos;
                spawnHelper.transform.position = pos;
            }

            if (!pass.useRandomNormal)
                splinePoints[i].normal = pass.normal;
            else
                splinePoints[i].normal = pass.GetRandomVector(pass.minNormal, pass.maxNormal);

            splinePoints[i].size = pass.pointSize;
            yield return null;
        }

        spline.SetPoints(splinePoints);
        splineObjects.Add(spline);

        //================== GENERAL SET UP =======================

        controller.objects = pass.splineObjectPrefabs.ToArray();
        controller.useCustomObjectDistance = pass.useObjectDist;
        controller.minObjectDistance = pass.minObjectDist;
        controller.maxObjectDistance = pass.maxObjectDist;

        if (pass.canRotate)
        {
            controller.minRotation = pass.minRotation;
            controller.maxRotation = pass.maxRotation;
        }

        //controller.customOffsetRule = pass.rule;

        if (pass.canScale)
        {
            controller.minScaleMultiplier = Vector3.one * pass.minScale;
            controller.maxScaleMultiplier = Vector3.one * pass.maxScale;
        }

        //controller.spawnCount = pass.objectSpawnCount;
        controller.spawnCount = Random.Range(pass.minObjectSpawnCount, pass.maxObjectSpawnCount);

        if (pass.useRandomSeedRange)
        {
            controller.randomSeed = Random.Range(pass.minSeedRange, pass.maxSeedRange + 1);
        }

        controller.Rebuild();

        //====================== On Post Build =======================

        SplineData data = newSpline.GetComponent<SplineData>();        
        Transform[] children = null;
        List<Vector3> rayPoses = null;
        RaycastHit[] hits = null;
        //SplineComputer[] splines = null;

        controller.onPostBuild += () =>
        {
            if (!data.initialized)
            {                
                children = newSpline.GetComponentsInChildren<Transform>();
                hits = new RaycastHit[children.Length];
                data.rayPoses = new List<Vector3>(children.Length);
                data.colliders = new List<Collider>(children.Length);
                data.spawnDatas = new List<SpawnPointData>(children.Length);
                data.objects = new List<GameObject>(children.Length);
                //splines = new SplineComputer[children.Length];

                if (children == null)
                    return;

                for (int i = 1; i < children.Length; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].transform != newSpline.transform)
                        {
                            if (children[i] != null)
                            {
                                if (children[i].TryGetComponent<Collider>(out Collider col))
                                {
                                    //spawnedObjects.Add(children[i].gameObject);
                                    //allSpawnedObjects.Add(children[i].gameObject);
                                    data.rayPoses.Add(new Vector3(col.bounds.center.x, -col.bounds.extents.y, col.bounds.center.z));
                                    data.colliders.Add(col);
                                    data.spawnDatas.Add(children[i].GetComponent<SpawnPointData>());
                                    data.objects.Add(children[i].gameObject);
                                }
                            }
                        }
                    }
                }
                
                data.pass = pass;
                rayPoses = data.rayPoses;
                data.initialized = true;                
            }
        };

        //================= MAIN LOOP FOR FINDING SPACE =====================

        while (!data.initialized)
            yield return null;

        while (spawnedObjects.Count < 1)
            yield return null;

        bool foundSpace = false;

        int attemptCounter = 0;

        while (!foundSpace && attemptCounter < pass.maxRayAttempts)
        {
            bool foundSpot = true;
            int index = -1;
            GameObject spawnObj = null;
            int rotAttemptCounter = 0;
            int maxRotAttemptCount = 10;

            if (pass.spawnOptions == SpawnOptions.SpawnedObjects || pass.spawnOptions == SpawnOptions.Default)
            {
                index = Random.Range(0, spawnedObjects.Count);
                spawnObj = spawnedObjects[index];
            }

            else if (pass.spawnOptions == SpawnOptions.AllSpawnedObjects)
            {
                index = Random.Range(0, allSpawnedObjects.Count);
                spawnObj = allSpawnedObjects[index];
            }

            else if (pass.spawnOptions == SpawnOptions.UseTargetObject)
            {
                spawnObj = targetObj;
            }

            else if (pass.spawnOptions == SpawnOptions.UseTargetSpline)
            {
                SplineData splineData = targetSpline.GetComponent<SplineData>();
                index = Random.Range(0, splineData.objects.Count);
                spawnObj = splineData.objects[index];
            }

            //Find Spot on Spawned Object
            SpawnPointData data2 = spawnObj.GetComponent<SpawnPointData>();

            Vector3 newSplinePos = spawnObj.transform.TransformPoint(data2.spawnPoints.yPoints[Random.Range(0, data2.spawnPoints.yPoints.Count)]);
            newSplinePos.y += pass.spawnOnTopRayDist - 1.0f;

            newSpline.transform.position = newSplinePos;

            RetryPos:

            if (pass.canRotateSpline)
            {
                newSpline.transform.rotation = pass.GetRandomRotation(pass.minSplineRotation, pass.maxSplineRotation);
            }

            //RaycastHit firstHit = new RaycastHit();
            NativeArray<RaycastHit> rayHits = new NativeArray<RaycastHit>(rayPoses.Count, Allocator.TempJob);
            NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(rayPoses.Count, Allocator.TempJob);

            //TO-DO: ADD A Check to ensure y deviation isn't too great
            for (int i = 0; i < rayPoses.Count; i++)
            {
                Vector3 rayPos = children[i].TransformPoint(rayPoses[i]);
                commands[i] = new RaycastCommand(rayPos, Vector3.down, pass.spawnOnTopRayDist);                
                //if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, pass.spawnOnTopRayDist))
                //{
                //    hits[i] = hit;
                //}
                //
                //else
                //{
                //    //Debug.Log($"Missed for {i} pos {newSplinePos.ToString()} ray {rayPos.ToString()}");
                //    foundSpot = false;
                //    break;
                //}
            }

            JobHandle rayHandle = RaycastCommand.ScheduleBatch(commands, rayHits, 1);
            rayHandle.Complete();

            int hitCount = 0;

            for (int i = 0; i < rayHits.Length; i++)
            {
                if (rayHits[i].collider != null)
                {
                    hitCount++;
                }

                if (rayHits[i].collider == null)                
                {
                    foundSpot = false;
                    break;
                }
            }

            bool canMove = false;

            if (foundSpot)
            {
                //newSpline.transform.position = firstHit.point;
                canMove = true;

                //for (int i = 0; i < hits.Length; i++)
                //{
                //    for (int j = i + 1; j < hits.Length - i; j++)
                //    {
                //        if (Mathf.Abs(hits[i].point.y - hits[j].point.y) > pass.maxHeightDiff)
                //        {
                //            canMove = false;
                //            break;
                //        }
                //    }
                //}

                for (int i = 0; i < rayHits.Length; i++)
                {
                    for (int j = i + 1; j < rayHits.Length - i; j++)
                    {                        
                        if (Mathf.Abs(rayHits[i].point.y - rayHits[j].point.y) > pass.maxHeightDiff)
                        {
                            canMove = false;
                            break;
                        }
                    }
                }


                if (canMove)
                {
                    foundSpace = true;
                    //newSpline.transform.position = hits[0].point;
                    newSpline.transform.position = rayHits[0].point;
                    allSpawnedObjects.AddRange(data.objects);

                    rayHits.Dispose();
                    commands.Dispose();

                    break;
                }
            }

            rayHits.Dispose();
            commands.Dispose();
            attemptCounter++;
            passAttempt++;

            if (passAttempt >= pass.maxPassAttempts)
            {
                splineObjects.Remove(spline);
                Destroy(newSpline);
                Debug.Log("Too many tries, screw this");
                goto End;
            }
            
            //Found a spot that hits most so go back and only try rotating the spline in that spot
            if ((!foundSpot || !canMove) && hitCount >= children.Length / 2 && rotAttemptCounter < maxRotAttemptCount && attemptCounter < pass.maxRayAttempts)
            {
                rotAttemptCounter++;
                foundSpot = true; //Reset for beginning
                yield return null;
                goto RetryPos;
            }

            yield return null;
        }

        if (!foundSpace)
        {
            Debug.Log("New Spline, Out of Ray attempts, retrying");
            splineObjects.Remove(spline);
            Destroy(newSpline);
            attemptCounter = 0;
            goto Restart;
        }

        if (pass.runMode == RunMode.EndOfCycle)
        {
            if (pass.subpasses != null && pass.subpasses.Count > 0)
            {
                if (pass.runSubPassesAsync)
                {
                    StartCoroutine(RunSubPassAsync(pass.subpasses, newSpline, targetObj, newSpline.transform.position));
                }

                else
                {
                    for (int i = 0; i < pass.subpasses.Count; i++)
                    {
                        PassBool subComplete = new PassBool();
                        StartCoroutine(MainPass(pass.subpasses[i], newSpline, targetObj, newSpline.transform.position, subComplete));

                        if (pass.subpasses[i].waitUntilComplete)
                            yield return new WaitUntil(() => { return subComplete.val; });
                    }
                }
            }
        }

        End:
        //newSplinePassComplete = true;
        complete.val = true;
    }

    
    IEnumerator NewStackSpline(SplinePass pass, GameObject targetSpline, GameObject targetObj, Vector3 startPos, PassBool complete)
    {
        SplineData spline = targetSpline.GetComponent<SplineData>();
        int spawnCount = Random.Range(pass.minObjectSpawnCount, pass.maxObjectSpawnCount);
        int counter = 0;

        if (spline.objects == null || spline.objects.Count < 1)
        {
            complete.val = true;
            yield break;
        }

        while (counter < spawnCount)
        {
            for (int i = 0; i < spline.objects.Count; i++)
            {
                //int rand = Random.Range(0, 101);

                //if (rand <= pass.stackSpawnChance)
                {
                    SpawnPointData data = spline.spawnDatas[i];
                    Transform obj = data.transform;
                    Collider col = spline.colliders[i];
                    Vector3 pos = data.transform.TransformPoint(data.spawnPoints.yPoints[Random.Range(0, data.spawnPoints.yPoints.Count)]);
                    Vector3 rayPos = pos + Vector3.up * (pass.stackRayDist - 1.0f);
                    GameObject newObj = null;

                    if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, pass.stackRayDist))
                    {
                        if (hit.collider.gameObject == data.gameObject)
                        {
                            Quaternion randRot = Quaternion.identity;
                            Vector3 scale = Vector3.one;

                            if (pass.canRotate)
                                randRot = pass.GetRandomRotation(pass.minRotation, pass.maxRotation);

                            if (pass.canScale)
                                scale *= Random.Range(pass.minScale, pass.maxScale);

                            GameObject prefab = pass.splineObjectPrefabs[Random.Range(0, pass.splineObjectPrefabs.Count)];
                            SpawnPointData objData = prefab.GetComponent<SpawnPointData>();

                            spawnHelper.transform.position = pos;
                            spawnHelper.transform.rotation = randRot;
                            spawnHelper.transform.localScale = scale;
                            helperBox.center = objData.center;
                            helperBox.size = objData.size;
                            Bounds helpBounds = helperBox.bounds;

                            float y = -helpBounds.extents.y;

                            Vector3 topRightPos = new Vector3(helpBounds.extents.x, y, helpBounds.extents.z);
                            Vector3 topLeftPos = new Vector3(-helpBounds.extents.x, y, helpBounds.extents.z);
                            Vector3 downRightPos = new Vector3(helpBounds.extents.x, y, -helpBounds.extents.z);
                            Vector3 downLeftPos = new Vector3(-helpBounds.extents.x, y, -helpBounds.extents.z);

                            topRightPos = spawnHelper.transform.TransformPoint(topRightPos);
                            //topRightPos += pass.stackRayOffset;

                            topLeftPos = spawnHelper.transform.TransformPoint(topLeftPos);
                            //topLeftPos += pass.stackRayOffset;

                            downRightPos = spawnHelper.transform.TransformPoint(downRightPos);
                            //downRightPos += pass.stackRayOffset;

                            downLeftPos = spawnHelper.transform.TransformPoint(downLeftPos);
                            //downLeftPos += pass.stackRayOffset;

                            bool topRight = col.Raycast(new Ray(topRightPos, Vector3.down), out RaycastHit topRightHit, pass.stackRayDist);
                            bool topLeft = col.Raycast(new Ray(topLeftPos, Vector3.down), out RaycastHit topLeftHit, pass.stackRayDist);
                            bool downRight = col.Raycast(new Ray(downRightPos, Vector3.down), out RaycastHit downRightHit, pass.stackRayDist);
                            bool downLeft = col.Raycast(new Ray(downLeftPos, Vector3.down), out RaycastHit downLeftHit, pass.stackRayDist);

                            //bool topRight = Physics.Raycast(new Ray(topRightPos, Vector3.down), out RaycastHit topRightHit, pass.stackRayDist);
                            //bool topLeft = Physics.Raycast(new Ray(topLeftPos, Vector3.down), out RaycastHit topLeftHit, pass.stackRayDist);
                            //bool downRight = Physics.Raycast(new Ray(downRightPos, Vector3.down), out RaycastHit downRightHit, pass.stackRayDist);
                            //bool downLeft = Physics.Raycast(new Ray(downLeftPos, Vector3.down), out RaycastHit downLeftHit, pass.stackRayDist);

                            //Debug.DrawLine(topRightPos, topRightPos + Vector3.down * pass.spawnOnTopRayDist, Color.red, 10.0f);
                            //Debug.DrawLine(topLeftPos, topLeftPos + Vector3.down * pass.spawnOnTopRayDist, Color.red, 10.0f);
                            //Debug.DrawLine(downRightPos, downRightPos + Vector3.down * pass.spawnOnTopRayDist, Color.red, 10.0f);
                            //Debug.DrawLine(downLeftPos, downLeftPos + Vector3.down * pass.spawnOnTopRayDist, Color.red, 10.0f);

                            //if (!topRight || !topLeft || !downRight || !downLeft)
                            if (topRightHit.transform != obj || topLeftHit.transform != obj || downRightHit.transform != obj || downLeftHit.transform != obj)
                            {
                                //Debug.Log("Corner Check Failed");
                                continue;
                            }

                            newObj = Instantiate(prefab, pos, randRot);
                            newObj.transform.localScale = scale;

                            allSpawnedObjects.Add(newObj);
                            counter++;

                            if (pass.runMode == RunMode.EndOfCycle)
                            {
                                if (pass.subpasses != null && pass.subpasses.Count > 0)
                                {
                                    if (pass.runSubPassesAsync)
                                    {
                                        StartCoroutine(RunSubPassAsync(pass.subpasses, targetSpline, newObj, newObj.transform.position));
                                    }

                                    else
                                    {
                                        for (int j = 0; j < pass.subpasses.Count; j++)
                                        {
                                            if (pass.subpasses[j].enabled)
                                            {
                                                PassBool subComplete = new PassBool();
                                                StartCoroutine(MainPass(pass.subpasses[j], targetSpline, newObj, newObj.transform.position, subComplete));

                                                if (pass.subpasses[i].waitUntilComplete)
                                                    yield return new WaitUntil(() => { return subComplete.val; });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                yield return null;
            }
           
            yield return null;
        }

        complete.val = true;
    }

    //IEnumerator StackSpline(SplinePass pass, GameObject obj)
    IEnumerator StackSpline(SplinePass pass, GameObject targetSpline, GameObject targetObj, Vector3 startPos, PassBool complete, List<GameObject> objectList = null)
    {
        //while (!canRunStack)
        //    yield return null;
        //SubSplinePass subpass = pass.subPass;
        int spawnCount = Random.Range(pass.minObjectSpawnCount, pass.maxObjectSpawnCount);
        int counter = 0;

        SplineData spline = targetSpline.GetComponent<SplineData>();
        
        //for (int i = 0; i < spawnCount; i++)
        while (counter < spawnCount)
        {
            int index = Random.Range(0, spline.spawnDatas.Count);
            SpawnPointData data = spline.spawnDatas[index];
            Transform obj = null;
            try { obj = spline.objects[index].transform; }
            catch { continue; }

            //if (data == null || data.stackables.objects.Count < 1)
            //{
            //    //canRunStack = false;
            //    yield return null;
            //    continue;
            //}
            //
            //if (data.stackables.objects.Count < 1 || data.spawnPoints.yPoints.Count < 1)
            //{
            //    //canRunStack = false;
            //    yield return null;
            //    continue;
            //}

            Vector3 pos = data.spawnPoints.yPoints[Random.Range(0, data.spawnPoints.yPoints.Count)];
            Quaternion rot = Quaternion.identity;
            Vector3 scale = Vector3.one;
                       
            pos = obj.TransformPoint(pos);

            if (pass.canRotate)
            {
                rot = pass.GetRandomRotation(pass.minRotation, pass.maxRotation);
            }

            if (pass.canScale)
                scale *= Random.Range(pass.minScale, pass.maxScale);

            //GameObject prefab = subpass.spawnPrefabs[Random.Range(0, subpass.spawnPrefabs.Count)];
            //GameObject prefab = data.stackables.objects[Random.Range(0, data.stackables.objects.Count)].obj;
            GameObject prefab = null;

            if (pass.spawnSource == SpawnSource.Default || pass.spawnSource == SpawnSource.Stackables)
            {                
                prefab = data.stackables.objects[Random.Range(0, data.stackables.objects.Count)].obj;
            }

            if (pass.spawnSource == SpawnSource.ObjectList)
            {
                prefab = pass.spawnObjects.objects[Random.Range(0, pass.spawnObjects.objects.Count)].obj;
            }

            if (pass.spawnSource == SpawnSource.SplinePrefabs)
            {
                prefab = pass.splineObjectPrefabs[Random.Range(0, pass.splineObjectPrefabs.Count)];
            }

            if (pass.spawnSource == SpawnSource.Adjacents)
            {
                prefab = data.adjacents.objects[Random.Range(0, data.adjacents.objects.Count)].obj;
            }

            GameObject newObj = Instantiate(prefab, pos, rot);
            pendingObjects.Add(newObj);
            newObj.transform.localScale = scale;
            //newObj.transform.parent = obj.transform;

            Bounds bounds = newObj.GetComponentInChildren<Collider>().bounds;

            Vector3 topRightPosLocal = new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 topLeftPosLocal = new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 downRightPosLocal = new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
            Vector3 downLeftPosLocal = new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);

            bool foundSpot = false;

            int attemptCounter = 0;

            while (!foundSpot && attemptCounter < pass.maxRayAttempts)
            {
                Vector3 topRightPos = newObj.transform.TransformPoint(topRightPosLocal);
                Vector3 topLeftPos = newObj.transform.TransformPoint(topLeftPosLocal);
                Vector3 downRightPos = newObj.transform.TransformPoint(downRightPosLocal);
                Vector3 downLeftPos = newObj.transform.TransformPoint(downLeftPosLocal);

                bool topRight = Physics.Raycast(topRightPos + pass.stackRayOffset, Vector3.down, out RaycastHit topRightHit, pass.stackRayDist);
                bool topLeft = Physics.Raycast(topLeftPos + pass.stackRayOffset, Vector3.down, out RaycastHit topLeftHit, pass.stackRayDist);
                bool downRight = Physics.Raycast(downRightPos + pass.stackRayOffset, Vector3.down, out RaycastHit downRightHit, pass.stackRayDist);
                bool downLeft = Physics.Raycast(downLeftPos + pass.stackRayOffset, Vector3.down, out RaycastHit downLeftHit, pass.stackRayDist);

                if (topRight && topLeft && downRight && downLeft)
                {
                    if (topRightHit.transform == obj && topLeftHit.transform == obj && downRightHit.transform == obj && downLeftHit.transform == obj)
                    {
                        foundSpot = true;
                        break;
                    }
                }

                //In case of struggling to find a spot, redo the random rotation and scales
                else
                {
                    Vector3 newPos = data.spawnPoints.yPoints[Random.Range(0, data.spawnPoints.yPoints.Count)];
                    newObj.transform.position = obj.TransformPoint(newPos);

                    if (pass.canRotate)
                    {
                        Quaternion newRot = pass.GetRandomRotation(pass.minRotation, pass.maxRotation);
                        newObj.transform.rotation = newRot;
                    }

                    if (pass.canScale)
                    {
                        newObj.transform.localScale = Vector3.one * Random.Range(pass.minScale, pass.maxScale);
                    }
                }

                attemptCounter++;
                yield return null;
            }

            if (!foundSpot)
            {
                Debug.Log($"Stack out of Ray Attempts on {pass.name}");
                pendingObjects.Remove(newObj);
                Destroy(newObj);
                continue;
            }

            pendingObjects.Remove(newObj);
            allSpawnedObjects.Add(newObj);
            //data = newObj.GetComponent<SpawnPointData>();
            //currentObj = newObj;
            counter++;

            if (pass.runMode == RunMode.EndOfCycle)
            {
                if (pass.subpasses != null && pass.subpasses.Count > 0)
                {
                    if (pass.runSubPassesAsync)
                    {
                        StartCoroutine(RunSubPassAsync(pass.subpasses, targetSpline, newObj, newObj.transform.position));
                    }

                    else
                    {
                        for (int i = 0; i < pass.subpasses.Count; i++)
                        {
                            if (pass.subpasses[i].enabled)
                            {
                                PassBool subComplete = new PassBool();
                                StartCoroutine(MainPass(pass.subpasses[i], targetSpline, newObj, newObj.transform.position, subComplete));

                                if (pass.subpasses[i].waitUntilComplete)
                                    yield return new WaitUntil(() => { return subComplete.val; });
                            }
                        }
                    }
                }
            }

            yield return null;
        }



        //canRunStack = false;
        //yield return null;
        complete.val = true;
    }

    //IEnumerator SpawnWallAttachments(SplinePass pass, GameObject obj)
    IEnumerator SpawnWallAttachments(SplinePass pass, GameObject targetSpline, GameObject targetObj, Vector3 startPos, PassBool complete, List<GameObject> objectList = null)
    {
        //while (!canRunAdjacents)
        //    yield return null;
        
        int spawnCount = Random.Range(pass.minObjectSpawnCount, pass.maxObjectSpawnCount);
        int counter = 0;
        SplineData spline = targetSpline.GetComponent<SplineData>();

        //for (int i = 0; i < spawnCount; i++)
        while (counter < spawnCount)
        {            
            Transform obj = spline.spawnDatas[Random.Range(0, spline.spawnDatas.Count)].transform;
            SpawnPointData data = obj.GetComponent<SpawnPointData>();

            bool emptyX = data.spawnPoints.xPoints.Count < 1;
            bool emptyZ = data.spawnPoints.zPoints.Count < 1;

            if (data == null || data.adjacents.objects.Count < 1)
            {
                //canRunAdjacents = false;
                yield return null;
                continue;
            }

            if (data.adjacents.objects.Count < 1 || (emptyX && emptyZ))
            {
                //canRunAdjacents = false;
                yield return null;
                continue;
            }

            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            Vector3 scale = Vector3.one;

            if (!emptyX && !emptyZ)
            {
                int pointSelect = Random.Range(0, 2);
                if (pointSelect == 0)
                    pos = data.spawnPoints.xPoints[Random.Range(0, data.spawnPoints.xPoints.Count)];
                else
                    pos = data.spawnPoints.zPoints[Random.Range(0, data.spawnPoints.zPoints.Count)];
            }

            else if (emptyX && !emptyZ)
                pos = pos = data.spawnPoints.zPoints[Random.Range(0, data.spawnPoints.zPoints.Count)];

            else if (!emptyX && emptyZ)
                pos = data.spawnPoints.xPoints[Random.Range(0, data.spawnPoints.xPoints.Count)];

            pos = obj.TransformPoint(pos);

            if (pass.canRotate)
                rot = pass.GetRandomRotation(pass.minRotation, pass.maxRotation);

            if (pass.canScale)
                scale *= Random.Range(pass.minScale, pass.maxScale);

            //GameObject prefab = data.adjacents.objects[Random.Range(0, data.adjacents.objects.Count)].obj;
            GameObject prefab = pass.splineObjectPrefabs[Random.Range(0, pass.splineObjectPrefabs.Count)];

            GameObject newObj = Instantiate(prefab, pos, rot);
            newObj.transform.localScale = scale;
            allSpawnedObjects.Add(newObj);

            if (pass.runMode == RunMode.EndOfCycle)
            {
                if (pass.subpasses != null && pass.subpasses.Count > 0)
                {
                    if (pass.runSubPassesAsync)
                    {
                        StartCoroutine(RunSubPassAsync(pass.subpasses, targetSpline, newObj, newObj.transform.position));
                    }

                    else
                    {
                        for (int i = 0; i < pass.subpasses.Count; i++)
                        {
                            if (pass.subpasses[i].enabled)
                            {
                                PassBool subComplete = new PassBool();
                                StartCoroutine(MainPass(pass.subpasses[i], targetSpline, newObj, newObj.transform.position, subComplete));

                                if (pass.subpasses[i].waitUntilComplete)
                                    yield return new WaitUntil(() => { return subComplete.val; });
                            }
                        }
                    }
                }
            }

            counter++;

            yield return null;
        }

        complete.val = true;
        //canRunAdjacents = false;                
    }

    IEnumerator SetupSpline(SplinePass pass, GameObject newSpline, GameObject targetObj, Vector3 startPos, PassBool complete, bool setFirstPointToSplineSpawn = false)
    {
        //newSplinePassComplete = false;        
        SplineComputer spline = newSpline.GetComponent<SplineComputer>();
        SplinePoint[] splinePoints = new SplinePoint[pass.splinePointCount];
        ObjectController controller = newSpline.GetComponent<ObjectController>();

        spline.space = SplineComputer.Space.Local;
        //spline.space = SplineComputer.Space.World;

        if (pass.offsetSplineObject)        
            newSpline.transform.position = newSpline.transform.TransformPoint(pass.splineOffset);
        
        for (int i = 0; i < splinePoints.Length; i++)
        {
            splinePoints[i] = new SplinePoint();
            float dist = Random.Range(pass.minSplinePointDist, pass.maxSplinePointDist);
            Vector3 angles = pass.GetRandomVector(pass.minAngles, pass.maxAngles);
            Vector3 dir = new Vector3(Mathf.Cos(angles.x), Mathf.Sin(angles.y), Mathf.Sin(angles.z)); //Mathf.Sin(angle)

            if (pass.useStartingPos && i == 0)
                splinePoints[i].position = newSpline.transform.position;

            else
                splinePoints[i].position = newSpline.transform.position + newSpline.transform.TransformDirection(dir) * (i + dist);

            if (!pass.useRandomNormal)
                splinePoints[i].normal = pass.normal;
            else
                splinePoints[i].normal = pass.GetRandomVector(pass.minNormal, pass.maxNormal);

            splinePoints[i].size = pass.pointSize;

            yield return null;
        }

        spline.SetPoints(splinePoints);
        splineObjects.Add(spline);

        controller.objects = pass.splineObjectPrefabs.ToArray();
        controller.useCustomObjectDistance = pass.useObjectDist;
        controller.minObjectDistance = pass.minObjectDist;
        controller.maxObjectDistance = pass.maxObjectDist;

        if (pass.canRotate)
        {
            controller.minRotation = pass.minRotation;
            controller.maxRotation = pass.maxRotation;
        }

        if (pass.canScale)
        {
            controller.minScaleMultiplier = Vector3.one * pass.minScale;
            controller.maxScaleMultiplier = Vector3.one * pass.maxScale;
        }
        
        //controller.spawnCount = pass.objectSpawnCount;
        controller.spawnCount = Random.Range(pass.minObjectSpawnCount, pass.maxObjectSpawnCount);

        if (pass.useRandomSeedRange)
        {
            controller.randomSeed = Random.Range(pass.minSeedRange, pass.maxSeedRange + 1);
        }

        controller.Rebuild();
        
        SplineData data = newSpline.GetComponent<SplineData>();
        Transform[] children = null;        
        List<SpawnPointData> spawnDatas = null;
        
        controller.onPostBuild += () =>
        {
            //Debug.Log("In Main Land Post Build");

            if (!data.initialized)
            {
                //controller.enabled = false;
                //controller.spline = null;

                children = newSpline.GetComponentsInChildren<Transform>();                
                data.rayPoses = new List<Vector3>(children.Length);
                data.colliders = new List<Collider>(children.Length);
                data.spawnDatas = new List<SpawnPointData>(children.Length);
                data.objects = new List<GameObject>(children.Length);

                spawnDatas = new List<SpawnPointData>(children.Length - 1);                

                if (children == null)
                    return;

                for (int i = 0; i < children.Length; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].transform != newSpline.transform)
                        {
                            if (children[i] != null)
                            {
                                //Collider col = children[i].GetComponent<Collider>();
                                //if (children[i].TryGetComponent<BoxCollider>(out BoxCollider col))
                                if (children[i].TryGetComponent<Collider>(out Collider col))
                                {                                    
                                    SpawnPointData spawn = children[i].GetComponent<SpawnPointData>();
                                    spawnDatas.Add(spawn);
                                    spawn.boxCollider = col;

                                    spawn.rayPos = new Vector3(col.bounds.center.x, -col.bounds.extents.y, col.bounds.center.z);                                    
                                    //Bounds bounds = new Bounds(spawn.center, spawn.size);
                                    //Debug.Log(children[i].position);
                                    //Vector3 center = new Vector3(bounds.center.x, bounds.center.y + bounds.extents.y, bounds.center.z);
                                    //Vector3 centerRight = new Vector3(bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z);
                                    //Vector3 centerLeft = new Vector3(-bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z);
                                    //
                                    //Vector3 topCenter = new Vector3(bounds.center.x, bounds.center.y + bounds.extents.y, bounds.extents.z);
                                    //Vector3 topRight = new Vector3(bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.extents.z);
                                    //Vector3 topLeft = new Vector3(-bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.extents.z);
                                    //
                                    //Vector3 downCenter = new Vector3(bounds.center.x, bounds.center.y + bounds.extents.y, -bounds.extents.z);
                                    //Vector3 downRight = new Vector3(bounds.extents.x, bounds.center.y + bounds.extents.y, -bounds.extents.z);
                                    //Vector3 downLeft = new Vector3(-bounds.extents.x, bounds.center.y + bounds.extents.y, -bounds.extents.z);

                                    //Vector3 center = spawn.rayPoints[0].position;
                                    //Vector3 centerRight = spawn.rayPoints[1].position;
                                    //Vector3 centerLeft = spawn.rayPoints[2].position;
                                    //
                                    //Vector3 topCenter = spawn.rayPoints[3].position;
                                    //Vector3 topRight = spawn.rayPoints[4].position;
                                    //Vector3 topLeft = spawn.rayPoints[5].position;
                                    //
                                    //Vector3 downCenter = spawn.rayPoints[6].position;
                                    //Vector3 downRight = spawn.rayPoints[7].position;
                                    //Vector3 downLeft = spawn.rayPoints[8].position;

                                }
                            }
                        }
                    }
                }
                
                data.pass = pass;
                data.initialized = true;                
            }
        };

        while (!data.initialized)
            yield return null;

        yield return new WaitForSeconds(pass.pruneWaitTime);

        //ORIGINAL WORKING VERSION

        controller.enabled = false;
        controller.spline = null;

        NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(9, Allocator.TempJob);
        NativeArray<RaycastHit> rayHits = new NativeArray<RaycastHit>(9, Allocator.TempJob);

        for (int ia = 0; ia < spawnDatas.Count; ia++)
        {
            //NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(9, Allocator.TempJob);            
            //NativeArray<RaycastHit> rayHits = new NativeArray<RaycastHit>(9, Allocator.TempJob);
        
            for (int ja = 0; ja < 9; ja++)
            {                
                Vector3 pos = spawnDatas[ia].rayPoints[ja].position;
                pos.y += (pass.spawnOnTopRayDist - 1.0f);
                
                commands[ja] = new RaycastCommand(pos, Vector3.down, pass.spawnOnTopRayDist);
                //commands[ja] = new SpherecastCommand(pos, 0.5f, Vector3.down, pass.spawnOnTopRayDist);
                //Debug.DrawLine(pos, pos + Vector3.down * pass.spawnOnTopRayDist, Color.red, 30.0f);
            }
        
            JobHandle rayHandle = RaycastCommand.ScheduleBatch(commands, rayHits, 9);
            //JobHandle rayHandle = SpherecastCommand.ScheduleBatch(commands, rayHits, 1);
            rayHandle.Complete();
        
            bool targetHit = false;
        
            for (int jb = 0; jb < 9; jb++)
            {                
                if (rayHits[jb].collider != null && rayHits[jb].collider.gameObject == spawnDatas[ia].gameObject)
                {
                    targetHit = true;
                    //GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //marker.transform.position = rayHits[jb].point;
                    //marker.GetComponent<Renderer>().material.color = Color.blue;
                    //Destroy(marker.GetComponent<SphereCollider>());                    
                    //marker.name = $"Hit {spawnDatas[ia].gameObject.name}";
                    //marker.transform.parent = spawnDatas[ia].gameObject.transform;

                    break;
                }
                
                //if (rayHits[jb].collider != null && rayHits[jb].collider.gameObject != spawnDatas[ia].gameObject)
                //{
                //    GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    marker.transform.position = rayHits[jb].point;
                //    marker.GetComponent<Renderer>().material.color = Color.yellow;
                //    Destroy(marker.GetComponent<SphereCollider>());
                //    marker.name = $"Miss, Hit {rayHits[jb].collider.gameObject.name}";                    
                //    marker.transform.parent = spawnDatas[ia].gameObject.transform;
                //}
        
                //if (rayHits[jb].collider == null)
                //{
                //    GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    marker.transform.position = rayHits[jb].point;
                //    marker.GetComponent<Renderer>().material.color = Color.red;
                //    Destroy(marker.GetComponent<SphereCollider>());                    
                //    marker.name = $"Missed {spawnDatas[ia].gameObject.transform}";
                //    marker.transform.parent = spawnDatas[ia].gameObject.transform;
                //}
            }
        
            if (targetHit)
            {
                spawnedObjects.Add(spawnDatas[ia].gameObject);
                allSpawnedObjects.Add(spawnDatas[ia].gameObject);
                data.rayPoses.Add(spawnDatas[ia].rayPos);
                data.colliders.Add(spawnDatas[ia].boxCollider);
                data.objects.Add(spawnDatas[ia].gameObject);
                data.spawnDatas.Add(spawnDatas[ia]);
            }
        
            if (!targetHit)
            {
                Destroy(spawnDatas[ia].gameObject);
                //spawnDatas[ia].gameObject.GetComponent<Renderer>().material.color = Color.red;
                //spawnedObjects.Add(spawnDatas[ia].gameObject);
                //allSpawnedObjects.Add(spawnDatas[ia].gameObject);
                //data.rayPoses.Add(spawnDatas[ia].rayPos);
                //data.colliders.Add(spawnDatas[ia].boxCollider);
                //data.objects.Add(spawnDatas[ia].gameObject);
                //data.spawnDatas.Add(spawnDatas[ia]);

            }
        
            //commands.Dispose();
            //rayHits.Dispose();        
            //yield return null;
        }

        commands.Dispose();
        rayHits.Dispose();

        data.spawnDatas.TrimExcess();
        data.rayPoses.TrimExcess();
        data.colliders.TrimExcess();
        data.objects.TrimExcess();
        spawnDatas.Clear();
        
        //Neighbor Search Potentially still useful
        //for (int i = 0; i < data.objects.Count; i++)
        //{
        //    Vector3 center = data.spawnDatas[i].center;
        //    center = data.transform.TransformPoint(center);
        //
        //    Vector3 extents = data.spawnDatas[i].size;
        //    extents *= pass.neighborScale;
        //    extents = data.transform.TransformPoint(extents);
        //
        //    Quaternion rot = data.objects[i].transform.rotation;
        //
        //    Collider[] colliders = Physics.OverlapBox(center, extents, rot);
        //
        //    foreach(Collider col in colliders)
        //    {
        //        if (col.gameObject != data.objects[i])
        //        {
        //            if (col.gameObject.TryGetComponent<SpawnPointData>(out SpawnPointData other))
        //            {
        //                //Debug.Log("Adding a neighbor", data.objects[i]);
        //                if (!data.spawnDatas[i].neighbors.Contains(other))
        //                    data.spawnDatas[i].neighbors.Add(other);
        //            }
        //        }
        //    }
        //
        //    yield return null;
        //}

        if (pass.runMode == RunMode.EndOfCycle)
        {
            if (pass.subpasses != null && pass.subpasses.Count > 0)
            {
                if (pass.runSubPassesAsync)
                {
                    StartCoroutine(RunSubPassAsync(pass.subpasses, newSpline, targetObj, newSpline.transform.position));
                }

                else
                {
                    for (int i = 0; i < pass.subpasses.Count; i++)
                    {
                        if (pass.subpasses[i].enabled)
                        {
                            PassBool subComplete = new PassBool();
                            StartCoroutine(MainPass(pass.subpasses[i], newSpline, targetObj, newSpline.transform.position, subComplete));

                            if (pass.subpasses[i].waitUntilComplete)
                                yield return new WaitUntil(() => { return subComplete.val; });
                        }
                    }
                }
            }
        }

        //newSplinePassComplete = true;
        complete.val = true;
    }

#if REFERENCE

    void OldAttempts()
    {
        for (int i = 0; i < spawnDatas.Count; i++)
        {
            if ((i * maxRays) + maxRays < rayHits.Length)
            {
                //Debug.Log("In Main Check");
                bool targetHit = false;
                int helpIndex = 0;
        
                for (int j = i * maxRays; j < (i * maxRays) + maxRays; j++)
                {
                    if (rayHits[j].collider != null)
                        Debug.Log($"{spawnDatas[i].gameObject.name} orig {spawnDatas[i].rays[helpIndex]} hit {rayHits[j].point}");
        
                    if (rayHits[j].collider != null && rayHits[j].collider.gameObject == spawnDatas[i].gameObject)
                    {
                        targetHit = true;
                        break;
                    }
        
                    helpIndex++;
                }
        
                if (targetHit)
                {
                    spawnedObjects.Add(spawnDatas[i].gameObject);
                    allSpawnedObjects.Add(spawnDatas[i].gameObject);
                    data.rayPoses.Add(spawnDatas[i].rayPos);
                    data.colliders.Add(spawnDatas[i].boxCollider);
                    data.objects.Add(spawnDatas[i].gameObject);
                }
        
                else
                {
                    //Destroy(spawnDatas[i].gameObject);                 
                    spawnDatas[i].gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < commands.Length; i++)
        {
            if ((i * maxRays) + maxRays < commands.Length)
            {
                SpawnPointData spawn = hitMap[commands[i].from];
        
                for (int j = i * maxRays; j < (i * maxRays) + maxRays; j++)
                {
                    SpawnPointData spawn2 = hitMap[commands[j].from];
        
                    if (spawn == spawn2)
                    {
                        Debug.Log($"Confirmed match for {spawn.gameObject.name + spawn.gameObject.GetInstanceID()}", spawn.gameObject);
                    }
                }
            }            
        }
        
    }

    void UnusedEndOfLoopReferences()
    {
        JobHandle rayHandle = RaycastCommand.ScheduleBatch(commands, rayHits, 1);
        rayHandle.Complete();
        
        bool targetHit = false;
        
        for (int j = 0; j < rayHits.Length; j++)
        {
            //if (rayHits[j].collider != null)
            //    Debug.Log($"Ray Hit {col.gameObject.name} {j} {rayHits[j].collider.gameObject.name}");
        
            if (rayHits[j].collider == col)
            {
                Debug.Log("Successful Hit");
                targetHit = true;
                break;
            }
        }
        
        if (!targetHit)
        {
            Debug.Log($"Destroying {col.gameObject.name}");
            col.gameObject.SetActive(false);
            //Destroy(col.gameObject);
        }
        
        else
        {
            spawnedObjects.Add(children[i].gameObject);
            allSpawnedObjects.Add(children[i].gameObject);
            data.rayPoses.Add(new Vector3(col.bounds.center.x, -col.bounds.extents.y, col.bounds.center.z));
            data.colliders.Add(col);
            data.spawnDatas.Add(spawn);                                    
            data.objects.Add(children[i].gameObject);
        }

        JobHandle rayHandle = RaycastCommand.ScheduleBatch(commands, rayHits, 1);
        rayHandle.Complete();
        
        for (int i = 0; i < rayHits.Length; i++)
        {
            if (rayHits[i].collider != null && rayHits[i].collider.gameObject != data.objects[i])
            {
                Debug.Log("Trying to set inactive");
                data.objects[i].SetActive(false);
            }
        }


        commands.Dispose();
        rayHits.Dispose();
    }

    void SwapPopBackReference()
    {
        //Used in Pruning, but would screw with the results by swapping entries
        SpawnPointData temp = spawnDatas[i];
        spawnDatas[i] = spawnDatas[spawnDatas.Count - 1];
        spawnDatas[spawnDatas.Count - 1] = temp;
        spawnDatas.RemoveAt(spawnDatas.Count - 1);
        Destroy(temp.gameObject);
        i--;
    }

    void OverlapReference()
    {
        Vector3 boxCenter = children[i].TransformPoint(col.center);
        
        Vector3 extents = children[i].TransformPoint(col.bounds.extents);
        
        Collider[] overlap = Physics.OverlapBox(boxCenter, extents);
        
        foreach (Collider collider in overlap)
        {
            if (collider != null && collider != col)
            {
                Bounds other = collider.bounds;                                            
        
                Vector3 topRight = new Vector3(other.extents.x, other.extents.y + other.center.y, other.extents.z);                                            
                Vector3 topLeft = new Vector3(-other.extents.x, other.extents.y + other.center.y, other.extents.z);                                            
                Vector3 downRight = new Vector3(other.extents.x, other.extents.y + other.center.y, -other.extents.z);                                            
                Vector3 downLeft = new Vector3(-other.extents.x, other.extents.y + other.center.y, -other.extents.z);
                Vector3 center = other.center;
                center.y += other.extents.y;
        
                topRight = collider.transform.TransformPoint(topRight);
                topLeft = collider.transform.TransformPoint(topLeft);
                downRight = collider.transform.TransformPoint(downRight);
                downLeft = collider.transform.TransformPoint(downLeft);
                center = collider.transform.TransformPoint(center);
        
                //min = collider.transform.TransformPoint(min);
                //max = collider.transform.TransformPoint(max);
        
                Bounds target = col.bounds;
                target.center = col.transform.TransformPoint(target.center);
                target.extents = col.transform.TransformPoint(target.extents);
        
                //if (target.Contains(min) && target.Contains(max))
                if (target.Contains(topRight) && target.Contains(topLeft) && target.Contains(downRight) && target.Contains(downLeft) && target.Contains(center))
                {
                    Debug.Log($"Should Delete {collider.gameObject.name}", collider.gameObject);
                }
            }
        }
    }

    void TotalBoxReference()
    {
        //Declaration Before Post Build
        GameObject totalBoundsObj = null;
                
        totalBoundsObj = new GameObject("Total Bounds");
        BoxCollider totalBox = totalBoundsObj.AddComponent<BoxCollider>();
        totalBox.isTrigger = true;
        data.totalBoundsObj = totalBoundsObj;
        data.totalBox = totalBox;

        Vector3 min = new Vector3(-col.bounds.extents.x, -col.bounds.extents.y, -col.bounds.extents.z);
        Vector3 max = new Vector3(col.bounds.extents.x, col.bounds.extents.y, col.bounds.extents.z);
        Vector3 min = new Vector3(-col.bounds.extents.x, col.bounds.extents.y, -col.bounds.extents.z);
        Vector3 max = new Vector3(col.bounds.extents.x, col.bounds.extents.y, col.bounds.extents.z);
        
        min = children[i].TransformPoint(min);
        max = children[i].TransformPoint(max);
        
        if (!totalBox.bounds.Contains(min) || !totalBox.bounds.Contains(max))
        {
            Bounds bounds = totalBox.bounds;
            bounds.Encapsulate(min);
            bounds.Encapsulate(max);
            totalBox.center = bounds.center;
            totalBox.size = bounds.size;
        }
        
        else
        {
            Debug.Log($"{col.gameObject.name} should be deleted", col.gameObject);                                        
        }
    }

    void JobBoxCastSetup()
    {
        NativeArray<RaycastHit> rayHits = new NativeArray<RaycastHit>((children.Length - 1) * 8, Allocator.TempJob);
        NativeArray<BoxcastCommand> commands = new NativeArray<BoxcastCommand>(children.Length - 1, Allocator.TempJob);
        int validIndex = 0;
        NativeArray<RaycastHit> rayHits = new NativeArray<RaycastHit>(children.Length - 1, Allocator.TempJob);
        NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(children.Length - 1, Allocator.TempJob);

        Vector3 pos = children[i].transform.position;
        pos.y += pass.neighborCastHeight - 1.0f;
        commands[validIndex] = new RaycastCommand(pos, Vector3.down, pass.neighborCastHeight);

        Vector3 extents = col.bounds.extents;
        Vector3 scale = children[i].transform.localScale;
        extents.x *= scale.x;
        extents.y *= scale.y;
        extents.z *= scale.z;
        extents = Vector3.Lerp(col.bounds.min, col.bounds.max, 0.5f);
        
        commands[validIndex] = new BoxcastCommand(pos, extents, children[i].rotation, Vector3.down, pass.maxNeighborCastDist);
        validIndex++;
    }

    void JobBoxCastExecution()
    {
        JobHandle rayHandle = BoxcastCommand.ScheduleBatch(commands, rayHits, 1);
        rayHandle.Complete();
        
        int maxHits = 8;
        
        for (int i = 0; i < data.spawnDatas.Count; i++)
        {
            if ((i * maxHits) + maxHits < rayHits.Length)
            {
                for (int j = i * maxHits; j < (i * maxHits) + maxHits; j++)
                {
                    //if (rayHits[j].collider != null && rayHits[j].collider != data.colliders[i])
                    if (rayHits[j].collider != null)
                    {
                        if (rayHits[j].collider.gameObject.TryGetComponent<SpawnPointData>(out SpawnPointData other))
                        {
                            Debug.Log("Adding a neighbor", data.spawnDatas[i]);
                            data.spawnDatas[i].neighbors.Add(other);
                        }
                    }
                }
            }
        }        
    }

#endif

    //IEnumerator StackSplineOld(SplinePass pass, SplineComputer spline)
    //{
    //    SubSplinePass subpass = pass.subPass;
    //
    //    if (!subpass.enabled) yield break;
    //
    //    ObjectController controller = spline.GetComponent<ObjectController>();
    //    int pointCount = Random.Range(subpass.minSplinePointCount, subpass.maxSplinePointCount);
    //    SplinePoint[] splinePoints = new SplinePoint[pointCount];
    //    spline.space = SplineComputer.Space.Local;
    //    Vector3 currentPos = Vector3.zero;
    //
    //    for (int i = 0; i < splinePoints.Length; i++)
    //    {
    //        float dist = 0.0f;
    //        Vector3 pos = Vector3.zero;
    //
    //        if (i == 0)
    //        {
    //            splinePoints[i].position = spline.transform.position;
    //            currentPos = spline.transform.position;
    //            splinePoints[i].normal = pass.normal;
    //            splinePoints[i].size = pass.pointSize;
    //            continue;
    //        }
    //
    //        else
    //        {
    //            splinePoints[i] = new SplinePoint();
    //            dist = Random.Range(subpass.minSplinePointDist, subpass.maxSplinePointDist);
    //            pos = currentPos + (Vector3.up * dist * i);
    //        }
    //
    //        splinePoints[i].position = pos;
    //        currentPos = pos;
    //        splinePoints[i].normal = pass.normal;
    //        splinePoints[i].size = pass.pointSize;
    //
    //        yield return null;
    //    }
    //
    //    spline.SetPoints(splinePoints);
    //    splineObjects.Add(spline);
    //
    //    controller.objects = subpass.spawnPrefabs.ToArray();
    //
    //    controller.useCustomObjectDistance = subpass.useCustomObjectDist;
    //    controller.minObjectDistance = pass.minObjectDist;
    //    controller.maxObjectDistance = pass.maxObjectDist;
    //
    //    if (subpass.canOffset)
    //    {
    //        controller.minOffset = subpass.minOffset;
    //        controller.maxOffset = subpass.maxOffset;
    //    }
    //
    //    if (subpass.canRotate)
    //    {
    //        controller.minRotation = subpass.minRotation;
    //        controller.maxRotation = subpass.maxRotation;
    //    }
    //
    //    if (subpass.canScale)
    //    {
    //        controller.minScaleMultiplier = Vector3.one * subpass.minScale;
    //        controller.maxScaleMultiplier = Vector3.one * subpass.maxScale;
    //    }
    //
    //    controller.spawnCount = Random.Range(subpass.minObjectSpawnCount, subpass.maxObjectSpawnCount + 1);
    //    controller.randomSeed = Random.Range(pass.minSeedRange, pass.maxSeedRange + 1);
    //
    //    controller.Rebuild();
    //    SplineData data = spline.GetComponent<SplineData>();
    //    Transform[] children = null;
    //
    //    controller.onPostBuild += () =>
    //    {
    //        if (!data.initialized)
    //        {
    //            children = spline.GetComponentsInChildren<Transform>();
    //
    //            for (int i = 0; i < children.Length; i++)
    //            {
    //                allSpawnedObjects.Add(children[i].gameObject);
    //            }
    //
    //            data.initialized = true;
    //        }
    //    };
    //
    //    yield return null;
    //}

    //IEnumerator NewSplineSetup4(SplinePass pass, PassBool complete)
    //{
    //    //newSplinePassComplete = false;
    //
    //    Vector3 spawnPos = new Vector3(0, 0, 0);
    //    Quaternion randRot = Quaternion.identity;
    //
    //    GameObject prefab = (pass.splinePrefab != null) ? pass.splinePrefab : splinePrefab;
    //    GameObject newSpline = Instantiate(prefab, spawnPos, randRot);
    //
    //    SplineComputer spline = newSpline.GetComponent<SplineComputer>();
    //    SplinePoint[] splinePoints = new SplinePoint[pass.splinePointCount];
    //    ObjectController controller = newSpline.GetComponent<ObjectController>();
    //    spline.space = SplineComputer.Space.Local;
    //
    //    Vector3 currentPos = Vector3.zero;
    //
    //    spawnHelper.transform.rotation = newSpline.transform.rotation;
    //    spawnHelper.transform.position = newSpline.transform.position;
    //    Vector3 dir = spawnHelper.transform.TransformDirection(Vector3.forward);
    //
    //    //Bounds totalSplineBounds = new Bounds();
    //
    //    //Vector3 angles = pass.GetRandomVector(pass.minAngles, pass.maxAngles);
    //    //Vector3 dir = new Vector3(Mathf.Cos(angles.x), Mathf.Sin(angles.y), Mathf.Sin(angles.z));
    //    //Vector3 dir = new Vector3(Mathf.Cos(angles.x), 0.0f, Mathf.Sin(angles.z));
    //    //spawnHelper.transform.rotation = Quaternion.LookRotation(dir);
    //    //spawnHelper.transform.rotation = Quaternion.Euler(new Vector3(0.0f, angles.y, 0.0f));
    //    //Vector3 dir = spawnHelper.transform.TransformDirection(Vector3.forward);
    //
    //    for (int i = 0; i < splinePoints.Length; i++)
    //    {
    //        //GameObject objRef = pass.splineObjectPrefabs[Random.Range(0, pass.splineObjectPrefabs.Count)];
    //        //SpawnPointData data = objRef.GetComponent<SpawnPointData>();
    //        spawnHelper.transform.rotation = newSpline.transform.rotation;
    //        spawnHelper.transform.localScale = Vector3.one;
    //
    //        if (pass.useStartingPos && i == 0)
    //        {
    //            splinePoints[i].position = newSpline.transform.position;
    //            currentPos = splinePoints[i].position;
    //            spawnHelper.transform.position = currentPos;
    //        }
    //
    //        else
    //        {
    //            splinePoints[i] = new SplinePoint();
    //
    //            //spawnHelper.transform.rotation = Quaternion.Euler(new Vector3(0.0f, angles.y, 0.0f));
    //            //Vector3 dir = spawnHelper.transform.TransformDirection(Vector3.forward);
    //
    //            float dist = Random.Range(pass.minSplinePointDist, pass.maxSplinePointDist);
    //            Vector3 pos = currentPos + dir * dist;
    //            spawnHelper.transform.position = pos;
    //
    //            if (pass.canOffset)
    //            {
    //                Vector3 offset = pass.GetRandomVector(pass.minOffset, pass.maxOffset);
    //                pos = spawnHelper.transform.TransformPoint(offset);
    //            }
    //
    //            //Right now ignoring ground placement
    //            splinePoints[i].position = pos;
    //            currentPos = pos;
    //            spawnHelper.transform.position = pos;
    //        }
    //
    //        if (!pass.useRandomNormal)
    //            splinePoints[i].normal = pass.normal;
    //        else
    //            splinePoints[i].normal = pass.GetRandomVector(pass.minNormal, pass.maxNormal);
    //
    //        splinePoints[i].size = pass.pointSize;
    //        yield return null;
    //    }
    //
    //    spline.SetPoints(splinePoints);
    //    splineObjects.Add(spline);
    //
    //    controller.objects = pass.splineObjectPrefabs.ToArray();
    //
    //    controller.useCustomObjectDistance = pass.useObjectDist;
    //    controller.minObjectDistance = pass.minObjectDist;
    //    controller.maxObjectDistance = pass.maxObjectDist;
    //
    //    if (pass.canRotate)
    //    {
    //        controller.minRotation = pass.minRotation;
    //        controller.maxRotation = pass.maxRotation;
    //    }
    //
    //    //controller.customOffsetRule = pass.rule;
    //
    //    if (pass.canScale)
    //    {
    //        controller.minScaleMultiplier = Vector3.one * pass.minScale;
    //        controller.maxScaleMultiplier = Vector3.one * pass.maxScale;
    //    }
    //
    //    controller.spawnCount = pass.objectSpawnCount;
    //
    //    if (pass.useRandomSeedRange)
    //    {
    //        controller.randomSeed = Random.Range(pass.minSeedRange, pass.maxSeedRange + 1);
    //    }
    //
    //    controller.Rebuild();
    //
    //    SplineData data = newSpline.GetComponent<SplineData>();
    //    Transform[] children = null;
    //    List<Vector3> rayPoses = null;
    //    RaycastHit[] hits = null;
    //
    //    controller.onPostBuild += () =>
    //    {
    //        if (!data.initialized)
    //        {
    //            splineObjects.Add(spline);
    //
    //            //Collider[] children = newSpline.GetComponentsInChildren<Collider>();
    //            children = newSpline.GetComponentsInChildren<Transform>();
    //            rayPoses = new List<Vector3>(children.Length);
    //            hits = new RaycastHit[children.Length];
    //            data.colliders = new List<Collider>(children.Length - 1);
    //
    //
    //            if (children == null)
    //                return;
    //
    //            for (int i = 0; i < children.Length; i++)
    //            {
    //                if (children[i] != null)
    //                {
    //                    if (children[i].transform != newSpline.transform)
    //                    {
    //                        if (children[i] != null)
    //                        {
    //                            if (children[i].TryGetComponent<Collider>(out Collider col))
    //                            {
    //                                //spawnedObjects.Add(children[i].gameObject);
    //                                allSpawnedObjects.Add(children[i].gameObject);
    //                                rayPoses[i] = new Vector3(col.bounds.center.x, -col.bounds.extents.y, col.bounds.center.z);
    //                                data.colliders[i] = col;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //
    //            data.rayPoses = rayPoses;
    //            data.pass = pass;
    //            data.initialized = true;
    //            //StartCoroutine(FindGroundForSpline(newSpline, pass, children, rayPoses));
    //        }
    //    };
    //
    //    while (!data.initialized)
    //        yield return null;
    //
    //    while (spawnedObjects.Count < 1)
    //        yield return null;
    //
    //    bool foundSpace = false;
    //
    //    while (!foundSpace)
    //    {
    //        if (pass.canRotateSpline)
    //        {
    //            newSpline.transform.rotation = pass.GetRandomRotation(pass.minSplineRotation, pass.maxSplineRotation);
    //        }
    //
    //        bool foundSpot = true;
    //
    //        int index = Random.Range(0, spawnedObjects.Count);
    //        GameObject spawnObj = spawnedObjects[index];
    //
    //        SpawnPointData data2 = spawnObj.GetComponent<SpawnPointData>();
    //
    //        Vector3 newSplinePos = spawnObj.transform.TransformPoint(data2.spawnPoints.yPoints[Random.Range(0, data2.spawnPoints.yPoints.Count)]);
    //        newSplinePos.y += pass.spawnOnTopRayDist - 1.0f;
    //
    //        newSpline.transform.position = newSplinePos;
    //
    //        //RaycastHit firstHit = new RaycastHit();
    //
    //        //TO-DO: ADD A Check to ensure y deviation isn't too great
    //        for (int i = 0; i < rayPoses.Count; i++)
    //        {
    //            Vector3 rayPos = children[i].TransformPoint(rayPoses[i]);
    //            //Debug.DrawLine(rayPos, rayPos + Vector3.down * pass.spawnOnTopRayDist, Color.red, 10.0f);
    //            //RaycastHit[] rayHits = Physics.RaycastAll(rayPos, Vector3.down, pass.spawnOnTopRayDist);
    //            //
    //            //if (rayHits != null)
    //            //{
    //            //    RaycastHit finalHit = new RaycastHit();                    
    //            //
    //            //    for (int j = 0; j < rayHits.Length; j++)
    //            //    {
    //            //        if (j == 0)                        
    //            //            finalHit = rayHits[j];                            
    //            //
    //            //        if (rayHits[j].point.y > finalHit.point.y)                        
    //            //            finalHit = rayHits[j];
    //            //
    //            //        //GameObject obj = new GameObject($"Hit {rayHits[j].point}");
    //            //        //obj.transform.position = rayHits[j].point;
    //            //    }
    //            //
    //            //    hits[i] = finalHit;
    //            //    //GameObject final = new GameObject($"Final Hit {finalHit.point}");
    //            //    //final.transform.position = finalHit.point;
    //            //}
    //
    //            if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, pass.spawnOnTopRayDist))
    //            {
    //                hits[i] = hit;
    //            }
    //
    //            else
    //            {
    //                //Debug.Log($"Missed for {i} pos {newSplinePos.ToString()} ray {rayPos.ToString()}");
    //                foundSpot = false;
    //                break;
    //            }
    //        }
    //
    //        if (foundSpot)
    //        {
    //            //newSpline.transform.position = firstHit.point;
    //            bool canMove = true;
    //
    //            for (int i = 0; i < hits.Length; i++)
    //            {
    //                for (int j = i + 1; j < hits.Length - i; j++)
    //                {
    //                    if (Mathf.Abs(hits[i].point.y - hits[j].point.y) > pass.maxHeightDiff)
    //                    {
    //                        canMove = false;
    //                        break;
    //                    }
    //                }
    //
    //                //GameObject obj = new GameObject($"Hit {hits[i].point}");
    //                //obj.transform.position = hits[i].point;
    //            }
    //
    //            if (canMove)
    //            {
    //                foundSpace = true;
    //                newSpline.transform.position = hits[0].point;
    //                break;
    //            }
    //        }
    //
    //        yield return null;
    //    }
    //
    //    //newSplinePassComplete = true;
    //    complete.val = true;
    //}

    [System.Serializable]
    public class SubSplinePass
    {
        public string name = "";
        public bool enabled = true;

        //public int minObjectSpawnRuns = 1;
        //public int maxObjectSelectRuns = 5;

        public int minSplinePointCount = 2;
        public int maxSplinePointCount = 5;

        public int minObjectSpawnCount = 1;
        public int maxObjectSpawnCount = 5;

        public float minSplinePointDist = 5.0f;
        public float maxSplinePointDist = 10.0f;

        public bool useCustomObjectDist = false;
        public float minObjectDist = 5.0f;
        public float maxObjectDist = 7.0f;

        public bool canOffset = false;
        public Vector3 minOffset = new Vector3(0, 0, 0);
        public Vector3 maxOffset = new Vector3(0, 0, 0);

        public bool canRotate = false;
        public Vector3 minRotation = new Vector3(0, 0, 0);
        public Vector3 maxRotation = new Vector3(0, 0, 0);

        public bool canScale = false;
        public float minScale = 1.0f;
        public float maxScale = 1.0f;

        public float stackRayDist = 3.0f;
        public Vector3 rayOffset = new Vector3(0, 1, 0);

        public List<GameObject> spawnPrefabs;
    }

    //Old

    /*Old Editor stuff
    public class EditorHelper
    {
        [UnityEditor.MenuItem("Window/Set Spline Circle")]
        public static void SetSplineCircle()
        {
            if (UnityEditor.Selection.activeGameObject != null)
            {
                SplineComputer spline = UnityEditor.Selection.activeGameObject.GetComponent<SplineComputer>();
                spline.space = SplineComputer.Space.World;
                
                SplinePoint[] points = new SplinePoint[8];
    
                for (int i = 0; i < 8; i++)
                {
                    points[i].position = new Vector3(20.0f * Mathf.Cos(45f * i), 0.0f, 20.0f * Mathf.Sin(45f * i));
                }
    
                spline.SetPoints(points);
            }
        }
    } 
     
    */


    //[Header("Main Land Settings")]
    //public int landCount = 50;
    //public Vector3 halfExtentBounds = new Vector3(500, 300, 500);
    //public float minScale = 1.0f;
    //public float maxScale = 3.0f;
    //public Vector3 minRotation = new Vector3(0, 0, 0);
    //public Vector3 maxRotation = new Vector3(0, 360.0f, 0);
    //public Vector3 minOffset = new Vector3(0, 0, 0);
    //public Vector3 maxOffset = new Vector3(20, 0, 20);
    //public List<GameObject> landPrefabs;
    //public GameObject surfacePrefab;
    //
    //[Header("Spline Settings")]
    //public int splineSpawnCount = 50;
    //public int splineSpawnCount2 = 50;
    //public int splinePointCount = 15;
    //public float minAngle = -90f;
    //public float maxAngle = 90f;
    //public float minDist = 3.0f;
    //public float maxDist = 10.0f;
    //public float pointSize = 1.0f;
    //public bool useClosestOnBounds = false;
    //public bool useClosestPoint = false;
    //public bool useRandomSeedRange = false;
    //public int minSeedRange = 0;
    //public int maxSeedRange = 99999999;
    //public GameObject splinePrefab;
    //public List<GameObject> splineObjectPrefabs;

    //IEnumerator CreateLands()
    //{
    //    landSpawns = new List<GameObject>();
    //
    //    int spawnCount = 0;
    //
    //    while (spawnCount < landCount)
    //    {
    //        float randPosX = Random.Range(-halfExtentBounds.x, halfExtentBounds.x);
    //        float randPosZ = Random.Range(-halfExtentBounds.z, halfExtentBounds.z);
    //        Vector3 randPos = new Vector3(randPosX, 0.0f, randPosZ);
    //
    //        float randRotX = Random.Range(minRotation.x, maxRotation.x);
    //        float randRotY = Random.Range(minRotation.y, maxRotation.y);
    //        float randRotZ = Random.Range(minRotation.z, maxRotation.z);
    //        Quaternion randRot = Quaternion.Euler(new Vector3(randRotX, randRotY, randRotZ));
    //
    //        Vector3 randScale = Vector3.one * Random.Range(minScale, maxScale);
    //
    //        int index = Random.Range(0, landPrefabs.Count);
    //
    //        GameObject newLand = Instantiate(landPrefabs[index], randPos, randRot);
    //        newLand.transform.localScale = randScale;
    //
    //        bool deleted = false;
    //
    //        Collider[] colliders = Physics.OverlapBox(randPos, newLand.GetComponent<Collider>().bounds.size, randRot);
    //
    //        if (colliders != null)
    //        {
    //            foreach (Collider col in colliders)
    //            {
    //                if (col.gameObject != newLand && !col.gameObject.transform.IsChildOf(newLand.transform))
    //                {
    //                    Destroy(newLand);
    //                    deleted = true;
    //                    break;
    //                }
    //            }
    //        }
    //
    //        if (!deleted)
    //        {
    //            landSpawns.Add(newLand);
    //            spawnCount++;
    //        }
    //        yield return null;
    //    }
    //
    //    StartCoroutine(CreateSplines());
    //}
    //
    //IEnumerator CreateSurfaces()
    //{
    //    int spawnCount = 0;
    //    landSpawns = new List<GameObject>();
    //
    //    while (spawnCount < landCount)
    //    {
    //        float randPosX = Random.Range(-halfExtentBounds.x, halfExtentBounds.x);
    //        float randPosZ = Random.Range(-halfExtentBounds.z, halfExtentBounds.z);
    //        Vector3 randPos = new Vector3(randPosX, 0.0f, randPosZ);
    //
    //        if (spawnCount == 0)
    //            randPos = Vector3.zero;
    //
    //        GameObject newSurface = Instantiate(surfacePrefab, randPos, Quaternion.identity);
    //        SplineComputer spline = newSurface.GetComponent<SplineComputer>();
    //
    //        SplinePoint[] points = spline.GetPoints();
    //
    //        for (int i = 0; i < spline.pointCount; i++)
    //        {
    //            float randX = Random.Range(minOffset.x, maxOffset.x);
    //            float randY = Random.Range(minOffset.y, maxOffset.y);
    //            float randZ = Random.Range(minOffset.z, maxOffset.z);
    //
    //            Vector3 randOffset = new Vector3(randX, randY, randZ);
    //            points[i].position += randOffset;
    //        }
    //
    //        spline.SetPoints(points);
    //        landSpawns.Add(newSurface);
    //        spawnCount++;
    //
    //        yield return null;
    //    }
    //
    //    StartCoroutine(CreateSplines());
    //}

}

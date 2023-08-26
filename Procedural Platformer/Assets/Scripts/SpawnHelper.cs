using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace Will
{
    public enum OverlapMode { Box, Sphere, Ray } //Don't know if Ray and Sphere will be used

    [System.Serializable]
    public class SpawnData
    {
        public bool enabled = true;
        public GameObject prefab;
        public bool onlyOnGround;
        public bool canHangOverEdge;
        public bool canBridge;
        public OverlapMode overlapMode;
        public OverlapMode rayCastMode;
    }

    public class SpawnHelper : MonoBehaviour
    {
        public List<SpawnData> spawnObjects;
        //public SpawnData data;

        [Header("Debug")]
        public GameObject floorDebug;
        public GameObject floorDebug2;
        public Collider[] colliders;
        public SplineComputer spline;
        public float rayDist = 50.0f;
        public float sphereRadius = 10.0f;
        public Vector3 halfExtents = new Vector3(10.0f, 10.0f, 10.0f);

        public GameObject spawnedObj = null;
        public BoxCollider spawnedBox = null;
        //public List<Collider> allCollidersEver;

        //private void Start()
        //{
        //    if (data.onlyOnGround)
        //    {
        //        transform.parent = null;
        //        transform.position = Vector3.zero;
        //        BoxCollider box = GetComponent<BoxCollider>();
        //
        //        //Vector3 minCheck = transform.position + new Vector3(box.bounds.min.x, box.bounds.min.y, box.center.z);
        //        //Vector3 maxCheck = transform.position + new Vector3(box.bounds.max.x, box.bounds.min.y, box.center.z);
        //        Vector3 minCheck = transform.InverseTransformPoint(new Vector3(box.bounds.min.x, box.bounds.min.y, box.center.z));
        //        Vector3 maxCheck = transform.InverseTransformPoint(new Vector3(box.bounds.max.x, box.bounds.min.y, box.center.z));
        //
        //        GameObject min = new GameObject("Min");
        //        min.transform.position = minCheck;
        //        min.transform.parent = transform;
        //
        //        GameObject max = new GameObject("Max");
        //        max.transform.position = maxCheck;
        //        max.transform.parent = transform;
        //
        //
        //        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit, rayDist))
        //        {
        //            if (hit.collider != null)
        //            {
        //                Collider floor = hit.collider;
        //                floorDebug = floor.gameObject;
        //                //BoxCollider box = GetComponent<BoxCollider>();
        //
        //                if (box != null)
        //                {
        //                    //Vector3 minCheck = transform.position + new Vector3(box.bounds.min.x, box.bounds.min.y, box.center.z);
        //                    //Vector3 maxCheck = transform.position + new Vector3(box.bounds.max.x, box.bounds.min.y, box.center.z);
        //                    //
        //                    //GameObject min = new GameObject("Min");
        //                    //min.transform.position = minCheck;
        //                    //min.transform.parent = transform;
        //                    //
        //                    //GameObject max = new GameObject("Max");
        //                    //max.transform.position = maxCheck;
        //                    //max.transform.parent = transform;
        //
        //                    if (!Physics.Raycast(minCheck, Vector3.down, out RaycastHit minRay, rayDist))
        //                    {
        //                        //minHit = true;
        //                        Debug.Log($"Failed Min Check {gameObject.name}", gameObject);
        //                        //Destroy(gameObject);
        //                        return;                
        //                    }
        //
        //                    if (!Physics.Raycast(maxCheck, Vector3.down, out RaycastHit maxRay, rayDist))
        //                    {
        //                        //maxHit = true;
        //                        Debug.Log($"Failed Max Check {gameObject.name}", gameObject);
        //                        //Destroy(gameObject);
        //                        return;                            
        //                    }
        //
        //                    //if (hit.distance > box.bounds.extents.y)
        //                    //{
        //                    //    Vector3 orig = transform.position;
        //                    //    //transform.position = new Vector3(hit.point.x, hit.point.y + box.bounds.extents.y, hit.point.z);
        //                    //    transform.position = hit.point;
        //                    //    Vector3 newPos = transform.position;
        //                    //    Debug.Log($"Orig {orig.ToString()} New {newPos.ToString()}", gameObject);
        //                    //}
        //
        //                    Collider[] colliders = Physics.OverlapBox(box.center, box.bounds.extents, transform.rotation);
        //
        //                    if (colliders != null && colliders.Length > 0)
        //                    {
        //                        bool failed = false;
        //
        //                        foreach (Collider col in colliders)
        //                        {
        //                            if (col != floor && col.gameObject != gameObject)
        //                            {
        //                                Debug.Log($"{gameObject.name} Overlap failed with {col.gameObject.name}", col.gameObject);
        //                                failed = true;
        //                                break;
        //                            }
        //                        }
        //
        //                        if (failed)
        //                        {
        //                            //Destroy(gameObject);
        //                            return;                                
        //                        }
        //                    }                        
        //                }
        //            }
        //
        //            else
        //            {
        //                Destroy(gameObject);
        //            }
        //        }
        //    }
        //}

        IEnumerator Start()
        {
            yield return null;

            bool complete = false;
            //transform.parent = null;
            //allCollidersEver = new List<Collider>();

            while (!complete)
            {
                int i = Random.Range(0, spawnObjects.Count);
                GameObject obj = Instantiate(spawnObjects[i].prefab, Vector3.zero, Quaternion.identity);
                obj.name += Random.Range(0, 1000000).ToString();
                BoxCollider box = obj.GetComponent<BoxCollider>();

                Vector3 minCheck = obj.transform.position + new Vector3(box.bounds.min.x, box.bounds.min.y, box.center.z);
                Vector3 maxCheck = obj.transform.position + new Vector3(box.bounds.max.x, box.bounds.min.y, box.center.z);

                GameObject min = new GameObject("Min");
                min.transform.position = minCheck;
                min.transform.parent = obj.transform;

                GameObject max = new GameObject("Max");
                max.transform.position = maxCheck;
                max.transform.parent = obj.transform;

                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;                
                obj.transform.parent = transform;
                //worry about scale later

                if (spawnObjects[i].enabled)
                {
                    if (spawnObjects[i].onlyOnGround)
                    {
                        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit, rayDist))
                        {
                            if (hit.collider != null)
                            {
                                Collider floor = hit.collider;
                                floorDebug = floor.gameObject;
                                //GameObject obj = Instantiate(spawnObjects[i].prefab, transform.position, transform.rotation);
                                //obj.transform.parent = transform;
                                //obj.transform.localScale = transform.localScale;

                                if (spawnObjects[i].overlapMode == OverlapMode.Box)
                                {
                                    //BoxCollider box = obj.GetComponent<BoxCollider>();

                                    if (box != null)
                                    {
                                        //bool minHit = false;
                                        //bool maxHit = false;

                                        //Vector3 minCheck = new Vector3(box.bounds.min.x, box.bounds.min.y, box.center.z);
                                        //Vector3 maxCheck = new Vector3(box.bounds.max.x, box.bounds.min.y, box.center.z);

                                        if (!Physics.Raycast(min.transform.position, Vector3.down, out RaycastHit minRay, rayDist))
                                        {
                                            //minHit = true;
                                            //Debug.Log($" {obj.name} Failed Min Check");
                                            Destroy(obj);
                                            yield return null;
                                            continue;
                                            //yield break;
                                        }

                                        if (!Physics.Raycast(max.transform.position, Vector3.down, out RaycastHit maxRay, rayDist))
                                        {
                                            //Debug.Log($" {obj.name} Failed Max Check");
                                            //maxHit = true;
                                            Destroy(obj);
                                            yield return null;
                                            continue;
                                            //yield break;
                                        }

                                        //obj.transform.position = hit.point;
                                        transform.position = hit.point;

                                        //if (hit.distance > box.bounds.extents.y)
                                        //{
                                        //    //obj.transform.position = new Vector3(hit.point.x, hit.point.y + box.bounds.extents.y, hit.point.z);
                                        //    obj.transform.position = hit.point;
                                        //}

                                        //Collider[] colliders = Physics.OverlapBox(box.center, box.bounds.extents, obj.transform.rotation);
                                        //Collider[] colliders = Physics.OverlapBox(transform.position, box.bounds.extents, Quaternion.identity);
                                        //colliders = Physics.OverlapBox(transform.position, box.bounds.extents, Quaternion.identity);
                                        //colliders = Physics.OverlapBox(box.center + transform.position, box.bounds.extents, transform.rotation);                                    
                                        colliders = Physics.OverlapBox(box.center + transform.position, box.bounds.size, transform.rotation);

                                        if (colliders != null && colliders.Length > 0)
                                        {
                                            //allCollidersEver.AddRange(colliders);

                                            bool failed = false;

                                            foreach (Collider col in colliders)
                                            {
                                                if (col.gameObject != floor.gameObject && col.gameObject != obj)
                                                {
                                                    floorDebug2 = col.gameObject;
                                                    //Debug.Log($" {obj.name} Failed Overlap Box with {col.gameObject.name} floor debug {floorDebug.name}", gameObject);
                                                    failed = true;
                                                    break;
                                                }

                                                //print($"{obj.name} vs {col.gameObject.name}, isTrigger: {col.isTrigger}");
                                            }

                                            if (failed)
                                            {
                                                Destroy(obj);
                                                yield return null;
                                                continue;
                                                //yield break;
                                            }

                                            else
                                            {
                                                complete = true;
                                                spawnedObj = obj;
                                                spawnedBox = box;
                                                //var debugBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                                ////debugBox.GetComponent<BoxCollider>().enabled = false;
                                                //debugBox.transform.position = transform.position + box.center;
                                                //debugBox.transform.rotation = transform.rotation;
                                                ////debugBox.transform.localScale = box.bounds.extents;
                                                //debugBox.transform.localScale = box.bounds.size;
                                                //transform.parent = null;

                                                SplineLevelCreator.Instance.spawnedObjects.Add(obj);
                                                yield break;
                                            }
                                        }

                                        else
                                        {

                                        }
                                    }

                                    else
                                    {
                                        //Debug.LogError($"{obj.name} does not have a box collider, but is used with Overlap Box");
                                        Destroy(obj);
                                    }
                                }
                            }
                        }

                        else
                        {
                            Destroy(obj);
                        }
                    }

                    if (spawnObjects[i].canHangOverEdge)
                    {
                        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit, rayDist))
                        {
                            if (hit.collider != null)
                            {
                                Collider floor = hit.collider;
                                //GameObject obj = Instantiate(spawnObjects[i].prefab, transform.position, transform.rotation);
                                //obj.transform.parent = transform;
                                //obj.transform.localScale = transform.localScale;

                                if (spawnObjects[i].overlapMode == OverlapMode.Box)
                                {
                                    //BoxCollider box = obj.GetComponent<BoxCollider>();

                                    if (box != null)
                                    {
                                        bool minHit = false;
                                        bool maxHit = false;

                                        //Vector3 minCheck = new Vector3(box.bounds.min.x, box.bounds.min.y, box.center.z);
                                        //Vector3 maxCheck = new Vector3(box.bounds.max.x, box.bounds.min.y, box.center.z);

                                        if (Physics.Raycast(minCheck, Vector3.down, out RaycastHit minRay, rayDist))
                                        {
                                            minHit = true;
                                        }

                                        if (Physics.Raycast(maxCheck, Vector3.down, out RaycastHit maxRay, rayDist))
                                        {
                                            maxHit = true;
                                        }

                                        if (hit.distance > box.bounds.extents.y)
                                        {
                                            obj.transform.position = new Vector3(hit.point.x, hit.point.y + box.bounds.extents.y, hit.point.z);
                                        }

                                        if (minHit || maxHit)
                                        {
                                            Collider[] colliders = Physics.OverlapBox(box.center, box.bounds.extents, obj.transform.rotation);

                                            if (colliders != null && colliders.Length > 0)
                                            {
                                                bool failed = false;

                                                foreach (Collider col in colliders)
                                                {
                                                    if (col != floor)
                                                    {
                                                        failed = true;
                                                        break;
                                                    }
                                                }

                                                if (failed)
                                                {
                                                    Destroy(obj);
                                                    yield return null;
                                                    //continue;
                                                }
                                            }

                                            else
                                            {
                                                complete = true;
                                                yield break;
                                            }
                                        }
                                    }

                                    else
                                        Destroy(obj);
                                }
                            }
                        }
                    }

                    if (spawnObjects[i].canBridge)
                    {
                        //GameObject obj = Instantiate(spawnObjects[i].prefab, transform.position, transform.rotation);
                        //obj.transform.parent = transform;
                        //obj.transform.localScale = transform.localScale;

                        if (spawnObjects[i].overlapMode == OverlapMode.Box)
                        {
                            //BoxCollider box = obj.GetComponent<BoxCollider>();

                            if (box != null)
                            {
                                bool minHit = false;
                                bool maxHit = false;

                                //Vector3 minCheck = new Vector3(box.bounds.min.x, box.bounds.min.y, box.center.z);
                                //Vector3 maxCheck = new Vector3(box.bounds.max.x, box.bounds.min.y, box.center.z);

                                if (Physics.Raycast(minCheck, Vector3.down, out RaycastHit minRay, rayDist))
                                {
                                    minHit = true;
                                }

                                if (Physics.Raycast(maxCheck, Vector3.down, out RaycastHit maxRay, rayDist))
                                {
                                    maxHit = true;
                                }

                                if (minHit)
                                {
                                    if (minRay.distance > box.bounds.extents.y)
                                    {
                                        obj.transform.position = new Vector3(minRay.point.x, minRay.point.y + box.bounds.extents.y, minRay.point.z);
                                    }
                                }

                                if (maxHit)
                                {
                                    if (maxRay.distance > box.bounds.extents.y)
                                    {
                                        obj.transform.position = new Vector3(maxRay.point.x, maxRay.point.y + box.bounds.extents.y, maxRay.point.z);
                                    }
                                }


                                if (minHit && maxHit)
                                {
                                    Collider[] colliders = Physics.OverlapBox(box.center, box.bounds.extents, obj.transform.rotation);

                                    if (colliders != null && colliders.Length > 0)
                                    {
                                        bool failed = false;

                                        foreach (Collider col in colliders)
                                        {
                                            if (col != minRay.collider && col != maxRay.collider)
                                            {
                                                failed = true;
                                                break;
                                            }
                                        }

                                        if (failed)
                                        {
                                            Destroy(obj);
                                            yield return null;
                                            //continue;
                                        }
                                    }

                                    else
                                    {
                                        complete = true;
                                        yield break;
                                    }
                                }
                            }
                        }

                        else
                        {
                            Destroy(obj);
                        }
                    }


                }

                yield return null;
            }
        }
    }

}

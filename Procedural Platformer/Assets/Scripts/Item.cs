using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;

    public enum ActionType { AddForce, Instantiate, SetDrag, Lerp };
    public enum ActionTarget { Player, Aimed_Position, SpawnTarget }

    public ActionType actionType;
    public ActionTarget actionTarget;

    [Space(10)]
    [Header("Add Force")]
    public float force = 100.0f;
    public ForceMode forceMode = ForceMode.Force;
    public bool forceDirIsRelative = false;
    public Vector3 forceDir = new Vector3(0, 1, 0);
    public bool useForceCurve = false;
    public AnimationCurve forceCurve; //Obtain max time by getting time value of last Key    

    [Space(10)]
    [Header("Instantiate")]
    public GameObject prefab;
    public Vector3 relativeSpawnPos = new Vector3(0, -1, 0);
    public bool useLifeTime = false;
    public float lifeTime = 10.0f;

    [Space(10)]
    [Header("Set Drag")]
    public float drag = 0.0f;
    public bool useDragTime = true;
    public float maxDragTime = 5.0f;
    public bool useDragCurve = false;
    public AnimationCurve dragCurve;
}


public class Item : MonoBehaviour
{
    public string itemName;
    public int quantity = 1;
    public ItemData[] itemActions;

    [HideInInspector] public bool isPickup = false;
    [HideInInspector] public bool isSpawned = false;

    IEnumerator Start()
    {
        yield return null;

        if (!isSpawned)
            isPickup = true;
    }      
}

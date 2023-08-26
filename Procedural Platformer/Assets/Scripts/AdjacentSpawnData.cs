using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdjacentData
{
    public GameObject obj;

    [Header("Bound Ranges")]
    [Range(0, 1)]
    public float minXBounds;
    [Range(0, 1)]
    public float maxXBounds;
    [Range(0, 1)]
    public float minYBounds;
    [Range(0, 1)]
    public float maxYBounds;
    [Range(0, 1)]
    public float minZBounds;
    [Range(0, 1)]
    public float maxZBounds;

    [Header("Rotation Settings")]
    public bool canRotate;
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;

    [Header("Scale Settings")]
    public bool canScale;
    public float minScale = 1;
    public float maxScale = 1;

}

public class AdjacentSpawnData : MonoBehaviour
{
    public List<AdjacentData> objects;
    public int minCount = 0;
    public int maxCount = 10;
    public bool isAnAdjacentObject = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

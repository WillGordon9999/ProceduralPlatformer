using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get { return mInstance; } private set { } }
    private static CollectibleManager mInstance;    
    List<GameObject> collectibles;
    uint totalCollectibleCount = 0;
    uint currentCollectibleCount = 0;
    UnityEngine.UI.Text text;

    private void Awake()
    {
        if (mInstance == null)
            mInstance = this;
        //collectibles = new List<GameObject>();
        text = GameObject.Find("Score").GetComponent<UnityEngine.UI.Text>();
    }
       
    void Update()
    {
        text.text = $"{currentCollectibleCount} / {totalCollectibleCount}";
    }

    //private void OnGUI()
    //{
    //    GUI.Label(new Rect(0, 0, 400, 300), $"{currentCollectibleCount} / {totalCollectibleCount}");
    //}

    public void Add(GameObject obj)
    {
        totalCollectibleCount++;
    }

    public void Remove(GameObject obj)
    {
        currentCollectibleCount++;
    }

}

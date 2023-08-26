using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Spawn Point Asset")]
public class SpawnPointAsset : ScriptableObject
{
    public List<Vector3> xPoints;
    public List<Vector3> yPoints;
    public List<Vector3> zPoints;
}

#if UNITY_EDITOR

public class SpawnPointEditor
{
    [UnityEditor.MenuItem("Window/Create Spawn Point List")]
    public static void CreateSpawnPointList()
    {
        GameObject obj = UnityEditor.Selection.activeGameObject;

        if (obj != null)
        {
            SpawnPointData data = obj.GetComponent<SpawnPointData>();

            SpawnPointAsset asset = ScriptableObject.CreateInstance<SpawnPointAsset>();
            asset.xPoints = new List<Vector3>();
            asset.yPoints = new List<Vector3>();
            asset.zPoints = new List<Vector3>();

            UnityEditor.AssetDatabase.CreateAsset(asset, $"Assets/Spawn Point Assets/{obj.name} Spawn Points.asset");
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectEntry
{
    public string name;
    public GameObject obj;
    public bool enabled = true;

    [Space(10)]
    [Header("Offset")]
    public bool overrideOffset = false;
    public Vector3 minOffset;
    public Vector3 maxOffset;

    [Space(10)]
    [Header("Rotate")]
    public bool overrideRotate = false;
    public Vector3 minRotation;
    public Vector3 maxRotation;

    [Space(10)]
    [Header("Scale")]
    public bool overrideScale = false;
    public float minScale = 1.0f;
    public float maxScale = 1.0f;
}

[CreateAssetMenu(menuName = "Create Object List")]
public class SpawnObjectList : ScriptableObject
{
    public List<ObjectEntry> objects;
}

#if UNITY_EDITOR
//public class ObjectListEditor
//{
//    [UnityEditor.MenuItem("Window/Create Spawn Object List")]
//    public static void CreateObjectList()
//    {
//        GameObject obj = UnityEditor.Selection.activeGameObject;
//
//        if (obj != null)
//        {
//            SpawnPointData data = obj.GetComponent<SpawnPointData>();
//
//            if (data.adjacents != null && data.adjacents.Count > 0)
//            {
//                SpawnObjectList adjacentList = ScriptableObject.CreateInstance<SpawnObjectList>();
//                adjacentList.objects = new List<ObjectEntry>();
//
//                foreach(GameObject entry in data.adjacents)
//                {
//                    ObjectEntry newEntry = new ObjectEntry();
//                    newEntry.name = entry.name;
//                    newEntry.obj = entry;
//                    adjacentList.objects.Add(newEntry);
//                }
//
//                UnityEditor.AssetDatabase.CreateAsset(adjacentList, $"Assets/Spawn Object Lists/{obj.name} Adjacents.asset");
//            }
//
//            if (data.stackables != null && data.stackables.Count > 0)
//            {
//                SpawnObjectList stackables = ScriptableObject.CreateInstance<SpawnObjectList>();
//                stackables.objects = new List<ObjectEntry>();
//
//                foreach(GameObject entry in data.stackables)
//                {
//                    ObjectEntry newEntry = new ObjectEntry();
//                    newEntry.name = entry.name;
//                    newEntry.obj = entry;
//                    stackables.objects.Add(newEntry);
//                }
//
//                UnityEditor.AssetDatabase.CreateAsset(stackables, $"Assets/Spawn Object Lists/{obj.name} Stackables.asset");
//            }
//
//            UnityEditor.AssetDatabase.Refresh();
//        }
//    }
//}


//[UnityEditor.CustomEditor(typeof(ObjectEntry))]
//public class ObjectEntryEditor : UnityEditor.Editor
//{
//    UnityEditor.SerializedProperty nameLabel;
//    UnityEditor.SerializedProperty obj;
//
//    private void OnEnable()
//    {
//        nameLabel = serializedObject.FindProperty("name");
//        obj = serializedObject.FindProperty("obj");
//    }
//
//    public override void OnInspectorGUI()
//    {
//        Debug.Log("In Inspector");
//        serializedObject.Update();
//
//        //UnityEditor.EditorGUILayout.PropertyField(nameLabel);
//        //UnityEditor.EditorGUILayout.PropertyField(obj);
//        //
//        //if (obj.objectReferenceValue.name != nameLabel.stringValue)
//        //{
//        //    nameLabel.stringValue = obj.objectReferenceValue.name;
//        //}
//
//        UnityEditor.EditorGUI.BeginChangeCheck();
//
//        UnityEditor.EditorGUILayout.PropertyField(obj);
//        
//        serializedObject.ApplyModifiedProperties();
//        
//        if (UnityEditor.EditorGUI.EndChangeCheck())
//        {
//            Debug.Log("Changing name");
//            nameLabel.stringValue = obj.objectReferenceValue.name;
//        }
//
//        serializedObject.ApplyModifiedProperties();
//    }
//}

#endif

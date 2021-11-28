using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for the camera to follow the player safely
/// </summary>
public class CameraMoveTracker : MonoBehaviour
{
    public Transform target;
        
    void Update()
    {
        if (target != null)
            transform.position = target.transform.position;
        //Quaternion rot = Quaternion.LookRotation(Vector3.forward, -Physics.gravity.normalized);
        //transform.rotation = rot;
    }
}

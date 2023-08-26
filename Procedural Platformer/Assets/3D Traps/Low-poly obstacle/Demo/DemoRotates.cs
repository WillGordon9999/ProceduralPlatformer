using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoRotates : MonoBehaviour
{
    [SerializeField]
    private float speed = 500f;


    public Transform customPivot;



    void Update()
    {


        transform.RotateAround(customPivot.position, Vector3.up, speed * Time.deltaTime);

    }
}

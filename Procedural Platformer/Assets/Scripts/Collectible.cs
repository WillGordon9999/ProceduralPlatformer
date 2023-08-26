using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    Rigidbody rb;    
    void Start()
    {        
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Collider>().isTrigger = true;
        CollectibleManager.Instance.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.transform.root.tag == "Player")
        {
            CollectibleManager.Instance.Remove(gameObject);
            Destroy(gameObject);
        }       
    }   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainState : State<Player>
{
    bool climbing = false;
    Vector3 climbPoint;
    public override void EnterState(Player owner) { }        
    public override void UpdateState(Player owner)
    {
        //owner.movement.Move();
        //owner.movement.Jump();
    }
    public override void ExitState(Player owner) {}
    public override void OnCollisionEnter(Player owner, Collision collision) {}
    public override void OnCollisionStay(Player owner, Collision collision)
    {
        //climbing = true;
        //climbPoint = collision.collider.ClosestPointOnBounds(owner.transform.position);
    }
    public override void OnCollisionExit(Player owner, Collision collision)
    {
        //climbing = false;
    }    
    public override void OnTriggerEnter(Player owner, Collider collider) {}

    public override void OnTriggerStay(Player owner, Collider collider) {}
    public override void OnTriggerExit(Player owner, Collider collider) {}   

}

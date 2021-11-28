using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public StateMachine<Player> stateMachine;
    public Movement movement;

    void Start()
    {
        movement = GetComponent<Movement>();
        stateMachine = new StateMachine<Player>(this);
        stateMachine.ChangeState<MainState>();
    }
    
    void Update()
    {
        stateMachine.Update();
    }

    private void OnCollisionEnter(Collision collision)
    {
        stateMachine.OnCollisionEnter(this, collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        stateMachine.OnCollisionStay(this, collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        stateMachine.OnCollisionExit(this, collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        stateMachine.OnTriggerEnter(this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        stateMachine.OnTriggerEnter(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        stateMachine.OnTriggerExit(this, other);
    }

}

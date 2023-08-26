using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateMachine : MonoBehaviour
{
    public static PlayerStateMachine Instance { get { return mInstance; } private set { } }
    private static PlayerStateMachine mInstance = null;

    //Main State Delegates
    public string stateName = "default";
    Action enter;
    Action update;
    Action fixedUpdate;
    Action exit;

    [HideInInspector] public Movement move;
    [HideInInspector] public Rigidbody rb;
    InventoryController inventory;    

    void Awake()
    {
        if (mInstance == null)
            mInstance = this;

        rb = GetComponent<Rigidbody>();
        move = GetComponent<Movement>();
        inventory = GetComponent<InventoryController>();
    }

    void Start()
    {
        ChangeState("Main", null, null, MainUpdate, MainFixedUpdate);
    }

    private void FixedUpdate()
    {
        if (fixedUpdate != null)
            fixedUpdate.Invoke();
    }

    void Update()
    {
        if (update != null)
            update.Invoke();
    }

    void ChangeState(string name, Action enter, Action exit, Action update, Action fixedUpdate = null)
    {
        if (exit != null)
            exit.Invoke();

        stateName = name;
        this.enter = enter;
        this.exit = exit;
        this.update = update;
        this.fixedUpdate = fixedUpdate;

        if (enter != null)
            enter.Invoke();
    }

    void MainUpdate()
    {
        move.FallCheck();

        if (!inventory.CheckForItemUse())
            move.Jump();

        move.GroundCheck();
        move.UpdateMove();

        
        inventory.UpdatePickup();

        move.UpdateJump();
    }

    void MainFixedUpdate()
    {
        move.FixedUpdateJump();
        move.FixedUpdateMove();
    }
}

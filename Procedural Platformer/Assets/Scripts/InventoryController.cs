using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public class InventorySlot
    {
        public string itemName;
        public ItemData[] itemActions;
        public int quantity = 0;
    }

    public int slotCount = 4;
    public List<InventorySlot> slots;
    
    [Space(10)]
    [Header("Pickup Settings")]
    public float pickUpRadius = 3.0f;
    public int itemPoolSize = 10;
    public float swapSlotTime = 2.0f;    
    int swapSlot = -1;
    int swapSlot2 = -1;
    Collider[] colliderList;
    Item closestItem;
    float itemDist = -1.0f;

    [Space(10)]
    [Header("Use Item Settings")]
    public float aimDist = 10.0f;


    [Space(10)]
    [Header("GUI Settings")]
    public float xOffset = 0.1f;
    public float yOffset = 0.7f;
    public float slotWidth = 300;
    public float slotHeight = 300;
    [Space(10)]
    public float xPickupOffset = 0.5f;
    public float yPickupOffset = 0.8f;
    public float pickUpWidth = 300;
    public float pickUpHeight = 100;

    Camera mainCamera;

    void Start()
    {
        slots = new List<InventorySlot>(slotCount);

        mainCamera = Camera.main;

        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new InventorySlot());
        }

    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width * xOffset, Screen.height * yOffset, slotWidth, slotHeight));

        GUILayout.BeginVertical();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemName != "")
            {
                if (swapSlot != i)
                    GUILayout.Label($"{slots[i].itemName} x {slots[i].quantity}");
                else
                    GUILayout.Label($"{slots[i].itemName} x {slots[i].quantity} SWAP");
            }

            else
            {
                if (swapSlot != i)
                    GUILayout.Label("None");
                else
                    GUILayout.Label("None SWAP");
            }
        }

        GUILayout.EndVertical();

        GUILayout.EndArea();

        if (closestItem != null && closestItem.itemName != "")
        {
            GUI.Label(new Rect(Screen.width * xPickupOffset, Screen.height * yPickupOffset, pickUpWidth, pickUpHeight), $"{closestItem.itemName}");
        }
    }

    public bool CheckForItemUse()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.leftTrigger.isPressed)
            {
                int index = -1;

                if (Gamepad.current.yButton.wasPressedThisFrame)
                    index = 0;

                if (Gamepad.current.bButton.wasPressedThisFrame)
                    index = 1;

                if (Gamepad.current.aButton.wasPressedThisFrame)
                    index = 2;

                if (Gamepad.current.xButton.wasPressedThisFrame)
                    index = 3;

                if (index != -1)
                {
                    if (slots[index].quantity > 0)
                    {
                        UseItem(slots[index]);
                    }
                }

                return true;
            }
        }

        return false;
    }

    public void UpdatePickup()
    {
        if (Gamepad.current == null)
            return;

        colliderList = Physics.OverlapSphere(transform.position, pickUpRadius);

        //if (colliderList == null || colliderList.Length == 0)
        //{
        //    closestItem = null;
        //    itemDist = -1.0f;
        //}
        closestItem = null;
        itemDist = -1.0f;

        for (int i = 0; i < colliderList.Length; i++)
        {
            if (colliderList[i] != null && colliderList[i].TryGetComponent<Item>(out Item item))
            {
                float dist = Vector3.Distance(transform.position, colliderList[i].ClosestPoint(transform.position));

                if (closestItem == null)
                {
                    closestItem = item;
                    itemDist = dist;
                }

                else
                {
                    if (dist < itemDist)
                    {
                        closestItem = item;
                        itemDist = dist;
                    }
                }
            }
        }

        if (closestItem != null)
        {           
            bool foundItem = false;

            for (int i = 0; i < slotCount; i++)
            {
                if (slots[i].itemName == closestItem.itemName)
                {
                    foundItem = true;
                    slots[i].quantity += closestItem.quantity;
                    Destroy(closestItem.gameObject);
                    closestItem = null;
                    break;
                }
            }

            if (!foundItem && Gamepad.current.leftTrigger.isPressed)
            {
                int addIndex = -1;

                if (Gamepad.current.yButton.wasPressedThisFrame)                
                    addIndex = 0;
                
                if (Gamepad.current.bButton.wasPressedThisFrame)                
                    addIndex = 1;
                
                if (Gamepad.current.aButton.wasPressedThisFrame)                
                    addIndex = 2;
                
                if (Gamepad.current.xButton.wasPressedThisFrame)                
                    addIndex = 3;
                
                if (addIndex != -1)
                {
                    slots[addIndex].itemName = closestItem.itemName;
                    slots[addIndex].itemActions = closestItem.itemActions;
                    slots[addIndex].quantity = closestItem.quantity;
                    Destroy(closestItem.gameObject);
                    closestItem = null;
                }

                //for (int i = 0; i < slotCount; i++)
                //{
                //    if (slots[i].quantity == 0)
                //    {
                //        slots[i].itemName = closestItem.itemName;
                //        slots[i].itemActions = closestItem.itemActions;
                //        slots[i].quantity = closestItem.quantity;
                //        Destroy(closestItem.gameObject);
                //        closestItem = null;
                //        break;
                //    }
                //}
            }
        } 
        
        if (Gamepad.current.dpad.up.wasPressedThisFrame)
        {
            HandleSwap(0);
        }

        if (Gamepad.current.dpad.right.wasPressedThisFrame)
        {
            HandleSwap(1);
        }

        if (Gamepad.current.dpad.down.wasPressedThisFrame)
        {
            HandleSwap(2);
        }

        if (Gamepad.current.dpad.left.wasPressedThisFrame)
        {
            HandleSwap(3);
        }
    }

    void HandleSwap(int index)
    {
        if (swapSlot < 0)
        {
            swapSlot = index;
            StartCoroutine(SwapTimer());
        }

        else
        {
            swapSlot2 = index;

            if (swapSlot != swapSlot2)
            {
                InventorySlot temp = slots[swapSlot];
                slots[swapSlot] = slots[swapSlot2];
                slots[swapSlot2] = temp;
            }

            swapSlot = -1;
            swapSlot2 = -1;
        }
    }

    IEnumerator SwapTimer()
    {
        yield return new WaitForSeconds(swapSlotTime);
        swapSlot = -1;
        swapSlot2 = -1;
    }

    public void UseItem(InventorySlot slot)
    {
        slot.quantity--;

        ItemData[] actions = slot.itemActions;

        for (int i = 0; i < actions.Length; i++)
        {
            switch (actions[i].actionType)
            {
                case ItemData.ActionType.AddForce:
                    AddForce(actions[i]);
                    break;

                case ItemData.ActionType.Instantiate:
                    Spawn(actions[i]);
                    break;

                case ItemData.ActionType.SetDrag:
                    SetDrag(actions[i]);
                    break;

                case ItemData.ActionType.Lerp:
                    Lerp(actions[i]);
                    break;
            }
        }
    }

    void AddForce(ItemData data)
    {
        GameObject target = null;
        Rigidbody rb = null;

        if (data.actionTarget == ItemData.ActionTarget.Player)
        {
            target = gameObject;
            rb = PlayerStateMachine.Instance.rb;
        }

        if (data.actionTarget == ItemData.ActionTarget.Aimed_Position)
        {
            Vector3 dir = mainCamera.transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(new Ray(mainCamera.transform.position, dir), out RaycastHit hit, aimDist))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody body))
                    {
                        rb = body;
                        target = hit.collider.gameObject;
                    }
                }
            }
        }

        if (!data.useForceCurve)
        {
            if (!data.forceDirIsRelative)
            {
                rb.AddForce(data.forceDir * data.force, data.forceMode);                
            }

            else
            {
                rb.AddRelativeForce(data.forceDir * data.force, data.forceMode);
            }
        }

        else
        {
            StartCoroutine(AddForceCurve(rb, data));
        }
    }

    IEnumerator AddForceCurve(Rigidbody rb, ItemData data)
    {
        float timer = 0.0f;
        float maxTime = 0.0f;

        if (data.forceCurve.keys.Length > 0)
        {
            maxTime = data.forceCurve.keys[data.forceCurve.length - 1].time;

            while (timer <= maxTime)
            {
                float force = data.forceCurve.Evaluate(timer);

                if (!data.forceDirIsRelative)
                {
                    rb.AddForce(data.forceDir * force, data.forceMode);
                }

                else
                {
                    rb.AddRelativeForce(data.forceDir * force, data.forceMode);
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }        
    }

    void Spawn(ItemData data)
    {
        if (data.actionTarget == ItemData.ActionTarget.Player)
        {
            Vector3 spawn = transform.TransformPoint(data.relativeSpawnPos);
            Instantiate(data.prefab, spawn, Quaternion.identity);
        }

        if (data.actionTarget == ItemData.ActionTarget.Aimed_Position)
        {
            Vector3 spawn = transform.position + mainCamera.transform.TransformDirection(Vector3.forward) * aimDist;
            Instantiate(data.prefab, spawn, Quaternion.identity);
        }
    }

    void SetDrag(ItemData data)
    {
        if (data.actionTarget == ItemData.ActionTarget.Player)
        {
            Rigidbody rb = PlayerStateMachine.Instance.rb;

            if (data.useDragTime)
                StartCoroutine(SetDragTimed(data, rb));

            else if (data.useDragCurve)
                StartCoroutine(SetDragCurve(data, rb));

            else
            {
                rb.drag = data.drag;
            }
        }
    }

    IEnumerator SetDragTimed(ItemData data, Rigidbody rb)
    {
        float origDrag = rb.drag;
        rb.drag = data.drag;
        yield return new WaitForSeconds(data.maxDragTime);
        rb.drag = origDrag;
    }

    IEnumerator SetDragCurve(ItemData data, Rigidbody rb)
    {
        float timer = 0.0f;
        float maxTime = 0.0f;

        if (data.forceCurve.keys.Length > 0)
        {
            maxTime = data.forceCurve.keys[data.forceCurve.length - 1].time;

            while (timer <= maxTime)
            {                
                rb.drag = data.forceCurve.Evaluate(timer);

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    void Lerp(ItemData data)
    {

    }    
}

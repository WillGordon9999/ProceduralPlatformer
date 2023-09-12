using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Rewired;
public class Movement : MonoBehaviour
{
    [Header("Main Movement")]
    public float moveSpeed = 10.0f;
    public float airMoveSpeed = 10.0f;
    public float rotSpeed = 0.15f;
    public bool useStopMove = true;
    public float stopMoveDistanceOrig = 1.5f;
    public float stopMoveOffset = 0.2f;
    
    //Invector Experimental
    public float stopMoveDistance = 0.1f;
    public float slopeLimit = 75.0f;
    public float stopMoveHeight = 0.65f;
    public float maxSlope = 85f;
    public LayerMask stopMoveLayer;
    public LayerMask groundLayer;

    [Header("Jumping")]
    public float jumpSpeed = 10.0f;
    public float jumpLerpVal = 10.0f;
    public float minJumpHeight = 0.1f;
    public float maxJumpHeight = 3.0f;
    public float jumpSweepDist = 0.1f;
    public bool jumpZeroVelocity = true; //Zero out the Vertical Velocity for the duration of the jump
    public bool stopAtTarget = true;
    public int maxJumpCount = 1;

    [Header("New Jump")]
    public float jumpForce = 10.0f;
    public ForceMode jumpMode = ForceMode.Force;    
   
    public float maxJumpTime = 5.0f;
    public bool useBaseDownForce = true;
    public float baseDownForce = 50.0f;
    public ForceMode downForceMode = ForceMode.Force;

    public bool useEarlyCancelDownForce = true;    
    public float earlyCancelDownForce = 50.0f;    
    public ForceMode earlyCancelMode = ForceMode.Force;
    public bool useCancelCoroutine = true;
    public float cancelCoroutineForce = 10.0f;
    public ForceMode coroutineMode = ForceMode.Force;
    float jumpTimer = 0.0f;

    float maxDeceleration = 5.0f;
    bool useEarlyCancelScale = true;
    float earlyCancelScale = 2.0f;

    bool useDecelerationCurve = true;
    AnimationCurve decelerationCurve;    
    bool useEarlyDecelerationCurve = true;
    AnimationCurve earlyDecelerationCurve;
    ForceMode decelerationMode = ForceMode.Force;
    ForceMode earlyDecelerationMode = ForceMode.Force;

    [Space(10)]
    [Header("Climbing")]
    public float climbSpeed = 20.0f;
    public float climbingAngle = 3.0f;

    [Header("Ground Check")]
    public Vector3 rayOrigin = Vector3.zero; //The position in player local space to fire the raycast from
    public float rayDistance = 1.5f;
    public LayerMask groundLayerMask = ~0; //By default everything

    [Header("Wall Check")]
    public float wallHitDist = 0.2f;
    public float wallMoveScale = 1.0f;
    public RaycastHit wallHit;

    [Header("Step Handling")]
    public float stepOffsetDist = 0.1f;
    public float stepOffsetMinHeight = 0.0f;
    public float stepOffsetMaxHeight = 0.5f;
    bool applyStepOffset = false;

    [SerializeField] bool isGrounded = false;
    [SerializeField] bool groundCheckEnabled = true;
    Vector3 rayPosLocal;
    Vector3 rayFinalPos;
    RaycastHit groundHit;

    CapsuleCollider capsule;
    Transform zeroAnchor;

    new GameObject camera;
    Rigidbody rb;
    Vector3 moveVector;
    Vector3 verticalVelocityVec;
    float verticalVelocity;
    bool moveInput = false;
    Transform wallRayPoint;
    Transform wallRayPoint2;

    //Step Offset 
    Vector3 stepPointTop;
    Vector3 stepPointDown;
    Vector3 stepHitPoint;
    Vector3 stepPos;
    Vector3 stepDir;
    bool sphere;


    //Jump variables
    [Header("Jump Debug")]
    public int jumpCount;
    public Vector3 origJumpPos;
    public Vector3 jumpTarget;
    public bool isJumping = false;
    public float distFromJumpStart;
    public float wallDebug = 0.0f;


    // Start is called before the first frame update
    void Start()
    {        
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        rb = GetComponent<Rigidbody>();
        jumpCount = maxJumpCount;
        capsule = GetComponent<CapsuleCollider>();
        zeroAnchor = camera.GetComponent<CameraOrbit>().zeroAnchor.transform;
        wallRayPoint = transform.Find("WallRayPoint");
        wallRayPoint2 = transform.Find("WallRayPoint2");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded && !groundCheckEnabled)
        {
            rayFinalPos = transform.TransformPoint(rayOrigin);

            Vector3 down = transform.TransformDirection(Vector3.down);
            QueryTriggerInteraction query = QueryTriggerInteraction.Ignore;

            Debug.DrawLine(rayFinalPos, transform.TransformPoint(Vector3.down * rayDistance), Color.red);

            if (Physics.Raycast(rayFinalPos, down, out groundHit, rayDistance, groundLayerMask.value, query))
            {
                print("Emergency Ground Check Reset");
                groundCheckEnabled = true;
                isGrounded = true;
                jumpCount = maxJumpCount;
            }

            else
            {
                //If player walks off an edge take one jump away, in case they have multiple jumps
                if (isGrounded)
                {
                    jumpCount--;
                }

                isGrounded = false;
            }
        }
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (wallHit.collider != null && wallHit.collider.gameObject == collision.gameObject)
    //    {
    //
    //    }
    //}

    /// <summary>
    /// Check for ground here, this is important as the OnCollision callbacks are not reliable for uneven terrain
    /// </summary>
    public void GroundCheck()
    {
        if (groundCheckEnabled)
        {
            //Recalculate the rayPos if needed
            //if (rayOrigin != rayPosLocal)
            //{
            //    rayPosLocal = rayOrigin;
            //    rayFinalPos = transform.TransformPoint(rayPosLocal);
            //}
            rayFinalPos = transform.TransformPoint(rayOrigin);

            Vector3 down = transform.TransformDirection(Vector3.down);
            QueryTriggerInteraction query = QueryTriggerInteraction.Ignore;

            Debug.DrawLine(rayFinalPos, transform.TransformPoint(Vector3.down * rayDistance), Color.red);

            if (Physics.Raycast(rayFinalPos, down, out groundHit, rayDistance, groundLayerMask.value, query))
            {
                isGrounded = true;
                jumpCount = maxJumpCount;
            }

            else
            {
                //If player walks off an edge take one jump away, in case they have multiple jumps
                if (isGrounded)
                {
                    jumpCount--;
                }

                isGrounded = false;
            }

        }
    }

    /// <summary>
    /// Primarily Calculate Vertical Velocity here to check if falling
    /// </summary>
    public void FallCheck()
    {
        verticalVelocityVec = rb.GetRelativePointVelocity(transform.up);
        Vector3 vel = Vector3.Project(verticalVelocityVec, -Physics.gravity.normalized);

        if (vel.x != 0.0f) verticalVelocity = vel.x;
        if (vel.y != 0.0f) verticalVelocity = vel.y;
        if (vel.z != 0.0f) verticalVelocity = vel.z;

        verticalVelocityVec = vel;
    }

    /// <summary>
    /// Zero out the vertical velocity, this is useful for early jump cancels and other mechanics
    /// </summary>
    public void ZeroVerticalVelocity()
    {
        rb.velocity -= verticalVelocityVec;
    }
    
    /// <summary>
    /// The function to check for input in main update for movement, actual movement is handled in fixed update
    /// to work better with physics
    /// </summary>
    public void UpdateMove()
    {        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        
        if (x != 0.0f || y != 0.0f)
        {
            moveInput = true;
            Vector3 dir = zeroAnchor.TransformDirection(new Vector3(x, 0.0f, y));
            if (dir.magnitude > 1) dir.Normalize();
            Quaternion rot = Quaternion.LookRotation(dir, transform.TransformDirection(Vector3.up));

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed);
            moveVector = dir;

            //HandleStepOffset();

            //if (!applyStepOffset)
            if (useStopMove)
            {
                Ray ray = new Ray(wallRayPoint.position, moveVector.normalized);
                Ray ray2 = new Ray(wallRayPoint2.position, moveVector.normalized);

                //Debug.DrawRay(ray.origin, ray.direction * stopMoveDistance, Color.red);

                //Vector3 fwd = transform.TransformDirection(Vector3.forward);
                //Vector3 zeroFwd = zeroAnchor.TransformDirection(Vector3.forward);
                //Vector3 left = transform.TransformDirection(Vector3.left);

                //if (Physics.Raycast(ray, out wallHit, capsule.radius + stopMoveDistance))                
                int layer = ~(1 << gameObject.layer);
                //if (Physics.SphereCast(ray, stopMoveRadius, out wallHit, stopMoveDistance, ~0, QueryTriggerInteraction.Ignore))
                //if (Physics.BoxCast(wallRayPoint.position, stopExtents, moveVector.normalized, out RaycastHit hit, Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)), stopMoveDistance, ~0))
                Vector3 right = wallRayPoint.TransformPoint(Vector3.right * stopMoveOffset);
                Vector3 left = wallRayPoint.TransformPoint(Vector3.left * stopMoveOffset);

                if (Physics.Raycast(ray, out wallHit, stopMoveDistanceOrig, layer, QueryTriggerInteraction.Ignore))
                {
                    moveVector = Vector3.ProjectOnPlane(moveVector, wallHit.normal);
                }

                if (Physics.Raycast(ray2, out wallHit, stopMoveDistanceOrig, layer, QueryTriggerInteraction.Ignore))
                {
                    moveVector = Vector3.ProjectOnPlane(moveVector, wallHit.normal);
                }

                if (Physics.Raycast(right, moveVector.normalized, out wallHit, stopMoveDistanceOrig, layer, QueryTriggerInteraction.Ignore))
                {
                    moveVector = Vector3.ProjectOnPlane(moveVector, wallHit.normal);
                }

                if (Physics.Raycast(left, moveVector.normalized, out wallHit, stopMoveDistanceOrig, layer, QueryTriggerInteraction.Ignore))
                {
                    moveVector = Vector3.ProjectOnPlane(moveVector, wallHit.normal);
                }

            }

            StopMove();

            //Wall Check
            //Ray ray = new Ray(transform.TransformPoint(rayOrigin), moveVector.normalized);
            //
            //if (Physics.Raycast(ray, out RaycastHit wallHit, wallHitDist))
            //{
            //    moveVector = Vector3.ProjectOnPlane(moveVector, wallHit.normal);
            //    //moveVector += wallHit.normal * wallMoveScale;
            //}

            if (isGrounded)
            {
                Vector3 origVector = moveVector;
                moveVector = Vector3.ProjectOnPlane(moveVector, groundHit.normal);

                float angle = Vector3.SignedAngle(origVector, moveVector, transform.TransformDirection(Vector3.left));
                float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
                sin = (1.0f - sin);
                sin = Mathf.Clamp(sin, 0.0f, 1.0f);
                angle = sin;
                moveVector = moveVector * moveSpeed * angle;
            }

            else
            {
                moveVector *= airMoveSpeed;
            }

          

        }

        else
        {
            moveInput = false;
            moveVector = Vector3.zero;
        }
             
    }

    void HandleStepOffset()
    {
        if (isGrounded)
        {
            Vector3 dir = moveVector;

            //float distance = capsule.radius + stepOffsetDistance;
            //float height = (stepOffsetMaxHeight + 0.01f + capsule.radius * 0.5f);

            float stepDistance = capsule.radius + stepOffsetDist;
            float stepHeight = (stepOffsetMaxHeight + 0.01f + capsule.radius * 0.5f);

            //Vector3 pA = transform.position + transform.up * (stepOffsetMinHeight + 0.05f);
            //Vector3 pB = pA + dir.normalized * stepDistance;

            stepPointTop = transform.position + transform.up * (stepOffsetMinHeight + 0.05f);
            stepPointDown = stepPointTop + dir.normalized * stepDistance;

            RaycastHit stepOffsetHit;

            if (Physics.Linecast(stepPointTop, stepPointDown, out stepOffsetHit, ~0))
            {
                Debug.DrawLine(stepPointTop, stepOffsetHit.point, Color.green);
                stepDistance = stepOffsetHit.distance + 0.1f;
            }

            //Ray ray = new Ray(transform.position + transform.up * stepHeight + dir.normalized * stepDistance, transform.TransformDirection(Vector3.down));
            Ray ray = new Ray(transform.position + transform.up * stepHeight + dir.normalized * stepDistance, transform.TransformDirection(Vector3.down));

            bool sphere = Physics.SphereCast(ray, capsule.radius * 0.5f, out stepOffsetHit, (stepOffsetMaxHeight - stepOffsetMinHeight), ~0);
            Debug.DrawRay(ray.origin, ray.direction, Color.yellow);

            //Vector3 localHitPoint = raycastOrigin.InverseTransformPoint(stepOffsetHit.point);
            //Vector3 localPos = raycastOrigin.InverseTransformPoint(raycastOrigin.position);

            stepHitPoint = transform.InverseTransformPoint(stepOffsetHit.point);
            stepPos = transform.InverseTransformPoint(transform.position);
            //steplocalHitPoint = stepOffsetHit.point;
            //steplocalPos = raycastOrigin.position;
            //steplocalPos = raycastOrigin.position;
            stepDir = dir;

            //if (sphere)
            //    print("Sphere has succeeded");

            bool yCheck = stepHitPoint.y > stepPos.y; //stepOffsetHit.point.y > transform.position.y;

            if (sphere && yCheck)
            {
                //dir = (stepOffsetHit.point) - transform.position;
                dir = stepOffsetHit.point - transform.position;
                dir.Normalize();
                stepDir = dir;

                moveVector = Vector3.Project(moveVector, dir);
                applyStepOffset = true;
                //targetVelocity = Vector3.Project(targetVelocity, dir);
                //applyingStepOffset = true;
                //useVerticalVelocity = false;
                return;
            }
        }

        applyStepOffset = false;
    }

    /// <summary>
    /// Actual movement code to be called when movement happens
    /// </summary>
    public void FixedUpdateMove()
    {
        if (moveInput && !applyStepOffset)
        //if (moveInput)
        {
            //StopMove();
            Physics.SyncTransforms();

            rb.position += moveVector * Time.deltaTime;

            //if (useStopMove)
            //{
            //    //if (Physics.Raycast(new Ray(transform.position, transform.TransformDirection(Vector3.forward)), out RaycastHit hit, stopMoveDistance))
            //    //Debug.DrawLine(wallRayPoint.position, transform.position + moveVector.normalized * stopMoveDistance, Color.red);
            //    bool middle = Physics.Raycast(new Ray(transform.position, moveVector.normalized), out RaycastHit hit, stopMoveDistance);
            //    bool top = Physics.Raycast(new Ray(wallRayPoint.position, moveVector.normalized), out RaycastHit hit2, stopMoveDistance);
            //
            //    if (middle && top)
            //    {
            //        Vector3 dir = (transform.position - hit.point).normalized;
            //
            //        float angle = Vector3.SignedAngle(hit.normal, dir, transform.TransformDirection(Vector3.left));
            //        wallDebug = angle;
            //
            //        if (angle > 0.0f)
            //        {
            //            rb.position += moveVector * Time.deltaTime;
            //        }
            //
            //        else
            //            print("Failed Wall Check");
            //    }
            //
            //    else
            //    {
            //        rb.position += moveVector * Time.deltaTime;
            //    }
            //
            //    //if (!rb.SweepTest(moveVector, out RaycastHit hit, stopMoveDistance))
            //    //{
            //    //    rb.position += moveVector * Time.deltaTime;
            //    //}
            //}
            //
            //else
            //{
            //    rb.position += moveVector * Time.deltaTime;
            //}            
        }
    }

    public void StopMove()
    {
        if (!moveInput)
        {
            return;
        }

        RaycastHit hitinfo;
        Ray ray = new Ray(transform.position + Vector3.up * stopMoveHeight, moveVector.normalized);
        var hitAngle = 0f;

        //if (debugWindow)
        //{
        //    Debug.DrawRay(ray.origin, ray.direction * stopMoveDistance, Color.red);
        //}

        if (Physics.Raycast(ray, out hitinfo, capsule.radius + stopMoveDistance, stopMoveLayer, QueryTriggerInteraction.Ignore))
        {
            //stopMove = true;
            moveVector = Vector3.zero;
            Debug.Log("Stopping Move");
            return;
        }

        if (Physics.Linecast(transform.position + Vector3.up * (capsule.height * 0.5f), transform.position + moveVector.normalized * (capsule.radius + 0.2f), out hitinfo, groundLayer))
        {
            hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);
            //if (debugWindow)
            //{
            //    Debug.DrawLine(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), transform.position + moveDirection.normalized * (_capsuleCollider.radius + 0.2f), (hitAngle > slopeLimit) ? Color.yellow : Color.blue, 0.01f);
            //}

            var targetPoint = hitinfo.point + moveVector.normalized * capsule.radius;
            if ((hitAngle > slopeLimit) && Physics.Linecast(transform.position + Vector3.up * (capsule.height * 0.5f), targetPoint, out hitinfo, groundLayer))
            {
                //if (debugWindow)
                //{
                //    Debug.DrawRay(hitinfo.point, hitinfo.normal);
                //}

                hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                //if (hitAngle > slopeLimit && hitAngle < 85f)
                if (hitAngle > slopeLimit && hitAngle < maxSlope)
                {
                    //if (debugWindow)
                    //{
                    //    Debug.DrawLine(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), hitinfo.point, Color.red, 0.01f);
                    //}

                    //stopMove = true;
                    moveVector = Vector3.zero;
                    Debug.Log("Stopping Move");
                    return;
                }
                else
                {
                    //if (debugWindow)
                    //{
                    //    Debug.DrawLine(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), hitinfo.point, Color.green, 0.01f);
                    //}
                }
            }
        }
        //else if (debugWindow)
        //{
        //    Debug.DrawLine(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), transform.position + moveDirection.normalized * (_capsuleCollider.radius * 0.2f), Color.blue, 0.01f);
        //}

        //stopMove = false;
    }

    /// <summary>
    /// This serves as the entry point function for the jumping
    /// </summary>
    public void Jump()
    {
        if (jumpCount > 0)
        {
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                print("Starting jump");
                StartJump();
            }
        }
    }

    /// <summary>
    /// Set up the initial state for the jump
    /// </summary>
    void StartJump()
    {
        origJumpPos = rb.position;
        jumpTarget = transform.TransformPoint(0.0f, maxJumpHeight, 0.0f);
        RaycastHit hit;

        //If the player is hitting something, reset the jump target
        if (rb.SweepTest((jumpTarget - rb.position), out hit, maxJumpHeight))
        {
            if (hit.distance > minJumpHeight && hit.distance < maxJumpHeight)
            {
                jumpTarget = hit.point;
            }

            if (hit.distance <= minJumpHeight)
            {                
                return;
            }
        }

        //decelerationCurve.keys[decelerationCurve.keys.Length - 1].time = maxJumpHeight;

        if (!isGrounded && jumpCount < maxJumpCount && jumpCount > 0)
            ZeroVerticalVelocity();

        rb.AddForce(Vector3.up * jumpForce, jumpMode);

        jumpCount--;
        isGrounded = false;
        groundCheckEnabled = false;
        distFromJumpStart = 0.0f;
        jumpTimer = 0.0f;
        isJumping = true;

        //if (useDecelerationCurve)
        //{
        //    StartCoroutine(JumpDeceleration());
        //}
    }

    /// <summary>
    /// The Jump Checks to run in update
    /// </summary>
    public void UpdateJump()
    {
        if (Input.GetButton("Jump") && isJumping)
        {
            distFromJumpStart = Vector3.Distance(origJumpPos, rb.position);
            jumpTimer += Time.deltaTime;
            //if (useDecelerationCurve)
            //{
            //    rb.AddForce(Vector3.down * decelerationCurve.Evaluate(distFromJumpStart), decelerationMode);                
            //}

            //if (useBaseDownForce)
            //{
            //    rb.AddForce(Vector3.down * baseDownForce, downForceMode);
            //}

            //if (distFromJumpStart >= maxJumpHeight)
            if (jumpTimer >= maxJumpTime || distFromJumpStart >= maxJumpHeight)
            {
                //print("Reached Max Jump Height");
                print("Reached Max Jump Time");

                if (useBaseDownForce)
                {
                    rb.AddForce(Vector3.down * baseDownForce, downForceMode);
                }

                groundCheckEnabled = true;
                isJumping = false;
                return;
            }

            verticalVelocity = 0.0f;
        }

        //A general safety just in case
        if (!Input.GetButton("Jump") && isJumping)
        {
            //if (useEarlyDecelerationCurve && !isGrounded)
            //{
            //    if (distFromJumpStart < maxJumpHeight)
            //        StartCoroutine(EarlyJumpCancel());
            //}

            if (useEarlyCancelDownForce)
            {
                rb.AddForce(Vector3.down * earlyCancelDownForce, earlyCancelMode);
            }

            if (useCancelCoroutine)
            {
                StartCoroutine(JumpCancel());
            }

            print("Reached Ground Check Reset");
            groundCheckEnabled = true;
            isJumping = false;
        }
    }

    IEnumerator JumpCancel()
    {
        while (!isGrounded)
        {
            rb.AddForce(Vector3.down * cancelCoroutineForce, coroutineMode);
            yield return null;
        }

        print("End Jump Cancel");
    }

    IEnumerator JumpDeceleration()
    {
        float timer = 0.0f;
        decelerationCurve.keys[decelerationCurve.keys.Length - 1].time = maxJumpTime;
        decelerationCurve.keys[decelerationCurve.keys.Length - 1].value = maxDeceleration;
        
        while (true)
        {
            if (isGrounded)
                yield break;

            if (isJumping)
                rb.AddForce(Vector3.down * baseDownForce * decelerationCurve.Evaluate(timer), decelerationMode);
            else                            
                rb.AddForce(Vector3.down * baseDownForce * decelerationCurve.Evaluate(timer) * earlyCancelScale, decelerationMode);
            

            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator EarlyJumpCancel()
    {
        float timer = 0.0f;
        earlyDecelerationCurve.keys[earlyDecelerationCurve.keys.Length - 1].time = maxJumpHeight;
        float maxTime = earlyDecelerationCurve.keys[earlyDecelerationCurve.keys.Length - 1].time;
        
        //Debug.Log("Start Early Jump Cancel");

        while (timer <= maxJumpTime)
        {
            if (isGrounded)
            {
                //Debug.Log("End Early Jump Cancel");
                yield break;
            }

            rb.AddForce(Vector3.down * earlyDecelerationCurve.Evaluate(timer), earlyDecelerationMode);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// The Jump updates to run in fixed update to respect physics
    /// </summary>
    public void FixedUpdateJump()
    {
        if (isJumping)
        {
            //Stop the rigidbody physics from interfering with the jump
            if (jumpZeroVelocity)
                ZeroVerticalVelocity();

            RaycastHit hit;

            //If you hit something on the way up, stop jumping
            if (rb.SweepTest(transform.TransformDirection(Vector3.up), out hit, jumpSweepDist))
            {                
                isJumping = false;
                return;
            }

            if (stopAtTarget)
            {
                if (Vector3.Distance(rb.position, jumpTarget) < 0.1f)
                {
                    print("jump target reached");
                    isJumping = false;
                    return;
                }
            }

            //Vector3 delta = Vector3.MoveTowards(rb.position, jumpTarget, jumpLerpVal);
            //delta = (delta - rb.position);
            //
            ////Account for custom gravity vectors 
            //Vector3 up = transform.TransformDirection(Vector3.up);
            //Vector3 proj = Vector3.Project(delta, up);
            //
            //rb.position += proj * jumpSpeed * Time.deltaTime;
        }
    }

    public void Climb(Vector3 climbPoint)
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 fwd = camera.transform.TransformDirection(Vector3.forward);
        Vector3 diff = (climbPoint - camera.transform.position);
        Vector3 dir = new Vector3(fwd.x, 0.0f, fwd.z);
        float angle = Vector3.Angle(diff, fwd);

        if (angle < climbingAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed);
            transform.Translate(x * climbSpeed * Time.deltaTime, y * climbSpeed * Time.deltaTime, x * climbSpeed * Time.deltaTime);
        }

        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed);
            transform.Translate(x * climbSpeed * Time.deltaTime, 0.0f, y * climbSpeed * Time.deltaTime);
        }
    }

}

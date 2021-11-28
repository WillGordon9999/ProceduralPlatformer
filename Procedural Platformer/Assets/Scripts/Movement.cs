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

    [Header("Jumping")]
    public float jumpSpeed = 10.0f;
    public float jumpLerpVal = 10.0f;
    public float minJumpHeight = 0.1f;
    public float maxJumpHeight = 3.0f;
    public float jumpSweepDist = 0.1f;
    public bool jumpZeroVelocity = true; //Zero out the Vertical Velocity for the duration of the jump
    public bool stopAtTarget = true;
    public int maxJumpCount = 1;

    public float climbSpeed = 20.0f;
    public float climbingAngle = 3.0f;

    [Header("Ground Check")]
    public Vector3 rayOrigin = Vector3.zero; //The position in player local space to fire the raycast from
    public float rayDistance = 1.5f;
    public LayerMask groundLayerMask = ~0; //By default everything

    [SerializeField] bool isGrounded = false;
    [SerializeField] bool groundCheckEnabled = true;
    Vector3 rayPosLocal;
    Vector3 rayFinalPos;
    RaycastHit groundHit;

    new GameObject camera;
    Rigidbody rb;
    Vector3 moveVector;
    Vector3 verticalVelocityVec;
    float verticalVelocity;
    bool moveInput = false;

    //Jump variables
    int jumpCount;
    Vector3 origJumpPos;
    Vector3 jumpTarget;
    bool isJumping = false;
    float distFromJumpStart;

    // Start is called before the first frame update
    void Start()
    {        
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        rb = GetComponent<Rigidbody>();
        jumpCount = maxJumpCount;
        
    }

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
            Vector3 dir = camera.GetComponent<CameraOrbit>().zeroAnchor.transform.TransformDirection(new Vector3(x, 0.0f, y));
            if (dir.magnitude > 1) dir.Normalize();
            Quaternion rot = Quaternion.LookRotation(dir, transform.TransformDirection(Vector3.up));

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed);
            moveVector = dir;

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

    /// <summary>
    /// Actual movement code to be called when movement happens
    /// </summary>
    public void FixedUpdateMove()
    {
        if (moveInput)
        {
            rb.position += moveVector * Time.deltaTime;
        }
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

        jumpCount--;
        isGrounded = false;
        groundCheckEnabled = false;
        distFromJumpStart = 0.0f;
        isJumping = true;
    }

    /// <summary>
    /// The Jump Checks to run in update
    /// </summary>
    public void UpdateJump()
    {
        if (Input.GetButton("Jump") && isJumping)
        {
            distFromJumpStart = Vector3.Distance(origJumpPos, rb.position);

            if (distFromJumpStart >= maxJumpHeight)
            {
                groundCheckEnabled = true;
                isJumping = false;
                return;
            }

            verticalVelocity = 0.0f;
        }

        //A general safety just in case
        else
        {
            groundCheckEnabled = true;
            isJumping = false;
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

            Vector3 delta = Vector3.MoveTowards(rb.position, jumpTarget, jumpLerpVal);
            delta = (delta - rb.position);

            //Account for custom gravity vectors 
            Vector3 up = transform.TransformDirection(Vector3.up);
            Vector3 proj = Vector3.Project(delta, up);

            rb.position += proj * jumpSpeed * Time.deltaTime;
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

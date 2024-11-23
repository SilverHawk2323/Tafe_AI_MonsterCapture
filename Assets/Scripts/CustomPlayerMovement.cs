using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomPlayerMovement : MonoBehaviour
{
    public enum State
    {
        Walk,
        Rise,
        Fall,
    }

    //The Tooltip attribute displays text in the Unity inspector when hovering the mouse over this variable
    [Tooltip("To track the current behaviour of the player")]
    //The SerializeField attribute makes a private variable visible and editable in the Unity inspector.
    [SerializeField] private State currentState;

    [Tooltip("How many units per sec the player should move by default")]
    [SerializeField] private float speedWalk;

    [Tooltip("How much upward momentum to start with when jumping")]
    [SerializeField] private float jumpPower;

    [Tooltip("Reduce the player's vertical momentum by this many units per second")]
    [SerializeField] private float gravity;

    [Tooltip("What physics layer should the player object recognise as the ground")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("The maximum number of jumps allowed before the player touches the ground again")]
    [SerializeField] private int jumpsAllowed = 2;

    [SerializeField] private Transform mc;

    private CapsuleCollider capsuleCollider;

    //The number of jumps the player currently has available
    private int jumpsRemaining;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentState = State.Walk;
        rb.useGravity = false;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Walk:
                WalkState();
                break;
            case State.Rise:
                RiseState();
                break;
            case State.Fall:
                FallState();
                break;
            default:
                break;
        }
    }

    private void WalkState()
    {
        speedWalk = 5f;

        jumpsRemaining = jumpsAllowed;

        Vector3 inputMovement = GetMovementFromInput();

        inputMovement *= speedWalk;

        inputMovement.y = Mathf.Clamp(rb.velocity.y - gravity * Time.deltaTime, 0f, float.PositiveInfinity);

        rb.velocity = inputMovement;

        if (!IsGrounded())
        {
            currentState = State.Fall;

            jumpsRemaining -= 1;

            return;
        }

        TryToJump();
    }

    private void RiseState()
    {
        Vector3 inputMovement = GetMovementFromInput();
        inputMovement *= speedWalk;

        //Apply gravity
        inputMovement.y = rb.velocity.y - gravity * Time.deltaTime;

        //Apply the determined movement to our rigidbody
        rb.velocity = inputMovement;

        if(rb.velocity.y < 0f)
        {
            currentState = State.Fall;
        }

        TryToJump();
    }

    private void FallState()
    {
        Vector3 inputMovement = GetMovementFromInput();
        inputMovement *= speedWalk;
        inputMovement.y = rb.velocity.y - gravity* Time.deltaTime;

        rb.velocity = inputMovement;

        if (IsGrounded())
        {
            currentState = State.Walk;
        }
        TryToJump();
    }

    private void TryToJump()
    {
        //If the player presses jump...
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            //... add upwards momentum to the player and change to the Rise state 
            RiseAtSpeed(jumpPower);

            //reduce our remaining jumps
            jumpsRemaining -= 1;
        }
    }

    private void RiseAtSpeed(float speed)
    {
        //Set our vertical momentum upward using the provided speed 
        rb.velocity = new Vector3(rb.velocity.x, speed, rb.velocity.z);

        //Change to the Rise state
        currentState = State.Rise;
    }

    /// <summary>
    /// Get current inputs and translate to a movement direction
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMovementFromInput()
    {
        //Get a local Vector2,                              | local - the variable cannot be called on by other methods or scripts, and the value is lost at the end of the function
        //constructing a new one                            | "new Vector2()" is a constructor. We can give the constructor values to set starting x and y values, e.g. "new Vector2(0.2f, -1f)"
        //using our horizontal and vertical input axes      | "Input.GetAxis()" looks for an axis in the Input Manager with the name provided
        Vector2 inputThisFrame = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //Get a local Vector3, constructing a new one using the inputs.
        //Since we're moving in 3D space, we need to convert the "Up/Down" input (the y axis), to a "Forward/Back" input (the Z axis)
        Vector3 moveDirection = new Vector3(inputThisFrame.x, 0, inputThisFrame.y);

        //Get the transform of the currently active camera
        Transform cameraTransform = mc;

        //translate the movement direction based on the camera's transform
        moveDirection = cameraTransform.TransformDirection(moveDirection);

        //return that result
        return moveDirection;
    }

    private bool IsGrounded()
    {
        //Raycast downwards from our centre, using half of our collider's height
        return Physics.Raycast(transform.position, Vector3.down, capsuleCollider.height / 2f + 0.01f, groundLayer);
    }
}

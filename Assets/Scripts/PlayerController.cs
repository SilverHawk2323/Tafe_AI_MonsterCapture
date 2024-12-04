using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float maxSpeed = 10f;
    public float gravity;
    public float jumpPower;
    private bool hasCoyoted;
    private float jumpInputTime = float.NegativeInfinity;
    private float lastGroundedTime = float.NegativeInfinity;

    private bool isGrounded;
    public float airControlMultiplier = 1.6f;
    Vector3 dampVelocity;
    Vector2 airDampVelocity;
    public Rigidbody rb;
    public LayerMask groundMask;
    public GameObject pauseMenu;

    [SerializeField] Camera mc;

    // Start is called before the first frame update
    void Start()
    {
        if(mc == null)
        {
            // "?" is a ternary operator and means basically if the variable on the left is true then use the first thing after the "?" if not use the code after the ":"
            //     condition ? consequent : alternative
            mc = Camera.main ? Camera.main : FindObjectOfType<Camera>();
        }

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale > 0f)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        if (Time.timeScale < 1f)
        {
            return;
        }
        Jump();
        //Movement();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 inputTransformed = mc.transform.TransformDirection(input);
        //to stop the camera direction from affecting the speed
        inputTransformed.y = 0f;

        input = inputTransformed.normalized * input.magnitude;
        if (input.magnitude > 1)
        {
            input.Normalize();
        }
        input *= speed * Time.deltaTime;

        if (isGrounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(input.x, rb.velocity.y, input.z), ref dampVelocity, 0.1f);

            airDampVelocity = Vector2.zero;
        }
        else
        {
            
            dampVelocity = Vector3.zero;
            rb.AddForce(new Vector3(input.x, 0f, input.z) * airControlMultiplier, ForceMode.Acceleration);
            Vector2 xzMovement = new Vector2(rb.velocity.x, rb.velocity.z);
            if (rb.velocity.magnitude > maxSpeed)
            {
                xzMovement = Vector2.SmoothDamp(xzMovement, xzMovement.normalized * maxSpeed,
                                                ref airDampVelocity, 0.1f);

                rb.velocity = new Vector3(xzMovement.x, rb.velocity.y, xzMovement.y);
            }
        }
        
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputTime = Time.time;
        }

        if ((isGrounded || (!hasCoyoted && (Time.time - lastGroundedTime) < 0.5f)) && (Time.time - jumpInputTime) < 0.5f)
        {
            hasCoyoted = true;
            jumpInputTime = float.NegativeInfinity;
            lastGroundedTime = float.NegativeInfinity;
            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasCoyoted = true;
    }

    private void OnCollisionStay(Collision collision)
    {

        int goLayer = 1 << collision.gameObject.layer;

        if ((groundMask & goLayer) != 0)
        {
            isGrounded = true;
            lastGroundedTime = Time.time;
        }
        else
        {
            isGrounded = false;
        }

        //Debug.Log(collision.gameObject.layer);
    }

    private void OnCollisionExit(Collision collision)
    {
        int goLayer = 1 << collision.gameObject.layer;

        if ((groundMask & goLayer) != 0)
        {
            isGrounded = false;
        }
    }
}
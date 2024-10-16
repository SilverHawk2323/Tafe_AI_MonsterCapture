using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public Vector2 input;
    private bool hasCoyoted;
    private float jumpInputTime = float.NegativeInfinity;
    private float lastGroundedTime = float.NegativeInfinity;

    private bool isGrounded;


    public Rigidbody rb;
    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputTime = Time.time;
        }

        if ((isGrounded || (!hasCoyoted && (Time.time - lastGroundedTime)  < 0.5f)) && (Time.time - jumpInputTime) < 0.5f)
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

        if((groundMask & goLayer) != 0)
        {
            Debug.Log("Grounded");

            isGrounded = true;
            lastGroundedTime = Time.time;
        }
        else
        {
            isGrounded = false;
        }

        Debug.Log(collision.gameObject.layer);
    }

    private void OnCollisionExit(Collision collision)
    {
        int goLayer = 1 << collision.gameObject.layer;

        if ((groundMask & goLayer) != 0)
        {
            Debug.Log("Grounded");

            isGrounded = false;
        }
    }
}
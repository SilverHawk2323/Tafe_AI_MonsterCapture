using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Investigating,
        Chasing,
        Attack,
        Captured,
    }

    public State state;

    PlayerController player;

    Rigidbody rb;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        NextState();
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Patrol:
                StartCoroutine(PatrolState());
                break;
            case State.Investigating:
                StartCoroutine (InvestigatingState());
                break;
            case State.Chasing: 
                StartCoroutine(ChasingState());
                break;

            case State.Attack: 
                StartCoroutine(AttackState());
                break;

            case State.Captured:
                StartCoroutine(CapturedState());
                break;

            default:
                Debug.LogWarning("State Doesn't exist");
                break;
        }
    }

    IEnumerator PatrolState()
    {
        //Setup/entry point / Start()/Awake()
        Debug.Log("Entering Patrol State");


        while(state == State.Patrol) // "Update loop"
        {
            
            
            //Direction from A to B
            //B - A, B being the player position and A being the AI.
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.Normalize();
            //Dot Product parameters should be "normalized" 
            float result = Vector3.Dot(transform.forward, directionToPlayer);
            float angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);
            bool direction = angle > 0f;
            transform.rotation *= Quaternion.Euler(0f, (direction ? 50f : -50f) * Time.deltaTime, 0f);
            //Debug.Log(result);
            if (result >= 0.9f)
            {
                state = State.Chasing;
            }



            yield return null; // Wait for a frame
        }


        //tear down/ exit point / OnDestroy()
        Debug.Log("Exiting Patrol State");
        NextState();
    }

    IEnumerator InvestigatingState()
    {
        //Setup/entry point / Start()/Awake()
        Debug.Log("Entering Investigating State");
        float starTime = Time.time;
        float deltaSum = 0f;


        while (state == State.Investigating) // "Update loop"
        {
            deltaSum += Time.deltaTime;
            yield return null; // Wait for a frame
        }

        float endTime = Time.time - starTime;
        Debug.Log("DeltaSum = " + deltaSum + " | End Time = " + endTime);

        //tear down/ exit point / OnDestroy()
        Debug.Log("Exiting Investigating State");
        NextState();
    }

    IEnumerator ChasingState()
    {
        //Setup/entry point / Start()/Awake()
        Debug.Log("Entering Chase State");


        while (state == State.Chasing) // "Update loop"
        {
            float wave = Mathf.Sin(Time.time * 15f) * 0.1f + 1f;
            float wave2 = Mathf.Cos(Time.time * 15f) * 0.1f + 1f;

            transform.localScale = new Vector3(wave, wave2, wave);

            float shimmy = Mathf.Cos(Time.time * 30f) * 10f + 30f;
            //Choose transform movement or rigidbody movement
            //transform.position += transform.right * shimmy * Time.deltaTime;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            //directionToPlayer.Normalize();

            float angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

            if (angle > 0)
            {
                transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);
            }
            else
            {
                transform.rotation *= Quaternion.Euler(0f, -50f  * Time.deltaTime, 0f);
            }

            if(rb.velocity.magnitude < 5f)
            {
                rb.AddForce(transform.forward * shimmy, ForceMode.Acceleration);
            }

            if(directionToPlayer.magnitude < 2f)
            {
                state = State.Attack;
            }
            else if (directionToPlayer.magnitude > 10f)
            {
                state = State.Patrol;
            }

            yield return new WaitForFixedUpdate(); // Wait for the next fixed update
        }


        //tear down/ exit point / OnDestroy()
        Debug.Log("Exiting Chase State");
        NextState();
    }

    IEnumerator AttackState()
    {
        //Setup/entry point / Start()/Awake()
        Debug.Log("Entering Attack State");


        while (state == State.Attack) // "Update loop"
        {
            Vector3 scale = transform.localScale;
            scale.z = Mathf.Cos(Time.time * 20f) * 0.1f + 2f;
            transform.localScale = scale;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            if(directionToPlayer.magnitude > 3f)
            {
                state = State.Chasing;
            }
            yield return null; // Wait for a frame
        }


        //tear down/ exit point / OnDestroy()
        Debug.Log("Exiting Attack State");
        NextState();
    }

    IEnumerator CapturedState()
    {
        //Setup/entry point / Start()/Awake()
        Debug.Log("Entering Captured State");


        while (state == State.Captured) // "Update loop"
        {
            yield return null; // Wait for a frame
        }


        //tear down/ exit point / OnDestroy()
        Debug.Log("Exiting Captured State");
        NextState();
    }
}
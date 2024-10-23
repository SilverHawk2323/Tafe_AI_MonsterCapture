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

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
                
                break;

            case State.Captured:
                
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

            float shimmy = Mathf.Cos(Time.time * 30f) * 10f + 10f;
            //Choose transform movement or rigidbody movement
            //transform.position += transform.right * shimmy * Time.deltaTime;
            rb.AddForce(Vector3.right * shimmy);

            yield return null; // Wait for a frame
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
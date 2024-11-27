using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
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
    protected NavMeshAgent _Agent;
    protected float _Range = 500f;
    public State state;

    protected CustomPlayerMovement player;

    protected Rigidbody rb;
    protected void Awake()
    {
        _Agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<CustomPlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        NextState();
    }

    protected virtual void NextState()
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

    protected virtual IEnumerator PatrolState()
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
            //if ai is still moving
            /*if (_Agent.pathPending || !_Agent.isOnNavMesh || _Agent.remainingDistance > 0.1f)
            {
                yield return null;
            }

            //Choose a random point
            Vector3 randomPosition = _Range * Random.insideUnitCircle;
            randomPosition = new Vector3(randomPosition.x, 0, randomPosition.z);
            _Agent.destination = transform.position + randomPosition;*/


            yield return null; // Wait for a frame
        }


        //tear down/ exit point / OnDestroy()
        Debug.Log("Exiting Patrol State");
        NextState();
    }

    protected virtual IEnumerator InvestigatingState()
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

    protected virtual IEnumerator ChasingState()
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
                _Agent.destination = player.transform.forward;
                transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);
            }
            else
            {
                _Agent.destination = player.transform.forward;
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

    protected virtual IEnumerator AttackState()
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

    protected virtual IEnumerator CapturedState()
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

    protected float[] EQS()
    {
        /* I need a list to add each point to it 
         * This list just needs the Vector3 from the hit
         * 
         * I need to cast a line trace from the player
         * This trace will create points along the path 
         * These points will have a value assigned to them, the higher the value the further it is from the player
         * the points closet to the AI will have a higher value
         * To determine the value I'll subtract the value if it's too far away from the AI
         * 
         */
        
        List<Vector3> points = new List<Vector3>();
        List<float> value = new List<float>();
        RaycastHit hit;
        float range = 100f;
        for(int i  = 0; i < 4; i++)
        {
            Physics.Raycast(player.transform.position, transform.forward, out hit, range);
            if(hit.collider != null)
            {
                points.Add(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + range));
                range -= 25f;
            }
            else
            {
                points.Add(hit.point);
                range -= 25f;
            }
            
        }
        foreach (var point in points)
        {
            value.Add(EQSValue(point));
        }


        Debug.Log(value.Capacity);
        return value.ToArray();
    }

    protected float EQSValue(Vector3 point)
    {
        return point.magnitude - _Agent.transform.position.magnitude;
    }
}
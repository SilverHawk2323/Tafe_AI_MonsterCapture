using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.VisualScripting;

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
    protected float _Range = 100f;
    public State state;

    [SerializeField] private TMP_Text aggressiveText;

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
        GetState();
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
        //Debug.Log("Entering Patrol State");


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
            //transform.rotation *= Quaternion.Euler(0f, (direction ? 50f : -50f) * Time.deltaTime, 0f);
            //Debug.Log(result);
            if (result >= 0.9f)
            {
                state = State.Chasing;
            }
            //if ai is still moving
            if (_Agent.pathPending || !_Agent.isOnNavMesh || _Agent.remainingDistance > 0.1f)
            {
                yield return null;
            }

            //Choose a random point
            Vector3 randomPosition = _Range * Random.insideUnitSphere;
            randomPosition = new Vector3(randomPosition.x, 0, randomPosition.z);
            _Agent.destination = transform.position + randomPosition;


            yield return null; // Wait for a frame
        }


        //tear down/ exit point / OnDestroy()
        //Debug.Log("Exiting Patrol State");
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
        //Debug.Log("Entering Chase State");
        _Agent.destination = player.transform.position;

        while (state == State.Chasing) // "Update loop"
        {
            float wave = Mathf.Sin(Time.time * 15f) * 0.1f + 1f;
            float wave2 = Mathf.Cos(Time.time * 15f) * 0.1f + 1f;

            //transform.localScale = new Vector3(wave, wave2, wave);

            //float shimmy = Mathf.Cos(Time.time * 30f) * 10f + 30f;
            //Choose transform movement or rigidbody movement
            //_Agent.destination = player.transform.forward;
            //transform.position += transform.right * shimmy * Time.deltaTime;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            //directionToPlayer.Normalize();

            /*float angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

            if (angle > 0)
            {
                //_Agent.destination = player.transform.forward;
                //transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);
            }
            else
            {
                
                //transform.rotation *= Quaternion.Euler(0f, -50f  * Time.deltaTime, 0f);
            }

            if(rb.velocity.magnitude > 2f)
            {
                //rb.AddForce(transform.forward * shimmy, ForceMode.Acceleration);
            }*/

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
        //Debug.Log("Exiting Chase State");
        NextState();
    }

    protected virtual IEnumerator AttackState()
    {
        //Setup/entry point / Start()/Awake()
        //Debug.Log("Entering Attack State");


        while (state == State.Attack) // "Update loop"
        {
            Vector3 scale = transform.localScale;
            scale.z = Mathf.Cos(Time.time * 20f) * 0.1f + 2f;
            //transform.localScale = scale;

            Vector3 directionToPlayer = player.transform.position - transform.position;
            if(directionToPlayer.magnitude > 3f)
            {
                state = State.Chasing;
            }
            yield return null; // Wait for a frame
        }

        //tear down/ exit point / OnDestroy()
        //Debug.Log("Exiting Attack State");
        NextState();
    }

    protected virtual IEnumerator CapturedState()
    {
        //Setup/entry point / Start()/Awake()
        //Debug.Log("Entering Captured State");
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        _Agent.destination = transform.position;
        while (state == State.Captured) // "Update loop"
        {
            yield return null; // Wait for a frame
        }


        //tear down/ exit point / OnDestroy()
        //Debug.Log("Exiting Captured State");
        //NextState();
    }

    protected virtual void GetState()
    {
        switch (state)
        {
            case State.Patrol:
                aggressiveText.text = "Agressive AI State: Patrolling";
                break;
            case State.Investigating:
                aggressiveText.text = "Agressive AI State: Investigating";
                break;
            case State.Chasing:
                aggressiveText.text = "Agressive AI State: Chasing";
                break;
            case State.Attack:
                aggressiveText.text = "Agressive AI State: Attacking";
                break;
            case State.Captured:
                aggressiveText.text = "Agressive AI State: Captured";
                break;
            default:
                Debug.LogWarning("NO STATE ACTIVE");
                break;
        }
    }

    public void Captured()
    {
        state = State.Captured;
        _Agent.destination = transform.position;
    }
}
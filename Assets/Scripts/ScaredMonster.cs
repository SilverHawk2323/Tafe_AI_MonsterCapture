using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScaredMonster : StateMachine
{

    [SerializeField] private TMP_Text passiveText;

    protected override IEnumerator ChasingState()
    {
        //Setup/entry point / Start()/Awake()
        //Debug.Log("Entering Scared State");


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
            /*if (_Agent.pathPending || !_Agent.isOnNavMesh || _Agent.remainingDistance > 0.1f)
            {
                yield return null;
            }*/

            Vector3 awayFromPlayer = -directionToPlayer;

            //Choose a random point
            Vector3 randomPosition = _Range * Random.insideUnitSphere + awayFromPlayer;
            randomPosition = new Vector3(randomPosition.x, 0, randomPosition.z);

             //float dot = Vector3.Dot(randomPosition, awayFromPlayer);

            _Agent.destination = transform.position + randomPosition;

            if (rb.velocity.magnitude < 5f)
            {
                rb.AddForce(transform.forward * shimmy, ForceMode.Acceleration);
            }

            if (directionToPlayer.magnitude < 2f)
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
        //Debug.Log("Exiting Scared State");
        NextState();
    }

    protected override void GetState()
    {
        switch (state)
        {
            case State.Patrol:
                passiveText.text = "Passive AI State: Patrolling";
                break;
            case State.Investigating:
                passiveText.text = "Passive AI State: Investigating";
                break;
            case State.Chasing:
                passiveText.text = "Passive AI State: Running";
                break;
            case State.Attack:
                passiveText.text = "Passive AI State: Attacking";
                break;
            case State.Captured:
                passiveText.text = "Passive AI State: Captured";
                break;
            default:
                Debug.LogWarning("NO STATE ACTIVE");
                break;
        }
    }
}

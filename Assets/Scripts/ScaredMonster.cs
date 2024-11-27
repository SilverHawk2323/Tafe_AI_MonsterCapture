using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/* Making an EQS
 * I need to have sphere cast from the gameObject's position in the world
 * I need to create a invisible objects created that have a value assigned to it based on conditions
 * I need the AI to go to the one with the highest value
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */

public class ScaredMonster : StateMachine
{
    protected override IEnumerator ChasingState()
    {
        //Setup/entry point / Start()/Awake()
        Debug.Log("Entering Scared State");


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
            if (_Agent.pathPending || !_Agent.isOnNavMesh || _Agent.remainingDistance > 0.1f)
            {
                yield return null;
            }

            Vector3 awayFromPlayer = -directionToPlayer;

            //Choose a random point
            Vector3 randomPosition = _Range * Random.onUnitSphere;
            randomPosition = new Vector3(randomPosition.x, 0, randomPosition.z);

             //float dot = Vector3.Dot(randomPosition, awayFromPlayer);

            _Agent.destination = transform.position + randomPosition;

            /*if (rb.velocity.magnitude < 5f)
            {
                rb.AddForce(transform.forward * shimmy, ForceMode.Acceleration);
            }*/

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
        Debug.Log("Exiting Scared State");
        NextState();
    }
}
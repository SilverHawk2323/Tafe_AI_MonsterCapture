using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RaycastFromScreenCentre : MonoBehaviour
{
    [Tooltip("The physics layers to try to hit with this raycast")]
    [SerializeField]protected LayerMask hitLayer;

    [Tooltip("The maximum distance this raycast can travel")]
    [SerializeField]protected float maxDistance;

    //Hold a reference to our camera selector
    private Camera playerCamera;

    //protected = like private, but child scripts can see it
    //virtual = lets a child script override this function with its own version
    protected virtual void Start()
    {
        playerCamera = FindObjectOfType<Camera>();
    }

    public RaycastHit TryToHit()
    {
        //a struct cannot be "null", se we have initialise an empty struct instead
        RaycastHit hit = new RaycastHit();

        //Use half the camera width and height to determine the screen centre, and cast a ray from there
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(playerCamera.pixelWidth, playerCamera.pixelHeight) * 0.5f);
        if(Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log(hit.collider.gameObject);
            return hit;
        }

        //if we hit nothing, record the furthest point we *could* have hit
        hit.point = ray.origin + ray.direction * maxDistance;

        //then we can return the otherwise empty hit
        Debug.Log("Hit Nothing");
        return hit;
    }
    

}

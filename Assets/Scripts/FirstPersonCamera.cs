using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Tooltip("How fast the camera should move as the player looks around")]
    [SerializeField] private float sensitivity;
    [Tooltip("The minimum allowed rotation around the x-axis")]
    [SerializeField] private float verticalRotationMin;
    [Tooltip("The maximum allowed rotation around the x-axis")]
    [SerializeField] private float verticalRotationMax;

    [Tooltip("The player object's transform")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("How far up from the player's centre should the camera rest")]
    [SerializeField] private float playerEyeLevel = 0.5f;

    //Track our current rotation values
    private float currentHorizontalRotation, currentVerticalRotation;

    //Hold the camera in use by this script
    new private Camera camera;


    /// <summary>
    /// Enable or disable the associated camera by passing true or false.
    /// </summary>
    /// <param name="active"></param>
    public void ToggleActive(bool active)
    {
        camera.enabled = active;
    }

    //Awake runs once when the script first loads in the scene
    private void Awake()
    {
        camera = GetComponent<Camera>();
        currentHorizontalRotation = transform.localEulerAngles.y;
        currentVerticalRotation = transform.localEulerAngles.x;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        GetRotationFromInput();
        ClampVerticalRotation();
        ApplyRotation();

        //snap to the player's position, adjusted upwards using the eye level provided
        transform.position = playerTransform.position + Vector3.up * playerEyeLevel;
    }

    /// <summary>
    /// Use the mouse inputs to adjust the current camera rotation
    /// </summary>
    private void GetRotationFromInput()
    {
        currentHorizontalRotation += Input.GetAxis("Mouse X") * sensitivity;
        currentVerticalRotation -= Input.GetAxis("Mouse Y") * sensitivity;
    }

    /// <summary>
    /// Clamp the x-axis rotation using the minimum and maximum provided
    /// </summary>
    private void ClampVerticalRotation()
    {
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, verticalRotationMin, verticalRotationMax);
    }

    /// <summary>
    /// Set the camera's rotation as determined
    /// </summary>
    private void ApplyRotation()
    {
        transform.localEulerAngles = new Vector3(currentVerticalRotation, currentHorizontalRotation, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // this gives us access to unity event.

public class EventOnInput : MonoBehaviour
{
    //this will tell our script what type of input to listen for
    public enum InputType
    {
        Down,
        Held,
        Released,
    }
    [SerializeField] private InputType type;
    [SerializeField] private KeyCode triggerKey;
    [SerializeField] private UnityEvent onTrigger;


    private void Update()
    {
        if (Time.timeScale < 1f)
        {
            return;
        }
        switch (type)
        {
            case InputType.Down:
                if (Input.GetKeyDown(triggerKey))
                {
                    onTrigger?.Invoke();
                }
                break;
            case InputType.Held:
                if (Input.GetKey(triggerKey))
                {
                    onTrigger?.Invoke();
                }
                break;
            case InputType.Released:
                if (Input.GetKeyUp(triggerKey))
                {
                    onTrigger?.Invoke();
                }
                break;
            default:
                break;
        }
        
    }
}

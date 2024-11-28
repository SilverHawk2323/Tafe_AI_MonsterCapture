using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffect : MonoBehaviour
{
    private LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

    public void Play(Vector3 hitposition)
    {
        StopCoroutine(StopAfterDelay());
        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hitposition);
        StartCoroutine(StopAfterDelay());
    }


    private IEnumerator StopAfterDelay()
    {
        yield return new WaitForSeconds(0.25f);
        line.enabled = false;
    }
}

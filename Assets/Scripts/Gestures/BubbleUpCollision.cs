using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleUpCollision : MonoBehaviour
{
    private Tracing trace;

    private void Start()
    {
        trace = GetComponentInParent<Tracing>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(trace != null)
        {
            Debug.Log("trigger enter");
            trace.TriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(trace != null)
        {
            Debug.Log("trigger exit");
            trace.TriggerExit(other);
        }
    }
}

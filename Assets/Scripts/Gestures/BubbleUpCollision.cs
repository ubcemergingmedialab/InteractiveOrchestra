using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to share events captured by a collider in a child with handlers on the parent
/// </summary>
public class BubbleUpCollision : MonoBehaviour
{
    /// <summary>
    /// The parent that will consume the event
    /// </summary>
    private Tracing trace;

    private void Start()
    {
        trace = GetComponentInParent<Tracing>();
    }

    /// <summary>
    /// This is the trigger enter event we want to capture
    /// </summary>
    /// <param name="other">the collider we are interacting with</param>
    private void OnTriggerEnter(Collider other)
    {
        if(trace != null)
        {
            Debug.Log("trigger enter");
            trace.TriggerEnter(other);
        }
    }

    /// <summary>
    /// this is the trigger exit event we want to capture
    /// </summary>
    /// <param name="other">The collider we are interacting with</param>
    private void OnTriggerExit(Collider other)
    {
        if(trace != null)
        {
            Debug.Log("trigger exit");
            trace.TriggerExit(other);
        }
    }
}

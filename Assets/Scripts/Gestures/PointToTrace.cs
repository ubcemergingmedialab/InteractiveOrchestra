﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// helper script used to get the trace elements to line up with each other
/// </summary>
public class PointToTrace : MonoBehaviour
{
    private Tracing tracing;
    private GestureTrace manager;
    // Start is called before the first frame update
    void Start()
    {
        tracing = GetComponentInParent<Tracing>();
        manager = GetComponentInParent<GestureTrace>();
        Tracing nextTrace = manager.GetNextTraceable(tracing.traceableIndex);
        if(nextTrace != null)
        {
            transform.up = nextTrace.transform.position - transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

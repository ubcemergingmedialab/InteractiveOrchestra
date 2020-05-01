using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script goes on the gesture trace parent, has some helper functions
/// that the individual trace elements need
/// </summary>
public class GestureTrace : MonoBehaviour
{
    public GameObject batonTip;

    public Tracing[] traceables;
    public int traceDifficulty;
    private int traceableIndex;

    private void Start()
    {
        traceableIndex = 0;
    }

    private void Update()
    {
        transform.localScale = transform.localScale;
    }

    private void OnDisable()
    {
        transform.localScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = transform.localScale;
    }

    /// <summary>
    /// Centralized place to get baton
    /// </summary>
    public GameObject GetBatonTip()
    {
        return batonTip;
    }

    /// <summary>
    /// The next traceable by index (last one gives index 0)
    /// </summary>
    public Tracing GetNextTraceable(int index)
    {
        int r = index + 1;
        if(r < 0)
        {
            return null;
        } else if(r >= traceables.Length)
        {
            return traceables[0];
        } else
        {
            return traceables[r];
        }
    }

    /// <summary>
    /// Previous traceable by index (first one and gives null)
    /// </summary>
    public Tracing GetPreviousTraceable(int index)
    {
        int r = index - 1;
        if(r >= traceables.Length)
        {
            return null;
        } else if(r < 0)
        {
            return null;
        } else
        {
            return traceables[r];
        }
    }
}

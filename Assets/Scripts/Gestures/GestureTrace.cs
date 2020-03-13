using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureTrace : MonoBehaviour
{
    public GameObject batonTip;

    public Tracing[] traceables;
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

    public GameObject GetBatonTip()
    {
        return batonTip;
    }

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

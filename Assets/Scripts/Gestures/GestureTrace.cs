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

    public GameObject GetBatonTip()
    {
        return batonTip;
    }

    public Tracing GetNextTraceable()
    {
        traceableIndex = (traceableIndex + 1) % traceables.Length;
        return traceables[traceableIndex];
    }
}

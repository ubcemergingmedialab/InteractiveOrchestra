using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// exposes segment event to then be consumed by trace elements to react appropriately
/// </summary>
public class SegmentController : MonoBehaviour
{

    public delegate void SegmentEventHandler(int segment);
    public event SegmentEventHandler SegmentEvent;

    public void publishCurrentSegment(int segment)
    {
        SegmentEvent(segment);
    }
}

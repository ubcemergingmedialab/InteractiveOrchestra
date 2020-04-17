using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentController : MonoBehaviour
{

    public delegate void SegmentEventHandler(int segment);
    public event SegmentEventHandler SegmentEvent;

    public void publishCurrentSegment(int segment)
    {
        SegmentEvent(segment);
    }
}

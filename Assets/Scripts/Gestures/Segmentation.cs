using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to segment intersections to publish even indicating segment has changed
/// </summary>
public class Segmentation : MonoBehaviour
{
    public int beatSegment;

    public void publishSegment()
    {
        GetComponentInParent<SegmentController>().publishCurrentSegment(beatSegment);
    }
}

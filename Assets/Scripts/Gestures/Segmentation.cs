using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segmentation : MonoBehaviour
{
    public int beatSegment;

    public void publishSegment()
    {
        GetComponentInParent<SegmentController>().publishCurrentSegment(beatSegment);
    }
}

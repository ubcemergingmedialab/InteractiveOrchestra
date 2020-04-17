using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIndicator : MonoBehaviour
{
    public int beatSegment;
    public GameObject beatIndicator;
    // Start is called before the first frame update
    void Start()
    {
        SegmentController controller = GetComponentInParent<SegmentController>();
        controller.SegmentEvent += (int segment) => {
            if (beatSegment == segment)
            {
                beatIndicator.SetActive(true);
            }
            else
            {
                beatIndicator.SetActive(false);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

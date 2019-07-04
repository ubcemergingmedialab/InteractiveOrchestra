using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceIndicator : MonoBehaviour {

    #region Variables
    private bool visible = false;
    private Renderer performanceIndicatorRenderer;
    private int userTiming;
    #endregion 

    // Use this for initialization
    void Awake () {
        performanceIndicatorRenderer = GetComponent<Renderer>();
        performanceIndicatorRenderer.enabled = visible;
    }

    // Update is called once per frame
    /// <summary>
    /// Checks whether user's gestures are in time with audio BPM, providing user feedback
    /// </summary> 
    /// <param name="timeBetweenBeats"></param>
    /// <param name="allowedTimingError"></param>
    /// <param name="timeSincePrevCollision"></param>
    public void CheckUserTiming (float allowedTimingError, float timeBetweenBeats, float timeSincePrevCollision) { 
        // TODO: play around with this value
        allowedTimingError = timeBetweenBeats * 0.10f;
        // user is on time 
        if (timeBetweenBeats - allowedTimingError <= timeSincePrevCollision && timeSincePrevCollision <= timeBetweenBeats + allowedTimingError)
        {  
            userTiming = 0;
        }
        // user is too fast
        else if (timeSincePrevCollision < timeBetweenBeats - allowedTimingError)
        { 
            userTiming = 1;
        }
        // user is too slow
        else
        { 
            userTiming = -1;
        }
        UpdateColour();
   }

    public void UpdateColour()
    {
        if (userTiming == -1)
        {
            performanceIndicatorRenderer.material.color = Color.blue;
        }
        if (userTiming == 0)
        {
            performanceIndicatorRenderer.material.color = Color.green;
        }
        if (userTiming == 1)
        {
            performanceIndicatorRenderer.material.color = Color.red;
        }
    }
}

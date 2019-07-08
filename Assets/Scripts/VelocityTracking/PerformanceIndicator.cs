using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceIndicator : MonoBehaviour {

    #region Variables
    private bool visible = true;
    private Renderer performanceIndicatorRenderer;
    private int userTiming;
    private float allowedTimingError;
    #endregion

    // Use this for initialization
    void Awake () {
        performanceIndicatorRenderer = GetComponent<Renderer>();
        performanceIndicatorRenderer.enabled = visible;
    }
     
    /// <summary>
    /// Checks whether user's gestures are in time with audio BPM, providing user feedback
    /// </summary> 
    /// <param name="timeBetweenBeats"></param> 
    /// <param name="timeSincePrevCollision"></param>
    public void CheckUserTiming (float timeBetweenBeats, float timeSincePrevCollision) {
        
        Debug.Log("Time between beats: " + timeBetweenBeats);
        // TODO: play around with this value
        allowedTimingError = timeBetweenBeats * 0.10f;
        Debug.Log("Allowed timing error: " + allowedTimingError);
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

    private void UpdateColour()
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

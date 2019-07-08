using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceIndicator : MonoBehaviour {

    #region Variables 
    Renderer performanceIndicatorRend; 
    private float allowedTimingError;
    public Material[] materials;
    #endregion

    // Use this for initialization
    void Start () {
        performanceIndicatorRend = this.GetComponent<Renderer>();
        performanceIndicatorRend.enabled = true; 
    }
     
    /// <summary>
    /// Checks whether user's gestures are in time with audio BPM, providing user feedback by changing its color
    /// </summary> 
    /// <param name="timeBetweenBeats"></param> 
    /// <param name="timeSincePrevCollision"></param>
    public void CheckUserTiming (float timeBetweenBeats, float timeSincePrevCollision)
    { 
        allowedTimingError = timeBetweenBeats * 0.2f;
        Debug.Log("Allowed timing error: " + allowedTimingError);
        if (timeSincePrevCollision > timeBetweenBeats + allowedTimingError)
        {
            performanceIndicatorRend.sharedMaterial = materials[0];
            Debug.Log("User is too slow!");
        }
        else if (timeBetweenBeats - allowedTimingError <= timeSincePrevCollision && 
            timeSincePrevCollision <= timeBetweenBeats + allowedTimingError)
        {
            performanceIndicatorRend.sharedMaterial = materials[1];
            Debug.Log("User is on time!"); 
        }
        else
        {
            performanceIndicatorRend.sharedMaterial = materials[2];
            Debug.Log("User is too fast!");
        } 
    } 
}

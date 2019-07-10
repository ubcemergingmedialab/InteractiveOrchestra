using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceIndicator : MonoBehaviour {

    #region Variables 
    Renderer pIRenderer; 
    private float allowedTimingError;
    [SerializeField] private Material[] materials;
    #endregion

    // Use this for initialization
    void Start () {
        pIRenderer = GetComponent<Renderer>();
        pIRenderer.enabled = true; 
    }
     
    /// <summary>
    /// Checks whether user's gestures are in time with audio BPM, providing user feedback by changing its color
    /// </summary> 
    /// <param name="timeBetweenBeats"></param> 
    /// <param name="timeSincePrevCollision"></param>
    public void CheckUserTiming (float timeBetweenBeats, float timeSincePrevCollision)
    {
        Debug.Log("================");
        allowedTimingError = timeBetweenBeats * 0.35f;
        // Debug.Log("Allowed timing error: " + allowedTimingError);
        if (timeSincePrevCollision > timeBetweenBeats + allowedTimingError)
        {
            pIRenderer.material = materials[0];
            //= materials[0];
            Debug.Log("User is too slow! " + timeSincePrevCollision + " > " + timeBetweenBeats + " + " + allowedTimingError);
        }
        else if (timeBetweenBeats - allowedTimingError <= timeSincePrevCollision && 
            timeSincePrevCollision <= timeBetweenBeats + allowedTimingError)
        {
            pIRenderer.material = materials[1];
            //pIRenderer.material.color = materials[1];
            Debug.Log("User is on time!"); 
        }
        else if (timeSincePrevCollision < timeBetweenBeats - allowedTimingError)
        {
            pIRenderer.material = materials[2];
            //pIRenderer.material = materials[2];
            Debug.Log("User is too fast !" + timeSincePrevCollision + " < " + timeBetweenBeats + " - " + allowedTimingError);
        } 
    } 
}

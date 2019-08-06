﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceIndicator : MonoBehaviour {

    #region Variables 
    SpriteRenderer pIRenderer; 
    private float allowedTimingError;
    private int userBPM;
    private int beatCount;
    [SerializeField] private Material[] materials; // Miss, OK, Perfect
    public Text BPMTextDisplay;
    private OVRVelocityTracker velocityTracker;
    #endregion

    // Use this for initialization
    void Start () {
        pIRenderer = GetComponent<SpriteRenderer>();
        pIRenderer.enabled = true;
        userBPM = 0;
        SetCurrentBPM(0.0f);
    }
     
    /// <summary>
    /// Checks whether user's gestures are in time with audio BPM, providing user feedback by changing its color
    /// </summary> 
    /// <param name="timeBetweenBeats"></param> 
    /// <param name="timeSincePrevCollision"></param>
    public void CheckUserTiming (float timeBetweenBeats, float timeSincePrevCollision)
    {
        Debug.Log("================"); 
        allowedTimingError = timeBetweenBeats * 0.25f; 
        if (timeSincePrevCollision > timeBetweenBeats + allowedTimingError)
        {
            pIRenderer.material = materials[0]; 
            Debug.Log("User is too slow! " + timeSincePrevCollision + " > " + timeBetweenBeats + " + " + allowedTimingError);
        }
        else if (timeBetweenBeats - allowedTimingError <= timeSincePrevCollision && 
            timeSincePrevCollision <= timeBetweenBeats + allowedTimingError)
        {
            pIRenderer.material = materials[1]; 
            Debug.Log("User is on time!"); 
        }
        else 
        {
            pIRenderer.material = materials[2]; 
            Debug.Log("User is too fast! " + timeSincePrevCollision + " < " + timeBetweenBeats + " - " + allowedTimingError);
        }
    }

    /// <summary>
    /// Updates beat count, resets count every 4th beat
    /// </summary>
    public void UpdateBeatCount(float timeSincePrevCollision)
    {
        beatCount++;
        if (beatCount == 5)
        { 
            beatCount = 1;
            SetCurrentBPM(timeSincePrevCollision);
            Debug.Log("beat count: " + beatCount);
        } 
         
    }

    /// <summary>
    /// Updates the userBPM based on timeSincePrevCollision and song's BPM
    /// </summary>
    /// <param name="timeSincePrevCollision"></param>
    private void SetCurrentBPM(float timeSincePrevCollision)
    {
        if (velocityTracker.planeHasBeenSpawned)
        {
            userBPM = (int)(60 / timeSincePrevCollision);
            Debug.Log("Time elapsed since previous collision: " + timeSincePrevCollision + " seconds");
            Debug.Log("User BPM: " + userBPM);
        }
        BPMTextDisplay.text = userBPM.ToString();
    }
}

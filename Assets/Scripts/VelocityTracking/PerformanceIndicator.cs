using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceIndicator : MonoBehaviour {

    #region Variables  
    SpriteRenderer pIRenderer;
    [SerializeField] public Sprite BPM_OK;
    [SerializeField] public Sprite BPM_Miss;
    [SerializeField] public Sprite BPM_Perfect;
    
    private int userBPM;
    private int beatCount;

    
    [SerializeField] private Text BPMTextDisplay;
    [SerializeField] private ParticleSystem BPMGuide;

    private OVRVelocityTracker velocityTracker;

    #endregion

    // Use this for initialization
    void Awake () {
        TempoController.PlayPiece += PlayGuide;
        pIRenderer = GetComponent<SpriteRenderer>();
        pIRenderer.enabled = true;
        BPMTextDisplay.text = "0";  
    }
     
    /// <summary>
    /// At every beat, checks whether user's gestures are in time with audio BPM, 
    /// providing user feedback by changing rendered BPM display
    /// </summary> 
    /// <param name="timeBetweenBeats"></param> 
    /// <param name="timeSincePrevCollision"></param> 
    public void CheckUserTiming (float timeBetweenBeats, float timeSincePrevCollision, float allowedTimingError)
    {
        Debug.Log("================"); 
        // MISS 
        if (timeSincePrevCollision > timeBetweenBeats + allowedTimingError || timeSincePrevCollision < timeBetweenBeats - allowedTimingError)
        { 
            pIRenderer.sprite = BPM_Miss; 
            Debug.Log("User timing is poor!"); 
        }
        // PERFECT timing
        else if (timeSincePrevCollision == timeBetweenBeats)
        { 
            pIRenderer.sprite = BPM_Perfect;
            Debug.Log("User is in perfect time!"); 
        }
        // OK timing
        else
        {
            pIRenderer.sprite = BPM_OK;
            Debug.Log("User timing is ok!");
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
            SetCurrentUserBPM(timeSincePrevCollision);
            Debug.Log("beat count: " + beatCount);
        } 
         
    }

    /// <summary>
    /// Updates and displays the userBPM based on timeSincePrevCollision and song's BPM
    /// </summary>
    /// <param name="timeSincePrevCollision"></param>
    private void SetCurrentUserBPM(float timeSincePrevCollision)
    {
        userBPM = (int)(60 / timeSincePrevCollision);
        Debug.Log("Time elapsed since previous collision: " + timeSincePrevCollision + " seconds");
        Debug.Log("User BPM: " + userBPM);
        BPMTextDisplay.text = userBPM.ToString();
    }

    /// <summary>
    /// Starts particle system that acts as BPM guide for user upon song play
    /// </summary>
    public void PlayGuide(float localBPM)
    {
        BPMGuide.Play();
    }

    /// <summary>
    /// Stops particle system that acts as BPM guide when user lets go of baton
    /// </summary>
    public void StopGuide()
    {
        BPMGuide.Stop();
    }
}

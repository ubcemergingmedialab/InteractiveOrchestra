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

    private int userBPM, targetBPM;
    private int beatCount; 
    private float timeBetweenBeats;
    private float allowedTimingError;  

    [SerializeField] private Text UserBPMTextDisplay, TargetBPMTextDisplay;
    [SerializeField] private ParticleSystem BPMGuide;

    [SerializeField] private OVRVelocityTracker velocityTracker;
    [SerializeField] private TempoController tempoController;
     
    #endregion

    /// Use this for initialization
    void Awake () {
        TempoController.PlayPiece += PlayGuide;
        pIRenderer = GetComponent<SpriteRenderer>();
        pIRenderer.enabled = true;
        UserBPMTextDisplay.text = "0";
        TargetBPMTextDisplay.text = "0";
    }

    /// <summary>
    /// Sets parameters needed to assess accuracy of user's timing, based on the targetBPM
    /// </summary>
    private void SetPerformanceMetrics()
    {
        timeBetweenBeats = (60 / targetBPM);
        allowedTimingError = timeBetweenBeats * 0.25f;
    }
     
    /// <summary>
    /// At every beat, checks whether user's gestures are in time with audio BPM, 
    /// providing user feedback by changing rendered BPM display
    /// </summary> 
    /// <param name="timeBetweenBeats"></param> 
    /// <param name="timeSincePrevCollision"></param> 
    public void CheckUserTiming (float timeSincePrevCollision)
    {
        int BPMDiff = Mathf.Abs((int)(userBPM - targetBPM));
        if (BPMDiff < 3)
        {
            Debug.Log("TargetBPM: " + targetBPM);
            Debug.Log("UserBPM: " + userBPM);
            Debug.Log("Perfect");
            pIRenderer.sprite = BPM_Perfect;
        }
        // PERFECT timing
        else if (BPMDiff < 10)
        {
            Debug.Log("TargetBPM: " + targetBPM);
            Debug.Log("UserBPM: " + userBPM);
            Debug.Log("OK");
            pIRenderer.sprite = BPM_OK;
        }
        // OK timing
        else
        {
            Debug.Log("TargetBPM: " + targetBPM);
            Debug.Log("UserBPM: " + userBPM);
            Debug.Log("Miss");
            pIRenderer.sprite = BPM_Miss;
        }
        /*
        if (timeSincePrevCollision > timeBetweenBeats + allowedTimingError || timeSincePrevCollision < timeBetweenBeats - allowedTimingError)
        { 
            pIRenderer.sprite = BPM_Miss; 
        }
        // PERFECT timing
        else if (timeSincePrevCollision == timeBetweenBeats)
        { 
            pIRenderer.sprite = BPM_Perfect;
        }
        // OK timing
        else
        {
            pIRenderer.sprite = BPM_OK;
        }*/
    }

    /// <summary>
    /// Updates beat count, resets count every 4th beat
    /// </summary>
    public void UpdateBeatCount(float timeSincePrevCollision)
    {
        //beatCount++;
        //if (beatCount == 5)
        //{ 
            beatCount = 1;
            SetUserBPM(timeSincePrevCollision);
            Debug.Log("Time since prev collision: " + timeSincePrevCollision);
        //} 
         
    }

    /// <summary>
    /// Updates and displays the userBPM based on timeSincePrevCollision and song's BPM
    /// </summary>
    /// <param name="timeSincePrevCollision"></param>
    private void SetUserBPM(float timeSincePrevCollision)
    {
        userBPM = (int)(60 / timeSincePrevCollision);
        Debug.Log("Time elapsed since previous collision: " + timeSincePrevCollision + " seconds");
        Debug.Log("User BPM: " + userBPM);
        UserBPMTextDisplay.text = userBPM.ToString();
    }

    /// <summary>
    /// Updates and displays the targetBPM based on local BPM set by BPM Predictor
    /// </summary>
    public void SetTargetBPM()
    {
        targetBPM = (int)tempoController.getLocalBPM();
        TargetBPMTextDisplay.text = targetBPM.ToString();
        SetGuideSpeed();
        SetPerformanceMetrics();
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
        ResetBPMDisplay();
    }

    /// <summary>
    /// Resets BPM text displays to 0
    /// </summary>
    private void ResetBPMDisplay()
    {
        targetBPM = 0;
        userBPM = 0;
        UserBPMTextDisplay.text = userBPM.ToString();
        TargetBPMTextDisplay.text = targetBPM.ToString();
    }

    /// <summary>
    /// Manipulates speed of BPM Guide based on targetBPM
    /// </summary>
    private void SetGuideSpeed()
    {
        var main = BPMGuide.main;
        if (targetBPM == 80)
            main.simulationSpeed = 1.33f;
        if (targetBPM == 100)
            main.simulationSpeed = 1.67f;
        if (targetBPM == 120)
            main.simulationSpeed = 2f;

        Debug.Log("Target BPM: " + targetBPM);
    }
}

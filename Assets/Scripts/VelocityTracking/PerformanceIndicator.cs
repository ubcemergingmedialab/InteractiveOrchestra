using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceIndicator : MonoBehaviour {

    #region Variables  
    SpriteRenderer pIRenderer;
    [SerializeField] private Sprite BPM_OK;
    [SerializeField] private Sprite BPM_Miss;
    [SerializeField] private Sprite BPM_Perfect;

    private int userBPM, targetBPM;
    private int beatCount; 
    private float timeBetweenBeats;
    private float allowedTimingError;
    private float bpmAccumulator = 0;

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
    public void CheckUserTiming ()
    {
        int BPMDiff = Mathf.Abs((int)(userBPM - targetBPM));
        if (BPMDiff < 3)
        {
            pIRenderer.sprite = BPM_Perfect;
        }
        // PERFECT timing
        else if (BPMDiff < 10)
        {
            pIRenderer.sprite = BPM_OK;
        }
        // OK timing
        else
        {
            pIRenderer.sprite = BPM_Miss;
        }
    }

    /// <summary>
    /// Updates and displays the userBPM based on timeSincePrevCollision and song's BPM
    /// </summary>
    /// <param name="timeSincePrevCollision"></param>
    public void SetUserBPM(float timeSincePrevCollision)
    {
        userBPM = (int)(60 / timeSincePrevCollision);
        Debug.Log("timeSincePrevCollision: " + timeSincePrevCollision);
        Debug.Log("userBPM: " + userBPM);
        UserBPMTextDisplay.text = userBPM.ToString();
        tempoController.UpdateOrchestraPiece(userBPM);
    }

    /// <summary>
    /// Updates and displays the targetBPM based on local BPM set by BPM Predictor
    /// </summary>
    public void SetTargetBPM()
    {
        targetBPM = (int)tempoController.GetLocalBPM();
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
        beatCount = 0;
        bpmAccumulator = 0;
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
        beatCount = 0;
        bpmAccumulator = 0;
    }

    /// <summary>
    /// Manipulates speed of BPM Guide based on targetBPM
    /// </summary>
    private void SetGuideSpeed()
    {
        var main = BPMGuide.main;
        float seconds = 60f;
        float speed = targetBPM / seconds;
        main.simulationSpeed = speed;
    }
}

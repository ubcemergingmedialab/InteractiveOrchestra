using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour {


    uint bankID;

    public string rtpcID;

    // Use this for initialization
    protected void LoadBank () {
        AkSoundEngine.LoadBank("Main", AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID);
	}

    private void Start()
    {
        LoadBank();
    }

    public void PlayEvent(string eventName)
    {
        AkSoundEngine.PostEvent(eventName, this.gameObject);
    }

    public void UpdateAudioPlaybackSpeed(float bpm)
    {
        AkSoundEngine.SetRTPCValue(rtpcID, bpm);
    }

    public void StopEvent(string eventname, int fadeout)
    {
        uint eventID;
        eventID = AkSoundEngine.GetIDFromString(eventname);
        AkSoundEngine.ExecuteActionOnEvent(eventID,AkActionOnEventType.AkActionOnEventType_Stop,gameObject, fadeout *1000 ,AkCurveInterpolation.AkCurveInterpolation_Sine);
    }

    public void PauseEvent(string eventname, int fadeout)
    {
        uint eventID;
        eventID = AkSoundEngine.GetIDFromString(eventname);
        AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Pause, gameObject, fadeout * 1000, AkCurveInterpolation.AkCurveInterpolation_Sine);
    }

    public void ResumeEvent(string eventname, int fadeout)
    {
        uint eventID;
       eventID = AkSoundEngine.GetIDFromString(eventname);
       AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Resume, gameObject, fadeout * 1000, AkCurveInterpolation.AkCurveInterpolation_Sine);
    }
}

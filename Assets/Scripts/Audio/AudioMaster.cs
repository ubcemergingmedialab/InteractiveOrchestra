using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script acts as a wrapper for the AKSoundEngine's functionality used in the system.
/// </summary>
public class AudioMaster : MonoBehaviour {

    uint bankID;
    public string rtpcID;

    /// <summary>
    /// Initializes AKSoundEngine.
    /// </summary>
    protected void LoadBank () {
        AkSoundEngine.LoadBank("Main", AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID);
	}

    private void Start()
    {
        LoadBank();
    }

    /// <summary>
    /// Wrapper function to play audio through AK.
    /// <param name="eventName"> Command to trigger specific audio recording in AKSoundEngine. </param>
    /// </summary>
    public void PlayEvent(string eventName)
    {
        AkSoundEngine.PostEvent(eventName, this.gameObject);
    }

    /// <summary>
    /// Updates AK audio playback bpm.
    /// <param name="bpm"> Value to change audio tempo. </param>
    /// </summary>
    public void UpdateAudioPlaybackSpeed(float bpm)
    {
        AkSoundEngine.SetRTPCValue(rtpcID, bpm);
    }

    /// <summary>
    /// Stops AK audio playback.
    /// <param name="eventname"> Command to trigger specific audio recording in AKSoundEngine. </param>
    /// <param name="fadeout"> Determines how fast AKSoundEngine stops audio. </param>
    /// </summary>
    public void StopEvent(string eventname, int fadeout)
    {
        uint eventID;
        eventID = AkSoundEngine.GetIDFromString(eventname);
        AkSoundEngine.ExecuteActionOnEvent(eventID,AkActionOnEventType.AkActionOnEventType_Stop,gameObject, fadeout *1000 ,AkCurveInterpolation.AkCurveInterpolation_Sine);
    }

    /// <summary>
    /// Pauses AK audio playback
    /// <param name="eventname"> Command to trigger specific audio recording in AKSoundEngine. </param>
    /// <param name="fadeout"> Determines how fast AKSoundEngine stops audio. </param>
    /// </summary>
    public void PauseEvent(string eventname, int fadeout)
    {
        uint eventID;
        eventID = AkSoundEngine.GetIDFromString(eventname);
        AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Pause, gameObject, fadeout * 1000, AkCurveInterpolation.AkCurveInterpolation_Sine);
    }

    /// <summary>
    /// Resume paused AK audio playback
    /// <param name="eventname"> Command to trigger specific audio recording in AKSoundEngine. </param>
    /// <param name="fadeout"> Determines how fast AKSoundEngine restarts audio. </param>
    /// </summary>
    public void ResumeEvent(string eventname, int fadeout)
    {
       uint eventID;
       eventID = AkSoundEngine.GetIDFromString(eventname);
       AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Resume, gameObject, fadeout * 1000, AkCurveInterpolation.AkCurveInterpolation_Sine);
    }
}
